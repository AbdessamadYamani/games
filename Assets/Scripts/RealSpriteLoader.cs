using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RealSpriteLoader : MonoBehaviour
{
    public static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    
    void Awake()
    {
        LoadAllSprites();
    }
    
    void LoadAllSprites()
    {
        Debug.Log("RealSpriteLoader: Starting sprite loading from actual file paths...");
        
        #if UNITY_EDITOR
        // Load sprites using AssetDatabase
        LoadSpritesFromPath("Assets/Sprites/background", "background");
        LoadSpritesFromPath("Assets/Sprites/Characters/animals", "animals");
        LoadSpritesFromPath("Assets/Sprites/Characters/Wizard", "wizard");
        LoadSpritesFromPath("Assets/Sprites/Letters/Glass_blue_alphabetics", "letters");
        
        // Additional debugging for letters
        Debug.Log("RealSpriteLoader: Checking letter sprites specifically...");
        var letterSprites = spriteCache.Where(kvp => kvp.Key.Length == 1 && char.IsLetter(kvp.Key[0])).ToList();
        Debug.Log($"RealSpriteLoader: Found {letterSprites.Count} single-letter sprites:");
        foreach (var kvp in letterSprites)
        {
            Debug.Log($"  - Letter sprite: '{kvp.Key}'");
        }
        #else
        // Fallback for builds
        LoadSpritesFromResources();
        #endif
        
        Debug.Log($"RealSpriteLoader: Total sprites loaded: {spriteCache.Count}");
        
        // Log all loaded sprites
        foreach (var kvp in spriteCache)
        {
            Debug.Log($"RealSpriteLoader: Cached sprite - {kvp.Key}");
        }
    }
    
    #if UNITY_EDITOR
    void LoadSpritesFromPath(string folderPath, string category)
    {
        Debug.Log($"RealSpriteLoader: Loading sprites from {folderPath}");
        
        // Get all assets in the folder
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });
        
        Debug.Log($"RealSpriteLoader: Found {guids.Length} sprites in {category}");
        
        foreach (string guid in guids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            
            if (sprite != null)
            {
                spriteCache[sprite.name] = sprite;
                Debug.Log($"RealSpriteLoader: Loaded {category} sprite: {sprite.name} from {assetPath}");
            }
        }
    }
    #endif
    
    void LoadSpritesFromResources()
    {
        Debug.Log("RealSpriteLoader: Loading sprites from Resources (build mode)");
        
        // Try to load from Resources folder if available
        var backgroundSprites = Resources.LoadAll<Sprite>("Sprites/background");
        var animalSprites = Resources.LoadAll<Sprite>("Sprites/Characters/animals");
        var wizardSprites = Resources.LoadAll<Sprite>("Sprites/Characters/Wizard");
        var letterSprites = Resources.LoadAll<Sprite>("Sprites/Letters/Glass_blue_alphabetics");
        
        foreach (var sprite in backgroundSprites) spriteCache[sprite.name] = sprite;
        foreach (var sprite in animalSprites) spriteCache[sprite.name] = sprite;
        foreach (var sprite in wizardSprites) spriteCache[sprite.name] = sprite;
        foreach (var sprite in letterSprites) spriteCache[sprite.name] = sprite;
    }
    
    public static Sprite GetSprite(string name)
    {
        Debug.Log($"RealSpriteLoader: Looking for sprite '{name}' in cache of {spriteCache.Count} sprites");
        
        if (spriteCache.ContainsKey(name))
        {
            Debug.Log($"RealSpriteLoader: Found sprite '{name}' in cache");
            return spriteCache[name];
        }
        
        // Try case-insensitive search
        var sprite = spriteCache.FirstOrDefault(kvp => 
            kvp.Key.Equals(name, System.StringComparison.OrdinalIgnoreCase));
        
        if (sprite.Value != null)
        {
            Debug.Log($"RealSpriteLoader: Found sprite '{name}' (case-insensitive)");
            return sprite.Value;
        }
        
        // Log all available sprites for debugging
        Debug.LogWarning($"RealSpriteLoader: Sprite '{name}' not found in cache. Available sprites:");
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
            GetAssetPath(kvp.Value).Contains(path));
        
        if (sprite.Value != null)
        {
            return sprite.Value;
        }
        
        Debug.LogWarning($"RealSpriteLoader: Sprite with path '{path}' not found");
        return null;
    }
    
    static string GetAssetPath(Sprite sprite)
    {
        #if UNITY_EDITOR
        return UnityEditor.AssetDatabase.GetAssetPath(sprite);
        #else
        return sprite.name; // Fallback for builds
        #endif
    }
} 