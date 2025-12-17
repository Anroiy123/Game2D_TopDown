#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class CreateAfterConfrontationSpawn : EditorWindow
{
    [MenuItem("Tools/VN Scene/Create After Confrontation Spawn Point")]
    static void CreateSpawnPoint()
    {
        // Check if StreetScene is loaded
        Scene streetScene = SceneManager.GetSceneByName("StreetScene");
        if (!streetScene.isLoaded)
        {
            Debug.LogError("StreetScene is not loaded! Please open StreetScene first.");
            EditorUtility.DisplayDialog("Error", 
                "StreetScene is not loaded!\n\nPlease open StreetScene in the Hierarchy first.", 
                "OK");
            return;
        }
        
        // Check if spawn point already exists
        GameObject[] rootObjects = streetScene.GetRootGameObjects();
        foreach (var obj in rootObjects)
        {
            SpawnPoint[] spawnPoints = obj.GetComponentsInChildren<SpawnPoint>(true);
            foreach (var sp in spawnPoints)
            {
                if (sp.SpawnPointId == "after_confrontation")
                {
                    Debug.LogWarning("Spawn point 'after_confrontation' already exists!");
                    Selection.activeGameObject = sp.gameObject;
                    EditorGUIUtility.PingObject(sp.gameObject);
                    return;
                }
            }
        }
        
        // Find SpawnManager or create one
        SpawnManager spawnManager = Object.FindFirstObjectByType<SpawnManager>();
        GameObject spawnContainer;
        
        if (spawnManager != null)
        {
            spawnContainer = spawnManager.gameObject;
        }
        else
        {
            // Create SpawnManager
            spawnContainer = new GameObject("SpawnManager");
            spawnManager = spawnContainer.AddComponent<SpawnManager>();
            Debug.Log("Created SpawnManager");
        }
        
        // Create spawn point GameObject
        GameObject spawnPointObj = new GameObject("SpawnPoint_AfterConfrontation");
        spawnPointObj.transform.SetParent(spawnContainer.transform);
        
        // Position: Giữa đường, gần BullyFollowTrigger zone
        // Default position - user should adjust in Inspector
        spawnPointObj.transform.position = new Vector3(0f, 5f, 0f);
        
        // Add SpawnPoint component
        SpawnPoint spawnPoint = spawnPointObj.AddComponent<SpawnPoint>();

        // Use SerializedObject to set private fields
        SerializedObject serializedSpawnPoint = new SerializedObject(spawnPoint);
        serializedSpawnPoint.FindProperty("spawnPointId").stringValue = "after_confrontation";
        serializedSpawnPoint.FindProperty("isDefaultSpawn").boolValue = false;
        serializedSpawnPoint.FindProperty("facingDirection").enumValueIndex = (int)SpawnPoint.FacingDirection.Down;
        serializedSpawnPoint.ApplyModifiedProperties();

        // Mark scene as dirty
        EditorSceneManager.MarkSceneDirty(streetScene);
        
        // Select the new spawn point
        Selection.activeGameObject = spawnPointObj;
        EditorGUIUtility.PingObject(spawnPointObj);
        
        Debug.Log("[CreateSpawnPoint] ✅ Spawn point 'after_confrontation' created successfully!");
        Debug.Log($"  - Position: {spawnPointObj.transform.position}");
        Debug.Log($"  - Facing: Down");
        Debug.LogWarning("⚠️ Please adjust the position in Scene view to match the confrontation location!");
        
        EditorUtility.DisplayDialog("Success", 
            "Spawn point 'after_confrontation' created!\n\n" +
            "Position: (0, 5, 0)\n" +
            "Facing: Down\n\n" +
            "Please adjust the position in Scene view to match where the confrontation happens.", 
            "OK");
    }
}
#endif

