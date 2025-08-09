using UnityEngine;

public class FramerateUnlocker : MonoBehaviour
{
    void Awake()
    {
        QualitySettings.vSyncCount = 0;            
        Application.targetFrameRate = 144;          
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}