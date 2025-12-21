using UnityEngine;
using System.Collections;

/// <summary>
/// TimeSkipTrigger - Component để trigger chuyển cảnh thời gian với text overlay
/// Dùng cho: "Trôi qua 1 tuần...", "Sau 1-2 tuần...", "Sáng hôm sau..."
/// Hỗ trợ nhiều đoạn text hiển thị tuần tự
/// </summary>
public class TimeSkipTrigger : MonoBehaviour
{
    [Header("Time Skip Texts")]
    [Tooltip("Danh sách các đoạn text hiển thị tuần tự trên màn đen")]
    [SerializeField] private TimeSkipTextEntry[] textEntries;

    [System.Serializable]
    public class TimeSkipTextEntry
    {
        [Tooltip("Text hiển thị (VD: 'Trôi qua 1 tuần...')")]
        [TextArea(2, 4)]
        public string text = "Trôi qua 1 tuần...";
        
        [Tooltip("Thời gian hiển thị text này (giây)")]
        public float displayDuration = 2.5f;
        
        [Tooltip("Delay trước khi hiển thị text này (giây)")]
        public float delayBefore = 0f;
    }

    [Header("Legacy (Single Text)")]
    [Tooltip("Text đơn - dùng nếu textEntries trống")]
    [TextArea(2, 4)]
    [SerializeField] private string timeSkipText = "Trôi qua 1 tuần...";
    
    [Tooltip("Thời gian hiển thị text đơn (giây)")]
    [SerializeField] private float textDisplayDuration = 2.5f;

    [Header("Trigger Settings")]
    [Tooltip("Mode kích hoạt")]
    [SerializeField] private TriggerMode mode = TriggerMode.OnTriggerEnter;
    
    [Tooltip("Phím tương tác (chỉ dùng cho OnInteract mode)")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Tooltip("Chỉ trigger một lần")]
    [SerializeField] private bool triggerOnce = true;
    
    [Tooltip("Delay trước khi bắt đầu (giây)")]
    [SerializeField] private float triggerDelay = 0f;

    [Header("Conditions")]
    [Tooltip("Flags cần có để trigger")]
    [SerializeField] private string[] requiredFlags;

    [Tooltip("Flags không được có")]
    [SerializeField] private string[] forbiddenFlags;

    [Header("Story Effects")]
    [Tooltip("Số ngày tăng thêm (0 = không tăng)")]
    [SerializeField] private int daysToAdvance = 7;
    
    [Tooltip("Flags sẽ được set TRUE")]
    [SerializeField] private string[] setFlagsTrue;
    
    [Tooltip("Flags sẽ được set FALSE")]
    [SerializeField] private string[] setFlagsFalse;
    
    [Tooltip("Variable changes")]
    [SerializeField] private VariableChange[] variableChanges;

    [Header("After Time Skip")]
    [Tooltip("VN Scene chạy sau time skip (optional)")]
    [SerializeField] private VNSceneData afterTimeSkipVNScene;
    
    [Tooltip("Chuyển sang Unity scene khác (optional, để trống nếu ở lại scene hiện tại)")]
    [SerializeField] private string targetSceneName;
    
    [Tooltip("Spawn point ID trong scene đích")]
    [SerializeField] private string targetSpawnPointId;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject interactionPrompt;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    public enum TriggerMode
    {
        OnTriggerEnter,
        OnInteract,
        OnSceneStart,
        Manual // Gọi từ code bên ngoài
    }

    private bool playerInRange = false;
    private bool hasTriggered = false;
    private bool isPlaying = false;

