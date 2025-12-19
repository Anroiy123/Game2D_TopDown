using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorController : MonoBehaviour
{
    // Tham chiếu đến Animator trên cánh cửa
    private Animator animator;

    // Đặt KeyCode bạn muốn dùng (Ví dụ: E)
    public KeyCode interactKey = KeyCode.E;

    // Collider dùng để chặn player khi cửa đóng (KHÔNG phải trigger)
    [Header("Collision Blocking")]
    [Tooltip("Kéo BoxCollider2D dùng để chặn player vào đây (Is Trigger = false)")]
    public BoxCollider2D blockingCollider;

    // Biến để kiểm tra xem nhân vật có đang trong vùng Trigger không
    private bool playerIsNear = false;

    // Animator hash để tối ưu hiệu suất
    private readonly int isOpenHash = Animator.StringToHash("IsOpen");
    private readonly int doorClosedStateHash = Animator.StringToHash("Door_Closed");

    void Start()
    {
        // Lấy component Animator khi script khởi chạy
        animator = GetComponent<Animator>();

        // FIX Issue 1: Khởi tạo cửa ở trạng thái đóng KHÔNG có animation
        // Play animation Door_Closed ở frame cuối (normalizedTime = 1) để không play animation
        animator.Play(doorClosedStateHash, 0, 1f);
        animator.SetBool(isOpenHash, false);

        // FIX Issue 2: Đảm bảo blocking collider được bật khi cửa đóng
        if (blockingCollider != null)
        {
            blockingCollider.enabled = true;
        }
    }

    void Update()
    {
        // QUAN TRỌNG: Không xử lý interaction khi VN mode đang active
        if (VisualNovelManager.Instance != null && VisualNovelManager.Instance.IsVNModeActive)
        {
            return;
        }

        // 1. Kiểm tra xem người chơi có đang trong vùng Trigger KHÔNG
        if (playerIsNear)
        {
            // 2. Kiểm tra xem phím tương tác (E) có được nhấn KHÔNG
            if (Input.GetKeyDown(interactKey))
            {
                ToggleDoor();
            }
        }
    }

    /// <summary>
    /// Đổi trạng thái cửa (mở ↔ đóng)
    /// </summary>
    private void ToggleDoor()
    {
        // Lấy trạng thái hiện tại của cửa (Đang mở hay Đang đóng)
        bool currentOpenState = animator.GetBool(isOpenHash);
        bool newState = !currentOpenState;

        // Đảo ngược trạng thái để mở/đóng cửa
        animator.SetBool(isOpenHash, newState);

        // FIX Issue 2: Bật/tắt blocking collider dựa trên trạng thái cửa
        if (blockingCollider != null)
        {
            // Nếu cửa MỞ -> TẮT blocking collider (player đi qua được)
            // Nếu cửa ĐÓNG -> BẬT blocking collider (player bị chặn)
            blockingCollider.enabled = !newState;
        }
    }

    // Khi nhân vật bước vào vùng Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            // (Optional: Hiển thị UI nhắc nhở "Bấm E để mở")
        }
    }

    // Khi nhân vật bước ra khỏi vùng Trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            // (Optional: Tự động đóng cửa sau khi nhân vật đi ra)
            // animator.SetBool(isOpenHash, false);
        }
    }
}