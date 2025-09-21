using System.Collections;
// using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnemyPigHealth : MonoBehaviour
{
    public int maxHealth = 2;
    private int currentHealth;
    private Animator animator;
    public float deathDelay = 0.5f;

    [Header("Knockback Settings")]
    // public float knockbackForce = 1f;

    private EnemyPigController controller;

    // public float knockbackDistance = 0.2f;
    private Rigidbody2D rb;


    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        // Lấy Rigidbody2D của đối tượng
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<EnemyPigController>();
    }

    public bool TakeDamage(int damage)
    {
        // Debug.Log("currentHealth: " + currentHealth);
        if (currentHealth <= 0) return false;
        currentHealth -= damage;
        // if (currentHealth <= 0) return; // chết rồi thì thoát khỏi hàm

        // Nếu máu <= 0, gọi hàm Die() và trả về true
        if (currentHealth <= 0)
        {
            Die();
            return true;
        }        
        
         // Nếu vẫn sống, trả về false
        return false;
    }

    void Die()
    {
        Debug.Log("Enemy is dead!");
        // animator?.SetBool("isRunning", false);
        animator?.SetTrigger("Die");               
        Destroy(gameObject, deathDelay);
    }    
}
