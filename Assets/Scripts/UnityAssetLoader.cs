using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UnityAssetLoader : MonoBehaviour
{
    public static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    
    void Awake()
    {
        LoadAllSprites();
    }
    
    void LoadAllSprites()
    {
        Debug.Log("UnityAssetLoader: Starting sprite loading...");
        
        // Load all sprites from the project using Resources.FindObjectsOfTypeAll
        var allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
        Debug.Log($"UnityAssetLoader: Found {allSprites.Length} total sprites in project");
        
        // Categorize sprites by their asset paths
        foreach (var sprite in allSprites)
        {
            string assetPath = GetAssetPath(sprite);
            
            if (assetPath.Contains("Sprites/background"))
            {
                spriteCache[sprite.name] = sprite;
                Debug.Log($"UnityAssetLoader: Loaded background sprite: {sprite.name}");
            }
            else if (assetPath.Contains("Sprites/Characters/animals"))
            {
                spriteCache[sprite.name] = sprite;
                Debug.Log($"UnityAssetLoader: Loaded animal sprite: {sprite.name}");
            }
            else if (assetPath.Contains("Sprites/Characters/Wizard"))
            {
                spriteCache[sprite.name] = sprite;
                Debug.Log($"UnityAssetLoader: Loaded wizard sprite: {sprite.name}");
            }
            else if (assetPath.Contains("Sprites/Letters"))
            {
                spriteCache[sprite.name] = sprite;
                Debug.Log($"UnityAssetLoader: Loaded letter sprite: {sprite.name}");
            }
        }
        
        Debug.Log($"UnityAssetLoader: Total categorized sprites loaded: {spriteCache.Count}");
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
        
        Debug.LogWarning($"UnityAssetLoader: Sprite '{name}' not found in cache");
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
        
        Debug.LogWarning($"UnityAssetLoader: Sprite with path '{path}' not found");
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