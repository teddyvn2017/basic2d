using UnityEngine;

public class Bomb : MonoBehaviour
{

    [Header("Explosion Settings")]
    // public float delay = 2f;                // Thời gian nổ sau khi ném
    // public float explosionRadius = 2f;      // Bán kính nổ
    // public int damage = 1;                 // Sát thương
    // public GameObject explosionEffect;      // Prefab hiệu ứng nổ

    public float lifeTime = 3f;

    private Rigidbody2D rb;
    private bool hasExploded = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);

    }

    // Update is called once per frame
    // void Update()
    // {

    // }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("💥 Bom đã nổ khi chạm đất!");
            Destroy(gameObject);
        }
    }
}
