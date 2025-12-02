using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] private string npcName = "Adam";
    [SerializeField] private string[] dialogueLines = new string[]
    {
        "Xin chào! Tôi là Adam.",
        "Chào mừng bạn đến lớp học!",
        "Bạn cần giúp gì không?"
    };

    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Header("UI References")]
    [SerializeField] private GameObject nameUI; // Canvas hiển thị tên
    [SerializeField] private Text nameText; // Text component

    [Header("Components")]
    private Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private DialogueSystem dialogueSystem;

    private bool playerInRange = false;
    private bool isTalking = false;
    private Vector3 originalScale;

    // Animation parameters
    private readonly int horizontalHash = Animator.StringToHash("Horizontal");
    private readonly int verticalHash = Animator.StringToHash("Vertical");
    private readonly int speedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    private void Start()
    {
        // Tìm player trong scene
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // Tìm DialogueSystem
        dialogueSystem = FindObjectOfType<DialogueSystem>();

        // Setup UI tên
        if (nameUI != null)
        {
            nameUI.SetActive(false);
        }

        if (nameText != null)
        {
            nameText.text = npcName;
        }

        // Set animation về hướng lên (idle - nhìn thấy lưng)
        if (animator != null)
        {
            animator.SetFloat(horizontalHash, 0f);
            animator.SetFloat(verticalHash, 1f); // Hướng lên (lưng)
            animator.SetFloat(speedHash, 0f);
        }
    }

    private void Update()
    {
        if (player == null) return;

        // Kiểm tra khoảng cách đến player
        float distance = Vector2.Distance(transform.position, player.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= interactionRange;

        // Thông báo cho Player biết đang gần NPC (để ưu tiên NPC hơn ghế)
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            if (playerInRange && !wasInRange)
            {
                // Vừa vào vùng NPC
                playerMovement.SetNearNPC(true);
            }
            else if (!playerInRange && wasInRange)
            {
                // Vừa rời khỏi vùng NPC
                playerMovement.SetNearNPC(false);
            }
        }

        // Hiển thị/ẩn UI tên
        if (nameUI != null)
        {
            nameUI.SetActive(playerInRange && !isTalking);
        }

        // Xử lý input tương tác
        if (playerInRange && Input.GetKeyDown(interactionKey) && !isTalking)
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        if (dialogueSystem == null) return;

        isTalking = true;

        // Ẩn UI tên
        if (nameUI != null)
        {
            nameUI.SetActive(false);
        }

        // Quay mặt về phía player
        FacePlayer();

        // Thông báo cho player dừng di chuyển
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.SetTalkingState(true);
        }

        // Bắt đầu hội thoại
        dialogueSystem.StartDialogue(npcName, dialogueLines, OnDialogueEnd);
    }

    private void FacePlayer()
    {
        if (player == null || animator == null) return;

        // Tính vector hướng từ NPC đến Player
        Vector2 direction = (player.position - transform.position).normalized;
        Debug.Log($"FacePlayer: direction = ({direction.x:F2}, {direction.y:F2})");

        // Xác định hướng chính (trái/phải/lên/xuống)
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        if (absX > absY)
        {
            // Quay ngang (trái hoặc phải)
            // Chỉ dùng 1 animation side và flip sprite
            animator.SetFloat(horizontalHash, -1f); // Luôn dùng position (-1, 0) cho side
            animator.SetFloat(verticalHash, 0f);
            
            if (spriteRenderer != null)
            {
                if (direction.x < 0)
                {
                    // Player ở bên TRÁI của Adam
                    Debug.Log("Adam quay TRÁI (Player ở bên trái)");
                    spriteRenderer.flipX = false; // Không flip
                }
                else
                {
                    // Player ở bên PHẢI của Adam
                    Debug.Log("Adam quay PHẢI (Player ở bên phải)");
                    spriteRenderer.flipX = true; // Flip sprite
                }
            }
            
            Debug.Log($"Set Horizontal = {animator.GetFloat(horizontalHash)}, flipX = {spriteRenderer?.flipX}");
        }
        else
        {
            // Quay dọc (lên hoặc xuống)
            animator.SetFloat(horizontalHash, 0f);
            animator.SetFloat(verticalHash, Mathf.Sign(direction.y));
        }

        animator.SetFloat(speedHash, 0f); // Đứng yên
    }

    private void OnDialogueEnd()
    {
        isTalking = false;

        // Cho phép player di chuyển lại
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.SetTalkingState(false);
            }
        }

        // Quay về hướng ban đầu (lên - nhìn thấy lưng)
        if (animator != null)
        {
            animator.SetFloat(horizontalHash, 0f);
            animator.SetFloat(verticalHash, 1f); // Hướng lên (lưng)
            animator.SetFloat(speedHash, 0f);
        }

        // Reset flip
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = false;
        }
    }

    // Vẽ gizmo để dễ debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
