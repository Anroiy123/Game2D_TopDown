using UnityEngine;
using System.Collections;

/// <summary>
/// NPCSurroundPlayer - Điều khiển NPCs di chuyển vào vị trí vây quanh player
/// Dùng cho cảnh 14A-1: Bị vây quanh (Week1_Scene4A1)
/// </summary>
public class NPCSurroundPlayer : MonoBehaviour
{
    [Header("Surround Settings")]
    [Tooltip("Khoảng cách từ player đến mỗi NPC (bán kính vòng tròn)")]
    [SerializeField] private float surroundRadius = 2.5f;

    [Tooltip("Các NPC sẽ vây quanh player")]
    [SerializeField] private GameObject[] npcBullies;

    [Tooltip("Tốc độ di chuyển đến vị trí vây quanh")]
    [SerializeField] private float moveSpeed = 3f;

    [Tooltip("Thời gian delay giữa mỗi NPC di chuyển (tạo hiệu ứng lần lượt)")]
    [SerializeField] private float delayBetweenNPCs = 0.2f;

    [Tooltip("Tự động trigger khi component được enable")]
    [SerializeField] private bool autoTriggerOnEnable = false;

    [Header("Formation Type")]
    [Tooltip("Kiểu formation: Circle (vòng tròn) hoặc Semicircle (nửa vòng tròn phía trước)")]
    [SerializeField] private FormationType formationType = FormationType.Circle;

    [Header("Animation")]
    [Tooltip("Có sử dụng flipX cho hướng trái/phải không")]
    [SerializeField] private bool useFlipX = true;

    [Header("Next Scene")]
    [Tooltip("VN Scene sẽ được chạy sau khi vây quanh xong")]
    [SerializeField] private VNSceneData nextVNScene;

    [Tooltip("Delay trước khi chạy VN scene")]
    [SerializeField] private float vnSceneDelay = 0.5f;

    private Transform player;
    private PlayerMovement playerMovement;
    private Vector3 surroundCenter; // Vị trí trung tâm để vây quanh
    private bool isMoving = false;

    public enum FormationType
    {
        Circle,      // Vòng tròn đầy đủ
        Semicircle   // Nửa vòng tròn phía trước player
    }

    // Public setters cho NPCSurroundController
    public void SetNPCs(GameObject[] npcs) => npcBullies = npcs;
    public void SetRadius(float r) => surroundRadius = r;
    public void SetFormationType(FormationType t) => formationType = t;
    public void SetMoveSpeed(float s) => moveSpeed = s;
    public void SetDelay(float d) => delayBetweenNPCs = d;
    public void SetNextVNScene(VNSceneData scene) => nextVNScene = scene;

    private void OnEnable()
    {
        if (autoTriggerOnEnable)
        {
            StartSurrounding();
        }
    }

    /// <summary>
    /// Bắt đầu vây quanh player
    /// </summary>
    public void StartSurrounding()
    {
        if (isMoving)
        {
            Debug.LogWarning("[NPCSurroundPlayer] Đang di chuyển rồi!");
            return;
        }

        // QUAN TRỌNG: Luôn tìm player MỚI mỗi khi trigger
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("[NPCSurroundPlayer] Không tìm thấy player!");
            return;
        }

        player = playerObj.transform;
        playerMovement = playerObj.GetComponent<PlayerMovement>();

        // LƯU VỊ TRÍ PLAYER NGAY LÚC NÀY
        surroundCenter = player.position;
        Debug.Log($"[NPCSurroundPlayer] Player position: {surroundCenter}");

        if (npcBullies == null || npcBullies.Length == 0)
        {
            Debug.LogError("[NPCSurroundPlayer] Không có NPC nào được gán!");
            return;
        }

        // VALIDATION: Kiểm tra nextVNScene
        if (nextVNScene != null)
        {
            Debug.Log($"[NPCSurroundPlayer] ✓ Next VN Scene configured: {nextVNScene.name}");
        }
        else
        {
            Debug.LogWarning("[NPCSurroundPlayer] ⚠ Next VN Scene is NULL! Sẽ không có VN scene sau khi vây quanh.");
        }

