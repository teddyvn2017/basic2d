using UnityEngine;

public class PlayerHealth : MonoBehaviour
{


    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    private PlayerController playerController;

    public UIManager uiManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        // playerController = GetComponent<PlayerController>();
        uiManager.UpdateHearts(currentHealth);

    }


    public void TakeDamage(int damage)
    {

        currentHealth -= damage;
        Debug.Log("currentHealth: " + currentHealth);

        uiManager.UpdateHearts(currentHealth);
        if (currentHealth <= 0)
            Die();
        
    }

    private void Die()
    {
        // playerController.Die();
        Debug.Log("Player is dead!");
    }


}
