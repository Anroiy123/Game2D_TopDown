using UnityEngine;

/// <summary>
/// StorytellingQuickTest - Component để test storytelling sequence nhanh trong Play Mode
/// Gắn vào GameObject bất kỳ, nhấn phím test để chạy sequence
/// </summary>
public class StorytellingQuickTest : MonoBehaviour
{
    [Header("Test Settings")]
    [Tooltip("Sequence để test")]
    [SerializeField] private StorytellingSequenceData sequenceToTest;

    [Tooltip("Phím để trigger test")]
    [SerializeField] private KeyCode testKey = KeyCode.T;

    [Tooltip("Hiển thị hướng dẫn trên màn hình")]
    [SerializeField] private bool showInstructions = true;

    [Header("Auto Test")]
    [Tooltip("Tự động chạy khi scene load")]
    [SerializeField] private bool autoTestOnStart = false;

    [Tooltip("Delay trước khi auto test (giây)")]
    [SerializeField] private float autoTestDelay = 1f;

    private void Start()
    {
        if (autoTestOnStart && sequenceToTest != null)
        {
            Invoke(nameof(TestSequence), autoTestDelay);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            TestSequence();
        }
    }

    private void TestSequence()
    {
        if (sequenceToTest == null)
        {
            Debug.LogWarning("[StorytellingQuickTest] No sequence assigned!");
            return;
        }

        Debug.Log($"[StorytellingQuickTest] Testing sequence: {sequenceToTest.sequenceName}");
        StorytellingManager.Instance.PlaySequence(sequenceToTest, OnTestComplete);
    }

    private void OnTestComplete()
    {
        Debug.Log("[StorytellingQuickTest] Test complete!");
    }

    private void OnGUI()
    {
        if (!showInstructions) return;

        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            normal = { textColor = Color.white }
        };

        string instruction = $"[Storytelling Test] Press [{testKey}] to test sequence";
        GUI.Label(new Rect(10, 10, 500, 30), instruction, style);

        if (sequenceToTest != null)
        {
            GUI.Label(new Rect(10, 35, 500, 30), $"Sequence: {sequenceToTest.sequenceName}", style);
        }
        else
        {
            GUIStyle warningStyle = new GUIStyle(style) { normal = { textColor = Color.red } };
            GUI.Label(new Rect(10, 35, 500, 30), "⚠️ NO SEQUENCE SET!", warningStyle);
        }
    }
}

