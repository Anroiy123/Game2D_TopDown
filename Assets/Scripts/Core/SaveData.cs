using System;
using System.Collections.Generic;

/// <summary>
/// SaveData - Cấu trúc dữ liệu lưu game
/// Serializable để convert sang JSON
/// </summary>
[Serializable]
public class SaveData
{
    // Metadata
    public string saveTime;
    public string gameVersion = "1.0";
    public int saveSlot = 0;

    // Scene/Position
    public string currentSceneName;
    public float playerPosX;
    public float playerPosY;
    public string playerFacingDirection;

    // Story Progress
    public int currentEnding;
    public List<StoryFlagEntry> storyFlags = new List<StoryFlagEntry>();
    public List<StoryVariableEntry> storyVariables = new List<StoryVariableEntry>();

    // Game Stats
    public float totalPlayTime;
    public int dialoguesCompleted;
    public int choicesMade;
}

/// <summary>
/// Entry cho story flag (key-value pair)
/// </summary>
[Serializable]
public class StoryFlagEntry
{
    public string key;
    public bool value;

    public StoryFlagEntry() { }
    public StoryFlagEntry(string key, bool value)
    {
        this.key = key;
        this.value = value;
    }
}

/// <summary>
/// Entry cho story variable (key-value pair)
/// </summary>
[Serializable]
public class StoryVariableEntry
{
    public string key;
    public int value;

    public StoryVariableEntry() { }
    public StoryVariableEntry(string key, int value)
    {
        this.key = key;
        this.value = value;
    }
}

