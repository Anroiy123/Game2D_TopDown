using System;
using UnityEngine;

/// <summary>
/// Đại diện cho một lựa chọn trong dialogue
/// Hỗ trợ conditional choices và story flag manipulation
/// </summary>
[Serializable]
public class DialogueChoice
{
    [Header("Basic Settings")]
    [Tooltip("Nội dung lựa chọn hiển thị cho player")]
    public string choiceText;

    [Tooltip("ID của DialogueNode sẽ chuyển đến khi chọn")]
    public int nextNodeId;

    [Tooltip("Callback ID để trigger event đặc biệt (optional)")]
    public string actionId;

    [Header("VN Scene Transition")]
    [Tooltip("VN Scene sẽ chuyển đến khi chọn (thay thế actionId cho VN transitions)")]
    public VNSceneData nextVNScene;

    [Tooltip("Kết thúc VN mode sau khi chọn (dùng khi không có nextVNScene)")]
    public bool endVNMode = false;

    [Header("Top-Down Scene Transition")]
    [Tooltip("Load scene top-down trực tiếp (không qua VN). Ưu tiên thấp hơn nextVNScene.")]
    public string loadTopDownScene;

    [Tooltip("SpawnPoint ID khi load scene top-down")]
    public string topDownSpawnPointId;

    [Header("Conditions (Khi nào hiển thị choice này)")]
    [Tooltip("Flags cần có để hiển thị choice này (tất cả phải true)")]
    public string[] requiredFlags;

    [Tooltip("Flags không được có (nếu có bất kỳ flag nào thì ẩn choice)")]
    public string[] forbiddenFlags;

    [Tooltip("Biến cần đạt giá trị tối thiểu (VD: money >= 10000)")]
    public VariableCondition[] variableConditions;

    [Header("Effects (Khi chọn choice này)")]
    [Tooltip("Flags sẽ được set thành TRUE khi chọn")]
    public string[] setFlagsTrue;

    [Tooltip("Flags sẽ được set thành FALSE khi chọn")]
    public string[] setFlagsFalse;

    [Tooltip("Thay đổi giá trị biến (VD: money -10000)")]
    public VariableChange[] variableChanges;

