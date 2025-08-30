using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    private PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {

        currentHealth -= damage;
        Debug.Log("currentHealth: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (playerController != null)
            {
                playerController.ApplyKnockback(knockbackDirection);
            }
        }
    }

    private void Die()
    {
        // playerController.Die();
        Debug.Log("Player is dead!");
    }


}
