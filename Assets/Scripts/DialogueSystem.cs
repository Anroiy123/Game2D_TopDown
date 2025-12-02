using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel; // Panel chứa toàn bộ UI dialogue
    [SerializeField] private Text npcNameText; // Hiển thị tên NPC
    [SerializeField] private Text dialogueText; // Hiển thị nội dung hội thoại
    [SerializeField] private GameObject continueIcon; // Icon "Nhấn E để tiếp tục"

    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.05f; // Tốc độ hiện text (typewriter effect)
    [SerializeField] private KeyCode continueKey = KeyCode.E;

    private string[] currentDialogueLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool dialogueActive = false;
    private Action onDialogueComplete;

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
        if (npcNameText != null)
        {
            npcNameText.gameObject.SetActive(false);
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

        // Nhấn phím để tiếp tục hoặc bỏ qua typing
        if (Input.GetKeyDown(continueKey))
        {
            if (isTyping)
            {
                // Bỏ qua animation typing, hiện full text luôn
                StopAllCoroutines();
                dialogueText.text = currentDialogueLines[currentLineIndex];
                isTyping = false;
                ShowContinueIcon();
            }
            else
            {
                // Chuyển sang dòng tiếp theo
                NextLine();
            }
        }
    }

    public void StartDialogue(string npcName, string[] dialogueLines, Action onComplete = null)
    {
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
        if (npcNameText != null)
        {
            npcNameText.text = npcName;
            npcNameText.gameObject.SetActive(true); // Đảm bảo nó được bật
            Debug.Log($"Set NPC Name: {npcName}, Active: {npcNameText.gameObject.activeSelf}");
        }
        else
        {
            Debug.LogError("NPCNameText is NULL! Please assign it in DialogueSystem Inspector.");
        }

        // Hiển thị dòng đầu tiên
        DisplayLine();
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
        ShowContinueIcon();
    }

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

        // Ẩn panel
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // Ẩn tên NPC
        if (npcNameText != null)
        {
            npcNameText.gameObject.SetActive(false);
        }

        // Gọi callback
        onDialogueComplete?.Invoke();
    }

    // Public method để force đóng dialogue (nếu cần)
    public void CloseDialogue()
    {
        StopAllCoroutines();
        EndDialogue();
    }
}
