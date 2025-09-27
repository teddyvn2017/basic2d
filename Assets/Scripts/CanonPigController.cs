using System.Collections;
using System; // Cần thêm namespace này để sử dụng Math.Round
using UnityEngine;
// using System.Numerics;

public class CanonPigController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private bool isFacingRight = true;
    private bool isMoving = true;

    public bool hasDetectedPlayer = false;
    // private bool canShoot = true;
    // private bool hasReachedCanon = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.6f;

    [Header("Shoot Settings")]
    public Transform shootPoint;
    // public float throwSpeed = 5f;
    public GameObject bombOffPrefab;
    public float timeBetweenShots = 1.5f;
    public float bombSpeed = 3f;
    // public float timeToTarget = 3f;

    [Header("Movement Routine Settings")]
    public float moveSpeed = 1f;
    public float stoppingDistance = 0.3f;
    private float randomMoveDuration;
    private float randomIdleDuration;

    [Header("References")]
    public Transform player; //dùng để so sánh vị trí cho hàm HandleFlip()
    public Transform canonTransform;
    public Transform canonDestination;
    // public GameObject muzzleExplosionPrefab;
    public Animator canonAnimator;//gán canon trong Inspector

    private bool hasStartedLighting = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isMoving = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("isRunning", true);
        // rb.freezeRotation = true;
        StartCoroutine(MoveRoutine());
        randomMoveDuration = UnityEngine.Random.Range(2f, 4f);
        randomIdleDuration = UnityEngine.Random.Range(1f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (hasDetectedPlayer)
        {
            moveSpeed = 1.5f;
            HandleFlip();
            MoveToCanon();
        }
        else
        {            
            moveSpeed = 1f;
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
            // animator.SetBool("isRunning", true);
            yield return new WaitForSeconds(randomMoveDuration);
            // nghỉ
            isMoving = false;
            // animator.SetBool("isRunning", false);
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
    }

    public void OnPlayerExitRange()
    {
        // Đặt lại các biến trạng thái
        hasDetectedPlayer = false;   
        hasStartedLighting = false;
    }

    private void MoveToCanon()
    {
        // Lấy vị trí của con heo và súng cannon
        Vector2 targetPos = canonTransform.position;
        float d = Vector2.Distance(transform.position, targetPos);
        d = (float)Math.Round(d, 2);
        if (d > stoppingDistance)
        {
            // Debug.Log("di chuyển tiếp d = " + d);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            HandleFlip();
        }
        //canh con heo ở khoảng cách vừa phải để đứng trước canon,đảm bảo lệnh else được chạy        
        else if (Mathf.Abs(d - stoppingDistance) < 0.1f)
        // else
        {
            // Debug.Log("Bắn liên tục");
            HandleFlip();   //đảm bảo con heo hướng về player trước khi châm ngòi nổ
            animator.SetBool("isRunning", false);
            // Lưu ý : không thể gọi liên tiếp 2 animation clip liên tiếp
            // do đó phải sử dụng coroutine, animation không hoạt động theo kiểu tuần tự 
            // animator.SetTrigger("MatchingOn");
            // animator.SetTrigger("LightingToCanon");           

            if (!hasStartedLighting)
            {
                StartCoroutine(LightCanonRoutine());
                hasStartedLighting = true;
            }
        }
    }

    private IEnumerator LightCanonRoutine()
    {
        // 1. Pig quẹt diêm
        animator.SetTrigger("MatchingOn");
        yield return new WaitForSeconds(1f);

        // 2. Bắt đầu bắn liên tục (canon animation + spawn bomb)
        StartCoroutine(CanonFireRoutine());
    }

    //animation của súng canon
    private IEnumerator CanonFireRoutine()
    {
        while (hasDetectedPlayer) // chỉ bắn khi player còn trong vùng
        {

            canonAnimator.SetTrigger("Fire"); // animation canon bắn
            ShootBomb();                       // spawn bomb
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }
    private void ShootBomb()
    {
        if (bombOffPrefab != null && shootPoint != null)
        {                   
            //tạo đối tượng quả bom ở vị trí đã khai báo sẵn ở inspector            
            GameObject bo = Instantiate(bombOffPrefab, shootPoint.position, Quaternion.identity);

            Rigidbody2D rb = bo.GetComponent<Rigidbody2D>();
            // --- Tính toán hướng bắn ---
            Vector2 target = player.position;
            Vector2 origin = shootPoint.position;
         
            // --- Tính lực bắn ---
            float angle = 45f * Mathf.Deg2Rad; // góc bắn
            float distance = Vector2.Distance(origin, target); // khoảng cách bắn
                                                               // Công thức đơn giản: v = sqrt(d * g / sin(2θ))
            float g = Physics2D.gravity.magnitude;
            float velocity = Mathf.Sqrt(distance * g / Mathf.Sin(2 * angle));

            // Vector vận tốc ban đầu theo góc
            Vector2 launchVelocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * velocity;
            // Đảm bảo bom bay theo hướng player (trái/phải)
            if (target.x < origin.x) launchVelocity.x = -launchVelocity.x;

            // --- Thực hiện bắn ---
            rb.AddForce(launchVelocity, ForceMode2D.Impulse);

            Destroy(bo, 1.5f);
        }
    }    
}