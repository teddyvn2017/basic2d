using System.Collections;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{


    private Vector2 respawnPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody2D rb;

    [Header("Respawn Settings")]
    public float respawnHeight = 0.5f;
    public float fallDelay = 0.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        respawnPoint = transform.position;
    }

    // void Start()
    // {
    //     // Khi game bắt đầu, vị trí ban đầu của Player là respawn mặc định
    //     respawnPoint = transform.position;
    // }

    public void UpdateCheckPoint(Vector2 newPoint)
    {
        respawnPoint = newPoint;
    }

    // Hàm này được gọi khi chạm vào Checkpoint
    public void SetCheckPoint(Vector3 position)
    {
        respawnPoint = position;
        Debug.Log("Respawn point set to: " + position);
    }

    // Hàm này gọi khi Player chết
    public void RespawnPlayer()
    {
        // transform.position = respawnPoint;
        StartCoroutine(RespawnRoutine());
    }
    
    private IEnumerator RespawnRoutine()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        // Fade out (ẩn dần)
        float fadeOutTime = 0.5f;
        for (float t = 0; t < fadeOutTime; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / fadeOutTime);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            yield return null;
        }
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);

        // Delay thêm nếu muốn
        yield return new WaitForSeconds(fallDelay);

        // Reset vị trí Player về checkpoint
        transform.position = new Vector2(respawnPoint.x, respawnPoint.y + respawnHeight);
        rb.linearVelocity = Vector2.zero;

        // Fade in (hiện dần)
        float fadeInTime = 0.5f;
        for (float t = 0; t < fadeInTime; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, t / fadeInTime);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            yield return null;
        }
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
    }
}
