using UnityEngine;
using UnityEditor;

public class ReassignVNSceneDataScript
{
    [MenuItem("Tools/Fix VNSceneData - Day1_Scene3 Only")]
    static void FixDay1Scene3()
    {
        string path = "Assets/Data/VisualNovel/Day1/Day1_Scene3_Classroom_VNScene.asset";

        // Đọc file YAML trực tiếp
        string yaml = System.IO.File.ReadAllText(path);

        // Tìm GUID của VisualNovelData.cs
        string scriptPath = "Assets/Scripts/VisualNovel/VisualNovelData.cs";
        string scriptGuid = AssetDatabase.AssetPathToGUID(scriptPath);

        if (string.IsNullOrEmpty(scriptGuid))
        {
            Debug.LogError($"Cannot find GUID for {scriptPath}");
            return;
        }

        // Replace m_Script line
        yaml = System.Text.RegularExpressions.Regex.Replace(
            yaml,
            @"m_Script: \{fileID: 0\}",
            $"m_Script: {{fileID: 11500000, guid: {scriptGuid}, type: 3}}"
        );

        System.IO.File.WriteAllText(path, yaml);
        AssetDatabase.Refresh();

        Debug.Log($"✓ Fixed {path}");
    }
}

