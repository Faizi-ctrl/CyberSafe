using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float healthRegenRate = 2f;
    public float healthRegenDelay = 2f;
    private float currentHealth;
    private float lastDamagedTime;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaRegenRate = 5f;
    public float staminaDrainRate = 20f; // per second
    public float staminaRegenDelay = 1f;
    private float currentStamina;
    private float lastSprintTime;

    [Header("Sprinting")]
    public float walkSpeed = 5.0f;
    public float sprintSpeed = 10.0f;

    public CharacterController cC;

    private Vector3 moveDirection;
    public bool isSprinting;

    private bool isDead = false;

    void Start()
    {
        // currentHealth = maxHealth;
        // currentStamina = maxStamina;
    }

    void Update()
    {
        HandleMovement();
        // RegenerateHealth();
        // RegenerateStamina();
    }

    public void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(h, 0, v).normalized;

        float speed = isSprinting ? sprintSpeed : walkSpeed;

        if (isSprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            lastSprintTime = Time.time;
        }

        moveDirection = input * speed;
    }

    // private void RegenerateHealth()
    // {
    //     if (Time.time > lastDamagedTime + healthRegenDelay && currentHealth < maxHealth)
    //     {
    //         currentHealth += healthRegenRate * Time.deltaTime;
    //         currentHealth = Mathf.Min(currentHealth, maxHealth);
    //     }
    // }

    // private void RegenerateStamina()
    // {
    //     if (!isSprinting && Time.time > lastSprintTime + staminaRegenDelay && currentStamina < maxStamina)
    //     {
    //         currentStamina += staminaRegenRate * Time.deltaTime;
    //         currentStamina = Mathf.Min(currentStamina, maxStamina);
    //     }
    // }

    // public void TakeDamage(float amount)
    // {
    //     if (isDead) return;

    //     currentHealth -= amount;
    //     currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    //     lastDamagedTime = Time.time;

    //     if (currentHealth <= 0)
    //     {
    //         Die();
    //     }
    // }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player Died");

        // Disable player control
        PlayerJoystickMovement movement = GetComponent<PlayerJoystickMovement>();
        if (movement != null)
            movement.enabled = false;

        // Trigger death animation
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("die");

            float deathDuration = 3f; // Default fallback
            RuntimeAnimatorController ac = animator.runtimeAnimatorController;

            foreach (var clip in ac.animationClips)
            {
                if (clip.name.ToLower().Contains("die"))
                {
                    deathDuration = clip.length;
                    break;
                }
            }

           // StartCoroutine(DelayedEndScene(deathDuration));
        }
        // else
        // {
        //     StartCoroutine(DelayedEndScene(3f));
        // }
    }

    // private IEnumerator DelayedEndScene(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     SceneManager.LoadScene("End_Scene");
    // }

    // public void RestoreHealth(float percent)
    // {
    //     if (currentHealth < maxHealth)
    //     {
    //         float amount = maxHealth * percent;
    //         currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    //     }
    // }

    // public void RestoreStamina(float percent)
    // {
    //     if (currentStamina < maxStamina)
    //     {
    //         float amount = maxStamina * percent;
    //         currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
    //     }
    // }

    public void StartSprinting()
    {
        if (maxStamina > 0f)
            isSprinting = true;
    }

    public void StopSprinting()
    {
        isSprinting = false;
    }

    // // For UI
    // public float GetHealth() => currentHealth;
    // public float GetStamina() => currentStamina;
    // public float GetMaxHealth() => maxHealth;
    // public float GetMaxStamina() => maxStamina;
}