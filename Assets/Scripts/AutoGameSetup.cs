using UnityEngine;

public static class AutoGameSetup
{
    // Disabled to prevent multiple initializations - use MasterInitializer instead
    /*
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnRuntimeMethodLoad()
    {
        Debug.Log("AutoGameSetup: Starting automatic game setup...");
        
        // Create the game launcher automatically
        var launcherGO = new GameObject("GameLauncher");
        launcherGO.AddComponent<GameLauncher>();
        
        Debug.Log("AutoGameSetup: Game launcher created automatically!");
    }
    */
} 