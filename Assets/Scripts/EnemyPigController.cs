using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyPigController : MonoBehaviour
{
    // khi khai báo public, biến sẽ hiện ra trong Inspector của Unity.
    // chỉnh sửa giá trị trực tiếp trong Unity mà không cần mở code.
    public float moveSpeed = 1.2f;
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
    public float attackRange = 0.5f;  // khoảng cách để tấn công, tầm đánh (mở rộng xung quanh attackPoint)
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

    void Update()
    {
        attackTimer += Time.deltaTime;

        if (isKnockBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
                isKnockBack = false;
            return;
        }

        if (playerInRange)
        {
            StopMovement();
            HandleFlip();
            if (attackTimer >= attackCooldown)
            {
                animator.SetTrigger("Attack");
                attackTimer = 0f;
            }
        }

        else
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


    // 🟥 Hàm này sẽ được gọi trong Animation Event (frame va chạm)
    public void AttackPlayer()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
        if (hitPlayer != null)
        {
            Debug.Log("hitPlayer: " + hitPlayer.gameObject.name);
            
            PlayerHealth playerHealth = hitPlayer.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                Debug.Log("playerHealth: " + playerHealth);
                Vector2 knockbackDirection = (hitPlayer.transform.position - transform.position).normalized;
                playerHealth.TakeDamage(attackDamage, knockbackDirection);                
            }
        }
    }


    // gọi cuối animation Attack
    public void FinishAttack()
    {
        isAttacking = false;
    }

    //hàm được gọi trong Animation Event (frame va chạm)
    public void AttackAnimationEnd()
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

    private void HandleFlip()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x && !isFacingRight)
            Flip();
        else if (player.position.x < transform.position.x && isFacingRight)
            Flip();
    }
    

    private void StopMovement()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("isRunning", false);
    }
}
