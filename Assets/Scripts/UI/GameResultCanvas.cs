using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameResultCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text highscoreText;
    [SerializeField] private TMP_Text resultText;

    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button exitButton;

    private void OnEnable()
    {
        var sm = ScoreManager.Instance;
        resultText.text    = $"Счет: {sm.LastScore}";
        highscoreText.text = $"Рекорд: {sm.Highscore}";

        playAgainButton.onClick.AddListener(() =>  SceneLoader.Instance.LoadSceneAsyncCoroutine(1));
        playAgainButton.onClick.AddListener(() =>  ScoreManager.Instance.ResetScore());
        exitButton.onClick.AddListener(() => SceneLoader.Instance.LoadSceneAsyncCoroutine(0));
    }

    private void OnDisable()
    {
        playAgainButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }
}