using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor tool để setup cảnh NPCs vây quanh player
/// </summary>
public class NPCSurroundSetupHelper : EditorWindow
{
    private GameObject[] selectedNPCs;
    private float surroundRadius = 2.5f;
    private NPCSurroundPlayer.FormationType formationType = NPCSurroundPlayer.FormationType.Circle;
    private float moveSpeed = 3f;
    private float delayBetweenNPCs = 0.2f;
    private VNTrigger nextVNTrigger;

    [MenuItem("Tools/NPC Setup/Setup Surround Formation")]
    public static void ShowWindow()
    {
        GetWindow<NPCSurroundSetupHelper>("Setup NPC Surround");
    }

    private void OnGUI()
    {
        GUILayout.Label("Setup NPCs Vây Quanh Player", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "Tool này giúp bạn setup cảnh NPCs vây quanh player.\n\n" +
            "Cách dùng:\n" +
            "1. Chọn các NPC GameObjects trong scene\n" +
            "2. Điều chỉnh settings\n" +
            "3. Click 'Create Surround Controller'",
            MessageType.Info
        );

        EditorGUILayout.Space();

        // Settings
        surroundRadius = EditorGUILayout.FloatField("Surround Radius", surroundRadius);
        formationType = (NPCSurroundPlayer.FormationType)EditorGUILayout.EnumPopup("Formation Type", formationType);
        moveSpeed = EditorGUILayout.FloatField("Move Speed", moveSpeed);
        delayBetweenNPCs = EditorGUILayout.FloatField("Delay Between NPCs", delayBetweenNPCs);
        nextVNTrigger = (VNTrigger)EditorGUILayout.ObjectField("Next VN Trigger", nextVNTrigger, typeof(VNTrigger), true);

        EditorGUILayout.Space();

        // Get selected NPCs
        if (GUILayout.Button("Get Selected NPCs"))
        {
            selectedNPCs = Selection.gameObjects;
            Debug.Log($"[NPCSurroundSetup] Selected {selectedNPCs.Length} NPCs");
        }

        // Show selected NPCs
        if (selectedNPCs != null && selectedNPCs.Length > 0)
        {
            EditorGUILayout.LabelField($"Selected NPCs: {selectedNPCs.Length}");
            foreach (var npc in selectedNPCs)
            {
                EditorGUILayout.LabelField($"  - {npc.name}");
            }
        }

        EditorGUILayout.Space();

        // Create button
        GUI.enabled = selectedNPCs != null && selectedNPCs.Length > 0;
        if (GUILayout.Button("Create Surround Controller", GUILayout.Height(40)))
        {
            CreateSurroundController();
        }
        GUI.enabled = true;
    }

    private void CreateSurroundController()
    {
        // Tạo GameObject mới
        GameObject controllerObj = new GameObject("NPCSurroundController");
        
        // Add component
        NPCSurroundPlayer controller = controllerObj.AddComponent<NPCSurroundPlayer>();

        // Setup via SerializedObject (để set private fields)
        SerializedObject so = new SerializedObject(controller);
        
        so.FindProperty("surroundRadius").floatValue = surroundRadius;
        so.FindProperty("formationType").enumValueIndex = (int)formationType;
        so.FindProperty("moveSpeed").floatValue = moveSpeed;
        so.FindProperty("delayBetweenNPCs").floatValue = delayBetweenNPCs;
        so.FindProperty("nextVNTrigger").objectReferenceValue = nextVNTrigger;

        // Set NPCs array
        SerializedProperty npcsProp = so.FindProperty("npcBullies");
        npcsProp.arraySize = selectedNPCs.Length;
        for (int i = 0; i < selectedNPCs.Length; i++)
        {
            npcsProp.GetArrayElementAtIndex(i).objectReferenceValue = selectedNPCs[i];
        }

        so.ApplyModifiedProperties();

        // Select the new controller
        Selection.activeGameObject = controllerObj;

        Debug.Log($"[NPCSurroundSetup] Created controller with {selectedNPCs.Length} NPCs");
        EditorUtility.DisplayDialog("Success", $"Created NPCSurroundController with {selectedNPCs.Length} NPCs!", "OK");
    }
}

