#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class FixScene7AAsset : EditorWindow
{
    [MenuItem("Tools/VN Scene/Fix Scene 7A Asset")]
    static void FixScene7A()
    {
        // Load assets
        var vnScene = AssetDatabase.LoadAssetAtPath<VNSceneData>(
            "Assets/Data/VisualNovel/Day1/Day1_Scene7A_Confrontation_VNScene_VNScene.asset");
        var dialogue = AssetDatabase.LoadAssetAtPath<DialogueData>(
            "Assets/Data/Dialogues/Day1/Day1_Scene7A_Confrontation_Dialogue.asset");
        
        if (vnScene == null)
        {
            Debug.LogError("VNSceneData not found at: Assets/Data/VisualNovel/Day1/Day1_Scene7A_Confrontation_VNScene_VNScene.asset");
            return;
        }
        
        if (dialogue == null)
        {
            Debug.LogError("DialogueData not found at: Assets/Data/Dialogues/Day1/Day1_Scene7A_Confrontation_Dialogue.asset");
            return;
        }
        
        // Fix dialogue reference
        vnScene.sceneData.dialogue = dialogue;
        
        // Fix return to top-down settings
        vnScene.sceneData.returnToTopDown = true;
        vnScene.sceneData.topDownSceneName = "StreetScene";
        vnScene.sceneData.spawnPointId = "after_confrontation";

        // Load character sprites
        Sprite bullySprite = LoadFirstSprite("Assets/Sprites/Characters/Bully/Bully_idle.png");
        Sprite danEmSprite = LoadFirstSprite("Assets/Sprites/Characters/DanEm1/DanEm_idle.png");

        // Add characters (Thủ lĩnh and Đàn em)
        vnScene.sceneData.characters = new VNCharacterDisplay[]
        {
            new VNCharacterDisplay
            {
                characterSprite = bullySprite,
                characterName = "Thủ lĩnh",
                position = VNCharacterDisplay.CharacterPosition.Left,
                scale = Vector2.one,
                flipX = false
            },
            new VNCharacterDisplay
            {
                characterSprite = danEmSprite,
                characterName = "Đàn em",
                position = VNCharacterDisplay.CharacterPosition.Right,
                scale = Vector2.one,
                flipX = false
            }
        };
        
        // Save changes
        EditorUtility.SetDirty(vnScene);
        AssetDatabase.SaveAssets();
        
        Debug.Log("[FixScene7A] ✅ Scene 7A asset fixed successfully!");
        Debug.Log($"  - Dialogue: {dialogue.name}");
        Debug.Log($"  - Return to: {vnScene.sceneData.topDownSceneName}");
        Debug.Log($"  - Spawn point: {vnScene.sceneData.spawnPointId}");
        Debug.Log($"  - Characters: {vnScene.sceneData.characters.Length}");
        Debug.Log($"  - Bully sprite: {bullySprite != null}");
        Debug.Log($"  - DanEm sprite: {danEmSprite != null}");

        // Select the asset in Project window
        Selection.activeObject = vnScene;
        EditorGUIUtility.PingObject(vnScene);
    }

    /// <summary>
    /// Load first sprite from a sprite sheet
    /// </summary>
    private static Sprite LoadFirstSprite(string path)
    {
        var sprites = AssetDatabase.LoadAllAssetsAtPath(path);
        foreach (var obj in sprites)
        {
            if (obj is Sprite sprite)
            {
                return sprite;
            }
        }
        Debug.LogWarning($"No sprite found at: {path}");
        return null;
    }
}
#endif

