using UnityEngine;
using System.Collections.Generic;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    
    [System.Serializable]
    public class CharacterSkin
    {
        public string name;
        public string characterType; // "Wizard", "Swordsman", "Archer"
        public int price;
        public bool isUnlocked;
        public string spriteName; // The sprite name to load
    }
    
    public List<CharacterSkin> availableSkins = new List<CharacterSkin>();
    public int currentStars = 0;
    public int currentRetries = 0;
    public string currentSkin = "Wizard";
    
    private const string STARS_KEY = "PlayerStars";
    private const string UNLOCKED_SKINS_KEY = "UnlockedSkins";
    private const string CURRENT_SKIN_KEY = "CurrentSkin";
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
            InitializeSkins();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeSkins()
    {
        // Initialize available skins if empty
        if (availableSkins.Count == 0)
        {
            // Wizard (default - free)
            availableSkins.Add(new CharacterSkin
            {
                name = "Wizard",
                characterType = "Wizard",
                price = 0,
                isUnlocked = true,
                spriteName = "Idle"
            });
            
            // Swordsman
            availableSkins.Add(new CharacterSkin
            {
                name = "Swordsman",
                characterType = "Swordsman",
                price = 30,
                isUnlocked = false,
                spriteName = "Idle"
            });
            
            // Archer
            availableSkins.Add(new CharacterSkin
            {
                name = "Archer",
                characterType = "Archer",
                price = 50,
                isUnlocked = false,
                spriteName = "Idle"
            });
        }
        
        LoadUnlockedSkins();
    }
    
    public void AddStars(int amount)
    {
        currentStars += amount;
        SaveData();
        Debug.Log($"CurrencyManager: Added {amount} stars. Total: {currentStars}");
    }
    
    public bool SpendStars(int amount)
    {
        if (currentStars >= amount)
        {
            currentStars -= amount;
            SaveData();
            Debug.Log($"CurrencyManager: Spent {amount} stars. Remaining: {currentStars}");
            return true;
        }
        return false;
    }
    
    public void StartLevel()
    {
        currentRetries = 0;
        Debug.Log("CurrencyManager: Level started, retries reset to 0");
    }
    
    public void AddRetry()
    {
        currentRetries++;
        Debug.Log($"CurrencyManager: Retry added. Total retries: {currentRetries}");
    }
    
    public int CalculateLevelReward()
    {
        int baseReward = 15;
        int penalty = currentRetries * 5;
        int finalReward = Mathf.Max(0, baseReward - penalty);
        
        Debug.Log($"CurrencyManager: Level reward calculated - Base: {baseReward}, Retries: {currentRetries}, Penalty: {penalty}, Final: {finalReward}");
        return finalReward;
    }
    
    public void CompleteLevel()
    {
        int reward = CalculateLevelReward();
        AddStars(reward);
        
        // Reset retries for next level
        currentRetries = 0;
        
        Debug.Log($"CurrencyManager: Level completed! Reward: {reward} stars");
    }
    
    public bool PurchaseSkin(string skinName)
    {
        CharacterSkin skin = availableSkins.Find(s => s.name == skinName);
        if (skin != null && !skin.isUnlocked)
        {
            if (SpendStars(skin.price))
            {
                skin.isUnlocked = true;
                SaveUnlockedSkins();
                Debug.Log($"CurrencyManager: Purchased skin '{skinName}' for {skin.price} stars");
                return true;
            }
        }
        return false;
    }
    
    public void SetCurrentSkin(string skinName)
    {
        CharacterSkin skin = availableSkins.Find(s => s.name == skinName);
        if (skin != null && skin.isUnlocked)
        {
            currentSkin = skinName;
            PlayerPrefs.SetString(CURRENT_SKIN_KEY, currentSkin);
            Debug.Log($"CurrencyManager: Current skin set to '{skinName}'");
        }
    }
    
    public CharacterSkin GetCurrentSkin()
    {
        return availableSkins.Find(s => s.name == currentSkin);
    }
    
    public Sprite GetCurrentSkinSprite()
    {
        CharacterSkin skin = GetCurrentSkin();
        if (skin != null)
        {
            return ManualSpriteLoader.GetSprite(skin.spriteName);
        }
        return null;
    }
    
    void SaveData()
    {
        PlayerPrefs.SetInt(STARS_KEY, currentStars);
        PlayerPrefs.Save();
    }
    
    void LoadData()
    {
        currentStars = PlayerPrefs.GetInt(STARS_KEY, 0);
        currentSkin = PlayerPrefs.GetString(CURRENT_SKIN_KEY, "Wizard");
        Debug.Log($"CurrencyManager: Loaded data - Stars: {currentStars}, Current Skin: {currentSkin}");
    }
    
    void SaveUnlockedSkins()
    {
        string unlockedSkins = "";
        foreach (var skin in availableSkins)
        {
            if (skin.isUnlocked)
            {
                unlockedSkins += skin.name + ",";
            }
        }
        PlayerPrefs.SetString(UNLOCKED_SKINS_KEY, unlockedSkins);
        PlayerPrefs.Save();
    }
    
    void LoadUnlockedSkins()
    {
        string unlockedSkins = PlayerPrefs.GetString(UNLOCKED_SKINS_KEY, "Wizard,");
        string[] unlockedNames = unlockedSkins.Split(',');
        
        foreach (var skin in availableSkins)
        {
            skin.isUnlocked = System.Array.Exists(unlockedNames, name => name == skin.name);
        }
        
        Debug.Log("CurrencyManager: Unlocked skins loaded");
    }
} 