    private void Start()
    {
        // Validate collider requirement
        if (mode == TriggerMode.OnTriggerEnter || mode == TriggerMode.OnInteract)
        {
            var collider = GetComponent<Collider2D>();
            if (collider == null)
            {
                Debug.LogWarning($"[TimeSkipTrigger] {gameObject.name}: Mode '{mode}' works better with a Collider2D component.", this);
            }
            else
            {
                collider.isTrigger = true;
            }
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // Auto trigger on scene start
        if (mode == TriggerMode.OnSceneStart)
        {
            if (triggerDelay > 0)
            {
                Invoke(nameof(TryTriggerTimeSkip), triggerDelay);
            }
            else
            {
                TryTriggerTimeSkip();
            }
        }
    }

    private void Update()
    {
        // Không trigger khi VN mode đang active
        if (VisualNovelManager.Instance != null && VisualNovelManager.Instance.IsVNModeActive)
        {
            return;
        }

        if (mode == TriggerMode.OnInteract && playerInRange && !hasTriggered && !isPlaying)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                TryTriggerTimeSkip();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (showDebugLogs) Debug.Log($"[TimeSkipTrigger] {gameObject.name}: Player entered trigger zone");
        
        playerInRange = true;

        if (mode == TriggerMode.OnTriggerEnter && !hasTriggered && !isPlaying)
        {
            if (triggerDelay > 0)
            {
                Invoke(nameof(TryTriggerTimeSkip), triggerDelay);
            }
            else
            {
                TryTriggerTimeSkip();
            }
        }
        else if (mode == TriggerMode.OnInteract && interactionPrompt != null && CheckConditions())
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

    /// <summary>
    /// Trigger time skip từ code bên ngoài
    /// </summary>
    public void TriggerFromExternal()
    {
        TryTriggerTimeSkip();
    }

    /// <summary>
    /// Trigger time skip với callback
    /// </summary>
    public void TriggerFromExternal(System.Action onComplete)
    {
        StartCoroutine(TimeSkipCoroutine(onComplete));
    }

    private void TryTriggerTimeSkip()
    {
        if (showDebugLogs) Debug.Log($"[TimeSkipTrigger] {gameObject.name}: TryTriggerTimeSkip() called");

        if (hasTriggered && triggerOnce)
        {
            if (showDebugLogs) Debug.Log($"[TimeSkipTrigger] {gameObject.name}: Already triggered, skipping.");
            return;
        }
        
        if (isPlaying)
        {
            if (showDebugLogs) Debug.Log($"[TimeSkipTrigger] {gameObject.name}: Already playing, skipping.");
            return;
        }

        if (!CheckConditions()) return;

        if (showDebugLogs) Debug.Log($"[TimeSkipTrigger] {gameObject.name}: ✓ Starting time skip: '{timeSkipText}'");
        
        hasTriggered = true;

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        StartCoroutine(TimeSkipCoroutine(null));
    }

    private IEnumerator TimeSkipCoroutine(System.Action onComplete)
    {
        isPlaying = true;

        // 1. Khóa player movement
        PlayerMovement playerMovement = FindPlayerMovement();
        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(false);
        }

        // 2. Fade ra màn đen và hiển thị text(s)
        if (ScreenFader.Instance != null)
        {
            // Kiểm tra có dùng multi-text hay single text
            if (textEntries != null && textEntries.Length > 0)
            {
                // Multi-text mode: hiển thị nhiều đoạn text tuần tự
                yield return MultiTextFadeCoroutine();
            }
            else
            {
                // Legacy single text mode
                yield return ScreenFader.Instance.FadeWithTextCoroutine(timeSkipText, textDisplayDuration);
            }
        }
        else
        {
            Debug.LogWarning("[TimeSkipTrigger] ScreenFader not found! Skipping fade effect.");
            yield return new WaitForSeconds(textDisplayDuration);
        }

        // 3. Apply story effects
        ApplyStoryEffects();

        // 4. Xử lý sau time skip
        if (afterTimeSkipVNScene != null)
        {
            // Chuyển sang VN scene
            if (showDebugLogs) Debug.Log($"[TimeSkipTrigger] Starting VN scene: {afterTimeSkipVNScene.name}");
            
            // Fade in sẽ được VNManager xử lý
            VisualNovelManager.Instance.StartVNScene(afterTimeSkipVNScene, () => {
                OnTimeSkipComplete(playerMovement);
                onComplete?.Invoke();
            });
        }
        else if (!string.IsNullOrEmpty(targetSceneName))
        {
            // Chuyển sang Unity scene khác
            if (showDebugLogs) Debug.Log($"[TimeSkipTrigger] Loading scene: {targetSceneName}");
            
            if (GameManager.Instance != null)
            {
                // GameManager sẽ xử lý fade in
                GameManager.Instance.LoadScene(targetSceneName, targetSpawnPointId);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
            }
            
            onComplete?.Invoke();
        }
        else
        {
            // Ở lại scene hiện tại, fade in
            if (ScreenFader.Instance != null)
            {
                yield return ScreenFader.Instance.FadeInCoroutine();
            }
            
            OnTimeSkipComplete(playerMovement);
            onComplete?.Invoke();
        }
    }

    private void ApplyStoryEffects()
    {
        if (StoryManager.Instance == null)
        {
            Debug.LogWarning("[TimeSkipTrigger] StoryManager not found!");
            return;
        }

        // Advance days
        if (daysToAdvance > 0)
        {
            StoryManager.Instance.AdvanceDay(daysToAdvance);
            if (showDebugLogs) Debug.Log($"[TimeSkipTrigger] Advanced {daysToAdvance} days");
        }

        // Set flags TRUE
        if (setFlagsTrue != null)
        {
            foreach (string flag in setFlagsTrue)
            {
                if (!string.IsNullOrEmpty(flag))
                {
                    StoryManager.Instance.SetFlag(flag, true);
                    if (showDebugLogs) Debug.Log($"[TimeSkipTrigger] Set flag '{flag}' = true");
                }
            }
        }

        // Set flags FALSE
        if (setFlagsFalse != null)
        {
            foreach (string flag in setFlagsFalse)
            {
                if (!string.IsNullOrEmpty(flag))
                {
                    StoryManager.Instance.SetFlag(flag, false);
                    if (showDebugLogs) Debug.Log($"[TimeSkipTrigger] Set flag '{flag}' = false");
                }
            }
        }

        // Variable changes
        if (variableChanges != null)
        {
            foreach (var change in variableChanges)
            {
                change.Apply();
                if (showDebugLogs) Debug.Log($"[TimeSkipTrigger] Applied variable change: {change.variableName}");
            }
        }
    }

    private void OnTimeSkipComplete(PlayerMovement playerMovement)
    {
        if (showDebugLogs) Debug.Log($"[TimeSkipTrigger] {gameObject.name}: Time skip completed!");
        
        isPlaying = false;

        // Mở khóa player movement
        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(true);
        }
    }

