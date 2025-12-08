#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor tools để tạo nhanh các objects cho scene
/// </summary>
public class SceneSetupHelper : EditorWindow
{
    [MenuItem("Tools/Game Setup/Create Door")]
    public static void CreateDoor()
    {
        GameObject door = new GameObject("Door_New");
        
        // Add BoxCollider2D
        var collider = door.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1f, 2f);
        
        // Add SceneTransition
        door.AddComponent<SceneTransition>();
        
        // Add visual indicator (sprite)
        var spriteRenderer = door.AddComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(0.5f, 0.3f, 0.1f, 0.5f); // Brown semi-transparent
        
        // Position at scene view center
        if (SceneView.lastActiveSceneView != null)
        {
            door.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
            door.transform.position = new Vector3(door.transform.position.x, door.transform.position.y, 0);
        }
        
        Selection.activeGameObject = door;
        Undo.RegisterCreatedObjectUndo(door, "Create Door");
        
        Debug.Log("[SceneSetupHelper] Door created! Configure SceneTransition in Inspector.");
    }

    [MenuItem("Tools/Game Setup/Create SpawnPoint")]
    public static void CreateSpawnPoint()
    {
        GameObject spawnPoint = new GameObject("SpawnPoint_New");
        spawnPoint.AddComponent<SpawnPoint>();
        
        // Position at scene view center
        if (SceneView.lastActiveSceneView != null)
        {
            spawnPoint.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
            spawnPoint.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, 0);
        }
        
        Selection.activeGameObject = spawnPoint;
        Undo.RegisterCreatedObjectUndo(spawnPoint, "Create SpawnPoint");
        
        Debug.Log("[SceneSetupHelper] SpawnPoint created! Configure in Inspector.");
    }

    [MenuItem("Tools/Game Setup/Create SpawnManager")]
    public static void CreateSpawnManager()
    {
        // Check if already exists
        var existing = FindFirstObjectByType<SpawnManager>();
        if (existing != null)
        {
            Selection.activeGameObject = existing.gameObject;
            Debug.LogWarning("[SceneSetupHelper] SpawnManager already exists in scene!");
            return;
        }
        
        GameObject manager = new GameObject("SpawnManager");
        manager.AddComponent<SpawnManager>();
        
        Selection.activeGameObject = manager;
        Undo.RegisterCreatedObjectUndo(manager, "Create SpawnManager");
        
        Debug.Log("[SceneSetupHelper] SpawnManager created!");
    }

    [MenuItem("Tools/Game Setup/Create Managers (All)")]
    public static void CreateAllManagers()
    {
        GameObject managers = new GameObject("--- MANAGERS ---");
        
        // GameManager
        if (FindFirstObjectByType<GameManager>() == null)
        {
            GameObject gm = new GameObject("GameManager");
            gm.AddComponent<GameManager>();
            gm.transform.SetParent(managers.transform);
        }
        
        // StoryManager
        if (FindFirstObjectByType<StoryManager>() == null)
        {
            GameObject sm = new GameObject("StoryManager");
            sm.AddComponent<StoryManager>();
            sm.transform.SetParent(managers.transform);
        }
        
        // SaveManager
        if (FindFirstObjectByType<SaveManager>() == null)
        {
            GameObject save = new GameObject("SaveManager");
            save.AddComponent<SaveManager>();
            save.transform.SetParent(managers.transform);
        }
        
        // SpawnManager
        if (FindFirstObjectByType<SpawnManager>() == null)
        {
            GameObject spawn = new GameObject("SpawnManager");
            spawn.AddComponent<SpawnManager>();
            spawn.transform.SetParent(managers.transform);
        }
        
        Selection.activeGameObject = managers;
        Undo.RegisterCreatedObjectUndo(managers, "Create All Managers");
        
        Debug.Log("[SceneSetupHelper] All managers created!");
    }

    [MenuItem("Tools/Game Setup/Setup StreetScene Quick")]
    public static void SetupStreetSceneQuick()
    {
        // Create folder structure
        CreateFolderObject("--- ENVIRONMENT ---");
        CreateFolderObject("--- SPAWN POINTS ---");
        CreateFolderObject("--- DOORS ---");
        CreateFolderObject("--- NPCS ---");
        CreateFolderObject("--- MANAGERS ---");
        
        // Create SpawnManager
        CreateSpawnManager();
        
        // Create default spawn points
        CreateSpawnPointWithId("SpawnPoint_FromSchool", "from_school", true);
        CreateSpawnPointWithId("SpawnPoint_FromHome", "from_home", false);
        
        // Create doors
        CreateDoorWithConfig("Door_ToSchool", "ClassroomScene", "from_street", new Vector3(-10, 0, 0));
        CreateDoorWithConfig("Door_ToHome", "HomeScene", "from_street", new Vector3(10, 0, 0));
        
        Debug.Log("[SceneSetupHelper] StreetScene basic setup complete!");
    }

    private static void CreateFolderObject(string name)
    {
        if (GameObject.Find(name) == null)
        {
            GameObject folder = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(folder, "Create Folder");
        }
    }

    private static void CreateSpawnPointWithId(string name, string id, bool isDefault)
    {
        GameObject sp = new GameObject(name);
        var component = sp.AddComponent<SpawnPoint>();
        
        // Set values via SerializedObject
        var so = new SerializedObject(component);
        so.FindProperty("spawnPointId").stringValue = id;
        so.FindProperty("isDefaultSpawn").boolValue = isDefault;
        so.ApplyModifiedProperties();
        
        Undo.RegisterCreatedObjectUndo(sp, "Create SpawnPoint");
    }

    private static void CreateDoorWithConfig(string name, string targetScene, string spawnId, Vector3 position)
    {
        GameObject door = new GameObject(name);
        door.transform.position = position;
        
        var collider = door.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(1f, 2f);
        
        var transition = door.AddComponent<SceneTransition>();
        
        // Set values via SerializedObject
        var so = new SerializedObject(transition);
        so.FindProperty("targetSceneName").stringValue = targetScene;
        so.FindProperty("targetSpawnPointId").stringValue = spawnId;
        so.ApplyModifiedProperties();
        
        Undo.RegisterCreatedObjectUndo(door, "Create Door");
    }
}
#endif

