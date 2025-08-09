using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    private void UpdateScoreText(int score)
    {
        scoreText.text = "Счет: " + score;
    }

    private void OnEnable()
    {
        ScoreManager.Instance.OnScoreUpdated += UpdateScoreText;
    }

    private void OnDisable()
    {
        ScoreManager.Instance.OnScoreUpdated -= UpdateScoreText;
    }
}
