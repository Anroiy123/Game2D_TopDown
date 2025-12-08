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
    public void StartDialogueWithChoices(DialogueData dialogueData, Action onComplete = null, Action<string> onAction = null)
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

        // Debug: Kiểm tra text
        Debug.Log($"Typing line: {line}");

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
        if (currentNode == null || currentNode.choices == null) return;

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

        // Tạo các button cho mỗi choice
        ClearChoiceButtons();
        
        for (int i = 0; i < currentNode.choices.Length; i++)
        {
            DialogueChoice choice = currentNode.choices[i];
            CreateChoiceButton(choice, i);
        }
    }

    /// <summary>
    /// Tạo button cho một choice
    /// </summary>
    private void CreateChoiceButton(DialogueChoice choice, int index)
    {
        if (choiceButtonPrefab == null || choiceContainer == null)
        {
            Debug.LogError("ChoiceButtonPrefab or ChoiceContainer is not assigned!");
            return;
        }

        GameObject buttonObj = Instantiate(choiceButtonPrefab, choiceContainer);
        spawnedChoiceButtons.Add(buttonObj);

        // Setup text
        Text buttonText = buttonObj.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = $"{index + 1}. {choice.choiceText}";
        }

        // Setup button click
        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            int nextNodeId = choice.nextNodeId;
            string actionId = choice.actionId;
            
            button.onClick.AddListener(() => OnChoiceSelected(nextNodeId, actionId));
        }
    }

    /// <summary>
    /// Xử lý khi player chọn một choice
    /// </summary>
    private void OnChoiceSelected(int nextNodeId, string actionId)
    {
        Debug.Log($"Choice selected: nextNode={nextNodeId}, action={actionId}");

        // Trigger action callback nếu có
        if (!string.IsNullOrEmpty(actionId))
        {
            onChoiceAction?.Invoke(actionId);
        }

        // Chuyển đến node tiếp theo
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
        OnChoiceSelected(choice.nextNodeId, choice.actionId);
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
}
