#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor tool để tạo nhanh VN Scene và Dialogue
/// </summary>
public class VNSceneCreator : EditorWindow
{
    [MenuItem("Tools/Visual Novel/Create VN Scene Quick Setup")]
    public static void ShowWindow()
    {
        GetWindow<VNSceneCreator>("VN Scene Creator");
    }

    private string sceneName = "Day1_Morning";
    private string locationText = "Phòng ngủ Đức";
    private Sprite backgroundSprite;
    private bool createDialogue = true;

    private void OnGUI()
    {
        GUILayout.Label("Tạo VN Scene nhanh", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        sceneName = EditorGUILayout.TextField("Scene Name:", sceneName);
        locationText = EditorGUILayout.TextField("Location Text:", locationText);
        backgroundSprite = (Sprite)EditorGUILayout.ObjectField("Background Sprite:", backgroundSprite, typeof(Sprite), false);
        
        EditorGUILayout.Space();
        createDialogue = EditorGUILayout.Toggle("Tạo DialogueData mẫu", createDialogue);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Tool này sẽ tạo:\n" +
            "1. DialogueData (nếu chọn)\n" +
            "2. VNSceneData\n" +
            "Tất cả sẽ được lưu trong Assets/Scripts/Data/VisualNovel/",
            MessageType.Info
        );

        EditorGUILayout.Space();

        if (GUILayout.Button("Tạo VN Scene", GUILayout.Height(40)))
        {
            CreateVNScene();
        }
    }

    private void CreateVNScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            EditorUtility.DisplayDialog("Lỗi", "Hãy nhập Scene Name!", "OK");
            return;
        }

        // Tạo thư mục nếu chưa có
        string folderPath = "Assets/Scripts/Data/VisualNovel";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            string[] folders = folderPath.Split('/');
            string currentPath = folders[0];
            for (int i = 1; i < folders.Length; i++)
            {
                string newPath = currentPath + "/" + folders[i];
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                }
                currentPath = newPath;
            }
        }

        DialogueData dialogue = null;

        // Tạo DialogueData nếu cần
        if (createDialogue)
        {
            dialogue = ScriptableObject.CreateInstance<DialogueData>();
            dialogue.conversationName = sceneName;
            dialogue.startNodeId = 0;
            dialogue.nodes = new DialogueNode[]
            {
                new DialogueNode
                {
                    nodeId = 0,
                    speakerName = "Narrator",
                    isPlayerSpeaking = false,
                    dialogueLines = new string[] 
                    { 
                        $"Chào mừng đến với {locationText}",
                        "Đây là dialogue mẫu. Hãy chỉnh sửa trong Inspector!"
                    },
                    choices = new DialogueChoice[0],
                    nextNodeId = -1
                }
            };

            string dialoguePath = $"{folderPath}/{sceneName}_Dialogue.asset";
            AssetDatabase.CreateAsset(dialogue, dialoguePath);
            Debug.Log($"Created DialogueData: {dialoguePath}");
        }

        // Tạo VNSceneData
        VNSceneData vnScene = ScriptableObject.CreateInstance<VNSceneData>();
        vnScene.sceneData = new VNScene
        {
            sceneName = sceneName,
            locationText = locationText,
            backgroundImage = backgroundSprite,
            backgroundTint = Color.white,
            characters = new VNCharacterDisplay[0],
            dialogue = dialogue,
            bgm = null,
            ambience = null,
            nextScene = null,
            returnToTopDown = false,
            topDownSceneName = "",
            spawnPointId = ""
        };

        string vnScenePath = $"{folderPath}/{sceneName}_VNScene.asset";
        AssetDatabase.CreateAsset(vnScene, vnScenePath);
        Debug.Log($"Created VNSceneData: {vnScenePath}");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Chọn asset vừa tạo
        Selection.activeObject = vnScene;
        EditorGUIUtility.PingObject(vnScene);

        EditorUtility.DisplayDialog("Thành công!", 
            $"Đã tạo VN Scene: {sceneName}\n\n" +
            $"Hãy:\n" +
            $"1. Chỉnh sửa DialogueData trong Inspector\n" +
            $"2. Thêm background sprite\n" +
            $"3. Thêm nhân vật nếu cần", 
            "OK");
    }
}
#endif

