using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    #region Instance Manager

    /// <summary>
    /// The one and only instance
    /// </summary>
    public static ScoreManager instance;

    #endregion
    #region Serialized Fields

    /// <summary>
    /// Banner used to show the score to the player
    /// </summary>
    [SerializeField] private TextMeshProUGUI ScoreText;

    /// <summary>
    /// Banner used to show the high score to the player
    /// </summary>
    [SerializeField] private TextMeshProUGUI HighScoreText;

    #endregion
    #region Fields

    /// <summary>
    /// Keep track of the player's score
    /// </summary>
    private int score = 0;

    /// <summary>
    /// Keep track of the player's highest score
    /// </summary>
    private int highScore = 0;

    #endregion
    #region Constants

    /// <summary>
    /// Tag to use for saving the player's high score in preferences
    /// </summary>
    private const string HighScoreTag = "HighScore";

    #endregion

    /// <summary>
    /// Awake is called before start
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        highScore = PlayerPrefs.GetInt(HighScoreTag, 0);
        DisplayScore();
    }

    /// <summary>
    /// Add points to the player's score
    /// </summary>
    public void AddPoint()
    {
        score++;
        if (score >= highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreTag, highScore);
        }
        DisplayScore();
    }

    /// <summary>
    /// Reset the player's score
    /// </summary>
    public void ResetScore()
    {
        score = 0;
        DisplayScore();
    }


    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey(HighScoreTag);
        highScore = 0;
        DisplayScore();
    }

    /// <summary>
    /// Display the score to the player
    /// </summary>
    private void DisplayScore()
    {
        ScoreText.text = score.ToString();
        HighScoreText.text = highScore.ToString();
    }
}
