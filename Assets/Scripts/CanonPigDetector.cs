using UnityEngine;

public class CanonPigDetector : MonoBehaviour
{

    private CanonPigController parentController =  new CanonPigController();
    // Start is called once before the first execution of Update after the MonoBehaviour is created


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
