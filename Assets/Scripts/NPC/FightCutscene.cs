using UnityEngine;
using System.Collections;

/// <summary>
/// FightCutscene - Cutscene đánh nhau 1v1 giữa Player và NPC (Thủ lĩnh)
/// Dùng cho Scene 27 Critical Day - Đức đối mặt và thắng Thủ lĩnh
/// Khác với BullyBeatCutscene (nhiều bully đánh 1 player), script này là combat 2 chiều
/// </summary>
public class FightCutscene : MonoBehaviour
{
    [Header("Combatants")]
    [Tooltip("NPC đối thủ (Thủ lĩnh)")]
    [SerializeField] private GameObject opponent;
    
    [Header("Fight Sequence")]
    [Tooltip("Số round đánh nhau (mỗi round = 1 lượt đánh qua lại)")]
    [SerializeField] private int fightRounds = 3;
    
    [Tooltip("Thời gian giữa mỗi cú đánh (giây)")]
    [SerializeField] private float attackInterval = 0.6f;
    
    [Tooltip("Delay giữa các round")]
    [SerializeField] private float roundDelay = 0.8f;
    
    [Header("Animation Triggers")]
    [Tooltip("Trigger đánh của Player")]
    [SerializeField] private string playerAttackTrigger = "Punch";
    
    [Tooltip("Trigger bị đánh của Player")]
    [SerializeField] private string playerHurtTrigger = "Hurt";
    
    [Tooltip("Trigger đánh của NPC")]
    [SerializeField] private string npcAttackTrigger = "Punch";
    
    [Tooltip("Trigger bị đánh của NPC")]
    [SerializeField] private string npcHurtTrigger = "Hurt";
    
    [Tooltip("Trigger ngã/thua của NPC")]
    [SerializeField] private string npcDefeatTrigger = "Defeat";
    
    [Header("Final Blow Settings")]
    [Tooltip("Player có cú đánh cuối cùng (thắng)")]
    [SerializeField] private bool playerWins = true;
    
    [Tooltip("Delay trước cú đánh cuối")]
    [SerializeField] private float finalBlowDelay = 0.5f;
    
    [Header("Screen Effects")]
    [Tooltip("Flash màn hình khi bị đánh")]
    [SerializeField] private bool useScreenFlash = true;
    
    [Tooltip("Màu flash khi Player bị đánh")]
    [SerializeField] private Color playerHitFlashColor = new Color(1f, 0f, 0f, 0.3f);
    
    [Tooltip("Màu flash khi NPC bị đánh")]
    [SerializeField] private Color npcHitFlashColor = new Color(1f, 1f, 0f, 0.3f);
    
    [SerializeField] private float flashDuration = 0.1f;
    
    [Header("Camera Shake")]
    [Tooltip("Rung camera khi đánh")]
    [SerializeField] private bool useCameraShake = true;
    
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeDuration = 0.15f;

    [Header("Trigger Settings")]
    [Tooltip("Chờ dialogue kết thúc trước khi bắt đầu")]
    [SerializeField] private bool waitForDialogueEnd = true;
    
    [Tooltip("Delay sau dialogue")]
    [SerializeField] private float delayAfterDialogue = 0.3f;
    
    [Header("After Fight")]
    [Tooltip("VN scene chạy sau khi đánh xong")]
    [SerializeField] private VNSceneData afterFightVNScene;
    
    [Tooltip("Dialogue NPC sau khi đánh (nếu không dùng VN)")]
    [SerializeField] private NPCInteraction afterFightDialogueNPC;
    
    [Tooltip("Delay trước khi chạy scene tiếp theo")]
    [SerializeField] private float afterFightDelay = 1f;
    
    [Header("Story Effects")]
    [Tooltip("Flags set TRUE sau cutscene")]
    [SerializeField] private string[] flagsToSetTrue;
    
    [Tooltip("Flags set FALSE sau cutscene")]
    [SerializeField] private string[] flagsToSetFalse;
    
