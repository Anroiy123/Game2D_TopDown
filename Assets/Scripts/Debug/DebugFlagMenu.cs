using UnityEngine;

/// <summary>
/// Debug Flag Menu - Nhấn F1 để mở menu set flags trong lúc Play
/// </summary>
public class DebugFlagMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.F1;
    [SerializeField] private bool editorOnly = true;

    private bool showMenu = false;
    private Vector2 scrollPosition;

    // Danh sách flags thường dùng để test
    private string[] commonFlags = new string[]
    {
        "ran_from_bullies",
        "confronted_bullies",
        "day1_scene1_completed",
        "day1_scene2_completed",
        "day1_scene6_completed",
        "day1_scene8_completed",
        "MET_BULLIES",
    };

    private void Update()
    {
        #if !UNITY_EDITOR
        if (editorOnly) return;
        #endif

        if (Input.GetKeyDown(toggleKey))
        {
            showMenu = !showMenu;
        }
    }

    private void OnGUI()
    {
        #if !UNITY_EDITOR
        if (editorOnly) return;
        #endif

        if (!showMenu) return;

        if (StoryManager.Instance == null)
        {
            GUI.Label(new Rect(10, 10, 300, 30), "StoryManager not found!");
            return;
        }

        // Background
        GUI.Box(new Rect(10, 10, 400, 500), "Debug Flag Menu (F1 to close)");

        GUILayout.BeginArea(new Rect(20, 40, 380, 460));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("=== COMMON FLAGS ===", GUI.skin.box);

        foreach (string flag in commonFlags)
        {
            GUILayout.BeginHorizontal();

            bool currentValue = StoryManager.Instance.GetFlag(flag);
            GUILayout.Label(flag, GUILayout.Width(200));
            GUILayout.Label(currentValue ? "✓ TRUE" : "✗ FALSE", GUILayout.Width(80));

            if (GUILayout.Button("Set TRUE", GUILayout.Width(80)))
            {
                StoryManager.Instance.SetFlag(flag, true);
                Debug.Log($"[DebugMenu] Set {flag} = TRUE");
            }

            if (GUILayout.Button("Set FALSE", GUILayout.Width(80)))
            {
                StoryManager.Instance.SetFlag(flag, false);
                Debug.Log($"[DebugMenu] Set {flag} = FALSE");
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.Space(20);
        GUILayout.Label("=== QUICK ACTIONS ===", GUI.skin.box);

        if (GUILayout.Button("Setup for Scene 8 Test (Mom at Door)", GUILayout.Height(30)))
        {
            SetupScene8Test();
        }

        if (GUILayout.Button("Reset All Flags", GUILayout.Height(30)))
        {
            ResetAllFlags();
        }

        GUILayout.Space(20);
        GUILayout.Label("=== VARIABLES ===", GUI.skin.box);

        int currentDay = StoryManager.Instance.GetVariable(StoryManager.VarKeys.CURRENT_DAY);
        GUILayout.Label($"Current Day: {currentDay}");

        int money = StoryManager.Instance.GetVariable("money");
        GUILayout.Label($"Money: {money}");

        int fearLevel = StoryManager.Instance.GetVariable(StoryManager.VarKeys.FEAR_LEVEL);
        GUILayout.Label($"Fear Level: {fearLevel}");

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void SetupScene8Test()
    {
        if (StoryManager.Instance == null) return;

        // Set flags cần thiết để Mom đứng ở cửa
        StoryManager.Instance.SetFlag("ran_from_bullies", true);
        StoryManager.Instance.SetFlag("day1_scene1_completed", true);
        StoryManager.Instance.SetFlag("day1_scene2_completed", true);
        StoryManager.Instance.SetFlag("day1_scene6_completed", true);

        Debug.Log("[DebugMenu] ✅ Setup Scene 8 test - Mom should be at door now!");
        Debug.Log("[DebugMenu] Di chuyển đến gần Mom và bấm E để test dialogue!");
    }

    private void ResetAllFlags()
    {
        if (StoryManager.Instance == null) return;

        foreach (string flag in commonFlags)
        {
            StoryManager.Instance.SetFlag(flag, false);
        }

        Debug.Log("[DebugMenu] ✅ Reset all flags to FALSE");
    }
}

