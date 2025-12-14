using UnityEngine;

/// <summary>
/// Test VN Scene - Đặt component này vào scene để test VN nhanh
/// Nhấn phím T trong Play mode để trigger VN scene
/// </summary>
public class VNSceneQuickTest : MonoBehaviour
{
    [Header("VN Scene to Test")]
    [Tooltip("Kéo VNSceneData asset vào đây")]
    [SerializeField] private VNSceneData vnSceneToTest;

    [Header("Test Settings")]
    [Tooltip("Phím để trigger VN (default: T)")]
    [SerializeField] private KeyCode testKey = KeyCode.T;

    [Tooltip("Show UI instructions")]
    [SerializeField] private bool showInstructions = true;

    [Header("Debug")]
    [SerializeField] private bool logFlagsAfterComplete = true;
    [SerializeField] private bool logVariablesAfterComplete = true;

    private void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            TestVNScene();
        }
    }

    private void TestVNScene()
    {
        if (vnSceneToTest == null)
        {
            Debug.LogError("[VNQuickTest] VN Scene Data is null! Kéo asset vào Inspector.");
            return;
        }

        Debug.Log($"[VNQuickTest] Starting VN Scene: {vnSceneToTest.name}");

        VisualNovelManager.Instance.StartVNScene(vnSceneToTest, OnVNComplete);
    }

    private void OnVNComplete()
    {
        Debug.Log("[VNQuickTest] VN Scene completed!");

        if (logFlagsAfterComplete)
        {
            var flags = StoryManager.Instance.GetAllFlags();
            Debug.Log($"[VNQuickTest] Active Flags ({flags.Count}): {string.Join(", ", flags)}");
        }

        if (logVariablesAfterComplete)
        {
            int money = StoryManager.Instance.GetVariable("money");
            int day = StoryManager.Instance.GetVariable(StoryManager.VarKeys.CURRENT_DAY);
            int relationshipMom = StoryManager.Instance.GetVariable("relationship_mom");

            Debug.Log($"[VNQuickTest] Variables:");
            Debug.Log($"  - Money: {money}");
            Debug.Log($"  - Current Day: {day}");
            Debug.Log($"  - Relationship (Mom): {relationshipMom}");
        }
    }

    private void OnGUI()
    {
        if (!showInstructions) return;

        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            normal = { textColor = Color.white }
        };

        string instruction = $"[VN Quick Test] Press [{testKey}] to test VN scene";
        GUI.Label(new Rect(10, 10, 500, 30), instruction, style);

        if (vnSceneToTest != null)
        {
            GUI.Label(new Rect(10, 35, 500, 30), $"Scene: {vnSceneToTest.name}", style);
        }
        else
        {
            GUIStyle warningStyle = new GUIStyle(style) { normal = { textColor = Color.red } };
            GUI.Label(new Rect(10, 35, 500, 30), "⚠️ NO VN SCENE SET!", warningStyle);
        }
    }
}

