using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private BombPigParabolicController parentController;
    void Start()
    {
        // Lấy tham chiếu đến script của đối tượng cha BombPigParabolicController
        parentController = GetComponentInParent<BombPigParabolicController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parentController.hasDetectedPlayer = true;
            parentController.OnPlayerDetected(other.transform);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("Player stay in range!");
            parentController.hasDetectedPlayer = true;
            parentController.OnPlayerDetected(other.transform);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("Player exit range!");    
            parentController.hasDetectedPlayer = false;
        }
    }
}