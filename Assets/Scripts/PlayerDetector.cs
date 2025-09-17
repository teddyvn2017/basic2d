using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private BombPigParabolicController parentController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Lấy tham chiếu đến script của đối tượng cha BombPigParabolicController
        parentController = GetComponentInParent<BombPigParabolicController>();

    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            parentController.hasDetectedPlayer = true;
        }
    }
}
