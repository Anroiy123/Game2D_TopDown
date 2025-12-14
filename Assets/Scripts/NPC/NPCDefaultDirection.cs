using UnityEngine;

/// <summary>
/// Set hướng mặc định cho NPC khi game bắt đầu.
/// Gắn script này vào NPC GameObject có Animator với Blend Tree.
/// </summary>
public class NPCDefaultDirection : MonoBehaviour
{
    public enum Direction
    {
        Down,   // Vertical = -1
        Up,     // Vertical = 1
        Left,   // Horizontal = -1
        Right   // Horizontal = 1
    }

    [Header("Default Direction")]
    [SerializeField] private Direction defaultDirection = Direction.Down;

    /// <summary>
    /// Lấy hướng mặc định đã config
    /// </summary>
    public Direction GetDefaultDirection() => defaultDirection;

    [Header("Optional")]
    [SerializeField] private bool flipSpriteForRight = false; // Flip sprite khi quay phải

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Cache hash để tối ưu performance
    private static readonly int horizontalHash = Animator.StringToHash("Horizontal");
    private static readonly int verticalHash = Animator.StringToHash("Vertical");
    private static readonly int speedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SetDirection(defaultDirection);
    }

    /// <summary>
    /// Set hướng cho NPC
    /// </summary>
    public void SetDirection(Direction direction)
    {
        if (animator == null) return;

        float horizontal = 0f;
        float vertical = 0f;
        bool shouldFlip = false;

        switch (direction)
        {
            case Direction.Down:
                vertical = -1f;
                break;
            case Direction.Up:
                vertical = 1f;
                break;
            case Direction.Left:
                horizontal = -1f;
                break;
            case Direction.Right:
                horizontal = -1f; // Dùng animation left
                shouldFlip = flipSpriteForRight; // Flip sprite nếu cần
                break;
        }

        animator.SetFloat(horizontalHash, horizontal);
        animator.SetFloat(verticalHash, vertical);
        animator.SetFloat(speedHash, 0f); // Đứng yên

        if (spriteRenderer != null && flipSpriteForRight)
        {
            spriteRenderer.flipX = shouldFlip;
        }

        Debug.Log($"[NPCDefaultDirection] {gameObject.name} set to {direction} (H={horizontal}, V={vertical})");
    }

    /// <summary>
    /// Set hướng bằng vector
    /// </summary>
    public void SetDirectionVector(Vector2 dir)
    {
        if (animator == null) return;

        animator.SetFloat(horizontalHash, dir.x);
        animator.SetFloat(verticalHash, dir.y);
        animator.SetFloat(speedHash, 0f);
    }
}
