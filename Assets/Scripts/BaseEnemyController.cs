using System.Collections;
using UnityEngine;

public class BaseEnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Attack,
        Die
    }

    [Header("Enemy Settings")]
    protected Rigidbody2D rb;
    protected Animator animator;
    public float moveSpeed = 0.5f;
    protected bool isFacingRight = false;
    protected bool isMoving = true;

    [Header("Knockback Settings")]
    public float knockbackForce = 2f;
    public float knockbackUpForce = 3f;
    public float knockbackDuration = 0.3f;
    protected bool isKnockBack = false;

    [Header("Movement Routine Settings")]
    protected float randomMoveDuration;
    protected float randomIdleDuration;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.5f;

    public EnemyState currentState = EnemyState.Patrol;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
        randomMoveDuration = Random.Range(2f, 4f);
        randomIdleDuration = Random.Range(1f, 2f);
        
        // Khởi tạo trạng thái ban đầu và bắt đầu coroutine tuần tra
        ChangeState(EnemyState.Patrol);
    }

    protected virtual void Update()
    {
        if (isKnockBack) return;

        switch (currentState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Die:
                break;
        }
    }

    protected virtual void Idle()
    {
        animator.SetBool("isRunning", false);
        rb.linearVelocity = Vector2.zero;
        // currentState = EnemyState.Idle; // Dòng này không cần thiết vì đã được set trong coroutine
    }

    protected virtual void Patrol()
    {
        if (CheckGroundAhead())
            Flip();
        Move();
    }

    protected virtual void Move()
    {
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
            currentState = EnemyState.Patrol;
            yield return new WaitForSeconds(randomMoveDuration);
            
            currentState = EnemyState.Idle;
            yield return new WaitForSeconds(randomIdleDuration);
        }
    }

    bool CheckGroundAhead()
    {
        Vector2 checkPos = groundCheck.position;
        checkPos.x += isFacingRight ? 0.5f : -0.5f;
        bool noGroundAhead = !Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);
        return noGroundAhead;
    }

    public virtual void KnockBack(Transform attacker = null)
    {
        if (isKnockBack) return;

        Debug.Log("KnockBack");
        isKnockBack = true;
        animator.SetTrigger("Hit");
        float horizontalDir = (transform.position.x < attacker.position.x) ? -1f : 1f;
        rb.linearVelocity = Vector2.zero;
        Vector2 knockbackDir = new Vector2(horizontalDir * knockbackForce, knockbackUpForce);
        rb.AddForce(knockbackDir, ForceMode2D.Impulse);
        Invoke(nameof(EndKnockback), knockbackDuration);


    }

    protected void EndKnockback()
    {
        isKnockBack = false;
        // Chuyển lại trạng thái tuần tra sau khi hết knockback
        ChangeState(EnemyState.Patrol);
    }
    
    protected virtual void Attack() { }

    protected void ChangeState(EnemyState newState)
    {
        // Điều kiện để coroutine tuần tra luôn được khởi động khi chuyển sang trạng thái Patrol
        if (currentState != newState || newState == EnemyState.Patrol)
        {
            StopAllCoroutines();
            currentState = newState;

            switch (currentState)
            {
                case EnemyState.Patrol:
                    StartCoroutine(MoveRoutine());
                    break;
                case EnemyState.Attack:
                    rb.linearVelocity = Vector2.zero;
                    animator?.SetBool("isRunning", false);
                    break;
            }
        }
    }
}
