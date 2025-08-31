using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    // static bool hasInitialized = false; // Disabled to prevent multiple initializations
    
    // Disabled to prevent multiple initializations - use MasterInitializer instead
    /*
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Initialize()
    {
        if (!hasInitialized)
        {
            hasInitialized = true;
            
            // Create the bootstrap object
            var bootstrapGO = new GameObject("Bootstrap");
            bootstrapGO.AddComponent<Bootstrap>();
            
            Debug.Log("Bootstrap: Automatic game initialization started");
        }
    }
    */
    
    void Awake()
    {
        // Create the game launcher
        var launcherGO = new GameObject("GameLauncher");
        launcherGO.AddComponent<GameLauncher>();
        
        // Destroy this bootstrap object after setup
        Destroy(gameObject);
        
        Debug.Log("Bootstrap: Game setup complete, bootstrap destroyed");
    }
} 