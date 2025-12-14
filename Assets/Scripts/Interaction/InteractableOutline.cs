using UnityEngine;

/// <summary>
/// Component để hiển thị viền trắng xung quanh object khi player đến gần.
/// Gắn vào bất kỳ object nào có thể tương tác (giường, NPC, cửa, v.v.)
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class InteractableOutline : MonoBehaviour
{
    public enum OutlineMethod
    {
        Shader,         // Dùng shader (tốt cho sprite vuông)
        DuplicateSprite // Dùng sprite copy phía sau (tốt cho sprite hẹp/dọc)
    }

    [Header("Outline Settings")]
    [SerializeField] private OutlineMethod method = OutlineMethod.DuplicateSprite;
    [SerializeField] private Color outlineColor = Color.white;
    [SerializeField] [Range(0f, 10f)] private float outlineWidth = 2f;

    [Header("Duplicate Sprite Settings (cho sprite hẹp như cửa)")]
    [SerializeField] [Range(1f, 2f)] private float outlineScale = 1.1f;
    [SerializeField] private int outlineSortingOrderOffset = -1;

    [Header("Manual Outline Object (optional)")]
    [SerializeField] private GameObject manualOutlineObject;

    private SpriteRenderer spriteRenderer;
    private Material outlineMaterial;
    private Material originalMaterial;
    private GameObject duplicateOutlineObject;
    private SpriteRenderer duplicateRenderer;
    private bool isOutlineEnabled = false;

    // Shader property IDs
    private static readonly int OutlineEnabledId = Shader.PropertyToID("_OutlineEnabled");
    private static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");
    private static readonly int OutlineWidthId = Shader.PropertyToID("_OutlineWidth");

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        switch (method)
        {
            case OutlineMethod.Shader:
                SetupShaderOutline();
                break;
            case OutlineMethod.DuplicateSprite:
                SetupDuplicateSpriteOutline();
                break;
        }
    }

    private void SetupDuplicateSpriteOutline()
    {
        // Tạo một GameObject con với sprite copy
        duplicateOutlineObject = new GameObject("OutlineSprite");
        duplicateOutlineObject.transform.SetParent(transform);
        duplicateOutlineObject.transform.localPosition = Vector3.zero;
        duplicateOutlineObject.transform.localRotation = Quaternion.identity;
        duplicateOutlineObject.transform.localScale = Vector3.one * outlineScale;

        // Thêm SpriteRenderer
        duplicateRenderer = duplicateOutlineObject.AddComponent<SpriteRenderer>();
        duplicateRenderer.sprite = spriteRenderer.sprite;
        duplicateRenderer.color = outlineColor;
        duplicateRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        duplicateRenderer.sortingOrder = spriteRenderer.sortingOrder + outlineSortingOrderOffset;

        // Ẩn mặc định
        duplicateOutlineObject.SetActive(false);
    }

    private void SetupShaderOutline()
    {
        // Tìm shader
        Shader outlineShader = Shader.Find("Custom/SpriteOutline");
        if (outlineShader == null)
        {
            Debug.LogWarning($"[InteractableOutline] Không tìm thấy shader 'Custom/SpriteOutline' trên {gameObject.name}. Chuyển sang DuplicateSprite.");
            method = OutlineMethod.DuplicateSprite;
            SetupDuplicateSpriteOutline();
            return;
        }

        // Tạo material instance riêng
        originalMaterial = spriteRenderer.material;
        outlineMaterial = new Material(outlineShader);

        // Copy texture từ material gốc
        if (originalMaterial.HasProperty("_MainTex"))
        {
            outlineMaterial.SetTexture("_MainTex", originalMaterial.GetTexture("_MainTex"));
        }

        // Set outline properties
        outlineMaterial.SetColor(OutlineColorId, outlineColor);
        outlineMaterial.SetFloat(OutlineWidthId, outlineWidth);
        outlineMaterial.SetFloat(OutlineEnabledId, 0f);

        // Áp dụng material mới
        spriteRenderer.material = outlineMaterial;
    }

    /// <summary>
    /// Bật outline effect
    /// </summary>
    public void EnableOutline()
    {
        if (isOutlineEnabled) return;
        isOutlineEnabled = true;

        switch (method)
        {
            case OutlineMethod.Shader:
                if (outlineMaterial != null)
                {
                    outlineMaterial.SetFloat(OutlineEnabledId, 1f);
                }
                break;
            case OutlineMethod.DuplicateSprite:
                if (duplicateOutlineObject != null)
                {
                    // Cập nhật sprite nếu đã thay đổi
                    if (duplicateRenderer != null && duplicateRenderer.sprite != spriteRenderer.sprite)
                    {
                        duplicateRenderer.sprite = spriteRenderer.sprite;
                    }
                    duplicateOutlineObject.SetActive(true);
                }
                break;
        }

        if (manualOutlineObject != null)
        {
            manualOutlineObject.SetActive(true);
        }
    }

    /// <summary>
    /// Tắt outline effect
    /// </summary>
    public void DisableOutline()
    {
        if (!isOutlineEnabled) return;
        isOutlineEnabled = false;

        switch (method)
        {
            case OutlineMethod.Shader:
                if (outlineMaterial != null)
                {
                    outlineMaterial.SetFloat(OutlineEnabledId, 0f);
                }
                break;
            case OutlineMethod.DuplicateSprite:
                if (duplicateOutlineObject != null)
                {
                    duplicateOutlineObject.SetActive(false);
                }
                break;
        }

        if (manualOutlineObject != null)
        {
            manualOutlineObject.SetActive(false);
        }
    }

    /// <summary>
    /// Đặt màu outline
    /// </summary>
    public void SetOutlineColor(Color color)
    {
        outlineColor = color;

        if (outlineMaterial != null)
        {
            outlineMaterial.SetColor(OutlineColorId, color);
        }

        if (duplicateRenderer != null)
        {
            duplicateRenderer.color = color;
        }
    }

    /// <summary>
    /// Đặt độ dày outline
    /// </summary>
    public void SetOutlineWidth(float width)
    {
        outlineWidth = width;
        if (outlineMaterial != null)
        {
            outlineMaterial.SetFloat(OutlineWidthId, width);
        }
    }

    public bool IsOutlineEnabled => isOutlineEnabled;

    private void OnDestroy()
    {
        // Cleanup
        if (outlineMaterial != null)
        {
            Destroy(outlineMaterial);
        }

        if (duplicateOutlineObject != null)
        {
            Destroy(duplicateOutlineObject);
        }
    }
}

