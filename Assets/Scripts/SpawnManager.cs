using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Quản lý tất cả spawn points trong scene
/// Tự động tìm và đăng ký các SpawnPoint components
/// </summary>
public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] 
    [Tooltip("Danh sách tất cả spawn points trong scene (tự động tìm khi Awake)")]
    private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    [Header("Debug")]
    [SerializeField] 
    private bool showDebugLogs = true;

    private void Awake()
    {
        // Tự động tìm tất cả SpawnPoint trong scene
        FindAllSpawnPoints();
    }

    /// <summary>
    /// Tìm tất cả SpawnPoint components trong scene
    /// </summary>
    private void FindAllSpawnPoints()
    {
        spawnPoints.Clear();

        // Tìm tất cả SpawnPoint trong scene (bao gồm cả inactive objects)
        SpawnPoint[] foundPoints = FindObjectsByType<SpawnPoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        spawnPoints.AddRange(foundPoints);

        if (showDebugLogs)
        {
            Debug.Log($"[SpawnManager] Tìm thấy {spawnPoints.Count} spawn points trong scene '{gameObject.scene.name}':");
            foreach (var point in spawnPoints)
            {
                string defaultMarker = point.IsDefaultSpawn ? " (DEFAULT)" : "";
                Debug.Log($"  - {point.gameObject.name}: ID='{point.SpawnPointId}'{defaultMarker}");
            }
        }
    }

    /// <summary>
    /// Lấy SpawnPoint theo ID
    /// </summary>
    public SpawnPoint GetSpawnPoint(string spawnPointId)
    {
        if (string.IsNullOrEmpty(spawnPointId))
        {
            if (showDebugLogs)
                Debug.LogWarning("[SpawnManager] SpawnPointId rỗng, sử dụng default spawn point");
            return GetDefaultSpawnPoint();
        }

        var point = spawnPoints.FirstOrDefault(sp => sp.SpawnPointId == spawnPointId);
        
        if (point == null)
        {
            Debug.LogWarning($"[SpawnManager] Không tìm thấy spawn point với ID '{spawnPointId}', sử dụng default spawn point");
            return GetDefaultSpawnPoint();
        }

        return point;
    }

    /// <summary>
    /// Lấy default spawn point (spawn point đầu tiên có IsDefaultSpawn = true)
    /// </summary>
    public SpawnPoint GetDefaultSpawnPoint()
    {
        var defaultPoint = spawnPoints.FirstOrDefault(sp => sp.IsDefaultSpawn);
        
        if (defaultPoint == null && spawnPoints.Count > 0)
        {
            Debug.LogWarning("[SpawnManager] Không có default spawn point, sử dụng spawn point đầu tiên");
            defaultPoint = spawnPoints[0];
        }

        if (defaultPoint == null)
        {
            Debug.LogError("[SpawnManager] Không có spawn point nào trong scene!");
        }

        return defaultPoint;
    }

    /// <summary>
    /// Spawn player tại spawn point với ID chỉ định
    /// </summary>
    public void SpawnPlayer(GameObject player, string spawnPointId)
    {
        if (player == null)
        {
            Debug.LogError("[SpawnManager] Player object null!");
            return;
        }

        SpawnPoint point = GetSpawnPoint(spawnPointId);

        if (point != null)
        {
            // Di chuyển player đến vị trí spawn point
            player.transform.position = point.transform.position;

            // Set hướng nhìn của player theo spawn point
            SetPlayerFacing(player, point.GetFacingVector());

            if (showDebugLogs)
                Debug.Log($"[SpawnManager] Đã spawn player tại '{point.gameObject.name}' (ID: '{point.SpawnPointId}')");
        }
    }

    /// <summary>
    /// Set hướng nhìn của player
    /// </summary>
    private void SetPlayerFacing(GameObject player, Vector2 direction)
    {
        var animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
        }
    }

    /// <summary>
    /// Refresh danh sách spawn points (gọi từ Editor hoặc runtime nếu cần)
    /// </summary>
    [ContextMenu("Refresh Spawn Points")]
    public void RefreshSpawnPoints()
    {
        FindAllSpawnPoints();
    }

    // Hiển thị gizmos trong Scene view
    private void OnDrawGizmos()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
            return;

        foreach (var point in spawnPoints)
        {
            if (point == null) continue;

            // Vẽ line từ SpawnManager đến các spawn points
            Gizmos.color = point.IsDefaultSpawn ? Color.green : Color.yellow;
            Gizmos.DrawLine(transform.position, point.transform.position);
        }
    }
}

