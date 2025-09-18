using UnityEngine;

public class BombPigController : BaseEnemyController
{
    // private Animator anim;
    private bool playerIsDetected = false;

    protected override void Start()
    {
        base.Start();        
    }

    protected override void Update()
    {
        base.Update();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {   
        
        if (other != null && other.CompareTag("Player"))
        {
            playerIsDetected = true;
            Debug.Log("Player is detected by enemy bomb pig !");
        }
    }
}
