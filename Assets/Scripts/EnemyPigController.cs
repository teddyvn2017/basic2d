using System.Collections;
using UnityEngine;

public class EnemyPigController : BaseEnemyController
{
    // khi khai báo public, biến sẽ hiện ra trong Inspector của Unity.
    // chỉnh sửa giá trị trực tiếp trong Unity mà không cần mở code.
    // public float moveSpeed = 1.2f;
    // private Rigidbody2D rb;
    // private Animator animator;
    // private bool isFacingRight = false;
    // private bool isMoving = true;

    [Header("Ground Check")]
    public Transform groundCheck;          // Empty Object dưới chân Enemy
    public float groundCheckRadius = 0.3f; // bán kính kiểm tra
    public LayerMask groundLayer;
    // public LayerMask playerLayer;

    public float moveDuration = 4f;  // thời gian chạy
    public float idleDuration = 1.5f;  // thời gian nghỉ


    [Header("References")]
    public Transform player; // gán Player trong Inspector 

    // [Header("Knockback Settings")]
    // public float knockbackForce = 1.5f;
    // public float knockbackUpForce = 2f;
    // public float knockbackDuration = 0.4f;
    // private float knockbackTimer;


    [Header("Attack Settings")]
    
    // public float attackCooldown = 2f; // thời gian chờ giữa 2 lần tấn công
    private float attackTimer = 0f;

    

    // [HideInInspector] public bool isKnockBack = false;    

    private EnemyPigAttack enemyPigAttack;
    // private bool playerInRange = false;

    protected override void Start()
    {
        base.Start();
        enemyPigAttack = GetComponent<EnemyPigAttack>();
        StartCoroutine(MoveRoutine());
    }
    
    protected override void Update()
    {

        if (isKnockBack) return;

        if (enemyPigAttack.playerInRange)
        {            
            StopMovement();
            HandleFlip();
        }
        else 
        {
            if (isMoving)
                base.Move();
            CheckGroundAhead();
        }
    }

    void CheckGroundAhead()
    {
        // Kiểm tra có mặt đất ở phía trước
        Vector2 checkPos = groundCheck.position;
        bool noGroundAhead = !Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);

        // Debug.Log("noGroundAhead: " + noGroundAhead);
        if (noGroundAhead)
            Flip();
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
        //transform là của enemy
        if (player == null) return;

        if (player.position.x > transform.position.x && !isFacingRight)
            Flip();
        else if (player.position.x < transform.position.x && isFacingRight)
            Flip();
    }    
    
}
