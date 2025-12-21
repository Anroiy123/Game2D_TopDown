using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Component cho Player tự thoại (inner thoughts)
/// Khác với NPCInteraction: KHÔNG có interaction bằng phím E
/// Chỉ trigger qua: triggerOnSceneStart, triggerOnFlagSet, hoặc TriggerDialogueFromExternal()
/// </summary>
public class PlayerSelfDialogue : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [Tooltip("Tên hiển thị khi player tự thoại (VD: 'Đức' hoặc để trống)")]
    [SerializeField] private string playerName = "Đức";

    [Header("Conditional Dialogues")]
    [Tooltip("Danh sách dialogue có điều kiện")]
    [SerializeField] private ConditionalDialogueEntry[] conditionalDialogues;

    [Header("Components")]
    private DialogueSystem dialogueSystem;
    private PlayerMovement playerMovement;

    private bool isTalking = false;
    private bool hasAutoTriggered = false;
    private HashSet<string> triggeredFlags = new HashSet<string>();
    
    // Track conditional dialogue đang được sử dụng
    private ConditionalDialogueEntry currentConditionalDialogue = null;
    
    // Callback từ bên ngoài
    private System.Action externalDialogueCallback = null;

    private void Start()
    {
        dialogueSystem = FindFirstObjectByType<DialogueSystem>();
        playerMovement = GetComponent<PlayerMovement>();

        // Subscribe vào StoryManager.OnFlagChanged
        if (StoryManager.Instance != null)
        {
            StoryManager.Instance.OnFlagChanged += OnFlagChanged;
        }

        // Kiểm tra auto trigger dialogue (triggerOnSceneStart)
        CheckAutoTriggerDialogue();
        
        // QUAN TRỌNG: Kiểm tra flags đã được set TRƯỚC khi subscribe
        // (Trường hợp QuickFlagSetter.Start() chạy trước PlayerSelfDialogue.Start())
        CheckAlreadySetFlags();
    }
    
    /// <summary>
    /// Kiểm tra các flags đã được set trước khi component này Start()
    /// Giải quyết race condition với QuickFlagSetter
    /// </summary>
    private void CheckAlreadySetFlags()
    {
        if (conditionalDialogues == null || conditionalDialogues.Length == 0) return;
        if (StoryManager.Instance == null) return;
        
        foreach (var entry in conditionalDialogues)
        {
            if (string.IsNullOrEmpty(entry.triggerOnFlagSet)) continue;
            
            string flagName = entry.triggerOnFlagSet;
            bool flagValue = StoryManager.Instance.GetFlag(flagName);
            
            if (flagValue && !triggeredFlags.Contains(flagName) && entry.dialogueData != null && entry.CanUse())
            {
                triggeredFlags.Add(flagName);
                
                if (entry.autoTriggerDelay > 0)
                {
                    StartCoroutine(DelayedTriggerDialogue(entry, flagName));
                }
                else
                {
                    TriggerDialogue(entry);
                }
                break;
            }
        }
    }

    private void OnDestroy()
    {
        if (StoryManager.Instance != null)
        {
            StoryManager.Instance.OnFlagChanged -= OnFlagChanged;
        }
    }

    /// <summary>
    /// Callback khi flag được thay đổi
    /// </summary>
    private void OnFlagChanged(string flagName, bool value)
    {
        if (!value) return;
        if (triggeredFlags.Contains(flagName)) return;
        if (conditionalDialogues == null || conditionalDialogues.Length == 0) return;

        foreach (var entry in conditionalDialogues)
        {
            if (!string.IsNullOrEmpty(entry.triggerOnFlagSet) &&
                string.Equals(entry.triggerOnFlagSet, flagName, System.StringComparison.OrdinalIgnoreCase) &&
                entry.dialogueData != null &&
                entry.CanUse())
            {
                triggeredFlags.Add(flagName);

                if (entry.autoTriggerDelay > 0)
                {
                    StartCoroutine(DelayedTriggerDialogue(entry, flagName));
                }
                else
                {
                    TriggerDialogue(entry);
                }
                break;
            }
        }
    }

    private IEnumerator DelayedTriggerDialogue(ConditionalDialogueEntry entry, string flagName)
    {
        yield return new WaitForSeconds(entry.autoTriggerDelay);

        if (entry.CanUse() && !isTalking)
        {
            TriggerDialogue(entry);
        }
        else
        {
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
                        Debug.Log($"[PlayerSelfDialogue] Auto trigger skipped - spawn point mismatch");
                        continue;
                    }
                }

                Debug.Log($"[PlayerSelfDialogue] Found auto trigger dialogue '{entry.dialogueData.conversationName}'");
                hasAutoTriggered = true;

                if (entry.autoTriggerDelay > 0)
                {
                    StartCoroutine(AutoTriggerCoroutine(entry));
                }
                else
                {
                    TriggerDialogue(entry);
                }
                break;
            }
        }
    }

    private IEnumerator AutoTriggerCoroutine(ConditionalDialogueEntry entry)
    {
        yield return new WaitForSeconds(entry.autoTriggerDelay);

        if (entry.CanUse() && !isTalking)
        {
            TriggerDialogue(entry);
        }
    }

    /// <summary>
    /// Trigger dialogue nội bộ
    /// </summary>
    private void TriggerDialogue(ConditionalDialogueEntry entry)
    {
        if (dialogueSystem == null || entry.dialogueData == null) return;

        isTalking = true;
        currentConditionalDialogue = entry;

        if (playerMovement != null)
        {
            playerMovement.SetTalkingState(true);
        }

        dialogueSystem.StartDialogueWithChoices(entry.dialogueData, OnDialogueEnd, null);
    }

    /// <summary>
    /// Trigger dialogue từ bên ngoài (VD: từ script khác)
    /// </summary>
    public void TriggerDialogueFromExternal(System.Action onComplete = null)
    {
        if (dialogueSystem == null)
        {
            Debug.LogError("[PlayerSelfDialogue] DialogueSystem not found!");
            onComplete?.Invoke();
            return;
        }

        if (isTalking)
        {
            Debug.LogWarning("[PlayerSelfDialogue] Already talking");
            onComplete?.Invoke();
            return;
        }

        // Tìm dialogue phù hợp
        DialogueData activeDialogue = GetActiveDialogue();

        if (activeDialogue == null)
        {
            Debug.LogWarning("[PlayerSelfDialogue] No matching dialogue found");
            onComplete?.Invoke();
            return;
        }

        Debug.Log($"[PlayerSelfDialogue] External trigger dialogue '{activeDialogue.conversationName}'");

        isTalking = true;
        externalDialogueCallback = onComplete;

        // Khóa player movement
        if (playerMovement != null)
        {
            playerMovement.SetTalkingState(true);
        }

        dialogueSystem.StartDialogueWithChoices(activeDialogue, OnDialogueEnd, null);
    }

    /// <summary>
    /// Lấy DialogueData phù hợp với điều kiện hiện tại
    /// </summary>
    private DialogueData GetActiveDialogue()
    {
        currentConditionalDialogue = null;

        if (conditionalDialogues == null || conditionalDialogues.Length == 0)
            return null;

        // Sắp xếp theo priority
        var sortedEntries = new List<ConditionalDialogueEntry>(conditionalDialogues);
        sortedEntries.Sort((a, b) => b.priority.CompareTo(a.priority));

        foreach (var entry in sortedEntries)
        {
            if (entry.dialogueData != null && entry.CanUse())
            {
                currentConditionalDialogue = entry;
                return entry.dialogueData;
            }
        }

        return null;
    }

    private void OnDialogueEnd()
    {
        isTalking = false;

        // Apply effects từ conditional dialogue
        if (currentConditionalDialogue != null)
        {
            currentConditionalDialogue.ApplyOnCompleteEffects();
            currentConditionalDialogue = null;
        }

        // Unlock player (nếu không có external callback)
        if (externalDialogueCallback == null && playerMovement != null)
        {
            playerMovement.SetTalkingState(false);
        }

        // Gọi external callback nếu có
        if (externalDialogueCallback != null)
        {
            var callback = externalDialogueCallback;
            externalDialogueCallback = null;
            callback.Invoke();
        }
    }
}
