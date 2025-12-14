using UnityEngine;

/// <summary>
/// Di chuyển NPC đến các vị trí khác nhau dựa trên story flags.
/// Gắn script này vào NPC GameObject.
/// </summary>
public class NPCPositionByFlag : MonoBehaviour
{
    [System.Serializable]
    public class PositionCondition
    {
        [Tooltip("Tên flag cần kiểm tra")]
        public string flagName;
        
        [Tooltip("Giá trị flag cần có (true/false)")]
        public bool flagValue = true;
        
        [Tooltip("Vị trí NPC sẽ di chuyển đến khi điều kiện thỏa mãn")]
        public Transform targetPosition;
        
        [Tooltip("Ẩn NPC thay vì di chuyển (nếu true)")]
        public bool hideInstead = false;
        
        [Tooltip("Độ ưu tiên - số cao hơn được kiểm tra trước")]
        public int priority = 0;
    }

    [Header("Position Conditions")]
    [SerializeField] private PositionCondition[] conditions;

    [Header("Default Settings")]
    [SerializeField] private Transform defaultPosition;
    [SerializeField] private bool hideByDefault = false;

    private Vector3 initialPosition;
    private bool initialActiveState;

    private void Awake()
    {
        // Lưu vị trí ban đầu
        initialPosition = transform.position;
        initialActiveState = gameObject.activeSelf;
    }

    private void Start()
    {
        // Kiểm tra và cập nhật vị trí khi scene load
        UpdatePosition();
        
        // Subscribe vào StoryManager để cập nhật khi flag thay đổi
        if (StoryManager.Instance != null)
        {
            StoryManager.Instance.OnFlagChanged += OnFlagChanged;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe
        if (StoryManager.Instance != null)
        {
            StoryManager.Instance.OnFlagChanged -= OnFlagChanged;
        }
    }

    private void OnFlagChanged(string flagName, bool value)
    {
        // Kiểm tra xem flag này có ảnh hưởng đến NPC không
        foreach (var condition in conditions)
        {
            if (condition.flagName == flagName)
            {
                UpdatePosition();
                return;
            }
        }
    }

    /// <summary>
    /// Cập nhật vị trí NPC dựa trên các điều kiện flag
    /// </summary>
    public void UpdatePosition()
    {
        if (StoryManager.Instance == null)
        {
            Debug.LogWarning($"[NPCPositionByFlag] StoryManager not found for {gameObject.name}");
            return;
        }

        // Sắp xếp theo priority (cao -> thấp)
        var sortedConditions = new System.Collections.Generic.List<PositionCondition>(conditions);
        sortedConditions.Sort((a, b) => b.priority.CompareTo(a.priority));

        // Kiểm tra từng điều kiện
        foreach (var condition in sortedConditions)
        {
            bool currentFlagValue = StoryManager.Instance.GetFlag(condition.flagName);
            
            if (currentFlagValue == condition.flagValue)
            {
                // Điều kiện thỏa mãn
                if (condition.hideInstead)
                {
                    gameObject.SetActive(false);
                    Debug.Log($"[NPCPositionByFlag] {gameObject.name} hidden due to flag '{condition.flagName}'");
                }
                else if (condition.targetPosition != null)
                {
                    gameObject.SetActive(true);
                    transform.position = condition.targetPosition.position;
                    Debug.Log($"[NPCPositionByFlag] {gameObject.name} moved to {condition.targetPosition.name} due to flag '{condition.flagName}'");
                }
                return; // Dừng sau khi tìm thấy điều kiện đầu tiên thỏa mãn
            }
        }

        // Không có điều kiện nào thỏa mãn - dùng default
        if (hideByDefault)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            if (defaultPosition != null)
            {
                transform.position = defaultPosition.position;
            }
            else
            {
                transform.position = initialPosition;
            }
        }
    }

    /// <summary>
    /// Reset về vị trí ban đầu
    /// </summary>
    public void ResetToInitial()
    {
        transform.position = initialPosition;
        gameObject.SetActive(initialActiveState);
    }
}
