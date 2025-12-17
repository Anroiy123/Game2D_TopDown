using System;
using UnityEngine;

/// <summary>
/// ScriptableObject chứa data cho một sequence VN
/// </summary>
[CreateAssetMenu(fileName = "NewVNScene", menuName = "Visual Novel/VN Scene Data")]
public class VNSceneData : ScriptableObject
{
    [Header("Scene Data")]
    public VNScene sceneData;

    [Header("Conditions (Optional)")]
    [Tooltip("Flags cần có để hiển thị cảnh này")]
    public string[] requiredFlags;

    [Tooltip("Flags không được có")]
    public string[] forbiddenFlags;

    [Header("Effects On Enter")]
    [Tooltip("Flags set TRUE khi vào cảnh")]
    public string[] setFlagsOnEnter;

    [Tooltip("Biến thay đổi khi vào cảnh")]
    public VariableChange[] variableChangesOnEnter;

    [Header("Effects On Complete")]
    [Tooltip("Flags set TRUE khi hoàn thành cảnh (sau khi dialogue kết thúc)")]
    public string[] setFlagsOnComplete;

    [Tooltip("Biến thay đổi khi hoàn thành cảnh")]
    public VariableChange[] variableChangesOnComplete;

    /// <summary>
    /// Kiểm tra điều kiện hiển thị cảnh
    /// </summary>
    public bool CanShow()
    {
        if (StoryManager.Instance == null) return true;
        return StoryManager.Instance.CheckRequiredFlags(requiredFlags) &&
               StoryManager.Instance.CheckForbiddenFlags(forbiddenFlags);
    }

    /// <summary>
    /// Áp dụng effects khi vào cảnh
    /// </summary>
    public void ApplyOnEnterEffects()
    {
        if (StoryManager.Instance == null) return;

        if (setFlagsOnEnter != null)
        {
            foreach (string flag in setFlagsOnEnter)
            {
                StoryManager.Instance.SetFlag(flag, true);
            }
        }

        if (variableChangesOnEnter != null)
        {
            foreach (var change in variableChangesOnEnter)
            {
                change.Apply();
            }
        }
    }

    /// <summary>
    /// Áp dụng effects khi hoàn thành cảnh
    /// </summary>
    public void ApplyOnCompleteEffects()
    {
        if (StoryManager.Instance == null) return;

        if (setFlagsOnComplete != null)
        {
            foreach (string flag in setFlagsOnComplete)
            {
                StoryManager.Instance.SetFlag(flag, true);
                Debug.Log($"[VNSceneData] Set flag on complete: {flag}");
            }
        }

        if (variableChangesOnComplete != null)
        {
            foreach (var change in variableChangesOnComplete)
            {
                change.Apply();
            }
        }
    }
}

