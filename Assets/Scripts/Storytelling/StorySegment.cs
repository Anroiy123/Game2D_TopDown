using System;
using UnityEngine;

/// <summary>
/// Một đoạn trong storytelling sequence
/// Bao gồm background, text lines, illustration, audio
/// </summary>
[Serializable]
public class StorySegment
{
    [Header("Visual")]
    [Tooltip("Background cho đoạn này (null = giữ nguyên background trước)")]
    public Sprite backgroundImage;
    
    [Tooltip("Màu tint cho background")]
    public Color backgroundTint = Color.white;
    
    [Tooltip("Ảnh minh họa (optional, hiển thị ở giữa màn hình)")]
    public Sprite illustrationImage;
    
    [Tooltip("Vị trí ảnh minh họa")]
    public IllustrationPosition illustrationPosition = IllustrationPosition.Center;
    
    [Tooltip("Scale của ảnh minh họa (1.0 = kích thước gốc)")]
    [Range(0.1f, 3f)]
    public float illustrationScale = 1f;
    
    [Header("Text Content")]
    [Tooltip("Các dòng text hiển thị (mỗi dòng = 1 lần nhấn E/Space)")]
    [TextArea(2, 5)]
    public string[] textLines;
    
    [Tooltip("Tốc độ typewriter (0 = hiện ngay, > 0 = typewriter effect)")]
    [Range(0f, 0.1f)]
    public float typewriterSpeed = 0.03f;
    
    [Tooltip("Auto advance: tự động chuyển dòng sau thời gian này (0 = chờ input)")]
    public float autoAdvanceDelay = 0f;
    
    [Header("Transition")]
    [Tooltip("Fade to black trước khi hiển thị đoạn này")]
    public bool fadeToBlackBefore = false;
    
    [Tooltip("Thời gian delay trước khi hiển thị (giây)")]
    public float delayBefore = 0f;
    
    [Tooltip("Transition effect khi chuyển background")]
    public TransitionEffect backgroundTransition = TransitionEffect.Fade;
    
    [Header("Audio")]
    [Tooltip("BGM cho đoạn này (null = giữ nguyên BGM trước)")]
    public AudioClip bgm;
    
    [Tooltip("SFX phát khi bắt đầu đoạn này")]
    public AudioClip sfx;
    
    [Tooltip("Volume của BGM (0-1)")]
    [Range(0f, 1f)]
    public float bgmVolume = 0.5f;
    
    [Tooltip("Volume của SFX (0-1)")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    
    public enum IllustrationPosition
    {
        Center,
        Top,
        Bottom,
        Left,
        Right,
        Custom
    }
    
    public enum TransitionEffect
    {
        None,           // Chuyển ngay
        Fade,           // Fade in/out
        CrossFade       // Fade đồng thời (background cũ fade out, mới fade in)
    }
    
    [Header("Custom Position (chỉ dùng khi position = Custom)")]
    [Tooltip("Vị trí custom cho illustration (normalized: 0-1)")]
    public Vector2 customPosition = new Vector2(0.5f, 0.5f);
}

