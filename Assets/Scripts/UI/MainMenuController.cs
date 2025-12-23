using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

/// <summary>
/// MainMenuController - Điều khiển Main Menu với video background
/// Tích hợp với GameManager, SaveManager, StoryManager
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Video Background")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoRawImage;
    [SerializeField] private string videoFileName = "grok-video-e5af2587-7500-482b-a932-69cd4afd119c.mp4";
    
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    
    [Header("Main Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    
    [Header("Settings")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Button backButton;
    
    [Header("Scene Settings")]
    [SerializeField] private string firstSceneName = "HomeScene";
    [SerializeField] private string firstSpawnPointId = "bedroom_spawn";
    
    [Header("Audio")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip menuBGM;
    
    private RenderTexture videoRenderTexture;
    
    private void Awake()
    {
        SetupVideoPlayer();
    }
    
    private void Start()
    {
        // Đảm bảo có EventSystem trong scene
        EnsureEventSystem();
        
        SetupButtons();
        ShowMainMenu();
        PlayBGM();
        
        Debug.Log("[MainMenu] Start completed. Buttons setup done.");
    }
    
    /// <summary>
    /// Đảm bảo có EventSystem để UI buttons hoạt động
    /// </summary>
    private void EnsureEventSystem()
    {
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("[MainMenu] Created EventSystem");
        }
    }
    
    private void OnDestroy()
    {
        if (videoRenderTexture != null)
        {
            videoRenderTexture.Release();
            Destroy(videoRenderTexture);
        }
    }
    
    #region Video Setup
    private void SetupVideoPlayer()
    {
        if (videoPlayer == null) return;
        
        // Tạo RenderTexture cho video
        videoRenderTexture = new RenderTexture(1920, 1080, 0);
        videoRenderTexture.Create();
        
        videoPlayer.targetTexture = videoRenderTexture;
        videoPlayer.isLooping = true;
        videoPlayer.playOnAwake = true;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        
        // Gán RenderTexture cho RawImage
        if (videoRawImage != null)
        {
            videoRawImage.texture = videoRenderTexture;
        }
        
        // Set video URL
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        
        // Fallback: tìm trong Assets nếu không có trong StreamingAssets
        if (!System.IO.File.Exists(videoPath))
        {
            // Dùng Resources hoặc direct path
            videoPlayer.source = VideoSource.VideoClip;
        }
        else
        {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = videoPath;
        }
        
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }
    
    private void OnVideoPrepared(VideoPlayer source)
    {
        source.Play();
    }
    #endregion
    
    #region Button Setup
    private void SetupButtons()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);
        
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);
        
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
        
        // Settings sliders
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        // Fullscreen toggle
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = PlayerPrefs.GetInt("IsFullscreen", 1) == 1;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
        }
    }
    #endregion
    
    #region Button Handlers
    private void OnPlayClicked()
    {
        Debug.Log("[MainMenu] Play clicked");
        
        // Đảm bảo StoryManager tồn tại (singleton sẽ tự tạo nếu chưa có)
        var storyManager = StoryManager.Instance;
        if (storyManager != null)
        {
            // Reset story progress trước
            storyManager.ResetStory();
            
            // Set flag game_start để VNTrigger trong HomeScene có thể trigger
            storyManager.SetFlag("game_start", true);
            Debug.Log("[MainMenu] Set flag 'game_start' = true");
        }
        else
        {
            Debug.LogWarning("[MainMenu] StoryManager.Instance is null!");
        }
        
        // Load scene đầu tiên
        StartCoroutine(StartGame());
    }
    
    private IEnumerator StartGame()
    {
        // Fade out (tạo ScreenFader nếu chưa có)
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeOutCoroutine();
        }
        else
        {
            // Tạo ScreenFader nếu chưa có
            GameObject faderObj = new GameObject("ScreenFader");
            faderObj.AddComponent<ScreenFader>();
            yield return new WaitForSeconds(0.1f); // Đợi khởi tạo
            yield return ScreenFader.Instance.FadeOutCoroutine();
        }
        
        // Stop video và BGM
        if (videoPlayer != null) videoPlayer.Stop();
        if (bgmSource != null) bgmSource.Stop();
        
        // Đảm bảo GameManager tồn tại
        var gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.LoadScene(firstSceneName, firstSpawnPointId);
        }
        else
        {
            Debug.LogWarning("[MainMenu] GameManager.Instance is null, using SceneManager directly");
            UnityEngine.SceneManagement.SceneManager.LoadScene(firstSceneName);
        }
    }
    
    private void OnSettingsClicked()
    {
        Debug.Log("[MainMenu] Settings clicked");
        ShowSettings();
    }
    
    private void OnExitClicked()
    {
        Debug.Log("[MainMenu] Exit clicked");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    private void OnBackClicked()
    {
        ShowMainMenu();
    }
    #endregion
    
    #region Settings
    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        if (bgmSource != null) bgmSource.volume = value;
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    private void OnFullscreenToggleChanged(bool isFullscreen)
    {
        if (ScreenModeManager.Instance != null)
        {
            if (isFullscreen)
                ScreenModeManager.Instance.SetFullscreen();
            else
                ScreenModeManager.Instance.SetWindowed();
        }
        else
        {
            // Fallback nếu chưa có ScreenModeManager
            if (isFullscreen)
            {
                Resolution res = Screen.currentResolution;
                Screen.SetResolution(res.width, res.height, FullScreenMode.FullScreenWindow);
            }
            else
            {
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
            }
            PlayerPrefs.SetInt("IsFullscreen", isFullscreen ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    #endregion
    
    #region Panel Management
    private void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }
    
    private void ShowSettings()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }
    #endregion
    
    #region Audio
    private void PlayBGM()
    {
        if (bgmSource != null && menuBGM != null)
        {
            bgmSource.clip = menuBGM;
            bgmSource.loop = true;
            bgmSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            bgmSource.Play();
        }
    }
    #endregion
}
