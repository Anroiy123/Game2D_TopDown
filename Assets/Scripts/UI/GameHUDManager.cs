using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// GameHUDManager - Hiển thị HUD trong game
/// - Location name (góc trên trái)
/// - Scene/Day info
/// - "Nhấn E để tương tác" hint khi gần interactable
/// Singleton, DontDestroyOnLoad
/// </summary>
public class GameHUDManager : MonoBehaviour
{
    public static GameHUDManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private float interactionCheckRadius = 2f;
    [SerializeField] private LayerMask interactableLayers;

    [Header("UI References")]
    [SerializeField] private Canvas hudCanvas;
    [SerializeField] private Text locationText;
    [SerializeField] private Text sceneInfoText;
    [SerializeField] private GameObject interactionHintPanel;
    [SerializeField] private Text interactionHintText;

    [Header("Display Settings")]
    [SerializeField] private bool showLocation = true;
    [SerializeField] private bool showSceneInfo = true;
    [SerializeField] private bool showInteractionHint = true;

    // Location mapping
    private Dictionary<string, string> sceneLocationMap = new Dictionary<string, string>
    {
        { "HomeScene", "Nhà" },
        { "ClassroomScene", "Lớp học" },
        { "StreetScene", "Đường phố" },
        { "MainMenu", "" }
    };

