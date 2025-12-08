using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Dictionary có thể serialize trong Unity Inspector
/// </summary>
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        
        foreach (var pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        Clear();
        
        if (keys.Count != values.Count)
        {
            Debug.LogError($"[SerializableDictionary] Key count ({keys.Count}) != Value count ({values.Count})");
            return;
        }
        
        for (int i = 0; i < keys.Count; i++)
        {
            if (!ContainsKey(keys[i]))
            {
                Add(keys[i], values[i]);
            }
        }
    }
}

/// <summary>
/// String-Bool Dictionary cho Story Flags
/// </summary>
[Serializable]
public class StoryFlagDictionary : SerializableDictionary<string, bool> { }

/// <summary>
/// String-Int Dictionary cho Story Variables
/// </summary>
[Serializable]
public class StoryVariableDictionary : SerializableDictionary<string, int> { }