    private bool CheckConditions()
    {
        if (StoryManager.Instance == null) return true;

        // Check required flags
        if (requiredFlags != null && requiredFlags.Length > 0)
        {
            foreach (string flag in requiredFlags)
            {
                if (!string.IsNullOrEmpty(flag) && !StoryManager.Instance.GetFlag(flag))
                {
                    if (showDebugLogs) Debug.Log($"[TimeSkipTrigger] {gameObject.name}: Missing required flag '{flag}'");
                    return false;
                }
            }
        }

        // Check forbidden flags
        if (forbiddenFlags != null && forbiddenFlags.Length > 0)
        {
            foreach (string flag in forbiddenFlags)
            {
                if (!string.IsNullOrEmpty(flag) && StoryManager.Instance.GetFlag(flag))
                {
                    if (showDebugLogs) Debug.Log($"[TimeSkipTrigger] {gameObject.name}: Has forbidden flag '{flag}'");
                    return false;
                }
            }
        }

        return true;
    }

    private PlayerMovement FindPlayerMovement()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            return player.GetComponent<PlayerMovement>();
        }
        return null;
    }

    /// <summary>
    /// Coroutine hiển thị nhiều đoạn text tuần tự trên màn đen
    /// </summary>
    private IEnumerator MultiTextFadeCoroutine()
    {
        // Fade out to black trước
        yield return ScreenFader.Instance.FadeOutCoroutine();

        // Loop qua từng text entry
        for (int i = 0; i < textEntries.Length; i++)
        {
            var entry = textEntries[i];
            
            if (string.IsNullOrEmpty(entry.text)) continue;

            // Delay trước khi hiển thị text này
            if (entry.delayBefore > 0)
            {
                yield return new WaitForSecondsRealtime(entry.delayBefore);
            }

            if (showDebugLogs) Debug.Log($"[TimeSkipTrigger] Showing text {i + 1}/{textEntries.Length}: '{entry.text}'");

            // Hiển thị text với fade in/out
            yield return ScreenFader.Instance.ShowTextCoroutine(entry.text, entry.displayDuration);
        }

        // Không fade in ở đây - để TimeSkipCoroutine xử lý sau khi apply effects
    }

    /// <summary>
    /// Reset trigger để có thể trigger lại
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        if (showDebugLogs) Debug.Log($"[TimeSkipTrigger] {gameObject.name}: Trigger reset");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        
        var collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }

        // Draw text preview
        #if UNITY_EDITOR
        string previewText;
        if (textEntries != null && textEntries.Length > 0)
        {
            previewText = $"[{textEntries.Length} texts] {textEntries[0].text}";
        }
        else
        {
            previewText = timeSkipText;
        }
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, previewText);
        #endif
    }
}
