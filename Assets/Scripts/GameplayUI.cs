using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    public LevelManager levelManager;

    [Header("Screens")]
    public GameObject mainMenu;
    public GameObject levelSelect;
    public GameObject hud;
    public GameObject pauseMenu;
    public GameObject resultScreen;

    [Header("HUD")]
    public Text timerText;
    public Text checkpointText;
    public Text speedText;
    public Text directionArrowText;
    public Text checkpointDistanceText;
    public RectTransform mapRoot;
    public RectTransform playerMapArrow;
    public RectTransform checkpointMapDot;
    public float mapWorldRadius = 120f;

    [Header("Mobile Controls")]
    public MobileCarControls mobileControls;
    public bool showMobileControls = true;
    public bool showMobileControlsInEditor = true;

    [Header("Result")]
    public Text resultTitleText;
    public Text resultTimeText;
    public Text resultStarsText;

    [Header("Level Select")]
    public Button level1Button;
    public Button level2Button;
    public Text level1InfoText;
    public Text level2InfoText;
    public Text soonText;

    private const float HudEdgePadding = 24f;
    private const float HudMapSize = 180f;
    private const float HudPauseWidth = 130f;
    private const float HudPauseHeight = 48f;
    private const float HudTopRightGap = 12f;
    private const float HudLabelWidth = 420f;
    private const float HudLabelHeight = 44f;
    private const float HudLabelGap = 8f;
    private static readonly Color HudLabelBackground = new Color(0.08f, 0.13f, 0.18f, 0.85f);

    private Rigidbody playerRigidbody;
    private Font uiFont;
    private GameAudioManager audioManager;

    private enum HudCorner
    {
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft
    }

    private void Awake()
    {
        if (levelManager == null) levelManager = FindFirstObjectByType<LevelManager>();
        audioManager = FindFirstObjectByType<GameAudioManager>();
        CarController3D car = FindFirstObjectByType<CarController3D>();
        if (car != null) playerRigidbody = car.GetComponent<Rigidbody>();
        EnsureBuiltUI();
        if (car != null && mobileControls != null) car.mobileControls = mobileControls;
    }

    private void OnEnable()
    {
        if (levelManager != null)
            levelManager.OnLevelDataChanged += RefreshHUD;
    }

    private void OnDisable()
    {
        if (levelManager != null)
            levelManager.OnLevelDataChanged -= RefreshHUD;
    }

    private void Update()
    {
        if (speedText != null && playerRigidbody != null)
            speedText.text = Mathf.RoundToInt(playerRigidbody.linearVelocity.magnitude * 3.6f) + " km/h";

        UpdateMapAndDirection();
    }

    public void ShowMainMenu()
    {
        SetOnly(mainMenu);
        RefreshLevelSelect();
    }

    public void ShowLevelSelect()
    {
        SetOnly(levelSelect);
        RefreshLevelSelect();
    }

    public void ShowHUD(GameMode mode)
    {
        SetOnly(hud);
        bool checkpointMode = mode == GameMode.Checkpoint;
        if (timerText != null) GetHudLabelPanel(timerText).gameObject.SetActive(checkpointMode);
        if (checkpointText != null) GetHudLabelPanel(checkpointText).gameObject.SetActive(checkpointMode);
        if (directionArrowText != null) directionArrowText.gameObject.SetActive(checkpointMode);
        if (checkpointDistanceText != null) checkpointDistanceText.gameObject.SetActive(checkpointMode);
        if (mapRoot != null) mapRoot.gameObject.SetActive(checkpointMode);
        EnsureMobileControls();
        RefreshHUD();
    }

    public void ShowPause()
    {
        SetOnly(pauseMenu);
    }

    public void ShowResult(bool complete, float time, int stars)
    {
        SetOnly(resultScreen);
        if (resultTitleText != null) resultTitleText.text = complete ? "LEVEL COMPLETE" : "FAILED";
        if (resultTimeText != null) resultTimeText.text = "Time: " + FormatTime(time);
        if (resultStarsText != null) resultStarsText.text = complete ? "Stars: " + new string('★', Mathf.Max(1, stars)) : "Stars: -";
        RefreshLevelSelect();
    }

    public void RefreshHUD()
    {
        if (levelManager == null) return;

        if (timerText != null)
            timerText.text = "Time: " + FormatTime(levelManager.TimeRemaining);

        if (checkpointText != null)
            checkpointText.text = "Checkpoint: " + levelManager.GetCurrentCheckpointNumber() + " / " + levelManager.GetCurrentCheckpointTotal();
    }

    public void RefreshLevelSelect()
    {
        int unlocked = SaveManager.UnlockedLevel;

        if (level1Button != null) level1Button.interactable = true;
        if (level2Button != null) level2Button.interactable = unlocked >= 2;

        if (level1InfoText != null) level1InfoText.text = BuildLevelInfo(1);
        if (level2InfoText != null) level2InfoText.text = unlocked >= 2 ? BuildLevelInfo(2) : "Locked";
        if (soonText != null) soonText.text = "Level 3+ Coming Soon";
    }

    public void ButtonLevel1() => GameManager.Instance.StartLevel(0);
    public void ButtonLevel2() => GameManager.Instance.StartLevel(1);
    public void ButtonFreeDrive() => GameManager.Instance.StartFreeDrive();
    public void ButtonLevelSelect() => GameManager.Instance.ShowLevelSelect();
    public void ButtonMainMenu() => GameManager.Instance.ShowMainMenu();
    public void ButtonResume() => GameManager.Instance.Resume();
    public void ButtonRestart() => GameManager.Instance.Restart();
    public void ButtonNextLevel() => GameManager.Instance.NextLevel();
    public void ButtonQuit() => Application.Quit();

    private void EnsureBuiltUI()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = GetComponent<CanvasScaler>();
        if (scaler == null) scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        if (GetComponent<GraphicRaycaster>() == null) gameObject.AddComponent<GraphicRaycaster>();
        EnsureEventSystem();

        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (uiFont == null) uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        if (mainMenu == null) BuildMainMenu();
        if (levelSelect == null) BuildLevelSelect();
        if (hud == null) BuildHUD();
        else EnsureHUDExtras();
        if (pauseMenu == null) BuildPauseMenu();
        if (resultScreen == null) BuildResultScreen();
    }

    private void BuildMainMenu()
    {
        mainMenu = CreatePanel("MainMenu");
        AddText(mainMenu.transform, "Title", "CITY CHECKPOINT DRIVER", 0, 250, 56);
        AddButton(mainMenu.transform, "PlayButton", "PLAY", 0, 90, ButtonLevelSelect);
        AddButton(mainMenu.transform, "FreeDriveButton", "FREE DRIVE", 0, 0, ButtonFreeDrive);
        AddButton(mainMenu.transform, "QuitButton", "QUIT", 0, -90, ButtonQuit);
        AddText(mainMenu.transform, "SoonHint", "2 levels playable • more levels coming soon", 0, -210, 26);
    }

    private void BuildLevelSelect()
    {
        levelSelect = CreatePanel("LevelSelect");
        AddText(levelSelect.transform, "Title", "LEVEL SELECT", 0, 280, 50);
        level1Button = AddButton(levelSelect.transform, "Level1Button", "LEVEL 1", -320, 90, ButtonLevel1);
        level1InfoText = AddText(levelSelect.transform, "Level1Info", "Best: --:--", -320, 20, 24);
        level2Button = AddButton(levelSelect.transform, "Level2Button", "LEVEL 2", 0, 90, ButtonLevel2);
        level2InfoText = AddText(levelSelect.transform, "Level2Info", "Locked", 0, 20, 24);
        AddButton(levelSelect.transform, "SoonButton", "LEVEL 3", 320, 90, null).interactable = false;
        soonText = AddText(levelSelect.transform, "SoonInfo", "Coming Soon", 320, 20, 24);
        AddButton(levelSelect.transform, "BackButton", "BACK", 0, -220, ButtonMainMenu);
    }

    private void BuildHUD()
    {
        hud = CreatePanel("HUD", false);
        timerText = AddHudLabel(hud.transform, "Timer", "Time: 00:00.00", 30);
        checkpointText = AddHudLabel(hud.transform, "Checkpoint", "Checkpoint: 1 / 4", 30);
        speedText = AddText(hud.transform, "Speed", "0 km/h", 0, 0, 30, TextAnchor.MiddleRight);
        AddButton(hud.transform, "PauseButton", "PAUSE", 0, 0, ButtonResume).onClick.RemoveAllListeners();
        Button pauseButton = hud.transform.Find("PauseButton").GetComponent<Button>();
        pauseButton.onClick.AddListener(() => GameManager.Instance.Pause());
        EnsureHUDExtras();
        ApplyHUDLayout();
    }

    private void EnsureHUDExtras()
    {
        if (hud == null) return;

        if (directionArrowText == null)
            directionArrowText = AddText(hud.transform, "DirectionArrow", "▲", 0, -330, 76);

        if (checkpointDistanceText == null)
            checkpointDistanceText = AddText(hud.transform, "CheckpointDistance", "Checkpoint 0m", 0, -405, 30);

        if (mapRoot != null)
        {
            ApplyHUDLayout();
            EnsureMobileControls();
            return;
        }

        GameObject map = new GameObject("MiniMap", typeof(RectTransform));
        map.transform.SetParent(hud.transform, false);
        mapRoot = map.GetComponent<RectTransform>();
        Image mapImage = map.AddComponent<Image>();
        mapImage.color = new Color(0f, 0f, 0f, 0.45f);

        GameObject playerDot = new GameObject("PlayerArrow", typeof(RectTransform));
        playerDot.transform.SetParent(mapRoot, false);
        playerMapArrow = playerDot.GetComponent<RectTransform>();
        playerMapArrow.sizeDelta = new Vector2(34f, 34f);
        Text playerText = playerDot.AddComponent<Text>();
        playerText.font = uiFont;
        playerText.text = "▲";
        playerText.fontSize = 30;
        playerText.alignment = TextAnchor.MiddleCenter;
        playerText.color = Color.white;

        GameObject checkpointDot = new GameObject("CheckpointDot", typeof(RectTransform));
        checkpointDot.transform.SetParent(mapRoot, false);
        checkpointMapDot = checkpointDot.GetComponent<RectTransform>();
        checkpointMapDot.sizeDelta = new Vector2(30f, 30f);
        Image dotImage = checkpointDot.AddComponent<Image>();
        dotImage.color = new Color(0f, 1f, 0.25f, 0.95f);

        ApplyHUDLayout();
        EnsureMobileControls();
    }

    private void ApplyHUDLayout()
    {
        if (hud == null) return;

        if (timerText != null)
        {
            RectTransform rect = GetHudLabelPanel(timerText);
            rect.sizeDelta = new Vector2(HudLabelWidth, HudLabelHeight);
            SetCornerAnchor(rect, HudCorner.TopLeft, new Vector2(HudEdgePadding, -HudEdgePadding));
            timerText.alignment = TextAnchor.MiddleLeft;
        }

        if (checkpointText != null)
        {
            RectTransform rect = GetHudLabelPanel(checkpointText);
            rect.sizeDelta = new Vector2(HudLabelWidth, HudLabelHeight);
            SetCornerAnchor(rect, HudCorner.TopLeft, new Vector2(HudEdgePadding, -(HudEdgePadding + HudLabelHeight + HudLabelGap)));
            checkpointText.alignment = TextAnchor.MiddleLeft;
        }

        if (speedText != null)
        {
            RectTransform rect = speedText.rectTransform;
            rect.sizeDelta = new Vector2(240f, 44f);
            SetCornerAnchor(rect, HudCorner.BottomRight, new Vector2(-HudEdgePadding, HudEdgePadding));
            speedText.alignment = TextAnchor.MiddleRight;
        }

        Transform pauseTransform = hud.transform.Find("PauseButton");
        if (pauseTransform != null)
        {
            RectTransform pauseRect = pauseTransform.GetComponent<RectTransform>();
            pauseRect.sizeDelta = new Vector2(HudPauseWidth, HudPauseHeight);
            float pauseYOffset = -(HudEdgePadding + (HudMapSize - HudPauseHeight) * 0.5f);
            SetCornerAnchor(pauseRect, HudCorner.TopRight, new Vector2(-HudEdgePadding, pauseYOffset));
        }

        if (mapRoot != null)
        {
            mapRoot.sizeDelta = new Vector2(HudMapSize, HudMapSize);
            float mapX = -(HudEdgePadding + HudPauseWidth + HudTopRightGap);
            SetCornerAnchor(mapRoot, HudCorner.TopRight, new Vector2(mapX, -HudEdgePadding));
        }
    }

    private static void SetCornerAnchor(RectTransform rect, HudCorner corner, Vector2 anchoredPosition)
    {
        Vector2 anchor = corner switch
        {
            HudCorner.TopLeft => new Vector2(0f, 1f),
            HudCorner.TopRight => new Vector2(1f, 1f),
            HudCorner.BottomRight => new Vector2(1f, 0f),
            HudCorner.BottomLeft => new Vector2(0f, 0f),
            _ => new Vector2(0.5f, 0.5f)
        };

        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = anchoredPosition;
    }

    private void EnsureMobileControls()
    {
        if (hud == null) return;

        if (mobileControls == null)
            mobileControls = hud.GetComponentInChildren<MobileCarControls>(true);

        if (mobileControls == null)
        {
            GameObject analog = new GameObject("MobileAnalog", typeof(RectTransform));
            analog.transform.SetParent(hud.transform, false);

            RectTransform analogRect = analog.GetComponent<RectTransform>();
            analogRect.anchorMin = Vector2.zero;
            analogRect.anchorMax = Vector2.zero;
            analogRect.pivot = new Vector2(0.5f, 0.5f);
            analogRect.sizeDelta = new Vector2(190f, 190f);
            analogRect.anchoredPosition = new Vector2(170f, 155f);

            Image baseImage = analog.AddComponent<Image>();
            baseImage.sprite = CreateCircleSprite("MobileAnalogBase", 96);
            baseImage.color = new Color(0.08f, 0.13f, 0.18f, 0.58f);

            mobileControls = analog.AddComponent<MobileCarControls>();
            mobileControls.radius = 72f;

            GameObject knob = new GameObject("Knob", typeof(RectTransform));
            knob.transform.SetParent(analog.transform, false);

            RectTransform knobRect = knob.GetComponent<RectTransform>();
            knobRect.anchorMin = new Vector2(0.5f, 0.5f);
            knobRect.anchorMax = new Vector2(0.5f, 0.5f);
            knobRect.pivot = new Vector2(0.5f, 0.5f);
            knobRect.sizeDelta = new Vector2(82f, 82f);
            knobRect.anchoredPosition = Vector2.zero;

            Image knobImage = knob.AddComponent<Image>();
            knobImage.sprite = CreateCircleSprite("MobileAnalogKnob", 96);
            knobImage.color = new Color(1f, 1f, 1f, 0.9f);
            knobImage.raycastTarget = false;
            mobileControls.knob = knobRect;

            Text label = AddText(analog.transform, "AnalogLabel", "DRIVE", 0f, -120f, 22);
            label.color = new Color(1f, 1f, 1f, 0.78f);
        }

        mobileControls.gameObject.SetActive(ShouldShowMobileControls());

        CarController3D car = FindFirstObjectByType<CarController3D>();
        if (car != null && car.mobileControls == null)
            car.mobileControls = mobileControls;
    }

    private bool ShouldShowMobileControls()
    {
        if (!showMobileControls) return false;

#if UNITY_EDITOR
        if (showMobileControlsInEditor) return true;
#endif

        return Application.isMobilePlatform || Input.touchSupported;
    }

    private static Sprite CreateCircleSprite(string name, int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color clear = new Color(1f, 1f, 1f, 0f);
        Color solid = Color.white;
        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                texture.SetPixel(x, y, distance <= radius ? solid : clear);
            }
        }

        texture.Apply();
        texture.name = name;
        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
    }

    private void BuildPauseMenu()
    {
        pauseMenu = CreatePanel("PauseMenu");
        AddText(pauseMenu.transform, "Title", "PAUSED", 0, 220, 52);
        AddButton(pauseMenu.transform, "ResumeButton", "RESUME", 0, 80, ButtonResume);
        AddButton(pauseMenu.transform, "RestartButton", "RESTART", 0, -10, ButtonRestart);
        AddButton(pauseMenu.transform, "MainMenuButton", "MAIN MENU", 0, -100, ButtonMainMenu);
    }

    private void BuildResultScreen()
    {
        resultScreen = CreatePanel("ResultScreen");
        resultTitleText = AddText(resultScreen.transform, "Title", "LEVEL COMPLETE", 0, 240, 54);
        resultTimeText = AddText(resultScreen.transform, "Time", "Time: --:--", 0, 140, 34);
        resultStarsText = AddText(resultScreen.transform, "Stars", "Stars: -", 0, 90, 34);
        AddButton(resultScreen.transform, "RetryButton", "RETRY", -220, -60, ButtonRestart);
        AddButton(resultScreen.transform, "NextButton", "NEXT", 0, -60, ButtonNextLevel);
        AddButton(resultScreen.transform, "MenuButton", "MAIN MENU", 220, -60, ButtonMainMenu);
    }

    private void UpdateMapAndDirection()
    {
        if (levelManager == null || levelManager.CurrentMode != GameMode.Checkpoint) return;
        if (levelManager.ActiveCheckpointGroup == null || levelManager.ActiveCheckpointGroup.CurrentCheckpoint == null) return;
        if (playerRigidbody == null) return;

        Transform playerTransform = playerRigidbody.transform;
        Vector3 toCheckpoint = levelManager.ActiveCheckpointGroup.CurrentCheckpoint.transform.position - playerTransform.position;
        Vector3 flatDirection = Vector3.ProjectOnPlane(toCheckpoint, Vector3.up);
        float distance = flatDirection.magnitude;

        if (checkpointDistanceText != null)
            checkpointDistanceText.text = "Checkpoint " + Mathf.RoundToInt(distance) + "m";

        if (directionArrowText != null && flatDirection.sqrMagnitude > 0.01f)
        {
            float signedAngle = Vector3.SignedAngle(playerTransform.forward, flatDirection.normalized, Vector3.up);
            directionArrowText.rectTransform.localEulerAngles = new Vector3(0f, 0f, -signedAngle);
        }

        if (mapRoot == null || checkpointMapDot == null || playerMapArrow == null) return;

        Vector3 localDirection = playerTransform.InverseTransformDirection(flatDirection);
        Vector2 mapPosition = new Vector2(localDirection.x, localDirection.z) / mapWorldRadius * (mapRoot.sizeDelta.x * 0.42f);
        mapPosition = Vector2.ClampMagnitude(mapPosition, mapRoot.sizeDelta.x * 0.42f);
        checkpointMapDot.anchoredPosition = mapPosition;
        playerMapArrow.anchoredPosition = Vector2.zero;
        playerMapArrow.localEulerAngles = Vector3.zero;
    }

    private GameObject CreatePanel(string name, bool dim = true)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform));
        panel.transform.SetParent(transform, false);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        if (dim)
        {
            Image image = panel.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.55f);
        }

        return panel;
    }

    private Text AddHudLabel(Transform parent, string name, string text, int fontSize)
    {
        GameObject panel = new GameObject(name + "Panel", typeof(RectTransform));
        panel.transform.SetParent(parent, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(HudLabelWidth, HudLabelHeight);

        Image background = panel.AddComponent<Image>();
        background.color = HudLabelBackground;
        background.raycastTarget = false;

        GameObject labelGo = new GameObject(name, typeof(RectTransform));
        labelGo.transform.SetParent(panel.transform, false);

        RectTransform labelRect = labelGo.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(12f, 0f);
        labelRect.offsetMax = new Vector2(-12f, 0f);

        Text label = labelGo.AddComponent<Text>();
        label.font = uiFont;
        label.text = text;
        label.fontSize = fontSize;
        label.alignment = TextAnchor.MiddleLeft;
        label.color = Color.white;
        label.raycastTarget = false;
        return label;
    }

    private RectTransform GetHudLabelPanel(Text label)
    {
        if (label == null) return null;

        RectTransform parent = label.rectTransform.parent as RectTransform;
        if (parent != null && parent.parent == hud.transform)
            return parent;

        return label.rectTransform;
    }

    private Text AddText(Transform parent, string name, string text, float x, float y, int size, TextAnchor anchor = TextAnchor.MiddleCenter)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(620f, 70f);
        rect.anchoredPosition = new Vector2(x, y);
        Text label = go.AddComponent<Text>();
        label.font = uiFont;
        label.text = text;
        label.fontSize = size;
        label.alignment = anchor;
        label.color = Color.white;
        return label;
    }

    private Button AddButton(Transform parent, string name, string text, float x, float y, UnityEngine.Events.UnityAction action)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(260f, 62f);
        rect.anchoredPosition = new Vector2(x, y);
        Image image = go.AddComponent<Image>();
        image.color = new Color(0.08f, 0.13f, 0.18f, 0.92f);
        Button button = go.AddComponent<Button>();
        if (action != null) button.onClick.AddListener(action);
        if (audioManager != null) button.onClick.AddListener(() => audioManager.PlayUIClick());
        Text label = AddText(go.transform, "Text", text, 0, 0, 28);
        label.rectTransform.sizeDelta = rect.sizeDelta;
        return button;
    }

    private static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() != null) return;
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    }

    private void SetOnly(GameObject active)
    {
        if (mainMenu != null) mainMenu.SetActive(active == mainMenu);
        if (levelSelect != null) levelSelect.SetActive(active == levelSelect);
        if (hud != null) hud.SetActive(active == hud);
        if (pauseMenu != null) pauseMenu.SetActive(active == pauseMenu);
        if (resultScreen != null) resultScreen.SetActive(active == resultScreen);
    }

    private static string BuildLevelInfo(int levelIndex)
    {
        float best = SaveManager.GetBestTime(levelIndex);
        int stars = SaveManager.GetBestStars(levelIndex);
        string time = best > 0f ? FormatTime(best) : "--:--";
        string starText = stars > 0 ? new string('★', stars) : "No stars";
        return "Best: " + time + "  " + starText;
    }

    private static string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        int millis = Mathf.FloorToInt((seconds * 100f) % 100f);
        return $"{minutes:00}:{secs:00}.{millis:00}";
    }
}
