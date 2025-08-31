using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SimpleSpriteLoader : MonoBehaviour
{
    public static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    
    void Awake()
    {
        LoadAllSprites();
    }
    
    void LoadAllSprites()
    {
        Debug.Log("SimpleSpriteLoader: Starting sprite loading...");
        
        #if UNITY_EDITOR
        // Load sprites using direct AssetDatabase calls
        LoadSpritesFromFolder("Assets/Sprites/background", "background");
        LoadSpritesFromFolder("Assets/Sprites/Characters/animals", "animals");
        LoadSpritesFromFolder("Assets/Sprites/Characters/Wizard", "wizard");
        LoadSpritesFromFolder("Assets/Sprites/Letters/Glass_blue_alphabetics", "letters");
        #else
        // Fallback for builds
        LoadSpritesFromResources();
        #endif
        
        Debug.Log($"SimpleSpriteLoader: Total sprites loaded: {spriteCache.Count}");
        
        // Log all loaded sprites
        foreach (var kvp in spriteCache)
        {
            Debug.Log($"SimpleSpriteLoader: Cached sprite - {kvp.Key}");
        }
    }
    
    #if UNITY_EDITOR
    void LoadSpritesFromFolder(string folderPath, string category)
    {
        Debug.Log($"SimpleSpriteLoader: Loading sprites from {folderPath}");
        
        try
        {
            // Get all assets in the folder
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });
            Debug.Log($"SimpleSpriteLoader: Found {guids.Length} sprites in {category}");
            
            foreach (string guid in guids)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log($"SimpleSpriteLoader: Loading asset from path: {assetPath}");
                
                Sprite sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                
                if (sprite != null)
                {
                    spriteCache[sprite.name] = sprite;
                    Debug.Log($"SimpleSpriteLoader: Successfully loaded {category} sprite: {sprite.name} from {assetPath}");
                }
                else
                {
                    Debug.LogWarning($"SimpleSpriteLoader: Failed to load sprite from {assetPath}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SimpleSpriteLoader: Error loading sprites from {folderPath}: {e.Message}");
        }
    }
    #endif
    
    void LoadSpritesFromResources()
    {
        Debug.Log("SimpleSpriteLoader: Loading sprites from Resources (build mode)");
        
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
        Debug.Log($"SimpleSpriteLoader: Looking for sprite '{name}' in cache of {spriteCache.Count} sprites");
        
        if (spriteCache.ContainsKey(name))
        {
            Debug.Log($"SimpleSpriteLoader: Found sprite '{name}' in cache");
            return spriteCache[name];
        }
        
        // Try case-insensitive search
        var sprite = spriteCache.FirstOrDefault(kvp => 
            kvp.Key.Equals(name, System.StringComparison.OrdinalIgnoreCase));
        
        if (sprite.Value != null)
        {
            Debug.Log($"SimpleSpriteLoader: Found sprite '{name}' (case-insensitive)");
            return sprite.Value;
        }
        
        // Log all available sprites for debugging
        Debug.LogWarning($"SimpleSpriteLoader: Sprite '{name}' not found in cache. Available sprites:");
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
        
        Debug.LogWarning($"SimpleSpriteLoader: Sprite with path '{path}' not found");
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