using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyPigController : MonoBehaviour
{
    // khi khai báo public, biến sẽ hiện ra trong Inspector của Unity.
    // chỉnh sửa giá trị trực tiếp trong Unity mà không cần mở code.
    public float moveSpeed = 1.5f;
    private Rigidbody2D rb;
    private Animator animator;
    private bool movingRight = false;
    private bool isMoving = true;

    [Header("Ground Check")]
    public Transform groundCheck;          // Empty Object dưới chân Enemy
    public float groundCheckRadius = 0.2f; // bán kính kiểm tra
    public LayerMask groundLayer;

    public float moveDuration = 5f;  // thời gian chạy
    public float idleDuration = 1f;  // thời gian nghỉ


    [Header("References")]
    public Transform player; // gán Player trong Inspector
    public float knockbackForce = 1f;
    public float knockbackUpForce = 0.5f;
    public float knockbackDuration = 0.4f;
    private float knockbackTimer;


    [Header("Attack Settings")]
    public float attackRange = 1.5f;  // khoảng cách để tấn công
    public float attackCooldown = 5f; // thời gian chờ giữa 2 lần tấn công
    private float attackTimer = 0f;

    [HideInInspector] public bool isKnockBack = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        StartCoroutine(MoveRoutine());        
    }


    void Update()
    {
        if (isKnockBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                isKnockBack = false;
            }
            return; // không chạy Move() khi knockback
        }

        // Move và check ground
        Move();
        CheckGroundAhead();

        // Tăng timer
        attackTimer += Time.deltaTime;
        // kiểm tra khoảng cách 
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && attackTimer >= attackCooldown)
        {
            AttackPlayer();
            attackTimer = 0f; // reset timer
        }
    }


    void Move()
    {
        float horizontal = movingRight ? 1f : -1f;

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
        movingRight = !movingRight;
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
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    void AttackPlayer()
    {
        animator.SetTrigger("Attack");
    }
}
