using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// VisualNovelManager - Quản lý chế độ Visual Novel
/// Hiển thị background tĩnh + dialogue overlay
/// Singleton, DontDestroyOnLoad
/// </summary>
public class VisualNovelManager : MonoBehaviour
{
    #region Singleton
    private static VisualNovelManager _instance;
    private static bool _applicationIsQuitting = false;

    public static VisualNovelManager Instance
    {
        get
        {
            // QUAN TRỌNG: Nếu đang quitting thì return null
            if (_applicationIsQuitting) 
            {
                Debug.LogWarning("[VNManager] Instance requested but application is quitting!");
                return null;
            }

            if (_instance == null)
            {
                _instance = FindFirstObjectByType<VisualNovelManager>();
                if (_instance == null && !_applicationIsQuitting)
                {
                    GameObject go = new GameObject("VisualNovelManager");
                    _instance = go.AddComponent<VisualNovelManager>();
                    Debug.Log("[VNManager] Auto-created VisualNovelManager instance");
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Reset application quitting flag - gọi khi domain reload (Enter Play Mode)
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticFields()
    {
        Debug.Log("[VNManager] ResetStaticFields called - resetting singleton state");
        _applicationIsQuitting = false;
        _instance = null;
    }

    /// <summary>
    /// Đảm bảo singleton được reset khi scene load mới
    /// Gọi sau khi scene load để fix timing issues
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnAfterSceneLoad()
    {
        // Reset quitting flag khi scene load mới (trong trường hợp bị stuck)
        if (_applicationIsQuitting)
        {
            Debug.LogWarning("[VNManager] _applicationIsQuitting was true after scene load - resetting!");
            _applicationIsQuitting = false;
        }
    }
    #endregion

    #region UI References
    [Header("Canvas & Panels")]
    [SerializeField] private Canvas vnCanvas;
    [SerializeField] private GameObject vnPanel; // Panel chứa toàn bộ VN UI
    [SerializeField] private Image backgroundImage; // Ảnh nền fullscreen
    [SerializeField] private Text locationText; // Text hiển thị địa điểm

    [Header("Character Display")]
    [SerializeField] private Transform characterContainer; // Container cho character sprites
    [SerializeField] private GameObject characterPrefab; // Prefab cho character display

    [Header("Dialogue Integration")]
    [SerializeField] private DialogueSystem dialogueSystem; // Reference đến DialogueSystem

    [Header("Day 1 Scene References")]
    [Tooltip("Scene 7A - Confrontation (VN mode)")]
    [SerializeField] private VNSceneData day1Scene7A;
    
    [Header("Ending Storytelling References")]
    [Tooltip("Ending 1 - Good StandUp")]
    [SerializeField] private StorytellingSequenceData ending1_GoodStandUp;
    [Tooltip("Ending 2 - True TellParents")]
    [SerializeField] private StorytellingSequenceData ending2_TrueTellParents;
    [Tooltip("Ending 3 - Bad DarkLife")]
    [SerializeField] private StorytellingSequenceData ending3_BadDarkLife;
    #endregion
    
    #region Public Getters for Endings
    private const string ENDING1_PATH = "Storytelling/Ending1_GoodStandUp_Sequence";
    private const string ENDING2_PATH = "Storytelling/Ending2_TrueTellParents_Sequence";
    private const string ENDING3_PATH = "Storytelling/Ending3_BadDarkLife_Sequence";
    
    public StorytellingSequenceData GetEnding1Data()
    {
        if (ending1_GoodStandUp == null)
        {
            ending1_GoodStandUp = Resources.Load<StorytellingSequenceData>(ENDING1_PATH);
            if (ending1_GoodStandUp == null)
            {
                Debug.LogError($"[VNManager] Ending 1 not found at Resources/{ENDING1_PATH}! Please move asset to Resources folder.");
            }
            else
            {
                Debug.Log($"[VNManager] Loaded Ending 1 from Resources: {ending1_GoodStandUp.sequenceName}");
            }
        }
        return ending1_GoodStandUp;
    }
    
    public StorytellingSequenceData GetEnding2Data()
    {
        if (ending2_TrueTellParents == null)
        {
            ending2_TrueTellParents = Resources.Load<StorytellingSequenceData>(ENDING2_PATH);
            if (ending2_TrueTellParents == null)
            {
                Debug.LogError($"[VNManager] Ending 2 not found at Resources/{ENDING2_PATH}! Please move asset to Resources folder.");
            }
            else
            {
                Debug.Log($"[VNManager] Loaded Ending 2 from Resources: {ending2_TrueTellParents.sequenceName}");
            }
        }
        return ending2_TrueTellParents;
    }
    
    public StorytellingSequenceData GetEnding3Data()
    {
        if (ending3_BadDarkLife == null)
        {
            ending3_BadDarkLife = Resources.Load<StorytellingSequenceData>(ENDING3_PATH);
            if (ending3_BadDarkLife == null)
            {
                Debug.LogError($"[VNManager] Ending 3 not found at Resources/{ENDING3_PATH}! Please move asset to Resources folder.");
            }
            else
            {
                Debug.Log($"[VNManager] Loaded Ending 3 from Resources: {ending3_BadDarkLife.sequenceName}");
            }
        }
        return ending3_BadDarkLife;
    }
    #endregion

    #region Settings
    [Header("Settings")]
    [SerializeField] private int canvasSortOrder = 300; // Trên DialogueSystem (200)
    #endregion

    #region State
    private bool isVNModeActive = false;
    private VNSceneData currentScene;
    private Action onVNComplete;
    private Vector3 savedPlayerPosition; // Lưu vị trí player trước khi vào VN mode
    private bool skipRestorePosition = false; // Khi surround_player được trigger, không restore position
    #endregion

    #region Character Positions
    private readonly Vector2[] characterPositions = new Vector2[]
    {
        new Vector2(-400f, 0f),  // Left
        new Vector2(0f, 0f),  // Center
        new Vector2(400f, 0f),   // Right
        new Vector2(-600f, 0f),  // FarLeft
        new Vector2(600f, 0f)    // FarRight
    };
    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        SetupUI();
        SetupDialogueSystem();
    }

    private void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
        Debug.Log("[VNManager] Application quitting - setting flag");
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            // CHỈ set _applicationIsQuitting khi thực sự quit application
            // KHÔNG set khi scene unload hoặc object bị destroy bình thường
            // Vì DontDestroyOnLoad, OnDestroy chỉ được gọi khi quit hoặc manual destroy
            
            // Kiểm tra xem có đang trong quá trình quit không
            // Nếu không phải quit, chỉ clear instance reference
            if (!_applicationIsQuitting)
            {
                Debug.Log("[VNManager] OnDestroy called but not quitting - clearing instance only");
                _instance = null;
            }
            else
            {
                Debug.Log("[VNManager] OnDestroy called during quit");
            }
        }
    }

    private void SetupDialogueSystem()
    {
        if (dialogueSystem == null)
        {
            dialogueSystem = FindFirstObjectByType<DialogueSystem>();
            if (dialogueSystem == null)
            {
                Debug.LogWarning("[VNManager] DialogueSystem not found in scene! VN dialogue will not work.");
            }
            else
            {
                Debug.Log("[VNManager] DialogueSystem found and linked!");
            }
        }
    }

    private void SetupUI()
    {
        // Use ReferenceEquals for proper Unity null check (handles destroyed objects)
        if (vnCanvas == null || !vnCanvas)
        {
            vnCanvas = GetComponentInChildren<Canvas>();
            if (vnCanvas == null || !vnCanvas)
            {
                GameObject canvasObj = new GameObject("VNCanvas");
                canvasObj.transform.SetParent(transform); // Parent to this (DontDestroyOnLoad)
                vnCanvas = canvasObj.AddComponent<Canvas>();
                vnCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                vnCanvas.sortingOrder = canvasSortOrder;

                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;

                canvasObj.AddComponent<GraphicRaycaster>();
            }
        }

        if (vnPanel == null || !vnPanel)
        {
            vnPanel = new GameObject("VNPanel");
            vnPanel.transform.SetParent(vnCanvas.transform, false);
            RectTransform panelRect = vnPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
        }

        if (backgroundImage == null || !backgroundImage)
        {
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(vnPanel.transform, false);
            backgroundImage = bgObj.AddComponent<Image>();
            RectTransform bgRect = backgroundImage.rectTransform;
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            backgroundImage.color = Color.black;
        }

        if (characterContainer == null || !characterContainer)
        {
            Debug.Log("[VNManager] Creating CharacterContainer...");
            GameObject charContObj = new GameObject("CharacterContainer");
            charContObj.transform.SetParent(vnPanel.transform, false);

            // Add RectTransform FIRST, then get the transform reference
            RectTransform charRect = charContObj.AddComponent<RectTransform>();
            charRect.anchorMin = Vector2.zero;
            charRect.anchorMax = Vector2.one;
            charRect.sizeDelta = Vector2.zero;

            characterContainer = charContObj.transform;
            Debug.Log($"[VNManager] CharacterContainer created: {characterContainer != null}, gameObject={charContObj != null}");
        }

        // Ensure characterContainer is last sibling (renders on top of background)
        if (characterContainer != null)
        {
            characterContainer.SetAsLastSibling();
        }

        vnPanel.SetActive(false);
        Debug.Log($"[VNManager] SetupUI complete: canvas={(vnCanvas != null && vnCanvas)}, panel={(vnPanel != null && vnPanel)}, bg={(backgroundImage != null && backgroundImage)}, charContainer={(characterContainer != null && characterContainer)}");
    }

    #region Public API
    public void StartVNScene(VNSceneData sceneData, Action onComplete = null)
    {
        if (sceneData == null)
        {
            Debug.LogError("[VNManager] Scene data is null!");
            onComplete?.Invoke(); // Gọi callback ngay để không bị stuck
            return;
        }

        if (!sceneData.CanShow())
        {
            Debug.Log("[VNManager] Scene conditions not met, skipping.");
            onComplete?.Invoke();
            return;
        }

        // QUAN TRỌNG: Nếu VN mode đang active, chuyển sang scene mới ngay
        // (Dùng cho trường hợp dialogue → surround → VN scene tiếp theo)
        if (isVNModeActive)
        {
            Debug.Log($"[VNManager] VN mode đang active, chuyển sang scene mới: {sceneData.name}");
            currentScene = sceneData;
            onVNComplete = onComplete;
            sceneData.ApplyOnEnterEffects();
            StartCoroutine(TransitionBetweenScenes());
            return;
        }

        Debug.Log($"[VNManager] Starting VN scene: {sceneData.name}");
        currentScene = sceneData;
        onVNComplete = onComplete;
        sceneData.ApplyOnEnterEffects();
        StartCoroutine(TransitionToVNMode());
    }

    public void EndVNMode()
    {
        StartCoroutine(TransitionFromVNMode());
    }

    public bool IsVNModeActive => isVNModeActive;
    #endregion

    #region Scene Display
    private void DisplayScene(VNScene scene)
    {
        // Set background
        if (backgroundImage != null)
        {
            backgroundImage.sprite = scene.backgroundImage;

            // Fix transparent background tint
            Color tint = scene.backgroundTint;
            if (tint.a < 0.1f)
            {
                Debug.LogWarning($"[VNManager] Background tint too transparent (a={tint.a}), forcing to white");
                tint = Color.white;
            }

            backgroundImage.color = tint;
            // Không preserve aspect để background full màn hình
            backgroundImage.preserveAspect = false;
            backgroundImage.type = Image.Type.Simple;

            Debug.Log($"[VNManager] Background set: sprite={scene.backgroundImage != null}, tint={tint}");
        }

        // Set location text
        if (locationText != null && !string.IsNullOrEmpty(scene.locationText))
        {
            locationText.text = scene.locationText;
            locationText.gameObject.SetActive(true);
        }
        else if (locationText != null)
        {
            locationText.gameObject.SetActive(false);
        }

        // Display characters
        ClearCharacters();

        // Ensure characterContainer is active and in front of background
        if (characterContainer != null)
        {
            characterContainer.gameObject.SetActive(true);
            characterContainer.SetAsLastSibling(); 
            // Đảm bảo render phía trên background
            Debug.Log($"[VNManager] CharacterContainer active: {characterContainer.gameObject.activeSelf}, sibling index: {characterContainer.GetSiblingIndex()}");
        }

        if (scene.characters != null && scene.characters.Length > 0)
        {
            Debug.Log($"[VNManager] Displaying {scene.characters.Length} characters");
            foreach (var charDisplay in scene.characters)
            {
                DisplayCharacter(charDisplay);
            }
        }
        else
        {
            Debug.Log("[VNManager] No characters to display");
        }

        // Start dialogue - Re-find DialogueSystem in case it was recreated after scene load
        if (dialogueSystem == null || !dialogueSystem)
        {
            dialogueSystem = FindFirstObjectByType<DialogueSystem>();
            if (dialogueSystem != null)
            {
                Debug.Log("[VNManager] DialogueSystem re-linked after scene load!");
            }
        }

        if (scene.dialogue != null && dialogueSystem != null)
        {
            // Subscribe to speaker change event
            dialogueSystem.OnSpeakerChanged -= OnSpeakerChanged; // Unsubscribe first to avoid duplicates
            dialogueSystem.OnSpeakerChanged += OnSpeakerChanged;

            dialogueSystem.StartDialogueWithChoices(
                scene.dialogue,
                OnDialogueComplete,
                OnDialogueAction,
                OnVNSceneTransition,
                () => EndVNMode(),
                OnTopDownSceneTransition);
        }
        else
        {
            Debug.LogWarning($"[VNManager] No dialogue or DialogueSystem! dialogue={scene.dialogue != null}, dialogueSystem={dialogueSystem != null}");
            OnDialogueComplete();
        }
    }

    /// <summary>
    /// Callback khi speaker thay đổi - ẩn/hiện character tương ứng
    /// </summary>
    private void OnSpeakerChanged(string speakerName)
    {
        if (characterContainer == null) return;

        foreach (Transform child in characterContainer)
        {
            // Hiện character nếu tên match với speaker, hoặc nếu speaker rỗng (narration) thì ẩn tất cả
            bool shouldShow = !string.IsNullOrEmpty(speakerName) &&
                              child.name.Equals(speakerName, StringComparison.OrdinalIgnoreCase);
            child.gameObject.SetActive(shouldShow);
        }
    }

    private void DisplayCharacter(VNCharacterDisplay charDisplay)
    {
        if (charDisplay.characterSprite == null)
        {
            Debug.LogWarning($"[VNManager] Character '{charDisplay.characterName}' has no sprite!");
            return;
        }

        Debug.Log($"[VNManager] Displaying character: {charDisplay.characterName}, position: {charDisplay.position}");

        // Ensure characterContainer exists (should be created by SetupUI)
        if (characterContainer == null || !characterContainer)
        {
            Debug.LogError("[VNManager] characterContainer is NULL! This should not happen after SetupUI.");
            return;
        }

        GameObject charObj;
        if (characterPrefab != null)
        {
            charObj = Instantiate(characterPrefab, characterContainer);
        }
        else
        {
            charObj = new GameObject(charDisplay.characterName ?? "Character");
            charObj.transform.SetParent(characterContainer, false);
            var img = charObj.AddComponent<Image>();
            // Add RectTransform explicitly
            if (charObj.GetComponent<RectTransform>() == null)
            {
                charObj.AddComponent<RectTransform>();
            }
        }

        Image charImage = charObj.GetComponent<Image>();
        if (charImage != null)
        {
            charImage.sprite = charDisplay.characterSprite;
            charImage.color = Color.white; // Đảm bảo character hiển thị đúng màu gốc
            charImage.preserveAspect = true;
            charImage.raycastTarget = false;

            Debug.Log($"[VNManager] Character Image created: sprite={charImage.sprite != null}, color={charImage.color}, enabled={charImage.enabled}");
        }
        else
        {
            Debug.LogError($"[VNManager] Failed to get Image component for character '{charDisplay.characterName}'!");
        }

        // Ensure character is active
        charObj.SetActive(true);

        RectTransform rect = charObj.GetComponent<RectTransform>();
        if (rect != null)
        {
            // Set anchor to bottom-center so character stands from bottom
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f); // Pivot at bottom-center

            // Set size based on sprite dimensions FIRST
            float charWidth = 400f;
            float charHeight = 900f;
            if (charDisplay.characterSprite != null)
            {
                float spriteWidth = charDisplay.characterSprite.rect.width;
                float spriteHeight = charDisplay.characterSprite.rect.height;

                // Limit to reasonable UI size (max 900px tall for 1080p screens)
                float maxHeight = 900f;
                float scaleRatio = Mathf.Min(1f, maxHeight / spriteHeight);

                charWidth = spriteWidth * scaleRatio;
                charHeight = spriteHeight * scaleRatio;
                rect.sizeDelta = new Vector2(charWidth, charHeight);

                Debug.Log($"[VNManager] Character size: {rect.sizeDelta}, sprite: {spriteWidth}x{spriteHeight}, scale: {scaleRatio}");
            }

            // Set position (X offset from center, Y offset from bottom)
            Vector2 basePos = charDisplay.position == VNCharacterDisplay.CharacterPosition.Custom
                ? charDisplay.positionOffset
                : characterPositions[(int)charDisplay.position] + charDisplay.positionOffset;

            // Y = 0 means bottom of screen, add small offset to not clip
            rect.anchoredPosition = new Vector2(basePos.x, Mathf.Max(0f, basePos.y));

            rect.localScale = new Vector3(
                charDisplay.scale.x * (charDisplay.flipX ? -1 : 1),
                charDisplay.scale.y, 1f);

            Debug.Log($"[VNManager] Character final position: anchoredPos={rect.anchoredPosition}, worldPos={rect.position}, parent={rect.parent?.name}");
        }

        // Log hierarchy info
        Debug.Log($"[VNManager] Character '{charDisplay.characterName}' created under '{charObj.transform.parent?.name}', active={charObj.activeSelf}, activeInHierarchy={charObj.activeInHierarchy}");
    }

