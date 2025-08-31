using UnityEngine;
using UnityEngine.UI;

public class StarDisplayUpdater : MonoBehaviour
{
    private Text starText;
    private float updateInterval = 0.5f; // Update every 0.5 seconds
    private float nextUpdateTime;
    
    void Start()
    {
        // Find the star count text component
        FindStarTextComponent();
        
        // Update immediately
        UpdateStarDisplay();
    }
    
    void FindStarTextComponent()
    {
        // Try to find StarCount in the current GameObject's children first
        var starCountChild = transform.Find("StarCount");
        if (starCountChild != null)
        {
            starText = starCountChild.GetComponent<Text>();
            if (starText != null)
            {
                Debug.Log("StarDisplayUpdater: Found star count text component in children");
                return;
            }
        }
        
        // If not found in children, search globally
        var starCountGO = GameObject.Find("StarCount");
        if (starCountGO != null)
        {
            starText = starCountGO.GetComponent<Text>();
            if (starText != null)
            {
                Debug.Log("StarDisplayUpdater: Found star count text component globally");
            }
            else
            {
                Debug.LogWarning("StarDisplayUpdater: StarCount GameObject found but no Text component");
            }
        }
        else
        {
            Debug.LogWarning("StarDisplayUpdater: Could not find StarCount GameObject");
        }
    }
    
    void Update()
    {
        // Update star display periodically
        if (Time.time >= nextUpdateTime)
        {
            UpdateStarDisplay();
            nextUpdateTime = Time.time + updateInterval;
        }
    }
    
    void UpdateStarDisplay()
    {
        if (starText != null)
        {
            // Get stars from PlayerPrefs (same as GameManager)
            int currentStars = PlayerPrefs.GetInt("PlayerStars", 0);
            starText.text = $"{currentStars}";
            Debug.Log($"StarDisplayUpdater: Updated star display to {currentStars} stars");
        }
    }
} 