using UnityEngine;

public class CameraDebug : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== Camera Debug Info ===");
        
        // Check for cameras in scene
        var cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        Debug.Log($"Found {cameras.Length} cameras in scene");
        
        foreach (var cam in cameras)
        {
            Debug.Log($"Camera: {cam.name}, Tag: {cam.tag}, Enabled: {cam.enabled}, Position: {cam.transform.position}");
            if (cam.tag == "MainCamera")
            {
                Debug.Log($"Main Camera: {cam.name} at position {cam.transform.position}");
            }
        }
        
        // Check if there's a main camera
        var mainCam = Camera.main;
        if (mainCam != null)
        {
            Debug.Log($"Camera.main found: {mainCam.name}");
        }
        else
        {
            Debug.Log("Camera.main is null!");
        }
        
        // Check scene view
        Debug.Log($"Screen dimensions: {Screen.width} x {Screen.height}");
        Debug.Log($"Application.isPlaying: {Application.isPlaying}");
        
        Debug.Log("=== End Camera Debug ===");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("Camera Debug Info:");
        
        var cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        GUILayout.Label($"Cameras found: {cameras.Length}");
        
        var mainCam = Camera.main;
        if (mainCam != null)
        {
            GUILayout.Label($"Main Camera: {mainCam.name}");
            GUILayout.Label($"Position: {mainCam.transform.position}");
            GUILayout.Label($"Enabled: {mainCam.enabled}");
        }
        else
        {
            GUILayout.Label("No Main Camera found!");
        }
        
        GUILayout.EndArea();
    }
} 