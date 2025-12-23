using UnityEngine;

public class BedInteraction : MonoBehaviour
{
    [Header("Sleep Settings")]
    [SerializeField] private bool advanceDay = true; // Có tăng ngày không khi thức dậy

    [Header("Sleep Position")]
    [Tooltip("Vị trí player sẽ nằm khi ngủ (kéo thả Transform hoặc để trống để dùng center của BoxCollider)")]
    [SerializeField] private Transform sleepPosition; // Vị trí nằm ngủ cụ thể

    [Header("UI References")]
    [SerializeField] private GameObject interactionPrompt; // Optional: UI hint "Press E"

    private bool isPlayerNearby = false;
    private bool isSleeping = false;
    private Vector3 originalPosition; // Lưu vị trí trước khi ngủ
    private Transform playerTransform;
    private PlayerMovement playerMovement;
    private Animator playerAnimator;
    private BoxCollider2D bedCollider;
    private InteractableOutline outline;

    // Animator hash
    private readonly int sleepingHash = Animator.StringToHash("IsSleeping");

    private void Awake()
    {
        bedCollider = GetComponent<BoxCollider2D>();
        outline = GetComponent<InteractableOutline>();
    }

    private void Update()
    {
        // QUAN TRỌNG: Không xử lý interaction khi VN mode đang active
        if (VisualNovelManager.Instance != null && VisualNovelManager.Instance.IsVNModeActive)
        {
            return;
        }

        // Toggle sleep: nhấn E lần 1 để ngủ, nhấn E lần 2 để thức dậy
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            ToggleSleep();
        }
    }

    /// <summary>
    /// Toggle trạng thái ngủ/thức - giống như Chair toggle
    /// </summary>
    private void ToggleSleep()
    {
        isSleeping = !isSleeping;
        Debug.Log($"ToggleSleep called! isSleeping = {isSleeping}");

        if (isSleeping)
        {
            // --- BẮT ĐẦU NGỦ ---
            StartSleeping();
        }
        else
        {
            // --- THỨC DẬY ---
            WakeUp();
        }
    }

    /// <summary>
    /// Bắt đầu ngủ
    /// </summary>
    private void StartSleeping()
    {
        // 1. Lưu vị trí hiện tại để có thể quay lại
        if (playerTransform != null)
        {
            originalPosition = playerTransform.position;
        }

        // 2. Khóa player movement
        if (playerMovement != null)
        {
            playerMovement.SetSleepingState(true);
        }

        // 3. Di chuyển player đến vị trí ngủ trên giường
        if (playerTransform != null)
        {
            Vector3 targetPosition = GetSleepPosition();
            playerTransform.position = targetPosition;
        }

        // 4. Bật animation ngủ
        if (playerAnimator != null)
        {
            playerAnimator.SetBool(sleepingHash, true);
        }

        // 5. Ẩn prompt
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        Debug.Log("Player bắt đầu ngủ... Nhấn E để thức dậy.");
    }

    /// <summary>
    /// Thức dậy
    /// </summary>
    private void WakeUp()
    {
        // 1. Tắt animation ngủ
        if (playerAnimator != null)
        {
            playerAnimator.SetBool(sleepingHash, false);
        }

        // 2. Xử lý logic sau khi ngủ (tăng ngày)
        if (advanceDay && StoryManager.Instance != null)
        {
            int currentDay = StoryManager.Instance.GetVariable(StoryManager.VarKeys.CURRENT_DAY);
            StoryManager.Instance.SetVariable(StoryManager.VarKeys.CURRENT_DAY, currentDay + 1);
            Debug.Log($"Đã sang ngày mới: Ngày {currentDay + 1}");
        }

        // 3. Đẩy player ra khỏi giường một chút (tránh trigger lại ngay)
        if (playerTransform != null)
        {
            // Đẩy xuống phía trước giường (Y dương = xuống trong hệ tọa độ này)
            playerTransform.position = originalPosition + new Vector3(0, 0.5f, 0);
        }

        // 4. Mở khóa player movement
        if (playerMovement != null)
        {
            playerMovement.SetSleepingState(false);
        }

        Debug.Log("Player đã thức dậy!");

        // 5. Hiện lại prompt nếu player vẫn ở gần
        if (isPlayerNearby && interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerTransform = collision.transform;
            playerMovement = collision.GetComponent<PlayerMovement>();
            playerAnimator = collision.GetComponent<Animator>();

            // Bật outline khi player đến gần
            if (outline != null)
            {
                outline.EnableOutline();
            }

            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
            Debug.Log("Gần giường! Nhấn E để ngủ.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            playerTransform = null;
            playerMovement = null;
            playerAnimator = null;

            // Tắt outline khi player rời đi
            if (outline != null)
            {
                outline.DisableOutline();
            }

            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Tính toán vị trí player sẽ nằm khi ngủ
    /// </summary>
    private Vector3 GetSleepPosition()
    {
        // Ưu tiên 1: Dùng Sleep Position nếu được gán
        if (sleepPosition != null)
        {
            return sleepPosition.position;
        }

        // Ưu tiên 2: Dùng center của BoxCollider
        if (bedCollider != null)
        {
            Vector2 colliderCenter = (Vector2)transform.position + bedCollider.offset;
            return new Vector3(colliderCenter.x, colliderCenter.y, transform.position.z);
        }

        // Fallback: Dùng vị trí của giường
        return transform.position;
    }

    // Vẽ vị trí ngủ trong Scene view để dễ debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 sleepPos = GetSleepPosition();
        Gizmos.DrawWireSphere(sleepPos, 0.3f);
        Gizmos.DrawLine(transform.position, sleepPos);
    }
}

