using UnityEngine;

/// <summary>
/// Quick Flag Setter - Set story flags trước khi Play để test
/// Đặt component này vào scene, nó sẽ tự động set flags khi Start()
/// </summary>
public class QuickFlagSetter : MonoBehaviour
{
    [Header("Flags to Set on Start")]
    [Tooltip("Danh sách flags sẽ được set TRUE khi scene load")]
    [SerializeField] private string[] flagsToSetTrue = new string[]
    {
        // Ví dụ: Uncomment dòng nào cần test
        // "ran_from_bullies",
        // "confronted_bullies",
        // "day1_scene1_completed",
        // "day1_scene2_completed",
    };

    [SerializeField] private string[] flagsToSetFalse = new string[]
    {
        // Flags cần set FALSE
    };

    [Header("Variables to Set")]
    [SerializeField] private VariableSetting[] variablesToSet = new VariableSetting[0];

    [Header("Settings")]
    [Tooltip("Chỉ chạy trong Editor, không chạy trong build")]
    [SerializeField] private bool editorOnly = true;

    [Tooltip("Hiển thị log khi set flags")]
    [SerializeField] private bool showLogs = true;

    [Tooltip("Hiển thị UI thông báo")]
    [SerializeField] private bool showUI = true;

    private void Start()
    {
        // Skip nếu đang build và editorOnly = true
        #if !UNITY_EDITOR
        if (editorOnly) return;
        #endif

        if (StoryManager.Instance == null)
        {
            Debug.LogWarning("[QuickFlagSetter] StoryManager not found!");
            return;
        }

        // Set flags TRUE
        foreach (string flag in flagsToSetTrue)
        {
            if (!string.IsNullOrEmpty(flag))
            {
                StoryManager.Instance.SetFlag(flag, true);
                if (showLogs)
                {
                    Debug.Log($"[QuickFlagSetter] Set flag TRUE: {flag}");
                }
            }
        }

        // Set flags FALSE
        foreach (string flag in flagsToSetFalse)
        {
            if (!string.IsNullOrEmpty(flag))
            {
                StoryManager.Instance.SetFlag(flag, false);
                if (showLogs)
                {
                    Debug.Log($"[QuickFlagSetter] Set flag FALSE: {flag}");
                }
            }
        }

        // Set variables
        foreach (var varSetting in variablesToSet)
        {
            if (!string.IsNullOrEmpty(varSetting.variableName))
            {
                StoryManager.Instance.SetVariable(varSetting.variableName, varSetting.value);
                if (showLogs)
                {
                    Debug.Log($"[QuickFlagSetter] Set variable: {varSetting.variableName} = {varSetting.value}");
                }
            }
        }

        if (showLogs)
        {
            Debug.Log($"[QuickFlagSetter] ✅ Set {flagsToSetTrue.Length} flags TRUE, {flagsToSetFalse.Length} flags FALSE, {variablesToSet.Length} variables");
        }
    }

    private void OnGUI()
    {
        if (!showUI) return;

        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            normal = { textColor = Color.green }
        };

        string message = $"[QuickFlagSetter] Active - {flagsToSetTrue.Length} flags set";
        GUI.Label(new Rect(10, 90, 500, 30), message, style);
    }

    [System.Serializable]
    public class VariableSetting
    {
        public string variableName;
        public int value;
    }
}

