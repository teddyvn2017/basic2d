using UnityEngine;

public class BombPigParabolicController : BaseEnemyController
{
    [Header("Explosion Settings")]
    // public float delay = 2f;                // Thời gian nổ sau khi ném
    // public float explosionRadius = 2f;      // Bán kính nổ
    // public int damage = 1;                 // Sát thương
    // public GameObject explosionEffect;      // Prefab hiệu ứng nổ

    public float lifeTime = 5f;

    //Dùng để lưu trữ vị trí ném bom
    public Transform throwPoint;

    // Tốc độ ném bom
    public float throwSpeed = 5f;

    // Dùng để lưu trữ prefab của quả bom
    public GameObject bombPrefab;

    public float timeToTarget = 3f; // bom bay trong 1 giây
    // Biến trạng thái để kiểm tra xem người chơi có trong vùng không
    
    // Dùng để ngăn ném bom liên tục
    private float lastThrowTime;
    public float throwCooldown = 2f;

    public int resolution = 30;
    
    public Transform playerTransform;

    [SerializeField] private Vector2 startPoint = new Vector2(0.8f, -0.2f);
    [SerializeField] private Vector2 endPoint = new Vector2(4.48f, 0.96f);
    // [SerializeField] private float timeToTarget = 1.2f;

    private Vector2 lastKnownPlayerPos;
    private bool hasDetectedPlayer = false;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        // if (hasDetectedPlayer && Time.time > lastThrowTime + throwCooldown)
        // {

        //     Debug.Log("Player is detected by enemy bomb pig !");
        //     lastThrowTime = Time.time;
        //     ThrowBomb(lastKnownPlayerPos);        
        // }
        if (hasDetectedPlayer)
        {
            // Cập nhật vị trí của người chơi liên tục khi đã phát hiện
            lastKnownPlayerPos = playerTransform.position;
            // cách bên dưới là không chính xác
            // lastKnownPlayerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            // Debug.Log("lastKnownPlayerPos: " + lastKnownPlayerPos.x + " " + lastKnownPlayerPos.y);
            if (Time.time > lastThrowTime + throwCooldown)
            {
                Debug.Log("Player is detected by enemy bomb pig!");
                lastThrowTime = Time.time;
                ThrowBomb(lastKnownPlayerPos);
            }
        }
    }

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Ground"))
    //     {
    //         Debug.Log("💥 Bom đã nổ khi chạm đất!");
    //         Destroy(gameObject);
    //     }
    // }

    private void ThrowBomb(Vector2 targetPos)
    {
        // Debug.Log("ThrowBomb");
        if (bombPrefab == null || throwPoint == null) return;

        // Tạo ra một instance của quả bom tại vị trí của ThrowPoint
        GameObject bombInstance = Instantiate(bombPrefab, throwPoint.position, Quaternion.identity);
        // Hủy quả bom sau lifeTime giây để tránh tạo ra quá nhiều instance
        Destroy(bombInstance, lifeTime);

        // Lấy Rigidbody2D của quả bom để ném nó
        Rigidbody2D rb = bombInstance.GetComponent<Rigidbody2D>();

        // Ném quả bom
        Vector2 throwVelocity = CalculateThrow(targetPos, throwPoint.position);
        // Debug.Log($"Distance: {target - start}, Velocity: {v}");
        rb.linearVelocity = throwVelocity;
        // Debug.Log("Throw bomb: X=" + throwVelocity.x + " Y=" + throwVelocity.y);
    }
    
   
   
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other != null && other.CompareTag("Player"))
        {

            // lastKnownPlayerPos = other.transform.position;
            hasDetectedPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // hasDetectedPlayer = false;
            lastKnownPlayerPos = Vector2.zero;
            // Debug.Log("isPlayerInDetectZone: " + isPlayerInDetectZone);
            //playerTransform = null;
        }
    }

    // Vẽ quỹ đạo trong Scene view
    private void OnDrawGizmos()
    {
        // Tính vận tốc ban đầu
        Vector2 velocity = CalculateThrow(endPoint, startPoint);
        Vector2 currentPos = startPoint;

        Gizmos.color = Color.red;
        Vector2 gravity = Physics2D.gravity;

        int steps = 30;
        float timestep = timeToTarget / steps;

        for (int i = 0; i < steps; i++)
        {
            Vector2 nextPos = currentPos + velocity * timestep + 0.5f * gravity * timestep * timestep;
            velocity += gravity * timestep;

            Gizmos.DrawLine(currentPos, nextPos);
            currentPos = nextPos;
        }

        // Vẽ chấm xanh tại điểm bắt đầu và kết thúc
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPoint, 0.05f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(endPoint, 0.05f);
    }

    /// <summary>
    /// Tính vận tốc ban đầu để ném vật từ start -> target trong đúng timeToTarget giây.
    /// </summary>
    private Vector2 CalculateThrow(Vector2 target, Vector2 start)
    {
        Vector2 distance = target - start;
        Vector2 gravity = Physics2D.gravity;

        float vx = distance.x / timeToTarget;
        float vy = (distance.y - 0.5f * gravity.y * timeToTarget * timeToTarget) / timeToTarget;

        return new Vector2(vx, vy);
    }


}

// Kiến thức
//Trong cơ học, tọa độ theo trục Y của một vật ném xiên được tính theo công thức
//y = y0 + v0 * t + 0.5 * g * t^2
//trong đó
//y0 = y điểm bắt đầu ném
//v0 = vận tốc ban đầu
//g = Gravity (Physics2D.gravity.y * rb.gravityScale) trọng lực
//t = thời gian ném
//y(t)=y0​+vy​⋅t+1/2​gt^2
//target.y=start.y+vy​⋅timeToTarget+1/2​g⋅(timeToTarget)^2
