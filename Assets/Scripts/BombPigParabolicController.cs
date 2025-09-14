using UnityEngine;

public class BombPigParabolicController : BaseEnemyController
{
    [Header("Explosion Settings")]
    // public float delay = 2f;                // Thời gian nổ sau khi ném
    // public float explosionRadius = 2f;      // Bán kính nổ
    // public int damage = 1;                 // Sát thương
    // public GameObject explosionEffect;      // Prefab hiệu ứng nổ

    public float lifeTime = 2f;

    //Dùng để lưu trữ vị trí ném bom
    public Transform throwPoint;

    // Tốc độ ném bom
    public float throwSpeed = 5f;

    // Dùng để lưu trữ prefab của quả bom
    public GameObject bombPrefab;

    public float timeToTarget = 1f; // bom bay trong 1 giây
                                    // Biến trạng thái để kiểm tra xem người chơi có trong vùng không
    
    private float lastThrowTime;
    public float throwCooldown = 3f;
    public Transform playerTransform;
    private Vector2 lastKnownPlayerPos;
    private bool hasDetectedPlayer = false;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (hasDetectedPlayer)
        {
            // Cập nhật vị trí của người chơi liên tục khi đã phát hiện
            lastKnownPlayerPos = playerTransform.position;            
            if (Time.time > lastThrowTime + throwCooldown)
            {
                lastThrowTime = Time.time;
                ThrowBomb(lastKnownPlayerPos);
            }
        }
    }
    private void ThrowBomb(Vector2 targetPos)
    {
        // Debug.Log("ThrowBomb");
        if (bombPrefab == null || throwPoint == null) return;

        // Tạo ra một instance của quả bom tại vị trí của ThrowPoint
        GameObject bombInstance = Instantiate(bombPrefab, throwPoint.position, Quaternion.identity);
        // Hủy quả bom sau lifeTime giây để tránh tạo ra quá nhiều instance
        Destroy(bombInstance, lifeTime);

        //Lấy animation có ngòi nổ
        Animator anim = bombInstance.GetComponent<Animator>();
        anim.Play("Bomb_Fuse");

        // Lấy Rigidbody2D của quả bom để ném nó
        Rigidbody2D rb = bombInstance.GetComponent<Rigidbody2D>();

        // Ném quả bom
        Vector2 throwVelocity = CalculateThrow(targetPos, throwPoint.position);

        rb.linearVelocity = throwVelocity;
        rb.angularVelocity = -360f;

    }

    public Vector2 CalculateThrow(Vector2 target, Vector2 start)
    {
        Vector2 distance = target - start;
        Vector2 gravity = Physics2D.gravity;

        float vx = distance.x / timeToTarget;
        float vy = (distance.y - 0.5f * gravity.y * timeToTarget * timeToTarget) / timeToTarget;

        return new Vector2(vx, vy);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other != null && other.CompareTag("Player"))
        {

            // lastKnownPlayerPos = playerTransform.position;
            Debug.Log("Player is in range !");
            // ThrowBomb(lastKnownPlayerPos);
            hasDetectedPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // lastKnownPlayerPos = Vector2.zero;
            hasDetectedPlayer = false;
            Debug.Log("Player out of range");
        }
    }
}