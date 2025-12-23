using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private Vector3 sitOffset = new Vector3(0, 1.2f, 0); // Dịch xuống để ngồi phía trước ghế (Y dương = xuống)

    // Sprint state
    private bool isSprinting = false;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 movementInput;

    // Các biến xử lý logic Ngồi
    private bool isSitting = false;
    private bool canSit = false;
    private Vector3 chairPosition;
    private int defaultSortingOrder; // Lưu lại thứ tự lớp mặc định của nhân vật

    // Biến xử lý trạng thái nói chuyện
    private bool isTalking = false;

    // Biến xử lý trạng thái ngủ
    private bool isSleeping = false;

    // Biến kiểm tra có đang gần NPC không (để ưu tiên NPC hơn ghế)
    private bool isNearNPC = false;

    // Biến khóa movement từ bên ngoài (VD: bị vây quanh)
    private bool isMovementLocked = false;

    // Tối ưu hóa tên tham số Animation
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int horizontalHash = Animator.StringToHash("Horizontal");
    private readonly int verticalHash = Animator.StringToHash("Vertical");
    private readonly int sittingHash = Animator.StringToHash("IsSitting");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Lưu lại Order in Layer ban đầu (ví dụ là 0) để sau này đứng dậy thì trả lại như cũ
        if (spriteRenderer != null)
        {
            defaultSortingOrder = spriteRenderer.sortingOrder;
        }

        // Reset movement lock khi scene load
        isMovementLocked = false;
    }

    private void Update()
    {
        // QUAN TRỌNG: Không xử lý input khi VN mode đang active
        // VN mode sẽ disable PlayerMovement component, nhưng thêm check này để chắc chắn
        if (VisualNovelManager.Instance != null && VisualNovelManager.Instance.IsVNModeActive)
        {
            return;
        }

        // 1. Kiểm tra lệnh Ngồi (Phím E) - CHỈ KHI KHÔNG Ở GẦN NPC
        if (Input.GetKeyDown(KeyCode.E) && canSit && !isNearNPC)
        {
            ToggleSit();
        }

        // Cập nhật Animator cho trạng thái ngồi
        if (animator != null)
        {
            animator.SetBool(sittingHash, isSitting);
        }

        // NẾU ĐANG NGỒI, NÓI CHUYỆN, NGỦ HOẶC BỊ KHÓA THÌ KHÔNG LÀM GÌ CẢ (Không di chuyển)
        if (isSitting || isTalking || isSleeping || isMovementLocked) return;

        // 2. Nhận Input di chuyển (Chỉ chạy khi KHÔNG ngồi)
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        movementInput = movementInput.normalized;

        // 3. Check Sprint (Shift key) - chỉ sprint khi đang di chuyển
        isSprinting = Input.GetKey(KeyCode.LeftShift) && movementInput.sqrMagnitude > 0.01f;

        UpdateAnimations();
        UpdateFacing();
    }

    private void FixedUpdate()
    {
        // Nếu đang ngồi, nói chuyện, ngủ hoặc bị khóa thì dừng hẳn vật lý
        if (isSitting || isTalking || isSleeping || isMovementLocked)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (rb != null)
        {
            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
            rb.linearVelocity = movementInput * currentSpeed;
        }
    }

    // --- HÀM XỬ LÝ NGỒI/ĐỨNG ---
    private void ToggleSit()
    {
        isSitting = !isSitting; // Đảo ngược trạng thái
        Debug.Log($"ToggleSit called! isSitting = {isSitting}");

        if (isSitting)
        {
            Debug.Log($"Ngồi xuống! Chair position: {chairPosition}, Sit offset: {sitOffset}");
            
            // --- KHI BẮT ĐẦU NGỒI ---
            // 1. Dịch chuyển vào vị trí ghế (+ tinh chỉnh offset)
            transform.position = chairPosition + sitOffset;
            Debug.Log($"Player new position: {transform.position}");

            // 2. Dừng vật lý
            rb.linearVelocity = Vector2.zero;

            // 3. Set animation hướng lên (nhìn từ phía sau lưng player)
            if (animator != null)
            {
                animator.SetFloat(horizontalHash, 0f);
                animator.SetFloat(verticalHash, 1f); // Hướng lên
                animator.SetFloat(speedHash, 0f);
            }

            // 4. GIỮ NGUYÊN Order in Layer (không giảm nữa)
            // Player ngồi phía trước ghế nên không cần bị ghế che
            // if (spriteRenderer != null)
            // {
            //     spriteRenderer.sortingOrder = defaultSortingOrder - 1;
            // }
        }
        else
        {
            // --- KHI ĐỨNG DẬY ---
            // Đẩy nhân vật ra phía trước ghế một chút để không bị kẹt trigger
            transform.position += new Vector3(0, 0.5f, 0); // Y dương = xuống/phía trước
        }
    }

    // --- XỬ LÝ VA CHẠM TRIGGER ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Chair"))
        {
            canSit = true;
            chairPosition = collision.transform.position; // Lấy tâm của cái ghế
            // Debug.Log("Đã thấy ghế! Bấm E để ngồi.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Chair"))
        {
            canSit = false;
            // Nếu đi ra xa mà vẫn đang ngồi (bug) thì tự động đứng dậy
            if (isSitting)
            {
                ToggleSit();
            }
        }
    }

    // --- XỬ LÝ ANIMATION DI CHUYỂN ---
    private void UpdateAnimations()
    {
        if (animator == null) return;
        animator.SetFloat(speedHash, movementInput.sqrMagnitude);

        if (movementInput.sqrMagnitude > 0.01f)
        {
            animator.SetFloat(horizontalHash, movementInput.x);
            animator.SetFloat(verticalHash, movementInput.y);
        }
    }

    private void UpdateFacing()
    {
        if (spriteRenderer == null) return;

        if (movementInput.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (movementInput.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }

    // --- PUBLIC METHOD ĐỂ NPC CONTROL ---
    public void SetTalkingState(bool talking)
    {
        isTalking = talking;
        
        // Dừng animation khi bắt đầu nói chuyện
        if (isTalking && animator != null)
        {
            animator.SetFloat(speedHash, 0f);
        }
    }

    // --- PUBLIC METHOD ĐỂ CHECK NPC PROXIMITY ---
    public void SetNearNPC(bool nearNPC)
    {
        isNearNPC = nearNPC;
    }

    // --- PUBLIC METHOD ĐỂ BED CONTROL ---
    public void SetSleepingState(bool sleeping)
    {
        isSleeping = sleeping;

        // Dừng animation khi bắt đầu ngủ
        if (isSleeping && animator != null)
        {
            animator.SetFloat(speedHash, 0f);
        }
    }

    /// <summary>
    /// Khóa/mở khóa movement từ bên ngoài (VD: bị vây quanh, cutscene)
    /// </summary>
    public void SetMovementEnabled(bool enabled)
    {
        isMovementLocked = !enabled;
        Debug.Log($"[PlayerMovement] Movement locked: {isMovementLocked}");

        // Dừng animation và velocity ngay lập tức
        if (isMovementLocked)
        {
            movementInput = Vector2.zero;
            if (rb != null) rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetFloat(speedHash, 0f);
        }
    }

    /// <summary>
    /// Kiểm tra player có thể di chuyển không
    /// </summary>
    public bool CanMove()
    {
        return !isSitting && !isTalking && !isSleeping && !isMovementLocked;
    }
}
