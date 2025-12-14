using UnityEngine;

/// <summary>
/// LocalTeleporter - Dịch chuyển player trong cùng scene
/// Sử dụng cho cầu thang, cổng bí mật, lối đi tắt...
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class LocalTeleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    [Tooltip("Vị trí đích sẽ dịch chuyển đến")]
    [SerializeField] private Transform targetPosition;

    [Tooltip("Hướng player nhìn sau khi dịch chuyển")]
    [SerializeField] private FacingDirection facingAfterTeleport = FacingDirection.Down;

    [Header("Transition Mode")]
    [Tooltip("Tự động chuyển khi vào trigger hoặc cần nhấn phím")]
    [SerializeField] private TransitionMode mode = TransitionMode.OnInteract;

    [Tooltip("Phím tương tác (chỉ dùng cho OnInteract mode)")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Header("Visual Feedback")]
    [Tooltip("UI prompt hiển thị khi player ở gần")]
    [SerializeField] private GameObject interactionPrompt;

    [Header("Fade Settings")]
    [Tooltip("Sử dụng hiệu ứng fade khi teleport")]
    [SerializeField] private bool useFadeEffect = true;

    [Header("Optional Settings")]
    [Tooltip("Thời gian delay trước khi teleport (giây)")]
    [SerializeField] private float teleportDelay = 0f;

    public enum TransitionMode
    {
        OnTriggerEnter,  // Tự động chuyển khi chạm vào
        OnInteract       // Cần nhấn phím E
    }

    public enum FacingDirection
    {
        Up,
        Down,
        Left,
        Right,
        None  // Giữ nguyên hướng hiện tại
    }

    private bool playerInRange = false;
    private Transform playerTransform;

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
                TryTeleport();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        playerTransform = other.transform;

        if (mode == TransitionMode.OnTriggerEnter)
        {
            TryTeleport();
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
        // KHÔNG set playerTransform = null ở đây!
        // Vì coroutine có thể vẫn đang chạy và cần reference này

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private bool isTeleporting = false;
    private Transform cachedPlayerTransform; // Cache player cho coroutine

    private void TryTeleport()
    {
        if (isTeleporting) return;

        if (targetPosition == null)
        {
            Debug.LogWarning($"[LocalTeleporter] {gameObject.name}: Target position is not set!");
            return;
        }

        // Tìm player nếu chưa có
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        if (playerTransform == null)
        {
            Debug.LogError("[LocalTeleporter] Cannot find player!");
            return;
        }

        // Cache player transform để dùng trong coroutine
        // Tránh bị null khi OnTriggerExit được gọi
        cachedPlayerTransform = playerTransform;

        // Sử dụng fade effect nếu được bật
        if (useFadeEffect && ScreenFader.Instance != null)
        {
            StartCoroutine(TeleportWithFade());
        }
        else if (teleportDelay > 0)
        {
            StartCoroutine(TeleportWithDelay());
        }
        else
        {
            DoTeleport();
        }
    }

    private System.Collections.IEnumerator TeleportWithFade()
    {
        isTeleporting = true;

        // Kiểm tra ScreenFader
        if (ScreenFader.Instance == null)
        {
            Debug.LogWarning("[LocalTeleporter] ScreenFader not found, teleporting without fade");
            DoTeleport();
            isTeleporting = false;
            yield break;
        }

        // Fade out
        yield return ScreenFader.Instance.FadeOutCoroutine();

        // Delay nếu có
        if (teleportDelay > 0)
        {
            yield return new WaitForSeconds(teleportDelay);
        }

        // Teleport
        DoTeleport();

        // Đợi vài frames để camera cập nhật
        yield return null;
        yield return null;

        // Fade in - đảm bảo luôn được gọi
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeInCoroutine();
        }
        else
        {
            Debug.LogError("[LocalTeleporter] ScreenFader bị null sau khi teleport!");
        }

        isTeleporting = false;
        Debug.Log("[LocalTeleporter] TeleportWithFade completed");
    }

    private System.Collections.IEnumerator TeleportWithDelay()
    {
        isTeleporting = true;
        yield return new WaitForSeconds(teleportDelay);
        DoTeleport();
        isTeleporting = false;
    }

    private void DoTeleport()
    {
        // Sử dụng cached player transform (đã được lưu trước khi OnTriggerExit)
        Transform player = cachedPlayerTransform;

        // Fallback nếu cache bị null
        if (player == null)
        {
            player = playerTransform;
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        if (player == null)
        {
            Debug.LogError("[LocalTeleporter] DoTeleport: Player is null!");
            return;
        }

        // Lưu vị trí cũ để tính delta
        Vector3 oldPosition = player.position;

        // Di chuyển player đến vị trí đích
        player.position = targetPosition.position;

        // Tính khoảng cách teleport
        Vector3 positionDelta = targetPosition.position - oldPosition;

        // Thông báo cho Cinemachine snap camera ngay lập tức
        try
        {
            CameraHelper.NotifyTargetTeleported(player, positionDelta);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[LocalTeleporter] CameraHelper error (ignored): {e.Message}");
        }

        // Set hướng nhìn của player
        if (facingAfterTeleport != FacingDirection.None)
        {
            SetPlayerFacing(player.gameObject, GetFacingVector(facingAfterTeleport));
        }

        Debug.Log($"[LocalTeleporter] Teleported player to {targetPosition.position}");
    }

    private Vector2 GetFacingVector(FacingDirection direction)
    {
        return direction switch
        {
            FacingDirection.Up => Vector2.up,
            FacingDirection.Down => Vector2.down,
            FacingDirection.Left => Vector2.left,
            FacingDirection.Right => Vector2.right,
            _ => Vector2.down
        };
    }

    private void SetPlayerFacing(GameObject player, Vector2 direction)
    {
        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
            animator.SetFloat("Speed", 0f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Vẽ đường nối từ teleporter đến đích
        if (targetPosition != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetPosition.position);
            Gizmos.DrawWireSphere(targetPosition.position, 0.3f);
        }
    }
}

