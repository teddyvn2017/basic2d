using UnityEditor.Callbacks;
using UnityEngine;

public class BombController : MonoBehaviour
{   
    // public float lifeTime = 3f;
    public GameObject explosionPrefab; // Prefab hiệu ứng nổ (BombExplosion)
    // private Rigidbody2D rb;
    private bool hasExploded = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // rb = GetComponent<Rigidbody2D>();
        //Destroy(gameObject, lifeTime);


    }

    // Update is called once per frame
    // void Update()
    // {

    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Vector2 bombPosition = transform.position;

        // if (other.gameObject.CompareTag("Ground"))
        // {            
        //     Vector2 newPos = new Vector2(bombPosition.x, bombPosition.y);            
        //     ExplodeAt(newPos);
        // }
        // if (other.gameObject.CompareTag("Player"))
        // {
        //     bombPosition = other.transform.position;
        //     ExplodeAt(bombPosition);
        // }        
    }
        
    private void ExplodeAt(Vector2 position)
    {
        if (hasExploded) return;
        hasExploded = true;

        Animator animator = GetComponent<Animator>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero; // tạm ngưng quả bom di chuyển tiếp do trọng lực
        Vector2 pos = new Vector2(position.x, position.y);

        // Gọi animation
        if (animator != null)
            animator.SetTrigger("Exploding");
            
        // Sinh ra hiệu ứng nổ tại vị trí bom
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, pos, Quaternion.identity);
            Destroy(explosion, 1f); // xóa hiệu ứng nổ
        }

        // Xóa bom sau khi nổ
        Destroy(gameObject, 0.3f); // xóa bom prefab
    }
}
