using UnityEngine;

/// <summary>
/// BullyFollowTrigger - Trigger zone để spawn NPCs bully và làm chúng đi theo player
/// Dùng cho Scene 5 (Day 1) - Đức đi về nhà, tụi bắt nạt theo sau
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BullyFollowTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [Tooltip("Chỉ trigger 1 lần duy nhất")]
    [SerializeField] private bool triggerOnce = true;

    [Tooltip("Flags cần có để trigger được (tất cả phải true)")]
    [SerializeField] private string[] requiredFlags;

    [Tooltip("Flags không được có (nếu có 1 trong số này thì không trigger)")]
    [SerializeField] private string[] forbiddenFlags;

    [Tooltip("Flag kiểm tra - nếu đã có flag này thì không trigger (legacy, dùng forbiddenFlags thay thế)")]
    [SerializeField] private string skipIfFlag = "day1_scene5_bullies_spawned";

    [Header("Bully NPCs")]
    [Tooltip("OPTION 1: Dùng NPCs có sẵn trong scene (khuyên dùng)")]
    [SerializeField] private GameObject[] existingBullyNPCs;

    [Tooltip("OPTION 2: Prefab của NPC bully (nếu muốn spawn mới)")]
    [SerializeField] private GameObject bullyPrefab;

    [Tooltip("Số lượng bully sẽ spawn (chỉ dùng khi spawn từ prefab)")]
    [SerializeField] private int bullyCount = 3;

    [Tooltip("Vị trí spawn của các bully (chỉ dùng khi spawn từ prefab)")]
    [SerializeField] private Transform[] spawnPoints;

    [Tooltip("Khoảng cách spawn phía sau player nếu không có spawn points")]
    [SerializeField] private float spawnDistanceBehind = 5f;

    [Tooltip("Khoảng cách giữa các bully khi spawn")]
    [SerializeField] private float spacingBetweenBullies = 1.5f;

    [Header("Follow Behavior")]
    [Tooltip("Khoảng cách tối thiểu giữa bully và player")]
    [SerializeField] private float followMinDistance = 2f;

    [Tooltip("Khoảng cách tối đa - nếu player xa hơn thì bully chạy nhanh")]
    [SerializeField] private float followMaxDistance = 5f;

    [Header("Formation Settings")]
    [Tooltip("Khoảng cách giữa các bully theo chiều ngang (để không dính vào nhau)")]
    [SerializeField] private float formationSpacing = 1.5f;

    [Tooltip("Offset phía sau player (Y âm = phía sau)")]
    [SerializeField] private float formationBehindOffset = -2f;

    [Header("Next Trigger")]
    [Tooltip("VNTrigger sẽ được activate sau khi spawn bullies (Scene 6)")]
    [SerializeField] private GameObject nextVNTrigger;
    
    [Tooltip("Delay trước khi activate VNTrigger tiếp theo (giây)")]
    [SerializeField] private float nextTriggerDelay = 3f;

    private bool hasTriggered = false;
    private GameObject[] activeBullies; // NPCs đang active (từ existing hoặc spawned)

    private void Start()
    {
        // Kiểm tra skipIfFlag (legacy support)
        if (StoryManager.Instance != null && !string.IsNullOrEmpty(skipIfFlag))
        {
            if (StoryManager.Instance.GetFlag(skipIfFlag))
            {
                hasTriggered = true;
                Debug.Log($"[BullyFollowTrigger] Đã có flag '{skipIfFlag}', không trigger");
            }
        }

        // Kiểm tra forbiddenFlags
        if (StoryManager.Instance != null && forbiddenFlags != null && forbiddenFlags.Length > 0)
        {
            foreach (string flag in forbiddenFlags)
            {
                if (!string.IsNullOrEmpty(flag) && StoryManager.Instance.GetFlag(flag))
                {
                    hasTriggered = true;
                    Debug.Log($"[BullyFollowTrigger] BLOCKED - Có forbidden flag: {flag}");
                    break;
                }
            }
        }

        // Kiểm tra nếu player đã chạy trốn - ẩn bullies vĩnh viễn
        if (StoryManager.Instance != null && StoryManager.Instance.GetFlag("ran_from_bullies"))
        {
            DeactivateAllBullies();
            hasTriggered = true; // Prevent re-trigger
            Debug.Log("[BullyFollowTrigger] Player đã chạy trốn, ẩn tất cả bullies");
            return; // Skip phần còn lại
        }

        // Ẩn existing bullies ban đầu
        if (existingBullyNPCs != null && existingBullyNPCs.Length > 0)
        {
            foreach (var bully in existingBullyNPCs)
            {
                if (bully != null)
                {
                    bully.SetActive(false);
                }
            }
        }

        // Ẩn next trigger ban đầu
        if (nextVNTrigger != null)
        {
            nextVNTrigger.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (hasTriggered && triggerOnce) return;

        // Kiểm tra requiredFlags
        if (StoryManager.Instance != null && requiredFlags != null && requiredFlags.Length > 0)
        {
            foreach (string flag in requiredFlags)
            {
                if (!string.IsNullOrEmpty(flag) && !StoryManager.Instance.GetFlag(flag))
                {
                    Debug.Log($"[BullyFollowTrigger] BLOCKED - Thiếu required flag: {flag}");
                    return; // Không trigger nếu thiếu flag
                }
            }
        }

        TriggerBullySpawn(other.transform);
    }

    private void TriggerBullySpawn(Transform player)
    {
        hasTriggered = true;

        Debug.Log("[BullyFollowTrigger] Player vào zone - Activate bullies!");

        // Set flag
        if (StoryManager.Instance != null && !string.IsNullOrEmpty(skipIfFlag))
        {
            StoryManager.Instance.SetFlag(skipIfFlag, true);
        }

        // Ưu tiên dùng existing NPCs, nếu không có thì spawn mới
        if (existingBullyNPCs != null && existingBullyNPCs.Length > 0)
        {
            ActivateExistingBullies();
        }
        else
        {
            SpawnBullies(player);
        }

        // Activate next trigger sau delay
        if (nextVNTrigger != null)
        {
            Invoke(nameof(ActivateNextTrigger), nextTriggerDelay);
        }
    }

    private void ActivateExistingBullies()
    {
        activeBullies = new GameObject[existingBullyNPCs.Length];

        // Tìm player để tính spawn position
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        Transform player = playerObj != null ? playerObj.transform : null;

        for (int i = 0; i < existingBullyNPCs.Length; i++)
        {
            GameObject bully = existingBullyNPCs[i];
            if (bully == null) continue;

            // Activate NPC
            bully.SetActive(true);
            activeBullies[i] = bully;

            // FIX: Đảm bảo Animator được enable
            Animator animator = bully.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = true;
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                Debug.Log($"[BullyFollowTrigger] {bully.name}: Animator enabled, CullingMode=AlwaysAnimate");
            }

            // Setup follow behavior
            NPCFollowPlayer followScript = bully.GetComponent<NPCFollowPlayer>();
            if (followScript == null)
            {
                // Tự động add component nếu chưa có
                followScript = bully.AddComponent<NPCFollowPlayer>();
                Debug.Log($"[BullyFollowTrigger] Đã thêm NPCFollowPlayer vào {bully.name}");
            }

            // Teleport đến spawn point nếu có
            if (spawnPoints != null && i < spawnPoints.Length && spawnPoints[i] != null)
            {
                followScript.TeleportTo(spawnPoints[i].position);
                Debug.Log($"[BullyFollowTrigger] Teleported {bully.name} đến {spawnPoints[i].name}");
            }
            else if (player != null)
            {
                // Spawn phía sau player nếu không có spawn point
                Vector3 spawnPos = CalculateSpawnPositionBehindPlayer(player, i);
                followScript.TeleportTo(spawnPos);
                Debug.Log($"[BullyFollowTrigger] Teleported {bully.name} đến vị trí phía sau player");
            }

            // Set offset để NPCs không dính vào nhau
            Vector2 offset = CalculateFollowOffset(i, existingBullyNPCs.Length);
            followScript.SetFollowOffset(offset);

            // Set khoảng cách follow
            followScript.SetFollowDistance(followMinDistance, followMaxDistance);

            // Bắt đầu follow
            followScript.StartFollowing();

            Debug.Log($"[BullyFollowTrigger] Activated {bully.name} với offset {offset}");
        }
    }

    private void SpawnBullies(Transform player)
    {
        if (bullyPrefab == null)
        {
            Debug.LogError("[BullyFollowTrigger] Chưa gán bullyPrefab!");
            return;
        }

        activeBullies = new GameObject[bullyCount];

        for (int i = 0; i < bullyCount; i++)
        {
            Vector3 spawnPos;

            // Xác định vị trí spawn
            if (spawnPoints != null && i < spawnPoints.Length && spawnPoints[i] != null)
            {
                // Dùng spawn point đã định sẵn
                spawnPos = spawnPoints[i].position;
            }
            else
            {
                // Spawn phía sau player
                spawnPos = CalculateSpawnPositionBehindPlayer(player, i);
            }

            // Spawn bully
            GameObject bully = Instantiate(bullyPrefab, spawnPos, Quaternion.identity);
            bully.name = $"Bully_{i + 1}";
            activeBullies[i] = bully;

            // Setup follow behavior
            NPCFollowPlayer followScript = bully.GetComponent<NPCFollowPlayer>();
            if (followScript != null)
            {
                followScript.SetFollowDistance(followMinDistance, followMaxDistance);
                followScript.StartFollowing();
            }
            else
            {
                Debug.LogWarning($"[BullyFollowTrigger] {bully.name} không có NPCFollowPlayer component!");
            }

            Debug.Log($"[BullyFollowTrigger] Spawned {bully.name} tại {spawnPos}");
        }
    }

    private Vector3 CalculateSpawnPositionBehindPlayer(Transform player, int index)
    {
        // Spawn phía sau player (Y âm = phía dưới trong coordinate system)
        Vector3 behindOffset = new Vector3(0, -spawnDistanceBehind, 0);

        // Tính offset ngang để các bully không chồng lên nhau
        int totalBullies = existingBullyNPCs != null ? existingBullyNPCs.Length : bullyCount;
        float horizontalOffset = (index - (totalBullies - 1) / 2f) * spacingBetweenBullies;

        return player.position + behindOffset + new Vector3(horizontalOffset, 0, 0);
    }

    private Vector2 CalculateFollowOffset(int index, int totalBullies)
    {
        // Tính offset để NPCs xếp thành hàng ngang phía sau player
        // Index 0 = giữa, 1 = trái, 2 = phải (hoặc tùy số lượng)

        float horizontalOffset = (index - (totalBullies - 1) / 2f) * formationSpacing;
        float verticalOffset = formationBehindOffset;

        return new Vector2(horizontalOffset, verticalOffset);
    }

    private void ActivateNextTrigger()
    {
        if (nextVNTrigger != null)
        {
            nextVNTrigger.SetActive(true);
            Debug.Log("[BullyFollowTrigger] Đã activate VNTrigger tiếp theo (Scene 6)");
        }
    }

    /// <summary>
    /// Dừng tất cả bullies đi theo (gọi từ bên ngoài nếu cần)
    /// </summary>
    public void StopAllBullies()
    {
        if (activeBullies == null) return;

        foreach (var bully in activeBullies)
        {
            if (bully != null)
            {
                NPCFollowPlayer followScript = bully.GetComponent<NPCFollowPlayer>();
                if (followScript != null)
                {
                    followScript.StopFollowing();
                }
            }
        }
    }

    /// <summary>
    /// Ẩn tất cả bullies (gọi khi player đã chạy trốn)
    /// </summary>
    private void DeactivateAllBullies()
    {
        if (existingBullyNPCs != null && existingBullyNPCs.Length > 0)
        {
            foreach (var bully in existingBullyNPCs)
            {
                if (bully != null)
                {
                    // Dừng follow trước khi ẩn
                    NPCFollowPlayer followScript = bully.GetComponent<NPCFollowPlayer>();
                    if (followScript != null)
                    {
                        followScript.StopFollowing();
                    }

                    // Ẩn NPC
                    bully.SetActive(false);
                }
            }
        }

        // Ẩn activeBullies nếu đã được spawn
        if (activeBullies != null && activeBullies.Length > 0)
        {
            foreach (var bully in activeBullies)
            {
                if (bully != null)
                {
                    NPCFollowPlayer followScript = bully.GetComponent<NPCFollowPlayer>();
                    if (followScript != null)
                    {
                        followScript.StopFollowing();
                    }
                    bully.SetActive(false);
                }
            }
        }
    }
}

