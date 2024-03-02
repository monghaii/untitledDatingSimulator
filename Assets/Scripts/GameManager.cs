using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Scene Management")] 
    public Canvas datingSimInterface;
    public EventSystem datingSimEventSystem;
    public DialogueRunner dialogueRunnerInstance;
    private bool fpsLoaded = false;
    public static bool isGamePaused = false;
    
    [Header("Characters")] 
    public Image characterImage;
    public Characters characterSO;

    // This should be populated in Unity Editor
    [Header("EndScreens")] 
    public GameObject winningScreen;

    // Magic numbers here are placeholders
    [Header("Attributes")] 
    public int currentDay = 1;
    public const int MaxDays = 1;
    // NOTE: naming convention for different day dialogues
    // dialogueDay + Number
    // e.g. dialogueDay2

    [Header("HealthBar")]
    public HealthBar healthBar;

    public float currentHealth { get; set; }
    public const float StartingHealth = 100.0f;
    public float currentLikability { get; set; }
    public const float StartingLikability = 60.0f;
    public const float LosingLikabilityThreshold = 20.0f;
    
    // This section is used to store data at the beginning of the day
    // used for sending the player back to the start
    [Header("SavedDayData")] 
    public float dayStart_health;
    public float dayStart_likability;

    [Header("Background")]
    public Image backgroundImage;
    public Backgrounds backgroundSO;

    private void Awake()
    {
        instance = this;
    }
    private IEnumerator Start()
    {
        var loaded = false;
        var loadedLevel = SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
        yield return loadedLevel;
        loaded = true;
        datingSimInterface = GameObject.Find("DatingCanvas").GetComponent<Canvas>();
        characterImage = GameObject.Find("CharacterSprite").GetComponent<Image>();
        backgroundImage = GameObject.Find("BackgroundSprite").GetComponent<Image>();
        healthBar = datingSimInterface.GetComponentInChildren<HealthBar>();
        
        // This may not be the best practice...
        dialogueRunnerInstance = FindAnyObjectByType<DialogueRunner>();
        MusicManager.Instance.PlayMusic(MusicManager.Instance.music_classroom);
        
        // Initializing attributes
        currentHealth = StartingHealth;
        currentLikability = StartingLikability;
        dayStart_health = currentHealth;
        dayStart_likability = currentLikability;
    }
    void Update()
    { 
        // this is just for testing additive scene loading
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (fpsLoaded) EndFPS();
            else StartFPS();
        }
    }
    
    
    public bool GetFpsLoaded()
    {
        return fpsLoaded;
    }

    [YarnCommand("StartFPS")]
    public void StartFPS()
    {
        fpsLoaded = true;
        
        // hides dating sim ui
        characterImage.enabled = false;
        backgroundImage.enabled = false;

        dialogueRunnerInstance.Stop();
        // load in fps scene
        SceneManager.LoadScene("FPSScene", LoadSceneMode.Additive);
        
        //switch music
        MusicManager.Instance.PlayMusic(MusicManager.Instance.music_FPS);
        dialogueRunnerInstance.StartDialogue("EnterFPS");
        isGamePaused = true;
    }

    [YarnCommand("EndFPS")]
    public void EndFPS()
    {
        fpsLoaded = false;
        
        // enable dating sim ui
        datingSimInterface.enabled = true;
        characterImage.enabled = true;

        // handles event system control back
        datingSimEventSystem.enabled = true;
        
        // trigger transition back dialogue
        dialogueRunnerInstance.StartDialogue("transitionBack");
        
        // unload fps scene
        Cursor.lockState = CursorLockMode.None;
        SceneManager.UnloadSceneAsync("FPSScene");
    }

    [YarnCommand("SetCharacter")]
    public void SetCharacter(string characterName, int spriteIndex = 0)
    {
        Debug.Log($"Switching to character: {characterName}");
        // Find the character in the CharacterList by name

        characterImage.gameObject.SetActive(characterName != "me");
        
        Character character = characterSO.CharacterList.Find(c => c.CharacterName == characterName);
        if (character != null)
        {
            characterImage.sprite = character.CharacterImage(spriteIndex);
        }
        else
        {
            Debug.LogWarning($"Character '{characterName}' not found.");
        }
    }

    // Function that can be called by Yarn to change between FPS/Dialogue mode
    [YarnCommand("ChangeMode")]
    public void ChangeMode(bool fpsMode)
    {
        if (fpsMode)
        {
            StartFPS();
        } else
        {
            EndFPS();
        }
    }

    // I am making this changing likability by a delta...
    [YarnCommand("UpdateLikability")]
    public void UpdateLikability(float delta)
    {
        currentLikability += delta;
        if (currentLikability < LosingLikabilityThreshold)
        {
            dialogueRunnerInstance.StartDialogue("dialogueDay" + currentDay);
            currentHealth = dayStart_health;
            currentLikability = dayStart_likability;
        }
    }
    
    [YarnCommand("ProgressDay")]
    public void ProgressDay()
    {
        // If we are at the last day
        if (currentDay == MaxDays)
        {
            // TODO: determine winning and losing, showing winning as default now.
            ShowEndScreen("winning");
        }
        else
        {
            currentDay++;
            dayStart_health = currentHealth;
            dayStart_likability = currentLikability;
            dialogueRunnerInstance.StartDialogue("dialogueDay" + currentDay);
        }
    }
    
    // This is a Yarn-callable function that shows screen if existed by name
    [YarnCommand("ShowEndScreen")]
    public void ShowEndScreen(string screenName)
    {
        if (screenName == "winning")
        {
            characterImage.enabled = false;
            winningScreen.SetActive(true);
        }
        // TODO: losing screen, or other endings
    }
    
    [YarnCommand("SetBackground")]
    public void SetBackground(string backgroundName)
    {
        Debug.Log($"Switching to background: {backgroundName}");
        // Find the character in the CharacterList by name
        Background background = backgroundSO.BackgroundList.Find(b => b.BackgroundName == backgroundName);
        if (background != null)
        {
            backgroundImage.sprite = background.BackgroundImage;
        }
        else
        {
            Debug.LogWarning($"Background '{backgroundName}' not found.");
        }
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [YarnCommand("ResumeFPS")]
    public void ResumeFPS()
    {
        isGamePaused = false;
        dialogueRunnerInstance.Stop();
        backgroundImage.enabled = false;

        // handles event system control over to FPS scene
        datingSimEventSystem.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
    }
}
