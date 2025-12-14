using UnityEngine;

/// <summary>
/// SpawnPoint - Điểm spawn player khi vào scene
/// Mỗi scene có thể có nhiều spawn point với ID khác nhau
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Point Settings")]
    [Tooltip("ID duy nhất của spawn point này")]
    [SerializeField] private string spawnPointId;
    
    [Tooltip("Là spawn point mặc định của scene này")]
    [SerializeField] private bool isDefaultSpawn = false;
    
    [Tooltip("Hướng player nhìn khi spawn")]
    [SerializeField] private FacingDirection facingDirection = FacingDirection.Down;

    [Header("Match from Previous Scene")]
    [Tooltip("Spawn point này dành cho player đến từ scene nào")]
    [SerializeField] private string fromSceneName;

    public enum FacingDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public string SpawnPointId => spawnPointId;
    public bool IsDefaultSpawn => isDefaultSpawn;
    public FacingDirection Facing => facingDirection;
    public string FromSceneName => fromSceneName;

    /// <summary>
    /// Lấy vector direction từ enum
    /// </summary>
    public Vector2 GetFacingVector()
    {
        return facingDirection switch
        {
            FacingDirection.Up => Vector2.up,
            FacingDirection.Down => Vector2.down,
            FacingDirection.Left => Vector2.left,
            FacingDirection.Right => Vector2.right,
            _ => Vector2.down
        };
    }

    #region Editor Visualization
    private void OnDrawGizmos()
    {
        // Vẽ spawn point trong editor
        Gizmos.color = isDefaultSpawn ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        
        // Vẽ mũi tên chỉ hướng facing
        Vector3 direction = (Vector3)GetFacingVector() * 0.5f;
        Gizmos.DrawLine(transform.position, transform.position + direction);
        
        // Vẽ đầu mũi tên
        Vector3 arrowHead = transform.position + direction;
        Gizmos.DrawWireSphere(arrowHead, 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        // Hiển thị tên spawn point khi selected
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, 
            $"Spawn: {spawnPointId}\n{(isDefaultSpawn ? "(Default)" : "")}");
        #endif
    }
    #endregion
}

