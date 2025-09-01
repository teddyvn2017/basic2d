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
    public float knockbackForce = 1.5f;
    public float knockbackUpForce = 2f;
    public float knockbackDuration = 0.4f;
    private float knockbackTimer;


    [Header("Attack Settings")]
    
    // public float attackCooldown = 2f; // thời gian chờ giữa 2 lần tấn công
    private float attackTimer = 0f;

    

    [HideInInspector] public bool isKnockBack = false;    

    private EnemyPigAttack enemyPigAttack;
    // private bool playerInRange = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyPigAttack = GetComponent<EnemyPigAttack>();
        StartCoroutine(MoveRoutine());
    }

    // void Update()
    // {
    //     attackTimer += Time.deltaTime;

    //     if (isKnockBack) return;

    //     if (playerInRange)
    //     {
    //         StopMovement();
    //         HandleFlip();

    //     }

    //     else
    //     {
    //         Move();
    //         CheckGroundAhead();
    //     }
    // }

    void Update()
    {

        if (isKnockBack) return;

        if (enemyPigAttack.playerInRange)
        {
            Debug.Log("playerInRange: " + enemyPigAttack.playerInRange);
            StopMovement();
            HandleFlip();
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

    //enemy knockback by player
    public void KnockBack(Transform attacker = null)
    {
        if (isKnockBack) return;

        isKnockBack = true;
        knockbackTimer = knockbackDuration;

        // Xác định hướng knockback
        float horizontalDir = (transform.position.x < attacker.position.x) ? -1f : 1f;

        // Reset vận tốc trước khi AddForce
        Vector2 knockbackDir = new Vector2(horizontalDir * knockbackForce, knockbackUpForce);
        // Debug.Log("knockbackDir x: " + knockbackDir.x);
        rb.AddForce(knockbackDir, ForceMode2D.Impulse);
        Invoke(nameof(EndKnockback), knockbackDuration);
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

    private void EndKnockback()
    {
        isKnockBack = false;
    }
    
}
