using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpriteTest : MonoBehaviour
{
    [Header("Sprite Testing")]
    [SerializeField] private bool testOnStart = true;
    
    void Start()
    {
        if (testOnStart)
        {
            TestSpriteLoading();
        }
    }
    
    [ContextMenu("Test Sprite Loading")]
    public void TestSpriteLoading()
    {
        Debug.Log("=== Sprite Loading Test ===");
        
        // Test ManualSpriteLoader cache
        Debug.Log($"Total sprites in ManualSpriteLoader cache: {ManualSpriteLoader.spriteCache.Count}");
        
        // Test background sprites
        var bgSprites = ManualSpriteLoader.spriteCache.Where(kvp => 
            GetAssetPath(kvp.Value).Contains("Sprites/background")).ToList();
        Debug.Log($"Background sprites found: {bgSprites.Count}");
        foreach (var kvp in bgSprites)
        {
            Debug.Log($"  - {kvp.Key}");
        }
        
        // Test animal sprites
        var animalSprites = ManualSpriteLoader.spriteCache.Where(kvp => 
            GetAssetPath(kvp.Value).Contains("Sprites/Characters/animals")).ToList();
        Debug.Log($"Animal sprites found: {animalSprites.Count}");
        foreach (var kvp in animalSprites)
        {
            Debug.Log($"  - {kvp.Key}");
        }
        
        // Test wizard sprites
        var wizardSprites = ManualSpriteLoader.spriteCache.Where(kvp => 
            GetAssetPath(kvp.Value).Contains("Sprites/Characters/Wizard")).ToList();
        Debug.Log($"Wizard sprites found: {wizardSprites.Count}");
        foreach (var kvp in wizardSprites)
        {
            Debug.Log($"  - {kvp.Key}");
        }
        
        // Test letter sprites
        var letterSprites = ManualSpriteLoader.spriteCache.Where(kvp => 
            GetAssetPath(kvp.Value).Contains("Sprites/Letters")).ToList();
        Debug.Log($"Letter sprites found: {letterSprites.Count}");
        foreach (var kvp in letterSprites)
        {
            Debug.Log($"  - {kvp.Key}");
        }
        
        Debug.Log("=== End Sprite Test ===");
    }
    
    string GetAssetPath(Object obj)
    {
        #if UNITY_EDITOR
        return UnityEditor.AssetDatabase.GetAssetPath(obj);
        #else
        return obj.name; // Fallback for builds
        #endif
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 120, 200, 100));
        if (GUILayout.Button("Test Sprites"))
        {
            TestSpriteLoading();
        }
        GUILayout.EndArea();
    }
} 