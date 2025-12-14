#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[Serializable]
public class DialogueJson
{
    public string conversationName;
    public int startNodeId = 0;
    public List<NodeJson> nodes = new List<NodeJson>();
}

[Serializable]
public class NodeJson
{
    public int id;
    public string speaker = "";
    public bool isPlayer = false;
    public List<string> lines = new List<string>();
    public List<ChoiceJson> choices;
    public int next = -1;
    public List<string> setFlags;
    public List<VarChangeJson> varChanges;
}

[Serializable]
public class ChoiceJson
{
    public string text;
    public int next;
    public string action;
    public List<string> requireFlags;
    public List<string> forbidFlags;
    public List<VarConditionJson> varConditions;
    public List<string> setTrue;
    public List<string> setFalse;
    public List<VarChangeJson> varChanges;
}

[Serializable]
public class VarChangeJson
{
    public string name;
    public string op = "add";
    public int value;
}

[Serializable]
public class VarConditionJson
{
    public string name;
    public string op = ">=";
    public int value;
}

public class DialogueJsonImporter : EditorWindow
{
    [MenuItem("Tools/Dialogue/Import JSON to DialogueData")]
    public static void ShowWindow()
    {
        GetWindow<DialogueJsonImporter>("Dialogue JSON Importer");
    }

    private TextAsset jsonFile;
    private string outputFolder = "Assets/Scripts/Data/Dialogues";
    private string previewText = "";
    private Vector2 scrollPos;

    private void OnGUI()
    {
        GUILayout.Label("Import JSON to DialogueData", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File:", jsonFile, typeof(TextAsset), false);

        EditorGUILayout.BeginHorizontal();
        outputFolder = EditorGUILayout.TextField("Output Folder:", outputFolder);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("Select Output Folder", "Assets", "");
            if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath))
                outputFolder = "Assets" + path.Substring(Application.dataPath.Length);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        if (GUILayout.Button("Preview JSON")) PreviewJson();

