using UnityEngine;
using System.Collections.Generic;

// AudioManager for handling letter pronunciation audio
public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    private AudioSource audioSource;
    private Dictionary<string, AudioClip> letterAudioClips = new Dictionary<string, AudioClip>();
    
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<AudioManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("AudioManager");
                    instance = go.AddComponent<AudioManager>();
                }
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioClips();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAudioClips()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.8f;
        
        // Load all letter audio clips
        string[] letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        
        #if UNITY_EDITOR
        foreach (string letter in letters)
        {
            string audioPath = $"Assets/Sprites/Letters/audio_alphabet/{letter}.wav";
            AudioClip clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(audioPath);
            
            if (clip != null && clip.length > 0)
            {
                letterAudioClips[letter] = clip;
                Debug.Log($"AudioManager: Loaded audio clip for letter '{letter}' (duration: {clip.length}s)");
            }
            else
            {
                Debug.LogWarning($"AudioManager: Could not load audio clip for letter '{letter}' from path: {audioPath} - will use generated sound");
            }
        }
        #else
        Debug.LogWarning("AudioManager: Audio clips need to be assigned manually in build");
        #endif
        
        Debug.Log($"AudioManager: Initialized with {letterAudioClips.Count} audio clips");
    }
    
    public AudioClip GetLetterAudioClip(string letter)
    {
        string upperLetter = letter.ToUpper();
        if (letterAudioClips.ContainsKey(upperLetter))
        {
            return letterAudioClips[upperLetter];
        }
        return null;
    }
    
    public void PlayLetterSound(string letter)
    {
        // Audio disabled for the entire game
        Debug.Log($"AudioManager: Audio disabled - would have played sound for letter '{letter.ToUpper()}'");
    }
    
    private void PlayGeneratedLetterSound(string letter)
    {
        // Generate a simple tone based on the letter
        float frequency = GetLetterFrequency(letter);
        AudioClip generatedClip = GenerateTone(frequency, 0.3f);
        
        if (generatedClip != null)
        {
            audioSource.clip = generatedClip;
            audioSource.Play();
        }
    }
    
    private float GetLetterFrequency(string letter)
    {
        // Map letters to different frequencies for variety
        switch (letter.ToUpper())
        {
            case "A": return 440f; // A4
            case "B": return 494f; // B4
            case "C": return 523f; // C5
            case "D": return 587f; // D5
            case "E": return 659f; // E5
            case "F": return 698f; // F5
            case "G": return 784f; // G5
            case "H": return 880f; // A5
            case "I": return 988f; // B5
            case "J": return 1047f; // C6
            case "K": return 1175f; // D6
            case "L": return 1319f; // E6
            case "M": return 1397f; // F6
            case "N": return 1568f; // G6
            case "O": return 1760f; // A6
            case "P": return 1976f; // B6
            case "Q": return 2093f; // C7
            case "R": return 2349f; // D7
            case "S": return 2637f; // E7
            case "T": return 2794f; // F7
            case "U": return 3136f; // G7
            case "V": return 3520f; // A7
            case "W": return 3951f; // B7
            case "X": return 4186f; // C8
            case "Y": return 4699f; // D8
            case "Z": return 5274f; // E8
            default: return 440f; // Default to A4
        }
    }
    
    private AudioClip GenerateTone(float frequency, float duration)
    {
        int sampleRate = 44100;
        int samples = (int)(sampleRate * duration);
        float[] samplesArray = new float[samples];
        
        for (int i = 0; i < samples; i++)
        {
            float time = (float)i / sampleRate;
            samplesArray[i] = Mathf.Sin(2f * Mathf.PI * frequency * time);
            
            // Add fade in/out to prevent clicks
            float fadeTime = 0.05f;
            if (time < fadeTime)
            {
                samplesArray[i] *= time / fadeTime;
            }
            else if (time > duration - fadeTime)
            {
                samplesArray[i] *= (duration - time) / fadeTime;
            }
        }
        
        AudioClip clip = AudioClip.Create("GeneratedTone", samples, 1, sampleRate, false);
        clip.SetData(samplesArray, 0);
        return clip;
    }
} 