using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Level4Manager : MonoBehaviour
{
    [Header("Level Settings")]
    public int currentPuzzle = 0;
    public int totalPuzzles = 3;
    public int stars = 0;
    
    [Header("UI Elements")]
    private Canvas gameCanvas;
    public Text questionText;
    public Text starsText;
    public Button returnButton;
    
    [Header("Game Objects")]
    public GameObject player;
    public GameObject ground;
    public List<GameObject> rocks = new List<GameObject>();
    public List<GameObject> letters = new List<GameObject>();
    
    // Audio disabled for Level 4 - it's a jumping puzzle
    
    [Header("Puzzle Data")]
    public List<string> vowelSequences = new List<string>
    {
        "OUI",
        "AEO",
        "UIA"
    };
    
    private int currentLetterIndex = 0;
    private string currentSequence;
    private GameObject currentLetterGO; // Track the current letter to show
    // Removed unused levelCompleted field
    
    void Start()
    {
        Debug.Log("Level4Manager: Start() called");
        SetupLevel();
    }
    
    void SetupLevel()
    {
        Debug.Log("Level4Manager: SetupLevel() called");
        
        // Setup camera
        SetupCamera();
        
        // Setup background
        SetupBackground();
        
        // Setup player
        SetupPlayer();
        
        // Setup ground
        CreateGroundSprite();
        
        // Setup UI
        SetupUI();
        
        // Setup audio
        SetupAudio();
        
        // Start first puzzle
        ShowNextSequence();
    }
    
    void SetupCamera()
    {
        // Create main camera for the game
        var camGO = new GameObject("MainCamera");
        var camera = camGO.AddComponent<Camera>();
        camGO.tag = "MainCamera";
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.cyan;
        camera.transform.position = new Vector3(0, 0, -10);
        
        Debug.Log("Level4Manager: Camera setup completed");
    }
    
    void SetupBackground()
    {
        Debug.Log("Level4Manager: SetupBackground() called");
        
        var backgroundGO = new GameObject("Background");
        var backgroundSR = backgroundGO.AddComponent<SpriteRenderer>();
        
        var backgroundSprite = ManualSpriteLoader.GetSprite("tempel_background");
        if (backgroundSprite != null)
        {
            backgroundSR.sprite = backgroundSprite;
            backgroundSR.sortingOrder = -10;
            backgroundGO.transform.position = new Vector3(0, 0, 1);
            backgroundGO.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            Debug.Log("Level4Manager: Background created successfully");
        }
        else
        {
            Debug.LogWarning("Level4Manager: Could not load tempel_background sprite");
        }
    }
    
    void SetupPlayer()
    {
        Debug.Log("Level4Manager: SetupPlayer() called");
        
        // Create player character
        var playerGO = new GameObject("Player");
        var playerSR = playerGO.AddComponent<SpriteRenderer>();
        
        var playerSprite = ManualSpriteLoader.GetSprite("Idle");
        if (playerSprite != null)
        {
            playerSR.sprite = playerSprite;
            playerSR.sortingOrder = 5;
            Debug.Log("Level4Manager: Player sprite loaded successfully");
        }
        else
        {
            // Create fallback player sprite
            var playerTexture = new Texture2D(1, 1);
            playerTexture.SetPixel(0, 0, Color.blue);
            playerTexture.Apply();
            
            var fallbackSprite = Sprite.Create(playerTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            playerSR.sprite = fallbackSprite;
            playerSR.sortingOrder = 5;
            Debug.LogWarning("Level4Manager: Using fallback player sprite");
        }
        
        // Add player controller (this will automatically add Rigidbody2D and BoxCollider2D)
        var playerController = playerGO.AddComponent<PlayerController>();
        
        // Get the Rigidbody2D that was automatically added by PlayerController
        var rb = playerGO.GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Get the BoxCollider2D that was automatically added by PlayerController
        var collider = playerGO.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(0.5f, 1f);
        
        // Position player on ground (ground is at -0.97, so player should be at 1)
        playerGO.transform.position = new Vector3(3.4f, 1f, 0);
        
        player = playerGO;
        Debug.Log("Level4Manager: Player created successfully");
    }
    
    void CreateGroundSprite()
    {
        Debug.Log("Level4Manager: CreateGroundSprite() called");
        
        // Create ground using the same approach as Level2Manager
        var groundGO = GameObject.CreatePrimitive(PrimitiveType.Quad);
        groundGO.name = "Ground";
        groundGO.transform.localScale = new Vector3(16f, 2f, 1);
        
        // Remove the default MeshCollider and add BoxCollider2D
        var meshCollider = groundGO.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            DestroyImmediate(meshCollider);
        }
        
        var groundSR = groundGO.GetComponent<MeshRenderer>();
        // Make ground fully visible with a brown color (like Level2Manager)
        groundSR.material.color = new Color(0.59f, 0.29f, 0f, 1f); // Brown color
        
        // Position ground as specified
        groundGO.transform.position = new Vector3(6.4f, -0.97f, 0);
        
        Debug.Log($"Level4Manager: Ground positioned at ({groundGO.transform.position.x}, {groundGO.transform.position.y}, {groundGO.transform.position.z})");
        
        // Add BoxCollider2D for physics
        groundGO.AddComponent<BoxCollider2D>();
        
        ground = groundGO;
        Debug.Log($"Level4Manager: Ground created successfully at position ({groundGO.transform.position.x}, {groundGO.transform.position.y}, {groundGO.transform.position.z}) with scale (16,2,1)");
    }
    
    void SetupUI()
    {
        Debug.Log("Level4Manager: SetupUI() called");
        
        // Create Canvas (same as Level 2)
        var canvasGO = new GameObject("GameCanvas");
        gameCanvas = canvasGO.AddComponent<Canvas>();
        gameCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        gameCanvas.sortingOrder = 100;
        var canvasScaler = canvasGO.AddComponent<CanvasScaler>();
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Create Question Text (same as Level 2)
        var questionGO = new GameObject("QuestionText");
        questionGO.transform.SetParent(canvasGO.transform, false);
        questionText = questionGO.AddComponent<Text>();
        questionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        questionText.fontSize = 48;
        questionText.color = Color.white;
        questionText.alignment = TextAnchor.MiddleCenter;
        questionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        questionText.verticalOverflow = VerticalWrapMode.Overflow;
        questionText.text = "Jump to the letters in order!";
        
        var questionRT = questionText.rectTransform;
        questionRT.anchorMin = new Vector2(0.1f, 0.7f);
        questionRT.anchorMax = new Vector2(0.9f, 0.9f);
        questionRT.offsetMin = Vector2.zero;
        questionRT.offsetMax = Vector2.zero;
        
        Debug.Log($"Level4Manager: Created question text: '{questionText.text}'");
        
        // Create Stars Text (same as Level 2)
        var starsGO = new GameObject("StarsText");
        starsGO.transform.SetParent(canvasGO.transform, false);
        starsText = starsGO.AddComponent<Text>();
        starsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        starsText.fontSize = 24;
        starsText.color = Color.yellow;
        starsText.alignment = TextAnchor.UpperRight;
        starsText.text = "Stars: 0";
        
        var starsRT = starsText.rectTransform;
        starsRT.anchorMin = new Vector2(0.7f, 0.9f);
        starsRT.anchorMax = new Vector2(0.95f, 0.95f);
        starsRT.offsetMin = Vector2.zero;
        starsRT.offsetMax = Vector2.zero;
        
        Debug.Log($"Level4Manager: Created stars text: '{starsText.text}'");
        
        // Create Return Button
        var returnGO = new GameObject("ReturnButton");
        returnGO.transform.SetParent(canvasGO.transform, false);
        returnButton = returnGO.AddComponent<Button>();
        var returnImage = returnGO.AddComponent<Image>();
        returnImage.color = Color.blue;
        
        var returnTextGO = new GameObject("ReturnText");
        returnTextGO.transform.SetParent(returnGO.transform, false);
        var returnText = returnTextGO.AddComponent<Text>();
        returnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        returnText.fontSize = 18;
        returnText.color = Color.white;
        returnText.text = "Return to Menu";
        returnText.alignment = TextAnchor.MiddleCenter;
        
        var returnTextRect = returnText.GetComponent<RectTransform>();
        returnTextRect.anchorMin = Vector2.zero;
        returnTextRect.anchorMax = Vector2.one;
        returnTextRect.offsetMin = Vector2.zero;
        returnTextRect.offsetMax = Vector2.zero;
        
        var returnButtonRect = returnButton.GetComponent<RectTransform>();
        returnButtonRect.anchorMin = new Vector2(0.05f, 0.9f);
        returnButtonRect.anchorMax = new Vector2(0.2f, 0.95f);
        returnButtonRect.offsetMin = Vector2.zero;
        returnButtonRect.offsetMax = Vector2.zero;
        
        returnButton.onClick.AddListener(() => ReturnToMenu());
        
        Debug.Log("Level4Manager: UI setup completed");
    }
    
    void SetupAudio()
    {
        Debug.Log("Level4Manager: SetupAudio() called - Audio disabled for Level 4");
        // Level 4 doesn't need audio feedback - it's a jumping puzzle
    }
    
    void ShowNextSequence()
    {
        Debug.Log("Level4Manager: ShowNextSequence() called");
        
        if (currentPuzzle >= totalPuzzles)
        {
            CompleteLevel();
            return;
        }
        
        // Clear previous letters and rocks
        ClearLevel();
        
        // Get current sequence
        currentSequence = vowelSequences[currentPuzzle];
        currentLetterIndex = 0;
        
        // Update question text for step-by-step guidance
        if (questionText != null)
        {
            questionText.text = $"Jump to letter: {currentSequence[currentLetterIndex]}";
            Debug.Log($"Level4Manager: Updated question text to: {questionText.text}");
        }
        else
        {
            Debug.LogError("Level4Manager: questionText is null!");
        }
        
        // Create rocks and letters
        CreateRocksAndLetters();
        
        // Show all letters and guide to first letter
        ShowCurrentLetter();
        
        Debug.Log($"Level4Manager: Showing puzzle {currentPuzzle + 1}: {currentSequence}");
    }
    
    void ShowCurrentLetter()
    {
        Debug.Log($"Level4Manager: ShowCurrentLetter() called for index {currentLetterIndex}");
        
        // Show all letters (they're all visible as platforms)
        foreach (var letter in letters)
        {
            if (letter != null)
            {
                var letterSR = letter.GetComponent<SpriteRenderer>();
                if (letterSR != null)
                {
                    letterSR.enabled = true;
                }
            }
        }
        
        // Update question text to guide player to current letter
        if (questionText != null && currentLetterIndex < currentSequence.Length)
        {
            questionText.text = $"Jump to letter: {currentSequence[currentLetterIndex]}";
            Debug.Log($"Level4Manager: Updated question text to guide player to letter '{currentSequence[currentLetterIndex]}'");
        }
    }
    
    void CreateRocksAndLetters()
    {
        Debug.Log("Level4Manager: CreateRocksAndLetters() called");
        
        Debug.Log($"Level4Manager: Creating {currentSequence.Length} letters for sequence: '{currentSequence}'");
        
        // Create rocks with letters
        for (int i = 0; i < currentSequence.Length; i++)
        {
            // Create rock
            var rockGO = new GameObject($"Rock_{i}");
            var rockSR = rockGO.AddComponent<SpriteRenderer>();
            
            // Create simple rock sprite
            var rockTexture = new Texture2D(1, 1);
            rockTexture.SetPixel(0, 0, Color.gray);
            rockTexture.Apply();
            
            var rockSprite = Sprite.Create(rockTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            rockSR.sprite = rockSprite;
            rockSR.sortingOrder = 2;
            
            // Add collider
            var rockCollider = rockGO.AddComponent<BoxCollider2D>();
            rockCollider.size = new Vector2(1f, 0.5f);
            
            // Position rocks below the ground (ground is at -0.97, so rocks should be at -2.5)
            float xPos = 2f + (i * 2f);
            rockGO.transform.position = new Vector3(xPos, -2.5f, 0);
            rockGO.transform.localScale = new Vector3(1f, 0.5f, 1f);
            
            rocks.Add(rockGO);
            
            // Create letter
            var letterGO = new GameObject($"Letter_{i}");
            var letterSR = letterGO.AddComponent<SpriteRenderer>();
            
            var letterSprite = ManualSpriteLoader.GetSprite($"{currentSequence[i].ToString().ToLower()}");
            Debug.Log($"Level4Manager: Trying to load letter sprite for '{currentSequence[i]}' (lowercase: '{currentSequence[i].ToString().ToLower()}')");
            if (letterSprite != null)
            {
                letterSR.sprite = letterSprite;
                letterSR.sortingOrder = 3;
                Debug.Log($"Level4Manager: Loaded letter sprite for '{currentSequence[i]}'");
            }
            else
            {
                // Create fallback letter (blue like in the image)
                var letterTexture = new Texture2D(1, 1);
                letterTexture.SetPixel(0, 0, Color.blue);
                letterTexture.Apply();
                
                var fallbackSprite = Sprite.Create(letterTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
                letterSR.sprite = fallbackSprite;
                letterSR.sortingOrder = 3;
                Debug.LogWarning($"Level4Manager: Using fallback sprite for letter '{currentSequence[i]}'");
            }
            
            // Add solid collider for player to jump on
            var letterCollider = letterGO.AddComponent<BoxCollider2D>();
            letterCollider.isTrigger = false;
            
            // Set collider size to match the sprite dimensions with extra coverage
            // Since letters are scaled to 0.34, we need to account for that
            letterCollider.size = new Vector2(2f, 2f); // Much larger coverage for both jumping and triggering
            
            // Add Rigidbody2D to make letter a solid platform
            var letterRb = letterGO.AddComponent<Rigidbody2D>();
            letterRb.bodyType = RigidbodyType2D.Static; // Static so it doesn't move
            letterRb.simulated = true; // Make sure physics simulation is enabled
            
            // Set the letter to the same layer as the ground for proper collision
            letterGO.layer = LayerMask.NameToLayer("Default");
            
            // Ensure the collider is properly configured
            letterCollider.sharedMaterial = null; // No friction material
            letterCollider.usedByEffector = false;
            
            // Add letter pickup script to the letter GameObject
            var letterPickup = letterGO.AddComponent<Level4Letter>();
            letterPickup.SetLevel4Manager(this);
            letterPickup.SetLetterIndex(i);
            letterPickup.SetLetter(currentSequence[i].ToString());
            Debug.Log($"Level4Manager: Set letter '{currentSequence[i]}' for trigger {i}");
            Debug.Log($"Level4Manager: Letter collider size: {letterCollider.size}, isTrigger: {letterCollider.isTrigger}");
            
            // Position letters at specified coordinates (adjusted for better jumping)
            Vector3[] letterPositions = {
                new Vector3(-6.40f, -1.5f, 0f),    // Letter 1 (higher)
                new Vector3(-3.70f, -0.5f, 0f),    // Letter 2 (higher)
                new Vector3(-9.17f, -0.3f, 0f)     // Letter 3 (higher)
            };
            
            if (i < letterPositions.Length)
            {
                letterGO.transform.position = letterPositions[i];
            }
            else
            {
                // Fallback position if more than 3 letters
                letterGO.transform.position = new Vector3(xPos, -2.5f - (i * 0.3f), 0);
            }
            letterGO.transform.localScale = new Vector3(0.34f, 0.34f, 1f); // Set letter scale to 0.34
            
            Debug.Log($"Level4Manager: Created letter '{currentSequence[i]}' at position {letterGO.transform.position} with scale (0.34, 0.34, 1)");
            Debug.Log($"Level4Manager: Letter '{currentSequence[i]}' has Rigidbody2D: {letterGO.GetComponent<Rigidbody2D>() != null}, Collider: {letterGO.GetComponent<BoxCollider2D>() != null}");
            Debug.Log($"Level4Manager: Letter '{currentSequence[i]}' collider size: {letterGO.GetComponent<BoxCollider2D>().size}");
            
            letters.Add(letterGO);
        }
        
        Debug.Log($"Level4Manager: Created {rocks.Count} rocks and {letters.Count} letters");
        
        // Debug: List all available sprites
        Debug.Log("Level4Manager: Available sprites in ManualSpriteLoader:");
        foreach (var kvp in ManualSpriteLoader.spriteCache)
        {
            if (kvp.Key.Length == 1 && char.IsLetter(kvp.Key[0]))
            {
                Debug.Log($"  - Letter sprite: '{kvp.Key}'");
            }
        }
        
        // Test specific letters we need
        Debug.Log("Level4Manager: Testing specific letter sprites:");
        foreach (char letter in currentSequence)
        {
            var testSprite = ManualSpriteLoader.GetSprite(letter.ToString().ToLower());
            Debug.Log($"  - Letter '{letter}' (lowercase '{letter.ToString().ToLower()}'): {(testSprite != null ? "FOUND" : "NOT FOUND")}");
        }
        
        // Debug: Check if ManualSpriteLoader has loaded any letter sprites
        int letterSpriteCount = 0;
        foreach (var kvp in ManualSpriteLoader.spriteCache)
        {
            if (kvp.Key.Length == 1 && char.IsLetter(kvp.Key[0]))
            {
                letterSpriteCount++;
            }
        }
        Debug.Log($"Level4Manager: ManualSpriteLoader has {letterSpriteCount} letter sprites loaded");
    }
    
    void ClearLevel()
    {
        Debug.Log("Level4Manager: ClearLevel() called");
        
        // Destroy rocks
        foreach (var rock in rocks)
        {
            if (rock != null)
                Destroy(rock);
        }
        rocks.Clear();
        
        // Destroy letters
        foreach (var letter in letters)
        {
            if (letter != null)
                Destroy(letter);
        }
        letters.Clear();
    }
    
    public void OnLetterCollected(int letterIndex)
    {
        Debug.Log($"Level4Manager: OnLetterCollected({letterIndex}) called");
        
        if (letterIndex == currentLetterIndex)
        {
            // Correct letter collected
            currentLetterIndex++;
            stars += 5;
            starsText.text = $"Stars: {stars}";
            
            Debug.Log($"Level4Manager: Correct letter collected. Progress: {currentLetterIndex}/{currentSequence.Length}");
            
            // Return player to ground
            ReturnPlayerToGround();
            
            if (currentLetterIndex >= currentSequence.Length)
            {
                // Sequence completed
                currentPuzzle++;
                ShowNextSequence();
            }
            else
            {
                // Show next letter
                ShowCurrentLetter();
            }
        }
        else
        {
            // Wrong letter collected
            stars = Mathf.Max(0, stars - 1);
            starsText.text = $"Stars: {stars}";
            
            // Show wrong letter message
            if (questionText != null)
            {
                questionText.text = "Wrong letter! Try again.";
                Debug.Log($"Level4Manager: Wrong letter collected. Expected: {currentLetterIndex}, Got: {letterIndex}");
                
                // Reset the question text after 2 seconds
                Invoke("ResetQuestionText", 2f);
            }
        }
    }
    
    void ReturnPlayerToGround()
    {
        Debug.Log("Level4Manager: Returning player to ground");
        
        if (player != null)
        {
            // Position player back on the ground
            player.transform.position = new Vector3(3.4f, 1f, 0);
            
            // Reset player velocity
            var playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
            }
            
            Debug.Log($"Level4Manager: Player returned to ground at position {player.transform.position}");
        }
    }
    
    void ResetQuestionText()
    {
        Debug.Log("Level4Manager: Resetting question text");
        
        if (questionText != null && currentLetterIndex < currentSequence.Length)
        {
            questionText.text = $"Jump to letter: {currentSequence[currentLetterIndex]}";
            Debug.Log($"Level4Manager: Question text reset to: {questionText.text}");
        }
    }
    
    void CompleteLevel()
    {
        Debug.Log("Level4Manager: CompleteLevel() called");
        
        questionText.text = "Level Complete!";
        
        // Return to menu after 2 seconds
        Invoke("ReturnToMenu", 2f);
    }
    
    void ReturnToMenu()
    {
        Debug.Log("Level4Manager: Returning to menu...");
        
        // Find MenuManager and return to level selection
        var menuManager = FindFirstObjectByType<MenuManager>();
        if (menuManager != null)
        {
            menuManager.ReturnToLevelSelection();
        }
        else
        {
            Debug.LogError("Level4Manager: MenuManager not found!");
        }
        
        // Destroy this Level4Manager completely
        DestroyImmediate(gameObject);
    }
} 