using UnityEngine;

public static class SceneInitializer
{
    // static bool hasInitialized = false; // Disabled to prevent multiple initializations
    
    // Disabled to prevent multiple initializations - use MasterInitializer instead
    /*
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void InitializeScene()
    {
        if (!hasInitialized)
        {
            hasInitialized = true;
            
            Debug.Log("SceneInitializer: Starting automatic scene setup...");
            
            // Clear any existing objects (except DontDestroyOnLoad)
            ClearScene();
            
            // Create all game components automatically
            CreateGameComponents();
            
            Debug.Log("SceneInitializer: Scene setup complete!");
        }
    }
    */
    
    static void ClearScene()
    {
        // Find all root objects except DontDestroyOnLoad
        var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        
        foreach (var obj in rootObjects)
        {
            if (obj.name != "DontDestroyOnLoad" && obj.name != "Bootstrap")
            {
                Object.DestroyImmediate(obj);
            }
        }
        
        Debug.Log("SceneInitializer: Scene cleared");
    }
    
    static void CreateGameComponents()
    {
        // Create SpriteLoader
        var spriteLoaderGO = new GameObject("SpriteLoader");
        spriteLoaderGO.AddComponent<SpriteLoader>();
        
        // Create GameInitializer
        var initializerGO = new GameObject("GameInitializer");
        initializerGO.AddComponent<GameInitializer>();
        
        // Create GameManager
        var gameManagerGO = new GameObject("GameManager");
        gameManagerGO.AddComponent<GameManager>();
        
        // Create debug components
        var debugGO = new GameObject("DebugComponents");
        debugGO.AddComponent<CameraDebug>();
        debugGO.AddComponent<SpriteTest>();
        
        Debug.Log("SceneInitializer: All game components created");
    }
} 