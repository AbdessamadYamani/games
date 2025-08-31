using UnityEngine;

public static class MasterInitializer
{
    static bool hasInitialized = false;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void InitializeGame()
    {
        if (!hasInitialized)
        {
            hasInitialized = true;
            
            Debug.Log("MasterInitializer: Starting automatic game setup...");
            
            // Clear any existing objects (except DontDestroyOnLoad)
            ClearScene();
            
            // Create all game components automatically
            CreateGameComponents();
            
            Debug.Log("MasterInitializer: Game setup complete!");
        }
    }
    
    static void ClearScene()
    {
        // Find all root objects except DontDestroyOnLoad
        var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        
        foreach (var obj in rootObjects)
        {
            if (obj.name != "DontDestroyOnLoad")
            {
                Object.DestroyImmediate(obj);
            }
        }
        
        Debug.Log("MasterInitializer: Scene cleared");
    }
    
    static void CreateGameComponents()
    {
        // Create ManualSpriteLoader (new sprite loading system)
        var assetLoaderGO = new GameObject("ManualSpriteLoader");
        assetLoaderGO.AddComponent<ManualSpriteLoader>();
        
        // Create a basic camera for menu rendering
        var cameraGO = new GameObject("MenuCamera");
        var camera = cameraGO.AddComponent<Camera>();
        cameraGO.tag = "MainCamera";
        camera.orthographic = true;
        camera.orthographicSize = 10f; // Larger size to see more of the scene
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f); // Dark background for menu
        camera.transform.position = new Vector3(0, 0, -10);
        
        Debug.Log($"MasterInitializer: MenuCamera created at position {camera.transform.position}");
        
        // Create GameManager (GameInitializer will be created when game starts)
        var gameManagerGO = new GameObject("GameManager");
        gameManagerGO.AddComponent<GameManager>();
        
        // Create MenuManager
        var menuGO = new GameObject("MenuManager");
        var menuManager = menuGO.AddComponent<MenuManager>();
        Debug.Log($"MasterInitializer: MenuManager created: {menuManager != null}");
        
        Debug.Log("MasterInitializer: All game components created");
    }
} 