using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

// GameManager for handling game initialization and level management
public class GameManager : MonoBehaviour
{
    public List<string> animalNames = new List<string>{"cat","dog","peng"};
    int currentAnimal = 0, score = 0;
    Image animalUI;
    Transform letterPanel;
    Text scoreText;
    GameObject successText;
    List<string> collectedLetters = new List<string>();
    private int currentLevel = 1;
    private int currentStars = 0;
    private int currentRetries = 0;

    void Start()
    {
        // Don't start automatically - wait for menu to call InitializeGame()
        Debug.Log("GameManager: Ready but waiting for menu to start game...");
    }

    public void InitializeGame(int levelNumber = 1)
    {
        Debug.Log($"GameManager: Initializing game for level {levelNumber}");
        currentLevel = levelNumber;
        
        // Always perform complete cleanup before starting any level
        CompleteLevelCleanup();
        
        // Setup camera and audio system
        SetupCamera();
        SetupAudioSystem();
        
                   // Check if this is Level 2 (word puzzle level)
           if (levelNumber == 2)
           {
               Debug.Log("GameManager: Level 2 detected, creating Level2Manager...");
               
               // Create new Level2Manager
               var level2GO = new GameObject("Level2Manager");
               level2GO.AddComponent<Level2Manager>();
               return; // Don't proceed with normal game setup
           }
           
           // Check if this is Level 3 (animal name puzzle level)
           if (levelNumber == 3)
           {
               Debug.Log("GameManager: Level 3 detected, creating Level3Manager...");
               
               // Create new Level3Manager
               var level3GO = new GameObject("Level3Manager");
               level3GO.AddComponent<Level3Manager>();
               return; // Don't proceed with normal game setup
           }
           
           // Check if this is Level 4 (jumping puzzle level)
           if (levelNumber == 4)
           {
               Debug.Log("GameManager: Level 4 detected, creating Level4Manager...");
               
               // Create new Level4Manager
               var level4GO = new GameObject("Level4Manager");
               var level4Manager = level4GO.AddComponent<Level4Manager>();
               Debug.Log($"GameManager: Level4Manager created: {level4Manager != null}");
               return; // Don't proceed with normal game setup
           }
        
        // Reset game state for new level
        ResetGameState();
        
        // Load saved stars
        currentStars = PlayerPrefs.GetInt("PlayerStars", 0);
        Debug.Log($"GameManager: Loaded {currentStars} stars from save");
        
        // Create GameInitializer first (this sets up camera, background, ground, UI)
        var initializerGO = new GameObject("GameInitializer");
        initializerGO.AddComponent<GameInitializer>();
        
        // Wait a frame for GameInitializer to complete setup
        Invoke("SetupGameAfterInitializer", 0.1f);
    }
    
    void ResetGameState()
    {
        currentAnimal = 0;
        score = 0;
        currentRetries = 0;
        collectedLetters.Clear();
        Debug.Log("GameManager: Game state reset for new level");
    }
    
    void SetupGameAfterInitializer()
    {
        Debug.Log("GameManager: Setting up game after initializer...");
        
        // find UI refs with error checking
        var animalDisplayGO = GameObject.Find("AnimalDisplay");
        if (animalDisplayGO != null)
        {
            animalUI = animalDisplayGO.GetComponent<Image>();
            Debug.Log("GameManager: Found AnimalDisplay");
        }
        else
        {
            Debug.LogError("GameManager: AnimalDisplay not found!");
        }
        
        var textFieldGO = GameObject.Find("TextField");
        if (textFieldGO != null)
        {
            letterPanel = textFieldGO.transform;
            Debug.Log("GameManager: Found TextField");
        }
        else
        {
            Debug.LogError("GameManager: TextField not found!");
        }
        
        var scoreTextGO = GameObject.Find("ScoreText");
        if (scoreTextGO != null)
        {
            scoreText = scoreTextGO.GetComponent<Text>();
            Debug.Log("GameManager: Found ScoreText");
        }
        else
        {
            Debug.LogError("GameManager: ScoreText not found!");
        }
        
        successText = GameObject.Find("SuccessText");
        if (successText != null)
        {
            Debug.Log("GameManager: Found SuccessText");
        }
        else
        {
            Debug.Log("GameManager: SuccessText not found (this is normal - created dynamically)");
        }

        // Only proceed if we have the essential UI elements
        if (animalUI != null && letterPanel != null && scoreText != null)
        {
            Debug.Log("GameManager: All UI elements found, starting game...");
            ShowNextAnimal();
            SpawnWizard();
            SpawnLetters();
        }
        else
        {
            Debug.LogError("GameManager: Missing essential UI elements, cannot start game!");
        }
    }

