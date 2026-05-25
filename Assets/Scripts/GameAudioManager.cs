using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    public AudioClip backgroundMusic;
    public float musicVolume = 0.35f;

    private AudioSource musicSource;

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.playOnAwake = true;
        musicSource.spatialBlend = 0f;
        musicSource.volume = musicVolume;
    }

    private void Start()
    {
        if (backgroundMusic != null && !musicSource.isPlaying)
            musicSource.Play();
    }
}
