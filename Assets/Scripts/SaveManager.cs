using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// SaveManager - Quản lý lưu/load game progress
/// Sử dụng JSON file để lưu trữ
/// </summary>
public class SaveManager : MonoBehaviour
{
    #region Singleton
    private static SaveManager _instance;
    public static SaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<SaveManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SaveManager");
                    _instance = go.AddComponent<SaveManager>();
                }
            }
            return _instance;
        }
    }
    #endregion

    [Header("Settings")]
    [SerializeField] private string saveFileName = "gamesave.json";
    [SerializeField] private bool autoSaveEnabled = true;
    [SerializeField] private float autoSaveInterval = 60f; // seconds

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, saveFileName);
    private float autoSaveTimer = 0f;

    public event Action OnGameSaved;
    public event Action OnGameLoaded;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (autoSaveEnabled)
        {
            autoSaveTimer += Time.deltaTime;
            if (autoSaveTimer >= autoSaveInterval)
            {
                autoSaveTimer = 0f;
                SaveGame();
            }
        }
    }

    #region Save/Load
    /// <summary>
    /// Lưu game vào file
    /// </summary>
    public void SaveGame()
    {
        try
        {
            SaveData data = CreateSaveData();
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SaveFilePath, json);
            
            Debug.Log($"[SaveManager] Game saved to: {SaveFilePath}");
            OnGameSaved?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Failed to save game: {e.Message}");
        }
    }

    /// <summary>
    /// Load game từ file
    /// </summary>
    public bool LoadGame()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.Log("[SaveManager] No save file found.");
            return false;
        }

        try
        {
            string json = File.ReadAllText(SaveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            ApplySaveData(data);
            
            Debug.Log("[SaveManager] Game loaded successfully!");
            OnGameLoaded?.Invoke();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Failed to load game: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// Kiểm tra có save file không
    /// </summary>
    public bool HasSaveFile()
    {
        return File.Exists(SaveFilePath);
    }

    /// <summary>
    /// Xóa save file
    /// </summary>
    public void DeleteSave()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
            Debug.Log("[SaveManager] Save file deleted.");
        }
    }
    #endregion

    #region Save Data Creation/Application
    private SaveData CreateSaveData()
    {
        SaveData data = new SaveData();
        data.saveTime = DateTime.Now.ToString();
        
        // Save scene info
        if (GameManager.Instance != null)
        {
            data.currentSceneName = GameManager.Instance.GetCurrentSceneName();
        }

        // Save story flags and variables từ StoryManager
        if (StoryManager.Instance != null)
        {
            // Lưu ending type
            data.currentEnding = (int)StoryManager.Instance.currentEnding;
        }

        return data;
    }

    private void ApplySaveData(SaveData data)
    {
        // Load scene nếu khác scene hiện tại
        if (GameManager.Instance != null && !string.IsNullOrEmpty(data.currentSceneName))
        {
            string currentScene = GameManager.Instance.GetCurrentSceneName();
            if (currentScene != data.currentSceneName)
            {
                GameManager.Instance.LoadScene(data.currentSceneName);
            }
        }
    }
    #endregion
}

