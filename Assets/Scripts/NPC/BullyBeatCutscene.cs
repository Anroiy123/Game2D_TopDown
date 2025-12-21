using UnityEngine;
using System.Collections;

/// <summary>
/// Điều khiển cutscene bullies đánh player trong top-down mode
/// Trigger khi player chọn "không đưa tiền" trong Scene 20
/// </summary>
public class BullyBeatCutscene : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Các NPC bully sẽ đánh player")]
    [SerializeField] private GameObject[] bullies;
    
    [Header("Animation Settings")]
    [Tooltip("Tên trigger parameter trong Animator của Bully")]
    [SerializeField] private string bullyPunchTrigger = "Punch";
    [Tooltip("Tên trigger parameter trong Animator của Player")]
    [SerializeField] private string playerHurtTrigger = "Hurt";
    [Tooltip("Số lần đánh")]
    [SerializeField] private int punchCount = 3;
    [Tooltip("Thời gian giữa mỗi cú đánh (giây)")]
    [SerializeField] private float punchInterval = 0.8f;
    [Tooltip("Delay trước khi bully đánh (để sync animation)")]
    [SerializeField] private float punchDelay = 0.2f;
    
    [Header("Screen Effect")]
    [Tooltip("Fade màn hình đỏ khi bị đánh")]
    [SerializeField] private bool useRedFlash = true;
    [SerializeField] private float flashDuration = 0.1f;
    
    [Header("Trigger Settings")]
    [Tooltip("Chờ dialogue kết thúc hoàn toàn trước khi bắt đầu cutscene")]
    [SerializeField] private bool waitForDialogueEnd = true;
    [Tooltip("Delay sau khi dialogue kết thúc trước khi bắt đầu cutscene")]
    [SerializeField] private float delayAfterDialogue = 0.3f;
    
    [Header("After Cutscene")]
    [Tooltip("VN scene chạy sau khi bị đánh (optional)")]
    [SerializeField] private VNSceneData afterBeatVNScene;
    [Tooltip("Dialogue của NPC sau khi đánh (nếu không dùng VN scene)")]
    [SerializeField] private NPCInteraction afterBeatDialogueNPC;
    [Tooltip("Delay trước khi chạy scene/dialogue tiếp theo")]
    [SerializeField] private float afterBeatDelay = 0.5f;
    
    [Header("Story Effects")]
    [Tooltip("Số fear_level tăng thêm")]
    [SerializeField] private int fearLevelIncrease = 20;
    [Tooltip("Các flags sẽ được set TRUE sau cutscene")]
    [SerializeField] private string[] flagsToSetTrue;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private Transform player;
    private PlayerMovement playerMovement;
    private Animator playerAnimator;
    private bool isPlaying = false;
    
    // Animator hashes
    private int punchHash;
    private int hurtHash;
    private readonly int speedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        punchHash = Animator.StringToHash(bullyPunchTrigger);
        hurtHash = Animator.StringToHash(playerHurtTrigger);
    }

    private void Start()
    {
        FindPlayer();
    }
    
    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerMovement = playerObj.GetComponent<PlayerMovement>();
            playerAnimator = playerObj.GetComponent<Animator>();
        }
    }

    /// <summary>
    /// Bắt đầu cutscene đánh player
    /// Gọi từ DialogueChoice action hoặc NPCSurroundPlayer callback
    /// </summary>
    public void StartBeatCutscene()
    {
        if (isPlaying)
        {
            if (showDebugLogs) Debug.LogWarning("[BullyBeatCutscene] Cutscene đang chạy!");
            return;
        }
        
        // Tìm lại player nếu chưa có
        if (player == null) FindPlayer();
        
        if (player == null)
        {
            Debug.LogError("[BullyBeatCutscene] Không tìm thấy Player!");
            return;
        }
        
        if (bullies == null || bullies.Length == 0)
        {
            Debug.LogError("[BullyBeatCutscene] Không có bully nào được gán!");
            return;
        }
        
        if (waitForDialogueEnd)
        {
            // Chờ dialogue kết thúc trước khi bắt đầu cutscene
            if (showDebugLogs) Debug.Log("[BullyBeatCutscene] Chờ dialogue kết thúc...");
            StartCoroutine(WaitForDialogueEndCoroutine());
        }
        else
        {
            if (showDebugLogs) Debug.Log($"[BullyBeatCutscene] Bắt đầu cutscene với {bullies.Length} bullies, {punchCount} cú đánh");
            StartCoroutine(BeatCutsceneCoroutine());
        }
    }
    
    /// <summary>
    /// Chờ dialogue kết thúc rồi mới bắt đầu cutscene
    /// </summary>
    private IEnumerator WaitForDialogueEndCoroutine()
    {
        // Tìm DialogueSystem
        DialogueSystem dialogueSystem = FindFirstObjectByType<DialogueSystem>();
        
        if (dialogueSystem != null)
        {
            // Chờ cho đến khi dialogue không còn active
            while (dialogueSystem.IsDialogueActive())
            {
                yield return null;
            }
            
            if (showDebugLogs) Debug.Log("[BullyBeatCutscene] Dialogue đã kết thúc");
        }
        
        // Delay thêm sau khi dialogue kết thúc
        if (delayAfterDialogue > 0)
        {
            yield return new WaitForSeconds(delayAfterDialogue);
        }
        
        if (showDebugLogs) Debug.Log($"[BullyBeatCutscene] Bắt đầu cutscene với {bullies.Length} bullies, {punchCount} cú đánh");
        StartCoroutine(BeatCutsceneCoroutine());
    }

    /// <summary>
    /// Bắt đầu cutscene với callback khi hoàn thành
    /// </summary>
    public void StartBeatCutscene(System.Action onComplete)
    {
        externalCallback = onComplete;
        StartBeatCutscene();
    }
    
    private System.Action externalCallback;

    private IEnumerator BeatCutsceneCoroutine()
    {
        isPlaying = true;
        
        // 1. Khóa player movement
        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(false);
            if (showDebugLogs) Debug.Log("[BullyBeatCutscene] Đã khóa player movement");
        }
        
        // 2. Dừng animation di chuyển của player
        if (playerAnimator != null)
        {
            playerAnimator.SetFloat(speedHash, 0f);
        }
        
        // 3. Bullies quay mặt về phía player
        FaceBulliesToPlayer();
        
        yield return new WaitForSeconds(0.3f);
        
        // 4. Chạy animation sequence đánh
        for (int i = 0; i < punchCount; i++)
        {
            if (showDebugLogs) Debug.Log($"[BullyBeatCutscene] Cú đánh {i + 1}/{punchCount}");
            
            // Chọn bully theo thứ tự vòng tròn
            int bullyIndex = i % bullies.Length;
            GameObject bully = bullies[bullyIndex];
            
            if (bully != null)
            {
                Animator bullyAnimator = bully.GetComponent<Animator>();
                
                // Bully đánh
                if (bullyAnimator != null)
                {
                    bullyAnimator.SetTrigger(punchHash);
                    if (showDebugLogs) Debug.Log($"[BullyBeatCutscene] {bully.name} đánh (trigger: {bullyPunchTrigger})");
                }
            }
            
            // Delay nhỏ để sync animation punch với hurt
            yield return new WaitForSeconds(punchDelay);
            
            // Player bị đánh
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(hurtHash);
                if (showDebugLogs) Debug.Log($"[BullyBeatCutscene] Player bị đánh (trigger: {playerHurtTrigger})");
            }
            
            // Flash màn hình đỏ (optional)
            if (useRedFlash)
            {
                yield return StartCoroutine(RedFlashCoroutine());
            }
            
            // Đợi trước cú đánh tiếp theo
            yield return new WaitForSeconds(punchInterval - punchDelay);
        }
        
        // 5. Đợi animation cuối hoàn thành
        yield return new WaitForSeconds(afterBeatDelay);
        
        // 6. Set story flags và variables
        ApplyStoryEffects();
        
        // 7. Chuyển sang scene/dialogue tiếp theo
        if (afterBeatVNScene != null)
        {
            if (showDebugLogs) Debug.Log($"[BullyBeatCutscene] Chuyển sang VN scene: {afterBeatVNScene.name}");
            VisualNovelManager.Instance.StartVNScene(afterBeatVNScene, OnCutsceneComplete);
        }
        else if (afterBeatDialogueNPC != null)
        {
            if (showDebugLogs) Debug.Log($"[BullyBeatCutscene] Trigger dialogue của: {afterBeatDialogueNPC.name}");
            afterBeatDialogueNPC.TriggerDialogueFromExternal(OnCutsceneComplete);
        }
        else
        {
            OnCutsceneComplete();
        }
    }

    private void FaceBulliesToPlayer()
    {
        if (player == null) return;
        
        foreach (var bully in bullies)
        {
            if (bully == null) continue;
            
            Animator animator = bully.GetComponent<Animator>();
            SpriteRenderer spriteRenderer = bully.GetComponent<SpriteRenderer>();
            
            if (animator == null) continue;
            
            Vector2 direction = (player.position - bully.transform.position).normalized;
            
            int horizontalHash = Animator.StringToHash("Horizontal");
            int verticalHash = Animator.StringToHash("Vertical");
            
            // Set hướng dựa trên direction chính
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                animator.SetFloat(horizontalHash, direction.x > 0 ? 1f : -1f);
                animator.SetFloat(verticalHash, 0f);
                
                // Flip sprite nếu cần
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = direction.x < 0;
                }
            }
            else
            {
                animator.SetFloat(horizontalHash, 0f);
                animator.SetFloat(verticalHash, direction.y > 0 ? 1f : -1f);
                
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = false;
                }
            }
            
            animator.SetFloat(speedHash, 0f);
        }
    }

    private IEnumerator RedFlashCoroutine()
    {
        // Tạo flash effect đơn giản bằng cách tint màn hình
        // Có thể mở rộng để dùng UI Image overlay
        
        // Nếu có ScreenFader, có thể dùng nó
        // Hiện tại chỉ log để demo
        if (showDebugLogs) Debug.Log("[BullyBeatCutscene] *FLASH* màn hình đỏ");
        
        yield return new WaitForSeconds(flashDuration);
    }

    private void ApplyStoryEffects()
    {
        if (StoryManager.Instance == null)
        {
            Debug.LogWarning("[BullyBeatCutscene] StoryManager không tồn tại!");
            return;
        }
        
        // Set flag got_beaten
        StoryManager.Instance.SetFlag(StoryManager.FlagKeys.GOT_BEATEN, true);
        if (showDebugLogs) Debug.Log("[BullyBeatCutscene] Set flag: got_beaten = true");
        
        // Tăng fear_level
        if (fearLevelIncrease > 0)
        {
            StoryManager.Instance.AddVariable(StoryManager.VarKeys.FEAR_LEVEL, fearLevelIncrease);
            int newFearLevel = StoryManager.Instance.GetVariable(StoryManager.VarKeys.FEAR_LEVEL);
            if (showDebugLogs) Debug.Log($"[BullyBeatCutscene] fear_level += {fearLevelIncrease} (now: {newFearLevel})");
        }
        
        // Set các flags tùy chỉnh
        if (flagsToSetTrue != null)
        {
            foreach (string flag in flagsToSetTrue)
            {
                if (!string.IsNullOrEmpty(flag))
                {
                    StoryManager.Instance.SetFlag(flag, true);
                    if (showDebugLogs) Debug.Log($"[BullyBeatCutscene] Set flag: {flag} = true");
                }
            }
        }
    }

    private void OnCutsceneComplete()
    {
        if (showDebugLogs) Debug.Log("[BullyBeatCutscene] Cutscene hoàn thành!");
        
        isPlaying = false;
        
        // Unlock player nếu không có VN scene hoặc dialogue tiếp theo
        if (afterBeatVNScene == null && afterBeatDialogueNPC == null)
        {
            if (playerMovement != null)
            {
                playerMovement.SetMovementEnabled(true);
                if (showDebugLogs) Debug.Log("[BullyBeatCutscene] Đã mở khóa player movement");
            }
        }
        
        // Gọi external callback nếu có
        if (externalCallback != null)
        {
            var callback = externalCallback;
            externalCallback = null;
            callback.Invoke();
        }
    }

    /// <summary>
    /// Dừng cutscene (emergency)
    /// </summary>
    public void StopCutscene()
    {
        if (!isPlaying) return;
        
        StopAllCoroutines();
        isPlaying = false;
        
        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(true);
        }
        
        Debug.Log("[BullyBeatCutscene] Cutscene bị dừng!");
    }

    // Gizmo để debug vị trí
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Vẽ line đến các bullies
        if (bullies != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var bully in bullies)
            {
                if (bully != null)
                {
                    Gizmos.DrawLine(transform.position, bully.transform.position);
                }
            }
        }
    }
}
