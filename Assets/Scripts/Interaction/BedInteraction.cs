using UnityEngine;
using System.Collections;

public class BedInteraction : MonoBehaviour
{
    [Header("Sleep Settings")]
    [SerializeField] private float sleepDuration = 2f; // Thời gian animation ngủ
    [SerializeField] private bool advanceDay = true; // Có tăng ngày không

    [Header("Sleep Position")]
    [Tooltip("Vị trí player sẽ nằm khi ngủ (kéo thả Transform hoặc để trống để dùng center của BoxCollider)")]
    [SerializeField] private Transform sleepPosition; // Vị trí nằm ngủ cụ thể

    [Header("UI References")]
    [SerializeField] private GameObject interactionPrompt; // Optional: UI hint "Press E"

    private bool isPlayerNearby = false;
    private bool isSleeping = false;
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
        if (isPlayerNearby && !isSleeping && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SleepRoutine());
        }
    }

    private IEnumerator SleepRoutine()
    {
        isSleeping = true;
        
        // 1. Khóa player movement
        if (playerMovement != null)
        {
            playerMovement.SetSleepingState(true);
        }

        // 2. Di chuyển player đến vị trí ngủ trên giường
        if (playerTransform != null)
        {
            Vector3 targetPosition = GetSleepPosition();
            playerTransform.position = targetPosition;
        }

        // 3. Bật animation ngủ
        if (playerAnimator != null)
        {
            playerAnimator.SetBool(sleepingHash, true);
        }

        // Ẩn prompt
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        Debug.Log("Player bắt đầu ngủ...");

        // 4. Đợi animation ngủ
        yield return new WaitForSeconds(sleepDuration);

        // 5. Xử lý logic sau khi ngủ (tăng ngày)
        if (advanceDay && StoryManager.Instance != null)
        {
            int currentDay = StoryManager.Instance.GetVariable(StoryManager.VarKeys.CURRENT_DAY);
            StoryManager.Instance.SetVariable(StoryManager.VarKeys.CURRENT_DAY, currentDay + 1);
            Debug.Log($"Đã sang ngày mới: Ngày {currentDay + 1}");
        }

        // 6. Tắt animation ngủ
        if (playerAnimator != null)
        {
            playerAnimator.SetBool(sleepingHash, false);
        }

        // 7. Mở khóa player movement
        if (playerMovement != null)
        {
            playerMovement.SetSleepingState(false);
        }

        isSleeping = false;
        Debug.Log("Player đã thức dậy!");

        // Hiện lại prompt nếu player vẫn ở gần
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