    private void ClearCharacters()
    {
        if (characterContainer == null) return;
        foreach (Transform child in characterContainer)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion

    #region Transitions
    private IEnumerator TransitionToVNMode()
    {
        isVNModeActive = true;

        // Ensure UI is setup (recreate if destroyed after scene load)
        SetupUI();

        // QUAN TRỌNG: Lưu vị trí player và dừng tất cả NPC follow TRƯỚC KHI disable player
        // Điều này ngăn NPC đẩy player đi trong khi VN mode đang chạy
        SavePlayerPositionAndStopFollowers();

        // Disable player
        DisablePlayerControls();

        // Fade out current scene
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeOutCoroutine();
        }

        // Show VN panel
        vnPanel.SetActive(true);

        // Display the scene
        DisplayScene(currentScene.sceneData);

        // Fade in
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeInCoroutine();
        }
    }

    /// <summary>
    /// Lưu vị trí player và dừng tất cả NPC đang follow
    /// Gọi TRƯỚC KHI disable player controls
    /// </summary>
    private void SavePlayerPositionAndStopFollowers()
    {
        // Lưu vị trí player
        if (cachedPlayer == null)
        {
            cachedPlayer = GameObject.FindGameObjectWithTag("Player");
        }

        if (cachedPlayer != null)
        {
            savedPlayerPosition = cachedPlayer.transform.position;
            Debug.Log($"[VNManager] Saved player position: {savedPlayerPosition}");
        }

        // Dừng tất cả NPCFollowPlayer ngay lập tức
        // Điều này ngăn NPC tiếp tục di chuyển và đẩy player
        NPCFollowPlayer[] followers = FindObjectsByType<NPCFollowPlayer>(FindObjectsSortMode.None);
        if (followers != null && followers.Length > 0)
        {
            foreach (var follower in followers)
            {
                if (follower != null && follower.enabled)
                {
                    follower.StopFollowing();
                }
            }
            Debug.Log($"[VNManager] Stopped {followers.Length} NPCFollowPlayer(s) on VN enter");
        }
    }

    private IEnumerator TransitionFromVNMode()
    {
        // Fade out
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeOutCoroutine();
        }

        // Hide VN panel
        vnPanel.SetActive(false);
        ClearCharacters();

        isVNModeActive = false;

        // Dừng bullies nếu player đã confronted hoặc ran away
        if (StoryManager.Instance != null)
        {
            bool confronted = StoryManager.Instance.GetFlag("confronted_bullies");
            bool ranAway = StoryManager.Instance.GetFlag("ran_from_bullies");

            if (confronted || ranAway)
            {
                StopAllBulliesFollowing();
                Debug.Log($"[VNManager] Dừng bullies - confronted={confronted}, ranAway={ranAway}");
            }
        }

        // Check if we need to load top-down scene
        if (currentScene != null && currentScene.sceneData.returnToTopDown)
        {
            string targetSceneName = currentScene.sceneData.topDownSceneName;
            string spawnId = currentScene.sceneData.spawnPointId;
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            // CASE 1: Load different scene
            if (!string.IsNullOrEmpty(targetSceneName) && targetSceneName != currentSceneName && GameManager.Instance != null)
            {
                Debug.Log($"[VNManager] Loading different scene: {targetSceneName} with spawn: {spawnId}");
                GameManager.Instance.LoadScene(targetSceneName, spawnId);
            }
            // CASE 2: Same scene + has spawn point → Teleport player
            else if (!string.IsNullOrEmpty(targetSceneName) && targetSceneName == currentSceneName && !string.IsNullOrEmpty(spawnId))
            {
                Debug.Log($"[VNManager] Same scene, teleporting to spawn point: {spawnId}");
                yield return TeleportPlayerToSpawnPoint(spawnId);
            }
            // CASE 3: Same scene + no spawn point → Restore to saved position (hoặc skip nếu surround đang chạy)
            else
            {
                if (skipRestorePosition)
                {
                    // Surround đang chạy - KHÔNG restore position, chỉ enable controls
                    Debug.Log("[VNManager] Skip restore position (surround active)");
                    skipRestorePosition = false; // Reset flag
                    EnablePlayerControls();
                }
                else
                {
                    Debug.Log($"[VNManager] Same scene, restoring to saved position: {savedPlayerPosition}");
                    RestorePlayerToSavedPosition();
                    EnablePlayerControls();
                }

                if (ScreenFader.Instance != null)
                {
                    yield return ScreenFader.Instance.FadeInCoroutine();
                }
            }
        }
        else
        {
            EnablePlayerControls();
            if (ScreenFader.Instance != null)
            {
                yield return ScreenFader.Instance.FadeInCoroutine();
            }
        }

        onVNComplete?.Invoke();
        currentScene = null;
    }

    /// <summary>
    /// Restore player về vị trí đã lưu trước khi vào VN mode
    /// </summary>
    private void RestorePlayerToSavedPosition()
    {
        if (cachedPlayer == null)
        {
            cachedPlayer = GameObject.FindGameObjectWithTag("Player");
        }

        if (cachedPlayer != null && savedPlayerPosition != Vector3.zero)
        {
            cachedPlayer.transform.position = savedPlayerPosition;

            // Snap camera về vị trí mới
            CameraHelper.SnapCameraToTarget(cachedPlayer.transform);

            Debug.Log($"[VNManager] Restored player to saved position: {savedPlayerPosition}");
        }
    }

    /// <summary>
    /// Teleport player to spawn point in current scene (without reloading scene)
    /// </summary>
    private IEnumerator TeleportPlayerToSpawnPoint(string spawnId)
    {
        // Find SpawnManager in current scene
        SpawnManager spawnManager = FindFirstObjectByType<SpawnManager>();
        if (spawnManager == null)
        {
            Debug.LogWarning("[VNManager] SpawnManager not found, staying at current position");
            EnablePlayerControls();
            if (ScreenFader.Instance != null)
            {
                yield return ScreenFader.Instance.FadeInCoroutine();
            }
            yield break;
        }

        // Enable player first (need to be active to move)
        EnablePlayerControls();

        // Find player
        if (cachedPlayer == null)
        {
            cachedPlayer = GameObject.FindGameObjectWithTag("Player");
        }

        if (cachedPlayer == null)
        {
            Debug.LogWarning("[VNManager] Player not found, cannot teleport");
            if (ScreenFader.Instance != null)
            {
                yield return ScreenFader.Instance.FadeInCoroutine();
            }
            yield break;
        }

        // Teleport player to spawn point
        spawnManager.SpawnPlayer(cachedPlayer, spawnId);

        // Snap camera to new position
        CameraHelper.SnapCameraToTarget(cachedPlayer.transform);

        // Fade in
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeInCoroutine();
        }

        Debug.Log($"[VNManager] Player teleported to spawn point: {spawnId}");
    }
    #endregion

    #region Dialogue Callbacks
    private void OnDialogueComplete()
    {
        // Unsubscribe from speaker change event
        if (dialogueSystem != null)
        {
            dialogueSystem.OnSpeakerChanged -= OnSpeakerChanged;
        }

        // Apply on complete effects for current scene
        if (currentScene != null)
        {
            currentScene.ApplyOnCompleteEffects();
        }

        // Check for next scene
        if (currentScene != null && currentScene.sceneData.nextScene != null)
        {
            var nextScene = currentScene.sceneData.nextScene;
            if (nextScene.CanShow())
            {
                currentScene = nextScene;
                nextScene.ApplyOnEnterEffects();
                StartCoroutine(TransitionBetweenScenes());
                return;
            }
        }

        // End VN mode
        EndVNMode();
    }

    private void OnDialogueAction(string actionId)
    {
        Debug.Log($"[VNManager] Dialogue action: {actionId}");
        // Handle special actions like ending triggers
        switch (actionId)
        {
            case "end_vn_mode":
                EndVNMode();
                break;
            case "trigger_good_ending":
                StoryManager.Instance?.SetFlag("stood_up_to_bullies", true);
                break;
            case "trigger_true_ending":
                StoryManager.Instance?.SetFlag(StoryManager.FlagKeys.CONFESSED_TO_MOM, true);
                break;
            case "trigger_bad_murder":
                StoryManager.Instance?.SetFlag(StoryManager.FlagKeys.BROUGHT_KNIFE, true);
                break;

            // NPC Surround Player - Cảnh 14A-1: Bị vây quanh
            case "surround_player":
                TriggerNPCSurround();
                break;

            // Critical Day - Fight Cutscene
            case "trigger_fight_cutscene":
                TriggerFightCutscene();
                break;

            // Critical Day - Scene 28B (Về nhà sau bị đánh)
            case "trigger_scene28b":
                TriggerScene28B();
                break;

            // Critical Day - Scene 28A (Về nhà sau thắng fight)
            case "trigger_scene28a":
                TriggerScene28A();
                break;

            // Endings
            case "trigger_ending1_storytelling":
                TriggerEnding1();
                break;
            case "trigger_ending2_storytelling":
                TriggerEnding2();
                break;
            case "trigger_ending3_storytelling":
                TriggerEnding3();
                break;

            // Day 1 Scene 6 choices (DEPRECATED - use nextVNScene instead)
            case "trigger_scene7a":
                HandleTriggerScene7A();
                break;
            case "trigger_scene7b":
                HandleTriggerScene7B();
                break;
        }
    }

    /// <summary>
    /// Trigger NPCSurroundPlayer để NPCs vây quanh player
    /// </summary>
    private void TriggerNPCSurround()
    {
        Debug.Log("[VNManager] ========== TRIGGER NPC SURROUND ==========");

        // QUAN TRỌNG: Cập nhật savedPlayerPosition về vị trí HIỆN TẠI của player
        // Vì player có thể đã di chuyển từ khi VN mode bắt đầu
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            savedPlayerPosition = playerObj.transform.position;
            Debug.Log($"[VNManager] Updated savedPlayerPosition to: {savedPlayerPosition}");
        }

        // Set flag để không restore position khi VN mode kết thúc
        skipRestorePosition = true;

        // QUAN TRỌNG: Kết thúc VN mode TRƯỚC KHI trigger surround
        // Điều này sẽ:
        // 1. Ẩn dialogue panel
        // 2. Enable player controls (renderer, collider)
        // 3. NHƯNG NPCSurroundPlayer sẽ LOCK movement ngay sau đó
        Debug.Log("[VNManager] Ending VN mode before surround...");
        EndVNMode();

        // Delay nhỏ để đảm bảo VN mode đã kết thúc hoàn toàn
        StartCoroutine(TriggerSurroundAfterDelay());
    }

    private System.Collections.IEnumerator TriggerSurroundAfterDelay()
    {
        // Đợi 1 frame để VN mode kết thúc hoàn toàn
        yield return null;

        Debug.Log("[VNManager] Triggering surround now...");

        NPCSurroundPlayer surroundPlayer = FindFirstObjectByType<NPCSurroundPlayer>();
        if (surroundPlayer != null)
        {
            surroundPlayer.StartSurrounding();
            Debug.Log("[VNManager] ✓ Triggered NPCSurroundPlayer.StartSurrounding()");
        }
        else
        {
            // Thử tìm NPCSurroundController (deprecated)
            NPCSurroundController controller = FindFirstObjectByType<NPCSurroundController>();
            if (controller != null)
            {
                controller.StartSurround();
                Debug.Log("[VNManager] ✓ Triggered NPCSurroundController.StartSurround()");
            }
            else
            {
                Debug.LogError("[VNManager] ✗ NPCSurroundPlayer/Controller not found in scene!");
            }
        }
    }

    #region Critical Day Action Handlers

    /// <summary>
    /// Trigger Fight Cutscene (Storytelling)
    /// </summary>
    private void TriggerFightCutscene()
    {
        Debug.Log("[VNManager] Triggering Fight Cutscene...");

        // Load StorytellingSequenceData
        StorytellingSequenceData fightSequence = Resources.Load<StorytellingSequenceData>("Storytelling/fightScene");

        if (fightSequence != null)
        {
            // End VN mode first
            EndVNMode();

            // Start storytelling sequence
            StorytellingManager.Instance.PlaySequence(fightSequence, () =>
            {
                Debug.Log("[VNManager] Fight Cutscene completed!");
                // Sequence tự động chuyển về HomeScene nhờ Next Scene Name
            });
        }
        else
        {
            Debug.LogError("[VNManager] Fight Cutscene not found at Resources/Storytelling/fightScene!");
        }
    }

    /// <summary>
    /// Trigger Scene 28B (Về nhà sau bị đánh)
    /// </summary>
    private void TriggerScene28B()
    {
        Debug.Log("[VNManager] Triggering Scene 28B...");
        EndVNMode();
        GameManager.Instance?.LoadScene("HomeScene", "after_beaten");
    }

    /// <summary>
    /// Trigger Scene 28A (Về nhà sau thắng fight)
    /// </summary>
    private void TriggerScene28A()
    {
        Debug.Log("[VNManager] Triggering Scene 28A...");
        EndVNMode();
        GameManager.Instance?.LoadScene("HomeScene", "after_fight");
    }

    /// <summary>
    /// Trigger Ending 1 Storytelling
    /// </summary>
    private void TriggerEnding1()
    {
        Debug.Log("[VNManager] Triggering Ending 1...");
        if (ending1_GoodStandUp != null)
        {
            EndVNMode();
            StorytellingManager.Instance.PlaySequence(ending1_GoodStandUp);
        }
        else
        {
            Debug.LogError("[VNManager] Ending 1 not assigned! Please assign in Inspector.");
        }
    }

    /// <summary>
    /// Trigger Ending 2 Storytelling
    /// </summary>
    private void TriggerEnding2()
    {
        Debug.Log("[VNManager] Triggering Ending 2...");
        if (ending2_TrueTellParents != null)
        {
            EndVNMode();
            StorytellingManager.Instance.PlaySequence(ending2_TrueTellParents);
        }
        else
        {
            Debug.LogError("[VNManager] Ending 2 not assigned! Please assign in Inspector.");
        }
    }

    /// <summary>
    /// Trigger Ending 3 Storytelling
    /// </summary>
    private void TriggerEnding3()
    {
        Debug.Log("[VNManager] Triggering Ending 3...");
        if (ending3_BadDarkLife != null)
        {
            EndVNMode();
            StorytellingManager.Instance.PlaySequence(ending3_BadDarkLife);
        }
        else
        {
            Debug.LogError("[VNManager] Ending 3 not assigned! Please assign in Inspector.");
        }
    }

    #endregion

    /// <summary>
    /// Handle VN scene transition from dialogue choice
    /// </summary>
    private void OnVNSceneTransition(VNSceneData nextScene)
    {
        if (nextScene == null)
        {
            Debug.LogError("[VNManager] nextVNScene is null!");
            return;
        }

        if (!nextScene.CanShow())
        {
            Debug.Log($"[VNManager] Scene '{nextScene.name}' conditions not met, ending VN mode.");
            EndVNMode();
            return;
        }

        Debug.Log($"[VNManager] Transitioning to VN scene: {nextScene.name}");
        currentScene = nextScene;
        nextScene.ApplyOnEnterEffects();
        StartCoroutine(TransitionBetweenScenes());
    }

    /// <summary>
    /// Handle top-down scene transition from dialogue choice/node
    /// Kết thúc VN mode và chuyển sang scene top-down
    /// </summary>
    private void OnTopDownSceneTransition(string sceneName, string spawnPointId)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[VNManager] OnTopDownSceneTransition: sceneName is null or empty!");
            return;
        }

        Debug.Log($"[VNManager] Top-down scene transition: {sceneName}, spawn: {spawnPointId}");

        // Kết thúc VN mode trước
        isVNModeActive = false;
        
        // Cleanup VN UI
        if (vnCanvas != null) vnCanvas.gameObject.SetActive(false);
        if (backgroundImage != null) backgroundImage.gameObject.SetActive(false);
        if (characterContainer != null)
        {
            foreach (Transform child in characterContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // Load scene top-down
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadScene(sceneName, spawnPointId);
        }
        else
        {
            Debug.LogError("[VNManager] GameManager.Instance is null! Cannot load scene.");
        }

        currentScene = null;
    }

    /// <summary>
    /// Handle choice: Quay lại hỏi thăm (Scene 7A - VN Confrontation)
    /// </summary>
    private void HandleTriggerScene7A()
    {
        Debug.Log("[VNManager] Triggering Scene 7A - Confrontation");

        if (day1Scene7A == null)
        {
            Debug.LogError("[VNManager] Scene 7A not assigned! Please assign Day1_Scene7A_Confrontation in VisualNovelManager Inspector.");
            EndVNMode();
            return;
        }

        // Chuyển sang Scene 7A (VN mode tiếp tục)
        currentScene = day1Scene7A;
        day1Scene7A.ApplyOnEnterEffects();
        StartCoroutine(TransitionBetweenScenes());
    }

    /// <summary>
    /// Handle choice: Chạy lẹ về nhà (Scene 7B - Top-down Escape)
    /// </summary>
    private void HandleTriggerScene7B()
    {
        Debug.Log("[VNManager] Triggering Scene 7B - Escape");

        // Dừng tất cả bullies đuổi theo
        StopAllBulliesFollowing();

        // Kết thúc VN mode và quay về top-down
        // Player sẽ điều khiển Đức chạy về nhà
        EndVNMode();
    }

    /// <summary>
    /// Dừng tất cả bullies đang follow player (gọi khi player escape)
    /// </summary>
    private void StopAllBulliesFollowing()
    {
        // Tìm BullyFollowTrigger trong scene
        BullyFollowTrigger bullyTrigger = FindFirstObjectByType<BullyFollowTrigger>();
        if (bullyTrigger != null)
        {
            bullyTrigger.StopAllBullies();
            Debug.Log("[VNManager] Đã dừng tất cả bullies qua BullyFollowTrigger");
        }

        // Tìm trực tiếp tất cả NPCFollowPlayer components (fallback)
        NPCFollowPlayer[] followers = FindObjectsByType<NPCFollowPlayer>(FindObjectsSortMode.None);
        if (followers != null && followers.Length > 0)
        {
            foreach (var follower in followers)
            {
                if (follower != null)
                {
                    follower.StopFollowing();
                }
            }
            Debug.Log($"[VNManager] Đã dừng {followers.Length} NPCFollowPlayer components");
        }
    }

    private IEnumerator TransitionBetweenScenes()
    {
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeOutCoroutine();
        }

        ClearCharacters();
        DisplayScene(currentScene.sceneData);

        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeInCoroutine();
        }
    }
    #endregion

    #region Player Control
    private GameObject cachedPlayer; // Cache player reference vì SetActive(false) sẽ khiến FindWithTag không tìm thấy
    private MonoBehaviour cachedCinemachineCamera; // Cache Cinemachine camera

    private void DisablePlayerControls()
    {
        // Tìm và cache player trước khi disable
        if (cachedPlayer == null)
        {
            cachedPlayer = GameObject.FindGameObjectWithTag("Player");
        }

        if (cachedPlayer != null)
        {
            var movement = cachedPlayer.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.enabled = false;
            }
            // Chỉ disable renderer và collider, KHÔNG SetActive(false) để còn tìm lại được
            var renderer = cachedPlayer.GetComponent<SpriteRenderer>();
            if (renderer != null) renderer.enabled = false;

            var collider = cachedPlayer.GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;

            Debug.Log("[VNManager] Player controls disabled");
        }

        // Disable Cinemachine camera để không follow player
        DisableCinemachineCamera();
    }

    private void DisableCinemachineCamera()
    {
        try
        {
            var allMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var mb in allMonoBehaviours)
            {
                if (mb == null) continue;
                var typeName = mb.GetType().Name;
                if (typeName == "CinemachineCamera" || typeName == "CinemachineVirtualCamera")
                {
                    cachedCinemachineCamera = mb;
                    mb.enabled = false;
                    Debug.Log($"[VNManager] Disabled Cinemachine camera: {mb.name}");
                    break;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[VNManager] Error disabling Cinemachine: {e.Message}");
        }
    }

    private void EnableCinemachineCamera()
    {
        if (cachedCinemachineCamera != null)
        {
            cachedCinemachineCamera.enabled = true;
            Debug.Log($"[VNManager] Enabled Cinemachine camera: {cachedCinemachineCamera.name}");
            cachedCinemachineCamera = null;
        }
    }

    private void EnablePlayerControls()
    {
        // Thử tìm lại nếu cache bị mất (sau scene load)
        if (cachedPlayer == null)
        {
            cachedPlayer = GameObject.FindGameObjectWithTag("Player");
        }

        if (cachedPlayer != null)
        {
            cachedPlayer.SetActive(true); // Đảm bảo active

            var renderer = cachedPlayer.GetComponent<SpriteRenderer>();
            if (renderer != null) renderer.enabled = true;

            var collider = cachedPlayer.GetComponent<Collider2D>();
            if (collider != null) collider.enabled = true;

            var movement = cachedPlayer.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.enabled = true;
                // QUAN TRỌNG: Reset movement lock
                movement.SetMovementEnabled(true);
            }

            Debug.Log("[VNManager] Player controls enabled");
        }
        else
        {
            Debug.LogWarning("[VNManager] Could not find Player to enable controls!");
        }

        // Enable Cinemachine camera
        EnableCinemachineCamera();

        // Clear cache sau khi enable (để tìm lại player mới nếu scene thay đổi)
        cachedPlayer = null;
    }
    #endregion
}

