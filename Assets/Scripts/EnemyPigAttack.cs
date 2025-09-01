using UnityEngine;

public class EnemyPigAttack : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.5f;
    //phải nhớ chọn layer Player ở unity inspector
    public LayerMask playerLayer;
    public int attackDamage = 1;

    [HideInInspector] public bool isAttacking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    // void Update()
    // {

    // }

    public void AttackPlayer()
    {

        Debug.Log("AttackPlayer");
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);
        if (hitPlayer != null)
        {
            PlayerHealth playerHealth = hitPlayer.GetComponent<PlayerHealth>();
            PlayerController playerController = hitPlayer.GetComponent<PlayerController>();

            if (playerHealth != null)       
                playerHealth.TakeDamage(attackDamage);
            
            if (playerController != null)            
                playerController.KnockBack(transform);
                // Debug.Log("playerController: " + playerController);
            
        }
    }

    //kết thúc tấn công
    public void FinishAttack()
    {
        isAttacking = false;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}
