using UnityEngine;


//tự động thêm LineRenderer component cho bomb
[RequireComponent(typeof(LineRenderer))]
public class BombTrajectory : MonoBehaviour
{
    public int resolution = 1;              // Số điểm vẽ
    public float timeStep = 0.1f;            // Bước thời gian
    public Transform throwPoint;             // Vị trí ném
    public Vector2 throwForce;               // Vận tốc ban đầu
    public float gravityScale = 1f;         // Trùng với Gravity Scale của Rigidbody

    private LineRenderer line;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = resolution;
    }
    // Update is called once per frame
    void Update()
    {
        if (throwPoint == null) return;

        // DrawTrajectory();

    }

    void DrawTrajectory()
    {
        Vector2 startPos = throwPoint.position;
        Vector2 velocity = throwForce;

        float g = Physics2D.gravity.y * gravityScale;

        for (int i = 0; i < resolution; i++)
        {
            float t = i * timeStep;
            float x = startPos.x + velocity.x * t;
            float y = startPos.y + velocity.y * t + 0.5f * g * t * t;

            line.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}

/*
Cách tính quỹ đạo parabol
Vị trí tại thời điểm t:
x(t) = x0 + vx * t
y(t) = y0 + vy * t + 0.5 * g * t^2

trong đó 
(x0, y0) = ThrowPoint.position
(vx, vy) = vận tốc ban đầu (lực ném)
g = Gravity (Physics2D.gravity.y * rb.gravityScale)
trọng lực

*/