    [Header("Bully Scatter")]
    [Tooltip("Các đàn em sẽ chạy tán loạn sau khi thủ lĩnh thua")]
    [SerializeField] private GameObject[] bullyMinions;
    
    [Tooltip("Tốc độ chạy tán loạn")]
    [SerializeField] private float scatterSpeed = 5f;
    
    [Tooltip("Khoảng cách chạy")]
    [SerializeField] private float scatterDistance = 10f;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    
    // Private references
    private Transform player;
    private PlayerMovement playerMovement;
    private Animator playerAnimator;
    private Animator opponentAnimator;
    private bool isPlaying = false;
    private System.Action externalCallback;
    
    // Animator hashes
    private int playerAttackHash;
    private int playerHurtHash;
    private int npcAttackHash;
    private int npcHurtHash;
    private int npcDefeatHash;
    private readonly int speedHash = Animator.StringToHash("Speed");
    
    // Screen flash
    private CanvasGroup flashOverlay;
    private UnityEngine.UI.Image flashImage;
    
    private void Awake()
    {
        // Hash animator parameters
        playerAttackHash = Animator.StringToHash(playerAttackTrigger);
        playerHurtHash = Animator.StringToHash(playerHurtTrigger);
        npcAttackHash = Animator.StringToHash(npcAttackTrigger);
        npcHurtHash = Animator.StringToHash(npcHurtTrigger);
        npcDefeatHash = Animator.StringToHash(npcDefeatTrigger);
    }
    
