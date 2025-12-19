using UnityEngine;

/// <summary>
/// Helper component để trigger NPCSurroundPlayer dễ dàng hơn
/// Có BoxCollider2D trigger zone - khi player đi vào sẽ tự động trigger
/// Hỗ trợ conditional flags để control khi nào trigger
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class NPCSurroundController : MonoBehaviour
{
    [Header("Quick Setup")]
    public GameObject[] npcBullies;
    public float surroundRadius = 1f;
    public NPCSurroundPlayer.FormationType formationType = NPCSurroundPlayer.FormationType.Semicircle;
    public float moveSpeed = 5f;
    public float delayBetweenNPCs = 0.2f;

    [Header("Next Scene")]
    public VNSceneData nextVNScene;

    [Header("Trigger Settings")]
    public bool triggerOnce = true;
    public bool autoSetupOnStart = true;

    [Header("Conditions")]
    [Tooltip("Flags cần có để trigger (VD: chose_confront_bullies)")]
    public string[] requiredFlags;

    [Tooltip("Flags không được có (VD: ran_from_bullies)")]
    public string[] forbiddenFlags;

    private NPCSurroundPlayer surroundPlayer;
    private bool hasTriggered = false;

    private void Start()
    {
        if (autoSetupOnStart) SetupSurroundPlayer();

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (hasTriggered && triggerOnce)
        {
            Debug.Log("[NPCSurroundController] Already triggered, skipping.");
            return;
        }

        // QUAN TRỌNG: Kiểm tra conditions trước khi trigger
        if (!CheckConditions())
        {
            Debug.Log("[NPCSurroundController] Conditions not met, skipping surround.");
            return;
        }

        hasTriggered = true;
        Debug.Log($"[NPCSurroundController] ✓ Player entered at {other.transform.position}, conditions met!");
        StartSurround();
    }

    /// <summary>
    /// Kiểm tra required flags và forbidden flags
    /// </summary>
    private bool CheckConditions()
    {
        if (StoryManager.Instance == null)
        {
            Debug.LogWarning("[NPCSurroundController] StoryManager not found, allowing trigger.");
            return true;
        }

        // Check required flags
        bool requiredMet = StoryManager.Instance.CheckRequiredFlags(requiredFlags);
        if (!requiredMet)
        {
            Debug.Log($"[NPCSurroundController] ✗ BLOCKED - Required flags not met. Need: [{string.Join(", ", requiredFlags ?? new string[0])}]");
            return false;
        }

        // Check forbidden flags
        bool forbiddenMet = StoryManager.Instance.CheckForbiddenFlags(forbiddenFlags);
        if (!forbiddenMet)
        {
            Debug.Log($"[NPCSurroundController] ✗ BLOCKED - Has forbidden flags. Forbidden: [{string.Join(", ", forbiddenFlags ?? new string[0])}]");
            return false;
        }

        Debug.Log("[NPCSurroundController] ✓ All conditions met!");
        return true;
    }

    [ContextMenu("Setup Surround Player")]
    public void SetupSurroundPlayer()
    {
        surroundPlayer = GetComponent<NPCSurroundPlayer>();
        if (surroundPlayer == null)
        {
            surroundPlayer = gameObject.AddComponent<NPCSurroundPlayer>();
        }

        // Dùng public setters
        surroundPlayer.SetNPCs(npcBullies);
        surroundPlayer.SetRadius(surroundRadius);
        surroundPlayer.SetFormationType(formationType);
        surroundPlayer.SetMoveSpeed(moveSpeed);
        surroundPlayer.SetDelay(delayBetweenNPCs);
        surroundPlayer.SetNextVNScene(nextVNScene);

        Debug.Log($"[NPCSurroundController] Setup done: {npcBullies?.Length ?? 0} NPCs");
    }

    [ContextMenu("Start Surrounding")]
    public void StartSurround()
    {
        if (surroundPlayer == null) SetupSurroundPlayer();

        if (surroundPlayer != null)
        {
            surroundPlayer.StartSurrounding();
        }
    }
}