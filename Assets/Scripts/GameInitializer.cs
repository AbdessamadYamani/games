using UnityEngine;
using UnityEngine.UI;

public class GameInitializer : MonoBehaviour
{
    Camera mainCam;
    float worldHeight, worldWidth, groundHeight;

    void Awake()
    {
        SetupCamera();
        SetupWorld();
        SetupUI();
    }

    void SetupCamera()
    {
        // Destroy the menu camera first
        var menuCamera = GameObject.Find("MenuCamera");
        if (menuCamera != null)
        {
            DestroyImmediate(menuCamera);
        }
        
        // Create main camera for the game
        var camGO = new GameObject("MainCamera");
        mainCam = camGO.AddComponent<Camera>();
        camGO.tag = "MainCamera";
        mainCam.orthographic = true;
        mainCam.orthographicSize = 5f; // Fixed size regardless of window ratio
        mainCam.clearFlags = CameraClearFlags.SolidColor;
        mainCam.backgroundColor = Color.cyan;
        mainCam.transform.position = new Vector3(0, 0, -10);

        // Calculate world extents (fixed regardless of window ratio)
        worldHeight = mainCam.orthographicSize * 2f; // = 10 units
        worldWidth = worldHeight * 1.6f; // Fixed aspect ratio (16:10)

        Debug.Log($"Camera setup complete. World size: {worldWidth} x {worldHeight}");
    }

    void SetupWorld()
    {
        // Background
        var bg = new GameObject("Background");
        var bgSR = bg.AddComponent<SpriteRenderer>();
        
        // Try to load the desert background sprite
        var bgSprite = ManualSpriteLoader.GetSprite("desert");
        if (bgSprite == null)
        {
            // Try alternative loading method
            bgSprite = ManualSpriteLoader.GetSpriteByPath("Sprites/background/desert");
        }
        
        if (bgSprite != null)
        {
            bgSR.sprite = bgSprite;
            // Scale to fill the entire screen completely
            float scaleX = worldWidth / bgSprite.bounds.size.x;
            float scaleY = worldHeight / bgSprite.bounds.size.y;
            // Use the larger scale to ensure full coverage
            float scale = Mathf.Max(scaleX, scaleY) * 1.5f; // 50% extra to ensure no borders
            bg.transform.localScale = Vector3.one * scale;
        }
        else
        {
            // Fallback: create a colored background
            bgSR.color = new Color(0.8f, 0.6f, 0.4f); // Desert-like color
            bg.transform.localScale = new Vector3(worldWidth, worldHeight, 1);
        }
        
        bg.transform.position = new Vector3(0, 0, 1); // Behind everything

        // Ground (bottom 20% of screen)
        groundHeight = worldHeight * 0.2f;
        var ground = GameObject.CreatePrimitive(PrimitiveType.Quad);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(worldWidth, groundHeight, 1);
        
        // Remove the default MeshCollider and add BoxCollider2D
        var meshCollider = ground.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            DestroyImmediate(meshCollider);
        }
        
        var groundSR = ground.GetComponent<MeshRenderer>();
        // Make ground fully visible
        groundSR.material.color = new Color(0.59f, 0.29f, 0f, 1f); // Fully visible brown
        
        // Position ground at specified Y position
        ground.transform.position = new Vector3(0, GameData.groundY, 0);
        
        // Add BoxCollider2D for physics
        ground.AddComponent<BoxCollider2D>();

        // Store world dimensions for other scripts to use
        GameData.worldHeight = worldHeight;
        GameData.worldWidth = worldWidth;
        GameData.groundHeight = groundHeight;

        Debug.Log("World setup complete");
    }

    void SetupUI()
    {
        // Canvas
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>().referenceResolution = new Vector2(800, 600);
        canvasGO.AddComponent<GraphicRaycaster>();

        // Animal Display (specified position)
        var animalUI = new GameObject("AnimalDisplay");
        animalUI.transform.SetParent(canvasGO.transform, false);
        var img = animalUI.AddComponent<Image>();
        img.rectTransform.sizeDelta = new Vector2(100, 100);
        img.rectTransform.anchoredPosition = new Vector2(GameData.animalX, GameData.animalY);
        
        // Set a default color to prevent transparency issues
        img.color = Color.white; // Full opacity white

        // Black Text Field (specified position)
        var textField = new GameObject("TextField");
        textField.transform.SetParent(canvasGO.transform, false);
        var textFieldImg = textField.AddComponent<Image>();
        textFieldImg.color = Color.black;
        var textFieldRT = textFieldImg.rectTransform;
        textFieldRT.sizeDelta = new Vector2(400, 60);
        textFieldRT.anchoredPosition = new Vector2(0, GameData.textFieldY);

        // Score Text (top-right)
        var scoreGO = new GameObject("ScoreText");
        scoreGO.transform.SetParent(canvasGO.transform, false);
        var scoreText = scoreGO.AddComponent<Text>();
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.text = "Score: 0/3";
        scoreText.fontSize = 24;
        scoreText.color = Color.white;
        scoreText.alignment = TextAnchor.UpperRight;
        var scoreRT = scoreText.rectTransform;
        scoreRT.sizeDelta = new Vector2(200, 50);
        scoreRT.anchoredPosition = new Vector2(350, 250);

        // Success Text (center, hidden initially)
        var successGO = new GameObject("SuccessText");
        successGO.transform.SetParent(canvasGO.transform, false);
        var successText = successGO.AddComponent<Text>();
        successText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        successText.text = "SUCCESS! You completed all animals!";
        successText.fontSize = 36;
        successText.color = Color.green;
        successText.alignment = TextAnchor.MiddleCenter;
        var successRT = successText.rectTransform;
        successRT.sizeDelta = new Vector2(600, 100);
        successRT.anchoredPosition = new Vector2(0, 0);
        successGO.SetActive(false);

        // Star Display (top-left)
        var starDisplayGO = new GameObject("StarDisplay");
        starDisplayGO.transform.SetParent(canvasGO.transform, false);
        
        // Star Icon (larger to contain text)
        var starIconGO = new GameObject("StarIcon");
        starIconGO.transform.SetParent(starDisplayGO.transform, false);
        var starIcon = starIconGO.AddComponent<Image>();
        
        var starSprite = ManualSpriteLoader.GetSprite("star");
        if (starSprite != null)
        {
            starIcon.sprite = starSprite;
        }
        else
        {
            starIcon.color = Color.yellow;
        }
        
        var starIconRT = starIcon.rectTransform;
        starIconRT.sizeDelta = new Vector2(80, 80); // Larger to contain text
        starIconRT.anchoredPosition = new Vector2(-350, 250);
        
        // Star Count Text (inside the star)
        var starTextGO = new GameObject("StarCount");
        starTextGO.transform.SetParent(starIconGO.transform, false); // Parent to star icon
        var starText = starTextGO.AddComponent<Text>();
        starText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        // Get current star count from PlayerPrefs
        int currentStars = PlayerPrefs.GetInt("PlayerStars", 0);
        starText.text = $"{currentStars}";
        
        starText.fontSize = 20;
        starText.color = Color.black;
        starText.alignment = TextAnchor.MiddleCenter;
        var starTextRT = starText.rectTransform;
        starTextRT.sizeDelta = new Vector2(80, 80); // Same size as star
        starTextRT.anchoredPosition = new Vector2(0, 0); // Center of star
        
        // Add StarDisplayUpdater to update the star count
        starDisplayGO.AddComponent<StarDisplayUpdater>();

        Debug.Log("UI setup complete");
    }
} 