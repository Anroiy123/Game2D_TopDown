using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple Interaction Prompt - Hiển thị "Press E" hoặc icon đơn giản
/// Dùng cho objects không cần animation phức tạp (cửa, giường, ghế, v.v.)
/// </summary>
public class SimpleInteractionPrompt : MonoBehaviour
{
    [Header("Prompt Settings")]
    [Tooltip("Sprite hiển thị (có thể là icon E key hoặc hand icon)")]
    [SerializeField] private Sprite promptSprite;

    [Tooltip("Hoặc dùng Text thay vì sprite")]
    [SerializeField] private bool useText = false;

    [Tooltip("Text hiển thị (ví dụ: 'Press E')")]
    [SerializeField] private string promptText = "E";

    [Header("Position")]
    [Tooltip("Offset từ vị trí object")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, 0f);

    [Header("Visibility")]
    [Tooltip("Chỉ hiện khi player ở gần")]
    [SerializeField] private float showDistance = 2f;

    [Tooltip("Fade in/out animation")]
    [SerializeField] private bool useFade = true;

    [Tooltip("Fade speed")]
    [SerializeField] private float fadeSpeed = 5f;

    // Components
    private GameObject promptObject;
    private SpriteRenderer spriteRenderer;
    private TextMesh textMesh;
    private Transform playerTransform;
    private float currentAlpha = 0f;
    private bool shouldShow = false;

    private void Start()
    {
        CreatePromptObject();
        FindPlayer();
    }

    private void Update()
    {
        if (promptObject == null) return;

        // Check distance to player
        UpdateVisibility();

        // Update fade
        if (useFade)
        {
            UpdateFade();
        }
        else
        {
            promptObject.SetActive(shouldShow);
        }

        // Update position
        promptObject.transform.position = transform.position + offset;
    }

    private void CreatePromptObject()
    {
        promptObject = new GameObject("InteractionPrompt");
        promptObject.transform.SetParent(transform);
        promptObject.transform.localPosition = offset;

        if (useText)
        {
            // Tạo TextMesh
            textMesh = promptObject.AddComponent<TextMesh>();
            textMesh.text = promptText;
            textMesh.fontSize = 20;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.color = Color.white;

            // Add MeshRenderer
            MeshRenderer meshRenderer = promptObject.GetComponent<MeshRenderer>();
            meshRenderer.sortingLayerName = "UI";
            meshRenderer.sortingOrder = 100;
        }
        else
        {
            // Tạo SpriteRenderer
            spriteRenderer = promptObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = promptSprite;
            spriteRenderer.sortingLayerName = "UI";
            spriteRenderer.sortingOrder = 100;
        }

        promptObject.SetActive(false);
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void UpdateVisibility()
    {
        if (playerTransform == null)
        {
            shouldShow = false;
            return;
        }

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        shouldShow = distance <= showDistance;
    }

    private void UpdateFade()
    {
        float targetAlpha = shouldShow ? 1f : 0f;
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);

        if (useText && textMesh != null)
        {
            Color color = textMesh.color;
            color.a = currentAlpha;
            textMesh.color = color;
        }
        else if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = currentAlpha;
            spriteRenderer.color = color;
        }

        // Tắt GameObject nếu alpha = 0 để tối ưu
        if (currentAlpha <= 0.01f && promptObject.activeSelf)
        {
            promptObject.SetActive(false);
        }
        else if (currentAlpha > 0.01f && !promptObject.activeSelf)
        {
            promptObject.SetActive(true);
        }
    }

    /// <summary>
    /// Ẩn prompt (gọi khi đang tương tác)
    /// </summary>
    public void Hide()
    {
        shouldShow = false;
    }

    /// <summary>
    /// Hiện prompt
    /// </summary>
    public void Show()
    {
        shouldShow = true;
    }
}