    void ShowNextAnimal()
    {
        Debug.Log($"GameManager: ShowNextAnimal called. currentAnimal: {currentAnimal}, animalNames.Count: {animalNames.Count}");
        
        if (currentAnimal < animalNames.Count)
        {
            var name = animalNames[currentAnimal];
            Debug.Log($"GameManager: Showing animal '{name}'");
            
            var animalSprite = ManualSpriteLoader.GetSprite(name);
            if (animalSprite == null)
            {
                Debug.LogWarning($"GameManager: Could not find sprite for '{name}', trying alternative method");
                // Try alternative loading method
                animalSprite = ManualSpriteLoader.GetSpriteByPath($"Sprites/Characters/animals/{name}");
            }
            
            if (animalSprite != null)
            {
                if (animalUI != null)
                {
                    animalUI.sprite = animalSprite;
                    animalUI.color = Color.white; // Ensure full opacity
                    Debug.Log($"GameManager: Successfully set animal sprite for '{name}' with full opacity");
                }
                else
                {
                    Debug.LogError("GameManager: animalUI is null, cannot set sprite!");
                }
            }
            else
            {
                Debug.LogError($"GameManager: Could not load sprite for animal '{name}'");
            }
        }
        else
        {
            Debug.LogWarning($"GameManager: currentAnimal ({currentAnimal}) >= animalNames.Count ({animalNames.Count})");
        }
    }

