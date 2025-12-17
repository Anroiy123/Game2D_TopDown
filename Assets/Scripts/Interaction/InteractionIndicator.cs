using UnityEngine;

/// <summary>
/// Interaction Indicator - Hiển thị icon animation phía trên NPC/objects
/// Sử dụng animated sprites từ UI assets (thinking emote, angry emote, etc.)
///
/// QUAN TRỌNG: Sử dụng PNG spritesheet đã slice (không dùng GIF vì Unity không extract animation từ GIF)
/// - Talk: UI_thinking_emotes_animation_16x16.png (32 frames)
/// - Warning: Cần tạo spritesheet tương tự
/// </summary>
public class InteractionIndicator : MonoBehaviour
{
    [Header("Indicator Settings")]
    [Tooltip("Loại indicator")]
    [SerializeField] private IndicatorType indicatorType = IndicatorType.Talk;

    [Header("Animation Sprites")]
    [Tooltip("Các frame animation cho indicator. Sử dụng PNG spritesheet đã slice (VD: UI_thinking_emotes_animation_16x16.png)")]
    [SerializeField] private Sprite[] animationFrames;

    [Header("Fallback Sprite")]
    [Tooltip("Sprite hiển thị khi không có animation frames")]
    [SerializeField] private Sprite fallbackSprite;

    [Header("Position")]
    [Tooltip("Offset từ vị trí object (Y+ = lên trên theo hệ tọa độ Unity chuẩn)")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, 0f);

    [Tooltip("Scale của indicator sprite (1.0 = kích thước gốc)")]
    [SerializeField] private float indicatorScale = 1.0f;

    [Header("Animation Settings")]
    [Tooltip("Tốc độ animation (frames per second)")]
    [SerializeField] private float animationSpeed = 10f;

    [Tooltip("Bounce animation")]
    [SerializeField] private bool enableBounce = true;

    [Tooltip("Bounce height")]
    [SerializeField] private float bounceHeight = 0.2f;

    [Tooltip("Bounce speed")]
    [SerializeField] private float bounceSpeed = 2f;

    [Header("Visibility")]
    [Tooltip("Chỉ hiện khi player ở gần")]
    [SerializeField] private bool showOnlyWhenNearPlayer = true;

    [Tooltip("Khoảng cách hiển thị")]
    [SerializeField] private float showDistance = 3f;

    [Tooltip("Tự động ẩn sau khi tương tác")]
    [SerializeField] private bool hideAfterInteraction = true;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = false;

    // Components
    private SpriteRenderer spriteRenderer;
    private GameObject indicatorObject;
    private Transform playerTransform;

    // Animation state
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private float bounceTimer = 0f;
    private Vector3 basePosition;
    private bool hasInteracted = false;
    private bool hasValidSprites = false;

    private void Start()
    {
        CreateIndicatorObject();
        FindPlayer();
        basePosition = offset;

        if (enableDebugLogs)
        {
            Debug.Log($"[InteractionIndicator] {gameObject.name} - Frames: {(animationFrames != null ? animationFrames.Length : 0)}, HasValidSprites: {hasValidSprites}");
        }
    }

    private void Update()
    {
        if (indicatorObject == null || !hasValidSprites)
            return;

        // Check visibility
        UpdateVisibility();

        if (!indicatorObject.activeSelf)
            return;

        // Update animation (only if multiple frames)
        if (animationFrames != null && animationFrames.Length > 1)
        {
            UpdateAnimation();
        }

        // Update bounce
        if (enableBounce)
        {
            UpdateBounce();
        }

        // Update position (ensure Z = 0 for 2D visibility)
        Vector3 newPos = transform.position + (enableBounce ? GetBounceOffset() : offset);
        newPos.z = 0f;
        indicatorObject.transform.position = newPos;
    }

    private void CreateIndicatorObject()
    {
        // Tạo GameObject con để hiển thị indicator
        indicatorObject = new GameObject("InteractionIndicator");
        indicatorObject.transform.SetParent(transform);

        // Set local position with Z = 0 to ensure it's visible
        Vector3 localPos = offset;
        localPos.z = 0f;
        indicatorObject.transform.localPosition = localPos;

        // Add SpriteRenderer
        spriteRenderer = indicatorObject.AddComponent<SpriteRenderer>();

        // Use highest sorting layer to ensure visibility above all game objects
        SetupSortingLayer();

        // Set sprite
        SetupSprite();

        // Set scale (configurable via Inspector)
        indicatorObject.transform.localScale = Vector3.one * indicatorScale;

        if (enableDebugLogs)
        {
            Debug.Log($"[InteractionIndicator] {gameObject.name} - Created at {indicatorObject.transform.position}, sorting: {spriteRenderer.sortingLayerName}/{spriteRenderer.sortingOrder}");
        }
    }

