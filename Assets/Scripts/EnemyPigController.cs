using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyPigController : MonoBehaviour
{
    // khi khai báo public, biến sẽ hiện ra trong Inspector của Unity.
    // chỉnh sửa giá trị trực tiếp trong Unity mà không cần mở code.
    public float moveSpeed = 1f;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isFacingRight = false;
    private bool isMoving = true;

    [Header("Ground Check")]
    public Transform groundCheck;          // Empty Object dưới chân Enemy
    public float groundCheckRadius = 0.3f; // bán kính kiểm tra
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    public float moveDuration = 4f;  // thời gian chạy
    public float idleDuration = 1.5f;  // thời gian nghỉ


    [Header("References")]
    public Transform player; // gán Player trong Inspector

    [Header("Knockback Settings")]
    public float knockbackForce = 1f;
    public float knockbackUpForce = 0.5f;
    public float knockbackDuration = 0.4f;
    private float knockbackTimer;


    [Header("Attack Settings")]
    public float attackRange = 0.5f;  // khoảng cách để tấn công
    public float attackCooldown = 2f; // thời gian chờ giữa 2 lần tấn công
    private float attackTimer = 0f;

    public int attackDamage = 1;
    public Transform attackPoint;

    [HideInInspector] public bool isKnockBack = false;

    [HideInInspector] public bool isAttacking = false;

    private bool playerInRange = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        StartCoroutine(MoveRoutine());
    }



    // void Update()
    // {
    //     if (isAttacking) return;

    //     if (isKnockBack)
    //     {
    //         knockbackTimer -= Time.deltaTime;
    //         if (knockbackTimer <= 0)
    //         {
    //             isKnockBack = false;
    //         }
    //         return; // không chạy Move() khi knockback
    //     }

    //     // Move và check ground
    //     Move();
    //     CheckGroundAhead();

    //     // Tăng timer
    //     attackTimer += Time.deltaTime;

    //     // kiểm tra khoảng cách 
    //     float distanceToPlayer = Vector2.Distance(transform.position, player.position);

    //     if (distanceToPlayer <= attackRange && attackTimer >= attackCooldown)
    //     {
    //         Debug.Log("Attack Player");
    //         TriggerAttackAnimation();
    //         attackTimer = 0f; // reset timer
    //     }
    // }

    void Update()
    {

        // Update attack timer
        attackTimer += Time.deltaTime;

        // Knockback logic
        if (isKnockBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
                isKnockBack = false;
            return;
        }

        // Player in attack range
        // if (playerInRange)
        // {
        //     // rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        //     animator.SetBool("isRunning", false);

        //     if (attackTimer >= attackCooldown)
        //     {
        //         TriggerAttackAnimation();
        //         attackTimer = 0f;
        //     }
        //     // Debug.Log("Attack Player");
        // }

        // Kiểm tra khoảng cách với player
        HandleAttack();
        //Lấy thông tin state hiện tại của animator ở layer 0 (layer mặc định)
        //kiểm tra xem state đó có phải là "Attack" hay không
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            Move();
            CheckGroundAhead();
        }



    }
    void Move()
    {
        if (!isMoving)
        {
            if (animator != null)
                animator.SetBool("isRunning", false);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // enemy đứng yên
            return;
        }

        float horizontal = isFacingRight ? 1f : -1f;

        transform.position += new Vector3(horizontal, 0, 0) * moveSpeed * Time.deltaTime;
        // rb.linearVelocity = new Vector2(horizontal * moveSpeed, 0);
        if (animator != null)
            animator.SetBool("isRunning", true);

    }

    void CheckGroundAhead()
    {
        // Kiểm tra có mặt đất ở phía trước
        Vector2 checkPos = groundCheck.position;
        bool noGroundAhead = !Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);

        // Debug.Log("noGroundAhead: " + noGroundAhead);
        if (noGroundAhead)
            Flip();
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            // chạy
            isMoving = true;
            yield return new WaitForSeconds(moveDuration);
            // nghỉ
            isMoving = false;
            yield return new WaitForSeconds(idleDuration);
        }
    }

    public void KnockBack(Transform attacker = null)
    {
        if (isKnockBack) return;

        isKnockBack = true;
        knockbackTimer = knockbackDuration;

        // Xác định hướng knockback
        float knockbackDir;
        knockbackDir = (transform.position.x < attacker.position.x) ? -1f : 1f;

        // Reset vận tốc trước khi AddForce
        rb.linearVelocity = Vector2.zero;

        // AddForce bật ngược
        rb.AddForce(new Vector2(knockbackDir * knockbackForce, knockbackUpForce), ForceMode2D.Impulse);
    }


    private IEnumerator ResetKnockback(float delay = 0.2f)
    {
        yield return new WaitForSeconds(delay);
        isKnockBack = false;
    }
    // Optional: hiển thị vùng groundCheck trên Scene để debug
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            // Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }

    //hiển thị animation tấn công
    void TriggerAttackAnimation()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
    }

    //hàm gây damege cho player, được gọi trong animation Attack
    public void AttackPlayer()
    {
        // Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);        
        // if (hitPlayer != null)
        // {
        //     PlayerHealth playerHealth = hitPlayer.GetComponent<PlayerHealth>();
        //     if (playerHealth != null)
        //     {
        //         Vector2 knockbackDirection = (hitPlayer.transform.position - transform.position).normalized;
        //         playerHealth.TakeDamage(attackDamage, knockbackDirection);
        //         Debug.Log("Attack Player");
        //     }
        // }

        // cách 2 
        if (player == null) return;

        // Kiem tra khoang cach giua enemy va player, thay vì dùng trigger như comment code tren
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            Vector2 knockbackDirection = (player.position - transform.position).normalized;
            player.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage, knockbackDirection);
            Debug.Log("Enemy hit Player!");
        }
    }

    // gọi cuối animation Attack
    public void FinishAttack()
    {
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            Debug.Log("Player đã vào phạm vi tấn công!");
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player đã rời khỏi phạm vi tấn công!");
            playerInRange = false;
        }
    }    
    

    private void HandleAttack()
    {
        if (player == null) return;

        // Tăng timer giữa các lần attack
        attackTimer += Time.deltaTime;

        // Tính khoảng cách đến player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Quay mặt về phía player nếu cần
            if (player.position.x > transform.position.x && !isFacingRight)
            {
                Flip();
            }
            else if (player.position.x < transform.position.x && isFacingRight)
            {
                Flip();
            }

            // Ngưng chạy khi vào range
            animator.SetBool("isRunning", false);

            // Nếu cooldown đủ → bắt đầu Attack
            if (attackTimer >= attackCooldown)
            {
                animator.SetTrigger("Attack"); // Animation Event sẽ gọi AttackPlayer()
                attackTimer = 0f;
            }
        }
    }

    
    // void UpdateAttackPoint()
    // {
    //     if (attackPoint != null)
    //     {
    //         attackPoint.localPosition = new Vector3(isFacingRight ? 0.5f : -0.5f, 0, 0);
    //     }
    // }

}
