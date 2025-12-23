#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor tool để fix VNSceneData assets bị mất script reference (m_Script: {fileID: 0})
/// Vấn đề: Khi m_Script = 0, Unity không thể deserialize asset, dẫn đến asset "biến mất"
/// khi ở scene không chứa VNTrigger reference đến asset đó.
/// </summary>
public class ReassignVNSceneDataScript
{
    [MenuItem("Tools/Fix VNSceneData/Fix ALL VNSceneData Assets")]
    static void FixAllVNSceneData()
    {
        string scriptPath = "Assets/Scripts/VisualNovel/VisualNovelData.cs";
        string scriptGuid = AssetDatabase.AssetPathToGUID(scriptPath);

        if (string.IsNullOrEmpty(scriptGuid))
        {
            Debug.LogError($"Cannot find GUID for {scriptPath}");
            return;
        }

        string[] assetPaths = AssetDatabase.FindAssets("t:VNSceneData", new[] { "Assets/Data/VisualNovel" });

        // Nếu không tìm thấy bằng type (vì script bị mất), tìm bằng file extension
        if (assetPaths.Length == 0)
        {
            Debug.Log("[FixVNSceneData] No VNSceneData found by type, searching by file...");
            FixAllVNSceneDataByFile(scriptGuid);
            return;
        }

        int fixedCount = 0;
        foreach (string guid in assetPaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (FixSingleAsset(path, scriptGuid))
                fixedCount++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"[FixVNSceneData] ✓ Fixed {fixedCount} VNSceneData assets");
    }

    [MenuItem("Tools/Fix VNSceneData/Fix ALL by File Search")]
    static void FixAllVNSceneDataByFileMenu()
    {
        string scriptPath = "Assets/Scripts/VisualNovel/VisualNovelData.cs";
        string scriptGuid = AssetDatabase.AssetPathToGUID(scriptPath);

        if (string.IsNullOrEmpty(scriptGuid))
        {
            Debug.LogError($"Cannot find GUID for {scriptPath}");
            return;
        }

        FixAllVNSceneDataByFile(scriptGuid);
    }

    static void FixAllVNSceneDataByFile(string scriptGuid)
    {
        string[] directories = { "Assets/Data/VisualNovel" };
        int fixedCount = 0;

        foreach (string dir in directories)
        {
            if (!Directory.Exists(dir)) continue;

            string[] files = Directory.GetFiles(dir, "*.asset", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string normalizedPath = file.Replace("\\", "/");

                // Đọc file và kiểm tra xem có phải VNSceneData không
                string yaml = File.ReadAllText(normalizedPath);
                if (yaml.Contains("m_EditorClassIdentifier: GameScripts::VNSceneData") ||
                    yaml.Contains("VNSceneData"))
                {
                    if (FixSingleAsset(normalizedPath, scriptGuid))
                        fixedCount++;
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"[FixVNSceneData] ✓ Fixed {fixedCount} VNSceneData assets by file search");
    }

    static bool FixSingleAsset(string path, string scriptGuid)
    {
        if (!File.Exists(path)) return false;

        string yaml = File.ReadAllText(path);

        // Kiểm tra xem có cần fix không
        if (!yaml.Contains("m_Script: {fileID: 0}"))
        {
            return false;
        }

        // Replace m_Script line
        string newYaml = System.Text.RegularExpressions.Regex.Replace(
            yaml,
            @"m_Script: \{fileID: 0\}",
            $"m_Script: {{fileID: 11500000, guid: {scriptGuid}, type: 3}}"
        );

        File.WriteAllText(path, newYaml);
        Debug.Log($"[FixVNSceneData] Fixed: {path}");
        return true;
    }
}


#endif