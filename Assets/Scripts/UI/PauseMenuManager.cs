using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// PauseMenuManager - Quản lý Pause Menu trong game
/// Nhấn ESC để mở/đóng pause menu (khi không trong dialogue/VN/storytelling)
/// Singleton, DontDestroyOnLoad để persist qua các scene
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("UI References")]
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Pause Panel Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("Settings Panel")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Button backButton;

    public bool IsPaused { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupUI();
        SetupButtons();
        HideAllPanels();
    }

    private void Update()
    {
        // Ẩn hoàn toàn khi ở MainMenu
        if (IsInMainMenu())
        {
            if (IsPaused)
            {
                IsPaused = false;
                Time.timeScale = 1f;
            }
            HideAllPanels();
            DisableCanvas();
            return;
        }

        if (Input.GetKeyDown(pauseKey))
        {
            // Nếu đang mở settings, quay về pause panel
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                ShowPausePanel();
                return;
            }

            // Toggle pause
            if (IsPaused)
                ResumeGame();
            else if (CanPause())
                PauseGame();
        }
    }

    /// <summary>
    /// Kiểm tra có thể pause không (không trong dialogue/VN/storytelling/cutscene)
    /// </summary>
    private bool CanPause()
    {
        // Check VN mode
        if (VisualNovelManager.Instance != null && VisualNovelManager.Instance.IsVNModeActive)
            return false;

        // Check Storytelling
        if (StorytellingManager.Instance != null && StorytellingManager.Instance.IsPlaying)
            return false;

        // Check GameManager states
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.isInDialogue) return false;
            if (GameManager.Instance.isTransitioningScene) return false;
        }

        return true;
    }

    private bool IsInMainMenu()
    {
        return SceneManager.GetActiveScene().name == mainMenuSceneName;
    }

    #region UI Setup
    private void SetupUI()
    {
        if (pauseCanvas == null)
        {
            pauseCanvas = GetComponent<Canvas>();
            if (pauseCanvas == null)
            {
                pauseCanvas = gameObject.AddComponent<Canvas>();
            }
        }
        
        pauseCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        pauseCanvas.sortingOrder = 9000; // Dưới ScreenFader (9999) nhưng trên game UI

        var scaler = GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = gameObject.AddComponent<CanvasScaler>();
        }
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        if (GetComponent<GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }

        // Đảm bảo có EventSystem trong scene
        EnsureEventSystemExists();

        // Auto-create panels if not assigned
        if (pausePanel == null) CreatePausePanel();
        if (settingsPanel == null) CreateSettingsPanel();
    }

    private void EnsureEventSystemExists()
    {
        if (EventSystem.current == null)
        {
            var eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            var inputModule = eventSystemObj.AddComponent<StandaloneInputModule>();
            Debug.Log("[PauseMenu] Created EventSystem");
        }
        
        // Đảm bảo input module hoạt động khi Time.timeScale = 0
        var existingInputModule = FindFirstObjectByType<StandaloneInputModule>();
        if (existingInputModule != null)
        {
            // Force module active để hoạt động khi paused
            existingInputModule.forceModuleActive = true;
        }
    }

    private void CreatePausePanel()
    {
        pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(transform, false);

        // Background overlay
        Image bg = pausePanel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.7f);
        RectTransform bgRect = pausePanel.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Title
        CreateText(pausePanel.transform, "Title", "TẠM DỪNG", 48, new Vector2(0.5f, 0.75f));

        // Buttons container
        GameObject buttonsContainer = new GameObject("ButtonsContainer");
        buttonsContainer.transform.SetParent(pausePanel.transform, false);
        RectTransform containerRect = buttonsContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.25f);
        containerRect.anchorMax = new Vector2(0.5f, 0.65f);
        containerRect.sizeDelta = new Vector2(300, 350);

        VerticalLayoutGroup layout = buttonsContainer.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 15;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        // Create buttons
        resumeButton = CreateButton(buttonsContainer.transform, "ResumeButton", "TIẾP TỤC");
        settingsButton = CreateButton(buttonsContainer.transform, "SettingsButton", "CÀI ĐẶT");
        mainMenuButton = CreateButton(buttonsContainer.transform, "MainMenuButton", "MENU CHÍNH");
        // Bỏ nút THOÁT GAME vì đã có MENU CHÍNH
    }

    private void CreateSettingsPanel()
    {
        settingsPanel = new GameObject("SettingsPanel");
        settingsPanel.transform.SetParent(transform, false);

        // Background
        Image bg = settingsPanel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.8f);
        RectTransform bgRect = settingsPanel.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Title
        CreateText(settingsPanel.transform, "Title", "CÀI ĐẶT", 48, new Vector2(0.5f, 0.8f));

        // Music Volume
        CreateText(settingsPanel.transform, "MusicLabel", "Âm nhạc", 24, new Vector2(0.35f, 0.6f));
        musicVolumeSlider = CreateSlider(settingsPanel.transform, "MusicSlider", new Vector2(0.55f, 0.6f));

        // SFX Volume
        CreateText(settingsPanel.transform, "SFXLabel", "Hiệu ứng", 24, new Vector2(0.35f, 0.5f));
        sfxVolumeSlider = CreateSlider(settingsPanel.transform, "SFXSlider", new Vector2(0.55f, 0.5f));

        // Fullscreen Toggle
        CreateText(settingsPanel.transform, "FullscreenLabel", "Toàn màn hình", 24, new Vector2(0.35f, 0.4f));
        fullscreenToggle = CreateToggle(settingsPanel.transform, "FullscreenToggle", new Vector2(0.55f, 0.4f));

        // Back button
        backButton = CreateButton(settingsPanel.transform, "BackButton", "QUAY LẠI");
        RectTransform backRect = backButton.GetComponent<RectTransform>();
        backRect.anchorMin = new Vector2(0.5f, 0.2f);
        backRect.anchorMax = new Vector2(0.5f, 0.2f);
        backRect.sizeDelta = new Vector2(200, 50);
    }


    private Text CreateText(Transform parent, string name, string content, int fontSize, Vector2 anchor)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        Text text = textObj.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = new Vector2(400, 60);
        rect.anchoredPosition = Vector2.zero;

        return text;
    }

    private Button CreateButton(Transform parent, string name, string text)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.3f, 0.3f, 0.4f, 0.9f);
        image.raycastTarget = true; // Đảm bảo có thể click

        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = image;
        button.interactable = true;
        
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.3f, 0.3f, 0.4f, 0.9f);
        colors.highlightedColor = new Color(0.4f, 0.4f, 0.5f, 1f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.3f, 1f);
        colors.selectedColor = new Color(0.4f, 0.4f, 0.5f, 1f);
        button.colors = colors;

        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(250, 50);

        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        Text textComp = textObj.AddComponent<Text>();
        textComp.text = text;
        textComp.fontSize = 24;
        textComp.color = Color.white;
        textComp.alignment = TextAnchor.MiddleCenter;
        textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComp.raycastTarget = false; // Text không cần raycast

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        return button;
    }

    private Slider CreateSlider(Transform parent, string name, Vector2 anchor)
    {
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent, false);

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = anchor;
        sliderRect.anchorMax = anchor;
        sliderRect.sizeDelta = new Vector2(200, 20);

        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = new Vector2(-10, 0);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.5f, 0.7f, 1f, 1f);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;

        slider.fillRect = fillRect;

        // Handle
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.sizeDelta = new Vector2(-10, 0);

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 20);

        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;

        return slider;
    }

    private Toggle CreateToggle(Transform parent, string name, Vector2 anchor)
    {
        GameObject toggleObj = new GameObject(name);
        toggleObj.transform.SetParent(parent, false);

        Toggle toggle = toggleObj.AddComponent<Toggle>();

        RectTransform toggleRect = toggleObj.GetComponent<RectTransform>();
        toggleRect.anchorMin = anchor;
        toggleRect.anchorMax = anchor;
        toggleRect.sizeDelta = new Vector2(30, 30);

        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(toggleObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Checkmark
        GameObject checkObj = new GameObject("Checkmark");
        checkObj.transform.SetParent(bgObj.transform, false);
        Image checkImage = checkObj.AddComponent<Image>();
        checkImage.color = new Color(0.5f, 0.8f, 0.5f, 1f);
        RectTransform checkRect = checkObj.GetComponent<RectTransform>();
        checkRect.anchorMin = new Vector2(0.1f, 0.1f);
        checkRect.anchorMax = new Vector2(0.9f, 0.9f);
        checkRect.sizeDelta = Vector2.zero;

        toggle.targetGraphic = bgImage;
        toggle.graphic = checkImage;

        return toggle;
    }
    #endregion

    #region Button Setup
    private void SetupButtons()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettingsPanel);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        if (backButton != null)
            backButton.onClick.AddListener(ShowPausePanel);

        // Settings listeners
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = PlayerPrefs.GetInt("IsFullscreen", 1) == 1;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
        }
    }
    #endregion

    #region Pause/Resume
    public void PauseGame()
    {
        if (IsPaused) return;

        IsPaused = true;
        Time.timeScale = 0f;

        if (GameManager.Instance != null)
            GameManager.Instance.isGamePaused = true;

        // Đảm bảo EventSystem hoạt động khi paused
        EnsureEventSystemExists();

        ShowPausePanel();
        Debug.Log("[PauseMenu] Game paused");
    }

    public void ResumeGame()
    {
        if (!IsPaused) return;

        IsPaused = false;
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
            GameManager.Instance.isGamePaused = false;

        HideAllPanels();
        Debug.Log("[PauseMenu] Game resumed");
    }
    #endregion

    #region Panel Management
    private void ShowPausePanel()
    {
        // Enable canvas trước
        EnableCanvas();
        
        if (pausePanel != null) pausePanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    private void ShowSettingsPanel()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);

        // Refresh settings values
        if (fullscreenToggle != null && ScreenModeManager.Instance != null)
            fullscreenToggle.isOn = ScreenModeManager.Instance.IsFullscreen;
    }

    private void HideAllPanels()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }
    
    /// <summary>
    /// Disable canvas hoàn toàn (dùng khi ở MainMenu)
    /// </summary>
    private void DisableCanvas()
    {
        if (pauseCanvas != null)
        {
            pauseCanvas.enabled = false;
        }
    }
    
    /// <summary>
    /// Enable canvas (dùng khi cần hiển thị pause menu)
    /// </summary>
    private void EnableCanvas()
    {
        if (pauseCanvas != null)
        {
            pauseCanvas.enabled = true;
        }
    }
    #endregion

    #region Settings Handlers
    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    private void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    private void OnFullscreenToggleChanged(bool isFullscreen)
    {
        if (ScreenModeManager.Instance != null)
        {
            if (isFullscreen)
                ScreenModeManager.Instance.SetFullscreen();
            else
                ScreenModeManager.Instance.SetWindowed();
        }
    }
    #endregion

    #region Navigation
    private void ReturnToMainMenu()
    {
        ResumeGame(); // Reset time scale
        
        // Reset story nếu cần
        if (StoryManager.Instance != null)
        {
            // Không reset story ở đây, để player có thể continue
        }

        SceneManager.LoadScene(mainMenuSceneName);
        Debug.Log("[PauseMenu] Returning to main menu");
    }

    private void QuitGame()
    {
        // Đổi thành quay về MainMenu thay vì thoát hẳn game
        ReturnToMainMenu();
    }
    #endregion
}
