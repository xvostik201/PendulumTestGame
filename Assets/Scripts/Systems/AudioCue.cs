using UnityEngine;

public class AudioCue : MonoBehaviour
{
    public enum Channel { Music, SFX }

    [SerializeField] private Channel channel = Channel.SFX;
    [SerializeField] private AudioClip clip;
    [SerializeField] private bool loopMusic = true;
    [SerializeField] private bool playOnStart = false;

    private void Start()
    {
        if (playOnStart) Play();
    }

    public void Play()
    {
        if (!AudioManager.Instance) return;
        if (channel == Channel.Music) AudioManager.Instance.PlayMusic(clip, loopMusic);
        else AudioManager.Instance.PlaySfx(clip);
    }

    public void StopMusic()
    {
        if (channel == Channel.Music && AudioManager.Instance) AudioManager.Instance.StopMusic();
    }
}