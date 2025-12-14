using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// StoryManager - Quản lý story flags, variables, và ending determination
/// Singleton pattern, tồn tại xuyên suốt các scene
/// </summary>
public class StoryManager : MonoBehaviour
{
    #region Singleton
    private static StoryManager _instance;
    public static StoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<StoryManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("StoryManager");
                    _instance = go.AddComponent<StoryManager>();
                }
            }
            return _instance;
        }
    }
    #endregion

    #region Story Flags (Bool)
    [Header("Story Flags")]
    [SerializeField] private SerializableDictionary<string, bool> storyFlags = new SerializableDictionary<string, bool>();
    
    // Predefined flag keys (dùng làm reference)
    public static class FlagKeys
    {
        public const string DAY_1_COMPLETED = "day_1_completed";
        public const string MET_BULLIES = "met_bullies";
        public const string BEFRIENDED_BULLIES = "befriended_bullies";
        public const string GOT_BEATEN = "got_beaten";
        public const string TALKED_TO_TEACHER = "talked_to_teacher";
        public const string INVITED_BY_CLASSMATE = "invited_by_classmate";
        public const string REJECTED_CLASSMATE = "rejected_classmate";
        public const string MOM_WORRIED = "mom_worried";
        public const string CONFESSED_TO_MOM = "confessed_to_mom";
        public const string BROUGHT_KNIFE = "brought_knife";
    }
    #endregion

    #region Story Variables (Int/Float)
    [Header("Story Variables")]
    [SerializeField] private SerializableDictionary<string, int> storyVariables = new SerializableDictionary<string, int>();
    
    public static class VarKeys
    {
        public const string CURRENT_DAY = "current_day";
        public const string MONEY = "money";
        public const string FEAR_LEVEL = "fear_level";
        public const string ESCAPED_COUNT = "escaped_count";
        public const string GAVE_MONEY_COUNT = "gave_money_count";
        public const string RELATIONSHIP_CLASSMATE = "relationship_classmate";
    }
    #endregion

    #region Ending System
    public enum EndingType
    {
        None,
        Good_StandUp,       // Tự đứng lên - Solo 1v1 thắng
        True_TellParents,   // Nhờ người lớn - Thú nhận với mẹ
        Bad_Murder,         // Án mạng - Mang dao đâm
        Bad_Death           // Tử vong - Bị đánh chết
    }
    
    [Header("Ending")]
    public EndingType currentEnding = EndingType.None;
    #endregion

    #region Events
    public event Action<string, bool> OnFlagChanged;
    public event Action<string, int> OnVariableChanged;
    public event Action<EndingType> OnEndingDetermined;
    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeDefaultValues();
    }

    private void InitializeDefaultValues()
    {
        // Set default values nếu chưa có
        if (!storyVariables.ContainsKey(VarKeys.CURRENT_DAY))
            storyVariables[VarKeys.CURRENT_DAY] = 1;
        
        if (!storyVariables.ContainsKey(VarKeys.MONEY))
            storyVariables[VarKeys.MONEY] = 50000; // 50k VND ban đầu
        
        if (!storyVariables.ContainsKey(VarKeys.FEAR_LEVEL))
            storyVariables[VarKeys.FEAR_LEVEL] = 0;
    }

    #region Flag Operations
    public void SetFlag(string key, bool value)
    {
        storyFlags[key] = value;
        OnFlagChanged?.Invoke(key, value);
        Debug.Log($"[StoryManager] Flag '{key}' = {value}");
    }

    public bool GetFlag(string key)
    {
        return storyFlags.TryGetValue(key, out bool value) && value;
    }

    public bool HasFlag(string key)
    {
        return storyFlags.ContainsKey(key);
    }

    /// <summary>
    /// Lấy tất cả flags đang active (true)
    /// </summary>
    public List<string> GetAllFlags()
    {
        List<string> activeFlags = new List<string>();
        foreach (var kvp in storyFlags)
        {
            if (kvp.Value) // Chỉ lấy flags = true
            {
                activeFlags.Add(kvp.Key);
            }
        }
        return activeFlags;
    }
    #endregion

    #region Variable Operations
    public void SetVariable(string key, int value)
    {
        storyVariables[key] = value;
        OnVariableChanged?.Invoke(key, value);
        Debug.Log($"[StoryManager] Variable '{key}' = {value}");
    }

    public void AddVariable(string key, int amount)
    {
        int current = GetVariable(key);
        SetVariable(key, current + amount);
    }

    public int GetVariable(string key)
    {
        return storyVariables.TryGetValue(key, out int value) ? value : 0;
    }
    #endregion

    #region Condition Checking
    /// <summary>
    /// Kiểm tra tất cả required flags đều true
    /// </summary>
    public bool CheckRequiredFlags(string[] requiredFlags)
    {
        if (requiredFlags == null || requiredFlags.Length == 0) return true;

        foreach (string flag in requiredFlags)
        {
            if (!GetFlag(flag)) return false;
        }
        return true;
    }

    /// <summary>
    /// Kiểm tra tất cả forbidden flags đều false
    /// </summary>
    public bool CheckForbiddenFlags(string[] forbiddenFlags)
    {
        if (forbiddenFlags == null || forbiddenFlags.Length == 0) return true;

        foreach (string flag in forbiddenFlags)
        {
            if (GetFlag(flag)) return false;
        }
        return true;
    }
    #endregion

    #region Ending Determination
    /// <summary>
    /// Xác định ending dựa trên story flags và variables
    /// </summary>
    public EndingType DetermineEnding()
    {
        // Bad Ending: Án mạng - Mang dao
        if (GetFlag(FlagKeys.BROUGHT_KNIFE))
        {
            currentEnding = EndingType.Bad_Murder;
        }
        // True Ending: Thú nhận với mẹ
        else if (GetFlag(FlagKeys.CONFESSED_TO_MOM))
        {
            currentEnding = EndingType.True_TellParents;
        }
        // Good Ending: Tự đứng lên (solo thắng)
        else if (GetFlag("stood_up_to_bullies"))
        {
            currentEnding = EndingType.Good_StandUp;
        }
        // Bad Ending: Tử vong (không có cách thoát)
        else if (GetFlag(FlagKeys.GOT_BEATEN) && GetVariable(VarKeys.FEAR_LEVEL) >= 100)
        {
            currentEnding = EndingType.Bad_Death;
        }

        if (currentEnding != EndingType.None)
        {
            OnEndingDetermined?.Invoke(currentEnding);
            Debug.Log($"[StoryManager] Ending determined: {currentEnding}");
        }

        return currentEnding;
    }

    /// <summary>
    /// Trigger ending cụ thể (được gọi từ dialogue choice)
    /// </summary>
    public void TriggerEnding(EndingType ending)
    {
        currentEnding = ending;
        OnEndingDetermined?.Invoke(ending);
        Debug.Log($"[StoryManager] Ending triggered: {ending}");

        // TODO: Load ending scene hoặc show ending UI
    }
    #endregion

    #region Day System
    public void AdvanceDay(int days = 1)
    {
        int currentDay = GetVariable(VarKeys.CURRENT_DAY);
        SetVariable(VarKeys.CURRENT_DAY, currentDay + days);
        Debug.Log($"[StoryManager] Day advanced to: {currentDay + days}");
    }

    public int GetCurrentDay()
    {
        return GetVariable(VarKeys.CURRENT_DAY);
    }
    #endregion

    #region Money System
    public bool SpendMoney(int amount)
    {
        int currentMoney = GetVariable(VarKeys.MONEY);
        if (currentMoney >= amount)
        {
            SetVariable(VarKeys.MONEY, currentMoney - amount);
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        AddVariable(VarKeys.MONEY, amount);
    }

    public int GetMoney()
    {
        return GetVariable(VarKeys.MONEY);
    }
    #endregion

    #region Reset / New Game
    public void ResetStory()
    {
        storyFlags.Clear();
        storyVariables.Clear();
        currentEnding = EndingType.None;
        InitializeDefaultValues();
        Debug.Log("[StoryManager] Story reset!");
    }
    #endregion
}

