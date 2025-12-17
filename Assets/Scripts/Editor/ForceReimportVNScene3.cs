using UnityEngine;
using UnityEditor;

public class ForceReimportVNScene3
{
    [MenuItem("Tools/Force Reimport Scene3")]
    static void Reimport()
    {
        string path = "Assets/Data/VisualNovel/Day1/Day1_Scene3_Classroom_VNScene.asset";
        
        // Force reimport
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();
        
        Debug.Log($"✓ Reimported {path}");
        
        // Load và verify
        var asset = AssetDatabase.LoadAssetAtPath<VNSceneData>(path);
        if (asset != null)
        {
            Debug.Log($"✓ Asset loaded successfully: {asset.name}");
            Debug.Log($"  Scene Name: {asset.sceneData.sceneName}");
            Debug.Log($"  Location: {asset.sceneData.locationText}");
        }
        else
        {
            Debug.LogError("✗ Asset still cannot be loaded!");
        }
    }
}

