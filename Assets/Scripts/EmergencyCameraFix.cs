using UnityEngine;

public class EmergencyCameraFix : MonoBehaviour
{
    void Start()
    {
        FixCamera();
    }
    
    [ContextMenu("Fix Camera")]
    public void FixCamera()
    {
        Debug.Log("=== Emergency Camera Fix ===");
        
        // Check if there's already a main camera
        var existingCamera = Camera.main;
        if (existingCamera != null)
        {
            Debug.Log($"Found existing main camera: {existingCamera.name}");
            ConfigureCamera(existingCamera);
        }
        else
        {
            Debug.Log("No main camera found, creating one...");
            CreateNewCamera();
        }
        
        Debug.Log("Camera fix complete!");
    }
    
    void CreateNewCamera()
    {
        // Create new camera
        var camGO = new GameObject("MainCamera");
        var camera = camGO.AddComponent<Camera>();
        camGO.tag = "MainCamera";
        
        ConfigureCamera(camera);
    }
    
    void ConfigureCamera(Camera camera)
    {
        // Configure camera settings
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.cyan;
        camera.transform.position = new Vector3(0, 0, -10);
        
        Debug.Log($"Camera configured: {camera.name} at position {camera.transform.position}");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 200, 100));
        if (GUILayout.Button("Fix Camera"))
        {
            FixCamera();
        }
        GUILayout.EndArea();
    }
} 