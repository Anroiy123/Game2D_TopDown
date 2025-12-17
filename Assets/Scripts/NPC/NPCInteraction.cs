using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] private string npcName = "Adam";
    
    [Header("Legacy Dialogue (Simple Mode)")]
    [SerializeField] private string[] dialogueLines = new string[]
    {
        "Xin chào! Tôi là Adam.",
        "Chào mừng bạn đến lớp học!",
        "Bạn cần giúp gì không?"
    };

    [Header("Advanced Dialogue (With Choices)")]
    [Tooltip("DialogueData mặc định (dùng khi không có conditional dialogue nào thỏa mãn)")]
    [SerializeField] private DialogueData dialogueData; // ScriptableObject cho dialogue phức tạp
    [SerializeField] private bool useAdvancedDialogue = false; // Sử dụng dialogue với choices

    [Header("Conditional Dialogues")]
    [Tooltip("Danh sách dialogue có điều kiện (ưu tiên cao hơn dialogueData mặc định)")]
    [SerializeField] private ConditionalDialogueEntry[] conditionalDialogues;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Header("Default Direction")]
    [SerializeField] private bool useCustomDefaultDirection = false;
    [SerializeField] private Vector2 defaultDirection = new Vector2(0f, -1f); // Mặc định nhìn xuống

    [Header("UI References")]
    [SerializeField] private GameObject nameUI; // Canvas hiển thị tên
    [SerializeField] private Text nameText; // Text component

    [Header("Interaction Indicator")]
    [Tooltip("Component hiển thị icon animation phía trên NPC")]
    [SerializeField] private InteractionIndicator interactionIndicator;

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
        dialogueSystem = FindFirstObjectByType<DialogueSystem>();

        // Setup UI tên
        if (nameUI != null)
        {
            nameUI.SetActive(false);
        }

        if (nameText != null)
        {
            nameText.text = npcName;
        }

        // Chỉ set hướng mặc định nếu KHÔNG có NPCDefaultDirection component
        // (để NPCDefaultDirection có thể control hướng)
        NPCDefaultDirection directionController = GetComponent<NPCDefaultDirection>();
        if (directionController == null && animator != null)
        {
            // Không có NPCDefaultDirection, dùng config của NPCInteraction
            if (useCustomDefaultDirection)
            {
                animator.SetFloat(horizontalHash, defaultDirection.x);
                animator.SetFloat(verticalHash, defaultDirection.y);
            }
            else
            {
                // Mặc định hướng lên (lưng) cho backward compatibility
                animator.SetFloat(horizontalHash, 0f);
                animator.SetFloat(verticalHash, 1f);
            }
            animator.SetFloat(speedHash, 0f);
        }
        // InteractionIndicator sẽ tự quản lý visibility dựa trên khoảng cách player
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

        // Bắt đầu hội thoại - chọn mode phù hợp
        if (useAdvancedDialogue)
        {
            // Tìm dialogue phù hợp với điều kiện hiện tại
            DialogueData activeDialogue = GetActiveDialogue();

            if (activeDialogue != null)
            {
                // Sử dụng dialogue với choices
                dialogueSystem.StartDialogueWithChoices(activeDialogue, OnDialogueEnd, OnDialogueAction);
            }
            else
            {
                // Không có dialogue phù hợp, dùng legacy mode
                Debug.LogWarning($"[NPCInteraction] {npcName}: No matching dialogue found, using legacy mode");
                dialogueSystem.StartDialogue(npcName, dialogueLines, OnDialogueEnd);
            }
        }
        else
        {
            // Sử dụng dialogue đơn giản (legacy)
            dialogueSystem.StartDialogue(npcName, dialogueLines, OnDialogueEnd);
        }
    }

    /// <summary>
    /// Lấy DialogueData phù hợp với điều kiện hiện tại
    /// Ưu tiên: conditionalDialogues (theo priority) > dialogueData mặc định
    /// </summary>
    private DialogueData GetActiveDialogue()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log($"[NPCInteraction] {npcName}: GetActiveDialogue() called in scene '{currentScene}'");

        // Kiểm tra conditional dialogues trước (sắp xếp theo priority giảm dần)
        if (conditionalDialogues != null && conditionalDialogues.Length > 0)
        {
            Debug.Log($"[NPCInteraction] {npcName}: Checking {conditionalDialogues.Length} conditional dialogue(s)...");

            // Sắp xếp theo priority (cao hơn = ưu tiên hơn)
            var sortedEntries = new System.Collections.Generic.List<ConditionalDialogueEntry>(conditionalDialogues);
            sortedEntries.Sort((a, b) => b.priority.CompareTo(a.priority));

            foreach (var entry in sortedEntries)
            {
                if (entry.dialogueData != null && entry.CanUse())
                {
                    Debug.Log($"[NPCInteraction] {npcName}: ✓ Using conditional dialogue '{entry.dialogueData.conversationName}' (priority: {entry.priority})");
                    return entry.dialogueData;
                }
            }

            Debug.Log($"[NPCInteraction] {npcName}: No conditional dialogue matched, falling back to default");
        }
        else
        {
            Debug.Log($"[NPCInteraction] {npcName}: No conditional dialogues configured");
        }

        // Fallback về dialogueData mặc định
        if (dialogueData != null)
        {
            Debug.Log($"[NPCInteraction] {npcName}: Using default dialogue '{dialogueData.conversationName}'");
        }
        else
        {
            Debug.LogWarning($"[NPCInteraction] {npcName}: No default dialogue set!");
        }
        return dialogueData;
    }

    /// <summary>
    /// Callback khi player chọn một action đặc biệt trong dialogue
    /// </summary>
    private void OnDialogueAction(string actionId)
    {
        Debug.Log($"NPC {npcName} received action: {actionId}");
        
        // Xử lý các action khác nhau
        switch (actionId)
        {
            case "give_item":
                // Ví dụ: cho item
                Debug.Log("Action: Give item to player");
                break;
            case "open_shop":
                // Ví dụ: mở shop
                Debug.Log("Action: Open shop");
                break;
            case "start_quest":
                // Ví dụ: bắt đầu quest
                Debug.Log("Action: Start quest");
                break;
            default:
                Debug.Log($"Unknown action: {actionId}");
                break;
        }
    }

    private void FacePlayer()
    {
        if (player == null || animator == null) return;

        // Tính vector hướng từ NPC đến Player
        Vector2 direction = (player.position - transform.position).normalized;
        Debug.Log($"[NPC] FacePlayer: direction = ({direction.x:F2}, {direction.y:F2})");

        // Xác định hướng chính (trái/phải/lên/xuống)
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        if (absX > absY)
        {
            // Quay ngang (trái hoặc phải)
            // Blend Tree dùng CÙNG animation NPC_Idle_Side cho cả (-1,0) và (1,0)
            // Animation gốc nhìn sang TRÁI, nên cần flip khi player ở bên PHẢI
            animator.SetFloat(horizontalHash, 1f); // Luôn dùng side animation
            animator.SetFloat(verticalHash, 0f);
            
            if (spriteRenderer != null)
            {
                // NPC_Idle_Side animation nhìn sang PHẢI (mặc định)
                // Nếu player ở bên TRÁI của NPC -> cần flip để NPC nhìn sang trái
                bool playerOnLeft = direction.x < 0;
                spriteRenderer.flipX = playerOnLeft;
                
                Debug.Log($"[NPC] Quay {(playerOnLeft ? "TRÁI (flip)" : "PHẢI")}, flipX = {spriteRenderer.flipX}");
            }
        }
        else
        {
            // Quay dọc (lên hoặc xuống)
            // NPC_Idle ở (0, -1) = nhìn xuống, NPC_Idle_Up ở (0, 1) = nhìn lên
            float verticalValue = direction.y > 0 ? 1f : -1f;
            
            animator.SetFloat(horizontalHash, 0f);
            animator.SetFloat(verticalHash, verticalValue);
            
            // Reset flip khi quay dọc
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = false;
            }
            
            Debug.Log($"[NPC] Quay {(direction.y > 0 ? "LÊN" : "XUỐNG")}, Vertical = {verticalValue}");
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

        // Quay về hướng ban đầu
        NPCDefaultDirection directionController = GetComponent<NPCDefaultDirection>();
        if (directionController != null)
        {
            // Có NPCDefaultDirection, để nó xử lý
            directionController.SetDirection(directionController.GetDefaultDirection());
        }
        else if (animator != null)
        {
            // Không có NPCDefaultDirection, dùng config của NPCInteraction
            if (useCustomDefaultDirection)
            {
                animator.SetFloat(horizontalHash, defaultDirection.x);
                animator.SetFloat(verticalHash, defaultDirection.y);
            }
            else
            {
                // Mặc định hướng lên (lưng)
                animator.SetFloat(horizontalHash, 0f);
                animator.SetFloat(verticalHash, 1f);
            }
            animator.SetFloat(speedHash, 0f);
        }

        // Reset flip
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = false;
        }

        // Ẩn indicator sau khi tương tác (nếu hideAfterInteraction = true)
        if (interactionIndicator != null)
        {
            interactionIndicator.OnInteracted();
        }
    }

    // Vẽ gizmo để dễ debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
