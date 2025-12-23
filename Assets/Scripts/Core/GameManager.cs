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
    private static bool _applicationIsQuitting = false;

    public static GameManager Instance
    {
        get
        {
            if (_applicationIsQuitting) return null;

            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
                if (_instance == null && !_applicationIsQuitting)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                    Debug.Log("[GameManager] Auto-created GameManager instance");
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Reset application quitting flag - gọi khi domain reload để đảm bảo singleton hoạt động
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticFields()
    {
        _applicationIsQuitting = false;
        _instance = null;
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

        // Tự động tạo ScreenFader nếu chưa có
        EnsureScreenFaderExists();
    }

    /// <summary>
    /// Đảm bảo ScreenFader tồn tại
    /// </summary>
    private void EnsureScreenFaderExists()
    {
        if (ScreenFader.Instance == null)
        {
            GameObject faderObj = new GameObject("ScreenFader");
            faderObj.AddComponent<ScreenFader>();
            Debug.Log("[GameManager] Tự động tạo ScreenFader");
        }
    }

    private void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (_instance == this)
        {
            // CHỈ clear instance, KHÔNG set _applicationIsQuitting
            // Vì DontDestroyOnLoad, OnDestroy chỉ gọi khi quit hoặc manual destroy
            if (!_applicationIsQuitting)
            {
                _instance = null;
            }
        }
    }

    #region Scene Loading
    /// <summary>
    /// Load scene với spawn point cụ thể (có fade effect)
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

        // Sử dụng fade nếu có ScreenFader
        if (ScreenFader.Instance != null)
        {
            StartCoroutine(LoadSceneWithFade(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    /// <summary>
    /// Load scene với fade effect
    /// </summary>
    private System.Collections.IEnumerator LoadSceneWithFade(string sceneName)
    {
        // Fade ra màn hình đen
        yield return ScreenFader.Instance.FadeOutCoroutine();

        // Load scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade in sẽ được gọi trong OnSceneLoaded
    }

    /// <summary>
    /// Load scene async với fade effect (deprecated - dùng LoadScene thay thế)
    /// </summary>
    public void LoadSceneAsync(string sceneName, string spawnPointId = "")
    {
        LoadScene(sceneName, spawnPointId);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] Scene loaded: {scene.name}");

        // Tìm player và spawn tại đúng vị trí, sau đó fade in
        StartCoroutine(SpawnPlayerAndFadeIn());
    }

    /// <summary>
    /// Spawn player và fade in màn hình
    /// </summary>
    private System.Collections.IEnumerator SpawnPlayerAndFadeIn()
    {
        // Đợi 1 frame để đảm bảo scene đã load hoàn toàn
        yield return null;

        // Spawn player trước
        yield return StartCoroutine(DoSpawnPlayer());

        // Fade in sau khi spawn xong
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeInCoroutine();
        }

        isTransitioningScene = false;
    }

    /// <summary>
    /// Thực hiện spawn player (tách riêng để dùng chung)
    /// </summary>
    private System.Collections.IEnumerator DoSpawnPlayer()
    {
        // Tìm SpawnManager trong scene mới
        SpawnManager spawnManager = FindFirstObjectByType<SpawnManager>();
        if (spawnManager == null)
        {
            Debug.LogWarning("[GameManager] SpawnManager not found in scene!");
            yield break;
        }

        // Tìm Player trong scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[GameManager] Player not found in scene!");
            yield break;
        }

        // Spawn player tại spawn point
        string spawnId = targetSpawnPointId;

        // Nếu không có targetSpawnPointId, thử tìm theo previous scene
        if (string.IsNullOrEmpty(spawnId) && !string.IsNullOrEmpty(previousSceneName))
        {
            spawnId = "from_" + previousSceneName.ToLower().Replace("scene", "");
            Debug.Log($"[GameManager] Auto-generated spawn ID: {spawnId}");
        }

        spawnManager.SpawnPlayer(player, spawnId);
        
        // Lưu lại spawn point ID đã dùng
        lastUsedSpawnPointId = spawnId;
        Debug.Log($"[GameManager] Saved lastUsedSpawnPointId: '{lastUsedSpawnPointId}'");

        // Snap camera ngay lập tức đến vị trí player
        CameraHelper.SnapCameraToTarget(player.transform);

        // Reset target spawn point sau khi spawn xong
        targetSpawnPointId = "";
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
    /// Lấy spawn point ID hiện tại (spawn point mà player vừa spawn vào)
    /// </summary>
    public string GetCurrentSpawnPointId()
    {
        return lastUsedSpawnPointId;
    }
    
    // Track spawn point ID đã dùng gần nhất
    private string lastUsedSpawnPointId = "";

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