    void SpawnWizard()
    {
        var wizGO = new GameObject("Wizard");
        var sr = wizGO.AddComponent<SpriteRenderer>();
        
        // Get the selected character
        string selectedCharacter = PlayerPrefs.GetString("CurrentCharacter", "Wizard");
        Debug.Log($"GameManager: Spawning character: {selectedCharacter}");
        
        // Try to load the character sprite
        var characterSprite = ManualSpriteLoader.GetSprite($"{selectedCharacter}_Idle");
        if (characterSprite == null)
        {
            // Try alternative path format
            characterSprite = ManualSpriteLoader.GetSpriteByPath($"Sprites/Characters/{selectedCharacter}/Idle");
        }
        if (characterSprite == null)
        {
            // Fallback to Wizard
            characterSprite = ManualSpriteLoader.GetSpriteByPath("Sprites/Characters/Wizard/Idle");
            Debug.LogWarning($"GameManager: Could not load {selectedCharacter} sprite, using Wizard as fallback");
        }
        
        if (characterSprite != null)
        {
            sr.sprite = characterSprite;
            float charHeight = sr.bounds.size.y;
            wizGO.transform.localScale = Vector3.one * (GameData.characterSize / charHeight); // make proper size
            Debug.Log($"GameManager: Successfully loaded {selectedCharacter} sprite");
        }
        
        wizGO.transform.position = new Vector3(0, GameData.characterY, 0);
        
        // Add components
        wizGO.AddComponent<PlayerController>();
        
        // Add Rigidbody2D if it doesn't exist
        var rb = wizGO.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = wizGO.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 1f;
        
        // Add BoxCollider2D if it doesn't exist
        var collider = wizGO.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            wizGO.AddComponent<BoxCollider2D>();
        }
    }

    void SpawnLetters()
    {
        Debug.Log("GameManager: Spawning letters...");
        
        // Spawn letters for the current animal's name
        if (currentAnimal < animalNames.Count)
        {
            string animalName = animalNames[currentAnimal];
            List<char> lettersToSpawn = animalName.ToCharArray().ToList();
            
            Debug.Log($"GameManager: Animal '{animalName}' letters: {string.Join(", ", lettersToSpawn)}");
            
            // Also spawn some random letters to make total of 10
            // Make sure animal letters are included in the floating letters
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            int lettersNeeded = 10 - lettersToSpawn.Count;
            for (int i = 0; i < lettersNeeded; i++)
            {
                char randomLetter = alphabet[Random.Range(0, alphabet.Length)];
                if (!lettersToSpawn.Contains(randomLetter))
                {
                    lettersToSpawn.Add(randomLetter);
                }
            }
            
            // Shuffle the letters
            lettersToSpawn = lettersToSpawn.OrderBy(x => Random.value).ToList();
            
            Debug.Log($"GameManager: Final letter list: {string.Join(", ", lettersToSpawn)}");
            
            // Calculate proper spacing to prevent overlapping
            float letterWidth = GameData.characterSize; // Approximate letter width
            float totalWidth = letterWidth * lettersToSpawn.Count;
            float availableWidth = GameData.worldWidth * 0.8f; // Use 80% of screen width
            float spacing = (availableWidth - totalWidth) / (lettersToSpawn.Count + 1);
            
            Debug.Log($"GameManager: Spawning {lettersToSpawn.Count} letters for animal '{animalName}'");
            
            for (int i = 0; i < lettersToSpawn.Count; i++)
            {
                char letter = lettersToSpawn[i];
                var letterSprite = ManualSpriteLoader.GetSprite(letter.ToString());
                
                                if (letterSprite != null)
                {
                    var go = new GameObject("Letter_" + letter);
                    var sr = go.AddComponent<SpriteRenderer>();
                    sr.sprite = letterSprite;
                    
                    // Scale letters to be the same size as character
                    float letterScale = GameData.characterSize / letterSprite.bounds.size.y;
                    go.transform.localScale = Vector3.one * letterScale;
                    
                    // Position letters with proper spacing
                    float x = -availableWidth/2 + spacing + (letterWidth + spacing) * i;
                    go.transform.position = new Vector3(x, GameData.letterY, 0);
                    
                    // Add collider and pickup script
                    go.AddComponent<BoxCollider2D>().isTrigger = true;
                    var pickup = go.AddComponent<LetterPickup>();
                    pickup.letter = letter.ToString();
                    
                    Debug.Log($"GameManager: Spawned letter '{letter}' at position ({x}, {go.transform.position.y}) with scale {letterScale}");
                }
                else
                {
                    Debug.LogWarning($"GameManager: Could not find sprite for letter '{letter}'");
                }
            }
        }
    }

    public void OnLetterCollected(string letter)
    {
        // Add to collected letters
        if (!collectedLetters.Contains(letter))
        {
            collectedLetters.Add(letter);
            
            // Display in black text field
            var txtGO = new GameObject("Txt_" + letter);
            txtGO.transform.SetParent(letterPanel, false);
            var txt = txtGO.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.text = letter.ToUpper();
            txt.fontSize = 32;
            txt.color = Color.white; // White text on black background
            txt.alignment = TextAnchor.MiddleCenter;
            
            // Position the text in the black field
            var rt = txt.rectTransform;
            rt.sizeDelta = new Vector2(40, 40);
            int letterIndex = collectedLetters.Count - 1;
            rt.anchoredPosition = new Vector2(-180 + (letterIndex * 50), 0);
            
            // Check if this letter is part of the current animal's name
            if (currentAnimal < animalNames.Count && animalNames[currentAnimal].Contains(letter))
            {
                // Check if we have all letters for the current animal in correct order
                string currentAnimalName = animalNames[currentAnimal];
                bool hasAllLetters = true;
                
                // Check if we have all the letters needed
                foreach (char c in currentAnimalName)
                {
                    if (!collectedLetters.Contains(c.ToString()))
                    {
                        hasAllLetters = false;
                        break;
                    }
                }
                
                // If we have all letters, check if they're in correct order
                if (hasAllLetters)
                {
                    string collectedString = string.Join("", collectedLetters);
                    if (collectedString.Contains(currentAnimalName))
                    {
                        // Letters are in correct order!
                        score++;
                        currentAnimal++;
                        UpdateScore();
                        
                        if (currentAnimal >= animalNames.Count)
                        {
                            // Success!
                            ShowSuccess();
                        }
                        else
                        {
                            ShowNextAnimal();
                            // Clear collected letters for next animal
                            collectedLetters.Clear();
                            ClearLetterPanel();
                            SpawnLetters();
                        }
                    }
                    else
                    {
                        // Have all letters but wrong order - restart
                        Debug.Log($"Have all letters but wrong order! Restarting animal '{currentAnimalName}'");
                        collectedLetters.Clear();
                        ClearLetterPanel();
                        ChangeExistingLetters();
                    }
                }
            }
            else
            {
                // Wrong letter collected! Restart current animal
                Debug.Log($"Wrong letter '{letter}' collected! Restarting animal '{animalNames[currentAnimal]}'");
                
                // Add retry for currency system
                currentRetries++;
                Debug.Log($"GameManager: Retry added. Total retries: {currentRetries}");
                
                // Clear collected letters
                collectedLetters.Clear();
                ClearLetterPanel();
                
                // Move player back to ground
                var player = GameObject.Find("Wizard");
                if (player != null)
                {
                    player.transform.position = new Vector3(player.transform.position.x, GameData.characterY, 0);
                    // Reset player velocity
                    var rb = player.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector2.zero;
                    }
                }
                
                // Change existing letters (respawn with new random letters)
                ChangeExistingLetters();
            }
        }
    }
    
    void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}/{animalNames.Count}";
        }
    }
    
    void ClearLetterPanel()
    {
        // Remove all letter texts from the panel
        for (int i = letterPanel.childCount - 1; i >= 0; i--)
        {
            if (letterPanel.GetChild(i).name.StartsWith("Txt_"))
            {
                Destroy(letterPanel.GetChild(i).gameObject);
            }
        }
    }
    
    void ShowSuccess()
    {
        Debug.Log($"GameManager: Level {currentLevel} completed! Showing success message...");
        
        // Create a level complete canvas
        var canvasGO = new GameObject("LevelCompleteCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 300; // Higher than game UI
        var canvasScaler = canvasGO.AddComponent<CanvasScaler>();
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Create background
        var bgGO = new GameObject("CompleteBackground");
        bgGO.transform.SetParent(canvasGO.transform, false);
        var bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black
        
        var bgRT = bgImage.rectTransform;
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        
        // Create success text
        var textGO = new GameObject("LevelCompleteText");
        textGO.transform.SetParent(canvasGO.transform, false);
        var text = textGO.AddComponent<Text>();
        
        // Get current star count for display
        // currentStars is already available as a class field
        
        text.text = $"LEVEL {currentLevel} COMPLETE!\n\nCongratulations!\nYou've completed all 3 animals!\n\nCurrent Stars: {currentStars}\n\nReturning to level selection in 2 seconds...";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 48;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        
        var textRT = text.rectTransform;
        textRT.anchorMin = new Vector2(0.1f, 0.3f);
        textRT.anchorMax = new Vector2(0.9f, 0.7f);
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        
        // Award stars for completing the level
        int baseReward = 15;
        int penalty = currentRetries * 5;
        int calculatedReward = baseReward - penalty;
        int reward = Mathf.Max(5, calculatedReward); // Minimum 5 stars
        currentStars += reward;
        
        // Save stars
        PlayerPrefs.SetInt("PlayerStars", currentStars);
        PlayerPrefs.Save();
        
        Debug.Log($"GameManager: Level completed! Reward: {reward} stars (Base: {baseReward}, Retries: {currentRetries}, Penalty: {penalty}, Min: 5). Total stars: {currentStars}");
        
        // Force update star display
        UpdateStarDisplay();
        
        // Wait 2 seconds then return to level selection
        Invoke("ReturnToLevelSelection", 2f);
        
        Debug.Log("GameManager: Level complete message displayed, will return to level selection in 2 seconds");
    }
    
    void ReturnToLevelSelection()
    {
        Debug.Log("GameManager: Returning to level selection...");
        
        // Clean up game objects
        CleanupGameObjects();
        
        // Find MenuManager and return to level selection
        var menuManager = FindFirstObjectByType<MenuManager>();
        if (menuManager != null)
        {
            menuManager.ReturnToLevelSelection();
        }
        else
        {
            Debug.LogError("GameManager: MenuManager not found! Cannot return to level selection.");
        }
    }
    
    void CleanupGameObjects()
    {
        Debug.Log("GameManager: Cleaning up game objects...");
        
        // Destroy game-specific objects
        var objectsToDestroy = new string[] { "Wizard", "GameInitializer", "LevelCompleteCanvas" };
        foreach (var objName in objectsToDestroy)
        {
            var obj = GameObject.Find(objName);
            if (obj != null)
            {
                DestroyImmediate(obj);
                Debug.Log($"GameManager: Destroyed {objName}");
            }
        }
        
        // Destroy all letter objects - use a safer approach
        DestroyLetterObjects();
        
        // Clear UI references to force re-initialization
        animalUI = null;
        letterPanel = null;
        scoreText = null;
        successText = null;
        
        Debug.Log("GameManager: Game objects cleaned up and UI references cleared");
    }
    
    void DestroyLetterObjects()
    {
        // Find all GameObjects first, then destroy them
        var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        var lettersToDestroy = new List<GameObject>();
        
        // Collect letter objects first
        foreach (var obj in allObjects)
        {
            if (obj != null && obj.name.StartsWith("Letter_"))
            {
                lettersToDestroy.Add(obj);
            }
        }
        
        // Now destroy them
        foreach (var letter in lettersToDestroy)
        {
            if (letter != null)
            {
                Debug.Log($"GameManager: Destroying letter {letter.name}");
                Destroy(letter); // Use Destroy instead of DestroyImmediate
            }
        }
        
        Debug.Log($"GameManager: Destroyed {lettersToDestroy.Count} letter objects");
    }
    
    void UpdateStarDisplay()
    {
        // Find and update the star display
        var starDisplayUpdater = FindFirstObjectByType<StarDisplayUpdater>();
        if (starDisplayUpdater != null)
        {
            // Force the updater to refresh
            var starCountGO = GameObject.Find("StarCount");
            if (starCountGO != null)
            {
                var starText = starCountGO.GetComponent<Text>();
                if (starText != null)
                {
                    int currentStars = PlayerPrefs.GetInt("PlayerStars", 0);
                    starText.text = $"{currentStars}";
                    Debug.Log($"GameManager: Updated star display to {currentStars} stars");
                }
            }
        }
        else
        {
            Debug.LogWarning("GameManager: StarDisplayUpdater not found");
        }
        
        // Also update the menu star display if we're returning to menu
        var menuManager = FindFirstObjectByType<MenuManager>();
        if (menuManager != null)
        {
            menuManager.UpdateStarDisplay();
        }
    }
    
    void ChangeExistingLetters()
    {
        Debug.Log("GameManager: Changing existing letters...");
        
        // Use the same safe approach as DestroyLetterObjects
        var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        var lettersToDestroy = new List<GameObject>();
        
        // Collect letter objects first
        foreach (var obj in allObjects)
        {
            if (obj != null && obj.name.StartsWith("Letter_"))
            {
                lettersToDestroy.Add(obj);
            }
        }
        
        // Now destroy them
        foreach (var letter in lettersToDestroy)
        {
            if (letter != null)
            {
                Debug.Log($"GameManager: Destroying letter {letter.name} for respawn");
                Destroy(letter); // Use Destroy instead of DestroyImmediate during physics callbacks
            }
        }
        
        Debug.Log($"GameManager: Destroyed {lettersToDestroy.Count} letter objects for respawn");
        
        // Spawn new letters for the same animal after a short delay to ensure destruction is complete
        Invoke("SpawnLetters", 0.1f);
    }
    
    void CompleteLevelCleanup()
    {
        Debug.Log("GameManager: Performing complete cleanup for all levels...");
        
                   // Destroy any existing Level2Manager
           var existingLevel2 = FindFirstObjectByType<Level2Manager>();
           if (existingLevel2 != null)
           {
               Debug.Log("GameManager: Destroying existing Level2Manager...");
               DestroyImmediate(existingLevel2.gameObject);
           }
           
           // Destroy any existing Level3Manager
           var existingLevel3 = FindFirstObjectByType<Level3Manager>();
           if (existingLevel3 != null)
           {
               Debug.Log("GameManager: Destroying existing Level3Manager...");
               DestroyImmediate(existingLevel3.gameObject);
           }
           
           // Destroy any existing Level4Manager
           var existingLevel4 = FindFirstObjectByType<Level4Manager>();
           if (existingLevel4 != null)
           {
               Debug.Log("GameManager: Destroying existing Level4Manager...");
               DestroyImmediate(existingLevel4.gameObject);
           }
        
        // Destroy any existing GameInitializer
        var existingInitializer = FindFirstObjectByType<GameInitializer>();
        if (existingInitializer != null)
        {
            Debug.Log("GameManager: Destroying existing GameInitializer...");
            DestroyImmediate(existingInitializer.gameObject);
        }
        
        // Destroy all ghost objects (Level 2)
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        List<GameObject> objectsToDestroy = new List<GameObject>();
        
                   // Collect objects to destroy first
           foreach (GameObject obj in allObjects)
           {
               if (obj != null && (obj.name.Contains("GhostAnswer") || 
                   obj.GetComponent<GhostMovement>() != null ||
                   obj.name.Contains("AnswerTextBox") ||
                   (obj.name.Contains("Text") && obj.transform.parent != null && obj.transform.parent.name.Contains("GhostAnswer")) ||
                   obj.name.Contains("NamePart_") ||
                   obj.GetComponent<DraggablePart>() != null ||
                   obj.name.Contains("AnimalDisplay") ||
                   obj.name.Contains("DropZone") ||
                   obj.name.Contains("Rock_") ||
                   obj.name.Contains("Letter_") ||
                   obj.GetComponent<Level4Letter>() != null))
               {
                   objectsToDestroy.Add(obj);
               }
           }
        
        // Destroy collected objects
        foreach (GameObject obj in objectsToDestroy)
        {
            if (obj != null)
            {
                Debug.Log($"GameManager: Destroying Level 2 object: {obj.name}");
                DestroyImmediate(obj);
            }
        }
        
                   // Collect and destroy UI elements related to Level 2 and Level 3
           GameObject[] uiObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
           List<GameObject> uiToDestroy = new List<GameObject>();
           
           foreach (GameObject obj in uiObjects)
           {
               if (obj != null && (obj.name.Contains("GameCanvas") || 
                   obj.name.Contains("QuestionText") || 
                   obj.name.Contains("ScoreText") || 
                   obj.name.Contains("StarsText") || 
                   obj.name.Contains("SuccessText") || 
                   obj.name.Contains("FailureText") || 
                   obj.name.Contains("ResultText") ||
                   obj.name.Contains("ReturnButton") ||
                   obj.name.Contains("InstructionText") ||
                   obj.name.Contains("ScoreText") ||
                   obj.name.Contains("StarsText")))
               {
                   uiToDestroy.Add(obj);
               }
           }
        
        foreach (GameObject obj in uiToDestroy)
        {
            if (obj != null)
            {
                Debug.Log($"GameManager: Destroying Level 2 UI: {obj.name}");
                DestroyImmediate(obj);
            }
        }
        
        // Collect and destroy Level 1 objects
        GameObject[] level1Objects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        List<GameObject> level1ToDestroy = new List<GameObject>();
        
        foreach (GameObject obj in level1Objects)
        {
            if (obj != null && (obj.name.StartsWith("Letter_") ||
                obj.name.Contains("Wizard") ||
                obj.name.Contains("Animal") ||
                obj.name.Contains("Background") ||
                obj.name.Contains("Ground") ||
                obj.name.Contains("Platform") ||
                obj.name.Contains("AnimalDisplay") ||
                obj.name.Contains("TextField") ||
                obj.name.Contains("ScoreText") ||
                obj.name.Contains("SuccessText")))
            {
                level1ToDestroy.Add(obj);
            }
        }
        
        foreach (GameObject obj in level1ToDestroy)
        {
            if (obj != null)
            {
                Debug.Log($"GameManager: Destroying Level 1 object: {obj.name}");
                DestroyImmediate(obj);
            }
        }
        
        // Clean up any remaining game objects
        CleanupGameObjects();
        
        Debug.Log("GameManager: Complete level cleanup finished");
    }

    void SetupCamera()
    {
        // Find or create main camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject cameraGO = new GameObject("Main Camera");
            mainCamera = cameraGO.AddComponent<Camera>();
            cameraGO.tag = "MainCamera";
        }
        
        // Ensure AudioListener is present
        AudioListener audioListener = mainCamera.GetComponent<AudioListener>();
        if (audioListener == null)
        {
            audioListener = mainCamera.gameObject.AddComponent<AudioListener>();
            Debug.Log("GameManager: Added AudioListener to main camera");
        }
        
        // Set camera position and size for 2D game
        mainCamera.transform.position = new Vector3(0, 0, -10);
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 5f;
        
        Debug.Log("GameManager: Camera setup completed");
    }
    
    void SetupAudioSystem()
    {
        // Create AudioManager if it doesn't exist
        if (FindFirstObjectByType<AudioManager>() == null)
        {
            GameObject audioManagerGO = new GameObject("AudioManager");
            audioManagerGO.AddComponent<AudioManager>();
            Debug.Log("GameManager: Created AudioManager");
        }
    }
} 