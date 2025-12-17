using UnityEngine;
using System.Collections;

/// <summary>
/// ClassmateExitController - Điều khiển Cảnh 11: Đám bạn đi ra khỏi lớp
/// Kích hoạt sau khi Cảnh 10 (VN) kết thúc, làm nhóm bạn đi ra cửa lớp
/// Sau đó fade to black và chuyển sang Cảnh 12
/// </summary>
public class ClassmateExitController : MonoBehaviour
{
    [Header("NPCs - Classmates")]
    [Tooltip("Các NPC bạn học sẽ đi ra ngoài (cần có NPCWaypointWalker)")]
    [SerializeField] private NPCWaypointWalker[] classmates;

    [Header("Shared Waypoints")]
    [Tooltip("Waypoints chung cho tất cả NPCs (nếu không set riêng cho từng NPC)")]
    [SerializeField] private Transform[] sharedWaypoints;

    [Header("Trigger Settings")]
    [Tooltip("Tự động trigger khi scene load")]
    [SerializeField] private bool triggerOnStart = true;

    [Tooltip("Delay trước khi NPCs bắt đầu đi (giây)")]
    [SerializeField] private float startDelay = 1f;

    [Tooltip("Delay giữa mỗi NPC bắt đầu đi (giây) - tạo hiệu ứng stagger")]
    [SerializeField] private float staggerDelay = 0.3f;

    [Header("Player Control")]
    [Tooltip("Disable player movement trong cảnh này")]
    [SerializeField] private bool disablePlayerMovement = true;

    [Tooltip("Ẩn player trong cảnh này")]
    [SerializeField] private bool hidePlayer = false;

    [Header("Scene Transition")]
    [Tooltip("Thời gian chờ sau khi NPC cuối đi hết (giây)")]
    [SerializeField] private float waitAfterExit = 2f;

    [Tooltip("Fade to black sau khi NPCs đi hết")]
    [SerializeField] private bool fadeToBlack = true;

    [Tooltip("Load scene tiếp theo (để trống = không load scene mới)")]
    [SerializeField] private string nextSceneName = "";

    [Tooltip("Spawn point ID trong scene tiếp theo")]
    [SerializeField] private string nextSpawnPointId = "";

    [Header("VN Scene Trigger (Alternative)")]
    [Tooltip("Trigger VN scene thay vì load scene mới")]
    [SerializeField] private VNSceneData vnSceneToTrigger;

    [Header("Story Flags")]
    [Tooltip("Flags cần có để trigger (tất cả phải true)")]
    [SerializeField] private string[] requiredFlags;

    [Tooltip("Flags không được có (nếu có thì không trigger)")]
    [SerializeField] private string[] forbiddenFlags;

    [Tooltip("Flag set khi hoàn thành cảnh")]
    [SerializeField] private string completionFlag = "scene11_classmates_left";

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private bool hasTriggered = false;
    private int completedCount = 0;
    private PlayerMovement playerMovement;

    private void Start()
    {
        // Tìm player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerMovement = playerObj.GetComponent<PlayerMovement>();
        }

