using UnityEngine;

public class EnemyPigHealth : MonoBehaviour
{
    public int maxHealth = 3;
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
        if (currentHealth <= 0) return; // chết rồi thì thoát khỏi hàm

        currentHealth -= damage;
        //Debug.Log("currentHealth: " + currentHealth);


        if (currentHealth <= 0)
        {
            Die();
            // Debug.Log("Enemy is dead!");
        }
        else
        {
            animator.SetTrigger("Hit");
            // Debug.Log("Enemy is Hit!");

            // Truy cập Controller và bật knockback
            // EnemyPigController controller = GetComponent<EnemyPigController>();
            // controller.isKnockBack = true;
            if (controller != null)                
                controller.KnockBack(attacker);
        }
    }

    void Die()
    {

        // animator?.SetBool("isRunning", false);
        animator?.SetTrigger("Death");

        // Tắt tất cả collider để con heo không còn va chạm
        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;

        // Dừng mọi chuyển động
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
        }

        Destroy(gameObject, deathDelay);
    }

    

    
}
