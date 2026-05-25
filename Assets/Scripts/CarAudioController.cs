using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarAudioController : MonoBehaviour
{
    [Header("Clips")]
    public AudioClip startEngineClip;
    public AudioClip engineLoopClip;
    public AudioClip crashClip;
    public AudioClip honkClip;

    [Header("Engine")]
    public float minPitch = 0.75f;
    public float maxPitch = 1.65f;
    public float maxPitchSpeedKph = 120f;
    public float engineVolume = 0.45f;

    [Header("Crash")]
    public float minCrashImpulse = 4f;
    public float crashCooldown = 0.45f;
    public float crashVolume = 0.9f;

    [Header("Honk")]
    public KeyCode honkKey = KeyCode.H;
    public float honkVolume = 0.8f;

    private Rigidbody rb;
    private AudioSource engineLoopSource;
    private AudioSource oneShotSource;
    private float lastCrashTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        engineLoopSource = gameObject.AddComponent<AudioSource>();
        engineLoopSource.clip = engineLoopClip;
        engineLoopSource.loop = true;
        engineLoopSource.playOnAwake = false;
        engineLoopSource.spatialBlend = 0.65f;
        engineLoopSource.volume = engineVolume;

        oneShotSource = gameObject.AddComponent<AudioSource>();
        oneShotSource.playOnAwake = false;
        oneShotSource.spatialBlend = 0.75f;
    }

    private void Start()
    {
        StartCoroutine(PlayEngineStartThenLoop());
    }

    private void Update()
    {
        UpdateEngineAudio();

        if (Input.GetKeyDown(honkKey) && honkClip != null)
            oneShotSource.PlayOneShot(honkClip, honkVolume);
    }

    private IEnumerator PlayEngineStartThenLoop()
    {
        if (startEngineClip != null)
        {
            oneShotSource.PlayOneShot(startEngineClip, engineVolume);
            yield return new WaitForSeconds(Mathf.Min(startEngineClip.length, 2.2f));
        }

        if (engineLoopClip != null && !engineLoopSource.isPlaying)
        {
            engineLoopSource.clip = engineLoopClip;
            engineLoopSource.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (crashClip == null) return;
        if (Time.time - lastCrashTime < crashCooldown) return;
        if (collision.impulse.magnitude < minCrashImpulse) return;

        lastCrashTime = Time.time;
        float volume = Mathf.Clamp01(collision.impulse.magnitude / 18f) * crashVolume;
        oneShotSource.PlayOneShot(crashClip, volume);
    }

    private void UpdateEngineAudio()
    {
        if (engineLoopSource == null || engineLoopClip == null) return;

        float speedKph = rb.linearVelocity.magnitude * 3.6f;
        float t = Mathf.InverseLerp(0f, maxPitchSpeedKph, speedKph);
        engineLoopSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);
        engineLoopSource.volume = Mathf.Lerp(engineVolume * 0.65f, engineVolume, t);
    }
}
