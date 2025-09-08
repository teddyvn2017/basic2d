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

    public void TakeDamage(int damage, Transform attacker = null)
    {
        // if (currentHealth <= 0) return; // chết rồi thì thoát khỏi hàm

        

        if (currentHealth <= 0)
        {
            Die();
            // Debug.Log("Enemy is dead!");
        }

        else
        {
            currentHealth -= damage;    
        }
        // else
        // {
        //     animator.SetTrigger("Hit");            
        //     if (controller != null)                
        //         controller.KnockBack(attacker);
        // }
    }

    void Die()
    {

        // animator?.SetBool("isRunning", false);
        animator?.SetTrigger("Death");        
        // rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        Destroy(gameObject, deathDelay);
    }

    

    
}
