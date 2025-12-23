#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor tool để tạo MainMenu Scene nhanh
/// </summary>
public class MainMenuSceneCreator : EditorWindow
{
    private string videoPath = "Assets/Sprites/User_Interface_Elements/grok-video-e5af2587-7500-482b-a932-69cd4afd119c.mp4";
    private string uiSpritePath = "Assets/Sprites/User_Interface_Elements/UiCozyFree.png";
    private string gameTitleText = "ĐỪNG IM LẶNG";
    private string subtitleText = "Câu chuyện về bạo lực học đường";
    
    [MenuItem("Tools/Game Setup/Create MainMenu Scene")]
    public static void ShowWindow()
    {
        GetWindow<MainMenuSceneCreator>("MainMenu Creator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("🎮 TẠO MAIN MENU SCENE", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        
        gameTitleText = EditorGUILayout.TextField("Tên Game:", gameTitleText);
        subtitleText = EditorGUILayout.TextField("Phụ đề:", subtitleText);
        
        EditorGUILayout.Space(5);
        videoPath = EditorGUILayout.TextField("Video Path:", videoPath);
        uiSpritePath = EditorGUILayout.TextField("UI Sprite Path:", uiSpritePath);
        
        EditorGUILayout.Space(20);
        
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("⚡ TẠO MAIN MENU SCENE", GUILayout.Height(40)))
        {
            CreateMainMenuScene();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "Tool sẽ tạo:\n" +
            "• MainMenuScene.unity mới\n" +
            "• Video background với loop\n" +
            "• UI Canvas với các buttons\n" +
            "• MainMenuController component\n\n" +
            "Sau khi tạo, nhớ thêm scene vào Build Settings!",
            MessageType.Info);
    }
    
    private void CreateMainMenuScene()
    {
        // Tạo scene mới
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        // 1. Tạo Main Camera
        GameObject cameraObj = new GameObject("Main Camera");
        Camera cam = cameraObj.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5;
        cam.backgroundColor = Color.black;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cameraObj.AddComponent<AudioListener>();
        cameraObj.tag = "MainCamera";
        
        // 2. Tạo Video Background Canvas (behind UI)
        GameObject videoCanvasObj = new GameObject("VideoCanvas");
        Canvas videoCanvas = videoCanvasObj.AddComponent<Canvas>();
        videoCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        videoCanvas.sortingOrder = -1; // Behind UI
        videoCanvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        videoCanvasObj.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        videoCanvasObj.AddComponent<GraphicRaycaster>();
        
        // Video RawImage
        GameObject videoImageObj = new GameObject("VideoImage");
        videoImageObj.transform.SetParent(videoCanvasObj.transform, false);
        RawImage videoRawImage = videoImageObj.AddComponent<RawImage>();
        videoRawImage.color = Color.white;
        RectTransform videoRect = videoRawImage.rectTransform;
        videoRect.anchorMin = Vector2.zero;
        videoRect.anchorMax = Vector2.one;
        videoRect.sizeDelta = Vector2.zero;
        
        // Video Player
        VideoPlayer videoPlayer = videoImageObj.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = true;
        videoPlayer.isLooping = true;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        
        // Load video clip
        VideoClip videoClip = AssetDatabase.LoadAssetAtPath<VideoClip>(videoPath);
        if (videoClip != null)
        {
            videoPlayer.clip = videoClip;
        }
        else
        {
            Debug.LogWarning($"[MainMenuCreator] Video không tìm thấy tại: {videoPath}");
        }
        
        // 3. Tạo UI Canvas
        GameObject uiCanvasObj = new GameObject("UICanvas");
        Canvas uiCanvas = uiCanvasObj.AddComponent<Canvas>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        uiCanvas.sortingOrder = 0;
        CanvasScaler scaler = uiCanvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        uiCanvasObj.AddComponent<GraphicRaycaster>();
        
        // 4. Tạo Main Menu Panel
        GameObject mainMenuPanel = CreatePanel(uiCanvasObj.transform, "MainMenuPanel");
        
        // Overlay tối để text dễ đọc
        GameObject overlay = new GameObject("DarkOverlay");
        overlay.transform.SetParent(mainMenuPanel.transform, false);
        Image overlayImg = overlay.AddComponent<Image>();
        overlayImg.color = new Color(0, 0, 0, 0.4f);
        overlayImg.raycastTarget = false;
        RectTransform overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;
        
        // Game Title
        GameObject titleObj = CreateText(mainMenuPanel.transform, "GameTitle", gameTitleText, 72, 
            new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.85f), Color.white);
        
