using UnityEngine;

/// <summary>
/// StorytellingTrigger - Component để trigger storytelling sequence
/// Có thể đặt trong scene hoặc gọi từ code
/// </summary>
public class StorytellingTrigger : MonoBehaviour
{
    [Header("Storytelling Sequence")]
    [Tooltip("Sequence sẽ chơi khi trigger")]
    [SerializeField] private StorytellingSequenceData sequenceData;

    [Header("Trigger Settings")]
    [Tooltip("Mode kích hoạt")]
    [SerializeField] private TriggerMode mode = TriggerMode.OnSceneStart;
    
    [Tooltip("Phím tương tác (chỉ dùng cho OnInteract mode)")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Tooltip("Chỉ trigger một lần")]
    [SerializeField] private bool triggerOnce = true;

    [Header("Conditions (Optional)")]
    [Tooltip("Flags cần có để trigger")]
    [SerializeField] private string[] requiredFlags;

    [Tooltip("Flags không được có")]
    [SerializeField] private string[] forbiddenFlags;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private bool hasTriggered = false;
    private bool playerInRange = false;

    public enum TriggerMode
    {
        OnSceneStart,       // Tự động khi scene load
        OnTriggerEnter,     // Khi player vào trigger zone
        OnInteract,         // Khi player nhấn phím trong trigger zone
        Manual              // Gọi từ code
    }

    private void Start()
    {
        if (mode == TriggerMode.OnSceneStart)
        {
            TriggerSequence();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (mode == TriggerMode.OnTriggerEnter)
            {
                TriggerSequence();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (mode == TriggerMode.OnInteract && playerInRange && !hasTriggered)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                TriggerSequence();
            }
        }
    }

    /// <summary>
    /// Trigger sequence (có thể gọi từ code)
    /// </summary>
    public void TriggerSequence()
    {
        if (hasTriggered && triggerOnce)
        {
            if (showDebugLogs) Debug.Log($"[StorytellingTrigger] Already triggered: {name}");
            return;
        }

        if (sequenceData == null)
        {
            Debug.LogError($"[StorytellingTrigger] No sequence data assigned: {name}");
            return;
        }

        // Check conditions
        if (!CheckConditions())
        {
            if (showDebugLogs) Debug.Log($"[StorytellingTrigger] Conditions not met: {name}");
            return;
        }

        if (showDebugLogs) Debug.Log($"[StorytellingTrigger] Triggering sequence: {sequenceData.sequenceName}");

        hasTriggered = true;
        StorytellingManager.Instance.PlaySequence(sequenceData, OnSequenceComplete);
    }

    private bool CheckConditions()
    {
        if (StoryManager.Instance == null) return true;

        // Check required flags
        if (requiredFlags != null && requiredFlags.Length > 0)
        {
            if (!StoryManager.Instance.CheckRequiredFlags(requiredFlags))
            {
                return false;
            }
        }

        // Check forbidden flags
        if (forbiddenFlags != null && forbiddenFlags.Length > 0)
        {
            if (!StoryManager.Instance.CheckForbiddenFlags(forbiddenFlags))
            {
                return false;
            }
        }

        return true;
    }

    private void OnSequenceComplete()
    {
        if (showDebugLogs) Debug.Log($"[StorytellingTrigger] Sequence complete: {sequenceData.sequenceName}");
    }

    private void OnDrawGizmos()
    {
        // Draw trigger zone if using trigger modes
        if (mode == TriggerMode.OnTriggerEnter || mode == TriggerMode.OnInteract)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
                Gizmos.DrawCube(transform.position, col.bounds.size);
            }
        }
    }
}

