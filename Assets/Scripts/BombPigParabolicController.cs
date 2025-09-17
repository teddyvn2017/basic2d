using System.Collections;
using UnityEngine;

public class BombPigParabolicController : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float lifeTime = 2f;
    public Transform throwPoint;
    public float throwSpeed = 5f;
    public GameObject bombPrefab;
    public float timeToTarget = 3f;

    [Header("Cooldown Settings")]
    public float throwCooldown = 3f;
    private float lastThrowTime;

    private Transform playerTransform;
    private Vector2 lastKnownPlayerPos;
    public bool hasDetectedPlayer = false;
    private bool canThrow = true;

    void Start()
    {
        lastThrowTime = -throwCooldown; // Cho phép ném ngay lần đầu
    }

    void Update()
    {

        // Debug.Log("hasDetectedPlayer: " + hasDetectedPlayer + "  canThrow: " + canThrow + "  playerTransform: " + playerTransform);

        if (hasDetectedPlayer && canThrow)
        {
            StartCoroutine(ThrowWithDelay());
        }
        // if (hasDetectedPlayer && playerTransform != null)
        // {
        //     lastKnownPlayerPos = playerTransform.position;

        //     float remainingCooldown = (lastThrowTime + throwCooldown) - Time.time;

        //     if (remainingCooldown > 0f)
        //     {
        //         // Luôn log countdown cho đến đúng 0.00
        //         // Debug.Log("Đang chờ cooldown... còn " + Mathf.Max(remainingCooldown, 0f).ToString("F2") + "s");
        //         // Debug.Log("Đang chờ cooldown... còn " + remainingCooldown.ToString("F2") + "s");
        //     }
        //     else
        //     {
        //         Debug.Log("Ném bom!");
        //         lastThrowTime = Time.time;
        //         // ThrowBomb(lastKnownPlayerPos);
        //     }
        // }
    }


    IEnumerator ThrowWithDelay()
    {
        canThrow = false;
        Debug.Log("Ném bom!");
        // ThrowBomb(lastKnownPlayerPos);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            lastKnownPlayerPos = other.transform.position;
            hasDetectedPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = null;
            lastKnownPlayerPos = Vector2.zero;
            hasDetectedPlayer = false;
        }
    }

    // private void OnTriggerStay2D(Collider2D other)
    // {
    //     Debug.Log("OnTriggerStay2D is called!");
    //     // hasDetectedPlayer = true;
    //     if (other.CompareTag("Player"))
    //     {
    //         // Debug.Log("vẫn còn trong vùng!");
    //         // Debug.Log("playerTransform: " + playerTransform);
    //         Debug.Log("compare true nằm trong vùng!");
    //         hasDetectedPlayer = true;
    //         playerTransform = other.transform;
    //         lastKnownPlayerPos = other.transform.position;
    //     }
    // }

    public void OnPlayerDetected(Transform pos)
    {
        hasDetectedPlayer = true;
        // playerTransform = playerPos;
        // lastKnownPlayerPos = new Vector2(pos.position.x, pos.position.y); // playerpos;
        playerTransform = pos;
    }
}
