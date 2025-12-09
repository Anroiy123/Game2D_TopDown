using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Editor tool để tự động tạo Animator Controller cho NPC từ sprite sheets
/// Menu: Tools > NPC Animator Generator
/// </summary>
public class NPCAnimatorGenerator : EditorWindow
{
    private const string CHARACTERS_PATH = "Assets/Sprites/Characters";
    private const int FRAME_RATE = 12;
    
    private Vector2 scrollPos;
    private Dictionary<string, bool> characterSelection = new Dictionary<string, bool>();
    private bool selectAll = false;

    [MenuItem("Tools/NPC Animator Generator")]
    public static void ShowWindow()
    {
        GetWindow<NPCAnimatorGenerator>("NPC Animator Generator");
    }

    private void OnEnable()
    {
        RefreshCharacterList();
    }

    private void RefreshCharacterList()
    {
        characterSelection.Clear();
        if (!Directory.Exists(CHARACTERS_PATH)) return;

        var directories = Directory.GetDirectories(CHARACTERS_PATH);
        foreach (var dir in directories)
        {
            string charName = Path.GetFileName(dir);
            // Bỏ qua các thư mục không phải nhân vật
            if (charName.StartsWith(".") || charName == "prototype_character") continue;
            characterSelection[charName] = false;
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("🎮 NPC Animator Generator", EditorStyles.boldLabel);
        GUILayout.Label("Tự động tạo Animation Clips và Animator Controller từ sprite sheets", EditorStyles.helpBox);
        
        EditorGUILayout.Space(10);
        
        // Select All toggle
        EditorGUI.BeginChangeCheck();
        selectAll = EditorGUILayout.Toggle("Chọn tất cả", selectAll);
        if (EditorGUI.EndChangeCheck())
        {
            var keys = characterSelection.Keys.ToList();
            foreach (var key in keys)
                characterSelection[key] = selectAll;
        }

        EditorGUILayout.Space(5);
        GUILayout.Label("Chọn nhân vật:", EditorStyles.boldLabel);

        // Character list
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
        var charKeys = characterSelection.Keys.ToList();
        foreach (var charName in charKeys)
        {
            characterSelection[charName] = EditorGUILayout.Toggle(charName, characterSelection[charName]);
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(10);

        // Buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🔄 Refresh", GUILayout.Height(30)))
        {
            RefreshCharacterList();
        }
        
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("⚡ Tạo Animator Controller", GUILayout.Height(30)))
        {
            GenerateSelectedAnimators();
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "Hướng dẫn:\n" +
            "1. Chọn nhân vật muốn tạo Animator\n" +
            "2. Click 'Tạo Animator Controller'\n" +
            "3. Animator sẽ được tạo trong thư mục của nhân vật đó\n\n" +
            "Yêu cầu sprite sheets: [Name]_idle.png, [Name]_walk.png",
            MessageType.Info);
    }

    private void GenerateSelectedAnimators()
    {
        var selected = characterSelection.Where(kv => kv.Value).Select(kv => kv.Key).ToList();
        if (selected.Count == 0)
        {
            EditorUtility.DisplayDialog("Thông báo", "Vui lòng chọn ít nhất 1 nhân vật!", "OK");
            return;
        }

        int success = 0;
        foreach (var charName in selected)
        {
            if (GenerateAnimatorForCharacter(charName))
                success++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("Hoàn thành", 
            $"Đã tạo thành công {success}/{selected.Count} Animator Controllers!", "OK");
    }

    private bool GenerateAnimatorForCharacter(string characterName)
    {
        string charPath = Path.Combine(CHARACTERS_PATH, characterName);
        if (!Directory.Exists(charPath))
        {
            Debug.LogError($"Không tìm thấy thư mục: {charPath}");
            return false;
        }

        Debug.Log($"[NPCAnimatorGenerator] Đang tạo animator cho: {characterName}");
        
        // Tìm sprite sheets
        var idleSprite = FindSpriteSheet(charPath, characterName, "idle");
        var walkSprite = FindSpriteSheet(charPath, characterName, "walk");
        
        if (idleSprite == null)
        {
            Debug.LogWarning($"[{characterName}] Không tìm thấy idle sprite sheet!");
            // Thử tìm sprite sheet chính
            idleSprite = FindSpriteSheet(charPath, characterName, "");
        }

        // Tạo Animation Clips
        var idleClip = CreateAnimationClip(charPath, characterName, "Idle", idleSprite);
        var walkClip = walkSprite != null ? 
            CreateAnimationClip(charPath, characterName, "Walk", walkSprite) : null;

        // Tạo Animator Controller
        return CreateAnimatorController(charPath, characterName, idleClip, walkClip);
    }

    private Sprite[] FindSpriteSheet(string charPath, string charName, string animType)
    {
        // Tìm theo nhiều pattern khác nhau
        string[] patterns = string.IsNullOrEmpty(animType)
            ? new[] { $"{charName}.png", $"{charName}_16x16.png" }
            : new[] {
                $"{charName}_{animType}.png",
                $"{charName}_{animType}_16x16.png",
                $"{charName.ToLower()}_{animType}.png"
            };

        foreach (var pattern in patterns)
        {
            string fullPath = Path.Combine(charPath, pattern);
            if (File.Exists(fullPath))
            {
                // Load all sprites from the sheet
                var sprites = AssetDatabase.LoadAllAssetsAtPath(fullPath)
                    .OfType<Sprite>()
                    .OrderBy(s => s.name)
                    .ToArray();

                if (sprites.Length > 0)
                {
                    Debug.Log($"[{charName}] Tìm thấy sprite sheet: {pattern} ({sprites.Length} sprites)");
                    return sprites;
                }
            }
        }
        return null;
    }

    private AnimationClip CreateAnimationClip(string charPath, string charName, string clipName, Sprite[] sprites)
    {
        if (sprites == null || sprites.Length == 0) return null;

        string clipPath = Path.Combine(charPath, $"{charName}_{clipName}.anim");

        // Kiểm tra nếu đã tồn tại
        var existingClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        if (existingClip != null)
        {
            Debug.Log($"[{charName}] Animation clip đã tồn tại: {clipName}");
            return existingClip;
        }

        var clip = new AnimationClip();
        clip.frameRate = FRAME_RATE;
        clip.name = $"{charName}_{clipName}";

        // Tạo keyframes
        var keyframes = new ObjectReferenceKeyframe[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            keyframes[i] = new ObjectReferenceKeyframe
            {
                time = i / (float)FRAME_RATE,
                value = sprites[i]
            };
        }

        // Tạo curve binding
        var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

        // Set loop
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        AssetDatabase.CreateAsset(clip, clipPath);
        Debug.Log($"[{charName}] Đã tạo animation clip: {clipName}");

        return clip;
    }

    private bool CreateAnimatorController(string charPath, string charName,
        AnimationClip idleClip, AnimationClip walkClip)
    {
        if (idleClip == null)
        {
            Debug.LogError($"[{charName}] Không có idle clip, không thể tạo controller!");
            return false;
        }

        string controllerPath = Path.Combine(charPath, $"{charName}.controller");

        // Kiểm tra nếu đã tồn tại
        if (AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath) != null)
        {
            Debug.Log($"[{charName}] Animator controller đã tồn tại, bỏ qua.");
            return true;
        }

        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        // Thêm parameters (tương tự player.controller)
        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("Horizontal", AnimatorControllerParameterType.Float);
        controller.AddParameter("Vertical", AnimatorControllerParameterType.Float);

        // Lấy root state machine
        var rootStateMachine = controller.layers[0].stateMachine;

        // Thêm Idle state (default)
        var idleState = rootStateMachine.AddState("Idle", new Vector3(300, 100, 0));
        idleState.motion = idleClip;
        rootStateMachine.defaultState = idleState;

        // Thêm Walk state nếu có
        if (walkClip != null)
        {
            var walkState = rootStateMachine.AddState("Walk", new Vector3(300, 250, 0));
            walkState.motion = walkClip;

            // Transition: Idle -> Walk (khi Speed > 0.01)
            var toWalk = idleState.AddTransition(walkState);
            toWalk.AddCondition(AnimatorConditionMode.Greater, 0.01f, "Speed");
            toWalk.hasExitTime = false;
            toWalk.duration = 0;

            // Transition: Walk -> Idle (khi Speed < 0.01)
            var toIdle = walkState.AddTransition(idleState);
            toIdle.AddCondition(AnimatorConditionMode.Less, 0.01f, "Speed");
            toIdle.hasExitTime = false;
            toIdle.duration = 0;
        }

        Debug.Log($"[{charName}] Đã tạo Animator Controller thành công!");
        return true;
    }
}
