using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    public AudioClip backgroundMusic;
    public float musicVolume = 0.35f;
    public AudioClip uiClickClip;
    public float sfxVolume = 0.7f;

    private AudioSource musicSource;
    private AudioSource uiClickSource;

    public float MusicVolume
    {
        get => musicVolume;
        set
        {
            musicVolume = Mathf.Clamp01(value);
            if (musicSource != null) musicSource.volume = musicVolume;
            SaveManager.SetMusicVolume(musicVolume);
        }
    }

    public float SfxVolume
    {
        get => sfxVolume;
        set
        {
            sfxVolume = Mathf.Clamp01(value);
            if (uiClickSource != null) uiClickSource.volume = sfxVolume;
            SaveManager.SetSfxVolume(sfxVolume);
        }
    }

    private void Awake()
    {
        musicVolume = SaveManager.MusicVolume;
        sfxVolume = SaveManager.SfxVolume;

        musicSource = GetComponent<AudioSource>();
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.playOnAwake = true;
        musicSource.spatialBlend = 0f;
        musicSource.volume = musicVolume;

        uiClickSource = gameObject.AddComponent<AudioSource>();
        uiClickSource.playOnAwake = false;
        uiClickSource.spatialBlend = 0f;
        uiClickSource.volume = sfxVolume;
    }

    private void Start()
    {
        if (backgroundMusic != null && !musicSource.isPlaying)
            musicSource.Play();

        if (uiClickClip == null)
            uiClickClip = GenerateClickClip();
    }

    public void PlayUIClick()
    {
        if (uiClickClip == null) return;
        uiClickSource.PlayOneShot(uiClickClip);
    }

    private AudioClip GenerateClickClip()
    {
        int sampleRate = 44100;
        float duration = 0.08f;
        int samples = Mathf.FloorToInt(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = 1f - (t / duration);
            data[i] = Mathf.Sin(t * 800f * Mathf.PI * 2f) * envelope * 0.5f;
        }

        AudioClip clip = AudioClip.Create("UIClick", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
