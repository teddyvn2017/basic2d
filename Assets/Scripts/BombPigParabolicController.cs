using System.Collections;
using UnityEngine;

public class BombPigParabolicController : MonoBehaviour
{
    [Header("Enemy Settings")]
    protected Rigidbody2D rb;
    protected Animator animator;
    public float moveSpeed = 1f;
    protected bool isFacingRight = false;
    protected bool isMoving = true;

    [Header("Explosion Settings")]
    public float lifeTime = 2f;
    public Transform throwPoint;
    public float throwSpeed = 5f;
    public GameObject bombPrefab;
    public float timeToTarget = 3f;

    [Header("Cooldown Settings")]
    public float throwCooldown = 3f;
    // private float lastThrowTime;

    private Transform playerTransform; //lưu giá vị trí mà player nằm trong vùng detect zone
    
    
    private Vector2 lastKnownPlayerPos;
    public bool hasDetectedPlayer = false;
    private bool canThrow = true;


    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.5f;


    [Header("References")]    
    public Transform player; //dùng để so sánh vị trí cho hàm HandleFlip


    [Header("Movement Routine Settings")]
    private float moveDuration = 3f;  // thời gian chạy
    private float idleDuration = 1.5f;  // thời gian nghỉ

    // private bool isThrowing = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
        StartCoroutine(MoveRoutine());
    }

    private void Update()
    {

        if (hasDetectedPlayer)
        {
            StopMovement();
            HandleFlip();
            if (canThrow)
            {
                
                StartCoroutine(ThrowWithDelay());
            }                     
        }
        else
        {
            isMoving = true;
            Move();
            CheckGroundAhead();
        }
    }

    IEnumerator ThrowWithDelay()
    {
        canThrow = false;
        ThrowBomb(playerTransform.position);
        yield return new WaitForSeconds(throwCooldown);
        canThrow = true;
    }

    private void ThrowBomb(Vector2 targetPos)
    {
        if (bombPrefab == null || throwPoint == null) return;

        GameObject bombInstance = Instantiate(bombPrefab, throwPoint.position, Quaternion.identity);
        Destroy(bombInstance, lifeTime);

        Animator anim = bombInstance.GetComponent<Animator>();
        if (anim != null) anim.Play("Bomb_Fuse");

        Rigidbody2D rb = bombInstance.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 throwVelocity = CalculateThrow(targetPos, throwPoint.position);
            rb.linearVelocity = throwVelocity;
            rb.angularVelocity = -360f;
        }
    }

    public Vector2 CalculateThrow(Vector2 target, Vector2 start)
    {
        Vector2 distance = target - start;
        Vector2 gravity = Physics2D.gravity;

        float vx = distance.x / timeToTarget;
        float vy = (distance.y - 0.5f * gravity.y * timeToTarget * timeToTarget) / timeToTarget;

        return new Vector2(vx, vy);
    }


    public void OnPlayerDetected(Transform pos)
    {
        hasDetectedPlayer = true;
        playerTransform = pos;// vị trí mà player đang đứng
    }
    
   

    void Move()
    {
        if (!isMoving)
        {
            if (animator != null)
                animator.SetBool("isRunning", false);
            rb.linearVelocity = Vector2.zero; // enemy đứng yên
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
        checkPos.x += isFacingRight ? 0.5f : -0.5f; 
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

    private void HandleFlip()
    {
        if (player.position.x > transform.position.x && !isFacingRight)
            Flip();
        else if (player.position.x < transform.position.x && isFacingRight)
            Flip();
    }

    private void StopMovement()
    {
        isMoving = false;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isRunning", false);
    }
}
