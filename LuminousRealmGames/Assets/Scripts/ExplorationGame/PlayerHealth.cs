using UnityEngine;
using TMPro;
using Unity.Cinemachine; // Import TextMeshPro namespace
using UnityEngine.UI;
using static Unity.Cinemachine.CinemachineCamera; // Import UI namespace for the screen effect

public class PlayerHealth : MonoBehaviour
{
    public enum PlayerState { Healed, Injured }
    public PlayerState currentState = PlayerState.Healed;

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health;
    public float injuryRate = 1f;  // How fast the player gets injured over time
    [SerializeField] private float damagePerSecond = 5f;  // Damage taken while injured
    [SerializeField] private float healCooldown = 30f;  // Time to remain healed after touching a checkpoint

    private float healTimer = 0f;  // Timer to track the healed state duration

    // Materials for player states
    [SerializeField] private Material healedMaterial;
    [SerializeField] private Material injuredMaterial;
    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private Animator animator;
    private PlayerAudioManager audioManager;

    [SerializeField] private ParticleSystem bloodParticle1;
    [SerializeField] private ParticleSystem bloodParticle2;

    // TextMeshPro for health display
    [SerializeField] private TextMeshProUGUI healthText;

    // UI Image for screen effect
    [SerializeField] private Image screenEffectImage;

    // Colors for screen effect
    private Color initialColor = new Color(1, 0, 0, 0); // Start with transparent red
    private Color maxColor = new Color(0.5f, 0, 0, 0.8f); // Darker red with higher opacity

    // Text for timer display
    [SerializeField] private TextMeshProUGUI timerText;
    private void Awake()
    {
        audioManager = GetComponent<PlayerAudioManager>();
    }
    void Start()
    {
        
        health = maxHealth;
        UpdatePlayerMaterial(); 
        UpdateHealthUI(); 
        UpdateScreenEffect(); 
        UpdateTimerUI(); 
    }

    public void HandleHealedState()
    {
        healTimer += Time.deltaTime;

        if(audioManager.aura.isPlaying == false)
        {
            audioManager.aura.loop = true;
            audioManager.PowerupContinuous();
        }
        
        UpdateTimerUI();

        if (healTimer >= healCooldown)
        {
            SwitchToInjured();  
        }
    }

    public void HandleInjuredState()
    {
        // Gradually become more injured over time
        health -= injuryRate * Time.deltaTime;
        Debug.Log($"Health: {health}");
        audioManager.PlayInjuredSounds();
        audioManager.PlayHeartbeat();

        // Calculate the hurt layer weight based on the time the player has been injured
        float hurtLayerWeight = Mathf.Clamp01(1 - (health / maxHealth));
        Debug.Log($"Hurt Layer Weight: {hurtLayerWeight}");

        animator.SetLayerWeight(1, hurtLayerWeight);

        if (health <= 0)
        {
            Debug.Log("Player has died!");
        }
    }

    public void HealPlayer()
    {
        health = maxHealth;  
        healTimer = 0f;  
        currentState = PlayerState.Healed;  
        UpdatePlayerMaterial(); 
        UpdateTimerUI();
        Debug.Log("Player healed and in Healed state.");
    }

    private void SwitchToInjured()
    {
        currentState = PlayerState.Injured;
        audioManager.aura.loop = false;
        audioManager.PowerupEnd();
        UpdatePlayerMaterial(); 
        Debug.Log("Player is now Injured.");
    }
    public float GetHealthPercentage()
    {
        float healthPercentage = health / maxHealth;
        Debug.Log($"Health Percentage: {healthPercentage}");
        return healthPercentage;
    }
    public void UpdatePlayerState(PlayerState newState)
    {
        if (newState == PlayerState.Injured)
        {

            // Activate blood particles
            if (bloodParticle1 != null && !bloodParticle1.isEmitting)
                bloodParticle1.Play();

            if (bloodParticle2 != null && !bloodParticle2.isEmitting)
                bloodParticle2.Play();
        }
        else if (newState == PlayerState.Healed)
        {
            // Stop blood particles when healed
            if (bloodParticle1 != null && bloodParticle1.isPlaying)
                bloodParticle1.Stop();

            if (bloodParticle2 != null && bloodParticle2.isPlaying)
                bloodParticle2.Stop();
        }
    }

    private void UpdatePlayerMaterial()
    {
        if (currentState == PlayerState.Healed)
        {
            playerRenderer.material = healedMaterial;
        }
        else if (currentState == PlayerState.Injured)
        {
            playerRenderer.material = injuredMaterial;
        }
    }

    public void UpdateHealthUI()
    {
        // Display rounded health value
        healthText.text = Mathf.RoundToInt(health).ToString();
    }

    public void UpdateScreenEffect()
    {
        // Lerp the color of the screen effect based on the player's health
        float healthPercentage = health / maxHealth;
        screenEffectImage.color = Color.Lerp(maxColor, initialColor, healthPercentage);
    }

    private void UpdateTimerUI()
    {
        // Display remaining time until injury
        float timeUntilInjured = healCooldown - healTimer;
        timerText.text = $"Injured in: {Mathf.Max(0, Mathf.RoundToInt(timeUntilInjured))}"; // Show 0 if negative
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            audioManager.aura.loop = false;
            audioManager.PowerupStart();
            audioManager.crystal.Play();
            HealPlayer();  // Heal the player when they touch a checkpoint
            Destroy(other.gameObject);
        }
    }
}