        // Subtitle
        GameObject subtitleObj = CreateText(mainMenuPanel.transform, "Subtitle", subtitleText, 28,
            new Vector2(0.5f, 0.78f), new Vector2(0.5f, 0.78f), new Color(0.9f, 0.9f, 0.9f));
        
        // Load UI sprites
        Sprite buttonSprite = LoadSprite(uiSpritePath, "UiCozyFree_1"); // Nút PLAY lớn
        
        // Buttons Container
        GameObject buttonsContainer = new GameObject("ButtonsContainer");
        buttonsContainer.transform.SetParent(mainMenuPanel.transform, false);
        RectTransform containerRect = buttonsContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.3f);
        containerRect.anchorMax = new Vector2(0.5f, 0.65f);
        containerRect.sizeDelta = new Vector2(300, 400);
        
        VerticalLayoutGroup layout = buttonsContainer.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 20;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        
        // Create Buttons
        Button newGameBtn = CreateMenuButton(buttonsContainer.transform, "NewGameButton", "CHƠI MỚI", buttonSprite);
        Button continueBtn = CreateMenuButton(buttonsContainer.transform, "ContinueButton", "TIẾP TỤC", buttonSprite);
        Button settingsBtn = CreateMenuButton(buttonsContainer.transform, "SettingsButton", "CÀI ĐẶT", buttonSprite);
        Button quitBtn = CreateMenuButton(buttonsContainer.transform, "QuitButton", "THOÁT", buttonSprite);
        
        // 5. Tạo Settings Panel (hidden by default)
        GameObject settingsPanel = CreatePanel(uiCanvasObj.transform, "SettingsPanel");
        settingsPanel.SetActive(false);
        
        // Settings overlay
        GameObject settingsOverlay = new GameObject("DarkOverlay");
        settingsOverlay.transform.SetParent(settingsPanel.transform, false);
        Image settingsOverlayImg = settingsOverlay.AddComponent<Image>();
        settingsOverlayImg.color = new Color(0, 0, 0, 0.7f);
        settingsOverlayImg.raycastTarget = false;
        RectTransform settingsOverlayRect = settingsOverlay.GetComponent<RectTransform>();
        settingsOverlayRect.anchorMin = Vector2.zero;
        settingsOverlayRect.anchorMax = Vector2.one;
        settingsOverlayRect.sizeDelta = Vector2.zero;
        
        // Settings Title
        CreateText(settingsPanel.transform, "SettingsTitle", "CÀI ĐẶT", 48,
            new Vector2(0.5f, 0.8f), new Vector2(0.5f, 0.8f), Color.white);
        
        // Music Volume
        CreateText(settingsPanel.transform, "MusicLabel", "Âm nhạc", 24,
            new Vector2(0.35f, 0.6f), new Vector2(0.35f, 0.6f), Color.white);
        Slider musicSlider = CreateSlider(settingsPanel.transform, "MusicVolumeSlider",
            new Vector2(0.55f, 0.6f), new Vector2(0.55f, 0.6f));
        
        // SFX Volume
        CreateText(settingsPanel.transform, "SFXLabel", "Hiệu ứng", 24,
            new Vector2(0.35f, 0.5f), new Vector2(0.35f, 0.5f), Color.white);
        Slider sfxSlider = CreateSlider(settingsPanel.transform, "SFXVolumeSlider",
            new Vector2(0.55f, 0.5f), new Vector2(0.55f, 0.5f));
        
        // Back Button
        Button backBtn = CreateMenuButton(settingsPanel.transform, "BackButton", "QUAY LẠI", buttonSprite);
        RectTransform backRect = backBtn.GetComponent<RectTransform>();
        backRect.anchorMin = new Vector2(0.5f, 0.25f);
        backRect.anchorMax = new Vector2(0.5f, 0.25f);
        backRect.sizeDelta = new Vector2(200, 50);
        
        // 6. Tạo MainMenuController
        GameObject controllerObj = new GameObject("MainMenuController");
        MainMenuController controller = controllerObj.AddComponent<MainMenuController>();
        
        // Gán references qua SerializedObject
        SerializedObject so = new SerializedObject(controller);
        so.FindProperty("videoPlayer").objectReferenceValue = videoPlayer;
        so.FindProperty("videoRawImage").objectReferenceValue = videoRawImage;
        so.FindProperty("mainMenuPanel").objectReferenceValue = mainMenuPanel;
        so.FindProperty("settingsPanel").objectReferenceValue = settingsPanel;
        so.FindProperty("newGameButton").objectReferenceValue = newGameBtn;
        so.FindProperty("continueButton").objectReferenceValue = continueBtn;
        so.FindProperty("settingsButton").objectReferenceValue = settingsBtn;
        so.FindProperty("quitButton").objectReferenceValue = quitBtn;
        so.FindProperty("musicVolumeSlider").objectReferenceValue = musicSlider;
        so.FindProperty("sfxVolumeSlider").objectReferenceValue = sfxSlider;
        so.FindProperty("backButton").objectReferenceValue = backBtn;
        so.ApplyModifiedProperties();
        
        // 7. Tạo AudioSource cho BGM
        AudioSource bgmSource = controllerObj.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
        
        SerializedObject so2 = new SerializedObject(controller);
        so2.FindProperty("bgmSource").objectReferenceValue = bgmSource;
        so2.ApplyModifiedProperties();
        
        // 8. Save scene
        string scenePath = "Assets/Scenes/MainMenuScene.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);
        
        Debug.Log($"[MainMenuCreator] ✅ Đã tạo MainMenuScene tại: {scenePath}");
        Debug.Log("[MainMenuCreator] ⚠️ Nhớ thêm scene vào Build Settings (File → Build Settings → Add Open Scenes)");
        
        EditorUtility.DisplayDialog("Thành công!", 
            $"Đã tạo MainMenuScene!\n\nĐường dẫn: {scenePath}\n\n⚠️ Nhớ thêm scene vào Build Settings và đặt làm scene đầu tiên (index 0)!", 
            "OK");
    }
    
    private GameObject CreatePanel(Transform parent, string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        return panel;
    }
    
    private GameObject CreateText(Transform parent, string name, string text, int fontSize, 
        Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        Text textComp = textObj.AddComponent<Text>();
        textComp.text = text;
        textComp.fontSize = fontSize;
        textComp.color = color;
        textComp.alignment = TextAnchor.MiddleCenter;
        textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        // Add outline for better visibility
        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);
        
        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.sizeDelta = new Vector2(600, 100);
        rect.anchoredPosition = Vector2.zero;
        
        return textObj;
    }
    
    private Button CreateMenuButton(Transform parent, string name, string text, Sprite sprite)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        Image image = buttonObj.AddComponent<Image>();
        if (sprite != null)
        {
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
        }
        else
        {
            image.color = new Color(0.8f, 0.7f, 0.5f); // Fallback color
        }
        
        Button button = buttonObj.AddComponent<Button>();
        
        // Button colors
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 1f, 0.8f);
        colors.pressedColor = new Color(0.8f, 0.8f, 0.6f);
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        button.colors = colors;
        
        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(250, 60);
        
        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        Text textComp = textObj.AddComponent<Text>();
        textComp.text = text;
        textComp.fontSize = 28;
        textComp.color = new Color(0.3f, 0.2f, 0.1f); // Dark brown
        textComp.alignment = TextAnchor.MiddleCenter;
        textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComp.fontStyle = FontStyle.Bold;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        return button;
    }
    
    private Slider CreateSlider(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent, false);
        
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        
        RectTransform rect = sliderObj.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.sizeDelta = new Vector2(200, 30);
        
        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0, 0.25f);
        fillAreaRect.anchorMax = new Vector2(1, 0.75f);
        fillAreaRect.sizeDelta = new Vector2(-20, 0);
        
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.8f, 0.7f, 0.5f);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.sizeDelta = Vector2.zero;
        
        // Handle
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.sizeDelta = new Vector2(-20, 0);
        
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 0);
        
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        
        return slider;
    }
    
    private Sprite LoadSprite(string path, string spriteName)
    {
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
        foreach (Object obj in sprites)
        {
            if (obj is Sprite sprite && sprite.name == spriteName)
            {
                return sprite;
            }
        }
        return null;
    }
}
#endif
