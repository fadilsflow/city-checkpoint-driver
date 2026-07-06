using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; } = GameState.MainMenu;
    public LevelManager levelManager;
    public GameplayUI ui;

    private GameState settingsReturnState = GameState.MainMenu;

    private void Awake()
    {
        Instance = this;
        if (levelManager == null) levelManager = FindFirstObjectByType<LevelManager>();
        if (ui == null) ui = FindFirstObjectByType<GameplayUI>();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        if (levelManager != null)
            levelManager.OnLevelEnded += HandleLevelEnded;
        ShowMainMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (State == GameState.Playing) Pause();
            else if (State == GameState.Paused) Resume();
            else if (State == GameState.Settings) CloseSettings();
        }
    }

    public void ShowMainMenu()
    {
        State = GameState.MainMenu;
        Time.timeScale = 1f;
        if (levelManager != null) levelManager.StopLevel();
        if (ui != null) ui.ShowMainMenu();
    }

    public void ShowLevelSelect()
    {
        State = GameState.LevelSelect;
        if (ui != null) ui.ShowLevelSelect();
    }

    public void ShowSettings()
    {
        settingsReturnState = State;
        State = GameState.Settings;
        if (settingsReturnState == GameState.Paused)
            Time.timeScale = 0f;
        if (ui != null) ui.ShowSettings();
    }

    public void CloseSettings()
    {
        switch (settingsReturnState)
        {
            case GameState.Paused:
                State = GameState.Paused;
                Time.timeScale = 0f;
                if (ui != null) ui.ShowPause();
                break;
            case GameState.LevelSelect:
                ShowLevelSelect();
                break;
            default:
                ShowMainMenu();
                break;
        }
    }

    public void StartFreeDrive()
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
        if (levelManager != null) levelManager.StartFreeDrive();
        if (ui != null) ui.ShowHUD(GameMode.FreeDrive);
    }

    public void StartLevel(int zeroBasedIndex)
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
        if (levelManager != null) levelManager.StartLevel(zeroBasedIndex);
        if (ui != null) ui.ShowHUD(GameMode.Checkpoint);
    }

    public void Pause()
    {
        if (State != GameState.Playing) return;
        State = GameState.Paused;
        Time.timeScale = 0f;
        if (ui != null) ui.ShowPause();
    }

    public void Resume()
    {
        if (State != GameState.Paused) return;
        State = GameState.Playing;
        Time.timeScale = 1f;
        if (ui != null) ui.ShowHUD(levelManager != null ? levelManager.CurrentMode : GameMode.FreeDrive);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        State = GameState.Playing;
        if (levelManager != null) levelManager.RestartCurrent();
        if (ui != null) ui.ShowHUD(levelManager != null ? levelManager.CurrentMode : GameMode.FreeDrive);
    }

    public void NextLevel()
    {
        if (levelManager == null) return;
        int next = levelManager.CurrentLevelIndex + 1;
        if (next < levelManager.levels.Length && SaveManager.UnlockedLevel >= next + 1)
            StartLevel(next);
        else
            ShowLevelSelect();
    }

    private void HandleLevelEnded(bool complete, float time, int stars)
    {
        State = complete ? GameState.Complete : GameState.Failed;
        if (ui != null) ui.ShowResult(complete, time, stars);
    }
}
