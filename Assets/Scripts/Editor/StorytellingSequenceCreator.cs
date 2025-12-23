#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor tool để tạo Storytelling Sequence nhanh
/// </summary>
public class StorytellingSequenceCreator : EditorWindow
{
    private string sequenceName = "NewEnding";
    private string description = "";
    private StoryManager.EndingType endingType = StoryManager.EndingType.None;
    private int segmentCount = 3;
    private string savePath = "Assets/Data/Storytelling/";
    private string nextSceneName = "MainMenu";

    [MenuItem("Tools/Storytelling/Create Sequence")]
    public static void ShowWindow()
    {
        GetWindow<StorytellingSequenceCreator>("Storytelling Sequence Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Storytelling Sequence Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        sequenceName = EditorGUILayout.TextField("Sequence Name", sequenceName);
        description = EditorGUILayout.TextField("Description", description);
        endingType = (StoryManager.EndingType)EditorGUILayout.EnumPopup("Ending Type", endingType);
        segmentCount = EditorGUILayout.IntSlider("Number of Segments", segmentCount, 1, 20);
        nextSceneName = EditorGUILayout.TextField("Next Scene", nextSceneName);

        EditorGUILayout.Space();
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Sequence", GUILayout.Height(40)))
        {
            CreateSequence();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Tạo một StorytellingSequenceData với số lượng segments đã chọn.\n" +
            "Sau khi tạo, bạn có thể chỉnh sửa từng segment trong Inspector.",
            MessageType.Info
        );
    }

    private void CreateSequence()
    {
        if (string.IsNullOrEmpty(sequenceName))
        {
            EditorUtility.DisplayDialog("Error", "Sequence name cannot be empty!", "OK");
            return;
        }

        // Ensure directory exists
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        // Create sequence data
        StorytellingSequenceData sequence = ScriptableObject.CreateInstance<StorytellingSequenceData>();
        sequence.sequenceName = sequenceName;
        sequence.description = description;
        sequence.endingType = endingType;
        sequence.nextSceneName = nextSceneName;
        sequence.allowSkip = true;
        sequence.skipKey = KeyCode.Escape;
        sequence.skipHintText = "Nhấn ESC để bỏ qua";

        // Create segments
        sequence.segments = new StorySegment[segmentCount];
        for (int i = 0; i < segmentCount; i++)
        {
            sequence.segments[i] = new StorySegment
            {
                textLines = new string[] { $"Dòng 1 của đoạn {i + 1}...", $"Dòng 2 của đoạn {i + 1}..." },
                typewriterSpeed = 0.03f,
                autoAdvanceDelay = 0f, // Chờ input
                backgroundTint = Color.white,
                illustrationScale = 1f,
                fadeToBlackBefore = (i == 0), // First segment fades from black
                backgroundTransition = StorySegment.TransitionEffect.Fade,
                bgmVolume = 0.5f,
                sfxVolume = 1f
            };
        }

        // Save asset
        string assetPath = $"{savePath}{sequenceName}_Sequence.asset";
        AssetDatabase.CreateAsset(sequence, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Select the created asset
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = sequence;

        Debug.Log($"[StorytellingSequenceCreator] Created sequence: {assetPath}");
        EditorUtility.DisplayDialog("Success", $"Storytelling Sequence created:\n{assetPath}", "OK");
    }
}

/// <summary>
/// Custom Inspector cho StorytellingSequenceData
/// </summary>
[CustomEditor(typeof(StorytellingSequenceData))]
public class StorytellingSequenceDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "💡 TIP: Kéo Sprite vào Background Image và Illustration Image của từng segment.\n" +
            "Sử dụng Typewriter Speed = 0 để hiện text ngay lập tức.",
            MessageType.Info
        );

        StorytellingSequenceData sequence = (StorytellingSequenceData)target;

        EditorGUILayout.Space();
        if (GUILayout.Button("▶️ Test Play Sequence", GUILayout.Height(30)))
        {
            if (Application.isPlaying)
            {
                StorytellingManager.Instance.PlaySequence(sequence);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Enter Play Mode to test!", "OK");
            }
        }

        if (GUILayout.Button("📋 Duplicate Sequence", GUILayout.Height(25)))
        {
            DuplicateSequence(sequence);
        }
    }

    private void DuplicateSequence(StorytellingSequenceData original)
    {
        string path = AssetDatabase.GetAssetPath(original);
        string newPath = path.Replace(".asset", "_Copy.asset");
        AssetDatabase.CopyAsset(path, newPath);
        AssetDatabase.Refresh();
        
        StorytellingSequenceData newSequence = AssetDatabase.LoadAssetAtPath<StorytellingSequenceData>(newPath);
        Selection.activeObject = newSequence;
        
        Debug.Log($"[StorytellingSequenceDataEditor] Duplicated: {newPath}");
    }
}
#endif

