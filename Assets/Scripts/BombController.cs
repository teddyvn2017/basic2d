using UnityEditor.Callbacks;
using UnityEngine;

public class BombController : MonoBehaviour
{

    [Header("Explosion Settings")]
    // public float delay = 2f;                // Thời gian nổ sau khi ném
    // public float explosionRadius = 2f;      // Bán kính nổ
    // public int damage = 1;                 // Sát thương
    // public GameObject explosionEffect;      // Prefab hiệu ứng nổ

    public float lifeTime = 3f;
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
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Vector2 bombPosition = transform.position;

        if (other.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Bomb to ground");
            // Use a Raycast to find the exact point on the ground
            Vector2 newPos = new Vector2(bombPosition.x, bombPosition.y + 1f);
            //ExplodeAt(newPos);
        }
        if (other.gameObject.CompareTag("Player"))
        {
            bombPosition = other.transform.position;
            // ExplodeAt(bombPosition);
        }        
    }
    

    // private void Explode()
    // {
    //     if (hasExploded) return;

    //     hasExploded = true;
    //     Animator animator = GetComponent<Animator>();

    //     animator.SetTrigger("Exploding");

    //     if (explosionPrefab != null)
    //     {
    //         GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);          
    //         Destroy(explosion, 1f);
    //     }
    // }
    
    
    private void ExplodeAt(Vector2 position)
    {
        if (hasExploded) return;
        hasExploded = true;

        Animator animator = GetComponent<Animator>();

        // Gọi animation
        if (animator != null)    
            animator.SetTrigger("Exploding");
            
        // Sinh ra hiệu ứng nổ tại vị trí bom
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
            Destroy(explosion, 1f); // xóa hiệu ứng nổ
        }

        // Xóa bom sau khi nổ
        Destroy(gameObject, 0.3f); // xóa bom prefab
    }

    

}
