using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance{get; private set;}
    
    public bool GameIsOver {get ; private set;}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GameOver()
    {
        GameIsOver = true;
        Debug.Log("GameOver");
        ScoreManager.Instance.CommitRun();
        SceneLoader.Instance.LoadSceneAsyncCoroutine(2);
    }
}
