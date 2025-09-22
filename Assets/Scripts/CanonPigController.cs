using System.Collections;
using UnityEngine;

public class CanonPigController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private bool isFacingRight = true;
    private bool isMoving = true;

    public bool hasDetectedPlayer = false;
    private bool canShoot = true;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.5f;

    [Header("Shoot Settings")]
    public float lifeTime = 2f;
    public Transform shootPoint;
    public float throwSpeed = 5f;
    public GameObject bombPrefab;
    // public float timeToTarget = 3f;


    [Header("Movement Routine Settings")]
    private float moveDuration = 3f;  // thời gian chạy
    private float idleDuration = 1.5f;  // thời gian nghỉ
    public float moveSpeed = 1f;
    private float randomMoveDuration;
    private float randomIdleDuration;

    [Header("References")]
    public Transform player; //dùng để so sánh vị trí cho hàm HandleFlip()

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
        StartCoroutine(MoveRoutine());
        animator.SetBool("isRunning", true);
        randomMoveDuration = Random.Range(2f, 4f);
        randomIdleDuration = Random.Range(1f, 2f);
    }

    // Update is called once per frame
    void Update()
    {

        CheckGroundAhead();
        Move();
        // if (hasDetectedPlayer)
        // {
        //     // StopMovement();
        //     HandleFlip();
        //     // if (canThrow)
        //     // {
        //     //     StartCoroutine(ThrowWithDelay());
        //     // }
        // }
        // else
        // {
        //     Move();
        //     CheckGroundAhead();
        // }
    }

    void Move()
    {
        if (!isMoving) return;// dùng để tạm dừng
        float horizontal = isFacingRight ? 1f : -1f;
        //tạm dùng transform position thay cho linearvelocity
        transform.position += new Vector3(horizontal, 0, 0) * moveSpeed * Time.deltaTime;
        // rb.linearVelocity = new Vector2(horizontal * moveSpeed, 0);
        if (animator != null)
            Debug.Log("isRunning");
            // animator.SetBool("isRunning", true);

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
        //thiết lập biến isMoving thông qua coroutine
        while (true)
        {
            // chạy
            isMoving = true;
            yield return new WaitForSeconds(randomMoveDuration);
            // nghỉ
            isMoving = false;
            yield return new WaitForSeconds(randomIdleDuration);
        }
    }
    
    private void HandleFlip()
    {
        if (player.position.x > transform.position.x && !isFacingRight)
            Flip();
        else if (player.position.x < transform.position.x && isFacingRight)
            Flip();
    }
}