        StartCoroutine(SurroundPlayerCoroutine());
    }

    private IEnumerator SurroundPlayerCoroutine()
    {
        isMoving = true;

        // KHÓA PLAYER NGAY LẬP TỨC
        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(false);
            Debug.Log("[NPCSurroundPlayer] ✓ Đã khóa player movement");
        }
        else
        {
            Debug.LogError("[NPCSurroundPlayer] ✗ PlayerMovement component không tìm thấy!");
        }

        Debug.Log($"[NPCSurroundPlayer] Bắt đầu vây quanh tại {surroundCenter} với {npcBullies.Length} NPCs, radius={surroundRadius}");

        // Tính toán vị trí cho mỗi NPC (dùng surroundCenter đã lưu)
        Vector3[] targetPositions = CalculateSurroundPositions();

        // Log để debug
        for (int i = 0; i < targetPositions.Length; i++)
        {
            Debug.Log($"[NPCSurroundPlayer] NPC {i} ({npcBullies[i]?.name}) sẽ đến: {targetPositions[i]}");
        }

        // Di chuyển TẤT CẢ NPCs cùng lúc (không delay)
        for (int i = 0; i < npcBullies.Length; i++)
        {
            if (npcBullies[i] == null)
            {
                Debug.LogWarning($"[NPCSurroundPlayer] NPC {i} is null, skipping");
                continue;
            }

            // Dừng NPCFollowPlayer nếu có
            NPCFollowPlayer followScript = npcBullies[i].GetComponent<NPCFollowPlayer>();
            if (followScript != null)
            {
                followScript.StopFollowing();
                Debug.Log($"[NPCSurroundPlayer] Stopped {npcBullies[i].name} from following");
            }

            // Bắt đầu di chuyển NPC này
            StartCoroutine(MoveNPCToPosition(npcBullies[i], targetPositions[i]));

            // Delay nhỏ giữa mỗi NPC (tạo hiệu ứng)
            if (delayBetweenNPCs > 0)
            {
                yield return new WaitForSeconds(delayBetweenNPCs);
            }
        }

        // Đợi tất cả NPCs đến vị trí (tăng thời gian chờ để chắc chắn)
        float waitTime = 2.0f;
        Debug.Log($"[NPCSurroundPlayer] Đợi {waitTime}s để NPCs về vị trí...");
        yield return new WaitForSeconds(waitTime);

        isMoving = false;

        Debug.Log("[NPCSurroundPlayer] ✓ Đã vây quanh xong!");

        // Chạy VN scene nếu có
        if (nextVNScene != null)
        {
            Debug.Log($"[NPCSurroundPlayer] Chuẩn bị trigger VN scene: {nextVNScene.name} (delay {vnSceneDelay}s)");
            yield return new WaitForSeconds(vnSceneDelay);

            // QUAN TRỌNG: Callback UnlockPlayer sẽ được gọi KHI VN SCENE HOÀN TẤT
            // Player sẽ bị khóa cho đến khi người chơi xem xong VN scene
            Debug.Log($"[NPCSurroundPlayer] → Triggering VN scene: {nextVNScene.name}");
            VisualNovelManager.Instance.StartVNScene(nextVNScene, OnVNSceneComplete);
        }
        else
        {
            Debug.LogWarning("[NPCSurroundPlayer] ✗ nextVNScene is NULL! Player sẽ được unlock ngay.");
            // Không có VN scene -> unlock player ngay
            UnlockPlayer();
        }

        OnSurroundComplete();
    }

    /// <summary>
    /// Callback khi VN scene hoàn tất - Unlock player
    /// </summary>
    private void OnVNSceneComplete()
    {
        Debug.Log("[NPCSurroundPlayer] VN scene hoàn tất, unlock player");
        UnlockPlayer();
    }

    /// <summary>
    /// Mở khóa player movement - gọi khi cần cho player di chuyển lại
    /// </summary>
    public void UnlockPlayer()
    {
        // Tìm lại PlayerMovement nếu reference bị mất
        if (playerMovement == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerMovement = playerObj.GetComponent<PlayerMovement>();
            }
        }

        if (playerMovement != null)
        {
            playerMovement.SetMovementEnabled(true);
            Debug.Log("[NPCSurroundPlayer] ✓ Player UNLOCKED - có thể di chuyển lại");
        }
        else
        {
            Debug.LogError("[NPCSurroundPlayer] ✗ Không thể unlock player - PlayerMovement not found!");
        }
    }

    private IEnumerator MoveNPCToPosition(GameObject npc, Vector3 targetPosition)
    {
        if (npc == null)
        {
            Debug.LogError("[NPCSurroundPlayer] NPC is null!");
            yield break;
        }

        Transform npcTransform = npc.transform;
        Animator animator = npc.GetComponent<Animator>();
        SpriteRenderer spriteRenderer = npc.GetComponent<SpriteRenderer>();

        // Animator hashes
        int speedHash = Animator.StringToHash("Speed");
        int horizontalHash = Animator.StringToHash("Horizontal");
        int verticalHash = Animator.StringToHash("Vertical");

        float startDistance = Vector3.Distance(npcTransform.position, targetPosition);
        Debug.Log($"[NPCSurroundPlayer] {npc.name}: Start pos={npcTransform.position}, Target={targetPosition}, Distance={startDistance}");

        while (Vector3.Distance(npcTransform.position, targetPosition) > 0.1f)
        {
            // Di chuyển
            npcTransform.position = Vector3.MoveTowards(
                npcTransform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Cập nhật animation
            Vector2 direction = (targetPosition - npcTransform.position).normalized;
            if (animator != null)
            {
                animator.SetFloat(speedHash, 1f);
                animator.SetFloat(horizontalHash, Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? direction.x : 0f);
                animator.SetFloat(verticalHash, Mathf.Abs(direction.y) > Mathf.Abs(direction.x) ? direction.y : 0f);
            }

            // FlipX cho hướng trái/phải
            if (useFlipX && spriteRenderer != null && Mathf.Abs(direction.x) > 0.1f)
            {
                spriteRenderer.flipX = direction.x > 0;
            }

            yield return null;
        }

        Debug.Log($"[NPCSurroundPlayer] {npc.name}: Arrived at {npcTransform.position}");

        // Dừng animation
        if (animator != null)
        {
            animator.SetFloat(speedHash, 0f);
        }

        // Quay mặt về phía player
        FacePlayer(npc);
    }

    private Vector3[] CalculateSurroundPositions()
    {
        Vector3[] positions = new Vector3[npcBullies.Length];
        int count = npcBullies.Length;

        // QUAN TRỌNG: Dùng surroundCenter (đã lưu) thay vì player.position (có thể thay đổi)
        if (formationType == FormationType.Circle)
        {
            // Vòng tròn đầy đủ (360 độ)
            float angleStep = 360f / count;
            for (int i = 0; i < count; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float x = surroundCenter.x + surroundRadius * Mathf.Cos(angle);
                float y = surroundCenter.y + surroundRadius * Mathf.Sin(angle);
                positions[i] = new Vector3(x, y, surroundCenter.z);
            }
        }
        else // Semicircle
        {
            // Nửa vòng tròn phía trước (180 độ)
            float angleStep = count > 1 ? 180f / (count - 1) : 0f;
            for (int i = 0; i < count; i++)
            {
                float angle = (90f + i * angleStep) * Mathf.Deg2Rad; // Bắt đầu từ 90 độ (phía trên)
                float x = surroundCenter.x + surroundRadius * Mathf.Cos(angle);
                float y = surroundCenter.y + surroundRadius * Mathf.Sin(angle);
                positions[i] = new Vector3(x, y, surroundCenter.z);
            }
        }

        return positions;
    }

    private void FacePlayer(GameObject npc)
    {
        // Dùng surroundCenter thay vì player.position
        Animator animator = npc.GetComponent<Animator>();
        SpriteRenderer spriteRenderer = npc.GetComponent<SpriteRenderer>();

        Vector2 direction = (surroundCenter - npc.transform.position).normalized;

        if (animator != null)
        {
            int horizontalHash = Animator.StringToHash("Horizontal");
            int verticalHash = Animator.StringToHash("Vertical");

            animator.SetFloat(horizontalHash, Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? direction.x : 0f);
            animator.SetFloat(verticalHash, Mathf.Abs(direction.y) > Mathf.Abs(direction.x) ? direction.y : 0f);
        }

        if (useFlipX && spriteRenderer != null && Mathf.Abs(direction.x) > 0.1f)
        {
            spriteRenderer.flipX = direction.x > 0;
        }
    }

    private void OnSurroundComplete()
    {
        // Có thể set flag hoặc trigger event ở đây
        if (StoryManager.Instance != null)
        {
            StoryManager.Instance.SetFlag("bullies_surrounded_player", true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Trong Editor, dùng player position để preview
        Transform previewTarget = null;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) previewTarget = playerObj.transform;

        if (previewTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(previewTarget.position, surroundRadius);

            // Vẽ vị trí dự kiến của NPCs
            if (npcBullies != null && npcBullies.Length > 0)
            {
                int count = npcBullies.Length;
                float angleStep = 360f / count;
                Gizmos.color = Color.yellow;
                for (int i = 0; i < count; i++)
                {
                    float angle = i * angleStep * Mathf.Deg2Rad;
                    float x = previewTarget.position.x + surroundRadius * Mathf.Cos(angle);
                    float y = previewTarget.position.y + surroundRadius * Mathf.Sin(angle);
                    Gizmos.DrawWireSphere(new Vector3(x, y, previewTarget.position.z), 0.3f);
                }
            }
        }
    }
}