using UnityEngine;

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

    public enum TriggerMode
    {
        OnTriggerEnter,
        OnInteract,
        OnSceneStart
    }

    private bool playerInRange = false;
    private bool hasTriggered = false;

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
        if (hasTriggered && triggerOnce) return;
        if (!CheckConditions()) return;

        if (vnScene == null)
        {
            Debug.LogWarning($"[VNTrigger] {gameObject.name}: VNScene is null!");
            return;
        }

        hasTriggered = true;

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        VisualNovelManager.Instance.StartVNScene(vnScene, OnVNComplete);
    }

    private bool CheckConditions()
    {
        if (StoryManager.Instance == null) return true;

        return StoryManager.Instance.CheckRequiredFlags(requiredFlags) &&
               StoryManager.Instance.CheckForbiddenFlags(forbiddenFlags);
    }

    private void OnVNComplete()
    {
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
    }
}

