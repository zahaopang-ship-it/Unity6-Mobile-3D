using UnityEngine;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    private static int coins;
    [SerializeField] private Text scoreText;

    private void Awake()
    {
        coins = 0;
        UpdateScore();
    }

    public void SetScoreText(Text value)
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
        if (Instance != null && Instance.scoreText != null)
            Instance.scoreText.text = "COINS  " + coins + " / 8";
    }

    private static GameSession Instance;

    private void OnEnable() { Instance = this; }
    private void OnDisable() { if (Instance == this) Instance = null; }
}
