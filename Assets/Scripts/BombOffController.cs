using UnityEngine;

public class BombOffController : MonoBehaviour
{

    public GameObject explosionPrefab;   

    private void OnTriggerEnter2D(Collider2D other)
    {
        Vector2 bombPosition = transform.position;

        if (other.gameObject.CompareTag("Ground"))
        {            
            Vector2 newPos = new Vector2(bombPosition.x, bombPosition.y);
            ExplodeAt(newPos);
        }

        if (other.gameObject.CompareTag("Player"))
        {         
            bombPosition = other.transform.position;
            ExplodeAt(bombPosition);
        }     
    }

    private void ExplodeAt(Vector2 position)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();        
        rb.linearVelocity = Vector2.zero; // tạm ngưng quả bom di chuyển tiếp do trọng lực
        // Sinh ra hiệu ứng nổ tại vị trí bom
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
            Animator expAnimator = explosion.GetComponent<Animator>();
            if (expAnimator != null)
                expAnimator.SetTrigger("Exploding");

            Destroy(explosion, 0.2f); // xóa hiệu ứng nổ
        }

        // Xóa bom sau khi nổ        
        Destroy(gameObject);
    }
        
}