    /// <summary>
    /// Kiểm tra xem choice này có thể hiển thị không (dựa trên conditions)
    /// </summary>
    public bool CanShow()
    {
        if (StoryManager.Instance == null) return true;

        // Check required flags
        if (!StoryManager.Instance.CheckRequiredFlags(requiredFlags))
            return false;

        // Check forbidden flags
        if (!StoryManager.Instance.CheckForbiddenFlags(forbiddenFlags))
            return false;

        // Check variable conditions
        if (variableConditions != null)
        {
            foreach (var condition in variableConditions)
            {
                if (!condition.IsMet()) return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Áp dụng effects khi player chọn choice này
    /// </summary>
    public void ApplyEffects()
    {
        if (StoryManager.Instance == null) return;

        // Set flags true
        if (setFlagsTrue != null)
        {
            foreach (string flag in setFlagsTrue)
            {
                StoryManager.Instance.SetFlag(flag, true);
            }
        }

        // Set flags false
        if (setFlagsFalse != null)
        {
            foreach (string flag in setFlagsFalse)
            {
                StoryManager.Instance.SetFlag(flag, false);
            }
        }

        // Apply variable changes
        if (variableChanges != null)
        {
            foreach (var change in variableChanges)
            {
                change.Apply();
            }
        }
    }
}

/// <summary>
/// Điều kiện kiểm tra biến (VD: money >= 10000)
/// </summary>
[Serializable]
public class VariableCondition
{
    public string variableName;
    public ComparisonOperator comparison;
    public int value;

    public enum ComparisonOperator
    {
        GreaterThan,        // >
        GreaterOrEqual,     // >=
        LessThan,           // <
        LessOrEqual,        // <=
        Equal,              // ==
        NotEqual            // !=
    }

    public bool IsMet()
    {
        if (StoryManager.Instance == null) return true;

        int currentValue = StoryManager.Instance.GetVariable(variableName);

        return comparison switch
        {
            ComparisonOperator.GreaterThan => currentValue > value,
            ComparisonOperator.GreaterOrEqual => currentValue >= value,
            ComparisonOperator.LessThan => currentValue < value,
            ComparisonOperator.LessOrEqual => currentValue <= value,
            ComparisonOperator.Equal => currentValue == value,
            ComparisonOperator.NotEqual => currentValue != value,
            _ => true
        };
    }
}

/// <summary>
/// Thay đổi giá trị biến (VD: money -10000, fear_level +20)
/// </summary>
[Serializable]
public class VariableChange
{
    public string variableName;
    public ChangeOperation operation;
    public int value;

    public enum ChangeOperation
    {
        Set,    // = value
        Add,    // += value
        Subtract // -= value
    }

    public void Apply()
    {
        if (StoryManager.Instance == null) return;

        switch (operation)
        {
            case ChangeOperation.Set:
                StoryManager.Instance.SetVariable(variableName, value);
                break;
            case ChangeOperation.Add:
                StoryManager.Instance.AddVariable(variableName, value);
                break;
            case ChangeOperation.Subtract:
                StoryManager.Instance.AddVariable(variableName, -value);
                break;
        }
    }
}

/// <summary>
/// Một node trong dialogue tree
/// Hỗ trợ conditions, effects, và visual/audio
/// </summary>
[Serializable]
public class DialogueNode
{
    [Header("Basic Settings")]
    [Tooltip("ID duy nhất của node này")]
    public int nodeId;

    [Tooltip("Tên người nói (NPC hoặc Player)")]
    public string speakerName;

    [Tooltip("Đây có phải là lời thoại của Player không")]
    public bool isPlayerSpeaking;

    [Tooltip("Nội dung dialogue (có thể có nhiều dòng)")]
    [TextArea(2, 5)]
    public string[] dialogueLines;

    [Tooltip("Các lựa chọn cho player (để trống nếu auto tiếp tục)")]
    public DialogueChoice[] choices;

    [Tooltip("ID của node tiếp theo (dùng khi không có choices, -1 = kết thúc)")]
    public int nextNodeId = -1;

    [Header("VN Scene Transition")]
    [Tooltip("VN Scene sẽ chuyển đến sau khi node này kết thúc (ưu tiên cao hơn nextNodeId)")]
    public VNSceneData nextVNScene;

    [Header("Top-Down Scene Transition")]
    [Tooltip("Load scene top-down sau khi node kết thúc (ưu tiên thấp hơn nextVNScene)")]
    public string loadTopDownScene;

    [Tooltip("SpawnPoint ID khi load scene top-down")]
    public string topDownSpawnPointId;

    [Header("On Enter Effects")]
    [Tooltip("Flags sẽ được set TRUE khi vào node này")]
    public string[] setFlagsOnEnter;

    [Tooltip("Thay đổi biến khi vào node này")]
    public VariableChange[] variableChangesOnEnter;

    [Header("Visual (Optional)")]
    [Tooltip("Portrait/Avatar của người nói")]
    public Sprite speakerPortrait;

    [Tooltip("Emotion/Expression của người nói")]
    public string emotion;

    [Header("Audio (Optional)")]
    [Tooltip("Audio clip phát khi vào node")]
    public AudioClip voiceClip;

    [Tooltip("Sound effect khi vào node")]
    public AudioClip sfxClip;

    /// <summary>
    /// Áp dụng effects khi vào node này
    /// </summary>
    public void ApplyOnEnterEffects()
    {
        if (StoryManager.Instance == null) return;

        // Set flags on enter
        if (setFlagsOnEnter != null)
        {
            foreach (string flag in setFlagsOnEnter)
            {
                StoryManager.Instance.SetFlag(flag, true);
            }
        }

        // Apply variable changes
        if (variableChangesOnEnter != null)
        {
            foreach (var change in variableChangesOnEnter)
            {
                change.Apply();
            }
        }
    }

    /// <summary>
    /// Lấy danh sách choices có thể hiển thị (đã filter theo conditions)
    /// </summary>
    public DialogueChoice[] GetAvailableChoices()
    {
        if (choices == null || choices.Length == 0) return choices;

        var availableChoices = new System.Collections.Generic.List<DialogueChoice>();
        foreach (var choice in choices)
        {
            if (choice.CanShow())
            {
                availableChoices.Add(choice);
            }
        }
        return availableChoices.ToArray();
    }
}

/// <summary>
/// ScriptableObject chứa toàn bộ dialogue conversation
/// </summary>
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Tooltip("Tên của conversation này")]
    public string conversationName;

    [Tooltip("ID của node bắt đầu")]
    public int startNodeId = 0;

    [Tooltip("Danh sách tất cả dialogue nodes")]
    public DialogueNode[] nodes;

    /// <summary>
    /// Tìm node theo ID
    /// </summary>
    public DialogueNode GetNodeById(int nodeId)
    {
        foreach (var node in nodes)
        {
            if (node.nodeId == nodeId)
                return node;
        }
        return null;
    }
}

/// <summary>
/// Entry chứa DialogueData với điều kiện để trigger
/// Dùng cho NPC có nhiều dialogue khác nhau tùy theo scene/flags/variables
/// </summary>
[Serializable]
public class ConditionalDialogueEntry
{
    [Header("Dialogue")]
    [Tooltip("DialogueData sẽ được sử dụng nếu điều kiện thỏa mãn")]
    public DialogueData dialogueData;

