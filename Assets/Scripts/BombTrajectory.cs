using UnityEngine;

// Lớp tính đường đi cho quả bom
public class BombTrajectory : MonoBehaviour
{
    public float timeToTarget = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public Vector2 CalculateThrow(Vector2 target, Vector2 start)
    {
        Vector2 distance = target - start;
        Vector2 gravity = Physics2D.gravity;

        float vx = distance.x / timeToTarget;
        float vy = (distance.y - 0.5f * gravity.y * timeToTarget * timeToTarget) / timeToTarget;

        return new Vector2(vx, vy);
    }
}

// Kiến thức
//Trong cơ học, tọa độ theo trục Y của một vật ném xiên được tính theo công thức
//y = y0 + v0 * t + 0.5 * g * t^2
//trong đó
//y0 = y điểm bắt đầu ném
//v0 = vận tốc ban đầu
//g = Gravity (Physics2D.gravity.y * rb.gravityScale) trọng lực
//t = thời gian ném
//y(t)=y0​+vy​⋅t+1/2​gt^2
//target.y=start.y+vy​⋅timeToTarget+1/2​g⋅(timeToTarget)^2

