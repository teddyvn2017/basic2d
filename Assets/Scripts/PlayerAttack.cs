using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack instance;
    public int attackDamage = 1;
    public KeyCode attackKey = KeyCode.Space;
    public Collider2D attackCollider; // collider vùng đánh
    private Animator animator;

    void Awake()
    {
        instance = this; // gán instance khi Player xuất hiện
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        attackCollider.enabled = false; // tắt collider ngay từ đầu
    }

    void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            Attack();
        }        
    }

    void Attack()
    {
        // Trigger animation Attack
        animator.SetTrigger("isAttacking");
        // Collider sẽ bật thông qua Animation Event
    }

    // Dùng Animation Event để bật collider đúng frame đánh
    public void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }

    public void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!attackCollider.enabled) return;

        if (other.CompareTag("Enemy"))    
        {
            EnemyPigHealth enemyHealth = other.GetComponent<EnemyPigHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage,this.transform);
            }
        }
    }
}
