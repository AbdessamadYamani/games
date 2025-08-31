using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    private Canvas menuCanvas;
    private Image backgroundImage;
    private Button playButton;
    private Button prizeButton;
    private Button closeButton;
    private Canvas levelDescriptionCanvas;
    private Canvas shopCanvas;
    
    void Awake()
    {
        Debug.Log("MenuManager: Awake() called");
        Debug.Log($"MenuManager: GameObject name: {gameObject.name}, active: {gameObject.activeInHierarchy}");
        
        // Wait longer to ensure ManualSpriteLoader is ready
        Invoke("CreateMenuUI", 0.5f);
        Debug.Log("MenuManager: Invoked CreateMenuUI with 0.5s delay");
    }
    
    void Start()
    {
        Debug.Log("MenuManager: Start() called");
        // Check if menu was created after a delay
        Invoke("CheckMenuStatus", 2f);
    }
    
    void CheckMenuStatus()
    {
        Debug.Log("MenuManager: Checking menu status...");
        var canvas = GameObject.Find("MenuCanvas");
        if (canvas != null)
        {
            Debug.Log("MenuManager: MenuCanvas found!");
            var buttons = canvas.GetComponentsInChildren<Button>();
            Debug.Log($"MenuManager: Found {buttons.Length} buttons in canvas");
            foreach (var button in buttons)
            {
                Debug.Log($"MenuManager: Button found: {button.name}");
            }
        }
        else
        {
            Debug.LogWarning("MenuManager: MenuCanvas not found! Attempting to recreate...");
            // Try to recreate the menu if it's missing
            Invoke("CreateMenuUI", 0.1f);
        }
    }
    
    void Update()
    {
        // Simple mouse click test
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("MenuManager: Mouse clicked!");
            var mousePos = Input.mousePosition;
            Debug.Log($"MenuManager: Mouse position: {mousePos}");
        }
        
        // Update star display periodically
        if (Time.frameCount % 60 == 0) // Update every 60 frames (about once per second)
        {
            UpdateStarDisplay();
        }
    }
    
    public void UpdateStarDisplay()
    {
        var starCountGO = GameObject.Find("StarCount");
        if (starCountGO != null)
        {
            var starText = starCountGO.GetComponent<Text>();
            if (starText != null)
            {
                int currentStars = PlayerPrefs.GetInt("PlayerStars", 0);
                starText.text = $"{currentStars}";
            }
        }
    }
    
    void CreateMenuUI()
    {
        Debug.Log("MenuManager: CreateMenuUI() called - starting menu creation...");
        
        // Check if ManualSpriteLoader is ready
        var spriteLoader = FindFirstObjectByType<ManualSpriteLoader>();
        if (spriteLoader == null)
        {
            Debug.LogError("MenuManager: ManualSpriteLoader not found!");
            return;
        }
        
        Debug.Log($"MenuManager: ManualSpriteLoader found, sprite cache has {ManualSpriteLoader.spriteCache.Count} sprites");
        
        // Log all available sprites
        foreach (var kvp in ManualSpriteLoader.spriteCache)
        {
            Debug.Log($"MenuManager: Available sprite: {kvp.Key}");
        }
        
        // If no sprites are loaded yet, wait a bit more
        if (ManualSpriteLoader.spriteCache.Count == 0)
        {
            Debug.Log("MenuManager: No sprites loaded yet, waiting...");
            Invoke("CreateMenuUI", 0.5f);
            return;
        }
        
        // Create Canvas for menu
        var canvasGO = new GameObject("MenuCanvas");
        menuCanvas = canvasGO.AddComponent<Canvas>();
        menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        menuCanvas.sortingOrder = 100; // Ensure menu appears on top
        var canvasScaler = canvasGO.AddComponent<CanvasScaler>();
        canvasScaler.referenceResolution = new Vector2(1920, 1080); // Use standard HD resolution
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f; // Balance between width and height
        canvasGO.AddComponent<GraphicRaycaster>();
        
        Debug.Log($"MenuManager: CanvasScaler configured with reference resolution: {canvasScaler.referenceResolution}");
        
        // Ensure Canvas is set up for full screen
        menuCanvas.pixelPerfect = false; // Allow scaling
        menuCanvas.planeDistance = 100f; // Distance from camera
        
        Debug.Log($"MenuManager: Canvas created with renderMode: {menuCanvas.renderMode}, sortingOrder: {menuCanvas.sortingOrder}");
        
        // Test if Canvas is visible
        Debug.Log($"MenuManager: Canvas enabled: {menuCanvas.enabled}, visible: {menuCanvas.isActiveAndEnabled}");
        
        // Ensure EventSystem exists for button clicks
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            var eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
            Debug.Log("MenuManager: Created EventSystem for button clicks");
        }
        else
        {
            Debug.Log("MenuManager: EventSystem already exists");
        }
        
        Debug.Log("MenuManager: Canvas created successfully");
        
        // Test if Canvas is in scene
        var foundCanvas = GameObject.Find("MenuCanvas");
        Debug.Log($"MenuManager: Found Canvas in scene: {foundCanvas != null}");
        
        // Create background
        var bgGO = new GameObject("MenuBackground");
        bgGO.transform.SetParent(canvasGO.transform, false);
        backgroundImage = bgGO.AddComponent<Image>();
        
        // Set a simple background color to ensure visibility
        backgroundImage.color = new Color(0.1f, 0.1f, 0.1f, 1f); // Full opacity
        
        // Load menu background sprite
        var bgSprite = ManualSpriteLoader.GetSprite("menu_background");
        if (bgSprite != null)
        {
            backgroundImage.sprite = bgSprite;
            backgroundImage.type = Image.Type.Simple; // Use Simple for full screen coverage
            Debug.Log("MenuManager: Menu background sprite loaded successfully");
        }
        else
        {
            backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            Debug.LogWarning("MenuManager: Could not load menu background sprite, using fallback color");
        }
        
        var bgRT = backgroundImage.rectTransform;
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        
        Debug.Log($"MenuManager: Background created with size: {bgRT.sizeDelta}, position: {bgRT.anchoredPosition}");
        
        // Ensure the canvas is properly set up
        canvasGO.SetActive(true);
        menuCanvas.enabled = true;
        
        // Test if background is visible
        Debug.Log($"MenuManager: Background enabled: {backgroundImage.enabled}, visible: {backgroundImage.isActiveAndEnabled}");
        
        // Create Play Button (shows levels)
        CreateButton("PlayButton", "play", new Vector2(0, 200), () => {
            Debug.Log("MenuManager: Play button clicked - calling ShowLevels()");
            ShowLevels();
        });
        
        // Create Prize Button
        CreateButton("PrizeButton", "prize", new Vector2(0, 0), () => ShowPrizes());
        
        // Create Close Button (top-right corner)
        CreateButton("CloseButton", "close_icon", new Vector2(800, 400), () => CloseGame(), new Vector2(80, 80));
        
        // Create star display
        CreateStarDisplay();
        
        // Final verification that menu was created successfully
        Debug.Log("MenuManager: Menu creation completed. Final verification...");
        var finalCanvasCheck = GameObject.Find("MenuCanvas");
        if (finalCanvasCheck != null)
        {
            Debug.Log($"MenuManager: SUCCESS - MenuCanvas found and active: {finalCanvasCheck.activeInHierarchy}");
            var finalButtons = finalCanvasCheck.GetComponentsInChildren<Button>();
            Debug.Log($"MenuManager: SUCCESS - Found {finalButtons.Length} buttons in final menu");
        }
        else
        {
            Debug.LogError("MenuManager: FAILED - MenuCanvas not found after creation!");
        }
    }
    
    void CreateButton(string buttonName, string spriteName, Vector2 position, System.Action onClick, Vector2? customSize = null)
    {
        Debug.Log($"MenuManager: Creating button '{buttonName}' with sprite '{spriteName}'");
        var buttonGO = new GameObject(buttonName);
        buttonGO.transform.SetParent(menuCanvas.transform, false);
        
        var button = buttonGO.AddComponent<Button>();
        var buttonImage = buttonGO.AddComponent<Image>();
        
        // Ensure button is properly configured
        button.interactable = true;
        button.targetGraphic = buttonImage;
        
        // Load button sprite
        var buttonSprite = ManualSpriteLoader.GetSprite(spriteName);
        if (buttonSprite != null)
        {
            buttonImage.sprite = buttonSprite;
            buttonImage.type = Image.Type.Sliced;
            Debug.Log($"MenuManager: Button sprite '{spriteName}' loaded successfully");
        }
        else
        {
            buttonImage.color = Color.red; // Make it red so we can see it clearly
            Debug.LogWarning($"MenuManager: Could not load button sprite '{spriteName}', using red fallback color");
        }
        
        var buttonRT = buttonImage.rectTransform;
        buttonRT.anchoredPosition = position;
        
        // Set button size
        if (customSize.HasValue)
        {
            buttonRT.sizeDelta = customSize.Value;
        }
        else
        {
            // Default size for main buttons
            buttonRT.sizeDelta = new Vector2(300, 120);
        }
        
        // Add text to button if it's the back button
        if (buttonName == "BackButton")
        {
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonGO.transform, false);
            var text = textGO.AddComponent<Text>();
            text.text = "BACK";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 20;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            
            var textRT = text.rectTransform;
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = Vector2.zero;
            textRT.offsetMax = Vector2.zero;
        }
        
        // Add click event with immediate debug
        button.onClick.AddListener(() => {
            Debug.Log($"MenuManager: Button '{buttonName}' clicked!");
            onClick();
        });
        
        // Add hover effect
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        button.colors = colors;
        
        Debug.Log($"MenuManager: Button '{buttonName}' created successfully at position {position}");
        
        // Test if button is visible
        Debug.Log($"MenuManager: Button '{buttonName}' enabled: {button.enabled}, interactable: {button.interactable}");
    }
    

    
    void CreateStarDisplay()
    {
        Debug.Log("MenuManager: Creating star display...");
        
        var starGO = new GameObject("StarDisplay");
        starGO.transform.SetParent(menuCanvas.transform, false);
        
        // Create star icon (larger to contain text)
        var starIconGO = new GameObject("StarIcon");
        starIconGO.transform.SetParent(starGO.transform, false);
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
        starIconRT.anchoredPosition = new Vector2(-800, 350);
        starIconRT.sizeDelta = new Vector2(80, 80); // Larger to contain text
        
        // Create star count text (inside the star)
        var starTextGO = new GameObject("StarCount");
        starTextGO.transform.SetParent(starIconGO.transform, false); // Parent to star icon
        var starText = starTextGO.AddComponent<Text>();
        
        // Get current star count
        int currentStars = PlayerPrefs.GetInt("PlayerStars", 0);
        
        starText.text = $"{currentStars}";
        starText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        starText.fontSize = 20;
        starText.color = Color.black;
        starText.alignment = TextAnchor.MiddleCenter;
        
        var starTextRT = starText.rectTransform;
        starTextRT.sizeDelta = new Vector2(80, 80); // Same size as star
        starTextRT.anchoredPosition = new Vector2(0, 0); // Center of star
        
        Debug.Log("MenuManager: Star display created successfully");
    }
    
    void ShowLevels()
    {
        Debug.Log("MenuManager: Showing levels...");
        
        // Hide main menu buttons (only if they exist)
        var playButton = GameObject.Find("PlayButton");
        var prizeButton = GameObject.Find("PrizeButton");
        var closeButton = GameObject.Find("CloseButton");
        var starDisplay = GameObject.Find("StarDisplay");
        
        Debug.Log($"MenuManager: Found PlayButton: {playButton != null}, PrizeButton: {prizeButton != null}, CloseButton: {closeButton != null}");
        
        if (playButton != null) 
        {
            playButton.SetActive(false);
            Debug.Log("MenuManager: PlayButton hidden");
        }
        
        if (prizeButton != null) 
        {
            prizeButton.SetActive(false);
            Debug.Log("MenuManager: PrizeButton hidden");
        }
        
        if (closeButton != null) 
        {
            closeButton.SetActive(false);
            Debug.Log("MenuManager: CloseButton hidden");
        }
        
        if (starDisplay != null) 
        {
            starDisplay.SetActive(false);
            Debug.Log("MenuManager: StarDisplay hidden");
        }
        
        // Create level buttons
        CreateLevelButton("Level1Button", "Lvl 1 Vowel Island", new Vector2(0, 300), () => StartLevel(1));
        CreateLevelButton("Level2Button", "Lvl 2 Word Puzzle", new Vector2(0, 100), () => StartLevel(2));
        CreateLevelButton("Level3Button", "Lvl 3 Animal Puzzle", new Vector2(0, -100), () => StartLevel(3));
        CreateLevelButton("Level4Button", "Lvl 4 Jumping Puzzle", new Vector2(0, -300), () => StartLevel(4));
        
        // Create back button to main menu
        CreateButton("BackButton", "btn", new Vector2(-800, 400), () => ShowMainMenu(), new Vector2(200, 80));
        
        Debug.Log("MenuManager: All level buttons created successfully");
    }
    
    void CreateLevelButton(string buttonName, string levelText, Vector2 position, System.Action onClick)
    {
        Debug.Log($"MenuManager: Creating level button '{buttonName}' with text '{levelText}'");
        var buttonGO = new GameObject(buttonName);
        buttonGO.transform.SetParent(menuCanvas.transform, false);
        
        var button = buttonGO.AddComponent<Button>();
        var buttonImage = buttonGO.AddComponent<Image>();
        
        // Ensure button is properly configured
        button.interactable = true;
        button.targetGraphic = buttonImage;
        
        // Use button sprite or create a colored background
        var buttonSprite = ManualSpriteLoader.GetSprite("btn");
        if (buttonSprite != null)
        {
            buttonImage.sprite = buttonSprite;
            buttonImage.type = Image.Type.Sliced;
        }
        else
        {
            buttonImage.color = new Color(0.3f, 0.6f, 1f, 0.9f);
        }
        
        var buttonRT = buttonImage.rectTransform;
        buttonRT.anchoredPosition = position;
        buttonRT.sizeDelta = new Vector2(500, 120);
        
        // Add text to button
        var textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        var text = textGO.AddComponent<Text>();
        text.text = levelText;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        var textRT = text.rectTransform;
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        
        // Add click event
        button.onClick.AddListener(() => {
            Debug.Log($"MenuManager: Level button '{buttonName}' clicked!");
            onClick();
        });
        
        // Add hover effect
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        button.colors = colors;
    }
    
    void ShowMainMenu()
    {
        Debug.Log("MenuManager: Returning to main menu...");
        
        // Hide level buttons
        var levelButtons = new string[] { "Level1Button", "Level2Button", "Level3Button", "Level4Button", "BackButton" };
        foreach (var buttonName in levelButtons)
        {
            var button = GameObject.Find(buttonName);
            if (button != null) 
            {
                button.SetActive(false);
                Debug.Log($"MenuManager: Hidden level button: {buttonName}");
            }
            else
            {
                Debug.LogWarning($"MenuManager: Could not find level button: {buttonName}");
            }
        }
        
        // Check if main menu buttons exist, if not recreate the menu
        var playButton = GameObject.Find("PlayButton");
        var prizeButton = GameObject.Find("PrizeButton");
        var closeButton = GameObject.Find("CloseButton");
        
        if (playButton == null || prizeButton == null || closeButton == null)
        {
            Debug.Log("MenuManager: Main menu buttons missing, recreating menu...");
            CreateMenuUI();
            return;
        }
        
        // Show main menu buttons
        playButton.SetActive(true);
        Debug.Log("MenuManager: Showed PlayButton");
        
        prizeButton.SetActive(true);
        Debug.Log("MenuManager: Showed PrizeButton");
        
        closeButton.SetActive(true);
        Debug.Log("MenuManager: Showed CloseButton");
        
        // Also ensure the star display is visible
        var starDisplay = GameObject.Find("StarDisplay");
        if (starDisplay != null)
        {
            starDisplay.SetActive(true);
            Debug.Log("MenuManager: Showed StarDisplay");
        }
        else
        {
            // Recreate star display if missing
            CreateStarDisplay();
        }
        
        Debug.Log("MenuManager: Main menu restoration completed");
    }
    
    void StartLevel(int levelNumber)
    {
        Debug.Log($"MenuManager: Starting level {levelNumber}...");
        
        // Hide menu
        menuCanvas.gameObject.SetActive(false);
        
        // Show level description first
        ShowLevelDescription(levelNumber);
    }
    
    void ShowLevelDescription(int levelNumber)
    {
        Debug.Log($"MenuManager: Showing description for level {levelNumber}");
        
        // Create Canvas for level description
        var canvasGO = new GameObject("LevelDescriptionCanvas");
        levelDescriptionCanvas = canvasGO.AddComponent<Canvas>();
        levelDescriptionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        levelDescriptionCanvas.sortingOrder = 200; // Higher than menu
        var canvasScaler = canvasGO.AddComponent<CanvasScaler>();
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Create background (same as main menu)
        var bgGO = new GameObject("DescriptionBackground");
        bgGO.transform.SetParent(canvasGO.transform, false);
        var bgImage = bgGO.AddComponent<Image>();
        
        // Set background color
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        
        // Load menu background sprite
        var bgSprite = ManualSpriteLoader.GetSprite("menu_background");
        if (bgSprite != null)
        {
            bgImage.sprite = bgSprite;
            bgImage.type = Image.Type.Simple;
            Debug.Log("MenuManager: Description background sprite loaded successfully");
        }
        
        var bgRT = bgImage.rectTransform;
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        
        // Create description text
        var textGO = new GameObject("DescriptionText");
        textGO.transform.SetParent(canvasGO.transform, false);
        var text = textGO.AddComponent<Text>();
        text.text = GetLevelDescription(levelNumber);
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 36;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        
        var textRT = text.rectTransform;
        textRT.anchorMin = new Vector2(0.1f, 0.45f);
        textRT.anchorMax = new Vector2(0.9f, 0.75f);
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        
        // Create OK button
        CreateDescriptionButton("OKButton", "OK", new Vector2(0, -350), () => {
            Debug.Log($"MenuManager: OK button clicked for level {levelNumber}");
            StartGameWithLevel(levelNumber);
        });
        
        // Create Back button
        CreateDescriptionButton("BackButton", "BACK", new Vector2(-400, -350), () => {
            Debug.Log($"MenuManager: Back button clicked from level description");
            ReturnToMainMenu();
        });
        
        Debug.Log($"MenuManager: Level description created for level {levelNumber}");
    }
    
    string GetLevelDescription(int levelNumber)
    {
        switch (levelNumber)
        {
            case 1:
                return "LEVEL 1: VOWEL ISLAND\n\n" +
                       "Welcome to Vowel Island! In this level, you'll help the wizard collect letters to spell animal names.\n\n" +
                       "HOW TO PLAY:\n" +
                       "• Use A/D or Arrow keys to move the wizard\n" +
                       "• Press SPACE to jump and collect letters\n" +
                       "• Collect letters in the correct order to spell the animal's name\n" +
                       "• If you collect the wrong letter, the level restarts\n" +
                       "• Complete all 3 animals to finish the level\n\n" +
                       "Good luck, young wizard!";
            case 2:
                return "LEVEL 2: WORD PUZZLE CHALLENGE\n\n" +
                       "Welcome to the Word Puzzle Challenge! Test your spelling skills with fun questions.\n\n" +
                       "HOW TO PLAY:\n" +
                       "• Read the question carefully\n" +
                       "• Click on the correctly spelled word from the three options\n" +
                       "• Correct answers give you +5 stars\n" +
                       "• Wrong answers cost you -1 star\n" +
                       "• Complete 3 puzzles to finish the level\n\n" +
                       "Can you spot the correctly spelled words?";
            case 3:
                return "LEVEL 3: ANIMAL NAME PUZZLE\n\n" +
                       "Welcome to the Animal Name Puzzle! Test your spelling skills by dragging and dropping the correct parts of animal names.\n\n" +
                       "HOW TO PLAY:\n" +
                       "• Look at the animal picture and read the question\n" +
                       "• Drag the correct two parts of the animal's name to the green drop zone\n" +
                       "• Correct answers give you +5 stars\n" +
                       "• Wrong parts cost you -1 star\n" +
                       "• Complete 3 puzzles to finish the level\n\n" +
                       "Can you assemble the animal names correctly?";
            case 4:
                return "LEVEL 4: JUMPING PUZZLE\n\n" +
                       "Welcome to the Jumping Puzzle! Test your jumping skills and letter recognition.\n\n" +
                       "HOW TO PLAY:\n" +
                       "• Use A/D or Arrow keys to move the wizard\n" +
                       "• Press SPACE to jump between rocks\n" +
                       "• Jump into letters in the correct order shown\n" +
                       "• Each correct letter gives you +5 stars\n" +
                       "• Wrong order costs you -1 star\n" +
                       "• Complete 3 sequences to finish the level\n\n" +
                       "Can you jump to the right letters in order?";
            default:
                return "LEVEL " + levelNumber + "\n\n" +
                       "Welcome to this exciting level!\n\n" +
                       "HOW TO PLAY:\n" +
                       "• Use A/D or Arrow keys to move the wizard\n" +
                       "• Press SPACE to jump and collect letters\n" +
                       "• Collect letters in the correct order to spell the animal's name\n" +
                       "• If you collect the wrong letter, the level restarts\n" +
                       "• Complete all 3 animals to finish the level\n\n" +
                       "Have fun learning!";
        }
    }
    
    void CreateDescriptionButton(string buttonName, string buttonText, Vector2 position, System.Action onClick)
    {
        Debug.Log($"MenuManager: Creating description button '{buttonName}'");
        var buttonGO = new GameObject(buttonName);
        
        // Use shop canvas if we're in shop, otherwise use level description canvas
        Transform parentCanvas = shopCanvas != null ? shopCanvas.transform : levelDescriptionCanvas.transform;
        buttonGO.transform.SetParent(parentCanvas, false);
        
        var button = buttonGO.AddComponent<Button>();
        var buttonImage = buttonGO.AddComponent<Image>();
        
        // Ensure button is properly configured
        button.interactable = true;
        button.targetGraphic = buttonImage;
        
        // Use button sprite or create a colored background
        var buttonSprite = ManualSpriteLoader.GetSprite("btn");
        if (buttonSprite != null)
        {
            buttonImage.sprite = buttonSprite;
            buttonImage.type = Image.Type.Sliced;
        }
        else
        {
            buttonImage.color = new Color(0.3f, 0.6f, 1f, 0.9f);
        }
        
        var buttonRT = buttonImage.rectTransform;
        buttonRT.anchoredPosition = position;
        buttonRT.sizeDelta = new Vector2(200, 80);
        
        // Add text to button
        var textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        var text = textGO.AddComponent<Text>();
        text.text = buttonText;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 28;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        var textRT = text.rectTransform;
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        
        // Add click event
        button.onClick.AddListener(() => {
            Debug.Log($"MenuManager: Description button '{buttonName}' clicked!");
            onClick();
        });
        
        // Add hover effect
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        button.colors = colors;
        
        Debug.Log($"MenuManager: Description button '{buttonName}' created successfully");
    }
    
    void StartGameWithLevel(int levelNumber)
    {
        Debug.Log($"MenuManager: Starting game with level {levelNumber}...");
        
        // Hide level description
        if (levelDescriptionCanvas != null)
        {
            levelDescriptionCanvas.gameObject.SetActive(false);
        }
        
        // Start the game with specific level
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            Debug.Log($"MenuManager: Found GameManager, calling InitializeGame({levelNumber})");
            gameManager.InitializeGame(levelNumber);
        }
        else
        {
            Debug.LogError("MenuManager: GameManager not found!");
        }
    }
    
    void ReturnToMainMenu()
    {
        Debug.Log("MenuManager: Returning to main menu from level description...");
        
        // Hide level description
        if (levelDescriptionCanvas != null)
        {
            levelDescriptionCanvas.gameObject.SetActive(false);
        }
        
        // Show main menu
        if (menuCanvas != null)
        {
            menuCanvas.gameObject.SetActive(true);
        }
        else
        {
            // If menu canvas was destroyed, recreate it
            CreateMenuUI();
        }
    }
    
    public void ReturnToLevelSelection()
    {
        Debug.Log("MenuManager: Returning to level selection from game completion...");
        
        // Hide any existing canvases
        if (levelDescriptionCanvas != null)
        {
            levelDescriptionCanvas.gameObject.SetActive(false);
        }
        
        // Show level selection (recreate if needed)
        if (menuCanvas != null)
        {
            menuCanvas.gameObject.SetActive(true);
            ShowLevels();
        }
        else
        {
            // If menu canvas was destroyed, recreate it and show levels
            CreateMenuUI();
            Invoke("ShowLevels", 0.1f);
        }
        
        // Update star display immediately and after a delay to ensure it updates
        UpdateStarDisplay();
        Invoke("UpdateStarDisplay", 0.2f);
        Invoke("UpdateStarDisplay", 0.5f);
    }
    
    void StartGame()
    {
        Debug.Log("MenuManager: Starting game...");
        // Hide menu
        menuCanvas.gameObject.SetActive(false);
        
        // Start the game
        var gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.InitializeGame();
        }
        else
        {
            Debug.LogError("MenuManager: GameManager not found!");
        }
    }
    
    void ShowPrizes()
    {
        Debug.Log("MenuManager: Show shop clicked");
        ShowShop();
    }
    
    void ShowShop()
    {
        Debug.Log("MenuManager: Showing shop...");
        
        // Hide main menu
        if (menuCanvas != null)
        {
            menuCanvas.gameObject.SetActive(false);
        }
        
        // Create shop canvas
        var canvasGO = new GameObject("ShopCanvas");
        shopCanvas = canvasGO.AddComponent<Canvas>();
        shopCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        shopCanvas.sortingOrder = 150; // Between menu and level description
        var canvasScaler = canvasGO.AddComponent<CanvasScaler>();
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Ensure EventSystem exists
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            var eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
        }
        
        // Create background (same as main menu)
        var bgGO = new GameObject("ShopBackground");
        bgGO.transform.SetParent(canvasGO.transform, false);
        var bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        
        var bgSprite = ManualSpriteLoader.GetSprite("menu_background");
        if (bgSprite != null)
        {
            bgImage.sprite = bgSprite;
            bgImage.type = Image.Type.Simple;
        }
        
        var bgRT = bgImage.rectTransform;
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        
        // Create title
        var titleGO = new GameObject("ShopTitle");
        titleGO.transform.SetParent(canvasGO.transform, false);
        var titleText = titleGO.AddComponent<Text>();
        titleText.text = "CHARACTER SHOP";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 48;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        
        var titleRT = titleText.rectTransform;
        titleRT.anchoredPosition = new Vector2(0, 350);
        titleRT.sizeDelta = new Vector2(600, 80);
        
        // Create character shop items
        CreateShopItems();
        
        // Create back button
        CreateDescriptionButton("ShopBackButton", "BACK", new Vector2(0, -300), () => {
            Debug.Log("MenuManager: Shop back button clicked");
            ReturnToMainMenuFromShop();
        });
        
        Debug.Log("MenuManager: Shop created successfully");
    }
    
    void CreateShopItem(string characterName, string displayName, int price, Vector2 position, bool isUnlocked)
    {
        var itemGO = new GameObject($"ShopItem_{characterName}");
        itemGO.transform.SetParent(shopCanvas.transform, false);
        // Don't use tags since they need to be predefined in Unity
        // We'll use name-based finding instead
        
        var itemImage = itemGO.AddComponent<Image>();
        itemImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        
        var itemRT = itemImage.rectTransform;
        itemRT.anchoredPosition = position;
        itemRT.sizeDelta = new Vector2(300, 200);
        
        // Create character preview
        var previewGO = new GameObject("CharacterPreview");
        previewGO.transform.SetParent(itemGO.transform, false);
        var previewImage = previewGO.AddComponent<Image>();
        
        // Try different sprite loading methods for character preview
        var characterSprite = ManualSpriteLoader.GetSprite($"{characterName}_Idle");
        if (characterSprite == null)
        {
            // Try alternative path format
            characterSprite = ManualSpriteLoader.GetSpriteByPath($"Sprites/Characters/{characterName}/Idle");
        }
        if (characterSprite == null)
        {
            // Try just the character name
            characterSprite = ManualSpriteLoader.GetSprite(characterName);
        }
        
        if (characterSprite != null)
        {
            previewImage.sprite = characterSprite;
            Debug.Log($"MenuManager: Loaded sprite for {characterName}");
        }
        else
        {
            previewImage.color = Color.gray;
            Debug.LogWarning($"MenuManager: Could not load sprite for {characterName}");
        }
        
        var previewRT = previewImage.rectTransform;
        previewRT.anchoredPosition = new Vector2(0, 30);
        previewRT.sizeDelta = new Vector2(100, 100);
        
        // Create character name
        var nameGO = new GameObject("CharacterName");
        nameGO.transform.SetParent(itemGO.transform, false);
        var nameText = nameGO.AddComponent<Text>();
        nameText.text = displayName;
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 24;
        nameText.color = Color.white;
        nameText.alignment = TextAnchor.MiddleCenter;
        
        var nameRT = nameText.rectTransform;
        nameRT.anchoredPosition = new Vector2(0, -20);
        nameRT.sizeDelta = new Vector2(280, 40);
        
        // Create price or status
        var priceGO = new GameObject("PriceText");
        priceGO.transform.SetParent(itemGO.transform, false);
        var priceText = priceGO.AddComponent<Text>();
        
        string currentCharacter = PlayerPrefs.GetString("CurrentCharacter", "Wizard");
        
        if (isUnlocked)
        {
            if (currentCharacter == characterName)
            {
                priceText.text = "✓ USING";
                priceText.color = Color.green;
            }
            else
            {
                priceText.text = "UNLOCKED";
                priceText.color = Color.blue;
            }
        }
        else
        {
            priceText.text = $"{price} ★";
            priceText.color = Color.yellow;
        }
        
        priceText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        priceText.fontSize = 20;
        priceText.alignment = TextAnchor.MiddleCenter;
        
        var priceRT = priceText.rectTransform;
        priceRT.anchoredPosition = new Vector2(0, -60);
        priceRT.sizeDelta = new Vector2(280, 40);
        
        // Add button functionality
        var button = itemGO.AddComponent<Button>();
        button.onClick.AddListener(() => {
            Debug.Log($"MenuManager: Shop item '{characterName}' clicked");
            OnShopItemClicked(characterName, price, isUnlocked);
        });
        
        // Add hover effect
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        button.colors = colors;
    }
    
    void OnShopItemClicked(string characterName, int price, bool isUnlocked)
    {
        Debug.Log($"MenuManager: Shop item '{characterName}' clicked. Unlocked: {isUnlocked}, Price: {price}");
        
        int currentStars = PlayerPrefs.GetInt("PlayerStars", 0);
        
        if (isUnlocked)
        {
            // Character is already unlocked - set as current character
            PlayerPrefs.SetString("CurrentCharacter", characterName);
            PlayerPrefs.Save();
            Debug.Log($"MenuManager: Set '{characterName}' as current character");
            
            // Show confirmation message
            ShowShopMessage($"Now using {characterName}!", Color.green);
            
            // Update shop display to show current character
            UpdateShopDisplay();
        }
        else
        {
            // Character is locked - try to purchase
            if (currentStars >= price)
            {
                // Purchase successful
                currentStars -= price;
                PlayerPrefs.SetInt("PlayerStars", currentStars);
                PlayerPrefs.Save();
                
                // Unlock the character
                PlayerPrefs.SetInt($"Unlocked_{characterName}", 1);
                PlayerPrefs.SetString("CurrentCharacter", characterName);
                PlayerPrefs.Save();
                
                Debug.Log($"MenuManager: Purchased '{characterName}' for {price} stars. Remaining stars: {currentStars}");
                
                // Show success message
                ShowShopMessage($"Purchased {characterName}!", Color.green);
                
                // Update shop display
                UpdateShopDisplay();
            }
            else
            {
                // Not enough stars
                Debug.Log($"MenuManager: Not enough stars to purchase '{characterName}'. Need: {price}, Have: {currentStars}");
                ShowShopMessage("Not enough stars!", Color.red);
            }
        }
    }
    
    void ShowShopMessage(string message, Color color)
    {
        // Create a temporary message display
        var messageGO = new GameObject("ShopMessage");
        messageGO.transform.SetParent(shopCanvas.transform, false);
        
        var messageImage = messageGO.AddComponent<Image>();
        messageImage.color = new Color(0, 0, 0, 0.8f);
        
        var messageRT = messageImage.rectTransform;
        messageRT.anchorMin = new Vector2(0.2f, 0.4f);
        messageRT.anchorMax = new Vector2(0.8f, 0.6f);
        messageRT.offsetMin = Vector2.zero;
        messageRT.offsetMax = Vector2.zero;
        
        var messageTextGO = new GameObject("MessageText");
        messageTextGO.transform.SetParent(messageGO.transform, false);
        var messageText = messageTextGO.AddComponent<Text>();
        messageText.text = message;
        messageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        messageText.fontSize = 32;
        messageText.color = color;
        messageText.alignment = TextAnchor.MiddleCenter;
        
        var messageTextRT = messageText.rectTransform;
        messageTextRT.anchorMin = Vector2.zero;
        messageTextRT.anchorMax = Vector2.one;
        messageTextRT.offsetMin = Vector2.zero;
        messageTextRT.offsetMax = Vector2.zero;
        
        // Destroy message after 3 seconds
        Destroy(messageGO, 3f);
    }
    
    void UpdateShopDisplay()
    {
        // Destroy current shop items and recreate them using name-based finding
        var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        var shopItemsToDestroy = new List<GameObject>();
        
        foreach (var obj in allObjects)
        {
            if (obj != null && obj.name.StartsWith("ShopItem_"))
            {
                shopItemsToDestroy.Add(obj);
            }
        }
        
        foreach (var item in shopItemsToDestroy)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        
        // Recreate shop items with updated status
        CreateShopItems();
        
        // Update star display
        UpdateStarDisplay();
    }
    
    void CreateShopItems()
    {
        // Define available characters
        var characters = new (string name, string displayName, int price)[]
        {
            ("Wizard", "Wizard", 0), // Free starter character
            ("Swordsman", "Swordsman", 20),
            ("Archer", "Archer", 30)
        };
        
        // Create shop items
        for (int i = 0; i < characters.Length; i++)
        {
            var character = characters[i];
            bool isUnlocked = PlayerPrefs.GetInt($"Unlocked_{character.name}", character.name == "Wizard" ? 1 : 0) == 1;
            Vector2 position = new Vector2(-400 + (i * 400), 0);
            
            CreateShopItem(character.name, character.displayName, character.price, position, isUnlocked);
        }
    }
    
    void ReturnToMainMenuFromShop()
    {
        Debug.Log("MenuManager: Returning to main menu from shop...");
        
        // Hide shop
        if (shopCanvas != null)
        {
            shopCanvas.gameObject.SetActive(false);
        }
        
        // Show main menu
        if (menuCanvas != null)
        {
            menuCanvas.gameObject.SetActive(true);
        }
        else
        {
            CreateMenuUI();
        }
    }
    
    void CloseGame()
    {
        Debug.Log("MenuManager: Closing game...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 