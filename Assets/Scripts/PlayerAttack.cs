using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack instance;
    public int attackDamage = 1;
    public KeyCode attackKey = KeyCode.Space;
    public Collider2D attackCollider; // collider vùng đánh
    private Animator animator;

    public float attackRange = 0.5f;

    public LayerMask enemyLayers;// nhớ chọn layer enemy ở inspector

    public Transform attackPoint;

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
        // Bật / tắt collider vùng đánh, theo phương pháp animation event
        // if (Input.GetKeyDown(attackKey))
        // {
        //     Attack();
        // }

        if (Input.GetKeyDown(attackKey))
        {
            Debug.Log("Attack");
            AttackEnemy();
        }

    }

    void Attack()
    {
        // Trigger animation Attack
        animator.SetTrigger("isAttacking");

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

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (!attackCollider.enabled) return;

    //     if (other.CompareTag("Enemy"))
    //     {
    //         EnemyPigHealth enemyHealth = other.GetComponent<EnemyPigHealth>();
    //         if (enemyHealth != null)
    //         {
    //             // enemyHealth.TakeDamage(attackDamage, this.transform);
    //             Debug.Log("Attack enemy");
    //         }
    //     }
    // }
    
    public void AttackEnemy()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        // Debug.Log("hitEnemies length: " + hitEnemies.Length);
        foreach (Collider2D enemy in hitEnemies)
        {
            // int enemyID = enemy.gameObject.GetInstanceID();

            EnemyPigHealth enemyHealth = enemy.GetComponent<EnemyPigHealth>();
            if (enemyHealth != null)
            {                
               bool isDead = enemyHealth.TakeDamage(attackDamage);
               if (isDead) continue;
            }

            // 1. Kiểm tra xem có phải là Heo Thường không
            EnemyPigController enemyPigController = enemy.GetComponent<EnemyPigController>();
            if (enemyPigController != null)
            {
                Debug.Log("Đã tấn công Heo Thường!");
                continue;
            }

            // 2. Kiểm tra xem có phải là Heo Mang Bom không
            BombPigParabolicController bombPigController = enemy.GetComponent<BombPigParabolicController>();

            if (bombPigController != null)
            {
                bombPigController.KnockBack(this.transform); //truyền vị trí player về heo mang bom
                continue;
            }
            

            // if (enemyHealth != null)
            //     enemyHealth.TakeDamage(attackDamage);

            // if (enemyPigController != null)
            //     enemyPigController.KnockBack(transform);    
            // Debug.Log("Attack enemy");
        }
    }

    // Debug: vẽ vùng attack trong Scene
    // void OnDrawGizmosSelected()
    // {
    //     if (attackCollider == null) return;
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    // }
    
    private void OnDrawGizmos()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
