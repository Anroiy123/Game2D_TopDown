using UnityEngine;

/// <summary>
/// DialogueTrigger - Component để trigger dialogue khi player tương tác
/// Tương tự VNTrigger nhưng dùng cho DialogueData thay vì VNSceneData
/// Dùng cho: Objects không phải NPC (TV, radio, bảng thông báo, etc.)
/// </summary>
public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    [Tooltip("Dialogue sẽ chạy khi trigger")]
    [SerializeField] private DialogueData dialogueData;

    [Header("Conditional Dialogues")]
    [Tooltip("Danh sách dialogue có điều kiện (ưu tiên cao hơn dialogueData mặc định)")]
    [SerializeField] private ConditionalDialogueEntry[] conditionalDialogues;

    [Header("Trigger Settings")]
    [Tooltip("Mode kích hoạt")]
    [SerializeField] private TriggerMode mode = TriggerMode.OnInteract;
    
    [Tooltip("Phím tương tác (chỉ dùng cho OnInteract mode)")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Tooltip("Chỉ trigger một lần")]
    [SerializeField] private bool triggerOnce = true;
    
    [Tooltip("Delay trước khi bắt đầu dialogue (giây)")]
    [SerializeField] private float triggerDelay = 0f;

    [Header("Conditions")]
    [Tooltip("Flags cần có để trigger")]
    [SerializeField] private string[] requiredFlags;

    [Tooltip("Flags không được có")]
    [SerializeField] private string[] forbiddenFlags;

    [Header("Avatar Mode")]
    [Tooltip("Bật chế độ hiển thị avatar trong dialogue")]
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
        [Tooltip("Flip avatar theo chiều ngang")]
        public bool flipHorizontal = true;
    }

    [Header("Effects")]
    [Tooltip("Flags sẽ được set TRUE sau khi dialogue kết thúc")]
    [SerializeField] private string[] setFlagsOnComplete;
    
    [Tooltip("Flags sẽ được set FALSE sau khi dialogue kết thúc")]
    [SerializeField] private string[] setFlagsFalse;
    
    [Tooltip("Thay đổi biến khi dialogue kết thúc")]
    [SerializeField] private VariableChange[] variableChanges;
    
    [Tooltip("Khóa player movement khi dialogue đang chạy")]
    [SerializeField] private bool lockPlayerMovement = true;

    [Header("After Dialogue")]
    [Tooltip("VN Scene chạy sau dialogue (ưu tiên cao nhất)")]
    [SerializeField] private VNSceneData nextVNScene;
    
    [Tooltip("Dialogue tiếp theo sau khi dialogue hiện tại kết thúc")]
    [SerializeField] private DialogueData nextDialogue;
    
    [Tooltip("Chuyển sang Unity scene khác (để trống = ở lại scene hiện tại)")]
    [SerializeField] private string nextSceneName;
    
    [Tooltip("Spawn point ID trong scene đích")]
    [SerializeField] private string nextSpawnPointId;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject interactionPrompt;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    public enum TriggerMode
    {
        OnTriggerEnter,
        OnInteract,
        OnSceneStart
    }

    private bool playerInRange = false;
    private bool hasTriggered = false;
    private DialogueSystem dialogueSystem;
    private PlayerMovement playerMovement;
    private ConditionalDialogueEntry currentConditionalDialogue;

    private void Start()
    {
        dialogueSystem = FindFirstObjectByType<DialogueSystem>();
        
        if (dialogueSystem == null)
        {
            Debug.LogError($"[DialogueTrigger] {gameObject.name}: DialogueSystem not found!");
        }

        // Validate collider requirement
        if (mode == TriggerMode.OnTriggerEnter || mode == TriggerMode.OnInteract)
        {
            var collider = GetComponent<Collider2D>();
            if (collider == null)
            {
                Debug.LogError($"[DialogueTrigger] {gameObject.name}: Mode '{mode}' requires a Collider2D component!", this);
                return;
            }
            collider.isTrigger = true;
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // Auto trigger on scene start
        if (mode == TriggerMode.OnSceneStart)
        {
            if (triggerDelay > 0)
            {
                Invoke(nameof(TryTriggerDialogue), triggerDelay);
            }
            else
            {
                TryTriggerDialogue();
            }
        }
    }

    private void Update()
    {
        // Không trigger khi VN mode đang active
        if (VisualNovelManager.Instance != null && VisualNovelManager.Instance.IsVNModeActive)
        {
            return;
        }
        
        // Không trigger khi dialogue đang active
        if (dialogueSystem != null && dialogueSystem.IsDialogueActive())
        {
            return;
        }

        if (mode == TriggerMode.OnInteract && playerInRange && !hasTriggered)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                TryTriggerDialogue();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: Player entered trigger zone. Mode={mode}");
        
        playerInRange = true;
        playerMovement = other.GetComponent<PlayerMovement>();

        if (mode == TriggerMode.OnTriggerEnter && !hasTriggered)
        {
            if (triggerDelay > 0)
            {
                Invoke(nameof(TryTriggerDialogue), triggerDelay);
            }
            else
            {
                TryTriggerDialogue();
            }
        }
        else if (mode == TriggerMode.OnInteract && interactionPrompt != null && CheckConditions())
        {
            interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void TryTriggerDialogue()
    {
        if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: TryTriggerDialogue() called");

        if (hasTriggered && triggerOnce)
        {
            if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: Already triggered, skipping.");
            return;
        }
        
        if (!CheckConditions()) return;

        // Tìm dialogue phù hợp
        DialogueData activeDialogue = GetActiveDialogue();
        
        if (activeDialogue == null)
        {
            Debug.LogWarning($"[DialogueTrigger] {gameObject.name}: No dialogue data assigned!");
            return;
        }

        if (dialogueSystem == null)
        {
            Debug.LogError($"[DialogueTrigger] {gameObject.name}: DialogueSystem not found!");
            return;
        }

        if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: ✓ Triggering dialogue '{activeDialogue.conversationName}'");
        
        hasTriggered = true;

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // Khóa player movement
        if (lockPlayerMovement && playerMovement != null)
        {
            playerMovement.SetTalkingState(true);
        }

        // Bắt đầu dialogue - chọn mode phù hợp
        if (useAvatarMode && avatarEntries != null && avatarEntries.Length > 0)
        {
            // Tạo avatar map từ avatarEntries
            var avatarMap = new System.Collections.Generic.Dictionary<string, Sprite>();
            var flipMap = new System.Collections.Generic.Dictionary<string, bool>();
            
            foreach (var entry in avatarEntries)
            {
                if (!string.IsNullOrEmpty(entry.speakerName) && entry.avatarSprite != null)
                {
                    avatarMap[entry.speakerName] = entry.avatarSprite;
                    flipMap[entry.speakerName] = entry.flipHorizontal;
                }
            }
            
            if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: Starting with avatar mode, {avatarMap.Count} avatars");
            dialogueSystem.StartDialogueWithAvatars(activeDialogue, avatarMap, flipMap, lockPlayerMovement, OnDialogueComplete, OnDialogueAction);
        }
        else
        {
            // Mode bình thường không có avatar
            dialogueSystem.StartDialogueWithChoices(activeDialogue, OnDialogueComplete, OnDialogueAction);
        }
    }

    /// <summary>
    /// Lấy DialogueData phù hợp với điều kiện hiện tại
    /// </summary>
    private DialogueData GetActiveDialogue()
    {
        currentConditionalDialogue = null;

        // Kiểm tra conditional dialogues trước
        if (conditionalDialogues != null && conditionalDialogues.Length > 0)
        {
            // Sắp xếp theo priority (cao hơn = ưu tiên hơn)
            var sortedEntries = new System.Collections.Generic.List<ConditionalDialogueEntry>(conditionalDialogues);
            sortedEntries.Sort((a, b) => b.priority.CompareTo(a.priority));

            foreach (var entry in sortedEntries)
            {
                if (entry.dialogueData != null && entry.CanUse())
                {
                    if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: Using conditional dialogue '{entry.dialogueData.conversationName}'");
                    currentConditionalDialogue = entry;
                    return entry.dialogueData;
                }
            }
        }

        // Fallback về dialogueData mặc định
        return dialogueData;
    }

    private bool CheckConditions()
    {
        if (StoryManager.Instance == null) return true;

        // Check required flags
        if (requiredFlags != null && requiredFlags.Length > 0)
        {
            foreach (string flag in requiredFlags)
            {
                if (!string.IsNullOrEmpty(flag) && !StoryManager.Instance.GetFlag(flag))
                {
                    if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: Missing required flag '{flag}'");
                    return false;
                }
            }
        }

        // Check forbidden flags
        if (forbiddenFlags != null && forbiddenFlags.Length > 0)
        {
            foreach (string flag in forbiddenFlags)
            {
                if (!string.IsNullOrEmpty(flag) && StoryManager.Instance.GetFlag(flag))
                {
                    if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: Has forbidden flag '{flag}'");
                    return false;
                }
            }
        }

        return true;
    }

    private void OnDialogueAction(string actionId)
    {
        if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: Received action '{actionId}'");
        // Có thể mở rộng để xử lý các action đặc biệt
    }

    private void OnDialogueComplete()
    {
        if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: Dialogue completed");

        // Apply effects từ conditional dialogue
        if (currentConditionalDialogue != null)
        {
            currentConditionalDialogue.ApplyOnCompleteEffects();
            currentConditionalDialogue = null;
        }

        // Set flags on complete
        if (setFlagsOnComplete != null && StoryManager.Instance != null)
        {
            foreach (string flag in setFlagsOnComplete)
            {
                if (!string.IsNullOrEmpty(flag))
                {
                    StoryManager.Instance.SetFlag(flag, true);
                    if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: Set flag '{flag}' = true");
                }
            }
        }
        
        // Set flags FALSE
        if (setFlagsFalse != null && StoryManager.Instance != null)
        {
            foreach (string flag in setFlagsFalse)
            {
                if (!string.IsNullOrEmpty(flag))
                {
                    StoryManager.Instance.SetFlag(flag, false);
                    if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: Set flag '{flag}' = false");
                }
            }
        }
        
        // Apply variable changes
        if (variableChanges != null)
        {
            foreach (var change in variableChanges)
            {
                change.Apply();
                if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: Applied variable change: {change.variableName}");
            }
        }

        // Xử lý After Dialogue - theo thứ tự ưu tiên
        // 1. VN Scene (ưu tiên cao nhất)
        if (nextVNScene != null)
        {
            if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: Starting next VN scene: {nextVNScene.name}");
            VisualNovelManager.Instance.StartVNScene(nextVNScene, () => {
                // Mở khóa player sau khi VN scene kết thúc
                if (lockPlayerMovement && playerMovement != null)
                {
                    playerMovement.SetTalkingState(false);
                }
            });
            return; // Không mở khóa player ở đây, VN scene sẽ xử lý
        }
        
        // 2. Next Dialogue
        if (nextDialogue != null)
        {
            if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: Starting next dialogue: {nextDialogue.conversationName}");
            
            // Bắt đầu dialogue tiếp theo với cùng settings
            if (useAvatarMode && avatarEntries != null && avatarEntries.Length > 0)
            {
                var avatarMap = new System.Collections.Generic.Dictionary<string, Sprite>();
                var flipMap = new System.Collections.Generic.Dictionary<string, bool>();
                
                foreach (var entry in avatarEntries)
                {
                    if (!string.IsNullOrEmpty(entry.speakerName) && entry.avatarSprite != null)
                    {
                        avatarMap[entry.speakerName] = entry.avatarSprite;
                        flipMap[entry.speakerName] = entry.flipHorizontal;
                    }
                }
                
                dialogueSystem.StartDialogueWithAvatars(nextDialogue, avatarMap, flipMap, lockPlayerMovement, OnNextDialogueComplete, OnDialogueAction);
            }
            else
            {
                dialogueSystem.StartDialogueWithChoices(nextDialogue, OnNextDialogueComplete, OnDialogueAction);
            }
            return; // Không mở khóa player ở đây
        }
        
        // 3. Next Scene
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: Loading next scene: {nextSceneName}");
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadScene(nextSceneName, nextSpawnPointId);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
            }
            return; // Scene sẽ thay đổi, không cần mở khóa
        }

        // Mở khóa player movement (nếu không có next action)
        if (lockPlayerMovement && playerMovement != null)
        {
            playerMovement.SetTalkingState(false);
        }
    }
    
    /// <summary>
    /// Callback khi next dialogue kết thúc
    /// </summary>
    private void OnNextDialogueComplete()
    {
        if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: Next dialogue completed");
        
        // Mở khóa player movement
        if (lockPlayerMovement && playerMovement != null)
        {
            playerMovement.SetTalkingState(false);
        }
    }

    /// <summary>
    /// Reset trigger để có thể trigger lại
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        if (showDebugLogs) Debug.Log($"[DialogueTrigger] {gameObject.name}: Trigger reset");
    }

    /// <summary>
    /// Trigger dialogue từ code bên ngoài
    /// </summary>
    public void TriggerFromExternal()
    {
        TryTriggerDialogue();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        
        var collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}
