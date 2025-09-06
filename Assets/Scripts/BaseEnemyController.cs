using UnityEngine;

public class BaseEnemyController : MonoBehaviour
{

    [Header("Enemy Settings")]
    protected Rigidbody2D rb;
    protected Animator animator;
    public float moveSpeed = 1.2f;
    public int health = 2;
    protected bool isFacingRight = true;
    protected bool isMoving = true;

    [Header("Knockback Settings")]

    public float knockbackForce = 1.5f;
    public float knockbackUpForce = 2f;
    public float knockbackDuration = 0.6f;
    protected bool isKnockBack = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isKnockBack) return;

        if (isMoving)
            Move();
        else StopMovement();        

    }

    protected virtual void StopMovement()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator?.SetBool("isRunning", false);
    }

    protected virtual void Move()
    {
        float horizontal = isFacingRight ? 1f : -1f;
        transform.position += new Vector3(horizontal, 0, 0) * moveSpeed * Time.deltaTime;

        if (animator != null)
            animator.SetBool("isRunning", true);
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // cho class con hoặc bên ngoài gọi (vd Player đánh enemy)
    public virtual void KnockBack(Transform attacker = null)
    {
        if (isKnockBack) return;

        animator.SetTrigger("Hit");

        // Xác định hướng knockback
        float horizontalDir = (transform.position.x < attacker.position.x) ? -1f : 1f;
        // Reset vận tốc trước khi AddForce
        Vector2 knockbackDir = new Vector2(horizontalDir * knockbackForce, knockbackUpForce);
        rb.AddForce(knockbackDir, ForceMode2D.Impulse);
        Invoke(nameof(EndKnockback), knockbackDuration);
    }

    protected void EndKnockback()
    {
        isKnockBack = false;
    }
}
