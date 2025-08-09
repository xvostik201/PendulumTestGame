using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public event Action<int> OnScoreUpdated;
    public event Action<int> OnHighscoreUpdated;

    [SerializeField] private float animationSpeed = 100f;

    private const string KeyHighscore = "HIGHSCORE";
    private const string KeyLastScore = "LAST_SCORE";

    private int   _targetScore;
    private float _displayedScoreF;
    private int   _displayedScoreInt;

    public int Highscore  { get; private set; }
    public int LastScore  { get; private set; }  
    public int TotalScore => _targetScore;        

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Highscore = PlayerPrefs.GetInt(KeyHighscore, 0);
        LastScore = PlayerPrefs.GetInt(KeyLastScore, 0);

        OnScoreUpdated?.Invoke(0);
        OnHighscoreUpdated?.Invoke(Highscore);
    }

    private void Update()
    {
        if (Mathf.Approximately(_displayedScoreF, _targetScore)) return;

        _displayedScoreF = Mathf.MoveTowards(
            _displayedScoreF,
            _targetScore,
            animationSpeed * Time.deltaTime
        );

        int rounded = Mathf.RoundToInt(_displayedScoreF);
        if (rounded != _displayedScoreInt)
        {
            _displayedScoreInt = rounded;
            OnScoreUpdated?.Invoke(_displayedScoreInt);
        }
    }

    public void AddScore(int amount)
    {
        _targetScore += amount;
        if (_targetScore > Highscore)
        {
            Highscore = _targetScore;
            PlayerPrefs.SetInt(KeyHighscore, Highscore);
            PlayerPrefs.Save();
            OnHighscoreUpdated?.Invoke(Highscore);
        }
    }

    public void CommitRun()
    {
        LastScore = _targetScore;
        PlayerPrefs.SetInt(KeyLastScore, LastScore);
        if (LastScore > Highscore)
        {
            Highscore = LastScore;
            PlayerPrefs.SetInt(KeyHighscore, Highscore);
            OnHighscoreUpdated?.Invoke(Highscore);
        }
        PlayerPrefs.Save();
    }

    public void ResetScore()
    {
        _targetScore = 0;
        _displayedScoreF = 0f;
        _displayedScoreInt = 0;
        OnScoreUpdated?.Invoke(0);
    }
}
