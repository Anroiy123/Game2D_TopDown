using UnityEngine;

/// <summary>
/// BullyEncounterZone - Khu vực trigger gặp nhóm bắt nạt
/// Khi player đi vào zone này, sẽ trigger dialogue với nhóm bully
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BullyEncounterZone : MonoBehaviour
{
    [Header("Encounter Settings")]
    [Tooltip("Chỉ trigger 1 lần duy nhất")]
    [SerializeField] private bool triggerOnce = true;
    
    [Tooltip("Flag sẽ được set khi encounter xảy ra")]
    [SerializeField] private string encounterFlag = "met_bullies";
    
    [Tooltip("Nếu đã có flag này thì không trigger")]
    [SerializeField] private string skipIfFlag = "";

    [Header("NPCs to Activate")]
    [Tooltip("Các NPC bully sẽ được activate khi trigger")]
    [SerializeField] private GameObject[] bullyNPCs;
    
    [Tooltip("NPC chính sẽ bắt đầu dialogue")]
    [SerializeField] private NPCInteraction mainBullyNPC;

    [Header("Dialogue")]
    [Tooltip("Dialogue sẽ tự động bắt đầu khi encounter")]
    [SerializeField] private DialogueData encounterDialogue;
    
    [Tooltip("Tự động start dialogue khi vào zone")]
    [SerializeField] private bool autoStartDialogue = true;

    [Header("Day-based Encounters")]
    [Tooltip("Chỉ trigger vào những ngày cụ thể (0 = mọi ngày)")]
    [SerializeField] private int[] triggerOnDays;
    
    [Tooltip("Số lần đã encounter (đếm tự động)")]
    [SerializeField] private string encounterCountVar = "bully_encounter_count";

    private bool hasTriggered = false;
    private DialogueSystem dialogueSystem;

    private void Start()
    {
        dialogueSystem = FindFirstObjectByType<DialogueSystem>();
        
        // Ẩn bully NPCs ban đầu nếu chưa met
        if (!StoryManager.Instance.GetFlag(encounterFlag))
        {
            SetBullyNPCsActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (hasTriggered && triggerOnce) return;
        
        // Check conditions
        if (!ShouldTrigger()) return;
        
        TriggerEncounter();
    }

    private bool ShouldTrigger()
    {
        if (StoryManager.Instance == null) return true;

        // Skip nếu đã có flag
        if (!string.IsNullOrEmpty(skipIfFlag) && StoryManager.Instance.GetFlag(skipIfFlag))
        {
            return false;
        }

        // Check ngày cụ thể
        if (triggerOnDays != null && triggerOnDays.Length > 0)
        {
            int currentDay = StoryManager.Instance.GetCurrentDay();
            bool dayMatch = false;
            foreach (int day in triggerOnDays)
            {
                if (day == currentDay || day == 0)
                {
                    dayMatch = true;
                    break;
                }
            }
            if (!dayMatch) return false;
        }

        return true;
    }

    private void TriggerEncounter()
    {
        hasTriggered = true;
        
        Debug.Log($"[BullyEncounterZone] Encounter triggered!");
        
        // Set flag
        if (StoryManager.Instance != null && !string.IsNullOrEmpty(encounterFlag))
        {
            StoryManager.Instance.SetFlag(encounterFlag, true);
            
            // Increase encounter count
            if (!string.IsNullOrEmpty(encounterCountVar))
            {
                StoryManager.Instance.AddVariable(encounterCountVar, 1);
            }
        }
        
        // Activate bully NPCs
        SetBullyNPCsActive(true);
        
        // Move bullies to surround player (optional animation)
        // TODO: Implement bully movement/animation
        
        // Start dialogue
        if (autoStartDialogue && encounterDialogue != null && dialogueSystem != null)
        {
            // Delay nhỏ để player thấy bullies xuất hiện
            Invoke(nameof(StartEncounterDialogue), 0.5f);
        }
    }

    private void StartEncounterDialogue()
    {
        if (mainBullyNPC != null)
        {
            // Use NPC's dialogue system
            mainBullyNPC.SendMessage("StartDialogue", SendMessageOptions.DontRequireReceiver);
        }
        else if (encounterDialogue != null && dialogueSystem != null)
        {
            // Direct dialogue start
            dialogueSystem.StartDialogueWithChoices(encounterDialogue, OnEncounterDialogueEnd);
        }
    }

    private void OnEncounterDialogueEnd()
    {
        Debug.Log("[BullyEncounterZone] Encounter dialogue ended");
        // Player có thể tiếp tục di chuyển
    }

    private void SetBullyNPCsActive(bool active)
    {
        if (bullyNPCs == null) return;
        
        foreach (var npc in bullyNPCs)
        {
            if (npc != null)
            {
                npc.SetActive(active);
            }
        }
    }

    #region Editor Visualization
    private void OnDrawGizmos()
    {
        // Vẽ zone màu đỏ
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        
        var collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            Gizmos.DrawCube(transform.position + (Vector3)collider.offset, collider.size);
        }
        else
        {
            Gizmos.DrawCube(transform.position, Vector3.one * 2f);
        }
    }
    #endregion
}

