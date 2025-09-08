using System.Collections;
using UnityEngine;

public class BaseEnemyController : MonoBehaviour
{

    [Header("Enemy Settings")]
    protected Rigidbody2D rb;
    protected Animator animator;
    public float moveSpeed = 0.5f;
    // public int health = 2;

    //con heo đang hướng mặt sang trái nên giá trị = false
    protected bool isFacingRight = false;
    protected bool isMoving = true;

    [Header("Knockback Settings")]
    public float knockbackForce = 0.5f;
    public float knockbackUpForce = 0.4f;
    public float knockbackDuration = 0.5f;
    protected bool isKnockBack = false;


    [Header("Movement Routine Settings")]
     private float moveDuration = 3f;  // thời gian chạy
    private float idleDuration = 1.5f;  // thời gian nghỉ


    [Header("Ground Check")]
    public Transform groundCheck;          // Empty Object dưới chân Enemy  
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.5f;        

    private bool playerInRange = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true; //ngăn con heo bị lộn ngược
    }
    protected virtual void Update()
    {
        if (isKnockBack) return;

        if (playerInRange)
        {
            Debug.Log("playerInRange: " + playerInRange);
        }

        // Nếu tới mép hố thì quay lại
        if (CheckGroundAhead())
            Flip();

        // CheckGroundAhead();
        Move();
    }

    protected virtual void StopMovement()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator?.SetBool("isRunning", false);
    }

    protected virtual void Move()
    {
                
        if (!isMoving)
        {
            StopMovement();
            return;
        }
        
        float direction = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        animator?.SetBool("isRunning", true);
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

    bool CheckGroundAhead()
    {
        // lấy vị trị của ground check
        Vector2 checkPos = groundCheck.position;
        checkPos.x += isFacingRight ? 0.5f : -0.5f; //tăng khoảng cách kiểm tra phía trước 0.5f theo hướng chạy    
        bool noGroundAhead = !Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);                
        return noGroundAhead;        
    }

   
    // public virtual void KnockBack(Transform attacker = null)
    // {
    //     // Dừng Coroutine cũ để tránh lỗi timing
    //     StopAllCoroutines();

    //     // Bắt đầu Coroutine mới
    //     StartCoroutine(KnockBackCoroutine(attacker));
    // }

    // cho class con hoặc bên ngoài gọi (vd Player đánh enemy)
    // transfrom từ player => attacker
    // public virtual void KnockBack(Transform attacker = null)
    // {
    //     if (isKnockBack) return;

    //     isKnockBack = true;
    //     animator.SetTrigger("Hit");

    //     // Xác định hướng knockback
    //     float horizontalDir = (transform.position.x < attacker.position.x) ? -1f : 1f;
    //     // Reset vận tốc trước khi AddForce       
    //     rb.linearVelocity = Vector2.zero;
    //     Vector2 knockbackDir = new Vector2(horizontalDir * knockbackForce, knockbackUpForce);
    //     // Debug.Log("knockbackDir x: " + knockbackDir.x + " knockbackDir y: " + knockbackDir.y);
    //     rb.AddForce(knockbackDir, ForceMode2D.Impulse);
    //     Invoke(nameof(EndKnockback), knockbackDuration);
    // }

    protected void EndKnockback()
    {
        isKnockBack = false;
    }
    
    private IEnumerator KnockBackCoroutine(Transform attacker)
    {
        // Kiểm tra an toàn để tránh lỗi NullReferenceException
        if (rb == null || attacker == null) 
        {
            isKnockBack = false;
            yield break;
        }

        isKnockBack = true;
        animator.SetTrigger("Hit");

        // Xác định hướng knockback dựa trên vị trí của attacker
        float horizontalDir = (transform.position.x < attacker.position.x) ? -1f : 1f;

        // Reset toàn bộ vận tốc để tránh cộng dồn lực
        rb.linearVelocity = Vector2.zero;

        // Tạo vector lực đẩy
        Vector2 knockbackDir = new Vector2(horizontalDir * knockbackForce, knockbackUpForce);
        
        // Áp dụng lực
        rb.AddForce(knockbackDir, ForceMode2D.Impulse);

        // Chờ trong thời gian knockbackDuration
        yield return new WaitForSeconds(knockbackDuration);

        // Kết thúc knockback, cho phép enemy di chuyển lại
        isKnockBack = false;
        
    }
}
