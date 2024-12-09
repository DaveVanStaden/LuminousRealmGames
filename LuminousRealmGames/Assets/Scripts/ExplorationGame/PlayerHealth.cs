using UnityEngine;
using TMPro;
using Unity.Cinemachine; // Import TextMeshPro namespace
using UnityEngine.UI;
using static Unity.Cinemachine.CinemachineCamera;
using MalbersAnimations; // Import UI namespace for the screen effect

public class PlayerHealth : MonoBehaviour
{
    public enum PlayerState { Healed, Injured }
    public PlayerState currentState = PlayerState.Healed;

    [SerializeField] private float maxHealth = 100f;
    public float health;
    public float injuryRate = 1f;  // How fast the player gets injured over time
    [SerializeField] private float damagePerSecond = 5f;  // Damage taken while injured
    [SerializeField] private float healCooldown = 30f;  // Time to remain healed after touching a checkpoint
    [SerializeField] private float arrowDamage = 10f;  // Time to remain healed after touching a checkpoint

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

    [SerializeField] private Transform lastCheckpoint;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private MalbersInput malbersInput;
    [SerializeField] private GameObject PlayerObject;
    private void Awake()
    {
        audioManager = GetComponent<PlayerAudioManager>();
        malbersInput = FindAnyObjectByType<MalbersInput>().GetComponent<MalbersInput>();
    }
    void Start()
    {
        lastCheckpoint = spawnPoint;
        health = maxHealth;
        UpdatePlayerMaterial(); 
        UpdateHealthUI(); 
        UpdateScreenEffect(); 
        UpdateTimerUI(); 
    }
    public void KillPlayer()
    {
        health = 0;
        HandleDeath();
    }
    public void HandleDeath()
    {
        //Play death sound and animation
        PlayerObject.transform.position = lastCheckpoint.position;
        HealPlayer();
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
        if (malbersInput.ActiveMap.name != "PowerUp")
        {
            malbersInput.SetMap("PowerUp");
        }
        health = maxHealth;  
        healTimer = 0f;  
        currentState = PlayerState.Healed;  
        UpdatePlayerMaterial(); 
        UpdateTimerUI();
        Debug.Log("Player healed and in Healed state.");
        animator.SetLayerWeight(1, 0);
    }

    private void SwitchToInjured()
    {
        if(malbersInput.ActiveMap.name != "Death")
        {
            malbersInput.SetMap("Death");
        }
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
        if(healthText != null)
            healthText.text = Mathf.RoundToInt(health).ToString();
    }

    public void UpdateScreenEffect()
    {
        // Lerp the color of the screen effect based on the player's health
        float healthPercentage = health / maxHealth;
        if (screenEffectImage != null)
            screenEffectImage.color = Color.Lerp(maxColor, initialColor, healthPercentage);
    }

    private void UpdateTimerUI()
    {
        // Display remaining time until injury
        float timeUntilInjured = healCooldown - healTimer;
        if (timerText != null)
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
            lastCheckpoint = other.transform;  // Store the last checkpoint
            //dissable checkpoint stuff
            /*var tempCollider = other.GetComponent<BoxCollider>();
            tempCollider.enabled = false;
            var tempRenderer = other.GetComponent<MeshRenderer>();
            tempRenderer.enabled = false;*/
        }
        if(other.CompareTag("Arrow"))
        {
            if(currentState == PlayerState.Healed)
            {
                SwitchToInjured();
            }else if(currentState == PlayerState.Injured)
            {
                health -= arrowDamage;
            }
        }
        if(other.CompareTag("KillPlayer"))
        {
            KillPlayer();
        }
    }
}