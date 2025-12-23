using UnityEngine;

/// <summary>
/// ScreenModeManager - Quản lý chế độ màn hình (Fullscreen/Windowed)
/// Nhấn F11 để toggle giữa fullscreen và windowed mode
/// Singleton, DontDestroyOnLoad để persist qua các scene
/// </summary>
public class ScreenModeManager : MonoBehaviour
{
    public static ScreenModeManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.F11;
    
    [Header("Windowed Mode Settings")]
    [SerializeField] private int windowedWidth = 1280;
    [SerializeField] private int windowedHeight = 720;

    private const string PREF_FULLSCREEN = "IsFullscreen";
    private const string PREF_WINDOWED_WIDTH = "WindowedWidth";
    private const string PREF_WINDOWED_HEIGHT = "WindowedHeight";

    public bool IsFullscreen => Screen.fullScreenMode == FullScreenMode.FullScreenWindow || 
                                Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFullscreen();
        }
    }

    /// <summary>
    /// Toggle giữa fullscreen và windowed mode
    /// </summary>
    public void ToggleFullscreen()
    {
        if (IsFullscreen)
        {
            SetWindowed();
        }
        else
        {
            SetFullscreen();
        }
    }

    /// <summary>
    /// Chuyển sang fullscreen mode
    /// </summary>
    public void SetFullscreen()
    {
        Resolution currentRes = Screen.currentResolution;
        Screen.SetResolution(currentRes.width, currentRes.height, FullScreenMode.FullScreenWindow);
        PlayerPrefs.SetInt(PREF_FULLSCREEN, 1);
        PlayerPrefs.Save();
        Debug.Log($"[ScreenMode] Switched to Fullscreen ({currentRes.width}x{currentRes.height})");
    }

    /// <summary>
    /// Chuyển sang windowed mode
    /// </summary>
    public void SetWindowed()
    {
        Screen.SetResolution(windowedWidth, windowedHeight, FullScreenMode.Windowed);
        PlayerPrefs.SetInt(PREF_FULLSCREEN, 0);
        PlayerPrefs.Save();
        Debug.Log($"[ScreenMode] Switched to Windowed ({windowedWidth}x{windowedHeight})");
    }

    /// <summary>
    /// Set windowed resolution
    /// </summary>
    public void SetWindowedResolution(int width, int height)
    {
        windowedWidth = width;
        windowedHeight = height;
        PlayerPrefs.SetInt(PREF_WINDOWED_WIDTH, width);
        PlayerPrefs.SetInt(PREF_WINDOWED_HEIGHT, height);
        
        if (!IsFullscreen)
        {
            Screen.SetResolution(width, height, FullScreenMode.Windowed);
        }
        
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        windowedWidth = PlayerPrefs.GetInt(PREF_WINDOWED_WIDTH, 1280);
        windowedHeight = PlayerPrefs.GetInt(PREF_WINDOWED_HEIGHT, 720);
        
        bool savedFullscreen = PlayerPrefs.GetInt(PREF_FULLSCREEN, 1) == 1;
        
        if (savedFullscreen)
        {
            SetFullscreen();
        }
        else
        {
            SetWindowed();
        }
    }
}
