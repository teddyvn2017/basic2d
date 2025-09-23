using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class CanonPigController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private bool isFacingRight = true;
    private bool isMoving = true;

    public bool hasDetectedPlayer = false;
    private bool canShoot = true;
    private bool hasReachedCanon = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.6f;

    [Header("Shoot Settings")]
    public float lifeTime = 2f;
    public Transform shootPoint;
    // public float throwSpeed = 5f;
    public GameObject bombPrefab;
    // public float timeToTarget = 3f;


    // [Header("Movement Routine Settings")]
    // private float moveDuration = 3.f;  // thời gian chạy
    // private float idleDuration = 1.5f;  // thời gian nghỉ
    [Header("Movement Routine Settings")]
    public float moveSpeed = 1f;
    public float stoppingDistance = 0.3f;
    private float randomMoveDuration;
    private float randomIdleDuration;

    [Header("References")]
    public Transform player; //dùng để so sánh vị trí cho hàm HandleFlip()

    public Transform canonTransform;
    public Transform canonDestination;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isMoving = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("isRunning", true);
        // rb.freezeRotation = true;
        StartCoroutine(MoveRoutine());
        randomMoveDuration = Random.Range(2f, 4f);
        randomIdleDuration = Random.Range(1f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (hasDetectedPlayer)
        {
            // StopCoroutine(MoveRoutine());     
            HandleFlip();
            MoveToCanon();
        }
        else
        {
            Move();
            CheckCanonAhead();
            CheckGroundAhead();
        }
    }

    void Move()
    {
        if (!isMoving) return; // kết hợp với kiểu di chuyển tuần tra 
        animator.SetBool("isRunning", true);
        float horizontal = isFacingRight ? 1f : -1f;
        //tạm dùng transform position thay cho linearvelocity
        transform.position += new Vector3(horizontal, 0, 0) * moveSpeed * Time.deltaTime;
        // rb.linearVelocity = new Vector2(horizontal * moveSpeed, 0);
    }

    // Kiểm tra có khẩu súng cannon ở phía trước hay không
    private void CheckCanonAhead()
    {
        Vector2 targetPos = canonTransform.position;
        float d = Vector2.Distance(transform.position, targetPos);
        if (d < stoppingDistance)
            Flip();
    }

    void CheckGroundAhead()
    {
        // Kiểm tra có mặt đất ở phía trước
        Vector2 checkPos = groundCheck.position;
        checkPos.x += isFacingRight ? 0.5f : -0.5f;
        bool noGroundAhead = !Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);
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

    public void OnPlayerDetected(Transform pos)
    {
        hasDetectedPlayer = true;
        // playerTransform = pos;// vị trí mà player đang đứng
    }


    private void MoveToCanon()
    {
        // Lấy vị trí của con heo và súng cannon
        Vector2 targetPos = canonTransform.position;
        float d = Vector2.Distance(transform.position, targetPos);
        if (d > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            // Đảm bảo con heo sẽ xoay về hướng player khi detect player bằng true
            HandleFlip();

        }
        else
        {
            //đảm bảo con heo hướng về player trước khi châm ngòi nổ
            HandleFlip();
            MoveBackLittleBit(targetPos);
            animator.SetBool("isRunning", false);
            animator.SetTrigger("LightingMatch");

        }
    }

    //Mục đích dịch chuyển con heo lại 1 chút để đứng phía trước canon
    private void MoveBackLittleBit(Vector2 targetPos)
    {
        Vector2 moveLitleBit = new Vector2(targetPos.x - stoppingDistance, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, moveLitleBit, moveSpeed * Time.deltaTime);
    }

    
}
