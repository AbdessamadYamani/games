using UnityEngine;

public class LetterPickup : MonoBehaviour
{
    public string letter;
    private AudioSource audioSource;
    
    void Start()
    {
        // Add AudioSource component for fallback sounds
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.8f;
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<PlayerController>() != null)
        {
            // Play letter pronunciation audio
            PlayLetterSound();
            
            // Notify GameManager about letter collection
            FindFirstObjectByType<GameManager>().OnLetterCollected(letter);
            Destroy(gameObject);
        }
    }
    
    void PlayLetterSound()
    {
        // Audio disabled for the entire game
        Debug.Log($"LetterPickup: Audio disabled - would have played sound for letter '{letter.ToUpper()}'");
    }
    
    void PlayFallbackSound()
    {
        // Generate more realistic letter pronunciation sounds
        string upperLetter = letter.ToUpper();
        char letterChar = upperLetter[0];
        
        // Different sound types for vowels vs consonants
        if (IsVowel(letterChar))
        {
            PlayVowelSound(letterChar);
        }
        else
        {
            PlayConsonantSound(letterChar);
        }
        
        Debug.Log($"LetterPickup: Generated pronunciation sound for letter '{upperLetter}'");
    }
    
    bool IsVowel(char letter)
    {
        return "AEIOU".Contains(letter.ToString());
    }
    
    void PlayVowelSound(char letter)
    {
        // Vowels get longer, more melodic sounds
        float frequency = GetVowelFrequency(letter);
        float duration = 0.5f; // Longer for vowels
        
        CreateAndPlayTone(frequency, duration, 0.4f);
    }
    
    void PlayConsonantSound(char letter)
    {
        // Consonants get shorter, sharper sounds
        float frequency = GetConsonantFrequency(letter);
        float duration = 0.2f; // Shorter for consonants
        
        CreateAndPlayTone(frequency, duration, 0.3f);
    }
    
    float GetVowelFrequency(char letter)
    {
        switch (letter)
        {
            case 'A': return 440f;  // A
            case 'E': return 523f;  // E
            case 'I': return 659f;  // I
            case 'O': return 392f;  // O
            case 'U': return 349f;  // U
            default: return 440f;
        }
    }
    
    float GetConsonantFrequency(char letter)
    {
        switch (letter)
        {
            case 'B': return 247f;  // B
            case 'C': return 262f;  // C
            case 'D': return 294f;  // D
            case 'F': return 311f;  // F
            case 'G': return 330f;  // G
            case 'H': return 349f;  // H
            case 'J': return 370f;  // J
            case 'K': return 392f;  // K
            case 'L': return 415f;  // L
            case 'M': return 440f;  // M
            case 'N': return 466f;  // N
            case 'P': return 494f;  // P
            case 'Q': return 523f;  // Q
            case 'R': return 554f;  // R
            case 'S': return 587f;  // S
            case 'T': return 622f;  // T
            case 'V': return 659f;  // V
            case 'W': return 698f;  // W
            case 'X': return 740f;  // X
            case 'Y': return 784f;  // Y
            case 'Z': return 831f;  // Z
            default: return 440f;
        }
    }
    
    void CreateAndPlayTone(float frequency, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = (int)(sampleRate * duration);
        
        AudioClip toneClip = AudioClip.Create($"Letter_{letter.ToUpper()}", samples, 1, sampleRate, false);
        float[] audioData = new float[samples];
        
        for (int i = 0; i < samples; i++)
        {
            float time = (float)i / sampleRate;
            // Add some harmonics for more realistic sound
            float wave = Mathf.Sin(2f * Mathf.PI * frequency * time) * 0.7f +
                        Mathf.Sin(2f * Mathf.PI * frequency * 2f * time) * 0.2f +
                        Mathf.Sin(2f * Mathf.PI * frequency * 3f * time) * 0.1f;
            
            // Apply fade in/out for smoother sound
            float fadeIn = Mathf.Clamp01(time / 0.05f);
            float fadeOut = Mathf.Clamp01((duration - time) / 0.05f);
            float envelope = fadeIn * fadeOut;
            
            audioData[i] = wave * volume * envelope;
        }
        
        toneClip.SetData(audioData, 0);
        audioSource.clip = toneClip;
        audioSource.Play();
    }
} 