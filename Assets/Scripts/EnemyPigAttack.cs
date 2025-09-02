using System.Collections;
using UnityEngine;

public class EnemyPigAttack : MonoBehaviour
{
    // public Transform attackPoint;

    
    [Header("Attack Settings")]
    public int attackDamage = 1;
    public float attackRange = 0.5f;  // khoảng cách để tấn công, tầm đánh (mở rộng xung quanh attackPoint)
    public float attackCooldown = 1.5f; // Thời gian giữa các đòn tần công
    public Collider2D attackCollider;

    [HideInInspector]
    public bool isAttacking = false;
    public bool playerInRange = false;


    [Header("References")]
    public Animator animator; // Animator của enemy - player
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && !isAttacking)
        {            
            StartCoroutine(AttackLoop());
        }
    }


    private IEnumerator AttackLoop()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        //Lưu ý: collider sẽ được bật/tắt qua Animation Event:
        // EnableAttackCollider() / DisableAttackCollider()        
        yield return new WaitForSeconds(attackCooldown);        
        isAttacking = false;
    }

    public void AttackPlayer(Collider2D other)
    {

        if (attackCollider == null || !attackCollider.enabled) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        PlayerController playerController = other.GetComponent<PlayerController>();

        if (playerHealth != null)
            playerHealth.TakeDamage(attackDamage);

        if (playerController != null)
            playerController.KnockBack(transform);       
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player đã vào phạm vi tấn công!");
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player đã rời khỏi phạm vi tấn công!");
            playerInRange = false;
        }
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && attackCollider.enabled)
        {
            AttackPlayer(collision);
        }
    }

    public void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }

    public void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }
}