    private void Start()
    {
        FindPlayer();
        SetupOpponent();
        
        if (useScreenFlash)
        {
            CreateFlashOverlay();
        }
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
    
    private void SetupOpponent()
    {
        if (opponent != null)
        {
            opponentAnimator = opponent.GetComponent<Animator>();
        }
    }

    private void CreateFlashOverlay()
    {
        // Tìm hoặc tạo Canvas cho flash effect
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("FightFlashCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        }
        
        // Tạo flash overlay
        GameObject flashObj = new GameObject("FightFlashOverlay");
        flashObj.transform.SetParent(canvas.transform, false);
        
        flashImage = flashObj.AddComponent<UnityEngine.UI.Image>();
        flashImage.color = Color.clear;
        flashImage.raycastTarget = false;
        
        // Full screen
        RectTransform rect = flashObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        flashOverlay = flashObj.AddComponent<CanvasGroup>();
        flashOverlay.alpha = 0;
        flashOverlay.blocksRaycasts = false;
        flashOverlay.interactable = false;
    }
    
    /// <summary>
    /// Bắt đầu cutscene đánh nhau
    /// Gọi từ DialogueChoice action hoặc script khác
    /// </summary>
    public void StartFightCutscene()
    {
        if (isPlaying)
        {
            if (showDebugLogs) Debug.LogWarning("[FightCutscene] Cutscene đang chạy!");
            return;
        }
        
        if (player == null) FindPlayer();
        if (opponentAnimator == null) SetupOpponent();
        
        if (player == null)
        {
            Debug.LogError("[FightCutscene] Không tìm thấy Player!");
            return;
        }
        
        if (opponent == null)
        {
            Debug.LogError("[FightCutscene] Không có opponent được gán!");
            return;
        }
        
        if (waitForDialogueEnd)
        {
            if (showDebugLogs) Debug.Log("[FightCutscene] Chờ dialogue kết thúc...");
            StartCoroutine(WaitForDialogueEndCoroutine());
        }
        else
        {
            StartCoroutine(FightCutsceneCoroutine());
        }
    }
    
    /// <summary>
    /// Bắt đầu với callback
    /// </summary>
    public void StartFightCutscene(System.Action onComplete)
    {
        externalCallback = onComplete;
        StartFightCutscene();
    }
    
    private IEnumerator WaitForDialogueEndCoroutine()
    {
        DialogueSystem dialogueSystem = FindFirstObjectByType<DialogueSystem>();
        
        if (dialogueSystem != null)
        {
            while (dialogueSystem.IsDialogueActive())
            {
                yield return null;
            }
            if (showDebugLogs) Debug.Log("[FightCutscene] Dialogue đã kết thúc");
        }
        
        if (delayAfterDialogue > 0)
        {
            yield return new WaitForSeconds(delayAfterDialogue);
        }
        
        StartCoroutine(FightCutsceneCoroutine());
    }

    private IEnumerator FightCutsceneCoroutine()
    {
        isPlaying = true;
        if (showDebugLogs) Debug.Log($"[FightCutscene] Bắt đầu fight với {fightRounds} rounds");
        
        // 1. Khóa player movement
        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(false);
            if (showDebugLogs) Debug.Log("[FightCutscene] Đã khóa player movement");
        }
        
        // 2. Dừng animation di chuyển
        if (playerAnimator != null)
        {
            playerAnimator.SetFloat(speedHash, 0f);
        }
        if (opponentAnimator != null)
        {
            opponentAnimator.SetFloat(speedHash, 0f);
        }
        
        // 3. Quay mặt về phía nhau
        FaceEachOther();
        
        yield return new WaitForSeconds(0.5f);
        
        // 4. Chạy các round đánh nhau
        for (int round = 0; round < fightRounds; round++)
        {
            if (showDebugLogs) Debug.Log($"[FightCutscene] Round {round + 1}/{fightRounds}");
            
            // NPC đánh trước
            yield return StartCoroutine(NPCAttackCoroutine());
            yield return new WaitForSeconds(attackInterval);
            
            // Player đánh lại
            yield return StartCoroutine(PlayerAttackCoroutine());
            
            if (round < fightRounds - 1)
            {
                yield return new WaitForSeconds(roundDelay);
            }
        }
        
        // 5. Cú đánh quyết định
        yield return new WaitForSeconds(finalBlowDelay);
        
        if (playerWins)
        {
            yield return StartCoroutine(PlayerFinalBlowCoroutine());
        }
        else
        {
            yield return StartCoroutine(NPCFinalBlowCoroutine());
        }
        
        // 6. Đàn em chạy tán loạn (nếu player thắng)
        if (playerWins && bullyMinions != null && bullyMinions.Length > 0)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(ScatterMinionsCoroutine());
        }
        
        // 7. Delay và apply effects
        yield return new WaitForSeconds(afterFightDelay);
        ApplyStoryEffects();
        
        // 8. Chuyển scene tiếp theo
        if (afterFightVNScene != null)
        {
            if (showDebugLogs) Debug.Log($"[FightCutscene] Chuyển sang VN scene: {afterFightVNScene.name}");
            VisualNovelManager.Instance.StartVNScene(afterFightVNScene, OnCutsceneComplete);
        }
        else if (afterFightDialogueNPC != null)
        {
            if (showDebugLogs) Debug.Log($"[FightCutscene] Trigger dialogue: {afterFightDialogueNPC.name}");
            afterFightDialogueNPC.TriggerDialogueFromExternal(OnCutsceneComplete);
        }
        else
        {
            OnCutsceneComplete();
        }
    }
    
    private void FaceEachOther()
    {
        if (player == null || opponent == null) return;
        
        Vector2 direction = (opponent.transform.position - player.position).normalized;
        
        // Player quay về phía opponent
        if (playerAnimator != null)
        {
            playerAnimator.SetFloat(Animator.StringToHash("Horizontal"), direction.x);
            playerAnimator.SetFloat(Animator.StringToHash("Vertical"), direction.y);
        }
        
        // Opponent quay về phía player
        if (opponentAnimator != null)
        {
            opponentAnimator.SetFloat(Animator.StringToHash("Horizontal"), -direction.x);
            opponentAnimator.SetFloat(Animator.StringToHash("Vertical"), -direction.y);
        }
    }

    private IEnumerator NPCAttackCoroutine()
    {
        if (showDebugLogs) Debug.Log("[FightCutscene] NPC đánh Player");
        
        // NPC đánh
        if (opponentAnimator != null)
        {
            opponentAnimator.SetTrigger(npcAttackHash);
        }
        
        yield return new WaitForSeconds(0.15f);
        
        // Player bị đánh
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(playerHurtHash);
        }
        
