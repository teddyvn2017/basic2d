using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private float speed = 5.0f; // Tốc độ chạy ngang
    public float jumpForce = 5f; // Lực nhảy
    private Rigidbody2D rb;

    //biến ground check
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    private bool isGrounded;
    public float checkRadius = 0.2f;

    public LayerMask groundLayer;
    // private bool isKnocked = false;

    [Header("Knockback Settings")]
    public float knockbackForce = 1f;
    public float knockbackDuration = 0.4f;
    public float knockbackUpForce = 1f;

    [HideInInspector] public bool isKnockBack = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //khi knockback thì tạm thời không cho về trạng thái idle
        if (isKnockBack) return;

        // Đảm bảo rotation của trục Z luôn bằng 0
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Kiểm tra có đang chạm đất không
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        float horizontalInput = Input.GetAxis("Horizontal");
        bool isRunning = (horizontalInput != 0);

        // Cập nhật biến isRunning trong Animator
        animator.SetBool("isRunning", isRunning);

        // Di chuyển nhân vật
        Vector2 movement = new Vector2(horizontalInput, 0);
        rb.linearVelocity = new Vector2(movement.x * speed, rb.linearVelocity.y);

        // Nhảy khi bấm Space và đang chạm đất
        if (isGrounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("isJumping");
        }

        // Tấn công khi bấm Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.ResetTrigger("isAttacking");
            animator.SetTrigger("isAttacking");
        }

        // Lật hướng nhân vật
        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // knockback by enemy
    public void KnockBack(Transform attacker)
    {
        // Debug.Log("KnockBack");
        if (isKnockBack) return;

        isKnockBack = true;
        rb.linearVelocity = Vector2.zero;

        animator.SetTrigger("Hit");
        // animator.SetBool("isRunning", false);

        // Tính hướng knockback: player bên trái enemy → hất sang trái (-1), bên phải → hất sang phải (+1)
        float horizontalDir = (transform.position.x < attacker.position.x) ? -1f : 1f;

        // Cộng lực theo hướng enemy
        // Add lực bật ngược

        Vector2 knockbackDir = new Vector2(horizontalDir * knockbackForce, knockbackUpForce);
        // Debug.Log("knockbackDir x: " + knockbackDir.x);
        rb.AddForce(knockbackDir, ForceMode2D.Impulse);

        // Debug.Log("Knockback: X=" + horizontalDir * knockbackForce + "  Y=" + knockbackUpForce);
        // rb.AddForce(new Vector2(horizontalDir * knockbackForce, knockbackUpForce), ForceMode2D.Impulse);

        Invoke(nameof(EndKnockback), knockbackDuration);
    }
    
    private void EndKnockback()
    {
        isKnockBack = false;
    }

}
