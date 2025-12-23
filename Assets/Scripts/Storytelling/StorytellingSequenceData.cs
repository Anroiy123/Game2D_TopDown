using System;
using UnityEngine;

/// <summary>
/// ScriptableObject chứa một sequence storytelling hoàn chỉnh
/// Dùng cho endings, cutscenes, intro, etc.
/// </summary>
[CreateAssetMenu(fileName = "NewStorytellingSequence", menuName = "Storytelling/Sequence Data")]
public class StorytellingSequenceData : ScriptableObject
{
    [Header("Sequence Info")]
    [Tooltip("Tên sequence (VD: 'Ending 1 - Good StandUp')")]
    public string sequenceName;
    
    [Tooltip("Mô tả sequence")]
    [TextArea(2, 4)]
    public string description;
    
    [Header("Story Segments")]
    [Tooltip("Các đoạn story theo thứ tự")]
    public StorySegment[] segments;
    
    [Header("Ending Settings")]
    [Tooltip("Ending type (để set vào StoryManager)")]
    public StoryManager.EndingType endingType = StoryManager.EndingType.None;
    
    [Tooltip("Flags set khi bắt đầu sequence")]
    public string[] setFlagsOnStart;
    
    [Tooltip("Flags set khi kết thúc sequence")]
    public string[] setFlagsOnComplete;
    
    [Tooltip("Variables thay đổi khi bắt đầu sequence")]
    public VariableChange[] variableChangesOnStart;
    
    [Tooltip("Variables thay đổi khi kết thúc sequence")]
    public VariableChange[] variableChangesOnComplete;
    
    [Header("After Sequence")]
    [Tooltip("Scene để load sau khi xong (VD: 'MainMenu', 'Credits')")]
    public string nextSceneName = "MainMenu";
    
    [Tooltip("Delay trước khi load scene tiếp theo (giây)")]
    public float delayBeforeNextScene = 2f;
    
    [Header("Skip Settings")]
    [Tooltip("Cho phép skip sequence")]
    public bool allowSkip = true;
    
    [Tooltip("Phím skip")]
    public KeyCode skipKey = KeyCode.Escape;
    
    [Tooltip("Text hiển thị khi có thể skip")]
    public string skipHintText = "Nhấn ESC để bỏ qua";
    
    [Header("Transition Settings")]
    [Tooltip("Thời gian fade giữa các segment (giây)")]
    [Range(0.1f, 3f)]
    public float segmentFadeDuration = 0.5f;
    
    /// <summary>
    /// Apply effects khi bắt đầu sequence
    /// </summary>
    public void ApplyOnStartEffects()
    {
        if (StoryManager.Instance == null) return;
        
        // Set flags
        if (setFlagsOnStart != null)
        {
            foreach (string flag in setFlagsOnStart)
            {
                if (!string.IsNullOrEmpty(flag))
                {
                    StoryManager.Instance.SetFlag(flag, true);
                }
            }
        }
        
        // Change variables
        if (variableChangesOnStart != null)
        {
            foreach (var change in variableChangesOnStart)
            {
                change.Apply();
            }
        }
        
        // Set ending type
        if (endingType != StoryManager.EndingType.None)
        {
            StoryManager.Instance.TriggerEnding(endingType);
        }
    }
    
    /// <summary>
    /// Apply effects khi kết thúc sequence
    /// </summary>
    public void ApplyOnCompleteEffects()
    {
        if (StoryManager.Instance == null) return;
        
        // Set flags
        if (setFlagsOnComplete != null)
        {
            foreach (string flag in setFlagsOnComplete)
            {
                if (!string.IsNullOrEmpty(flag))
                {
                    StoryManager.Instance.SetFlag(flag, true);
                }
            }
        }
        
        // Change variables
        if (variableChangesOnComplete != null)
        {
            foreach (var change in variableChangesOnComplete)
            {
                change.Apply();
            }
        }
    }
}