        // Screen flash
        if (useScreenFlash)
        {
            yield return StartCoroutine(FlashCoroutine(playerHitFlashColor));
        }
        
        // Camera shake
        if (useCameraShake)
        {
            StartCoroutine(CameraShakeCoroutine());
        }
    }
    
    private IEnumerator PlayerAttackCoroutine()
    {
        if (showDebugLogs) Debug.Log("[FightCutscene] Player đánh NPC");
        
        // Player đánh
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(playerAttackHash);
        }
        
        yield return new WaitForSeconds(0.15f);
        
        // NPC bị đánh
        if (opponentAnimator != null)
        {
            opponentAnimator.SetTrigger(npcHurtHash);
        }
        
        // Screen flash
        if (useScreenFlash)
        {
            yield return StartCoroutine(FlashCoroutine(npcHitFlashColor));
        }
        
        // Camera shake
        if (useCameraShake)
        {
            StartCoroutine(CameraShakeCoroutine());
        }
    }
    
    private IEnumerator PlayerFinalBlowCoroutine()
    {
        if (showDebugLogs) Debug.Log("[FightCutscene] Player cú đánh quyết định - THẮNG!");
        
        // Player đánh mạnh
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(playerAttackHash);
        }
        
        yield return new WaitForSeconds(0.2f);
        
        // NPC ngã
        if (opponentAnimator != null)
        {
            // Thử trigger Defeat, nếu không có thì dùng Hurt
            opponentAnimator.SetTrigger(npcDefeatHash);
        }
        
        // Flash mạnh hơn
        if (useScreenFlash)
        {
            yield return StartCoroutine(FlashCoroutine(new Color(1f, 1f, 1f, 0.5f)));
        }
        
        // Camera shake mạnh
        if (useCameraShake)
        {
            StartCoroutine(CameraShakeCoroutine(shakeIntensity * 2f, shakeDuration * 2f));
        }
    }
    
    private IEnumerator NPCFinalBlowCoroutine()
    {
        if (showDebugLogs) Debug.Log("[FightCutscene] NPC cú đánh quyết định - Player thua");
        
        // NPC đánh mạnh
        if (opponentAnimator != null)
        {
            opponentAnimator.SetTrigger(npcAttackHash);
        }
        
        yield return new WaitForSeconds(0.2f);
        
        // Player ngã
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(playerHurtHash);
        }
        
        // Flash đỏ mạnh
        if (useScreenFlash)
        {
            yield return StartCoroutine(FlashCoroutine(new Color(1f, 0f, 0f, 0.6f)));
        }
        
        if (useCameraShake)
        {
            StartCoroutine(CameraShakeCoroutine(shakeIntensity * 2f, shakeDuration * 2f));
        }
    }

    private IEnumerator ScatterMinionsCoroutine()
    {
        if (showDebugLogs) Debug.Log($"[FightCutscene] Đàn em chạy tán loạn: {bullyMinions.Length} NPCs");
        
        foreach (var minion in bullyMinions)
        {
            if (minion == null) continue;
            
            // Dừng follow nếu có
            NPCFollowPlayer followScript = minion.GetComponent<NPCFollowPlayer>();
            if (followScript != null)
            {
                followScript.StopFollowing();
            }
            
            // Chạy ra hướng ngẫu nhiên
            StartCoroutine(MinionRunAwayCoroutine(minion));
        }
        
        yield return new WaitForSeconds(2f);
        
        // Ẩn đàn em sau khi chạy xa
        foreach (var minion in bullyMinions)
        {
            if (minion != null)
            {
                minion.SetActive(false);
            }
        }
    }
    
    private IEnumerator MinionRunAwayCoroutine(GameObject minion)
    {
        // Hướng chạy ngẫu nhiên (xa player)
        Vector2 runDirection = (minion.transform.position - player.position).normalized;
        runDirection += new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        runDirection.Normalize();
        
        Animator minionAnimator = minion.GetComponent<Animator>();
        Vector3 startPos = minion.transform.position;
        Vector3 targetPos = startPos + (Vector3)(runDirection * scatterDistance);
        
        float elapsed = 0f;
        float duration = scatterDistance / scatterSpeed;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            minion.transform.position = Vector3.Lerp(startPos, targetPos, t);
            
            // Update animator direction
            if (minionAnimator != null)
            {
                minionAnimator.SetFloat(Animator.StringToHash("Horizontal"), runDirection.x);
                minionAnimator.SetFloat(Animator.StringToHash("Vertical"), runDirection.y);
                minionAnimator.SetFloat(speedHash, scatterSpeed);
            }
            
            yield return null;
        }
        
        // Dừng animation
        if (minionAnimator != null)
        {
            minionAnimator.SetFloat(speedHash, 0f);
        }
    }
    
    private IEnumerator FlashCoroutine(Color flashColor)
    {
        if (flashImage == null) yield break;
        
        flashImage.color = flashColor;
        flashOverlay.alpha = 1f;
        
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            flashOverlay.alpha = 1f - (elapsed / flashDuration);
            yield return null;
        }
        
        flashOverlay.alpha = 0f;
    }
    
    private IEnumerator CameraShakeCoroutine()
    {
        yield return CameraShakeCoroutine(shakeIntensity, shakeDuration);
    }
    
    private IEnumerator CameraShakeCoroutine(float intensity, float duration)
    {
        Camera cam = Camera.main;
        if (cam == null) yield break;
        
        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            
            cam.transform.localPosition = originalPos + new Vector3(x, y, 0);
            yield return null;
        }
        
        cam.transform.localPosition = originalPos;
    }

    private void ApplyStoryEffects()
    {
        if (StoryManager.Instance == null)
        {
            Debug.LogWarning("[FightCutscene] StoryManager không tồn tại!");
            return;
        }
        
        // Set flags TRUE
        if (flagsToSetTrue != null)
        {
            foreach (string flag in flagsToSetTrue)
            {
                if (!string.IsNullOrEmpty(flag))
                {
                    StoryManager.Instance.SetFlag(flag, true);
                    if (showDebugLogs) Debug.Log($"[FightCutscene] Set flag: {flag} = true");
                }
            }
        }
        
        // Set flags FALSE
        if (flagsToSetFalse != null)
        {
            foreach (string flag in flagsToSetFalse)
            {
                if (!string.IsNullOrEmpty(flag))
                {
                    StoryManager.Instance.SetFlag(flag, false);
                    if (showDebugLogs) Debug.Log($"[FightCutscene] Set flag: {flag} = false");
                }
            }
        }
    }
    
    private void OnCutsceneComplete()
    {
        if (showDebugLogs) Debug.Log("[FightCutscene] Cutscene hoàn thành!");
        
        isPlaying = false;
        
        // Unlock player nếu không có scene tiếp theo
        if (afterFightVNScene == null && afterFightDialogueNPC == null)
        {
            if (playerMovement != null)
            {
                playerMovement.SetMovementEnabled(true);
                if (showDebugLogs) Debug.Log("[FightCutscene] Đã mở khóa player movement");
            }
        }
        
        // Ẩn opponent nếu player thắng
        if (playerWins && opponent != null)
        {
            // Có thể ẩn hoặc để nằm đó tùy design
            // opponent.SetActive(false);
        }
        
        // Gọi external callback
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
        
        Debug.Log("[FightCutscene] Cutscene bị dừng!");
    }
    
    /// <summary>
    /// Kiểm tra cutscene đang chạy không
    /// </summary>
    public bool IsPlaying => isPlaying;
    
    // Gizmo debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        if (opponent != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, opponent.transform.position);
            Gizmos.DrawWireSphere(opponent.transform.position, 0.3f);
        }
        
        if (bullyMinions != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var minion in bullyMinions)
            {
                if (minion != null)
                {
                    Gizmos.DrawLine(transform.position, minion.transform.position);
                }
            }
        }
    }
}