        if (triggerOnStart)
        {
            // Kiểm tra flags
            if (!CheckRequiredFlags() || !CheckForbiddenFlags())
            {
                if (showDebugLogs)
                    Debug.Log("[ClassmateExitController] Không đủ điều kiện flags, skip cảnh");
                return;
            }

            StartCoroutine(TriggerClassmatesExit());
        }
    }

    private bool CheckRequiredFlags()
    {
        if (StoryManager.Instance == null) return true;
        if (requiredFlags == null || requiredFlags.Length == 0) return true;

        foreach (string flag in requiredFlags)
        {
            if (!string.IsNullOrEmpty(flag) && !StoryManager.Instance.GetFlag(flag))
            {
                if (showDebugLogs)
                    Debug.Log($"[ClassmateExitController] Thiếu required flag: {flag}");
                return false;
            }
        }
        return true;
    }

    private bool CheckForbiddenFlags()
    {
        if (StoryManager.Instance == null) return true;
        if (forbiddenFlags == null || forbiddenFlags.Length == 0) return true;

        foreach (string flag in forbiddenFlags)
        {
            if (!string.IsNullOrEmpty(flag) && StoryManager.Instance.GetFlag(flag))
            {
                if (showDebugLogs)
                    Debug.Log($"[ClassmateExitController] Có forbidden flag: {flag}");
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Trigger thủ công từ bên ngoài
    /// </summary>
    public void ManualTrigger()
    {
        if (!hasTriggered)
        {
            StartCoroutine(TriggerClassmatesExit());
        }
    }

    private IEnumerator TriggerClassmatesExit()
    {
        if (hasTriggered) yield break;
        hasTriggered = true;

        if (showDebugLogs)
            Debug.Log("[ClassmateExitController] === BẮT ĐẦU CẢNH 11: Đám bạn đi ra ngoài ===");

        // Disable player movement
        if (disablePlayerMovement && playerMovement != null)
        {
            playerMovement.enabled = false;
            if (showDebugLogs)
                Debug.Log("[ClassmateExitController] Disabled player movement");
        }

        // Ẩn player nếu cần
        if (hidePlayer)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerObj.SetActive(false);
            }
        }

        // Delay ban đầu
        yield return new WaitForSeconds(startDelay);

        // Kích hoạt từng NPC với delay stagger
        completedCount = 0;
        foreach (var classmate in classmates)
        {
            if (classmate != null)
            {
                // Set shared waypoints nếu NPC chưa có waypoints riêng
                if (sharedWaypoints != null && sharedWaypoints.Length > 0)
                {
                    classmate.SetWaypoints(sharedWaypoints);
                }

                // Subscribe event khi NPC đi xong
                classmate.OnReachedDestination += OnClassmateReachedDestination;

                // Bắt đầu đi
                classmate.StartWalking();

                if (showDebugLogs)
                    Debug.Log($"[ClassmateExitController] {classmate.name} bắt đầu đi ra ngoài");

                yield return new WaitForSeconds(staggerDelay);
            }
        }

        // Chờ tất cả NPCs đi hết
        yield return new WaitUntil(() => completedCount >= classmates.Length);

        if (showDebugLogs)
            Debug.Log("[ClassmateExitController] Tất cả bạn học đã ra khỏi lớp");

        // Chờ thêm một chút
        yield return new WaitForSeconds(waitAfterExit);

        // Set completion flag
        if (StoryManager.Instance != null && !string.IsNullOrEmpty(completionFlag))
        {
            StoryManager.Instance.SetFlag(completionFlag, true);
            if (showDebugLogs)
                Debug.Log($"[ClassmateExitController] Set flag: {completionFlag}");
        }

        // Chuyển scene hoặc trigger VN
        yield return HandleSceneTransition();
    }

    private void OnClassmateReachedDestination()
    {
        completedCount++;
        if (showDebugLogs)
            Debug.Log($"[ClassmateExitController] NPC đã đi xong ({completedCount}/{classmates.Length})");
    }

    private IEnumerator HandleSceneTransition()
    {
        // Option 1: Trigger VN scene
        if (vnSceneToTrigger != null)
        {
            if (showDebugLogs)
                Debug.Log($"[ClassmateExitController] Trigger VN scene: {vnSceneToTrigger.name}");

            if (fadeToBlack && ScreenFader.Instance != null)
            {
                yield return ScreenFader.Instance.FadeOutCoroutine();
            }

            // Re-enable player trước khi vào VN
            RestorePlayerState();

            if (VisualNovelManager.Instance != null)
            {
                VisualNovelManager.Instance.StartVNScene(vnSceneToTrigger);
            }
            yield break;
        }

        // Option 2: Load scene mới
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            if (showDebugLogs)
                Debug.Log($"[ClassmateExitController] Load scene: {nextSceneName}");

            // GameManager.LoadScene đã có fade built-in
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadScene(nextSceneName, nextSpawnPointId);
            }
            yield break;
        }

        // Option 3: Chỉ fade to black (không chuyển scene)
        if (fadeToBlack)
        {
            if (ScreenFader.Instance != null)
            {
                yield return ScreenFader.Instance.FadeOutCoroutine();
            }

            // Chờ một chút rồi fade in lại
            yield return new WaitForSeconds(1f);

            RestorePlayerState();

            if (ScreenFader.Instance != null)
            {
                yield return ScreenFader.Instance.FadeInCoroutine();
            }
        }
        else
        {
            RestorePlayerState();
        }

        if (showDebugLogs)
            Debug.Log("[ClassmateExitController] === KẾT THÚC CẢNH 11 ===");
    }

    private void RestorePlayerState()
    {
        // Re-enable player movement
        if (disablePlayerMovement && playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // Show player nếu đã ẩn
        if (hidePlayer)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerObj.SetActive(true);
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe events
        if (classmates != null)
        {
            foreach (var classmate in classmates)
            {
                if (classmate != null)
                {
                    classmate.OnReachedDestination -= OnClassmateReachedDestination;
                }
            }
        }
    }
}

