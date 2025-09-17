using System.Collections;
using UnityEngine;

public class BombPigParabolicController : BaseEnemyController
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

    protected override void Start()
    {
        base.Start();
        // EnemyState = EnemyState.Patrol;
        lastThrowTime = Time.time;
    }

    protected override void Update()
    {

        base.Update();
        // Debug.Log("hasDetectedPlayer: " + hasDetectedPlayer);
        if (hasDetectedPlayer && canThrow)
        {
            Attack();
        }
        //  Debug.Log("hasDetectedPlayer: " + hasDetectedPlayer);

            // if (hasDetectedPlayer && canThrow)
            // {
            //     StartCoroutine(ThrowWithDelay());
            // }
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
    protected override void Attack()
    {
        // Quay đầu về phía người chơi 
        Vector2 directionToPlayer = lastKnownPlayerPos - (Vector2)transform.position;
        if (directionToPlayer.x > 0 && !isFacingRight)
            Flip();
        else if (directionToPlayer.x < 0 && isFacingRight) Flip();
        // Debug.Log("lastThrowTime: " + lastThrowTime + " throwCooldown: " + throwCooldown);
        // if (Time.time >= lastThrowTime + throwCooldown)
        // {
        //     Debug.Log("NÉM BOM vào " + playerTransform.position);
        //     lastThrowTime = Time.time;
        //     // ThrowBomb(playerTarget.position); //  
        // }

        StartCoroutine(ThrowWithDelay());
    }

    IEnumerator ThrowWithDelay()
    {
        canThrow = false;
        Debug.Log("Ném bom!"+ playerTransform.position.x + " " + playerTransform.position.y);
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
        // Change
        playerTransform = pos;
        Debug.Log("Player detected!");  

    }
}
