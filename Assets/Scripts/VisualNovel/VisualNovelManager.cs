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
    public static VisualNovelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<VisualNovelManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("VisualNovelManager");
                    _instance = go.AddComponent<VisualNovelManager>();
                }
            }
            return _instance;
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
    #endregion

    #region Settings
    [Header("Settings")]
    [SerializeField] private int canvasSortOrder = 300; // Trên DialogueSystem (200)
    #endregion

    #region State
    private bool isVNModeActive = false;
    private VNSceneData currentScene;
    private Action onVNComplete;
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
            return;
        }

        if (!sceneData.CanShow())
        {
            Debug.Log("[VNManager] Scene conditions not met, skipping.");
            onComplete?.Invoke();
            return;
        }

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
            backgroundImage.preserveAspect = true;

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

            dialogueSystem.StartDialogueWithChoices(scene.dialogue, OnDialogueComplete, OnDialogueAction);
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

        // Check if we need to load top-down scene
        if (currentScene != null && currentScene.sceneData.returnToTopDown)
        {
            string sceneName = currentScene.sceneData.topDownSceneName;
            string spawnId = currentScene.sceneData.spawnPointId;

            if (!string.IsNullOrEmpty(sceneName) && GameManager.Instance != null)
            {
                GameManager.Instance.LoadScene(sceneName, spawnId);
            }
            else
            {
                EnablePlayerControls();
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
    #endregion

    #region Dialogue Callbacks
    private void OnDialogueComplete()
    {
        // Unsubscribe from speaker change event
        if (dialogueSystem != null)
        {
            dialogueSystem.OnSpeakerChanged -= OnSpeakerChanged;
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
            }

            Debug.Log("[VNManager] Player controls enabled");
        }
        else
        {
            Debug.LogWarning("[VNManager] Could not find Player to enable controls!");
        }

        // Clear cache sau khi enable (để tìm lại player mới nếu scene thay đổi)
        cachedPlayer = null;
    }
    #endregion
}

