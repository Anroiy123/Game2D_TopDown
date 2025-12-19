using UnityEngine;

/// <summary>
/// NPCFollowPlayer - AI đơn giản để NPC tự động đi theo player
/// Dùng cho tụi bắt nạt đi theo player trong Scene 5
/// </summary>
[RequireComponent(typeof(Animator))]
public class NPCFollowPlayer : MonoBehaviour
{
    [Header("Follow Settings")]
    [Tooltip("Khoảng cách tối thiểu giữa NPC và player (không đến gần hơn)")]
    [SerializeField] private float minDistance = 2f;

    [Tooltip("Khoảng cách tối đa - nếu player xa hơn thì NPC sẽ chạy nhanh hơn")]
    [SerializeField] private float maxDistance = 5f;

    [Tooltip("Tốc độ di chuyển bình thường")]
    [SerializeField] private float moveSpeed = 2f;

    [Tooltip("Tốc độ khi player quá xa (chạy nhanh)")]
    [SerializeField] private float runSpeed = 4f;

    [Tooltip("Có đang follow player không")]
    [SerializeField] private bool isFollowing = false;

    [Header("Formation Offset")]
    [Tooltip("Offset vị trí so với player (để NPCs không dính vào nhau)")]
    [SerializeField] private Vector2 followOffset = Vector2.zero;

    [Header("Animation")]
    [Tooltip("Có sử dụng flipX cho hướng trái/phải không")]
    [SerializeField] private bool useFlipX = true;

    [Header("Debug")]
    [Tooltip("Hiển thị debug logs")]
    [SerializeField] private bool showDebugLogs = false;

