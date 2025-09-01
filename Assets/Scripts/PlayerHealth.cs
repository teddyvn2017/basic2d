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


    public void TakeDamage(int damage)
    {

        currentHealth -= damage;
        Debug.Log("currentHealth: " + currentHealth);
        if (currentHealth <= 0)
            Die();
        
    }

    private void Die()
    {
        // playerController.Die();
        Debug.Log("Player is dead!");
    }


}
