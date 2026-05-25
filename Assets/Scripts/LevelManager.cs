using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Serializable]
    public class LevelConfig
    {
        public string levelName;
        public Transform spawnPoint;
        public CheckpointGroup checkpointGroup;
        public float timeLimit = 90f;
        public float twoStarTime = 70f;
        public float threeStarTime = 55f;
    }

    public Transform player;
    public Transform freeDriveSpawn;
    public LevelConfig[] levels = Array.Empty<LevelConfig>();

    public GameMode CurrentMode { get; private set; }
    public int CurrentLevelIndex { get; private set; } = -1;
    public float TimeRemaining { get; private set; }
    public float ElapsedTime { get; private set; }
    public bool IsRunning { get; private set; }
    public CheckpointGroup ActiveCheckpointGroup { get; private set; }

    public event Action OnLevelDataChanged;
    public event Action<bool, float, int> OnLevelEnded;

    private Rigidbody playerRigidbody;
    private CarController3D playerCar;

    private void Awake()
    {
        if (player == null)
        {
            CarController3D car = FindFirstObjectByType<CarController3D>();
            if (car != null) player = car.transform;
        }

        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody>();
            playerCar = player.GetComponent<CarController3D>();
        }

        DeactivateAllCheckpoints();
    }

    private void Update()
    {
        if (!IsRunning || CurrentMode != GameMode.Checkpoint) return;

        ElapsedTime += Time.deltaTime;
        TimeRemaining = Mathf.Max(0f, TimeRemaining - Time.deltaTime);
        OnLevelDataChanged?.Invoke();

        if (TimeRemaining <= 0f)
            FailLevel();
    }

    public void StartFreeDrive()
    {
        CurrentMode = GameMode.FreeDrive;
        CurrentLevelIndex = -1;
        ActiveCheckpointGroup = null;
        IsRunning = true;
        ElapsedTime = 0f;
        TimeRemaining = 0f;
        DeactivateAllCheckpoints();
        MovePlayerTo(freeDriveSpawn);
        OnLevelDataChanged?.Invoke();
    }

    public void StartLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length) return;

        CurrentMode = GameMode.Checkpoint;
        CurrentLevelIndex = levelIndex;
        LevelConfig config = levels[levelIndex];

        DeactivateAllCheckpoints();
        MovePlayerTo(config.spawnPoint);

        ActiveCheckpointGroup = config.checkpointGroup;
        if (ActiveCheckpointGroup != null)
            ActiveCheckpointGroup.Initialize(this);

        ElapsedTime = 0f;
        TimeRemaining = config.timeLimit;
        IsRunning = true;
        OnLevelDataChanged?.Invoke();
    }

    public void RestartCurrent()
    {
        if (CurrentMode == GameMode.FreeDrive) StartFreeDrive();
        else if (CurrentLevelIndex >= 0) StartLevel(CurrentLevelIndex);
    }

    public void StopLevel()
    {
        IsRunning = false;
        DeactivateAllCheckpoints();
    }

    public void CompleteLevel()
    {
        if (!IsRunning) return;
        IsRunning = false;
        int stars = CalculateStars(CurrentLevelIndex, ElapsedTime);
        SaveManager.SaveLevelResult(CurrentLevelIndex + 1, ElapsedTime, stars);
        OnLevelEnded?.Invoke(true, ElapsedTime, stars);
    }

    public void FailLevel()
    {
        if (!IsRunning) return;
        IsRunning = false;
        OnLevelEnded?.Invoke(false, ElapsedTime, 0);
    }

    public void OnCheckpointChanged()
    {
        OnLevelDataChanged?.Invoke();
    }

    public int GetCurrentCheckpointNumber()
    {
        if (ActiveCheckpointGroup == null) return 0;
        return Mathf.Clamp(ActiveCheckpointGroup.CurrentIndex + 1, 1, ActiveCheckpointGroup.Total);
    }

    public int GetCurrentCheckpointTotal()
    {
        return ActiveCheckpointGroup != null ? ActiveCheckpointGroup.Total : 0;
    }

    public int CalculateStars(int levelIndex, float finishTime)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length) return 0;
        LevelConfig config = levels[levelIndex];
        if (finishTime <= config.threeStarTime) return 3;
        if (finishTime <= config.twoStarTime) return 2;
        if (finishTime <= config.timeLimit) return 1;
        return 0;
    }

    private void MovePlayerTo(Transform spawn)
    {
        if (player == null || spawn == null) return;

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.position = spawn.position;
            playerRigidbody.rotation = spawn.rotation;
            playerRigidbody.Sleep();
        }

        player.SetPositionAndRotation(spawn.position, spawn.rotation);
        if (playerCar != null) playerCar.ResetCarState();
        Physics.SyncTransforms();
    }

    private void DeactivateAllCheckpoints()
    {
        CheckpointGroup[] groups = FindObjectsByType<CheckpointGroup>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < groups.Length; i++)
            groups[i].DeactivateAll();
    }
}
