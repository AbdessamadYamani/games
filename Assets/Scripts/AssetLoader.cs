using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AssetLoader : MonoBehaviour
{
    public static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    
    void Awake()
    {
        LoadAllSprites();
    }
    
    void LoadAllSprites()
    {
        Debug.Log("AssetLoader: Starting sprite loading from actual folders...");
        
        // Load all sprites from the project
        var allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
        Debug.Log($"AssetLoader: Found {allSprites.Length} total sprites in project");
        
        // Filter and categorize sprites by their asset paths
        foreach (var sprite in allSprites)
        {
            string assetPath = GetAssetPath(sprite);
            
            if (assetPath.Contains("Sprites/background"))
            {
                spriteCache[sprite.name] = sprite;
                Debug.Log($"AssetLoader: Loaded background sprite: {sprite.name} from {assetPath}");
            }
            else if (assetPath.Contains("Sprites/Characters/animals"))
            {
                spriteCache[sprite.name] = sprite;
                Debug.Log($"AssetLoader: Loaded animal sprite: {sprite.name} from {assetPath}");
            }
            else if (assetPath.Contains("Sprites/Characters/Wizard"))
            {
                spriteCache[sprite.name] = sprite;
                Debug.Log($"AssetLoader: Loaded wizard sprite: {sprite.name} from {assetPath}");
            }
            else if (assetPath.Contains("Sprites/Letters"))
            {
                spriteCache[sprite.name] = sprite;
                Debug.Log($"AssetLoader: Loaded letter sprite: {sprite.name} from {assetPath}");
            }
        }
        
        Debug.Log($"AssetLoader: Total categorized sprites loaded: {spriteCache.Count}");
        
        // Log all loaded sprites
        foreach (var kvp in spriteCache)
        {
            Debug.Log($"AssetLoader: Cached sprite - {kvp.Key}");
        }
    }
    
    string GetAssetPath(Object obj)
    {
        #if UNITY_EDITOR
        return UnityEditor.AssetDatabase.GetAssetPath(obj);
        #else
        return obj.name; // Fallback for builds
        #endif
    }
    
    public static Sprite GetSprite(string name)
    {
        if (spriteCache.ContainsKey(name))
        {
            return spriteCache[name];
        }
        
        // Try case-insensitive search
        var sprite = spriteCache.FirstOrDefault(kvp => 
            kvp.Key.Equals(name, System.StringComparison.OrdinalIgnoreCase));
        
        if (sprite.Value != null)
        {
            return sprite.Value;
        }
        
        Debug.LogWarning($"AssetLoader: Sprite '{name}' not found in cache");
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
        
        Debug.LogWarning($"AssetLoader: Sprite with path '{path}' not found");
        return null;
    }
    
    static string GetAssetPathStatic(Object obj)
    {
        #if UNITY_EDITOR
        return UnityEditor.AssetDatabase.GetAssetPath(obj);
        #else
        return obj.name; // Fallback for builds
        #endif
    }
} 