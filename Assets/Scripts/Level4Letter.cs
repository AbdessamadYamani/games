using UnityEngine;

public class Level4Letter : MonoBehaviour
{
    private Level4Manager level4Manager;
    private int letterIndex;
    private string letter;
    private bool isCollected = false;
    
    void Start()
    {
        // Get the letter from the sprite name or set it directly
        var letterSR = GetComponent<SpriteRenderer>();
        if (letterSR != null && letterSR.sprite != null)
        {
            letter = letterSR.sprite.name.Split('_')[0].ToUpper();
        }
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log($"Level4Letter: Collision with {col.gameObject.name}, isCollected: {isCollected}, letter: '{letter}', index: {letterIndex}");
        
        if (col.gameObject.GetComponent<PlayerController>() != null && !isCollected)
        {
            isCollected = true;
            
            // Audio disabled for the entire game
            Debug.Log($"Level4Letter: Letter '{letter}' collected (audio disabled)");
            
            // Notify Level4Manager
            if (level4Manager != null)
            {
                Debug.Log($"Level4Letter: Notifying Level4Manager about letter '{letter}' at index {letterIndex}");
                level4Manager.OnLetterCollected(letterIndex);
            }
            else
            {
                Debug.LogWarning("Level4Letter: Level4Manager is null!");
            }
            
            // Visual feedback - make letter disappear
            var letterSR = GetComponent<SpriteRenderer>();
            if (letterSR != null)
            {
                letterSR.color = Color.green;
            }
            
            // Disable the collider to prevent multiple triggers
            var letterCollider = GetComponent<BoxCollider2D>();
            if (letterCollider != null)
            {
                letterCollider.enabled = false;
                Debug.Log($"Level4Letter: Disabled collider for letter '{letter}'");
            }
            
            Debug.Log($"Level4Letter: Collected letter '{letter}' at index {letterIndex}");
        }
        else
        {
            Debug.Log($"Level4Letter: Collision ignored - PlayerController: {col.gameObject.GetComponent<PlayerController>() != null}, isCollected: {isCollected}");
        }
    }
    
    public void SetLevel4Manager(Level4Manager manager)
    {
        level4Manager = manager;
        Debug.Log($"Level4Letter: Level4Manager set to {manager}");
    }
    
    public void SetLetterIndex(int index)
    {
        letterIndex = index;
        Debug.Log($"Level4Letter: Letter index set to {index}");
    }
    
    public void SetLetter(string letterValue)
    {
        letter = letterValue;
        Debug.Log($"Level4Letter: Letter set to '{letterValue}'");
    }
} 