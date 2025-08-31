using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ManualSpriteLoader : MonoBehaviour
{
    public static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    
    void Awake()
    {
        LoadAllSprites();
    }
    
    void LoadAllSprites()
    {
        Debug.Log("ManualSpriteLoader: Starting sprite loading...");
        
        // Try multiple loading methods
        LoadSpritesMethod1();
        LoadSpritesMethod2();
        LoadSpritesMethod3();
        
        Debug.Log($"ManualSpriteLoader: Total sprites loaded: {spriteCache.Count}");
        
        // Log all loaded sprites
        foreach (var kvp in spriteCache)
        {
            Debug.Log($"ManualSpriteLoader: Cached sprite - {kvp.Key}");
        }
    }
    
    void LoadSpritesMethod1()
    {
        Debug.Log("ManualSpriteLoader: Method 1 - Using Resources.FindObjectsOfTypeAll");
        
        var allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
        Debug.Log($"ManualSpriteLoader: Found {allSprites.Length} sprites with FindObjectsOfTypeAll");
        
        foreach (var sprite in allSprites)
        {
            string assetPath = GetAssetPath(sprite);
            if (assetPath.Contains("Sprites/background") || 
                assetPath.Contains("Sprites/Characters/animals") ||
                assetPath.Contains("Sprites/Characters/Wizard") ||
                assetPath.Contains("Sprites/Letters"))
            {
                spriteCache[sprite.name] = sprite;
                Debug.Log($"ManualSpriteLoader: Loaded sprite: {sprite.name} from {assetPath}");
            }
        }
    }
    
    void LoadSpritesMethod2()
    {
        Debug.Log("ManualSpriteLoader: Method 2 - Using Resources.LoadAll");
        
        // Try loading from Resources paths
        var backgroundSprites = Resources.LoadAll<Sprite>("Sprites/background");
        var animalSprites = Resources.LoadAll<Sprite>("Sprites/Characters/animals");
        var wizardSprites = Resources.LoadAll<Sprite>("Sprites/Characters/Wizard");
        var letterSprites = Resources.LoadAll<Sprite>("Sprites/Letters/Glass_blue_alphabetics");
        
        Debug.Log($"ManualSpriteLoader: Found {backgroundSprites.Length} background sprites");
        Debug.Log($"ManualSpriteLoader: Found {animalSprites.Length} animal sprites");
        Debug.Log($"ManualSpriteLoader: Found {wizardSprites.Length} wizard sprites");
        Debug.Log($"ManualSpriteLoader: Found {letterSprites.Length} letter sprites");
        
        foreach (var sprite in backgroundSprites) spriteCache[sprite.name] = sprite;
        foreach (var sprite in animalSprites) spriteCache[sprite.name] = sprite;
        foreach (var sprite in wizardSprites) spriteCache[sprite.name] = sprite;
        foreach (var sprite in letterSprites) spriteCache[sprite.name] = sprite;
    }
    
    void LoadSpritesMethod3()
    {
        Debug.Log("ManualSpriteLoader: Method 3 - Using AssetDatabase (Editor only)");
        
        #if UNITY_EDITOR
        try
        {
            // Try to load specific sprites by path
            LoadSpriteByPath("Assets/Sprites/background/desert.png", "desert");
            LoadSpriteByPath("Assets/Sprites/background/twilight.png", "twilight");
            LoadSpriteByPath("Assets/Sprites/background/mountains.png", "mountains");
        LoadSpriteByPath("Assets/Sprites/background/tempel_background.png", "tempel_background");
            LoadSpriteByPath("Assets/Sprites/Characters/animals/cat.png", "cat");
            LoadSpriteByPath("Assets/Sprites/Characters/animals/dog.png", "dog");
            LoadSpriteByPath("Assets/Sprites/Characters/animals/peng.png", "peng");
            LoadSpriteByPath("Assets/Sprites/Characters/animals/bear.png", "bear");
            LoadSpriteByPath("Assets/Sprites/Characters/animals/panda.png", "panda");
            LoadSpriteByPath("Assets/Sprites/Characters/animals/Dolphin.png", "Dolphin");
            LoadSpriteByPath("Assets/Sprites/Characters/animals/elph.png", "elph");
            LoadSpriteByPath("Assets/Sprites/Characters/animals/mouse.png", "mouse");
            LoadSpriteByPath("Assets/Sprites/Characters/animals/sanke.png", "sanke");
            LoadSpriteByPath("Assets/Sprites/Characters/animals/knagaroo.png", "knagaroo");
            LoadSpriteByPath("Assets/Sprites/Characters/Wizard/Idle.png", "Idle");
            LoadSpriteByPath("Assets/Sprites/Characters/Wizard/Idle.png", "Wizard");
            LoadSpriteByPath("Assets/Sprites/Characters/Ghost.png", "Ghost");
            
            // Load letter sprites from the correct folder
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            foreach (char letter in alphabet)
            {
                LoadSpriteByPath($"Assets/Sprites/Letters/Glass_blue_alphabetics/{letter}.png", letter.ToString());
            }
            
            // Load menu sprites
            LoadSpriteByPath("Assets/Sprites/Menu/menu_background.png", "menu_background");
            LoadSpriteByPath("Assets/Sprites/Menu/play.png", "play");
            LoadSpriteByPath("Assets/Sprites/Menu/prize.png", "prize");
            LoadSpriteByPath("Assets/Sprites/Menu/close_icon.png", "close_icon");
            LoadSpriteByPath("Assets/Sprites/Menu/btn.png", "btn");
            
            // Load star sprite for currency
            LoadSpriteByPath("Assets/Sprites/starts_and_hearts/star.png", "star");
            
            // Load character sprites for shop
            LoadSpriteByPath("Assets/Sprites/Characters/Swordsman/Idle.png", "Swordsman_Idle");
            LoadSpriteByPath("Assets/Sprites/Characters/Archer/Idle.png", "Archer_Idle");
            LoadSpriteByPath("Assets/Sprites/Characters/Wizard/Idle.png", "Wizard_Idle");
            
            Debug.Log("ManualSpriteLoader: Menu sprites and character sprites loading completed");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ManualSpriteLoader: Error in Method 3: {e.Message}");
        }
        #endif
    }
    
    #if UNITY_EDITOR
    void LoadSpriteByPath(string assetPath, string spriteName)
    {
        Sprite sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        if (sprite != null)
        {
            spriteCache[spriteName] = sprite;
            Debug.Log($"ManualSpriteLoader: Loaded sprite '{spriteName}' from {assetPath}");
        }
        else
        {
            Debug.LogWarning($"ManualSpriteLoader: Could not load sprite from {assetPath}");
        }
    }
    #endif
    
    string GetAssetPath(Sprite sprite)
    {
        #if UNITY_EDITOR
        return UnityEditor.AssetDatabase.GetAssetPath(sprite);
        #else
        return sprite.name; // Fallback for builds
        #endif
    }
    
    public static Sprite GetSprite(string name)
    {
        Debug.Log($"ManualSpriteLoader: Looking for sprite '{name}' in cache of {spriteCache.Count} sprites");
        
        if (spriteCache.ContainsKey(name))
        {
            Debug.Log($"ManualSpriteLoader: Found sprite '{name}' in cache");
            return spriteCache[name];
        }
        
        // Try case-insensitive search
        var sprite = spriteCache.FirstOrDefault(kvp => 
            kvp.Key.Equals(name, System.StringComparison.OrdinalIgnoreCase));
        
        if (sprite.Value != null)
        {
            Debug.Log($"ManualSpriteLoader: Found sprite '{name}' (case-insensitive)");
            return sprite.Value;
        }
        
        // Log all available sprites for debugging
        Debug.LogWarning($"ManualSpriteLoader: Sprite '{name}' not found in cache. Available sprites:");
        foreach (var kvp in spriteCache)
        {
            Debug.Log($"  - {kvp.Key}");
        }
        
        return null;
    }
    
    public static Sprite GetSpriteByPath(string path)
    {
        // Try to find sprite by partial path match
        var sprite = spriteCache.FirstOrDefault(kvp => 
            GetAssetPathStatic(kvp.Value).Contains(path));
        
        if (sprite.Value != null)
        {
            return sprite.Value;
        }
        
        Debug.LogWarning($"ManualSpriteLoader: Sprite with path '{path}' not found");
        return null;
    }
    
    static string GetAssetPathStatic(Sprite sprite)
    {
        #if UNITY_EDITOR
        return UnityEditor.AssetDatabase.GetAssetPath(sprite);
        #else
        return sprite.name; // Fallback for builds
        #endif
    }
} 