using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    void Awake()
    {
        // Create SpriteLoader first
        var spriteLoaderGO = new GameObject("SpriteLoader");
        spriteLoaderGO.AddComponent<SpriteLoader>();
        
        // Create GameInitializer
        var initializerGO = new GameObject("GameInitializer");
        initializerGO.AddComponent<GameInitializer>();
        
        // Create GameManager
        var gameManagerGO = new GameObject("GameManager");
        gameManagerGO.AddComponent<GameManager>();
    }
} 