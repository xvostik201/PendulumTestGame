using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance{get; private set;}
    
    private const float MinimumLoadTime = 1f;
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
        DontDestroyOnLoad(gameObject);
    }
    private IEnumerator LoadSceneAsync(int sceneIndex, float minimumLoadTime = MinimumLoadTime)
    {
        float startTime = Time.time;
        var op = SceneManager.LoadSceneAsync(sceneIndex);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
            yield return null;

        float elapsed = Time.time - startTime;
        if (elapsed < minimumLoadTime)
            yield return new WaitForSeconds(minimumLoadTime - elapsed);

        op.allowSceneActivation = true;
    }

    public void LoadSceneAsyncCoroutine(int sceneIndex, float timeLoad = MinimumLoadTime)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex, timeLoad));
    }
}
