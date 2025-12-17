using UnityEngine;
using System;

/// <summary>
/// NPCWaypointWalker - NPC tự động đi theo các waypoints định sẵn
/// Dùng cho Cảnh 11 - Đám bạn đi ra khỏi lớp
/// </summary>
[RequireComponent(typeof(Animator))]
public class NPCWaypointWalker : MonoBehaviour
{
    [Header("Waypoints")]
    [Tooltip("Các điểm đi (theo thứ tự)")]
    [SerializeField] private Transform[] waypoints;

    [Header("Movement")]
    [Tooltip("Tốc độ di chuyển")]
    [SerializeField] private float moveSpeed = 2f;

    [Tooltip("Khoảng cách đến waypoint để coi như đã đến")]
    [SerializeField] private float waypointReachDistance = 0.1f;

    [Header("Behavior")]
    [Tooltip("Tự động bắt đầu đi khi Start()")]
    [SerializeField] private bool autoStart = false;

    [Tooltip("Ẩn NPC khi đi hết waypoints")]
    [SerializeField] private bool hideOnComplete = true;

    [Tooltip("Xóa GameObject khi đi hết waypoints")]
    [SerializeField] private bool destroyOnComplete = false;

    [Header("Animation")]
    [Tooltip("Sử dụng flipX cho hướng trái/phải")]
    [SerializeField] private bool useFlipX = true;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private int currentWaypointIndex = 0;
    private bool isWalking = false;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Animator hashes (matching existing NPC setup)
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int horizontalHash = Animator.StringToHash("Horizontal");
    private readonly int verticalHash = Animator.StringToHash("Vertical");

    // Event khi NPC đi hết waypoints
    public event Action OnReachedDestination;

    public bool IsWalking => isWalking;
    public bool HasReachedDestination => currentWaypointIndex >= waypoints.Length;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator != null)
        {
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }
    }

    private void Start()
    {
        if (autoStart)
        {
            StartWalking();
        }
    }

    private void Update()
    {
        if (!isWalking || waypoints == null || waypoints.Length == 0) return;

        MoveToCurrentWaypoint();
    }

    /// <summary>
    /// Bắt đầu đi theo waypoints
    /// </summary>
    public void StartWalking()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning($"[NPCWaypointWalker] {name}: Không có waypoints!");
            return;
        }

        isWalking = true;
        currentWaypointIndex = 0;

        if (showDebugLogs)
            Debug.Log($"[NPCWaypointWalker] {name} bắt đầu đi ({waypoints.Length} waypoints)");
    }

    /// <summary>
    /// Dừng di chuyển
    /// </summary>
    public void StopWalking()
    {
        isWalking = false;
        SetIdleAnimation();

        if (showDebugLogs)
            Debug.Log($"[NPCWaypointWalker] {name} dừng đi");
    }

    /// <summary>
    /// Set waypoints từ code (runtime)
    /// </summary>
    public void SetWaypoints(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
        currentWaypointIndex = 0;
    }

    private void MoveToCurrentWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Length)
        {
            OnReachedEnd();
            return;
        }

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        if (targetWaypoint == null)
        {
            Debug.LogWarning($"[NPCWaypointWalker] {name}: Waypoint {currentWaypointIndex} is null!");
            currentWaypointIndex++;
            return;
        }

        // Tính hướng và khoảng cách
        Vector2 direction = (targetWaypoint.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetWaypoint.position);

        // Kiểm tra đã đến waypoint chưa
        if (distance <= waypointReachDistance)
        {
            currentWaypointIndex++;
            if (showDebugLogs)
                Debug.Log($"[NPCWaypointWalker] {name} đến waypoint {currentWaypointIndex}/{waypoints.Length}");
            return;
        }

        // Di chuyển
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetWaypoint.position,
            moveSpeed * Time.deltaTime
        );

        // Cập nhật animation
        UpdateWalkAnimation(direction);
    }

    private void UpdateWalkAnimation(Vector2 direction)
    {
        if (animator == null) return;

        animator.SetFloat(speedHash, 1f);

        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        if (absX > absY)
        {
            // Di chuyển ngang
            animator.SetFloat(horizontalHash, direction.x > 0 ? 1f : -1f);
            animator.SetFloat(verticalHash, 0f);

            if (useFlipX && spriteRenderer != null)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }
        else
        {
            // Di chuyển dọc
            animator.SetFloat(horizontalHash, 0f);
            animator.SetFloat(verticalHash, direction.y > 0 ? 1f : -1f);

            if (useFlipX && spriteRenderer != null)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    private void SetIdleAnimation()
    {
        if (animator == null) return;
        animator.SetFloat(speedHash, 0f);
    }

    private void OnReachedEnd()
    {
        isWalking = false;
        SetIdleAnimation();

        if (showDebugLogs)
            Debug.Log($"[NPCWaypointWalker] {name} đã đi hết waypoints");

        OnReachedDestination?.Invoke();

        if (hideOnComplete)
        {
            gameObject.SetActive(false);
        }

        if (destroyOnComplete)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            Gizmos.DrawWireSphere(waypoints[i].position, 0.2f);

            if (i > 0 && waypoints[i - 1] != null)
            {
                Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);
            }
        }

        // Draw line from NPC to first waypoint
        if (waypoints[0] != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, waypoints[0].position);
        }
    }
}

