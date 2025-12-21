using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel; // Panel chứa toàn bộ UI dialogue
    [FormerlySerializedAs("npcNameText")]
    [SerializeField] private Text speakerNameText; // Hiển thị tên người nói (NPC hoặc Player)
    [SerializeField] private Text dialogueText; // Hiển thị nội dung hội thoại
    [SerializeField] private GameObject continueIcon; // Icon "Nhấn E để tiếp tục"

    [Header("Choice UI References")]
    [SerializeField] private GameObject choicePanel; // Panel chứa các lựa chọn
    [SerializeField] private GameObject choiceButtonPrefab; // Prefab cho mỗi nút lựa chọn
    [SerializeField] private Transform choiceContainer; // Container để spawn các nút

    [Header("Avatar Display (Top-Down Mode)")]
    [Tooltip("Container cho avatar - hiển thị bên trái dialogue panel")]
    [SerializeField] private GameObject avatarContainer;
    [Tooltip("Image hiển thị avatar của speaker hiện tại")]
    [SerializeField] private Image avatarImage;
    [Tooltip("Vị trí avatar (Left/Right)")]
    [SerializeField] private AvatarPosition avatarPosition = AvatarPosition.Right;
    [Tooltip("Flip avatar theo chiều ngang (mặc định: true để nhân vật quay vào trong)")]
    [SerializeField] private bool flipAvatarHorizontal = true;

    public enum AvatarPosition { Left, Right }

    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.05f; // Tốc độ hiện text (typewriter effect)
    [SerializeField] private KeyCode continueKey = KeyCode.E;
    [SerializeField] private Color playerNameColor = new Color(0.2f, 0.6f, 1f); // Màu tên player
    [SerializeField] private Color npcNameColor = Color.white; // Màu tên NPC

    // Legacy mode variables (for backward compatibility)
    private string[] currentDialogueLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool dialogueActive = false;
    private Action onDialogueComplete;

    // New dialogue system variables
    private DialogueData currentDialogueData;
    private DialogueNode currentNode;
    private int currentLineInNode = 0;
    private bool isShowingChoices = false;
    private List<GameObject> spawnedChoiceButtons = new List<GameObject>();
    private Action<string> onChoiceAction; // Callback khi player chọn một action đặc biệt
    private Action<VNSceneData> onVNSceneTransition; // Callback khi chuyển sang VN scene khác
    private Action onEndVNMode; // Callback khi kết thúc VN mode

    // Avatar mode variables
    private bool isAvatarModeActive = false;
    private Dictionary<string, Sprite> currentAvatarMap;
    private Dictionary<string, bool> currentAvatarFlipMap; // Flip setting per speaker
    private bool lockPlayerDuringDialogue = true;
    private GameObject cachedPlayer;
    private bool currentFlipSetting = true; // Default flip setting

    // Event khi speaker thay đổi (để VNManager có thể ẩn/hiện character)
    public event Action<string> OnSpeakerChanged;

    /// <summary>
    /// Lấy tên speaker hiện tại
    /// </summary>
    public string CurrentSpeaker => currentNode?.speakerName ?? "";

    private void Awake()
    {
        SetupUI();
    }

    private void Start()
    {
        // Ẩn dialogue panel lúc khởi đầu
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (continueIcon != null)
        {
            continueIcon.SetActive(false);
        }

        // Ẩn tên NPC lúc khởi đầu
        if (speakerNameText != null)
        {
            speakerNameText.gameObject.SetActive(false);
        }

        // Ẩn choice panel
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        // Ẩn avatar container
        if (avatarContainer != null)
        {
            avatarContainer.SetActive(false);
        }
    }

    private void SetupUI()
    {
        // Force canvas sorting order to be above VN canvas
        // Tìm Canvas: trước tiên trên chính object này, sau đó trong parent hierarchy
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }
        
        // Nếu vẫn không tìm thấy, tìm DialogueCanvas trong scene
        if (canvas == null)
        {
            GameObject dialogueCanvasObj = GameObject.Find("DialogueCanvas");
            if (dialogueCanvasObj != null)
            {
                canvas = dialogueCanvasObj.GetComponent<Canvas>();
                if (canvas == null)
                {
                    // DialogueCanvas tồn tại nhưng không có Canvas component - thêm vào
                    Debug.Log("[DialogueSystem] DialogueCanvas found but has no Canvas component, adding...");
                    canvas = dialogueCanvasObj.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    
                    var scaler = dialogueCanvasObj.AddComponent<CanvasScaler>();
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920, 1080);
                    scaler.matchWidthOrHeight = 0.5f;
                    
                    dialogueCanvasObj.AddComponent<GraphicRaycaster>();
                }
                
                // Di chuyển DialogueSystem vào DialogueCanvas nếu chưa
                if (transform.parent != dialogueCanvasObj.transform)
                {
                    Debug.Log("[DialogueSystem] Moving DialogueSystem under DialogueCanvas...");
                    transform.SetParent(dialogueCanvasObj.transform, false);
                }
            }
        }
        
        if (canvas != null)
        {
            canvas.sortingOrder = 400; // Above VN canvas (300)
            canvas.overrideSorting = true; // Đảm bảo sorting order được áp dụng
            Debug.Log($"[DialogueSystem] Canvas sorting order set to {canvas.sortingOrder}");
        }
        else
        {
            // Không tìm thấy Canvas - tạo Canvas mới trên chính object này
            Debug.LogWarning("[DialogueSystem] No Canvas found anywhere! Creating Canvas on this object...");
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 400; // Above VN canvas (300)
            
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            gameObject.AddComponent<GraphicRaycaster>();
            Debug.Log($"[DialogueSystem] Canvas created with sorting order {canvas.sortingOrder}");
        }

        // Auto-create UI if not assigned
        if (dialoguePanel == null || speakerNameText == null || dialogueText == null)
        {
            Debug.Log("[DialogueSystem] Auto-creating UI...");
            CreateDialogueUI();
        }
    }

    private void Update()
    {
        if (!dialogueActive) return;

        // Nhấn ESC để thoát dialog
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDialogue();
            return;
        }

        // Xử lý input khi đang hiện choices - cho phép nhấn số để chọn
        if (isShowingChoices)
        {
            HandleChoiceInput();
            return;
        }

        // Nhấn phím để tiếp tục hoặc bỏ qua typing
        if (Input.GetKeyDown(continueKey))
        {
            if (isTyping)
            {
                // Bỏ qua animation typing, hiện full text luôn
                StopAllCoroutines();
                if (currentDialogueData != null && currentNode != null)
                {
                    dialogueText.text = currentNode.dialogueLines[currentLineInNode];
                }
                else
                {
                    dialogueText.text = currentDialogueLines[currentLineIndex];
                }
                isTyping = false;
                
                // Kiểm tra xem có cần hiện choices không
                if (currentDialogueData != null && currentNode != null)
                {
                    CheckAndShowChoices();
                }
                else
                {
                    ShowContinueIcon();
                }
            }
            else
            {
                // Chuyển sang dòng tiếp theo
                if (currentDialogueData != null)
                {
                    NextLineInNode();
                }
                else
                {
                    NextLine();
                }
            }
        }
    }

    /// <summary>
    /// Legacy method - Bắt đầu dialogue đơn giản (chỉ NPC nói)
    /// </summary>
    public void StartDialogue(string npcName, string[] dialogueLines, Action onComplete = null)
    {
        // Reset new dialogue system
        currentDialogueData = null;
        currentNode = null;

        // Lưu callback
        onDialogueComplete = onComplete;

        // Lưu dialogue lines
        currentDialogueLines = dialogueLines;
        currentLineIndex = 0;
        dialogueActive = true;

        // Hiển thị panel
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        // Set tên NPC
        if (speakerNameText != null)
        {
            speakerNameText.text = npcName;
            speakerNameText.color = npcNameColor;
            speakerNameText.gameObject.SetActive(true);
            Debug.Log($"Set NPC Name: {npcName}, Active: {speakerNameText.gameObject.activeSelf}");
        }
        else
        {
            Debug.LogError("SpeakerNameText is NULL! Please assign it in DialogueSystem Inspector.");
        }

        // Ẩn choice panel
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        // Hiển thị dòng đầu tiên
        DisplayLine();
    }

    /// <summary>
    /// New method - Bắt đầu dialogue với choices
    /// </summary>
    public void StartDialogueWithChoices(
        DialogueData dialogueData,
        Action onComplete = null,
        Action<string> onAction = null,
        Action<VNSceneData> onVNTransition = null,
        Action onEndVN = null)
    {
        if (dialogueData == null || dialogueData.nodes == null || dialogueData.nodes.Length == 0)
        {
            Debug.LogError("DialogueData is null or empty!");
            return;
        }

        // Reset legacy system
        currentDialogueLines = null;
        currentLineIndex = 0;

        // Setup new dialogue
        currentDialogueData = dialogueData;
        onDialogueComplete = onComplete;
        onChoiceAction = onAction;
        onVNSceneTransition = onVNTransition;
        onEndVNMode = onEndVN;
        dialogueActive = true;
        isShowingChoices = false;

        // Hiển thị panel
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        // Ẩn choice panel ban đầu
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        // Bắt đầu từ node đầu tiên
        GoToNode(dialogueData.startNodeId);
    }

    /// <summary>
    /// Bắt đầu dialogue với avatar support (cho top-down mode)
    /// Background = scene top-down hiện tại, chỉ hiển thị dialogue panel + avatar
    /// </summary>
    /// <param name="dialogueData">DialogueData chứa nội dung hội thoại</param>
    /// <param name="avatarMap">Dictionary mapping speaker name → avatar sprite</param>
    /// <param name="avatarFlipMap">Dictionary mapping speaker name → flip horizontal (null = dùng default)</param>
    /// <param name="lockPlayer">Có khóa player movement không (default: true)</param>
    /// <param name="onComplete">Callback khi dialogue kết thúc</param>
    /// <param name="onAction">Callback khi có action đặc biệt</param>
    public void StartDialogueWithAvatars(
        DialogueData dialogueData,
        Dictionary<string, Sprite> avatarMap,
        Dictionary<string, bool> avatarFlipMap = null,
        bool lockPlayer = true,
        Action onComplete = null,
        Action<string> onAction = null)
    {
        if (dialogueData == null || dialogueData.nodes == null || dialogueData.nodes.Length == 0)
        {
            Debug.LogError("[DialogueSystem] DialogueData is null or empty!");
            onComplete?.Invoke();
            return;
        }

        Debug.Log($"[DialogueSystem] Starting dialogue with avatars: {dialogueData.name}, avatars={avatarMap?.Count ?? 0}");

        // Setup avatar mode
        isAvatarModeActive = true;
        currentAvatarMap = avatarMap;
        currentAvatarFlipMap = avatarFlipMap;
        lockPlayerDuringDialogue = lockPlayer;

        // Lock player movement nếu cần
        if (lockPlayer)
        {
            LockPlayerMovement(true);
        }

        // Setup avatar UI
        SetupAvatarUI();
        if (avatarContainer != null)
        {
            avatarContainer.SetActive(true);
        }

        // Reset legacy system
        currentDialogueLines = null;
        currentLineIndex = 0;

        // Setup new dialogue
        currentDialogueData = dialogueData;
        onDialogueComplete = onComplete;
        onChoiceAction = onAction;
        onVNSceneTransition = null; // Không dùng VN transition trong avatar mode
        onEndVNMode = null;
        dialogueActive = true;
        isShowingChoices = false;

        // Hiển thị panel
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        // Ẩn choice panel ban đầu
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        // Bắt đầu từ node đầu tiên
        GoToNode(dialogueData.startNodeId);
    }

    /// <summary>
    /// Setup Avatar UI - tạo nếu chưa có
    /// </summary>
    private void SetupAvatarUI()
    {
        if (avatarContainer != null && avatarImage != null) return;

        Debug.Log("[DialogueSystem] Auto-creating Avatar UI...");

        // Tìm Canvas parent - ưu tiên từ dialoguePanel
        Transform canvasParent = null;
        if (dialoguePanel != null)
        {
            // Tìm Canvas trong hierarchy của dialoguePanel
            Canvas canvas = dialoguePanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvasParent = canvas.transform;
            }
        }
        
        // Fallback: tìm Canvas trong scene
        if (canvasParent == null)
        {
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (var c in canvases)
            {
                if (c.renderMode == RenderMode.ScreenSpaceOverlay || c.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    canvasParent = c.transform;
                    break;
                }
            }
        }

        if (canvasParent == null)
        {
            Debug.LogError("[DialogueSystem] Cannot find Canvas for Avatar UI!");
            return;
        }

        // Tìm hoặc tạo avatar container
        if (avatarContainer == null)
        {
            avatarContainer = new GameObject("AvatarContainer");
            avatarContainer.transform.SetParent(canvasParent, false);
            
            // Đặt avatar TRƯỚC dialogue panel trong hierarchy (render phía sau)
            if (dialoguePanel != null)
            {
                int dialogueIndex = dialoguePanel.transform.GetSiblingIndex();
                avatarContainer.transform.SetSiblingIndex(dialogueIndex);
            }
            else
            {
                avatarContainer.transform.SetAsFirstSibling();
            }

            RectTransform containerRect = avatarContainer.AddComponent<RectTransform>();
            
            // Avatar to hơn, nằm bên trái/phải, dịch lên trên dialogue panel
            if (avatarPosition == AvatarPosition.Left)
            {
                containerRect.anchorMin = new Vector2(0f, 0.15f);
                containerRect.anchorMax = new Vector2(0.35f, 0.85f);
                containerRect.offsetMin = new Vector2(10f, 0f);
                containerRect.offsetMax = new Vector2(0f, 0f);
            }
            else // Right
            {
                containerRect.anchorMin = new Vector2(0.65f, 0.15f);
                containerRect.anchorMax = new Vector2(1f, 0.85f);
                containerRect.offsetMin = new Vector2(0f, 0f);
                containerRect.offsetMax = new Vector2(-10f, 0f);
            }
        }

        // Tạo avatar image
        if (avatarImage == null)
        {
            GameObject avatarObj = new GameObject("AvatarImage");
            avatarObj.transform.SetParent(avatarContainer.transform, false);

            avatarImage = avatarObj.AddComponent<Image>();
            avatarImage.preserveAspect = true;
            avatarImage.raycastTarget = false;

            RectTransform avatarRect = avatarObj.GetComponent<RectTransform>();
            avatarRect.anchorMin = Vector2.zero;
            avatarRect.anchorMax = Vector2.one;
            avatarRect.sizeDelta = Vector2.zero;
            avatarRect.pivot = new Vector2(0.5f, 0f); // Pivot ở bottom-center
        }

        Debug.Log("[DialogueSystem] Avatar UI created successfully!");
    }

    /// <summary>
    /// Cập nhật avatar dựa trên speaker hiện tại
    /// </summary>
    private void UpdateAvatarForSpeaker(string speakerName)
    {
        if (!isAvatarModeActive || avatarImage == null) return;

        if (currentAvatarMap != null && !string.IsNullOrEmpty(speakerName))
        {
            if (currentAvatarMap.TryGetValue(speakerName, out Sprite avatar))
            {
                avatarImage.sprite = avatar;
                avatarImage.color = Color.white;
                avatarImage.gameObject.SetActive(true);
                
                // Xử lý flip - kiểm tra flip map trước, nếu không có thì dùng default
                bool shouldFlip = flipAvatarHorizontal; // Default setting
                if (currentAvatarFlipMap != null && currentAvatarFlipMap.TryGetValue(speakerName, out bool speakerFlip))
                {
                    shouldFlip = speakerFlip;
                }
                
                // Apply flip bằng cách scale X âm
                RectTransform rect = avatarImage.GetComponent<RectTransform>();
                if (rect != null)
                {
                    Vector3 scale = rect.localScale;
                    scale.x = shouldFlip ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
                    rect.localScale = scale;
                }
                
                Debug.Log($"[DialogueSystem] Avatar updated for speaker: {speakerName}, flip: {shouldFlip}");
            }
            else
            {
                // Không có avatar cho speaker này - ẩn đi
                avatarImage.gameObject.SetActive(false);
                Debug.Log($"[DialogueSystem] No avatar found for speaker: {speakerName}");
            }
        }
        else
        {
            // Narration hoặc không có avatar map - ẩn avatar
            avatarImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Lock/Unlock player movement
    /// </summary>
    private void LockPlayerMovement(bool locked)
    {
        if (cachedPlayer == null)
        {
            cachedPlayer = GameObject.FindGameObjectWithTag("Player");
        }

        if (cachedPlayer != null)
        {
            var movement = cachedPlayer.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                if (locked)
                {
                    movement.SetTalkingState(true);
                }
                else
                {
                    movement.SetTalkingState(false);
                }
                Debug.Log($"[DialogueSystem] Player movement locked: {locked}");
            }
        }
    }

    private void DisplayLine()
    {
        if (currentLineIndex >= currentDialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        // Ẩn continue icon
        if (continueIcon != null)
        {
            continueIcon.SetActive(false);
        }

        // Bắt đầu typewriter effect
        StartCoroutine(TypeLine(currentDialogueLines[currentLineIndex]));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;

        // Kiểm tra xem có cần hiện choices không (cho new system)
        if (currentDialogueData != null && currentNode != null)
        {
            CheckAndShowChoices();
        }
        else
        {
            ShowContinueIcon();
        }
    }

    #region New Dialogue System Methods

    /// <summary>
    /// Chuyển đến một node cụ thể
    /// </summary>
    private void GoToNode(int nodeId)
    {
        if (nodeId == -1)
        {
            EndDialogue();
            return;
        }

        currentNode = currentDialogueData.GetNodeById(nodeId);
        if (currentNode == null)
        {
            Debug.LogError($"Node with ID {nodeId} not found!");
            EndDialogue();
            return;
        }

        currentLineInNode = 0;
        ClearChoiceButtons();

        // Ẩn choice panel
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }
        isShowingChoices = false;

        // Apply on enter effects (set flags, variable changes)
        currentNode.ApplyOnEnterEffects();

        // Fire event khi speaker thay đổi
        OnSpeakerChanged?.Invoke(currentNode.speakerName);

        // Cập nhật avatar nếu đang ở avatar mode
        UpdateAvatarForSpeaker(currentNode.speakerName);

        // Hiển thị dòng đầu tiên của node
        DisplayNodeLine();
    }

    /// <summary>
    /// Hiển thị dòng hiện tại trong node
    /// </summary>
    private void DisplayNodeLine()
    {
        if (currentNode == null || currentLineInNode >= currentNode.dialogueLines.Length)
        {
            // Hết lines trong node, kiểm tra choices hoặc chuyển node
            HandleNodeEnd();
            return;
        }

        // Ẩn continue icon
        if (continueIcon != null)
        {
            continueIcon.SetActive(false);
        }

        // Cập nhật tên người nói
        if (speakerNameText != null)
        {
            speakerNameText.text = currentNode.speakerName;
            speakerNameText.color = currentNode.isPlayerSpeaking ? playerNameColor : npcNameColor;
            speakerNameText.gameObject.SetActive(true);
        }

        // Bắt đầu typewriter effect
        StartCoroutine(TypeLine(currentNode.dialogueLines[currentLineInNode]));
    }

    /// <summary>
    /// Chuyển sang dòng tiếp theo trong node
    /// </summary>
    private void NextLineInNode()
    {
        currentLineInNode++;
        DisplayNodeLine();
    }

    /// <summary>
    /// Kiểm tra và hiện choices nếu cần
    /// </summary>
    private void CheckAndShowChoices()
    {
        if (currentNode == null) return;

        // Chỉ hiện choices khi ở dòng cuối cùng của node
        bool isLastLine = currentLineInNode >= currentNode.dialogueLines.Length - 1;
        bool hasChoices = currentNode.choices != null && currentNode.choices.Length > 0;

        if (isLastLine && hasChoices)
        {
            ShowChoices();
        }
        else
        {
            ShowContinueIcon();
        }
    }

    /// <summary>
    /// Xử lý khi hết lines trong một node
    /// </summary>
    private void HandleNodeEnd()
    {
        if (currentNode == null)
        {
            EndDialogue();
            return;
        }

        // Nếu có choices thì đã được hiện rồi, không làm gì thêm
        if (isShowingChoices) return;

        // Nếu không có choices, chuyển đến nextNode
        GoToNode(currentNode.nextNodeId);
    }

    /// <summary>
    /// Hiển thị các lựa chọn
    /// </summary>
    private void ShowChoices()
    {
        if (currentNode == null || currentNode.choices == null)
        {
            Debug.LogWarning($"[DialogueSystem] ShowChoices: currentNode={currentNode != null}, choices={currentNode?.choices != null}");
            return;
        }

        isShowingChoices = true;

        // Ẩn continue icon
        if (continueIcon != null)
        {
            continueIcon.SetActive(false);
        }

        // Hiện choice panel
        if (choicePanel != null)
        {
            choicePanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[DialogueSystem] ChoicePanel is NULL!");
        }

        // Tạo các button cho mỗi choice (filter theo conditions)
        ClearChoiceButtons();

        var availableChoices = currentNode.GetAvailableChoices();

        for (int i = 0; i < availableChoices.Length; i++)
        {
            DialogueChoice choice = availableChoices[i];
            CreateChoiceButton(choice, i);
        }
    }

    /// <summary>
    /// Tạo button cho một choice
    /// </summary>
    private void CreateChoiceButton(DialogueChoice choice, int index)
    {
        // Auto-create choicePanel if missing (use ReferenceEquals for proper Unity null check)
        if (choicePanel == null || !choicePanel)
        {
            Debug.Log("[DialogueSystem] Auto-creating ChoicePanel...");
            choicePanel = new GameObject("ChoicePanel");
            choicePanel.transform.SetParent(transform, false);

            RectTransform choicePanelRect = choicePanel.AddComponent<RectTransform>();
            choicePanelRect.anchorMin = new Vector2(0.2f, 0.4f);
            choicePanelRect.anchorMax = new Vector2(0.8f, 0.95f);
            choicePanelRect.sizeDelta = Vector2.zero;

            // Reset choiceContainer since we just created a new panel
            choiceContainer = null;
        }

        // Auto-create choiceButtonPrefab if missing
        if (choiceButtonPrefab == null || !choiceButtonPrefab)
        {
            choiceButtonPrefab = CreateChoiceButtonPrefab();
        }

        // Auto-create choiceContainer if missing
        if (choiceContainer == null || !choiceContainer)
        {
            Debug.Log("[DialogueSystem] Auto-creating ChoiceContainer...");
            GameObject containerObj = new GameObject("ChoiceContainer");
            containerObj.transform.SetParent(choicePanel.transform, false);

            // VerticalLayoutGroup tự động thêm RectTransform, nên phải GetComponent thay vì AddComponent
            var layoutGroup = containerObj.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 20;
            layoutGroup.padding = new RectOffset(20, 20, 20, 20);
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;

            RectTransform containerRect = containerObj.GetComponent<RectTransform>();
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.sizeDelta = Vector2.zero;

            choiceContainer = containerObj.transform;
        }

        if (choiceButtonPrefab == null || !choiceButtonPrefab || choiceContainer == null || !choiceContainer)
        {
            Debug.LogError($"ChoiceButtonPrefab or ChoiceContainer could not be created! prefab={choiceButtonPrefab != null}, container={choiceContainer != null}");
            return;
        }

        GameObject buttonObj = Instantiate(choiceButtonPrefab, choiceContainer);
        buttonObj.SetActive(true); // Đảm bảo button active vì prefab có thể bị inactive
        spawnedChoiceButtons.Add(buttonObj);

        // Setup text
        Text buttonText = buttonObj.GetComponentInChildren<Text>(true); // includeInactive=true
        if (buttonText != null)
        {
            buttonText.text = $"{index + 1}. {choice.choiceText}";
        }

        // Setup button click
        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            // Capture choice reference for ApplyEffects()
            DialogueChoice capturedChoice = choice;

            button.onClick.AddListener(() => {
                // Apply choice effects BEFORE processing choice
                capturedChoice.ApplyEffects();
                OnChoiceSelected(capturedChoice.nextNodeId, capturedChoice.actionId, capturedChoice.nextVNScene, capturedChoice.endVNMode);
            });
        }
    }

    /// <summary>
    /// Xử lý khi player chọn một choice
    /// </summary>
    private void OnChoiceSelected(int nextNodeId, string actionId, VNSceneData nextVNScene, bool endVNMode)
    {
        // Priority 1: VN Scene transition
        if (nextVNScene != null)
        {
            // Notify VNManager to transition to next VN scene
            onVNSceneTransition?.Invoke(nextVNScene);
            return; // Don't go to next node, VN scene will handle it
        }

        // Priority 2: End VN mode
        if (endVNMode)
        {
            // IMPORTANT: Gọi action callback TRƯỚC KHI end VN mode
            // Để VisualNovelManager có thể xử lý (ví dụ: dừng bullies)
            if (!string.IsNullOrEmpty(actionId))
            {
                onChoiceAction?.Invoke(actionId);
            }

            // End dialogue first to prevent dialogue overlay on top-down mode
            EndDialogue();
            // Then notify VNManager to end VN mode
            onEndVNMode?.Invoke();
            return;
        }

        // Priority 3: Action callback (legacy)
        if (!string.IsNullOrEmpty(actionId))
        {
            onChoiceAction?.Invoke(actionId);
        }

        // Continue dialogue
        GoToNode(nextNodeId);
    }

    /// <summary>
    /// Xử lý input phím số để chọn lựa chọn
    /// </summary>
    private void HandleChoiceInput()
    {
        if (currentNode == null || currentNode.choices == null) return;

        int choiceCount = currentNode.choices.Length;

        // Kiểm tra phím số 1-9 (hỗ trợ tối đa 9 lựa chọn)
        for (int i = 0; i < Mathf.Min(choiceCount, 9); i++)
        {
            // Kiểm tra phím số hàng trên (Alpha1, Alpha2, ...)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectChoice(i);
                return;
            }

            // Kiểm tra phím số Numpad (Keypad1, Keypad2, ...)
            if (Input.GetKeyDown(KeyCode.Keypad1 + i))
            {
                SelectChoice(i);
                return;
            }
        }
    }

    /// <summary>
    /// Chọn một lựa chọn theo index
    /// </summary>
    private void SelectChoice(int index)
    {
        if (currentNode == null || currentNode.choices == null) return;
        if (index < 0 || index >= currentNode.choices.Length) return;

        DialogueChoice choice = currentNode.choices[index];

        // IMPORTANT: Apply choice effects (set flags, change variables) BEFORE processing choice
        choice.ApplyEffects();

        OnChoiceSelected(choice.nextNodeId, choice.actionId, choice.nextVNScene, choice.endVNMode);
    }

    /// <summary>
    /// Xóa tất cả choice buttons
    /// </summary>
    private void ClearChoiceButtons()
    {
        foreach (var button in spawnedChoiceButtons)
        {
            if (button != null)
            {
                Destroy(button);
            }
        }
        spawnedChoiceButtons.Clear();
    }

    #endregion

    private void ShowContinueIcon()
    {
        if (continueIcon != null)
        {
            continueIcon.SetActive(true);
        }
    }

    private void NextLine()
    {
        currentLineIndex++;
        DisplayLine();
    }

    private void EndDialogue()
    {
        dialogueActive = false;
        isShowingChoices = false;

        // Xóa choice buttons
        ClearChoiceButtons();

        // Ẩn panel
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // Ẩn choice panel
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        // Ẩn tên người nói
        if (speakerNameText != null)
        {
            speakerNameText.gameObject.SetActive(false);
        }

        // Cleanup avatar mode
        if (isAvatarModeActive)
        {
            if (avatarContainer != null)
            {
                avatarContainer.SetActive(false);
            }
            if (avatarImage != null)
            {
                avatarImage.sprite = null;
                avatarImage.gameObject.SetActive(false);
                
                // Reset scale (in case it was flipped)
                RectTransform rect = avatarImage.GetComponent<RectTransform>();
                if (rect != null)
                {
                    Vector3 scale = rect.localScale;
                    scale.x = Mathf.Abs(scale.x);
                    rect.localScale = scale;
                }
            }

            // Unlock player movement
            if (lockPlayerDuringDialogue)
            {
                LockPlayerMovement(false);
            }

            isAvatarModeActive = false;
            currentAvatarMap = null;
            currentAvatarFlipMap = null;
            cachedPlayer = null;
        }

        // Reset dialogue data
        currentDialogueData = null;
        currentNode = null;

        // Gọi callback
        onDialogueComplete?.Invoke();
    }

    // Public method để force đóng dialogue (nếu cần)
    public void CloseDialogue()
    {
        StopAllCoroutines();
        EndDialogue();
    }

    /// <summary>
    /// Kiểm tra xem dialogue có đang active không
    /// </summary>
    public bool IsDialogueActive()
    {
        return dialogueActive;
    }

    #region Auto UI Creation
    private void CreateDialogueUI()
    {
        // Create Canvas if needed
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 400; // Above VN canvas (300)

            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            gameObject.AddComponent<GraphicRaycaster>();
        }

        // Create DialoguePanel
        if (dialoguePanel == null)
        {
            dialoguePanel = new GameObject("DialoguePanel");
            dialoguePanel.transform.SetParent(transform, false);

            var panelImage = dialoguePanel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f);

            RectTransform panelRect = dialoguePanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.1f, 0.05f);
            panelRect.anchorMax = new Vector2(0.9f, 0.35f);
            panelRect.sizeDelta = Vector2.zero;
        }

        // Create SpeakerNameText
        if (speakerNameText == null)
        {
            GameObject nameObj = new GameObject("SpeakerName");
            nameObj.transform.SetParent(dialoguePanel.transform, false);

            speakerNameText = nameObj.AddComponent<Text>();
            speakerNameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            speakerNameText.fontSize = 32;
            speakerNameText.color = Color.white;
            speakerNameText.alignment = TextAnchor.MiddleLeft;

            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.05f, 0.7f);
            nameRect.anchorMax = new Vector2(0.5f, 0.95f);
            nameRect.sizeDelta = Vector2.zero;
        }

        // Create DialogueText
        if (dialogueText == null)
        {
            GameObject textObj = new GameObject("DialogueText");
            textObj.transform.SetParent(dialoguePanel.transform, false);

            dialogueText = textObj.AddComponent<Text>();
            dialogueText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            dialogueText.fontSize = 28;
            dialogueText.color = Color.white;
            dialogueText.alignment = TextAnchor.UpperLeft;

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.05f, 0.1f);
            textRect.anchorMax = new Vector2(0.95f, 0.65f);
            textRect.sizeDelta = Vector2.zero;
        }

        // Create ContinueIcon
        if (continueIcon == null)
        {
            continueIcon = new GameObject("ContinueIcon");
            continueIcon.transform.SetParent(dialoguePanel.transform, false);

            var iconText = continueIcon.AddComponent<Text>();
            iconText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            iconText.fontSize = 24;
            iconText.color = Color.yellow;
            iconText.text = "▼ Press E";
            iconText.alignment = TextAnchor.MiddleRight;

            RectTransform iconRect = continueIcon.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.7f, 0.05f);
            iconRect.anchorMax = new Vector2(0.95f, 0.2f);
            iconRect.sizeDelta = Vector2.zero;
        }

        // Create ChoicePanel
        if (choicePanel == null)
        {
            choicePanel = new GameObject("ChoicePanel");
            choicePanel.transform.SetParent(transform, false);

            RectTransform choicePanelRect = choicePanel.AddComponent<RectTransform>();
            choicePanelRect.anchorMin = new Vector2(0.2f, 0.4f);
            choicePanelRect.anchorMax = new Vector2(0.8f, 0.95f);
            choicePanelRect.sizeDelta = Vector2.zero;

            // Create ChoiceContainer with VerticalLayoutGroup
            if (choiceContainer == null)
            {
                GameObject containerObj = new GameObject("ChoiceContainer");
                containerObj.transform.SetParent(choicePanel.transform, false);

                choiceContainer = containerObj.transform;

                var layoutGroup = containerObj.AddComponent<VerticalLayoutGroup>();
                layoutGroup.spacing = 20;
                layoutGroup.padding = new RectOffset(20, 20, 20, 20);
                layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                layoutGroup.childControlHeight = false;
                layoutGroup.childControlWidth = true;
                layoutGroup.childForceExpandHeight = false;
                layoutGroup.childForceExpandWidth = true;

                RectTransform containerRect = containerObj.GetComponent<RectTransform>();
                containerRect.anchorMin = Vector2.zero;
                containerRect.anchorMax = Vector2.one;
                containerRect.sizeDelta = Vector2.zero;
            }
        }

        // Create ChoiceButtonPrefab
        if (choiceButtonPrefab == null)
        {
            choiceButtonPrefab = CreateChoiceButtonPrefab();
        }

        Debug.Log("[DialogueSystem] UI created successfully!");
    }

    private GameObject CreateChoiceButtonPrefab()
    {
        GameObject prefab = new GameObject("ChoiceButton");
        prefab.SetActive(false); // Keep it inactive as prefab

        var button = prefab.AddComponent<Button>();
        var image = prefab.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

        var buttonRect = prefab.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(600, 60);

        // Create Text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(prefab.transform, false);

        var text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 26;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        return prefab;
    }
    #endregion
}
