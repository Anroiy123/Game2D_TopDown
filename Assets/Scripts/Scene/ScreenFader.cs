using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// ScreenFader - Quản lý fade in/out màn hình đen khi chuyển scene
/// Singleton, DontDestroyOnLoad để persist qua các scene
/// Hỗ trợ hiển thị text trên màn hình đen (transition text)
/// </summary>
public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }
    private static bool _applicationIsQuitting = false;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Color fadeColor = Color.black;

    [Header("Text Settings")]
    [SerializeField] private float textFadeDuration = 0.5f;

    [Header("References")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private Canvas fadeCanvas;
    private Text transitionText;

    private bool isFading = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Tự động tạo Canvas và Image nếu chưa có
        if (fadeCanvas == null || fadeImage == null)
        {
            SetupFadeUI();
        }

        // Luôn tạo transition text nếu chưa có
        if (transitionText == null)
        {
            SetupTransitionText();
        }

        // Bắt đầu với màn hình trong suốt
        SetAlpha(0f);
    }

    private void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            _applicationIsQuitting = true;
        }
    }

    private void SetupFadeUI()
    {
        // Tạo Canvas
        fadeCanvas = GetComponent<Canvas>();
        if (fadeCanvas == null)
        {
            fadeCanvas = gameObject.AddComponent<Canvas>();
        }
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 9999; // Luôn ở trên cùng

        // Thêm CanvasScaler
        var scaler = GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = gameObject.AddComponent<CanvasScaler>();
        }
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Thêm GraphicRaycaster
        if (GetComponent<GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }

        // Tạo Image con
        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(transform, false);
        
        fadeImage = imageObj.AddComponent<Image>();
        fadeImage.color = fadeColor;
        fadeImage.raycastTarget = false; // Không chặn input

        // Stretch full screen
        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        // Tạo Text cho transition
        SetupTransitionText();
    }

    private void SetupTransitionText()
    {
        GameObject textObj = new GameObject("TransitionText");
        textObj.transform.SetParent(transform, false);

        transitionText = textObj.AddComponent<Text>();
        transitionText.text = "";
        transitionText.fontSize = 48;
        transitionText.alignment = TextAnchor.MiddleCenter;
        transitionText.color = new Color(1f, 1f, 1f, 0f);
        transitionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        RectTransform textRect = transitionText.rectTransform;
        textRect.anchorMin = new Vector2(0.1f, 0.4f);
        textRect.anchorMax = new Vector2(0.9f, 0.6f);
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// Fade ra màn hình đen
    /// </summary>
    public void FadeOut(System.Action onComplete = null)
    {
        // Dừng tất cả fade đang chạy
        StopAllCoroutines();
        isFading = false;
        StartCoroutine(FadeRoutine(0f, 1f, onComplete));
    }

    /// <summary>
    /// Fade từ màn hình đen về trong suốt
    /// </summary>
    public void FadeIn(System.Action onComplete = null)
    {
        // Dừng tất cả fade đang chạy
        StopAllCoroutines();
        isFading = false;
        StartCoroutine(FadeRoutine(1f, 0f, onComplete));
    }

    /// <summary>
    /// Fade với callback - Coroutine version
    /// </summary>
    public IEnumerator FadeOutCoroutine()
    {
        // Đợi nếu đang fade
        while (isFading)
        {
            yield return null;
        }
        yield return StartCoroutine(FadeRoutine(0f, 1f, null));
    }

    public IEnumerator FadeInCoroutine()
    {
        // Không cần đợi - force fade in ngay lập tức
        // Điều này đảm bảo màn hình luôn sáng lên sau khi teleport
        isFading = false;
        yield return StartCoroutine(FadeRoutine(1f, 0f, null));
    }

    /// <summary>
    /// Force reset fade state - dùng khi bị kẹt
    /// </summary>
    public void ForceReset()
    {
        StopAllCoroutines();
        isFading = false;
        SetAlpha(0f);
        Debug.Log("[ScreenFader] Force reset to transparent");
    }

    private IEnumerator FadeRoutine(float startAlpha, float endAlpha, System.Action onComplete)
    {
        isFading = true;
        float elapsed = 0f;

        SetAlpha(startAlpha);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Unscaled để fade khi game pause
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(endAlpha);
        isFading = false;
        onComplete?.Invoke();
    }

    private void SetAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color c = fadeColor;
            c.a = alpha;
            fadeImage.color = c;
        }
    }

    public bool IsFading => isFading;
    public float FadeDuration => fadeDuration;

    #region Transition with Text
    /// <summary>
    /// Fade ra màn đen, hiển thị text, chờ, rồi fade in
    /// Dùng cho chuyển cảnh với text như "Cuối giờ học..."
    /// </summary>
    public IEnumerator FadeWithTextCoroutine(string text, float displayDuration = 2f)
    {
        Debug.Log($"[ScreenFader] FadeWithText: '{text}' for {displayDuration}s");

        // Fade out to black
        yield return FadeOutCoroutine();

        // Show text với fade in
        if (transitionText != null)
        {
            Debug.Log($"[ScreenFader] Showing text: {text}");
            transitionText.text = text;
            yield return FadeTextRoutine(0f, 1f);
        }
        else
        {
            Debug.LogWarning("[ScreenFader] transitionText is NULL!");
        }

        // Giữ text trên màn hình
        yield return new WaitForSecondsRealtime(displayDuration);

        // Fade out text
        if (transitionText != null)
        {
            yield return FadeTextRoutine(1f, 0f);
            transitionText.text = "";
        }
    }

    /// <summary>
    /// Chỉ hiển thị text (khi đã ở màn đen)
    /// </summary>
    public IEnumerator ShowTextCoroutine(string text, float displayDuration = 2f)
    {
        if (transitionText != null)
        {
            transitionText.text = text;
            yield return FadeTextRoutine(0f, 1f);
        }

        yield return new WaitForSecondsRealtime(displayDuration);

        if (transitionText != null)
        {
            yield return FadeTextRoutine(1f, 0f);
            transitionText.text = "";
        }
    }

    private IEnumerator FadeTextRoutine(float startAlpha, float endAlpha)
    {
        if (transitionText == null) yield break;

        float elapsed = 0f;
        Color c = transitionText.color;

        while (elapsed < textFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / textFadeDuration);
            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            transitionText.color = c;
            yield return null;
        }

        c.a = endAlpha;
        transitionText.color = c;
    }

    /// <summary>
    /// Ẩn text ngay lập tức
    /// </summary>
    public void HideText()
    {
        if (transitionText != null)
        {
            transitionText.text = "";
            Color c = transitionText.color;
            c.a = 0f;
            transitionText.color = c;
        }
    }
    #endregion
}

