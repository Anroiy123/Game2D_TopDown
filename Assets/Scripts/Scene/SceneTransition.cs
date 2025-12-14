using UnityEngine;

/// <summary>
/// SceneTransition - Component gắn vào Door/Portal để chuyển scene
/// Player đi vào trigger zone hoặc nhấn phím tương tác sẽ chuyển scene
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SceneTransition : MonoBehaviour
{
    [Header("Target Scene")]
    [Tooltip("Tên scene sẽ load khi tương tác")]
    [SerializeField] private string targetSceneName;
    
    [Tooltip("ID của spawn point trong scene đích")]
    [SerializeField] private string targetSpawnPointId;

    [Header("Transition Mode")]
    [Tooltip("Tự động chuyển khi vào trigger hoặc cần nhấn phím")]
    [SerializeField] private TransitionMode mode = TransitionMode.OnInteract;
    
    [Tooltip("Phím tương tác (chỉ dùng cho OnInteract mode)")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Header("Conditions (Optional)")]
    [Tooltip("Flags cần có để có thể chuyển scene")]
    [SerializeField] private string[] requiredFlags;
    
    [Tooltip("Flags không được có (cấm chuyển nếu có)")]
    [SerializeField] private string[] forbiddenFlags;

    [Header("Visual Feedback")]
    [Tooltip("UI prompt hiển thị khi player ở gần")]
    [SerializeField] private GameObject interactionPrompt;

    public enum TransitionMode
    {
        OnTriggerEnter,  // Tự động chuyển khi chạm vào
        OnInteract       // Cần nhấn phím E
    }

    private bool playerInRange = false;

    private void Start()
    {
        // Đảm bảo Collider là trigger
        var collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }

        // Ẩn prompt lúc đầu
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (mode == TransitionMode.OnInteract && playerInRange)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                TryTransition();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (mode == TransitionMode.OnTriggerEnter)
        {
            TryTransition();
        }
        else if (interactionPrompt != null)
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
    /// Thử chuyển scene (kiểm tra conditions trước)
    /// </summary>
    private void TryTransition()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning($"[SceneTransition] {gameObject.name}: Target scene name is empty!");
            return;
        }

        // Kiểm tra conditions
        if (!CheckConditions())
        {
            Debug.Log($"[SceneTransition] Conditions not met for transition to {targetSceneName}");
            return;
        }

        // Thực hiện chuyển scene
        DoTransition();
    }

    private bool CheckConditions()
    {
        if (StoryManager.Instance == null) return true;

        // Kiểm tra required flags
        if (!StoryManager.Instance.CheckRequiredFlags(requiredFlags))
        {
            return false;
        }

        // Kiểm tra forbidden flags
        if (!StoryManager.Instance.CheckForbiddenFlags(forbiddenFlags))
        {
            return false;
        }

        return true;
    }

    private void DoTransition()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadScene(targetSceneName, targetSpawnPointId);
        }
        else
        {
            Debug.LogWarning("[SceneTransition] GameManager not found! Using direct scene load.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
        }
    }

    #region Editor Helpers
    private void OnDrawGizmos()
    {
        // Vẽ icon door trong editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
    }
    #endregion
}

