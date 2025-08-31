using UnityEngine;
using System.Collections.Generic;

public class SpriteLoader : MonoBehaviour
{
    public static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    
    void Awake()
    {
        LoadAllSprites();
    }
    
    void LoadAllSprites()
    {
        Debug.Log("SpriteLoader: Starting sprite loading...");
        
        // Load background sprites
        var backgroundSprites = Resources.LoadAll<Sprite>("Sprites/background");
        Debug.Log($"SpriteLoader: Found {backgroundSprites.Length} background sprites");
        foreach (var sprite in backgroundSprites)
        {
            spriteCache[sprite.name] = sprite;
        }
        
        // Load animal sprites
        var animalSprites = Resources.LoadAll<Sprite>("Sprites/Characters/animals");
        Debug.Log($"SpriteLoader: Found {animalSprites.Length} animal sprites");
        foreach (var sprite in animalSprites)
        {
            spriteCache[sprite.name] = sprite;
        }
        
        // Load wizard sprites
        var wizardSprites = Resources.LoadAll<Sprite>("Sprites/Characters/Wizard");
        Debug.Log($"SpriteLoader: Found {wizardSprites.Length} wizard sprites");
        foreach (var sprite in wizardSprites)
        {
            spriteCache[sprite.name] = sprite;
        }
        
        // Load letter sprites
        var letterSprites = Resources.LoadAll<Sprite>("Sprites/Letters/Glass_blue_alphabetics");
        Debug.Log($"SpriteLoader: Found {letterSprites.Length} letter sprites");
        foreach (var sprite in letterSprites)
        {
            spriteCache[sprite.name] = sprite;
        }
        
        Debug.Log($"SpriteLoader: Total sprites loaded: {spriteCache.Count}");
    }
    
    public static Sprite GetSprite(string name)
    {
        if (spriteCache.ContainsKey(name))
        {
            return spriteCache[name];
        }
        return null;
    }
} 