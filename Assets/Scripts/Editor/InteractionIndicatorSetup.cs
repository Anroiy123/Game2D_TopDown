#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor tool để setup Interaction Indicator nhanh
/// </summary>
public class InteractionIndicatorSetup : EditorWindow
{
    [MenuItem("Tools/Interaction/Setup Indicator on Selected NPC")]
    public static void SetupIndicatorOnSelected()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Error", "Hãy chọn một GameObject (NPC) trước!", "OK");
            return;
        }

        // Add InteractionIndicator component
        InteractionIndicator indicator = selected.GetComponent<InteractionIndicator>();
        if (indicator == null)
        {
            indicator = selected.AddComponent<InteractionIndicator>();
            Debug.Log($"[Setup] Added InteractionIndicator to {selected.name}");
        }

        // Link to NPCInteraction if exists
        NPCInteraction npcInteraction = selected.GetComponent<NPCInteraction>();
        if (npcInteraction != null)
        {
            SerializedObject so = new SerializedObject(npcInteraction);
            SerializedProperty indicatorProp = so.FindProperty("interactionIndicator");
            if (indicatorProp != null)
            {
                indicatorProp.objectReferenceValue = indicator;
                so.ApplyModifiedProperties();
                Debug.Log($"[Setup] Linked InteractionIndicator to NPCInteraction");
            }
        }

        EditorUtility.DisplayDialog("Success", 
            $"✅ Setup InteractionIndicator on {selected.name}\n\n" +
            "Bước tiếp theo:\n" +
            "1. Kéo animation frames vào 'Animation Frames'\n" +
            "2. Điều chỉnh Offset (Y = 1.5 để hiện phía trên đầu)\n" +
            "3. Chọn Show Only When Near Player = true", 
            "OK");

        Selection.activeGameObject = selected;
    }

    [MenuItem("Tools/Interaction/Create Indicator Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<InteractionIndicatorSetup>("Indicator Setup");
    }

    private string prefabName = "InteractionIndicator_Talk";
    private Sprite[] animationFrames;
    private Vector2 scrollPosition;

    private void OnGUI()
    {
        GUILayout.Label("Tạo Interaction Indicator Prefabs", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "QUAN TRỌNG: Sử dụng PNG spritesheet đã slice, KHÔNG dùng GIF!\n\n" +
            "Spritesheet có sẵn:\n" +
            "- UI_thinking_emotes_animation_16x16.png (32 frames)\n\n" +
            "GIF files chỉ có 1 frame vì Unity không extract animation từ GIF.",
            MessageType.Warning);

        EditorGUILayout.Space();

        if (GUILayout.Button("1. Setup Thinking Emote (Talk) - PNG Spritesheet", GUILayout.Height(40)))
        {
            CreateIndicatorFromSpritesheet("InteractionIndicator_Talk",
                "Assets/Sprites/User_Interface_Elements/UI_thinking_emotes_animation_16x16.png",
                new Vector3(0f, 1.5f, 0f));
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Các loại khác (cần tạo spritesheet):", EditorStyles.boldLabel);

        if (GUILayout.Button("2. Setup Angry Emote (Warning) - Single Frame", GUILayout.Height(30)))
        {
            CreateIndicatorFromGIF("InteractionIndicator_Warning",
                "UI_angry_emote_16x16",
                new Vector3(0f, 1.5f, 0f));
        }

        if (GUILayout.Button("3. Setup Mail Icon (Quest) - Single Frame", GUILayout.Height(30)))
        {
            CreateIndicatorFromGIF("InteractionIndicator_Quest",
                "UI_mail_16x16",
                new Vector3(0f, 1.5f, 0f));
        }

        if (GUILayout.Button("4. Setup Arrow (Direction) - Single Frame", GUILayout.Height(30)))
        {
            CreateIndicatorFromGIF("InteractionIndicator_Direction",
                "UI_arrow_pointing_down_16x16",
                new Vector3(0f, 1.5f, 0f));
        }
    }

    private void CreateIndicatorFromSpritesheet(string prefabName, string spritesheetPath, Vector3 offset)
    {
        // Load all sprites from spritesheet
        Object[] allSprites = AssetDatabase.LoadAllAssetsAtPath(spritesheetPath);
        System.Collections.Generic.List<Sprite> frames = new System.Collections.Generic.List<Sprite>();

        foreach (Object obj in allSprites)
        {
            if (obj is Sprite sprite)
            {
                frames.Add(sprite);
            }
        }

        if (frames.Count == 0)
        {
            EditorUtility.DisplayDialog("Error",
                $"Không tìm thấy sprites trong:\n{spritesheetPath}\n\n" +
                "Hãy kiểm tra file đã được slice chưa.",
                "OK");
            return;
        }

        // Sort by name to ensure correct order
        frames.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));

        CreatePrefab(prefabName, frames.ToArray(), offset);
        EditorUtility.DisplayDialog("Success",
            $"✅ Tạo prefab: {prefabName}\n" +
            $"Số frames: {frames.Count}", "OK");
    }

    private void CreateIndicatorFromGIF(string prefabName, string spriteName, Vector3 offset)
    {
        // Tìm sprite từ GIF (chỉ có 1 frame)
        string[] guids = AssetDatabase.FindAssets($"t:Sprite {spriteName}");
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("Error",
                $"Không tìm thấy sprite: {spriteName}",
                "OK");
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

        if (sprite == null)
        {
            EditorUtility.DisplayDialog("Error", $"Không load được sprite từ: {path}", "OK");
            return;
        }

        CreatePrefab(prefabName, new Sprite[] { sprite }, offset);
        EditorUtility.DisplayDialog("Success",
            $"✅ Tạo prefab: {prefabName}\n" +
            "⚠️ Chỉ có 1 frame (static, không animation)", "OK");
    }

    private void CreatePrefab(string prefabName, Sprite[] frames, Vector3 offset)
    {
        // Tạo GameObject
        GameObject prefab = new GameObject(prefabName);
        InteractionIndicator indicator = prefab.AddComponent<InteractionIndicator>();

        // Setup qua SerializedObject
        SerializedObject so = new SerializedObject(indicator);
        so.FindProperty("animationFrames").arraySize = frames.Length;
        for (int i = 0; i < frames.Length; i++)
        {
            so.FindProperty("animationFrames").GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
        }
        so.FindProperty("offset").vector3Value = offset;
        so.ApplyModifiedProperties();

        // Save prefab
        string prefabPath = $"Assets/Prefabs/Indicators/{prefabName}.prefab";
        System.IO.Directory.CreateDirectory("Assets/Prefabs/Indicators");
        PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
        DestroyImmediate(prefab);

        Debug.Log($"[Setup] Created prefab: {prefabPath} with {frames.Length} frames");
    }
}
#endif

