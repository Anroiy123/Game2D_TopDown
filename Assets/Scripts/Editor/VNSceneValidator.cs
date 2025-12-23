#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor tool để validate VNSceneData
/// Menu: Tools/Visual Novel/Validate VN Scene
/// </summary>
public class VNSceneValidator : EditorWindow
{
    private VNSceneData sceneToValidate;
    private Vector2 scrollPosition;

    [MenuItem("Tools/Visual Novel/Validate VN Scene")]
    private static void ShowWindow()
    {
        var window = GetWindow<VNSceneValidator>("VN Scene Validator");
        window.minSize = new Vector2(400, 300);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("VN Scene Data Validator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        sceneToValidate = (VNSceneData)EditorGUILayout.ObjectField(
            "VN Scene Data",
            sceneToValidate,
            typeof(VNSceneData),
            false
        );

        EditorGUILayout.Space();

        if (GUILayout.Button("Validate", GUILayout.Height(30)))
        {
            ValidateScene();
        }

        EditorGUILayout.Space();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUILayout.EndScrollView();
    }

    private void ValidateScene()
    {
        if (sceneToValidate == null)
        {
            Debug.LogError("[Validator] No VN Scene selected!");
            return;
        }

        Debug.Log($"═══════════════════════════════════════");
        Debug.Log($"Validating: {sceneToValidate.name}");
        Debug.Log($"═══════════════════════════════════════");

        int errorCount = 0;
        int warningCount = 0;

        var scene = sceneToValidate.sceneData;

        // Check scene name
        if (string.IsNullOrEmpty(scene.sceneName))
        {
            Debug.LogWarning("⚠️ Scene Name is empty");
            warningCount++;
        }
        else
        {
            Debug.Log($"✅ Scene Name: {scene.sceneName}");
        }

        // Check background
        if (scene.backgroundImage == null)
        {
            Debug.LogError("❌ Background Image is NULL!");
            errorCount++;
        }
        else
        {
            Debug.Log($"✅ Background: {scene.backgroundImage.name}");
        }

        // Check dialogue
        if (scene.dialogue == null)
        {
            Debug.LogError("❌ Dialogue is NULL!");
            errorCount++;
        }
        else
        {
            Debug.Log($"✅ Dialogue: {scene.dialogue.name}");
            ValidateDialogue(scene.dialogue, ref errorCount, ref warningCount);
        }

        // Check characters
        if (scene.characters == null || scene.characters.Length == 0)
        {
            Debug.LogWarning("⚠️ No characters defined (narrator-only scene?)");
            warningCount++;
        }
        else
        {
            Debug.Log($"✅ Characters: {scene.characters.Length}");
            ValidateCharacters(scene.characters, ref errorCount, ref warningCount);
        }

        // Check return settings
        if (scene.returnToTopDown)
        {
            if (string.IsNullOrEmpty(scene.topDownSceneName))
            {
                Debug.LogError("❌ Return to top-down enabled but scene name is empty!");
                errorCount++;
            }
            else
            {
                Debug.Log($"✅ Returns to: {scene.topDownSceneName}");
            }

            if (string.IsNullOrEmpty(scene.spawnPointId))
            {
                Debug.LogWarning("⚠️ Spawn Point ID is empty (will use default)");
                warningCount++;
            }
            else
            {
                Debug.Log($"✅ Spawn at: {scene.spawnPointId}");
            }
        }

        Debug.Log($"═══════════════════════════════════════");
        Debug.Log($"Validation Complete:");
        Debug.Log($"  ❌ Errors: {errorCount}");
        Debug.Log($"  ⚠️ Warnings: {warningCount}");
        Debug.Log($"═══════════════════════════════════════");
    }

    private void ValidateDialogue(DialogueData dialogue, ref int errors, ref int warnings)
    {
        if (dialogue.nodes == null || dialogue.nodes.Length == 0)
        {
            Debug.LogError("  ❌ Dialogue has no nodes!");
            errors++;
            return;
        }

        Debug.Log($"  - Nodes: {dialogue.nodes.Length}");

        // Check start node exists
        bool hasStartNode = false;
        foreach (var node in dialogue.nodes)
        {
            if (node.nodeId == dialogue.startNodeId)
            {
                hasStartNode = true;
                break;
            }
        }

        if (!hasStartNode)
        {
            Debug.LogError($"  ❌ Start node ID {dialogue.startNodeId} not found!");
            errors++;
        }
    }

    private void ValidateCharacters(VNCharacterDisplay[] characters, ref int errors, ref int warnings)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            var character = characters[i];

            if (character.characterSprite == null)
            {
                Debug.LogError($"  ❌ Character {i}: Sprite is NULL!");
                errors++;
            }
            else
            {
                Debug.Log($"  - Character {i}: {character.characterName ?? "Unnamed"} at {character.position}");
            }

            if (string.IsNullOrEmpty(character.characterName))
            {
                Debug.LogWarning($"  ⚠️ Character {i}: Name is empty");
                warnings++;
            }
        }
    }
}


#endif