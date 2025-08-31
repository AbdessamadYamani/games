using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Level3Manager : MonoBehaviour
{
    [System.Serializable]
    public struct AnimalPuzzle
    {
        public string animalName;
        public string animalSpriteName;
        public string[] correctParts;
        public string[] wrongParts;
    }
    
    private AnimalPuzzle[] puzzles = new AnimalPuzzle[]
    {
        new AnimalPuzzle 
        { 
            animalName = "PANDA", 
            animalSpriteName = "panda",
            correctParts = new string[] { "PAN", "DA" },
            wrongParts = new string[] { "PEN", "DA", "PAN", "DE", "PON", "DA", "PAN", "DO" }
        },
        new AnimalPuzzle 
        { 
            animalName = "DOLPHIN", 
            animalSpriteName = "Dolphin",
            correctParts = new string[] { "DOL", "PHIN" },
            wrongParts = new string[] { "DOL", "PHYN", "DOL", "PHEN", "DAL", "PHIN", "DOL", "PHAN" }
        },
        new AnimalPuzzle 
        { 
            animalName = "BEAR", 
            animalSpriteName = "bear",
            correctParts = new string[] { "BE", "AR" },
            wrongParts = new string[] { "BA", "ER", "BE", "RR", "BA", "AR", "BE", "ER" }
        }
    };
    
    private int currentPuzzleIndex = 0;
    private int currentStars = 0;
    private int totalPuzzles = 3;
    
    // UI Elements
    private Canvas gameCanvas;
    private Text questionText;
    private Text scoreText;
    private Text starsText;
    private Text resultText;
    private Button returnButton;
    private GameObject animalDisplay;
    private GameObject dropZone;
    
    // Drag and drop elements
    private List<GameObject> nameParts = new List<GameObject>();
    private GameObject draggedObject;
    private Vector3 dragOffset;
    
    void Start()
    {
        Debug.Log("Level3Manager: Starting Level 3 - Animal Name Puzzle");
        SetupLevel();
    }
    
    void SetupLevel()
    {
        SetupCamera();
        SetupBackground();
        SetupPlayerCharacter();
        SetupUI();
        ShowNextPuzzle();
    }
    
    void SetupCamera()
    {
        var camera = Camera.main;
        if (camera == null)
        {
            var cameraGO = new GameObject("Main Camera");
            camera = cameraGO.AddComponent<Camera>();
            cameraGO.tag = "MainCamera";
        }
        
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.transform.position = new Vector3(0, 0, -10);
        camera.backgroundColor = Color.white;
        
        Debug.Log("Level3Manager: Camera setup completed");
    }
    
    void SetupBackground()
    {
        var bgGO = new GameObject("Background");
        var bgSR = bgGO.AddComponent<SpriteRenderer>();
        var bgSprite = ManualSpriteLoader.GetSprite("mountains");
        
        if (bgSprite != null)
        {
            bgSR.sprite = bgSprite;
            Debug.Log("Level3Manager: Mountains background loaded successfully");
        }
        else
        {
            bgSR.color = Color.blue;
            Debug.LogWarning("Level3Manager: Mountains background not found, using fallback");
        }
        
        bgGO.transform.position = new Vector3(0, 0, 1);
        bgGO.transform.localScale = new Vector3(1.38f, 0.78f, 1f);
        
        Debug.Log("Level3Manager: Background setup completed");
    }
    
    void SetupPlayerCharacter()
    {
        var playerCharacter = new GameObject("PlayerCharacter");
        var playerSR = playerCharacter.AddComponent<SpriteRenderer>();
        var playerSprite = ManualSpriteLoader.GetSprite("Wizard");
        
        if (playerSprite != null)
        {
            playerSR.sprite = playerSprite;
            Debug.Log("Level3Manager: Player character loaded successfully");
        }
        else
        {
            playerSR.color = Color.red;
            Debug.LogWarning("Level3Manager: Player character not found, using fallback");
        }
        
        // Position character on the right side, big size
        playerCharacter.transform.position = new Vector3(8.02f, -2.52f, 0f);
        playerCharacter.transform.localScale = new Vector3(6.39f, 6.51f, 6.39f);
        
        Debug.Log("Level3Manager: Player character setup completed");
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
        
        // Create background for UI
        var bgGO = new GameObject("UIBackground");
        bgGO.transform.SetParent(canvasGO.transform, false);
        var bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.5f);
        var bgRT = bgImage.rectTransform;
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        
        // Question Text
        var questionGO = new GameObject("QuestionText");
        questionGO.transform.SetParent(canvasGO.transform, false);
        questionText = questionGO.AddComponent<Text>();
        questionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        questionText.fontSize = 36;
        questionText.color = Color.white;
        questionText.alignment = TextAnchor.MiddleCenter;
        var questionRT = questionText.rectTransform;
        questionRT.anchorMin = new Vector2(0.1f, 0.8f);
        questionRT.anchorMax = new Vector2(0.9f, 0.95f);
        questionRT.offsetMin = Vector2.zero;
        questionRT.offsetMax = Vector2.zero;
        
        // Score Text
        var scoreGO = new GameObject("ScoreText");
        scoreGO.transform.SetParent(canvasGO.transform, false);
        scoreText = scoreGO.AddComponent<Text>();
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.fontSize = 24;
        scoreText.color = Color.white;
        scoreText.alignment = TextAnchor.MiddleLeft;
        var scoreRT = scoreText.rectTransform;
        scoreRT.anchorMin = new Vector2(0.05f, 0.9f);
        scoreRT.anchorMax = new Vector2(0.4f, 0.98f);
        scoreRT.offsetMin = Vector2.zero;
        scoreRT.offsetMax = Vector2.zero;
        
        // Stars Text
        var starsGO = new GameObject("StarsText");
        starsGO.transform.SetParent(canvasGO.transform, false);
        starsText = starsGO.AddComponent<Text>();
        starsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        starsText.fontSize = 24;
        starsText.color = Color.yellow;
        starsText.alignment = TextAnchor.MiddleRight;
        var starsRT = starsText.rectTransform;
        starsRT.anchorMin = new Vector2(0.6f, 0.9f);
        starsRT.anchorMax = new Vector2(0.95f, 0.98f);
        starsRT.offsetMin = Vector2.zero;
        starsRT.offsetMax = Vector2.zero;
        
        // Result Text
        var resultGO = new GameObject("ResultText");
        resultGO.transform.SetParent(canvasGO.transform, false);
        resultText = resultGO.AddComponent<Text>();
        resultText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        resultText.fontSize = 48;
        resultText.color = Color.green;
        resultText.alignment = TextAnchor.MiddleCenter;
        resultText.gameObject.SetActive(false);
        var resultRT = resultText.rectTransform;
        resultRT.anchorMin = new Vector2(0.1f, 0.4f);
        resultRT.anchorMax = new Vector2(0.9f, 0.6f);
        resultRT.offsetMin = Vector2.zero;
        resultRT.offsetMax = Vector2.zero;
        
        // Return Button
        var returnGO = new GameObject("ReturnButton");
        returnGO.transform.SetParent(canvasGO.transform, false);
        returnButton = returnGO.AddComponent<Button>();
        var returnImage = returnGO.AddComponent<Image>();
        returnImage.color = Color.red;
        var returnTextGO = new GameObject("ReturnText");
        returnTextGO.transform.SetParent(returnGO.transform, false);
        var returnText = returnTextGO.AddComponent<Text>();
        returnText.text = "Return to Menu";
        returnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        returnText.fontSize = 24;
        returnText.color = Color.white;
        returnText.alignment = TextAnchor.MiddleCenter;
        var returnTextRT = returnText.rectTransform;
        returnTextRT.anchorMin = Vector2.zero;
        returnTextRT.anchorMax = Vector2.one;
        returnTextRT.offsetMin = Vector2.zero;
        returnTextRT.offsetMax = Vector2.zero;
        var returnRT = returnImage.rectTransform;
        returnRT.anchorMin = new Vector2(0.4f, 0.05f);
        returnRT.anchorMax = new Vector2(0.6f, 0.15f);
        returnRT.offsetMin = Vector2.zero;
        returnRT.offsetMax = Vector2.zero;
        returnButton.onClick.AddListener(ReturnToMenu);
        returnButton.gameObject.SetActive(false);
        
        // Animal Display (world space)
        animalDisplay = new GameObject("AnimalDisplay");
        var animalSR = animalDisplay.AddComponent<SpriteRenderer>();
        animalSR.transform.position = new Vector3(3.48f, 4.12f, 0f);
        animalSR.transform.localScale = new Vector3(0.51f, 0.51f, 1f);
        
        // Drop Zone (world space)
        dropZone = new GameObject("DropZone");
        var dropZoneSR = dropZone.AddComponent<SpriteRenderer>();
        dropZoneSR.color = new Color(0f, 1f, 0f, 0.3f);
        dropZoneSR.sprite = CreateWhiteBoxSprite();
        dropZone.transform.position = new Vector3(0f, -3f, 0f);
        dropZone.transform.localScale = new Vector3(12f, 2f, 1f); // Made wider
        var dropZoneCollider = dropZone.AddComponent<BoxCollider2D>();
        dropZoneCollider.isTrigger = true;
        
        Debug.Log($"Level3Manager: Drop zone created at position {dropZone.transform.position} with scale {dropZone.transform.localScale}");
        Debug.Log($"Level3Manager: Drop zone boundaries - X: -6 to 6, Y: -4 to -2");
        
        UpdateUI();
        Debug.Log("Level3Manager: UI setup completed");
    }
    
    void ShowNextPuzzle()
    {
        if (currentPuzzleIndex >= totalPuzzles)
        {
            CompleteLevel();
            return;
        }
        
        var puzzle = puzzles[currentPuzzleIndex];
        questionText.text = $"Gather the name of this animal:";
        
        // Set animal image
        var animalSR = animalDisplay.GetComponent<SpriteRenderer>();
        var animalSprite = ManualSpriteLoader.GetSprite(puzzle.animalSpriteName);
        if (animalSprite != null)
        {
            animalSR.sprite = animalSprite;
            Debug.Log($"Level3Manager: Loaded animal sprite: {puzzle.animalSpriteName}");
        }
        else
        {
            animalSR.color = Color.gray;
            Debug.LogWarning($"Level3Manager: Animal sprite not found: {puzzle.animalSpriteName}");
        }
        
        CreateNameParts(puzzle);
        Debug.Log($"Level3Manager: Showing puzzle {currentPuzzleIndex + 1}: {puzzle.animalName}");
    }
    
    void CreateNameParts(AnimalPuzzle puzzle)
    {
        // Clear existing parts
        foreach (var part in nameParts)
        {
            if (part != null)
            {
                DestroyImmediate(part);
            }
        }
        nameParts.Clear();
        
        // Combine correct and wrong parts, shuffle them
        var allParts = new List<string>();
        allParts.AddRange(puzzle.correctParts);
        allParts.AddRange(puzzle.wrongParts);
        
        Debug.Log($"Level3Manager: Creating parts for {puzzle.animalName}. Correct parts: [{string.Join(", ", puzzle.correctParts)}], Wrong parts: [{string.Join(", ", puzzle.wrongParts)}]");
        
        // Shuffle the parts
        for (int i = allParts.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = allParts[i];
            allParts[i] = allParts[j];
            allParts[j] = temp;
        }
        
        Debug.Log($"Level3Manager: Shuffled all parts: [{string.Join(", ", allParts)}]");
        
        // Create draggable parts
        for (int i = 0; i < allParts.Count; i++)
        {
            var partGO = new GameObject($"NamePart_{i}");
            var partSR = partGO.AddComponent<SpriteRenderer>();
            partSR.color = Color.white;
            partSR.sprite = CreateWhiteBoxSprite();
            
            // Position parts in a grid - moved higher to be above the drop zone
            int row = i / 4;
            int col = i % 4;
            float x = -6f + col * 3f;
            float y = 1f - row * 1.5f; // Changed from -1f to 1f to move higher
            Vector3 gridPosition = new Vector3(x, y, 0f);
            partGO.transform.position = gridPosition;
            partGO.transform.localScale = new Vector3(2f, 1f, 1f);
            
            Debug.Log($"Level3Manager: Created part '{allParts[i]}' at position {gridPosition}");
            
            // Add white box background (separate object)
            var whiteBoxGO = new GameObject("WhiteBox");
            whiteBoxGO.transform.SetParent(partGO.transform, false);
            var whiteBoxSR = whiteBoxGO.AddComponent<SpriteRenderer>();
            whiteBoxSR.color = Color.white;
            whiteBoxSR.sprite = CreateWhiteBoxSprite();
            whiteBoxGO.transform.localPosition = new Vector3(0f, 0f, -0.05f);
            whiteBoxGO.transform.localScale = new Vector3(2.5f, 1.5f, 1f); // Bigger white box
            
            // Add text (separate object, not child of white box)
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(partGO.transform, false); // Parent to partGO, not whiteBoxGO
            var textMesh = textGO.AddComponent<TextMesh>();
            textMesh.text = allParts[i];
            textMesh.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textMesh.fontSize = 30;
            textMesh.color = Color.black;
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textGO.transform.localPosition = new Vector3(0f, 0f, -0.1f);
            textGO.transform.localScale = Vector3.one * 0.1f;
            
            // Add collider and drag component
            var collider = partGO.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(2f, 1f);
            var dragComponent = partGO.AddComponent<DraggablePart>();
            dragComponent.SetLevel3Manager(this);
            dragComponent.SetPartText(allParts[i]);
            dragComponent.SetInitialGridPosition(gridPosition);
            
            nameParts.Add(partGO);
        }
        
        Debug.Log($"Level3Manager: Created {nameParts.Count} name parts");
    }
    
    public void OnPartDropped()
    {
        List<string> droppedParts = GetDroppedParts();
        Debug.Log($"Level3Manager: OnPartDropped called. Dropped parts count: {droppedParts.Count}");
        Debug.Log($"Level3Manager: Dropped parts: [{string.Join(", ", droppedParts)}]");
        
        if (droppedParts.Count == 2)
        {
            AnimalPuzzle puzzle = puzzles[currentPuzzleIndex];
            Debug.Log($"Level3Manager: Validating 2 parts. Puzzle: {puzzle.animalName}");
            Debug.Log($"Level3Manager: Expected correct parts: [{string.Join(", ", puzzle.correctParts)}]");
            
            // Check if both correct parts are present AND no wrong parts are included
            bool isCorrect = droppedParts.All(part => puzzle.correctParts.Contains(part)) && 
                           puzzle.correctParts.All(part => droppedParts.Contains(part));
            
            Debug.Log($"Level3Manager: Is correct combination: {isCorrect}");
            
            if (isCorrect)
            {
                // Correct combination
                int currentStars = PlayerPrefs.GetInt("PlayerStars", 0);
                PlayerPrefs.SetInt("PlayerStars", currentStars + 5);
                PlayerPrefs.Save();
                
                Debug.Log($"Level3Manager: Correct! +5 stars. New total: {currentStars + 5}");
                ShowResult("Correct! +5 stars", Color.green);
                Invoke(nameof(HideResultText), 2f);
                
                currentPuzzleIndex++;
                if (currentPuzzleIndex >= puzzles.Length)
                {
                    Invoke(nameof(CompleteLevel), 2f);
                }
                else
                {
                    Invoke(nameof(ShowNextPuzzle), 2f);
                }
            }
            else
            {
                // Wrong combination - clear both pieces from drop zone
                int currentStars = PlayerPrefs.GetInt("PlayerStars", 0);
                PlayerPrefs.SetInt("PlayerStars", Mathf.Max(0, currentStars - 1));
                PlayerPrefs.Save();
                
                Debug.Log($"Level3Manager: Wrong combination! -1 star. Clearing both pieces. New total: {Mathf.Max(0, currentStars - 1)}");
                ShowResult("Wrong combination! -1 star", Color.red);
                Invoke(nameof(HideResultText), 2f);
                
                // Clear both pieces from drop zone and return them to original positions
                ClearDropZone();
            }
        }
        else if (droppedParts.Count > 2)
        {
            Debug.Log($"Level3Manager: Too many parts ({droppedParts.Count}), removing last one");
            RemoveLastDroppedPart();
            ShowResult("Too many parts! Remove one piece.", Color.red);
            Invoke(nameof(HideResultText), 2f);
        }
        else
        {
            Debug.Log($"Level3Manager: Waiting for more parts. Current count: {droppedParts.Count}");
        }
        
        UpdateUI();
    }

    public bool IsInDropZone(Vector3 position)
    {
        // Drop zone is at (0, -3, 0) with scale (12, 2, 1), so it extends from:
        // X: -6 to 6, Y: -4 to -2
        bool inDropZone = position.x >= -6f && position.x <= 6f && 
                         position.y >= -4f && position.y <= -2f;
        
        Debug.Log($"Level3Manager: IsInDropZone check for position {position} - Result: {inDropZone}");
        return inDropZone;
    }

    List<string> GetDroppedParts()
    {
        List<string> droppedParts = new List<string>();
        
        DraggablePart[] allParts = FindObjectsByType<DraggablePart>(FindObjectsSortMode.None);
        Debug.Log($"Level3Manager: GetDroppedParts - Found {allParts.Length} total parts");
        
        foreach (DraggablePart part in allParts)
        {
            Debug.Log($"Level3Manager: Part '{part.GetPartText()}' - IsCurrentlyInDropZone: {part.IsCurrentlyInDropZone}, Position: {part.transform.position}");
            if (part.IsCurrentlyInDropZone)
            {
                droppedParts.Add(part.GetPartText());
                Debug.Log($"Level3Manager: Added '{part.GetPartText()}' to dropped parts list");
            }
        }
        
        Debug.Log($"Level3Manager: GetDroppedParts returning: [{string.Join(", ", droppedParts)}]");
        return droppedParts;
    }

    void ClearDropZone()
    {
        DraggablePart[] allParts = FindObjectsByType<DraggablePart>(FindObjectsSortMode.None);
        foreach (DraggablePart part in allParts)
        {
            if (part.IsCurrentlyInDropZone)
            {
                part.ReturnToInitialPosition();
            }
        }
    }

    void RemoveLastDroppedPart()
    {
        DraggablePart[] allParts = FindObjectsByType<DraggablePart>(FindObjectsSortMode.None);
        List<DraggablePart> droppedParts = new List<DraggablePart>();
        
        foreach (DraggablePart part in allParts)
        {
            if (part.IsCurrentlyInDropZone)
            {
                droppedParts.Add(part);
            }
        }
        
        if (droppedParts.Count > 0)
        {
            // Remove the last dropped part
            DraggablePart lastPart = droppedParts[droppedParts.Count - 1];
            lastPart.ReturnToInitialPosition();
        }
    }
    
    void ShowResult(string message, Color color)
    {
        resultText.text = message;
        resultText.color = color;
        resultText.gameObject.SetActive(true);
        Invoke("HideResultText", 2f);
    }
    
    void HideResultText()
    {
        resultText.gameObject.SetActive(false);
    }
    
    void UpdateUI()
    {
        scoreText.text = $"Puzzle: {currentPuzzleIndex + 1}/{totalPuzzles}";
        starsText.text = $"Stars: {currentStars}";
    }
    
    void CompleteLevel()
    {
        Debug.Log($"Level3Manager: Level completed! Final stars: {currentStars}");
        
        // Save stars
        int savedStars = PlayerPrefs.GetInt("PlayerStars", 0);
        PlayerPrefs.SetInt("PlayerStars", savedStars + currentStars);
        PlayerPrefs.Save();
        
        // Show completion message
        questionText.text = $"Level Complete!";
        scoreText.text = $"Final Score: {currentStars} stars";
        starsText.text = $"Total Stars: {savedStars + currentStars}";
        
        // Show return button
        returnButton.gameObject.SetActive(true);
        
        // Hide animal display and name parts
        if (animalDisplay != null) animalDisplay.SetActive(false);
        foreach (var part in nameParts)
        {
            if (part != null) part.SetActive(false);
        }
        if (dropZone != null) dropZone.SetActive(false);
    }
    
    void ReturnToMenu()
    {
        Debug.Log("Level3Manager: Returning to menu...");
        
        // Clean up all objects
        CleanupAllObjects();
        
        // Destroy this manager
        DestroyImmediate(gameObject);
        
        // Return to menu
        var menuManager = FindFirstObjectByType<MenuManager>();
        if (menuManager != null)
        {
            menuManager.ReturnToLevelSelection();
        }
    }
    
    void CleanupAllObjects()
    {
        Debug.Log("Level3Manager: Cleaning up all objects...");
        
        // Destroy all name parts
        foreach (var part in nameParts)
        {
            if (part != null)
            {
                DestroyImmediate(part);
            }
        }
        nameParts.Clear();
        
        // Destroy other objects
        if (animalDisplay != null) DestroyImmediate(animalDisplay);
        if (dropZone != null) DestroyImmediate(dropZone);
        
        // Destroy UI elements
        if (gameCanvas != null) DestroyImmediate(gameCanvas.gameObject);
        
        Debug.Log("Level3Manager: Cleanup completed");
    }
    
    Sprite CreateWhiteBoxSprite()
    {
        var texture = new Texture2D(32, 32);
        var pixels = new Color[32 * 32];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }
} 