using UnityEngine;

public class QuickStart : MonoBehaviour
{
    [Header("Quick Setup")]
    [SerializeField] private bool autoStart = true;
    [SerializeField] private bool includeDebug = true;
    
    void Start()
    {
        if (autoStart)
        {
            SetupGame();
        }
    }
    
    [ContextMenu("Setup Game")]
    public void SetupGame()
    {
        // Clear existing game objects (except this one)
        var existingObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var obj in existingObjects)
        {
            if (obj != gameObject && obj.name != "Main Camera" && obj.name != "Directional Light")
            {
                DestroyImmediate(obj);
            }
        }
        
        // Create debug components if enabled
        if (includeDebug)
        {
            var debugGO = new GameObject("DebugComponents");
            debugGO.AddComponent<CameraDebug>();
            debugGO.AddComponent<SpriteTest>();
        }
        
        // Create the game launcher
        var launcherGO = new GameObject("GameLauncher");
        launcherGO.AddComponent<GameLauncher>();
        
        Debug.Log("Game setup complete! Press Play to start.");
    }
    
    void OnGUI()
    {
        if (!autoStart)
        {
            GUILayout.BeginArea(new Rect(10, 10, 200, 100));
            if (GUILayout.Button("Setup Game"))
            {
                SetupGame();
            }
            GUILayout.EndArea();
        }
    }
} 