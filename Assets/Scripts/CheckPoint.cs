using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerRespawn playerRespawn = collision.gameObject.GetComponent<PlayerRespawn>();
            if (playerRespawn != null)
                playerRespawn.SetCheckPoint(transform.position);
        }
    }
}
