using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Level2Manager : MonoBehaviour
{
    [System.Serializable]
    public class WordPuzzle
    {
        public string question;
        public string correctAnswer;
        public string wrongAnswer1;
        public string wrongAnswer2;
    }

    public List<WordPuzzle> puzzles = new List<WordPuzzle>
    {
        new WordPuzzle { question = "What is the color of the bear?", correctAnswer = "Brown", wrongAnswer1 = "Brwon", wrongAnswer2 = "Browm" },
        new WordPuzzle { question = "What do you call a baby cat?", correctAnswer = "Kitten", wrongAnswer1 = "Kittin", wrongAnswer2 = "Kiten" },
        new WordPuzzle { question = "What do you use to write on paper?", correctAnswer = "Pencil", wrongAnswer1 = "Pensil", wrongAnswer2 = "Pencel" },
        new WordPuzzle { question = "What do you call a baby dog?", correctAnswer = "Puppy", wrongAnswer1 = "Pupy", wrongAnswer2 = "Puppi" },
        new WordPuzzle { question = "What color is the sky?", correctAnswer = "Blue", wrongAnswer1 = "Bule", wrongAnswer2 = "Blu" },
        new WordPuzzle { question = "What do you call a baby bird?", correctAnswer = "Chick", wrongAnswer1 = "Chik", wrongAnswer2 = "Chic" },
        new WordPuzzle { question = "What do you use to eat soup?", correctAnswer = "Spoon", wrongAnswer1 = "Spon", wrongAnswer2 = "Spoo" },
        new WordPuzzle { question = "What color is grass?", correctAnswer = "Green", wrongAnswer1 = "Gren", wrongAnswer2 = "Grean" },
        new WordPuzzle { question = "What do you call a baby cow?", correctAnswer = "Calf", wrongAnswer1 = "Caf", wrongAnswer2 = "Cal" },
        new WordPuzzle { question = "What do you use to cut paper?", correctAnswer = "Scissors", wrongAnswer1 = "Sissors", wrongAnswer2 = "Scisor" }
    };

    private Canvas gameCanvas;
    private Text questionText;
    private GameObject[] ghostAnswers = new GameObject[3];
    private Text scoreText;
    private Text starsText;
    private GameObject successText;
    private GameObject failureText;
    
    private int currentPuzzleIndex = 0;
    private int currentScore = 0;
    private int currentStars = 0;
    private int puzzlesCompleted = 0;
    private int totalPuzzles = 3; // Player needs to complete 3 puzzles
    
    private Camera mainCam;
    private GameObject playerCharacter;
    private GameObject background;

    void Start()
    {
        Debug.Log("Level2Manager: Starting Level 2...");
        
        // Complete cleanup of everything before starting
        CompleteCleanup();
        
        // Load saved stars
        currentStars = PlayerPrefs.GetInt("PlayerStars", 0);
        
        // Setup the level
        SetupLevel();
    }

    void SetupLevel()
    {
        Debug.Log("Level2Manager: Setting up Level 2...");
        
        // Setup camera
        SetupCamera();
        
        // Setup background (twilight)
        SetupBackground();
        
        // Setup ground/platforms (similar to Level 1)
        SetupGround();
        
        // Setup player character (static)
        SetupPlayerCharacter();
        
        // Setup UI
        SetupUI();
        
        // Start first puzzle
        ShowNextPuzzle();
    }

    void SetupCamera()
    {
        // Create main camera for the game
        var camGO = new GameObject("MainCamera");
        mainCam = camGO.AddComponent<Camera>();
        camGO.tag = "MainCamera";
        mainCam.orthographic = true;
        mainCam.orthographicSize = 5f;
        mainCam.clearFlags = CameraClearFlags.SolidColor;
        mainCam.backgroundColor = Color.cyan;
        mainCam.transform.position = new Vector3(0, 0, -10);
    }

    void SetupBackground()
    {
        // Create background with twilight sprite
        background = new GameObject("Background");
        var bgSR = background.AddComponent<SpriteRenderer>();
        
        var bgSprite = ManualSpriteLoader.GetSprite("twilight");
        if (bgSprite != null)
        {
            bgSR.sprite = bgSprite;
            float scaleX = 16f / bgSprite.bounds.size.x;
            float scaleY = 10f / bgSprite.bounds.size.y;
            float scale = Mathf.Max(scaleX, scaleY) * 1.5f;
            background.transform.localScale = Vector3.one * scale;
        }
        else
        {
            // Fallback: create a twilight-colored background
            bgSR.color = new Color(0.2f, 0.1f, 0.4f); // Dark purple-blue
            background.transform.localScale = new Vector3(16f, 10f, 1);
        }
        
        background.transform.position = new Vector3(0, 0, 1);
    }

    void SetupGround()
    {
        // Create ground (bottom 20% of screen) - similar to Level 1
        var ground = GameObject.CreatePrimitive(PrimitiveType.Quad);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(16f, 2f, 1);
        
        // Remove the default MeshCollider and add BoxCollider2D
        var meshCollider = ground.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            DestroyImmediate(meshCollider);
        }
        
        var groundSR = ground.GetComponent<MeshRenderer>();
        // Make ground fully visible with a brown color
        groundSR.material.color = new Color(0.59f, 0.29f, 0f, 1f); // Brown color
        
        // Position ground at the bottom
        ground.transform.position = new Vector3(0, -4f, 0);
        
        // Add BoxCollider2D for physics
        ground.AddComponent<BoxCollider2D>();
        
        // Create some decorative platforms
        CreatePlatform(new Vector3(-6f, -2f, 0), new Vector3(2f, 0.5f, 1));
        CreatePlatform(new Vector3(6f, -2f, 0), new Vector3(2f, 0.5f, 1));
        CreatePlatform(new Vector3(0f, -1f, 0), new Vector3(3f, 0.5f, 1));
    }
    
    void CreatePlatform(Vector3 position, Vector3 scale)
    {
        var platform = GameObject.CreatePrimitive(PrimitiveType.Quad);
        platform.name = "Platform";
        platform.transform.localScale = scale;
        
        // Remove the default MeshCollider
        var meshCollider = platform.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            DestroyImmediate(meshCollider);
        }
        
        var platformSR = platform.GetComponent<MeshRenderer>();
        // Use a lighter brown color for platforms
        platformSR.material.color = new Color(0.8f, 0.6f, 0.4f, 1f);
        
        platform.transform.position = position;
        
        // Add BoxCollider2D for physics
        platform.AddComponent<BoxCollider2D>();
    }

    void SetupPlayerCharacter()
    {
        // Create player character (bigger, positioned on the right)
        playerCharacter = new GameObject("PlayerCharacter");
        var playerSR = playerCharacter.AddComponent<SpriteRenderer>();
        
        // Try to load the current character sprite
        string currentCharacter = PlayerPrefs.GetString("CurrentCharacter", "Wizard");
        var characterSprite = ManualSpriteLoader.GetSprite(currentCharacter);
        
        if (characterSprite != null)
        {
            playerSR.sprite = characterSprite;
            playerCharacter.transform.localScale = new Vector3(6.39f, 6.51f, 6.39f);
        }
        else
        {
            // Fallback: create a simple colored square
            playerSR.color = Color.blue;
            playerCharacter.transform.localScale = new Vector3(6.39f, 6.51f, 6.39f);
        }
        
        // Position character at exact coordinates
        playerCharacter.transform.position = new Vector3(8.02f, -2.52f, 0f);
    }

    void SetupUI()
    {
        // Create Canvas
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

        // Create question text
        var questionGO = new GameObject("QuestionText");
        questionGO.transform.SetParent(canvasGO.transform, false);
        questionText = questionGO.AddComponent<Text>();
        questionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        questionText.fontSize = 48;
        questionText.color = Color.white;
        questionText.alignment = TextAnchor.MiddleCenter;
        questionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        questionText.verticalOverflow = VerticalWrapMode.Overflow;
        
        var questionRT = questionText.rectTransform;
        questionRT.anchorMin = new Vector2(0.1f, 0.7f);
        questionRT.anchorMax = new Vector2(0.9f, 0.9f);
        questionRT.offsetMin = Vector2.zero;
        questionRT.offsetMax = Vector2.zero;

        // Create score text
        var scoreGO = new GameObject("ScoreText");
        scoreGO.transform.SetParent(canvasGO.transform, false);
        scoreText = scoreGO.AddComponent<Text>();
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.fontSize = 24;
        scoreText.color = Color.white;
        scoreText.alignment = TextAnchor.UpperLeft;
        
        var scoreRT = scoreText.rectTransform;
        scoreRT.anchorMin = new Vector2(0.05f, 0.9f);
        scoreRT.anchorMax = new Vector2(0.3f, 0.95f);
        scoreRT.offsetMin = Vector2.zero;
        scoreRT.offsetMax = Vector2.zero;

        // Create stars text
        var starsGO = new GameObject("StarsText");
        starsGO.transform.SetParent(canvasGO.transform, false);
        starsText = starsGO.AddComponent<Text>();
        starsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        starsText.fontSize = 24;
        starsText.color = Color.yellow;
        starsText.alignment = TextAnchor.UpperRight;
        
        var starsRT = starsText.rectTransform;
        starsRT.anchorMin = new Vector2(0.7f, 0.9f);
        starsRT.anchorMax = new Vector2(0.95f, 0.95f);
        starsRT.offsetMin = Vector2.zero;
        starsRT.offsetMax = Vector2.zero;

        // Create success text (hidden initially)
        var successGO = new GameObject("SuccessText");
        successGO.transform.SetParent(canvasGO.transform, false);
        successText = successGO;
        var successTextComponent = successGO.AddComponent<Text>();
        successTextComponent.text = "CORRECT! +5 Stars";
        successTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        successTextComponent.fontSize = 72;
        successTextComponent.color = Color.green;
        successTextComponent.alignment = TextAnchor.MiddleCenter;
        
        var successRT = successTextComponent.rectTransform;
        successRT.anchorMin = new Vector2(0.1f, 0.4f);
        successRT.anchorMax = new Vector2(0.9f, 0.6f);
        successRT.offsetMin = Vector2.zero;
        successRT.offsetMax = Vector2.zero;
        
        successText.SetActive(false);

        // Create failure text (hidden initially)
        var failureGO = new GameObject("FailureText");
        failureGO.transform.SetParent(canvasGO.transform, false);
        failureText = failureGO;
        var failureTextComponent = failureGO.AddComponent<Text>();
        failureTextComponent.text = "WRONG! -1 Star";
        failureTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        failureTextComponent.fontSize = 72;
        failureTextComponent.color = Color.red;
        failureTextComponent.alignment = TextAnchor.MiddleCenter;
        
        var failureRT = failureTextComponent.rectTransform;
        failureRT.anchorMin = new Vector2(0.1f, 0.4f);
        failureRT.anchorMax = new Vector2(0.9f, 0.6f);
        failureRT.offsetMin = Vector2.zero;
        failureRT.offsetMax = Vector2.zero;
        
        failureText.SetActive(false);

        UpdateUI();
    }

    void ShowNextPuzzle()
    {
        if (currentPuzzleIndex >= puzzles.Count)
        {
            currentPuzzleIndex = 0; // Loop back to start if needed
        }

        var puzzle = puzzles[currentPuzzleIndex];
        questionText.text = puzzle.question;

        // Create list of answers and shuffle them
        var answers = new List<string> { puzzle.correctAnswer, puzzle.wrongAnswer1, puzzle.wrongAnswer2 };
        answers = answers.OrderBy(x => Random.value).ToList();

        // Create ghost answer options
        CreateGhostAnswers(answers);

        currentPuzzleIndex++;
    }

    void CreateGhostAnswers(List<string> answers)
    {
        // Clear existing ghost answers
        foreach (var ghost in ghostAnswers)
        {
            if (ghost != null)
            {
                DestroyImmediate(ghost);
            }
        }
        
        // Also clean up any ghost objects that might exist in the scene
        GameObject[] existingGhosts = GameObject.FindGameObjectsWithTag("Untagged");
        foreach (GameObject obj in existingGhosts)
        {
            if (obj.name.Contains("GhostAnswer") || obj.GetComponent<GhostMovement>() != null)
            {
                Debug.Log($"Level2Manager: Destroying existing ghost during creation: {obj.name}");
                DestroyImmediate(obj);
            }
        }

        // Create new ghost answers as world objects
        for (int i = 0; i < 3; i++)
        {
            var ghostGO = new GameObject($"GhostAnswer{i}");
            
            // Add SpriteRenderer instead of Image for world objects
            var ghostSR = ghostGO.AddComponent<SpriteRenderer>();
            var ghostSprite = ManualSpriteLoader.GetSprite("Ghost");
            
            if (ghostSprite != null)
            {
                ghostSR.sprite = ghostSprite;
                Debug.Log($"Level2Manager: Ghost sprite loaded for ghost {i}");
            }
            else
            {
                // Fallback: create a white circle
                ghostSR.color = Color.white;
                Debug.LogWarning($"Level2Manager: Ghost sprite not found, using fallback for ghost {i}");
            }
            
            // Make ghosts smaller
            ghostGO.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            
            // Set initial random position within screen bounds
            float x = Random.Range(-6f, 6f);
            float y = Random.Range(-3f, 2f);
            ghostGO.transform.position = new Vector3(x, y, 0);
            
            // Add white box for answer text on ghost's head (as world object)
            var textBoxGO = new GameObject("AnswerTextBox");
            textBoxGO.transform.SetParent(ghostGO.transform, false);
            
            var textBoxSR = textBoxGO.AddComponent<SpriteRenderer>();
            textBoxSR.color = Color.white;
            textBoxSR.sprite = CreateWhiteBoxSprite();
            
            // Position text box on ghost's head at Y 1.45
            textBoxGO.transform.localPosition = new Vector3(0, 1.45f, -0.1f);
            textBoxGO.transform.localScale = new Vector3(8f, 3f, 1f); // Much bigger white box
            
            // Add text as separate world object (not child of white box)
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(ghostGO.transform, false); // Parent to ghost, not white box
            var textMesh = textGO.AddComponent<TextMesh>();
            textMesh.text = answers[i];
            textMesh.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textMesh.fontSize = 50; // Keep same font size
            textMesh.color = Color.black;
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
            
            // Position text at same position as white box but as separate object
            textGO.transform.localPosition = new Vector3(0, 1.45f, -0.2f); // Slightly behind white box
            textGO.transform.localScale = Vector3.one * 0.15f; // Independent scale
            
            // Add collider for clicking and collision detection
            var collider = ghostGO.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1f, 1f);
            
            // No need to set tag - we'll use component check instead
            
            // Add ghost movement component
            var ghostMovement = ghostGO.AddComponent<GhostMovement>();
            ghostMovement.SetAnswer(answers[i]);
            
            // Add click handler directly to ghost movement
            ghostMovement.SetLevel2Manager(this);
            
            ghostAnswers[i] = ghostGO;
            
            Debug.Log($"Level2Manager: Created ghost {i} at position {ghostGO.transform.position}");
        }
    }
    
    Sprite CreateWhiteBoxSprite()
    {
        // Create a simple white square sprite
        Texture2D texture = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }

    public void OnAnswerSelected(string selectedAnswer)
    {
        var currentPuzzle = puzzles[currentPuzzleIndex - 1]; // -1 because we incremented after showing the puzzle
        
        if (selectedAnswer == currentPuzzle.correctAnswer)
        {
            // Correct answer
            currentScore += 5;
            currentStars += 5;
            puzzlesCompleted++;
            
            successText.SetActive(true);
            Invoke("HideResultText", 2f);
            
            Debug.Log($"Level2Manager: Correct answer! Score: {currentScore}, Stars: {currentStars}");
        }
        else
        {
            // Wrong answer
            currentStars = Mathf.Max(0, currentStars - 1); // Don't go below 0
            
            failureText.SetActive(true);
            Invoke("HideResultText", 2f);
            
            Debug.Log($"Level2Manager: Wrong answer! Score: {currentScore}, Stars: {currentStars}");
        }

        // Save stars
        PlayerPrefs.SetInt("PlayerStars", currentStars);
        PlayerPrefs.Save();

        UpdateUI();

        // Check if level is complete
        if (puzzlesCompleted >= totalPuzzles)
        {
            Invoke("CompleteLevel", 2.5f);
        }
        else
        {
            Invoke("ShowNextPuzzle", 2.5f);
        }
    }

    void HideResultText()
    {
        successText.SetActive(false);
        failureText.SetActive(false);
    }

    void UpdateUI()
    {
        scoreText.text = $"Score: {currentScore}";
        starsText.text = $"Stars: {currentStars}";
    }

    void CompleteLevel()
    {
        Debug.Log($"Level2Manager: Level completed! Final score: {currentScore}, Stars: {currentStars}");
        
        // Show completion message
        questionText.text = $"LEVEL COMPLETE!\n\nFinal Score: {currentScore}\nStars Earned: {currentStars}\n\nGreat job!";
        
        // Disable ghost movement
        foreach (var ghost in ghostAnswers)
        {
            if (ghost != null)
            {
                var movement = ghost.GetComponent<GhostMovement>();
                if (movement != null)
                {
                    Destroy(movement);
                        }
    }
    

}
        
        // Create return to menu button
        var returnButtonGO = new GameObject("ReturnButton");
        returnButtonGO.transform.SetParent(gameCanvas.transform, false);
        
        var returnButton = returnButtonGO.AddComponent<Button>();
        var returnButtonImage = returnButtonGO.AddComponent<Image>();
        
        // Use blue box for return button (similar to menu style)
        returnButtonImage.color = new Color(0.3f, 0.6f, 1f, 0.9f);
        
        var returnButtonRT = returnButtonImage.rectTransform;
        returnButtonRT.anchorMin = new Vector2(0.3f, 0.1f);
        returnButtonRT.anchorMax = new Vector2(0.7f, 0.2f);
        returnButtonRT.offsetMin = Vector2.zero;
        returnButtonRT.offsetMax = Vector2.zero;
        
        // Add text to return button
        var textGO = new GameObject("Text");
        textGO.transform.SetParent(returnButtonGO.transform, false);
        var text = textGO.AddComponent<Text>();
        text.text = "Return to Menu";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 36;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        var textRT = text.rectTransform;
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        
        returnButton.onClick.AddListener(() => ReturnToMenu());
    }

    void ReturnToMenu()
    {
        Debug.Log("Level2Manager: Returning to menu...");
        
        // Find MenuManager and return to level selection
        var menuManager = FindFirstObjectByType<MenuManager>();
        if (menuManager != null)
        {
            menuManager.ReturnToLevelSelection();
        }
        else
        {
            Debug.LogError("Level2Manager: MenuManager not found!");
        }
        
        // Destroy this Level2Manager completely
        DestroyImmediate(gameObject);
    }
    
    void CleanupAllGhosts()
    {
        Debug.Log("Level2Manager: Cleaning up all ghosts before returning to menu...");
        
        // Destroy all ghost objects
        GameObject[] allGhosts = GameObject.FindGameObjectsWithTag("Untagged");
        foreach (GameObject obj in allGhosts)
        {
            if (obj.name.Contains("GhostAnswer") || obj.GetComponent<GhostMovement>() != null)
            {
                Debug.Log($"Level2Manager: Destroying ghost: {obj.name}");
                DestroyImmediate(obj);
            }
        }
        
        // Clear the ghost answers array
        for (int i = 0; i < ghostAnswers.Length; i++)
        {
            ghostAnswers[i] = null;
        }
    }
    
    void CompleteCleanup()
    {
        Debug.Log("Level2Manager: Performing complete cleanup...");
        
        // Clean up all ghosts
        CleanupAllGhosts();
        
        // Clean up UI elements
        if (gameCanvas != null)
        {
            DestroyImmediate(gameCanvas.gameObject);
        }
        
        // Clean up any remaining UI elements
        GameObject[] uiElements = GameObject.FindGameObjectsWithTag("Untagged");
        foreach (GameObject obj in uiElements)
        {
            if (obj.name.Contains("GameCanvas") || 
                obj.name.Contains("QuestionText") || 
                obj.name.Contains("ScoreText") || 
                obj.name.Contains("StarsText") || 
                obj.name.Contains("SuccessText") || 
                obj.name.Contains("FailureText") || 
                obj.name.Contains("ReturnButton"))
            {
                Debug.Log($"Level2Manager: Destroying UI element: {obj.name}");
                DestroyImmediate(obj);
            }
        }
        
        // Reset game state
        currentPuzzleIndex = 0;
        currentScore = 0;
        puzzlesCompleted = 0;
        
        // Clear references
        gameCanvas = null;
        questionText = null;
        scoreText = null;
        starsText = null;
        successText = null;
        failureText = null;
        
        Debug.Log("Level2Manager: Complete cleanup finished");
    }
} 