    private void SetupSortingLayer()
    {
        // Try "High / Overhead" first, then "UI", then fallback to "Default"
        if (SortingLayer.IsValid(SortingLayer.NameToID("High / Overhead")))
        {
            spriteRenderer.sortingLayerName = "High / Overhead";
            spriteRenderer.sortingOrder = 1000;
        }
        else if (SortingLayer.IsValid(SortingLayer.NameToID("UI")))
        {
            spriteRenderer.sortingLayerName = "UI";
            spriteRenderer.sortingOrder = 1000;
        }
        else
        {
            spriteRenderer.sortingLayerName = "Default";
            spriteRenderer.sortingOrder = 10000;
        }
    }

    private void SetupSprite()
    {
        // Ưu tiên animation frames
        if (animationFrames != null && animationFrames.Length > 0 && animationFrames[0] != null)
        {
            spriteRenderer.sprite = animationFrames[0];
            hasValidSprites = true;

            if (enableDebugLogs)
            {
                Debug.Log($"[InteractionIndicator] {gameObject.name} - Using {animationFrames.Length} animation frames");
            }
        }
        // Fallback to single sprite
        else if (fallbackSprite != null)
        {
            spriteRenderer.sprite = fallbackSprite;
            hasValidSprites = true;

            if (enableDebugLogs)
            {
                Debug.Log($"[InteractionIndicator] {gameObject.name} - Using fallback sprite: {fallbackSprite.name}");
            }
        }
        else
        {
            hasValidSprites = false;
            Debug.LogWarning($"[InteractionIndicator] {gameObject.name} - No sprites assigned! " +
                "Drag sprites from UI_thinking_emotes_animation_16x16.png (PNG spritesheet, not GIF)");
        }
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void UpdateVisibility()
    {
        if (!showOnlyWhenNearPlayer)
        {
            indicatorObject.SetActive(!hasInteracted || !hideAfterInteraction);
            return;
        }

        if (playerTransform == null)
        {
            // Try to find player again (might have spawned later)
            FindPlayer();
            if (playerTransform == null)
            {
                indicatorObject.SetActive(false);
                return;
            }
        }

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        bool shouldShow = distance <= showDistance && (!hasInteracted || !hideAfterInteraction);

        // Debug log (chỉ log khi state thay đổi)
        if (enableDebugLogs && indicatorObject.activeSelf != shouldShow)
        {
            Debug.Log($"[InteractionIndicator] {gameObject.name} - Visibility: {shouldShow}, Distance: {distance:F2}/{showDistance}");
        }

        indicatorObject.SetActive(shouldShow);
    }

    private void UpdateAnimation()
    {
        frameTimer += Time.deltaTime;

        if (frameTimer >= 1f / animationSpeed)
        {
            frameTimer = 0f;
            currentFrame = (currentFrame + 1) % animationFrames.Length;
            spriteRenderer.sprite = animationFrames[currentFrame];
        }
    }

    private void UpdateBounce()
    {
        bounceTimer += Time.deltaTime * bounceSpeed;
    }

    private Vector3 GetBounceOffset()
    {
        float bounce = Mathf.Sin(bounceTimer) * bounceHeight;
        return basePosition + new Vector3(0f, bounce, 0f);
    }

    /// <summary>
    /// Gọi khi player đã tương tác với object
    /// </summary>
    public void OnInteracted()
    {
        hasInteracted = true;
        if (hideAfterInteraction && indicatorObject != null)
        {
            indicatorObject.SetActive(false);
        }
    }

    /// <summary>
    /// Reset indicator (hiện lại)
    /// </summary>
    public void ResetIndicator()
    {
        hasInteracted = false;
    }

    /// <summary>
    /// Kiểm tra indicator có sprites hợp lệ không
    /// </summary>
    public bool HasValidSprites => hasValidSprites;

    /// <summary>
    /// Số lượng animation frames
    /// </summary>
    public int FrameCount => animationFrames != null ? animationFrames.Length : 0;

    public enum IndicatorType
    {
        Talk,       // Dấu hỏi (thinking emote) - Dùng UI_thinking_emotes_animation_16x16.png
        Warning,    // Dấu chấm than (angry emote)
        Quest,      // Mail icon
        Direction   // Arrow
    }
}