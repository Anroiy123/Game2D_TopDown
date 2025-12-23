using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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

    [Header("Avatar Mode (Top-Down with Avatar)")]
    [Tooltip("Bật chế độ hiển thị avatar trong dialogue (background = scene top-down)")]
    [SerializeField] private bool useAvatarMode = false;
    [Tooltip("Avatar sprites cho các speaker (key = speaker name trong DialogueData)")]
    [SerializeField] private AvatarEntry[] avatarEntries;

    [System.Serializable]
    public class AvatarEntry
    {
        [Tooltip("Tên speaker (phải khớp với speakerName trong DialogueNode)")]
        public string speakerName;
        [Tooltip("Avatar sprite cho speaker này")]
        public Sprite avatarSprite;
        [Tooltip("Flip avatar theo chiều ngang (để nhân vật quay vào trong màn hình)")]
        public bool flipHorizontal = true;
    }

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
    
    // Track conditional dialogue đang được sử dụng để apply effects khi kết thúc
    private ConditionalDialogueEntry currentConditionalDialogue = null;
    
    // Track đã auto trigger chưa (tránh trigger nhiều lần)
    private bool hasAutoTriggered = false;
    
    // Track các flag đã trigger (tránh trigger nhiều lần cho cùng flag)
    private System.Collections.Generic.HashSet<string> triggeredFlags = new System.Collections.Generic.HashSet<string>();

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
        
        // Subscribe vào StoryManager.OnFlagChanged để trigger dialogue khi flag được set
        if (StoryManager.Instance != null)
        {
            StoryManager.Instance.OnFlagChanged += OnFlagChanged;
        }
        
        // Kiểm tra và trigger auto dialogue nếu có
        CheckAutoTriggerDialogue();
    }
    
    private void OnDestroy()
    {
        // Unsubscribe khi NPC bị destroy
        if (StoryManager.Instance != null)
        {
            StoryManager.Instance.OnFlagChanged -= OnFlagChanged;
        }
    }
    
    /// <summary>
    /// Callback khi một flag được thay đổi trong StoryManager
    /// </summary>
    private void OnFlagChanged(string flagName, bool value)
    {
        // Chỉ xử lý khi flag được set TRUE
        if (!value) return;
        
        // Kiểm tra xem đã trigger cho flag này chưa
        if (triggeredFlags.Contains(flagName)) return;
        
        // Tìm conditional dialogue có triggerOnFlagSet khớp với flag vừa được set
        if (conditionalDialogues == null || conditionalDialogues.Length == 0) return;
        
        foreach (var entry in conditionalDialogues)
        {
            if (!string.IsNullOrEmpty(entry.triggerOnFlagSet) && 
                string.Equals(entry.triggerOnFlagSet, flagName, System.StringComparison.OrdinalIgnoreCase) &&
                entry.dialogueData != null && 
                entry.CanUse())
            {
                Debug.Log($"[NPCInteraction] {npcName}: Flag '{flagName}' set, triggering dialogue '{entry.dialogueData.conversationName}'");
                
                // Đánh dấu đã trigger cho flag này
                triggeredFlags.Add(flagName);
                
                // Trigger sau delay
                if (entry.autoTriggerDelay > 0)
                {
                    StartCoroutine(FlagTriggerDialogueCoroutine(entry, flagName));
                }
                else
                {
                    TriggerAutoDialogue(entry);
                }
                break;
            }
        }
    }
    
    private System.Collections.IEnumerator FlagTriggerDialogueCoroutine(ConditionalDialogueEntry entry, string flagName)
    {
        yield return new WaitForSeconds(entry.autoTriggerDelay);
        
        // Kiểm tra lại điều kiện sau delay
        if (entry.CanUse() && !isTalking)
        {
            TriggerAutoDialogue(entry);
        }
        else
        {
            // Nếu không trigger được, cho phép trigger lại sau
            triggeredFlags.Remove(flagName);
        }
    }
    
    /// <summary>
    /// Kiểm tra và trigger dialogue tự động khi scene start
    /// </summary>
    private void CheckAutoTriggerDialogue()
    {
        if (hasAutoTriggered || conditionalDialogues == null || conditionalDialogues.Length == 0)
            return;
            
        // Tìm conditional dialogue có triggerOnSceneStart = true và thỏa mãn điều kiện
        foreach (var entry in conditionalDialogues)
        {
            if (entry.triggerOnSceneStart && entry.dialogueData != null && entry.CanUse())
            {
                // Kiểm tra spawn point nếu có yêu cầu
                if (!string.IsNullOrEmpty(entry.requiredSpawnPointId))
                {
                    string currentSpawnId = GameManager.Instance?.GetCurrentSpawnPointId() ?? "";
                    if (!string.Equals(currentSpawnId, entry.requiredSpawnPointId, System.StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Log($"[NPCInteraction] {npcName}: Auto trigger skipped - spawn point mismatch. Required: '{entry.requiredSpawnPointId}', Current: '{currentSpawnId}'");
                        continue;
                    }
                }
                
                Debug.Log($"[NPCInteraction] {npcName}: Found auto trigger dialogue '{entry.dialogueData.conversationName}', delay: {entry.autoTriggerDelay}s");
                hasAutoTriggered = true;
                
                // Trigger sau delay
                if (entry.autoTriggerDelay > 0)
                {
                    StartCoroutine(AutoTriggerDialogueCoroutine(entry));
                }
                else
                {
                    TriggerAutoDialogue(entry);
                }
                break;
            }
        }
    }
    
    private System.Collections.IEnumerator AutoTriggerDialogueCoroutine(ConditionalDialogueEntry entry)
    {
        yield return new WaitForSeconds(entry.autoTriggerDelay);
        
        // Kiểm tra lại điều kiện sau delay (có thể đã thay đổi)
        if (entry.CanUse() && !isTalking)
        {
            TriggerAutoDialogue(entry);
        }
    }
    
    /// <summary>
    /// Trigger dialogue tự động (không cần player đến gần)
    /// </summary>
    private void TriggerAutoDialogue(ConditionalDialogueEntry entry)
    {
        TriggerAutoDialogue(entry, null);
    }

    /// <summary>
    /// Trigger dialogue tự động với callback tùy chọn
    /// </summary>
    private void TriggerAutoDialogue(ConditionalDialogueEntry entry, System.Action externalCallback)
    {
        if (dialogueSystem == null || entry.dialogueData == null) return;
        
        Debug.Log($"[NPCInteraction] {npcName}: Auto triggering dialogue '{entry.dialogueData.conversationName}'");
        
        isTalking = true;
        currentConditionalDialogue = entry;
        externalDialogueCallback = externalCallback;
        
        // Ẩn UI tên
        if (nameUI != null)
        {
            nameUI.SetActive(false);
        }
        
        // Khóa player movement
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.SetTalkingState(true);
            }
        }
        
        // Bắt đầu dialogue
        dialogueSystem.StartDialogueWithChoices(entry.dialogueData, OnDialogueEnd, OnDialogueAction, OnVNSceneTransition);
    }

    // Callback từ bên ngoài (NPCSurroundPlayer)
    private System.Action externalDialogueCallback = null;

    /// <summary>
    /// Trigger dialogue từ bên ngoài (VD: NPCSurroundPlayer gọi sau khi surround xong)
    /// Sẽ tìm conditional dialogue phù hợp với flags hiện tại
    /// </summary>
    /// <param name="onComplete">Callback khi dialogue kết thúc</param>
    public void TriggerDialogueFromExternal(System.Action onComplete = null)
    {
        if (dialogueSystem == null)
        {
            Debug.LogError($"[NPCInteraction] {npcName}: DialogueSystem not found!");
            onComplete?.Invoke();
            return;
        }

        if (isTalking)
        {
            Debug.LogWarning($"[NPCInteraction] {npcName}: Already talking, skipping external trigger");
            onComplete?.Invoke();
            return;
        }

        // Tìm dialogue phù hợp
        DialogueData activeDialogue = GetActiveDialogue();
        
        if (activeDialogue == null)
        {
            Debug.LogWarning($"[NPCInteraction] {npcName}: No matching dialogue found for external trigger");
            onComplete?.Invoke();
            return;
        }

        Debug.Log($"[NPCInteraction] {npcName}: External trigger dialogue '{activeDialogue.conversationName}'");
        
        isTalking = true;
        externalDialogueCallback = onComplete;
        
        // Ẩn UI tên
        if (nameUI != null)
        {
            nameUI.SetActive(false);
        }
        
        // Player đã bị khóa bởi NPCSurroundPlayer, không cần khóa lại
        
        // Bắt đầu dialogue
        if (useAvatarMode)
        {
            var (avatarMap, flipMap) = BuildAvatarMaps();
            dialogueSystem.StartDialogueWithAvatars(activeDialogue, avatarMap, flipMap, false, OnDialogueEnd, OnDialogueAction, OnVNSceneTransition);
        }
        else
        {
            dialogueSystem.StartDialogueWithChoices(activeDialogue, OnDialogueEnd, OnDialogueAction, OnVNSceneTransition);
        }
    }

    private void Update()
    {
        if (player == null) return;

        // QUAN TRỌNG: Không xử lý interaction khi VN mode đang active
        // Tránh conflict giữa VN dialogue và top-down NPC interaction
        if (VisualNovelManager.Instance != null && VisualNovelManager.Instance.IsVNModeActive)
        {
            // Ẩn UI tên khi VN mode active
            if (nameUI != null && nameUI.activeSelf)
            {
                nameUI.SetActive(false);
            }
            return;
        }

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

        // Thông báo cho player dừng di chuyển (nếu không dùng avatar mode)
        // Avatar mode sẽ tự lock player
        if (!useAvatarMode)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.SetTalkingState(true);
            }
        }

        // Bắt đầu hội thoại - chọn mode phù hợp
        if (useAdvancedDialogue)
        {
            // Tìm dialogue phù hợp với điều kiện hiện tại
            DialogueData activeDialogue = GetActiveDialogue();

            if (activeDialogue != null)
            {
                // Kiểm tra có dùng avatar mode không
                if (useAvatarMode)
                {
                    // Tạo avatar map và flip map từ avatarEntries
                    var (avatarMap, flipMap) = BuildAvatarMaps();
                    dialogueSystem.StartDialogueWithAvatars(activeDialogue, avatarMap, flipMap, true, OnDialogueEnd, OnDialogueAction, OnVNSceneTransition);
                }
                else
                {
                    // Sử dụng dialogue với choices (không có avatar)
                    dialogueSystem.StartDialogueWithChoices(activeDialogue, OnDialogueEnd, OnDialogueAction, OnVNSceneTransition);
                }
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
    /// Tạo Dictionary avatar và flip map từ avatarEntries
    /// </summary>
    private (Dictionary<string, Sprite> avatarMap, Dictionary<string, bool> flipMap) BuildAvatarMaps()
    {
        Dictionary<string, Sprite> avatarMap = new Dictionary<string, Sprite>();
        Dictionary<string, bool> flipMap = new Dictionary<string, bool>();
        
        if (avatarEntries != null)
        {
            foreach (var entry in avatarEntries)
            {
                if (!string.IsNullOrEmpty(entry.speakerName) && entry.avatarSprite != null)
                {
                    avatarMap[entry.speakerName] = entry.avatarSprite;
                    flipMap[entry.speakerName] = entry.flipHorizontal;
                }
            }
        }
        
        Debug.Log($"[NPCInteraction] {npcName}: Built avatar map with {avatarMap.Count} entries");
        return (avatarMap, flipMap);
    }

    /// <summary>
    /// Lấy DialogueData phù hợp với điều kiện hiện tại
    /// Ưu tiên: conditionalDialogues (theo priority) > dialogueData mặc định
    /// </summary>
    private DialogueData GetActiveDialogue()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log($"[NPCInteraction] {npcName}: GetActiveDialogue() called in scene '{currentScene}'");

        // Reset current conditional dialogue
        currentConditionalDialogue = null;

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
                    // Lưu lại entry để apply effects khi dialogue kết thúc
                    currentConditionalDialogue = entry;
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
            case "trigger_beat_cutscene":
                // Trigger cutscene bullies đánh player
                TriggerBeatCutscene();
                break;
            case "trigger_fight_cutscene":
                // Trigger cutscene đánh nhau 1v1 (Scene 27 - Đức vs Thủ lĩnh)
                TriggerFightCutscene();
                break;
            
            // Storytelling Endings
            case "trigger_ending1_storytelling":
            case "trigger_ending2_storytelling":
            case "trigger_ending3_storytelling":
                // Delegate to VisualNovelManager which has the storytelling references
                if (VisualNovelManager.Instance != null)
                {
                    // Use reflection or direct method call
                    TriggerStorytellingEnding(actionId);
                }
                else
                {
                    Debug.LogError($"[NPCInteraction] Cannot trigger {actionId} - VisualNovelManager not found!");
                }
                break;
                
            default:
                Debug.Log($"Unknown action: {actionId}");
                break;
        }
    }

    /// <summary>
    /// Callback khi dialogue node yêu cầu chuyển sang VN scene
    /// </summary>
    private void OnVNSceneTransition(VNSceneData nextVNScene)
    {
        if (nextVNScene == null)
        {
            Debug.LogError("[NPCInteraction] OnVNSceneTransition: nextVNScene is null!");
            return;
        }

        Debug.Log($"[NPCInteraction] Transitioning to VN scene: {nextVNScene.name}");

        // Gọi VisualNovelManager để chuyển scene
        if (VisualNovelManager.Instance != null)
        {
            VisualNovelManager.Instance.StartVNScene(nextVNScene, null);
        }
        else
        {
            Debug.LogError("[NPCInteraction] VisualNovelManager.Instance is null!");
        }
    }

    /// <summary>
    /// Trigger cutscene bullies đánh player
    /// Tìm BullyBeatCutscene trong scene và chạy
    /// </summary>
    private void TriggerBeatCutscene()
    {
        BullyBeatCutscene cutscene = FindFirstObjectByType<BullyBeatCutscene>();
        if (cutscene != null)
        {
            Debug.Log($"[NPCInteraction] {npcName}: Triggering BullyBeatCutscene");
            cutscene.StartBeatCutscene();
        }
        else
        {
            Debug.LogWarning($"[NPCInteraction] {npcName}: BullyBeatCutscene not found in scene!");
        }
    }

    /// <summary>
    /// Trigger cutscene đánh nhau 1v1 (Scene 27)
    /// Tìm FightCutscene trong scene và chạy
    /// </summary>
    private void TriggerFightCutscene()
    {
        FightCutscene cutscene = FindFirstObjectByType<FightCutscene>();
        if (cutscene != null)
        {
            Debug.Log($"[NPCInteraction] {npcName}: Triggering FightCutscene");
            cutscene.StartFightCutscene();
        }
        else
        {
            Debug.LogWarning($"[NPCInteraction] {npcName}: FightCutscene not found in scene!");
        }
    }

    /// <summary>
    /// Trigger Storytelling Ending thông qua VisualNovelManager
    /// </summary>
    private void TriggerStorytellingEnding(string actionId)
    {
        Debug.Log($"[NPCInteraction] {npcName}: Triggering storytelling ending: {actionId}");
        
        // Get the storytelling data from VisualNovelManager and play it
        StorytellingSequenceData sequenceData = null;
        
        switch (actionId)
        {
            case "trigger_ending1_storytelling":
                sequenceData = VisualNovelManager.Instance.GetEnding1Data();
                break;
            case "trigger_ending2_storytelling":
                sequenceData = VisualNovelManager.Instance.GetEnding2Data();
                break;
            case "trigger_ending3_storytelling":
                sequenceData = VisualNovelManager.Instance.GetEnding3Data();
                break;
        }
        
        if (sequenceData != null && StorytellingManager.Instance != null)
        {
            StorytellingManager.Instance.PlaySequence(sequenceData);
        }
        else
        {
            Debug.LogError($"[NPCInteraction] Cannot play storytelling - sequenceData={sequenceData != null}, StorytellingManager={StorytellingManager.Instance != null}");
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

        // Apply effects từ conditional dialogue (nếu có)
        if (currentConditionalDialogue != null)
        {
            currentConditionalDialogue.ApplyOnCompleteEffects();
            currentConditionalDialogue = null;
        }

        // Cho phép player di chuyển lại (nếu không dùng avatar mode VÀ không có external callback)
        // Avatar mode sẽ tự unlock player trong DialogueSystem.EndDialogue()
        // External callback (từ NPCSurroundPlayer) sẽ tự unlock player
        if (!useAvatarMode && externalDialogueCallback == null && player != null)
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

        // Gọi external callback nếu có (từ NPCSurroundPlayer)
        if (externalDialogueCallback != null)
        {
            var callback = externalDialogueCallback;
            externalDialogueCallback = null;
            callback.Invoke();
        }
    }

    // Vẽ gizmo để dễ debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
