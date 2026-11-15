using UnityEngine;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    private static int coins;
    private static Text scoreText;

    public static void RegisterScoreText(Text value)
    {
        scoreText = value;
        UpdateScore();
    }

    public static void AddCoin()
    {
        coins++;
        UpdateScore();
    }

    private static void UpdateScore()
    {
        if (scoreText != null) scoreText.text = "COINS  " + coins + " / 8";
    }

    public static void ResetSession()
    {
        coins = 0;
        UpdateScore();
    }
}
