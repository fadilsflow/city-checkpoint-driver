using System.Collections.Generic;
using UnityEngine;

public class CheckpointGroup : MonoBehaviour
{
    public List<Checkpoint> checkpoints = new List<Checkpoint>();
    public AudioClip checkpointClip;
    public float checkpointVolume = 0.85f;
    public int CurrentIndex { get; private set; }
    public int Total => checkpoints.Count;
    public Checkpoint CurrentCheckpoint => CurrentIndex >= 0 && CurrentIndex < checkpoints.Count ? checkpoints[CurrentIndex] : null;

    private LevelManager levelManager;
    private AudioSource audioSource;

    public void Initialize(LevelManager manager)
    {
        levelManager = manager;
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
        RefreshList();
        CurrentIndex = 0;
        ActivateOnly(CurrentIndex);
    }

    public void RefreshList()
    {
        checkpoints.Clear();
        GetComponentsInChildren(true, checkpoints);
        checkpoints.Sort((a, b) => a.index.CompareTo(b.index));
        for (int i = 0; i < checkpoints.Count; i++)
        {
            checkpoints[i].group = this;
            checkpoints[i].index = i;
        }
    }

    public void DeactivateAll()
    {
        RefreshList();
        for (int i = 0; i < checkpoints.Count; i++)
            checkpoints[i].SetActive(false);
    }

    public void HitCheckpoint(Checkpoint checkpoint)
    {
        if (checkpoint != CurrentCheckpoint) return;

        checkpoint.SetActive(false);
        PlayCheckpointSound();
        CurrentIndex++;

        if (CurrentIndex >= checkpoints.Count)
        {
            levelManager.CompleteLevel();
            return;
        }

        ActivateOnly(CurrentIndex);
        levelManager.OnCheckpointChanged();
    }

    private void PlayCheckpointSound()
    {
        if (checkpointClip == null) return;
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.PlayOneShot(checkpointClip, checkpointVolume);
    }

    private void ActivateOnly(int index)
    {
        for (int i = 0; i < checkpoints.Count; i++)
            checkpoints[i].SetActive(i == index);
    }
}