    private Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    // Animator parameter hashes
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int horizontalHash = Animator.StringToHash("Horizontal");
    private readonly int verticalHash = Animator.StringToHash("Vertical");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // FIX: Set Animator Culling Mode to Always Animate
        // Để animator vẫn chạy khi NPC ở xa camera
        if (animator != null)
        {
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            Debug.Log($"[NPCFollowPlayer] {gameObject.name}: Set Animator Culling Mode to AlwaysAnimate");
        }
    }

    private void Start()
    {
        // Tìm player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning($"[NPCFollowPlayer] {gameObject.name} - Không tìm thấy Player!");
        }
    }

    private void Update()
    {
        if (!isFollowing || player == null) return;

        // QUAN TRỌNG: Dừng follow khi VN mode đang active
        // Tránh NPC đẩy player đi trong khi đang xem VN scene
        if (VisualNovelManager.Instance != null && VisualNovelManager.Instance.IsVNModeActive)
        {
            StopMoving();
            return;
        }

        FollowPlayer();
    }

    private void FollowPlayer()
    {
        // Tính vị trí target (player + offset)
        Vector2 targetPosition = (Vector2)player.position + followOffset;

        // Tính khoảng cách đến target
        float distance = Vector2.Distance(transform.position, targetPosition);

        // Debug log
        if (showDebugLogs && Time.frameCount % 60 == 0) // Log mỗi 60 frames
        {
            Debug.Log($"[NPCFollowPlayer] {gameObject.name}: distance={distance:F2}, minDist={minDistance}, maxDist={maxDistance}");
        }

        // Nếu quá gần thì dừng lại
        if (distance <= minDistance)
        {
            if (showDebugLogs && Time.frameCount % 60 == 0)
            {
                Debug.Log($"[NPCFollowPlayer] {gameObject.name}: TOO CLOSE - Stopping (distance={distance:F2} <= {minDistance})");
            }
            StopMoving();
            return;
        }

        // Tính hướng đến target
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // Chọn tốc độ dựa trên khoảng cách
        float currentSpeed = distance > maxDistance ? runSpeed : moveSpeed;

        if (showDebugLogs && Time.frameCount % 60 == 0)
        {
            Debug.Log($"[NPCFollowPlayer] {gameObject.name}: MOVING - speed={currentSpeed}, direction=({direction.x:F2}, {direction.y:F2})");
        }

        // Di chuyển
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            currentSpeed * Time.deltaTime
        );

        // Cập nhật animation - LUÔN update khi đang di chuyển
        UpdateAnimation(direction, currentSpeed);
    }

    private void UpdateAnimation(Vector2 direction, float speed)
    {
        if (animator == null) return;

        // Set speed - Chỉ cần > 0.01 để trigger Walk state
        // Dùng 1.0 khi đang di chuyển để đảm bảo trigger transition
        animator.SetFloat(speedHash, 1f);

        if (showDebugLogs && Time.frameCount % 120 == 0) // Log mỗi 120 frames
        {
            // Debug: Kiểm tra animator state hiện tại
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            string currentState = stateInfo.IsName("Walk Blend Tree") ? "Walk" :
                                  stateInfo.IsName("Idle blend tree") ? "Idle" : "Unknown";

            Debug.Log($"[NPCFollowPlayer] {gameObject.name}: UpdateAnimation - Speed=1.0, CurrentState={currentState}, " +
                      $"IsInTransition={animator.IsInTransition(0)}");
        }

        // Xác định hướng chính (ngang hoặc dọc)
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        if (absX > absY)
        {
            // Di chuyển ngang (trái/phải)
            // Set Horizontal = 1 hoặc -1 để Blend Tree chọn đúng animation
            animator.SetFloat(horizontalHash, direction.x > 0 ? 1f : -1f);
            animator.SetFloat(verticalHash, 0f);

            // Flip sprite nếu cần (tùy vào setup của animator)
            if (useFlipX && spriteRenderer != null)
            {
                spriteRenderer.flipX = direction.x < 0; // Flip khi đi sang trái
            }
        }
        else
        {
            // Di chuyển dọc (lên/xuống)
            animator.SetFloat(horizontalHash, 0f);
            animator.SetFloat(verticalHash, direction.y > 0 ? 1f : -1f);

            // Reset flip khi đi dọc
            if (useFlipX && spriteRenderer != null)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    private void StopMoving()
    {
        if (animator == null) return;

        animator.SetFloat(speedHash, 0f);

        if (showDebugLogs && Time.frameCount % 120 == 0)
        {
            Debug.Log($"[NPCFollowPlayer] {gameObject.name}: StopMoving - Speed set to 0");
        }

        // Giữ nguyên hướng hiện tại (không reset horizontal/vertical)
    }

    /// <summary>
    /// Bắt đầu follow player
    /// </summary>
    public void StartFollowing()
    {
        isFollowing = true;

        // FIX: Force animator to update immediately
        if (animator != null)
        {
            // Reset animator để đảm bảo transitions hoạt động
            animator.Update(0f);

            // Force set Speed = 0 rồi = 1 để trigger transition
            animator.SetFloat(speedHash, 0f);
            animator.Update(0f);
            animator.SetFloat(speedHash, 1f);

            Debug.Log($"[NPCFollowPlayer] {gameObject.name} bắt đầu đi theo player - Animator reset");
        }
        else
        {
            Debug.Log($"[NPCFollowPlayer] {gameObject.name} bắt đầu đi theo player");
        }
    }

    /// <summary>
    /// Dừng follow player
    /// </summary>
    public void StopFollowing()
    {
        isFollowing = false;
        StopMoving();
        Debug.Log($"[NPCFollowPlayer] {gameObject.name} dừng đi theo player");
    }

    /// <summary>
    /// Set khoảng cách follow
    /// </summary>
    public void SetFollowDistance(float min, float max)
    {
        minDistance = min;
        maxDistance = max;
    }

    /// <summary>
    /// Set offset vị trí follow (để NPCs không dính vào nhau)
    /// </summary>
    public void SetFollowOffset(Vector2 offset)
    {
        followOffset = offset;
    }

    /// <summary>
    /// Teleport NPC đến vị trí cụ thể (dùng khi spawn)
    /// </summary>
    public void TeleportTo(Vector3 position)
    {
        transform.position = position;
    }
}

