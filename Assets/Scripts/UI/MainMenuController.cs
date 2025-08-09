using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Pendulun pendulum;
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;
    private void Start()
    {
        playButton.onClick.AddListener(OnPlay);
        exitButton.onClick.AddListener(Application.Quit);
    }

    private void OnPlay()
    {
        playButton.interactable = false;
        pendulum.ReleaseBall();
        SceneLoader.Instance.LoadSceneAsyncCoroutine(1);
    }
}