using UnityEngine;

public static class GameData
{
    public static float worldHeight = 10f;
    public static float worldWidth = 20f; // Increased to ensure full screen coverage
    public static float groundHeight = 2f;
    
    // Character and letter sizes
    public static float characterSize = 0.8f; // Smaller character size
    public static float letterSize = 0.8f;    // Same size as character
    
    // Positioning constants (World coordinates)
    public static float characterY = -3f; // Character walks at this Y level
    public static float letterY = 0.1f;   // Letters at Y 0.1
    public static float groundY = -5f;    // Ground at Y -5
    
    // UI Positioning constants (Canvas coordinates - converted from world coordinates)
    public static float animalX = 0.08f;  // Animal X position (Canvas)
    public static float animalY = 200f;   // Animal Y position (Canvas - converted from 4.19 world)
    public static float textFieldY = 120f; // Black text field Y position (Canvas - converted from 2.49 world)
} 