    // Current state
    private string currentLocation = "";
    private int currentDay = 1;
    private bool isNearInteractable = false;
    private Transform playerTransform;

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
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Subscribe vào StoryManager để cập nhật khi ngày thay đổi
        SubscribeToStoryManager();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnsubscribeFromStoryManager();
    }

    private bool isSubscribedToStoryManager = false;

    private void SubscribeToStoryManager()
    {
        if (isSubscribedToStoryManager) return;
        
        if (StoryManager.Instance != null)
        {
            StoryManager.Instance.OnVariableChanged += OnStoryVariableChanged;
            isSubscribedToStoryManager = true;
        }
    }

    private void UnsubscribeFromStoryManager()
    {
        if (!isSubscribedToStoryManager) return;
        
        if (StoryManager.Instance != null)
        {
            StoryManager.Instance.OnVariableChanged -= OnStoryVariableChanged;
            isSubscribedToStoryManager = false;
        }
    }

    private void OnStoryVariableChanged(string variableName, int newValue)
    {
        // Cập nhật HUD khi current_day thay đổi
        if (variableName == StoryManager.VarKeys.CURRENT_DAY)
        {
            currentDay = newValue;
            if (sceneInfoText != null)
            {
                sceneInfoText.text = GetDayDisplayText(currentDay);
            }
            Debug.Log($"[GameHUD] Day updated to: {currentDay}");
        }
    }

    private void Start()
    {
        FindPlayer();
        UpdateLocationDisplay();
        UpdateSceneInfoDisplay();
        
        // Subscribe lại nếu StoryManager được tạo sau Awake
        SubscribeToStoryManager();
    }

    private void Update()
    {
        // Ẩn HUD khi ở MainMenu hoặc trong VN/Storytelling/Dialogue
        if (ShouldHideHUD())
        {
            SetHUDVisible(false);
            return;
        }

        SetHUDVisible(true);

        // Check interaction hint
        if (showInteractionHint)
        {
            CheckNearbyInteractables();
        }
    }


    #region UI Setup
    private void SetupUI()
    {
        if (hudCanvas == null)
        {
            hudCanvas = GetComponent<Canvas>();
            if (hudCanvas == null)
            {
                hudCanvas = gameObject.AddComponent<Canvas>();
            }
        }

        hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        hudCanvas.sortingOrder = 100; // Dưới dialogue/VN UI

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

        // Auto-create UI elements if not assigned
        if (locationText == null) CreateLocationText();
        if (sceneInfoText == null) CreateSceneInfoText();
        if (interactionHintPanel == null) CreateInteractionHintPanel();
    }

    private void CreateLocationText()
    {
        GameObject textObj = new GameObject("LocationText");
        textObj.transform.SetParent(transform, false);

        locationText = textObj.AddComponent<Text>();
        locationText.text = "";
        locationText.fontSize = 28;
        locationText.color = Color.white;
        locationText.alignment = TextAnchor.UpperLeft;
        locationText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Outline for better visibility
        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);

        // Shadow
        Shadow shadow = textObj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.5f);
        shadow.effectDistance = new Vector2(3, -3);

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(20, -20);
        rect.sizeDelta = new Vector2(400, 40);
    }

    private void CreateSceneInfoText()
    {
        GameObject textObj = new GameObject("SceneInfoText");
        textObj.transform.SetParent(transform, false);

        sceneInfoText = textObj.AddComponent<Text>();
        sceneInfoText.text = "";
        sceneInfoText.fontSize = 20;
        sceneInfoText.color = new Color(0.9f, 0.9f, 0.9f, 0.8f);
        sceneInfoText.alignment = TextAnchor.UpperLeft;
        sceneInfoText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(1, -1);

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(20, -55);
        rect.sizeDelta = new Vector2(300, 30);
    }

    private void CreateInteractionHintPanel()
    {
        interactionHintPanel = new GameObject("InteractionHintPanel");
        interactionHintPanel.transform.SetParent(transform, false);

        // Background
        Image bg = interactionHintPanel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.6f);

        RectTransform panelRect = interactionHintPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0);
        panelRect.anchorMax = new Vector2(0.5f, 0);
        panelRect.pivot = new Vector2(0.5f, 0);
        panelRect.anchoredPosition = new Vector2(0, 80);
        panelRect.sizeDelta = new Vector2(250, 45);

        // Text
        GameObject textObj = new GameObject("HintText");
        textObj.transform.SetParent(interactionHintPanel.transform, false);

        interactionHintText = textObj.AddComponent<Text>();
        interactionHintText.text = "Nhấn E để tương tác";
        interactionHintText.fontSize = 22;
        interactionHintText.color = Color.white;
        interactionHintText.alignment = TextAnchor.MiddleCenter;
        interactionHintText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        interactionHintPanel.SetActive(false);
    }
    #endregion


    #region Visibility Control
    private bool ShouldHideHUD()
    {
        // Hide in MainMenu
        if (SceneManager.GetActiveScene().name == mainMenuSceneName)
            return true;

        // Hide during VN mode
        if (VisualNovelManager.Instance != null && VisualNovelManager.Instance.IsVNModeActive)
            return true;

        // Hide during Storytelling
        if (StorytellingManager.Instance != null && StorytellingManager.Instance.IsPlaying)
            return true;

        // Hide during dialogue
        if (GameManager.Instance != null && GameManager.Instance.isInDialogue)
            return true;

        // Hide during scene transition
        if (GameManager.Instance != null && GameManager.Instance.isTransitioningScene)
            return true;

        // Hide when paused
        if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsPaused)
            return true;

        return false;
    }

    private void SetHUDVisible(bool visible)
    {
        if (locationText != null) locationText.gameObject.SetActive(visible && showLocation);
        if (sceneInfoText != null) sceneInfoText.gameObject.SetActive(visible && showSceneInfo);
        
        // Interaction hint có logic riêng
        if (!visible && interactionHintPanel != null)
        {
            interactionHintPanel.SetActive(false);
        }
    }
    #endregion

    #region Location & Scene Info
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
        UpdateLocationDisplay();
        UpdateSceneInfoDisplay();
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void UpdateLocationDisplay()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        
        if (sceneLocationMap.TryGetValue(sceneName, out string location))
        {
            currentLocation = location;
        }
        else
        {
            currentLocation = sceneName;
        }

        if (locationText != null)
        {
            locationText.text = currentLocation;
        }
    }

    private void UpdateSceneInfoDisplay()
    {
        // Lấy current_day từ StoryManager
        if (StoryManager.Instance != null)
        {
            currentDay = StoryManager.Instance.GetVariable(StoryManager.VarKeys.CURRENT_DAY);
        }

        if (sceneInfoText != null)
        {
            string dayText = GetDayDisplayText(currentDay);
            sceneInfoText.text = dayText;
        }
    }

    private string GetDayDisplayText(int day)
    {
        if (day <= 1) return "Ngày 1";
        if (day <= 7) return $"Ngày {day}";
        if (day <= 14) return "Tuần 1";
        if (day <= 21) return "Tuần 3";
        return "Ngày quyết định";
    }

    /// <summary>
    /// Cập nhật location thủ công (gọi từ bên ngoài)
    /// </summary>
    public void SetLocation(string location)
    {
        currentLocation = location;
        if (locationText != null)
        {
            locationText.text = location;
        }
    }

    /// <summary>
    /// Cập nhật scene info thủ công
    /// </summary>
    public void SetSceneInfo(string info)
    {
        if (sceneInfoText != null)
        {
            sceneInfoText.text = info;
        }
    }

    /// <summary>
    /// Refresh display (gọi khi story variables thay đổi)
    /// </summary>
    public void RefreshDisplay()
    {
        UpdateLocationDisplay();
        UpdateSceneInfoDisplay();
    }
    #endregion


    #region Interaction Hint
    private void CheckNearbyInteractables()
    {
        if (playerTransform == null)
        {
            FindPlayer();
            if (playerTransform == null)
            {
                SetInteractionHintVisible(false);
                return;
            }
        }

        bool foundInteractable = false;

        // Check NPCs
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        foreach (var npc in npcs)
        {
            if (Vector2.Distance(playerTransform.position, npc.transform.position) <= interactionCheckRadius)
            {
                NPCInteraction npcInteraction = npc.GetComponent<NPCInteraction>();
                if (npcInteraction != null && npcInteraction.enabled)
                {
                    foundInteractable = true;
                    break;
                }
            }
        }

        // Check Chairs
        if (!foundInteractable)
        {
            GameObject[] chairs = GameObject.FindGameObjectsWithTag("Chair");
            foreach (var chair in chairs)
            {
                if (Vector2.Distance(playerTransform.position, chair.transform.position) <= interactionCheckRadius)
                {
                    foundInteractable = true;
                    break;
                }
            }
        }

        // Check objects with SimpleInteractionPrompt
        if (!foundInteractable)
        {
            SimpleInteractionPrompt[] prompts = FindObjectsByType<SimpleInteractionPrompt>(FindObjectsSortMode.None);
            foreach (var prompt in prompts)
            {
                if (Vector2.Distance(playerTransform.position, prompt.transform.position) <= interactionCheckRadius)
                {
                    foundInteractable = true;
                    break;
                }
            }
        }

        // Check BedInteraction
        if (!foundInteractable)
        {
            BedInteraction[] beds = FindObjectsByType<BedInteraction>(FindObjectsSortMode.None);
            foreach (var bed in beds)
            {
                if (Vector2.Distance(playerTransform.position, bed.transform.position) <= interactionCheckRadius)
                {
                    foundInteractable = true;
                    break;
                }
            }
        }

        // Check SceneTransition (doors)
        if (!foundInteractable)
        {
            SceneTransition[] doors = FindObjectsByType<SceneTransition>(FindObjectsSortMode.None);
            foreach (var door in doors)
            {
                if (Vector2.Distance(playerTransform.position, door.transform.position) <= interactionCheckRadius)
                {
                    foundInteractable = true;
                    break;
                }
            }
        }

        SetInteractionHintVisible(foundInteractable);
    }

    private void SetInteractionHintVisible(bool visible)
    {
        if (isNearInteractable == visible) return;

        isNearInteractable = visible;
        if (interactionHintPanel != null)
        {
            interactionHintPanel.SetActive(visible && showInteractionHint);
        }
    }

    /// <summary>
    /// Ẩn interaction hint (gọi khi đang tương tác)
    /// </summary>
    public void HideInteractionHint()
    {
        if (interactionHintPanel != null)
        {
            interactionHintPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Hiện interaction hint
    /// </summary>
    public void ShowInteractionHint()
    {
        if (interactionHintPanel != null && isNearInteractable && showInteractionHint)
        {
            interactionHintPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Đặt text cho interaction hint
    /// </summary>
    public void SetInteractionHintText(string text)
    {
        if (interactionHintText != null)
        {
            interactionHintText.text = text;
        }
    }
    #endregion

    #region Public Settings
    public void SetShowLocation(bool show)
    {
        showLocation = show;
        if (locationText != null)
        {
            locationText.gameObject.SetActive(show);
        }
    }

    public void SetShowSceneInfo(bool show)
    {
        showSceneInfo = show;
        if (sceneInfoText != null)
        {
            sceneInfoText.gameObject.SetActive(show);
        }
    }

    public void SetShowInteractionHint(bool show)
    {
        showInteractionHint = show;
        if (!show && interactionHintPanel != null)
        {
            interactionHintPanel.SetActive(false);
        }
    }
    #endregion
}