        if (!string.IsNullOrEmpty(previewText))
        {
            EditorGUILayout.LabelField("Preview:", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
            EditorGUILayout.TextArea(previewText, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Import & Create DialogueData", GUILayout.Height(40))) ImportJson();
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "JSON Format:\n{\n  \"conversationName\": \"Day1\",\n  \"nodes\": [\n" +
            "    { \"id\": 0, \"speaker\": \"Mom\", \"lines\": [\"Wake up!\"], \"next\": 1 }\n  ]\n}",
            MessageType.Info);
    }

    private void PreviewJson()
    {
        if (jsonFile == null) { previewText = "No JSON file selected!"; return; }
        try
        {
            DialogueJson data = JsonUtility.FromJson<DialogueJson>(jsonFile.text);
            previewText = "Conversation: " + data.conversationName + "\nNodes: " + data.nodes.Count + "\n\n";
            foreach (var node in data.nodes)
            {
                string speaker = string.IsNullOrEmpty(node.speaker) ? "(Narrator)" : node.speaker;
                previewText += "[Node " + node.id + "] " + speaker + ":\n";
                if (node.lines != null) foreach (var line in node.lines) previewText += "  \"" + line + "\"\n";
                previewText += node.choices != null && node.choices.Count > 0
                    ? "  Choices: " + node.choices.Count + "\n\n" : "  -> Next: " + node.next + "\n\n";
            }
        }
        catch (Exception e) { previewText = "JSON Error: " + e.Message; }
    }

    private void ImportJson()
    {
        if (jsonFile == null) { EditorUtility.DisplayDialog("Error", "No JSON file selected!", "OK"); return; }
        try
        {
            DialogueJson data = JsonUtility.FromJson<DialogueJson>(jsonFile.text);
            CreateOutputFolder();
            DialogueData dialogueData = ScriptableObject.CreateInstance<DialogueData>();
            dialogueData.conversationName = data.conversationName;
            dialogueData.startNodeId = data.startNodeId;
            dialogueData.nodes = ConvertNodes(data.nodes);

            string fileName = string.IsNullOrEmpty(data.conversationName) ? jsonFile.name : data.conversationName;
            string assetPath = outputFolder + "/" + fileName + "_Dialogue.asset";

            if (AssetDatabase.LoadAssetAtPath<DialogueData>(assetPath) != null)
            {
                if (!EditorUtility.DisplayDialog("File exists", assetPath + " exists. Overwrite?", "Yes", "No")) return;
                AssetDatabase.DeleteAsset(assetPath);
            }

            AssetDatabase.CreateAsset(dialogueData, assetPath);
            AssetDatabase.SaveAssets();
            Selection.activeObject = dialogueData;
            EditorGUIUtility.PingObject(dialogueData);
            EditorUtility.DisplayDialog("Success", "Created: " + assetPath + "\nNodes: " + dialogueData.nodes.Length, "OK");
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Error", "Import failed: " + e.Message, "OK");
            Debug.LogError("[DialogueJsonImporter] " + e);
        }
    }

    private void CreateOutputFolder()
    {
        if (AssetDatabase.IsValidFolder(outputFolder)) return;
        string[] folders = outputFolder.Split('/');
        string currentPath = folders[0];
        for (int i = 1; i < folders.Length; i++)
        {
            string newPath = currentPath + "/" + folders[i];
            if (!AssetDatabase.IsValidFolder(newPath)) AssetDatabase.CreateFolder(currentPath, folders[i]);
            currentPath = newPath;
        }
    }

    private DialogueNode[] ConvertNodes(List<NodeJson> nodeJsons)
    {
        List<DialogueNode> nodes = new List<DialogueNode>();
        foreach (var nj in nodeJsons)
        {
            DialogueNode node = new DialogueNode
            {
                nodeId = nj.id,
                speakerName = nj.speaker ?? "",
                isPlayerSpeaking = nj.isPlayer,
                dialogueLines = nj.lines != null ? nj.lines.ToArray() : new string[0],
                nextNodeId = nj.next,
                setFlagsOnEnter = nj.setFlags != null ? nj.setFlags.ToArray() : new string[0],
                variableChangesOnEnter = ConvertVarChanges(nj.varChanges),
                choices = ConvertChoices(nj.choices)
            };
            nodes.Add(node);
        }
        return nodes.ToArray();
    }

    private DialogueChoice[] ConvertChoices(List<ChoiceJson> choiceJsons)
    {
        if (choiceJsons == null || choiceJsons.Count == 0) return new DialogueChoice[0];
        List<DialogueChoice> choices = new List<DialogueChoice>();
        foreach (var cj in choiceJsons)
        {
            choices.Add(new DialogueChoice
            {
                choiceText = cj.text,
                nextNodeId = cj.next,
                actionId = cj.action ?? "",
                requiredFlags = cj.requireFlags != null ? cj.requireFlags.ToArray() : new string[0],
                forbiddenFlags = cj.forbidFlags != null ? cj.forbidFlags.ToArray() : new string[0],
                variableConditions = ConvertVarConditions(cj.varConditions),
                setFlagsTrue = cj.setTrue != null ? cj.setTrue.ToArray() : new string[0],
                setFlagsFalse = cj.setFalse != null ? cj.setFalse.ToArray() : new string[0],
                variableChanges = ConvertVarChanges(cj.varChanges)
            });
        }
        return choices.ToArray();
    }

    private VariableChange[] ConvertVarChanges(List<VarChangeJson> vcs)
    {
        if (vcs == null || vcs.Count == 0) return new VariableChange[0];
        List<VariableChange> result = new List<VariableChange>();
        foreach (var v in vcs)
        {
            var op = VariableChange.ChangeOperation.Add;
            if (v.op == "set") op = VariableChange.ChangeOperation.Set;
            else if (v.op == "sub") op = VariableChange.ChangeOperation.Subtract;
            result.Add(new VariableChange { variableName = v.name, operation = op, value = v.value });
        }
        return result.ToArray();
    }

    private VariableCondition[] ConvertVarConditions(List<VarConditionJson> vcs)
    {
        if (vcs == null || vcs.Count == 0) return new VariableCondition[0];
        List<VariableCondition> result = new List<VariableCondition>();
        foreach (var c in vcs)
        {
            var op = VariableCondition.ComparisonOperator.GreaterOrEqual;
            switch (c.op)
            {
                case ">": op = VariableCondition.ComparisonOperator.GreaterThan; break;
                case "<": op = VariableCondition.ComparisonOperator.LessThan; break;
                case "<=": op = VariableCondition.ComparisonOperator.LessOrEqual; break;
                case "==": op = VariableCondition.ComparisonOperator.Equal; break;
                case "!=": op = VariableCondition.ComparisonOperator.NotEqual; break;
            }
            result.Add(new VariableCondition { variableName = c.name, comparison = op, value = c.value });
        }
        return result.ToArray();
    }
}
#endif