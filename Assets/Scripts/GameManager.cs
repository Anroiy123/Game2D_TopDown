using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameManager - Singleton quản lý game state, scene loading, player data persistence
/// Tồn tại xuyên suốt các scene (DontDestroyOnLoad)
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }
    #endregion

    #region Scene Management
    [Header("Scene Names")]
    [Tooltip("Tên các scene trong game")]
    public string homeSceneName = "HomeScene";
    public string classroomSceneName = "ClassroomScene";
    public string streetSceneName = "StreetScene";

    [Header("Scene Transition")]
    [Tooltip("Scene trước đó (để xác định spawn point)")]
    public string previousSceneName;
    
    [Tooltip("ID của spawn point muốn spawn vào")]
    public string targetSpawnPointId;
    #endregion

    #region Game State
    [Header("Game State")]
    public bool isGamePaused = false;
    public bool isInDialogue = false;
    public bool isTransitioningScene = false;
    #endregion

    #region Player Data (Persist across scenes)
    [Header("Player Data")]
    public Vector3 lastPlayerPosition;
    public string lastPlayerDirection = "down";
    #endregion

    private void Awake()
    {
        // Singleton pattern với DontDestroyOnLoad
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #region Scene Loading
    /// <summary>
    /// Load scene với spawn point cụ thể
    /// </summary>
    /// <param name="sceneName">Tên scene cần load</param>
    /// <param name="spawnPointId">ID của spawn point (optional)</param>
    public void LoadScene(string sceneName, string spawnPointId = "")
    {
        if (isTransitioningScene) return;
        
        isTransitioningScene = true;
        previousSceneName = SceneManager.GetActiveScene().name;
        targetSpawnPointId = spawnPointId;
        
        Debug.Log($"[GameManager] Loading scene: {sceneName}, SpawnPoint: {spawnPointId}");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Load scene async với fade effect (nếu có SceneFader)
    /// </summary>
    public void LoadSceneAsync(string sceneName, string spawnPointId = "")
    {
        if (isTransitioningScene) return;
        
        isTransitioningScene = true;
        previousSceneName = SceneManager.GetActiveScene().name;
        targetSpawnPointId = spawnPointId;
        
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }

    private System.Collections.IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        // TODO: Trigger fade out animation here
        yield return new WaitForSeconds(0.5f);
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isTransitioningScene = false;
        Debug.Log($"[GameManager] Scene loaded: {scene.name}");
    }
    #endregion

    #region Game State Control
    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
    }

    public void SetDialogueState(bool inDialogue)
    {
        isInDialogue = inDialogue;
    }
    #endregion

    #region Utility
    /// <summary>
    /// Lấy tên scene hiện tại
    /// </summary>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Kiểm tra scene có tồn tại trong build settings không
    /// </summary>
    public bool IsSceneValid(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName) return true;
        }
        return false;
    }
    #endregion
}

