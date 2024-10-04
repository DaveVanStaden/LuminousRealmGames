using UnityEngine;
using TMPro;  // Import TextMeshPro namespace

public class PlayerHealth : MonoBehaviour
{
    public enum PlayerState { Healed, Injured }
    public PlayerState currentState = PlayerState.Healed;

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health;
    [SerializeField] private float injuryRate = 1f;  // How fast the player gets injured over time
    [SerializeField] private float damagePerSecond = 5f;  // Damage taken while injured
    [SerializeField] private float healCooldown = 30f;  // Time to remain healed after touching a checkpoint

    private float healTimer = 0f;  // Timer to track the healed state duration

    // Materials for player states
    [SerializeField] private Material healedMaterial;
    [SerializeField] private Material injuredMaterial;
    [SerializeField] private Renderer playerRenderer;

    // TextMeshPro for health display
    [SerializeField] private TextMeshProUGUI healthText;

    void Start()
    {
        health = maxHealth;
        UpdatePlayerMaterial(); // Set initial material based on state
        UpdateHealthUI(); // Set initial health display
    }

    void Update()
    {
        if (currentState == PlayerState.Healed)
        {
            HandleHealedState();
        }
        else if (currentState == PlayerState.Injured)
        {
            HandleInjuredState();
        }
    }

    void FixedUpdate()
    {
        UpdateHealthUI();  // Update health display every physics frame
    }

    private void HandleHealedState()
    {
        healTimer += Time.deltaTime;

        if (healTimer >= healCooldown)
        {
            SwitchToInjured();  // Become injured after cooldown period
        }
    }

    private void HandleInjuredState()
    {
        // Gradually become more injured over time
        health -= injuryRate * Time.deltaTime;

        if (health <= maxHealth * 0.5f)  // Start taking damage when health is less than 50%
        {
            health -= damagePerSecond * Time.deltaTime;
        }

        if (health <= 0)
        {
            // Handle player death (e.g., game over, respawn logic)
            Debug.Log("Player has died!");
        }
    }

    public void HealPlayer()
    {
        health = maxHealth;  // Restore full health
        healTimer = 0f;  // Reset the heal timer
        currentState = PlayerState.Healed;  // Switch to Healed state
        UpdatePlayerMaterial(); // Update to healed material
        Debug.Log("Player healed and in Healed state.");
    }

    private void SwitchToInjured()
    {
        currentState = PlayerState.Injured;
        UpdatePlayerMaterial(); // Update to injured material
        Debug.Log("Player is now Injured.");
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

    private void UpdateHealthUI()
    {
        // Display rounded health value
        healthText.text = Mathf.RoundToInt(health).ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            HealPlayer();  // Heal the player when they touch a checkpoint
            Destroy(other.gameObject);
        }
    }
}
