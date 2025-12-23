using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// StorytellingManager - Quản lý storytelling sequences (endings, cutscenes, intro)
/// Singleton pattern với auto-creation
/// </summary>
public class StorytellingManager : MonoBehaviour
{
    #region Singleton
    private static StorytellingManager _instance;
    public static StorytellingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<StorytellingManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("StorytellingManager");
                    _instance = go.AddComponent<StorytellingManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("[StorytellingManager] Initialized");
    }
    #endregion

    #region UI References
    [Header("Canvas & Panels")]
    [SerializeField] private Canvas storytellingCanvas;
    [SerializeField] private GameObject storytellingPanel;
    
    [Header("Visual Elements")]
    [SerializeField] private Image blackBaseImage;  // Luôn màu đen, không bao giờ fade - che cảnh top-down
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image illustrationImage;
    
    [Header("Text Elements")]
    [SerializeField] private GameObject textPanel;
    [SerializeField] private Text storyText;
    [SerializeField] private GameObject continueIcon;
    
    [Header("UI Elements")]
    [SerializeField] private Text skipHintText;
    
    [Header("Audio")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Settings")]
    [SerializeField] private int canvasSortingOrder = 500; // Above dialogue (400) and VN (300)
    [SerializeField] private float fadeDuration = 0.5f;
    #endregion

    #region State
    private StorytellingSequenceData currentSequence;
    private int currentSegmentIndex = 0;
    private bool isPlaying = false;
    private bool isTyping = false;
    private bool canContinue = false;
    private bool skipRequested = false;
    private Action onSequenceComplete;
    #endregion

    #region Initialization
    private void Start()
    {
        SetupUI();
    }

    private void SetupUI()
    {
        // Create Canvas if needed
        if (storytellingCanvas == null || !storytellingCanvas)
        {
            GameObject canvasObj = new GameObject("StorytellingCanvas");
            canvasObj.transform.SetParent(transform);
            storytellingCanvas = canvasObj.AddComponent<Canvas>();
            storytellingCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            storytellingCanvas.sortingOrder = canvasSortingOrder;

            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create Panel if needed
        if (storytellingPanel == null || !storytellingPanel)
        {
            storytellingPanel = new GameObject("StorytellingPanel");
            storytellingPanel.transform.SetParent(storytellingCanvas.transform, false);
            RectTransform panelRect = storytellingPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
        }

        // Create Black Base Image - ALWAYS solid black, never fades - covers top-down scene
        if (blackBaseImage == null || !blackBaseImage)
        {
            GameObject blackBaseObj = new GameObject("BlackBase");
            blackBaseObj.transform.SetParent(storytellingPanel.transform, false);
            blackBaseObj.transform.SetAsFirstSibling(); // Đặt ở dưới cùng
            blackBaseImage = blackBaseObj.AddComponent<Image>();
            RectTransform blackRect = blackBaseImage.rectTransform;
            blackRect.anchorMin = Vector2.zero;
            blackRect.anchorMax = Vector2.one;
            blackRect.sizeDelta = Vector2.zero;
            blackBaseImage.color = Color.black; // Luôn đen, không bao giờ thay đổi
            blackBaseImage.raycastTarget = false;
        }

        // Create Background Image if needed (on top of black base)
        if (backgroundImage == null || !backgroundImage)
        {
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(storytellingPanel.transform, false);
            bgObj.transform.SetSiblingIndex(1); // Ngay trên BlackBase
            backgroundImage = bgObj.AddComponent<Image>();
            RectTransform bgRect = backgroundImage.rectTransform;
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            backgroundImage.color = Color.black;
        }

        // Create Illustration Image if needed
        if (illustrationImage == null || !illustrationImage)
        {
            GameObject illusObj = new GameObject("Illustration");
            illusObj.transform.SetParent(storytellingPanel.transform, false);
            illusObj.transform.SetSiblingIndex(2); // Trên Background
            illustrationImage = illusObj.AddComponent<Image>();
            RectTransform illusRect = illustrationImage.rectTransform;
            illusRect.anchorMin = new Vector2(0.5f, 0.5f);
            illusRect.anchorMax = new Vector2(0.5f, 0.5f);
            illusRect.sizeDelta = new Vector2(800, 600); // Default size
            illusRect.anchoredPosition = Vector2.zero;
            illustrationImage.preserveAspect = true;
            illustrationImage.color = Color.white;
            illustrationImage.gameObject.SetActive(false);
        }

        // Create Text Panel if needed
        if (textPanel == null || !textPanel)
        {
            textPanel = new GameObject("TextPanel");
            textPanel.transform.SetParent(storytellingPanel.transform, false);
            RectTransform textPanelRect = textPanel.AddComponent<RectTransform>();
            textPanelRect.anchorMin = new Vector2(0f, 0f);
            textPanelRect.anchorMax = new Vector2(1f, 0.3f);
            textPanelRect.sizeDelta = Vector2.zero;

            // Add semi-transparent background
            Image panelBg = textPanel.AddComponent<Image>();
            panelBg.color = new Color(0, 0, 0, 0.7f);
        }

        // Create Story Text if needed
        if (storyText == null || !storyText)
        {
            GameObject textObj = new GameObject("StoryText");
            textObj.transform.SetParent(textPanel.transform, false);
            storyText = textObj.AddComponent<Text>();
            RectTransform textRect = storyText.rectTransform;
            textRect.anchorMin = new Vector2(0.05f, 0.1f);
            textRect.anchorMax = new Vector2(0.95f, 0.9f);
            textRect.sizeDelta = Vector2.zero;

            storyText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            storyText.fontSize = 32;
            storyText.color = Color.white;
            storyText.alignment = TextAnchor.MiddleCenter;
            storyText.horizontalOverflow = HorizontalWrapMode.Wrap;
            storyText.verticalOverflow = VerticalWrapMode.Overflow;
        }

        // Create Continue Icon if needed
        if (continueIcon == null || !continueIcon)
        {
            continueIcon = new GameObject("ContinueIcon");
            continueIcon.transform.SetParent(textPanel.transform, false);
            RectTransform iconRect = continueIcon.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.95f, 0.05f);
            iconRect.anchorMax = new Vector2(0.95f, 0.05f);
            iconRect.sizeDelta = new Vector2(50, 50);
            iconRect.anchoredPosition = new Vector2(-30, 30);

            Text iconText = continueIcon.AddComponent<Text>();
            iconText.text = "▼";
            iconText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            iconText.fontSize = 24;
            iconText.color = Color.white;
            iconText.alignment = TextAnchor.MiddleCenter;

            continueIcon.SetActive(false);
        }

        // Create Skip Hint Text if needed
        if (skipHintText == null || !skipHintText)
        {
            GameObject skipObj = new GameObject("SkipHint");
            skipObj.transform.SetParent(storytellingPanel.transform, false);
            skipHintText = skipObj.AddComponent<Text>();
            RectTransform skipRect = skipHintText.rectTransform;
            skipRect.anchorMin = new Vector2(1f, 1f);
            skipRect.anchorMax = new Vector2(1f, 1f);
            skipRect.sizeDelta = new Vector2(300, 50);
            skipRect.anchoredPosition = new Vector2(-160, -30);

            skipHintText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            skipHintText.fontSize = 20;
            skipHintText.color = new Color(1f, 1f, 1f, 0.7f);
            skipHintText.alignment = TextAnchor.MiddleRight;
            skipHintText.gameObject.SetActive(false);
        }

        // Create Audio Sources if needed
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        storytellingPanel.SetActive(false);
        Debug.Log("[StorytellingManager] UI Setup complete");
    }
    #endregion

    #region Public API
    /// <summary>
    /// Bắt đầu chạy một storytelling sequence
    /// </summary>
    public void PlaySequence(StorytellingSequenceData sequenceData, Action onComplete = null)
    {
        if (sequenceData == null)
        {
            Debug.LogError("[StorytellingManager] Sequence data is null!");
            onComplete?.Invoke();
            return;
        }

        Debug.Log($"[StorytellingManager] Starting sequence: {sequenceData.sequenceName}");
        currentSequence = sequenceData;
        currentSegmentIndex = 0;
        onSequenceComplete = onComplete;
        skipRequested = false;

        currentSequence.ApplyOnStartEffects();
        StartCoroutine(PlaySequenceCoroutine());
    }

    /// <summary>
    /// Dừng sequence hiện tại
    /// </summary>
    public void StopSequence()
    {
        if (isPlaying)
        {
            StopAllCoroutines();
            EndSequence();
        }
    }

    /// <summary>
    /// Check xem có đang chạy sequence không
    /// </summary>
    public bool IsPlaying => isPlaying;
    #endregion

    #region Sequence Playback
    private IEnumerator PlaySequenceCoroutine()
    {
        isPlaying = true;

        // Disable player controls
        DisablePlayerControls();

        // Fade out current scene using ScreenFader (only once at the start)
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeOutCoroutine();
        }

        // Show storytelling panel with black background initially
        storytellingPanel.SetActive(true);
        
        // Ensure black base is always solid black (never changes)
        if (blackBaseImage != null)
        {
            blackBaseImage.color = Color.black;
        }
        
        // Ensure background starts as solid black to cover the top-down scene
        if (backgroundImage != null)
        {
            backgroundImage.sprite = null;
            backgroundImage.color = Color.black;
        }
        
        // Hide illustration initially
        if (illustrationImage != null)
        {
            illustrationImage.gameObject.SetActive(false);
        }

        // Show skip hint if allowed
        if (currentSequence.allowSkip && skipHintText != null)
        {
            skipHintText.text = currentSequence.skipHintText;
            skipHintText.gameObject.SetActive(true);
        }
        
        // Fade in the storytelling panel (from ScreenFader's black)
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeInCoroutine();
        }

        // Play all segments
        for (currentSegmentIndex = 0; currentSegmentIndex < currentSequence.segments.Length; currentSegmentIndex++)
        {
            if (skipRequested) break;

            StorySegment segment = currentSequence.segments[currentSegmentIndex];
            yield return DisplaySegmentCoroutine(segment);
        }

        // End sequence
        EndSequence();
    }

    private IEnumerator DisplaySegmentCoroutine(StorySegment segment)
    {
        Debug.Log($"[StorytellingManager] Displaying segment {currentSegmentIndex + 1}/{currentSequence.segments.Length}");

        // Delay before
        if (segment.delayBefore > 0)
        {
            yield return new WaitForSecondsRealtime(segment.delayBefore);
        }

        // Fade to black if needed - use internal fade, NOT ScreenFader (which would show top-down scene)
        if (segment.fadeToBlackBefore)
        {
            yield return FadeStorytellingPanelCoroutine(0f); // Fade panel to transparent (shows black bg)
        }

        // Update background
        if (segment.backgroundImage != null)
        {
            yield return UpdateBackgroundCoroutine(segment);
        }
        else
        {
            // No background image - set to black
            backgroundImage.sprite = null;
            backgroundImage.color = segment.backgroundTint;
        }

        // Update illustration
        UpdateIllustration(segment);

        // Play audio
        PlayAudio(segment);

        // Fade in the storytelling panel content
        if (segment.fadeToBlackBefore)
        {
            yield return FadeStorytellingPanelCoroutine(1f); // Fade panel back to visible
        }

        // Display text lines one by one
        if (segment.textLines != null && segment.textLines.Length > 0)
        {
            for (int i = 0; i < segment.textLines.Length; i++)
            {
                if (skipRequested) break;
                
                string line = segment.textLines[i];
                if (string.IsNullOrEmpty(line)) continue;
                
                // Type this line
                yield return TypeTextCoroutine(line, segment.typewriterSpeed);
                
                // Wait for input or auto-advance
                canContinue = false;
                continueIcon.SetActive(true);
                
                if (segment.autoAdvanceDelay > 0)
                {
                    // Auto advance mode: wait for delay OR user input
                    float elapsed = 0f;
                    while (elapsed < segment.autoAdvanceDelay && !skipRequested)
                    {
                        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                        {
                            break;
                        }
                        elapsed += Time.unscaledDeltaTime;
                        yield return null;
                    }
                }
                else
                {
                    // Manual mode: wait for user input
                    while (!skipRequested)
                    {
                        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                        {
                            break;
                        }
                        yield return null;
                    }
                }
                
                continueIcon.SetActive(false);
            }
        }
        else
        {
            // No text lines - just wait briefly for visual-only segments
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    private IEnumerator UpdateBackgroundCoroutine(StorySegment segment)
    {
        if (backgroundImage == null) yield break;

        switch (segment.backgroundTransition)
        {
            case StorySegment.TransitionEffect.None:
                backgroundImage.sprite = segment.backgroundImage;
                backgroundImage.color = segment.backgroundTint;
                break;

            case StorySegment.TransitionEffect.Fade:
                // Fade out
                yield return FadeImageCoroutine(backgroundImage, backgroundImage.color, new Color(segment.backgroundTint.r, segment.backgroundTint.g, segment.backgroundTint.b, 0f), fadeDuration);
                // Change sprite
                backgroundImage.sprite = segment.backgroundImage;
                // Fade in
                yield return FadeImageCoroutine(backgroundImage, new Color(segment.backgroundTint.r, segment.backgroundTint.g, segment.backgroundTint.b, 0f), segment.backgroundTint, fadeDuration);
                break;

            case StorySegment.TransitionEffect.CrossFade:
                // TODO: Implement cross-fade (requires 2 images)
                backgroundImage.sprite = segment.backgroundImage;
                backgroundImage.color = segment.backgroundTint;
                break;
        }
    }

    private void UpdateIllustration(StorySegment segment)
    {
        if (illustrationImage == null) return;

        if (segment.illustrationImage != null)
        {
            illustrationImage.sprite = segment.illustrationImage;
            illustrationImage.gameObject.SetActive(true);

            // Set position
            RectTransform rect = illustrationImage.rectTransform;
            switch (segment.illustrationPosition)
            {
                case StorySegment.IllustrationPosition.Center:
                    rect.anchoredPosition = Vector2.zero;
                    break;
                case StorySegment.IllustrationPosition.Top:
                    rect.anchoredPosition = new Vector2(0, 200);
                    break;
                case StorySegment.IllustrationPosition.Bottom:
                    rect.anchoredPosition = new Vector2(0, -200);
                    break;
                case StorySegment.IllustrationPosition.Left:
                    rect.anchoredPosition = new Vector2(-300, 0);
                    break;
                case StorySegment.IllustrationPosition.Right:
                    rect.anchoredPosition = new Vector2(300, 0);
                    break;
                case StorySegment.IllustrationPosition.Custom:
                    // Convert normalized position to anchored position
                    float x = (segment.customPosition.x - 0.5f) * 1920;
                    float y = (segment.customPosition.y - 0.5f) * 1080;
                    rect.anchoredPosition = new Vector2(x, y);
                    break;
            }

            // Set scale
            rect.localScale = Vector3.one * segment.illustrationScale;
        }
        else
        {
            illustrationImage.gameObject.SetActive(false);
        }
    }

    private void PlayAudio(StorySegment segment)
    {
        // Play BGM
        if (segment.bgm != null && bgmSource != null)
        {
            if (bgmSource.clip != segment.bgm)
            {
                bgmSource.clip = segment.bgm;
                bgmSource.volume = segment.bgmVolume;
                bgmSource.Play();
            }
        }

        // Play SFX
        if (segment.sfx != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(segment.sfx, segment.sfxVolume);
        }
    }

    private IEnumerator TypeTextCoroutine(string text, float speed)
    {
        if (storyText == null) yield break;

        isTyping = true;
        storyText.text = "";

        if (speed <= 0f)
        {
            // No typewriter effect, show all at once
            storyText.text = text;
        }
        else
        {
            // Typewriter effect
            foreach (char c in text)
            {
                if (skipRequested) break;

                storyText.text += c;
                yield return new WaitForSecondsRealtime(speed);
            }
        }

        isTyping = false;
    }

    private IEnumerator FadeImageCoroutine(Image image, Color from, Color to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            image.color = Color.Lerp(from, to, t);
            yield return null;
        }
        image.color = to;
    }

    /// <summary>
    /// Fade storytelling panel content (background, illustration) without using ScreenFader
    /// This prevents showing the top-down scene during segment transitions
    /// </summary>
    private IEnumerator FadeStorytellingPanelCoroutine(float targetAlpha)
    {
        float startAlpha = targetAlpha == 0f ? 1f : 0f;
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            
            // Fade background
            if (backgroundImage != null)
            {
                Color bgColor = backgroundImage.color;
                bgColor.a = alpha;
                backgroundImage.color = bgColor;
            }
            
            // Fade illustration
            if (illustrationImage != null && illustrationImage.gameObject.activeSelf)
            {
                Color illusColor = illustrationImage.color;
                illusColor.a = alpha;
                illustrationImage.color = illusColor;
            }
            
            yield return null;
        }
        
        // Ensure final alpha
        if (backgroundImage != null)
        {
            Color bgColor = backgroundImage.color;
            bgColor.a = targetAlpha;
            backgroundImage.color = bgColor;
        }
        if (illustrationImage != null && illustrationImage.gameObject.activeSelf)
        {
            Color illusColor = illustrationImage.color;
            illusColor.a = targetAlpha;
            illustrationImage.color = illusColor;
        }
    }
    #endregion

    #region Sequence End
    private void EndSequence()
    {
        Debug.Log("[StorytellingManager] Ending sequence");

        // Apply on complete effects
        if (currentSequence != null)
        {
            currentSequence.ApplyOnCompleteEffects();
        }

        // Stop audio
        if (bgmSource != null) bgmSource.Stop();

        // Hide UI
        storytellingPanel.SetActive(false);
        if (skipHintText != null) skipHintText.gameObject.SetActive(false);

        isPlaying = false;

        // Call completion callback
        onSequenceComplete?.Invoke();

        // Load next scene if specified
        if (currentSequence != null && !string.IsNullOrEmpty(currentSequence.nextSceneName))
        {
            StartCoroutine(LoadNextSceneCoroutine());
        }
        else
        {
            // Re-enable player controls if staying in same scene
            EnablePlayerControls();
        }
    }

    private IEnumerator LoadNextSceneCoroutine()
    {
        // Wait for delay
        if (currentSequence.delayBeforeNextScene > 0)
        {
            yield return new WaitForSecondsRealtime(currentSequence.delayBeforeNextScene);
        }

        // Fade out
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeOutCoroutine();
        }

        // Load scene
        Debug.Log($"[StorytellingManager] Loading next scene: {currentSequence.nextSceneName}");
        SceneManager.LoadScene(currentSequence.nextSceneName);
    }
    #endregion

    #region Input Handling
    private void Update()
    {
        if (!isPlaying) return;

        // Check for skip input
        if (currentSequence != null && currentSequence.allowSkip)
        {
            if (Input.GetKeyDown(currentSequence.skipKey))
            {
                skipRequested = true;
                Debug.Log("[StorytellingManager] Skip requested");
            }
        }
    }
    #endregion

    #region Player Control
    private void DisablePlayerControls()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.SetMovementEnabled(false);
            }
        }
    }

    private void EnablePlayerControls()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.SetMovementEnabled(true);
            }
        }
    }
    #endregion
}

