using UnityEngine;

/// <summary>
/// VNSequenceData - Chứa một chuỗi các cảnh VN
/// Dùng cho các đoạn story dài như Day 1, Day 7, etc.
/// </summary>
[CreateAssetMenu(fileName = "NewVNSequence", menuName = "Visual Novel/VN Sequence")]
public class VNSequenceData : ScriptableObject
{
    [Header("Sequence Info")]
    [Tooltip("Tên sequence (VD: 'Day 1 - Morning')")]
    public string sequenceName;

    [Tooltip("Mô tả sequence")]
    [TextArea(2, 4)]
    public string description;

    [Header("Scenes")]
    [Tooltip("Các cảnh trong sequence (theo thứ tự)")]
    public VNSceneData[] scenes;

    [Header("Transition Settings")]
    [Tooltip("Thời gian fade giữa các cảnh")]
    public float sceneFadeDuration = 0.5f;

    [Header("Story Integration")]
    [Tooltip("Day number (0 = không set)")]
    public int dayNumber = 0;

    [Tooltip("Time of day")]
    public TimeOfDay timeOfDay = TimeOfDay.Morning;

    [Header("Conditions")]
    [Tooltip("Flags cần có để chơi sequence này")]
    public string[] requiredFlags;

    [Tooltip("Flags không được có")]
    public string[] forbiddenFlags;

    [Header("Effects On Complete")]
    [Tooltip("Flags set TRUE khi hoàn thành sequence")]
    public string[] setFlagsOnComplete;

    [Tooltip("Biến thay đổi khi hoàn thành")]
    public VariableChange[] variableChangesOnComplete;

    public enum TimeOfDay
    {
        Morning,
        Noon,
        Afternoon,
        Evening,
        Night
    }

    /// <summary>
    /// Kiểm tra điều kiện chơi sequence
    /// </summary>
    public bool CanPlay()
    {
        if (StoryManager.Instance == null) return true;
        return StoryManager.Instance.CheckRequiredFlags(requiredFlags) &&
               StoryManager.Instance.CheckForbiddenFlags(forbiddenFlags);
    }

    /// <summary>
    /// Áp dụng effects khi hoàn thành sequence
    /// </summary>
    public void ApplyOnCompleteEffects()
    {
        if (StoryManager.Instance == null) return;

        if (setFlagsOnComplete != null)
        {
            foreach (string flag in setFlagsOnComplete)
            {
                StoryManager.Instance.SetFlag(flag, true);
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

    /// <summary>
    /// Lấy cảnh đầu tiên có thể chơi
    /// </summary>
    public VNSceneData GetFirstPlayableScene()
    {
        if (scenes == null || scenes.Length == 0) return null;

        foreach (var scene in scenes)
        {
            if (scene != null && scene.CanShow())
            {
                return scene;
            }
        }

        return scenes[0];
    }
}

/// <summary>
/// VN Sequence Manager - Quản lý chơi các sequence
/// </summary>
public static class VNSequenceManager
{
    private static VNSequenceData currentSequence;
    private static int currentSceneIndex;
    private static System.Action onSequenceComplete;

    /// <summary>
    /// Bắt đầu chơi một sequence
    /// </summary>
    public static void PlaySequence(VNSequenceData sequence, System.Action onComplete = null)
    {
        if (sequence == null || !sequence.CanPlay())
        {
            Debug.LogWarning("[VNSequenceManager] Sequence is null or conditions not met");
            onComplete?.Invoke();
            return;
        }

        currentSequence = sequence;
        currentSceneIndex = 0;
        onSequenceComplete = onComplete;

        PlayCurrentScene();
    }

    private static void PlayCurrentScene()
    {
        if (currentSequence == null || currentSequence.scenes == null)
        {
            EndSequence();
            return;
        }

        if (currentSceneIndex >= currentSequence.scenes.Length)
        {
            EndSequence();
            return;
        }

        var scene = currentSequence.scenes[currentSceneIndex];
        if (scene != null && scene.CanShow())
        {
            VisualNovelManager.Instance.StartVNScene(scene, OnSceneComplete);
        }
        else
        {
            // Skip scene if conditions not met
            currentSceneIndex++;
            PlayCurrentScene();
        }
    }

    private static void OnSceneComplete()
    {
        currentSceneIndex++;
        PlayCurrentScene();
    }

    private static void EndSequence()
    {
        currentSequence?.ApplyOnCompleteEffects();
        onSequenceComplete?.Invoke();

        currentSequence = null;
        currentSceneIndex = 0;
        onSequenceComplete = null;
    }
}

