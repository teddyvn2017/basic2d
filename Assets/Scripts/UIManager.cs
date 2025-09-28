using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Health UI")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Score UI")]
    public Text scoreText;

    [Header("Lives UI")]
    public Text livesText;

    private int currentScore = 0;
    private int currentLives = 3;
    // Cập nhật hearts dựa trên currentHealth
    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            // tim nào có chỉ số >= currentHealth thì ẩn
            if (i < currentHealth)
                hearts[i].gameObject.SetActive(true);  // hiện tim
            else
                hearts[i].gameObject.SetActive(false); // ẩn tim
        }
    }

    // Cập nhật điểm số
    public void AddScore(int amount)
    {
        currentScore += amount;
        scoreText.text = "Score: " + currentScore;
    }

    // Đặt điểm trực tiếp
    public void SetScore(int value)
    {
        currentScore = value;
        scoreText.text = "Score: " + currentScore;
    }

    // Cập nhật mạng
    public void SetLives(int value)
    {
        currentLives = value;
        livesText.text = "Lives: " + currentLives;
    }

}
