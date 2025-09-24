using UnityEngine;

public class CanonPigDetector : MonoBehaviour
{

    private CanonPigController parentController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        parentController = GetComponentInParent<CanonPigController>();
        if (parentController == null)
        {
            Debug.LogError("Lỗi: Không tìm thấy CanonPigController trên đối tượng cha.");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {         
            parentController.hasDetectedPlayer = true;
            parentController.OnPlayerDetected(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {     
            parentController.OnPlayerExitRange();
        }
    }    
}
