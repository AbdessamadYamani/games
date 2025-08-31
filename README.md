# Wizard Letter Collection Mini Game

## Setup Instructions

### **Fully Automatic Setup (No Unity GUI Required!)**

1. **Open Unity and load the project**
2. **Open the SampleScene** (Assets/Scenes/SampleScene.unity)
3. **Press Play** - Everything is created automatically!

### **How It Works:**

The game uses Unity's `[RuntimeInitializeOnLoadMethod]` attribute to automatically:
- ✅ Clear the scene
- ✅ Create all necessary GameObjects
- ✅ Set up the camera, background, and ground
- ✅ Spawn the wizard character
- ✅ Create the UI system
- ✅ Start the game logic

### **Available Auto-Setup Scripts:**

- **`MasterInitializer`** - Complete automatic setup (recommended)
- **`SceneInitializer`** - Alternative automatic setup (disabled by default)
- **`AutoGameSetup`** - Simple automatic launcher creation (disabled by default)
- **`Bootstrap`** - Alternative automatic initialization (disabled by default)

**Note:** Only `MasterInitializer` is active to prevent multiple initializations. All scripts run automatically when you press Play - no manual GameObject creation needed!

## Game Controls

- **A/D or Arrow Keys** - Move left/right
- **Space** - Jump
- **Goal** - Collect letters to spell the animal names shown at the top

## How It Works

The game automatically creates:
- Camera with orthographic view
- Desert background
- Brown ground (15% of screen height)
- Wizard character that can walk and jump
- Animal display at the top
- Letter collection panel at the bottom
- Score display
- Success message when completed

## Game Flow

1. An animal appears at the top of the screen
2. Letters spawn in the air above the ground
3. Control the wizard to jump and collect letters
4. When you collect all letters for the animal's name, you get a point
5. Complete 3 animals to win!

## Scripts Overview

- **GameLauncher.cs** - Main entry point, creates all other components
- **GameInitializer.cs** - Sets up camera, background, ground, and UI
- **GameManager.cs** - Handles game logic, spawning, and scoring
- **PlayerController.cs** - Controls wizard movement and jumping
- **LetterPickup.cs** - Handles letter collection
- **SpriteLoader.cs** - Manages sprite loading and caching

## File Structure

The game expects sprites in these locations:
- Background: `Assets/Sprites/background/desert.png`
- Wizard: `Assets/Sprites/Characters/Wizard/Idle.png`
- Animals: `Assets/Sprites/Characters/animals/` (cat.png, dog.png, peng.png, etc.)
- Letters: `Assets/Sprites/Letters/Glass_blue_alphabetics/` (a.png, b.png, c.png, etc.)

## Troubleshooting

### Camera Issues ("Display 1 no camera rendering")
If you see "Display 1 no camera rendering", try these steps:

1. **Quick Fix**: Create an empty GameObject and attach the `EmergencyCameraFix` script to it
2. **Manual Fix**: 
   - Delete any existing cameras in the scene
   - Create a new GameObject named "MainCamera"
   - Add a Camera component to it
   - Set the tag to "MainCamera"
   - Position it at (0, 0, -10)
   - Set it to Orthographic with size 5

### Sprite Loading Issues
If sprites don't load:
1. Make sure all sprite files are in the correct folders
2. Check that sprite import settings are correct in Unity
3. Verify the sprite names match what the code expects
4. Use the `SpriteTest` script to debug sprite loading

### Debug Setup
For troubleshooting, use Method 3 (Debug Setup) which includes:
- Camera debugging
- Sprite loading tests
- Console logging for all setup steps

The game includes fallback mechanisms - if sprites can't be loaded, it will use colored rectangles instead. 