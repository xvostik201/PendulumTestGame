using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volumes")]
    [Range(0f,1f)] [SerializeField] private float musicVolume = 1f;
    [Range(0f,1f)] [SerializeField] private float sfxVolume   = 1f;

    const string KeyMusic = "AM_MUSIC";
    const string KeySFX   = "AM_SFX";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!musicSource) musicSource = CreateSource("Music", loop:true);
        if (!sfxSource)   sfxSource   = CreateSource("SFX", loop:false);

        musicVolume = PlayerPrefs.GetFloat(KeyMusic, musicVolume);
        sfxVolume   = PlayerPrefs.GetFloat(KeySFX,   sfxVolume);
        musicSource.volume = musicVolume;
        sfxSource.volume   = sfxVolume;
    }

    private AudioSource CreateSource(string name, bool loop)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = loop;
        src.spatialBlend = 0f;
        return src;
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (!clip) return;
        musicSource.loop = loop;
        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();

    public void PlaySfx(AudioClip clip, float volume = 1f)
    {
        if (!clip) return;
        sfxSource.PlayOneShot(clip, sfxVolume * Mathf.Clamp01(volume));
    }

    public void SetMusicVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);
        musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat(KeyMusic, musicVolume);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(float v)
    {
        sfxVolume = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat(KeySFX, sfxVolume);
        PlayerPrefs.Save();
    }

    public float GetMusicVolume() => musicVolume;
    public float GetSfxVolume()   => sfxVolume;
}
