using UnityEngine;
using System.Collections;

/// <summary>
/// VNTrigger - Component để trigger cảnh Visual Novel
/// Có thể đặt trong scene để kích hoạt VN mode
/// Note: Chỉ cần Collider2D nếu dùng OnTriggerEnter hoặc OnInteract mode
/// </summary>
public class VNTrigger : MonoBehaviour
{
    [Header("VN Scene")]
    [Tooltip("Cảnh VN sẽ chơi khi trigger")]
    [SerializeField] private VNSceneData vnScene;

    [Header("Trigger Settings")]
    [Tooltip("Mode kích hoạt")]
    [SerializeField] private TriggerMode mode = TriggerMode.OnTriggerEnter;
    
    [Tooltip("Phím tương tác (chỉ dùng cho OnInteract mode)")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Tooltip("Chỉ trigger một lần")]
    [SerializeField] private bool triggerOnce = true;

    [Header("Conditions")]
    [Tooltip("Flags cần có để trigger")]
    [SerializeField] private string[] requiredFlags;

    [Tooltip("Flags không được có")]
    [SerializeField] private string[] forbiddenFlags;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject interactionPrompt;

    [Header("Debug")]
    [Tooltip("Hiển thị debug logs")]
    [SerializeField] private bool showDebugLogs = true;

    public enum TriggerMode
    {
        OnTriggerEnter,
        OnInteract,
        OnSceneStart
    }

    private bool playerInRange = false;
    private bool hasTriggered = false;
    private bool isPendingTrigger = false; // Đang chờ retry trigger
    private const int MAX_RETRY_ATTEMPTS = 5;
    private const float RETRY_DELAY = 0.1f;