    [Header("Trigger Mode")]
    [Tooltip("Tự động trigger dialogue khi scene bắt đầu (không cần player đến gần)")]
    public bool triggerOnSceneStart = false;

    [Tooltip("Tự động trigger dialogue khi flag này được set TRUE")]
    public string triggerOnFlagSet = "";

    [Tooltip("Delay trước khi auto trigger (giây). Dùng cho cả triggerOnSceneStart và triggerOnFlagSet")]
    public float autoTriggerDelay = 0.5f;

    [Tooltip("Spawn Point ID cụ thể để trigger (để trống = trigger bất kể spawn point nào). Chỉ dùng với triggerOnSceneStart")]
    public string requiredSpawnPointId = "";

    [Header("Scene Conditions")]
    [Tooltip("Chỉ trigger trong các scene này (để trống = mọi scene)")]
    public string[] allowedScenes;

    [Header("Flag Conditions")]
    [Tooltip("Flags cần có để trigger dialogue này (tất cả phải true)")]
    public string[] requiredFlags;

    [Tooltip("Flags không được có (nếu có bất kỳ flag nào thì không trigger)")]
    public string[] forbiddenFlags;

    [Header("Variable Conditions")]
    [Tooltip("Điều kiện biến (VD: current_day == 8)")]
    public VariableCondition[] variableConditions;

    [Header("Effects On Complete")]
    [Tooltip("Flags sẽ được set TRUE khi dialogue kết thúc")]
    public string[] setFlagsOnComplete;

    [Tooltip("Thay đổi biến khi dialogue kết thúc")]
    public VariableChange[] variableChangesOnComplete;

    [Header("Priority")]
    [Tooltip("Độ ưu tiên (cao hơn = kiểm tra trước). Mặc định = 0")]
    public int priority = 0;

    /// <summary>
    /// Kiểm tra xem entry này có thể được sử dụng không
    /// </summary>
    public bool CanUse()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Check scene condition
        if (allowedScenes != null && allowedScenes.Length > 0)
        {
            bool sceneMatch = false;
            foreach (string scene in allowedScenes)
            {
                if (string.Equals(scene, currentScene, StringComparison.OrdinalIgnoreCase))
                {
                    sceneMatch = true;
                    break;
                }
            }
            if (!sceneMatch) return false;
        }

        // Check story flags
        if (StoryManager.Instance != null)
        {
            if (!StoryManager.Instance.CheckRequiredFlags(requiredFlags))
                return false;

            if (!StoryManager.Instance.CheckForbiddenFlags(forbiddenFlags))
                return false;

            if (variableConditions != null)
            {
                foreach (var condition in variableConditions)
                {
                    if (!condition.IsMet()) return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Áp dụng effects khi dialogue kết thúc
    /// </summary>
    public void ApplyOnCompleteEffects()
    {
        if (StoryManager.Instance == null) return;

        // Set flags on complete
        if (setFlagsOnComplete != null)
        {
            foreach (string flag in setFlagsOnComplete)
            {
                if (!string.IsNullOrEmpty(flag))
                {
                    StoryManager.Instance.SetFlag(flag, true);
                    Debug.Log($"[ConditionalDialogue] Set flag on complete: {flag}");
                }
            }
        }

        // Apply variable changes
        if (variableChangesOnComplete != null)
        {
            foreach (var change in variableChangesOnComplete)
            {
                change.Apply();
                Debug.Log($"[ConditionalDialogue] Applied variable change on complete: {change.variableName}");
            }
        }
    }
}
