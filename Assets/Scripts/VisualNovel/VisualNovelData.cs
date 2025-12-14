using System;
using UnityEngine;

/// <summary>
/// Dữ liệu cho một cảnh visual novel
/// Bao gồm background, nhân vật hiển thị, và dialogue
/// </summary>
[Serializable]
public class VNScene
{
    [Header("Scene Info")]
    [Tooltip("Tên cảnh (để debug)")]
    public string sceneName;

    [Tooltip("Mô tả địa điểm hiển thị trên màn hình (VD: 'Phòng ngủ Đức')")]
    public string locationText;

    [Header("Background")]
    [Tooltip("Ảnh background cho cảnh này")]
    public Sprite backgroundImage;

    [Tooltip("Màu overlay trên background (để tạo mood)")]
    public Color backgroundTint = Color.white;

    [Header("Characters On Screen")]
    [Tooltip("Các nhân vật hiển thị trong cảnh này")]
    public VNCharacterDisplay[] characters;

    [Header("Dialogue")]
    [Tooltip("DialogueData cho cảnh này")]
    public DialogueData dialogue;

    [Header("Audio")]
    [Tooltip("Nhạc nền cho cảnh")]
    public AudioClip bgm;

    [Tooltip("Ambient sound")]
    public AudioClip ambience;

    [Header("Transition")]
    [Tooltip("Cảnh tiếp theo (null = kết thúc VN mode)")]
    public VNSceneData nextScene;

    [Tooltip("Chuyển về top-down mode sau cảnh này")]
    public bool returnToTopDown = false;

    [Tooltip("Scene (Unity) để load khi chuyển về top-down")]
    public string topDownSceneName;

    [Tooltip("SpawnPoint ID khi chuyển về top-down")]
    public string spawnPointId;
}

/// <summary>
/// Hiển thị một nhân vật trong cảnh VN
/// </summary>
[Serializable]
public class VNCharacterDisplay
{
    [Tooltip("Sprite của nhân vật")]
    public Sprite characterSprite;

    [Tooltip("Tên nhân vật (để matching với dialogue speaker)")]
    public string characterName;

    [Tooltip("Vị trí trên màn hình")]
    public CharacterPosition position = CharacterPosition.Center;

    [Tooltip("Offset tùy chỉnh từ vị trí mặc định")]
    public Vector2 positionOffset;

    [Tooltip("Scale của nhân vật")]
    public Vector2 scale = Vector2.one;

    [Tooltip("Flip horizontal")]
    public bool flipX = false;

    public enum CharacterPosition
    {
        Left,
        Center,
        Right,
        FarLeft,
        FarRight,
        Custom
    }
}

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
}