    private void Start()
    {
        // Validate collider requirement
        if (mode == TriggerMode.OnTriggerEnter || mode == TriggerMode.OnInteract)
        {
            var collider = GetComponent<Collider2D>();
            if (collider == null)
            {
                Debug.LogError($"[VNTrigger] {gameObject.name}: Mode '{mode}' requires a Collider2D component! Please add BoxCollider2D.", this);
                return;
            }
            collider.isTrigger = true;
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // Auto trigger on scene start
        if (mode == TriggerMode.OnSceneStart)
        {
            TryTriggerVN();
        }
    }

    private void Update()
    {
        // QUAN TRỌNG: Không trigger VN mới khi VN mode đang active
        // (Tránh nested VN scenes hoặc conflict với dialogue trong VN)
        if (VisualNovelManager.Instance != null && VisualNovelManager.Instance.IsVNModeActive)
        {
            return;
        }

        if (mode == TriggerMode.OnInteract && playerInRange && !hasTriggered)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                TryTriggerVN();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log($"[VNTrigger] {gameObject.name}: Player entered trigger zone. Mode={mode}");
        playerInRange = true;

        if (mode == TriggerMode.OnTriggerEnter && !hasTriggered)
        {
            TryTriggerVN();
        }
        else if (mode == TriggerMode.OnInteract && interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void TryTriggerVN()
    {
        if (showDebugLogs)
            Debug.Log($"[VNTrigger] {gameObject.name}: TryTriggerVN() called. hasTriggered={hasTriggered}, triggerOnce={triggerOnce}");

        if (hasTriggered && triggerOnce)
        {
            if (showDebugLogs)
                Debug.Log($"[VNTrigger] {gameObject.name}: Already triggered, skipping.");
            return;
        }
        if (!CheckConditions()) return;

        if (vnScene == null)
        {
            Debug.LogWarning($"[VNTrigger] {gameObject.name}: VNScene is null!");
            return;
        }

        // QUAN TRỌNG: Check null cho VisualNovelManager.Instance
        // Có thể null trong quá trình scene transition
        if (VisualNovelManager.Instance == null)
        {
            Debug.LogWarning($"[VNTrigger] {gameObject.name}: VisualNovelManager.Instance is null! Starting retry...");
            
            // Chỉ retry nếu chưa đang pending
            if (!isPendingTrigger)
            {
                isPendingTrigger = true;
                StartCoroutine(RetryTriggerVN());
            }
            return;
        }

        // Kiểm tra VN mode đang active không
        if (VisualNovelManager.Instance.IsVNModeActive)
        {
            if (showDebugLogs)
                Debug.Log($"[VNTrigger] {gameObject.name}: VN mode is active, skipping trigger.");
            return;
        }

        if (showDebugLogs)
            Debug.Log($"[VNTrigger] {gameObject.name}: ✓ Triggering VN scene '{vnScene.name}'");
        
        hasTriggered = true;
        isPendingTrigger = false;

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        VisualNovelManager.Instance.StartVNScene(vnScene, OnVNComplete);
    }

    /// <summary>
    /// Retry trigger VN scene khi VisualNovelManager chưa sẵn sàng
    /// </summary>
    private IEnumerator RetryTriggerVN()
    {
        int attempts = 0;
        
        while (attempts < MAX_RETRY_ATTEMPTS)
        {
            yield return new WaitForSeconds(RETRY_DELAY);
            attempts++;
            
            if (showDebugLogs)
                Debug.Log($"[VNTrigger] {gameObject.name}: Retry attempt {attempts}/{MAX_RETRY_ATTEMPTS}");

            // Check nếu đã triggered bởi cách khác
            if (hasTriggered && triggerOnce)
            {
                if (showDebugLogs)
                    Debug.Log($"[VNTrigger] {gameObject.name}: Already triggered during retry, stopping.");
                isPendingTrigger = false;
                yield break;
            }

            // Check VisualNovelManager đã sẵn sàng chưa
            if (VisualNovelManager.Instance != null)
            {
                if (showDebugLogs)
                    Debug.Log($"[VNTrigger] {gameObject.name}: VisualNovelManager ready! Triggering now.");
                
                isPendingTrigger = false;
                TryTriggerVN();
                yield break;
            }
        }

        // Hết retry attempts
        Debug.LogError($"[VNTrigger] {gameObject.name}: Failed to trigger VN scene after {MAX_RETRY_ATTEMPTS} attempts! VisualNovelManager not available.");
        isPendingTrigger = false;
        hasTriggered = false; // Reset để có thể thử lại sau
    }

    private bool CheckConditions()
    {
        if (StoryManager.Instance == null) return true;

        bool requiredMet = StoryManager.Instance.CheckRequiredFlags(requiredFlags);
        bool forbiddenMet = StoryManager.Instance.CheckForbiddenFlags(forbiddenFlags);

        if (!requiredMet && showDebugLogs)
        {
            Debug.Log($"[VNTrigger] {gameObject.name}: BLOCKED - Required flags not met. Need: [{string.Join(", ", requiredFlags ?? new string[0])}]");
        }
        if (!forbiddenMet && showDebugLogs)
        {
            Debug.Log($"[VNTrigger] {gameObject.name}: BLOCKED - Has forbidden flags. Forbidden: [{string.Join(", ", forbiddenFlags ?? new string[0])}]");
        }

        return requiredMet && forbiddenMet;
    }

    private void OnVNComplete()
    {
        if (showDebugLogs)
            Debug.Log($"[VNTrigger] {gameObject.name}: VN scene completed");

        // Reset trigger if needed
        if (!triggerOnce)
        {
            hasTriggered = false;
        }
    }

    /// <summary>
    /// Manual trigger (gọi từ script khác)
    /// </summary>
    public void TriggerManually()
    {
        TryTriggerVN();
    }

    /// <summary>
    /// Reset trigger state
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        isPendingTrigger = false;
    }

    /// <summary>
    /// Kiểm tra trigger đang pending không
    /// </summary>
    public bool IsPendingTrigger => isPendingTrigger;
}

