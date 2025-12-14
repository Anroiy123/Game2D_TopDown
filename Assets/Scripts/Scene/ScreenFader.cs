using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// ScreenFader - Quản lý fade in/out màn hình đen khi chuyển scene
/// Singleton, DontDestroyOnLoad để persist qua các scene
/// </summary>
public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Color fadeColor = Color.black;
    
    [Header("References")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private Canvas fadeCanvas;

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

        // Bắt đầu với màn hình trong suốt
        SetAlpha(0f);
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
}

