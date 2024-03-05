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
using mixpanel;
using Yarn;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Scene Management")] 
    public Canvas datingSimInterface;
    public EventSystem datingSimEventSystem;
    public DialogueRunner dialogueRunnerInstance;
    private bool fpsLoaded = false;
    public static bool isGamePaused = false;
    private string currentCharacter;
    private int currentLine = 0;                // for returning to dialogue after successfully exiting FPS
    private string currentNode = "Start";       // for returning to dialogue after successfully exiting FPS
    private int dialogueLineToReturn = 0;   // for returning to dialogue after successfully exiting FPS
    private string dialogueNodeToReturn = "";   // for returning to dialogue after successfully exiting FPS

    [Header("GAME FEATURE FLAGS (IMPORTANT)")]
    public bool FLAG_ENABLE_AFFECTION_INTERRUPT;
    public bool FLAG_DEV_DISABLE_LOGGING;
    
    [Header("Characters")] 
    public Image characterImage;
    public Characters characterSO;
    public int characterInterruptWithFPSAffectionThreshhold = 70;
    
    [Header("Dialogue")]
    private List<string> protectedNodeNames = new List<string> { "LOSE_", "INTERRUPT_", "FPS_", "EnterFPS", "ExitFPS", "DAY_" };

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
    public float maxHealth = 100.0f;

    public float currentHealth { get; set; }
    public float StartingHealth = 100.0f;
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

    //for exit dialogue in FPS mode
    public int FPScounter = 0;

    [Header("PauseMenu")] 
    public bool pauseMenu = false;
    private int counter = 0;
    
    // Analytics
    private int analytics_dayCounter = 1;
    private int analytics_timesFPSEnteredThisDay = 0;
    

    private void Awake()
    {
        instance = this;
    }
    private IEnumerator Start()
    {
        foreach(Character c in characterSO.CharacterList) {
            c.InitializeCharacter();
        }
        var loaded = false;
        var loadedLevel = SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
        SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
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
        
        Analytics.LogAnalyticEvent("Gameplay Started");
    }
    void Update()
    { 
        // this is just for testing additive scene loading
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (fpsLoaded) EndFPS(false);
            else StartFPS();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (!pauseMenu)
            {
                GameObject pauseUI = GameObject.Find("PauseMenu");
                if (pauseUI)
                {
                    pauseUI.GetComponent<PauseMenu>().PauseGame();
                }

                pauseMenu = true;
            }
            else
            {
                GameObject pauseUI = GameObject.Find("PauseMenu");
                if (pauseUI)
                {
                    pauseUI.GetComponent<PauseMenu>().ResumeGame();
                }
                pauseMenu = false;
            }
        }
    }

    [YarnCommand("Checkpoint")]
    public void Checkpoint()
    {
        // method is scuffed but we can't modify yarn directly 
        // we will be using node names as checkpoints. Make sure to call this Checkpoint function regularly
        // i.e. at the start of each node. No way at the moment to resume to an exact line, so saving a checkpoint
        // at any point in the node means that the player gets sent to the start of the node if they are to resume at that point.
        
        string currentNodeName = dialogueRunnerInstance.Dialogue.CurrentNode;
        // Debug.Log("next line clicked; now in " + currentNodeName);

        // Check if currentNodeName is different from currentNode and not a protected node name
        bool containsProtectedNode = false;
        foreach (string protectedNode in protectedNodeNames)
        {
            if (currentNodeName.Contains(protectedNode))
            {
                containsProtectedNode = true;
                // If any substring is found, set containsProtectedNode to true and continue checking
            }
        }

        if (currentNodeName != currentNode && !containsProtectedNode)
        {
            dialogueNodeToReturn = currentNodeName;
            currentLine = 0;
        }
        else
        {
            currentLine++;
        }            
        // Debugging current node and line number
        // Debug.Log("//////// line debug: " + currentNode + ", " + currentLine);
    }

    public bool GetFpsLoaded()
    {
        return fpsLoaded;
    }

    [YarnCommand("StartFPS")]
    public void StartFPS()
    {
        fpsLoaded = true;
        if(FPScounter >= 2)
        {
            EndFPS(false);
        }
        FPScounter++;
        analytics_timesFPSEnteredThisDay++;
        
        // hides dating sim ui
        characterImage.enabled = false;
        backgroundImage.enabled = false;

        dialogueRunnerInstance.Stop();
        // load in fps scene
        SceneManager.LoadScene("FPSScene", LoadSceneMode.Additive);
        
        //switch music
        MusicManager.Instance.PlayMusic(MusicManager.Instance.music_FPS);
        PauseFPS();
        dialogueRunnerInstance.StartDialogue("EnterFPS");
    }

    public void ExitDialogue()
    {
        backgroundImage.enabled = false;
        
        Cursor.lockState = CursorLockMode.None;
        if(counter == 0)
        {
            counter = 1;
            PauseFPS();
            dialogueRunnerInstance.StartDialogue("ExitFPS");
        }
        else
        {
            EndFPS(false);
            RefreshHealth(-1.0f);
            counter = 0;
        }

    }

    [YarnCommand("EndFPS")]
    public void EndFPS(bool didDefeatEnemy)
    {
        fpsLoaded = false;
        FPScounter = 0;
        
        // reset character likeability
        Character character = characterSO.CharacterList.Find(c => c.CharacterName == currentCharacter);
        if (character != null)
        {
            character.SetAffection(80);
            Debug.Log("LIKABILITY: " + currentLikability + " | THRESHHOLD: " + LosingLikabilityThreshold);
        }
        else
        {
            Debug.LogWarning($"Character '{currentCharacter}' not found.");
        }
        
        // enable dating sim ui
        datingSimInterface.enabled = true;
        characterImage.enabled = true;
        backgroundImage.enabled = true;

        // handles event system control back
        datingSimEventSystem.enabled = true;
        
        dialogueRunnerInstance.Stop();
        if (!didDefeatEnemy)
        {
            // trigger transition back dialogue
            dialogueRunnerInstance.StartDialogue("transitionBack");
        }
        else
        {
            // reload back into FPS_SUCCESS node
            dialogueRunnerInstance.StartDialogue("FPS_SUCCESS_" + currentCharacter);
            SetCharacter(currentCharacter, 0);                                  
        }
        
        MusicManager.Instance.PlayMusic(MusicManager.Instance.music_classroom);
        
        // unload fps scene
        Cursor.lockState = CursorLockMode.None;
        SceneManager.UnloadSceneAsync("FPSScene");
    }

    [YarnCommand("ContinueFromPreFPSNode")]
    public void ContinueFromPreFPSNode()
    {
        dialogueRunnerInstance.Stop();
        dialogueRunnerInstance.StartDialogue(dialogueNodeToReturn);
    }
    
    [YarnCommand("SetCharacter")]
    public void SetCharacter(string characterName, int spriteIndex = 0)
    {
        Debug.Log($"Switching to character: {characterName}");
        currentCharacter = characterName;
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

    [YarnCommand("TakeDamage")]
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        healthBar.SetHealthPercentage(StartingHealth, currentHealth);
    }
    // Function that can be called by Yarn to change between FPS/Dialogue mode
    // [YarnCommand("ChangeMode")]
    // public void ChangeMode(bool fpsMode)
    // {
    //     if (fpsMode)
    //     {
    //         StartFPS();
    //     } else
    //     {
    //         EndFPS();
    //     }
    // }

    // I am making this changing likability by a delta...
    [YarnCommand("UpdateLikability")]
    public void UpdateLikability(float delta)
    {
        currentLikability += delta;
        if (currentLikability < LosingLikabilityThreshold)
        {
            // TODO possibly remove this?
            dialogueRunnerInstance.StartDialogue("dialogueDay" + currentDay);
            currentHealth = dayStart_health;
            currentLikability = dayStart_likability;
        }
    }

    [YarnCommand("ChangeAffection")]
    public void ChangeAffection(string characterName, float amount)
    {
        Character character = characterSO.CharacterList.Find(c => c.CharacterName == characterName);
        if (character != null)
        {
            character.ChangeAffection(amount);
            Debug.Log("LIKABILITY: " + currentLikability + " | THRESHHOLD: " + LosingLikabilityThreshold);
            if (FLAG_ENABLE_AFFECTION_INTERRUPT && character.characterAffection < characterInterruptWithFPSAffectionThreshhold)
            {
                // reset dialogue and re-enter at low affection interrupt
                dialogueRunnerInstance.Stop();
                dialogueRunnerInstance.StartDialogue("INTERRUPT_LOW_AFFECTION_" + currentCharacter);
            }
        }
        else
        {
            Debug.LogWarning($"Character '{characterName}' not found.");
        }
    }

    [YarnCommand("LogDayEndAnalytics")]

    public void LogDayEndAnalytics()
    {
        var props = new Value();
        props["day"] = analytics_dayCounter;
        props["times_fps_triggered"] = analytics_timesFPSEnteredThisDay;
        Analytics.LogAnalyticEvent("Times FPS triggered per day cycle", props);

        analytics_dayCounter++;
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

    
    [YarnCommand("PauseFPS")]
    public void PauseFPS()
    {
        isGamePaused = true;
    
        // handles event system control over to FPS scene
        datingSimEventSystem.enabled = true;
    
        Cursor.lockState = CursorLockMode.None;
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

    // Display and apply buffs earned through dialogue
    [YarnCommand("AddHealthBuff")]
    public void AddHealthBuff()
    {
        StartingHealth += 20.0f;
        maxHealth += 20.0f;
        healthBar.DisplayBuff("Health");
    }

    [YarnCommand("AddDamageBuff")]
    public void AddDamageBuff()
    {
        healthBar.DisplayBuff("Damage");
    }

    [YarnCommand("AddGrossnessBuff")]
    public void AddGrossnessBuff()
    {
        currentLikability -= 20.0f;
        healthBar.DisplayBuff("Grossness");
    }

    [YarnCommand("RefreshHealth")]
    public void RefreshHealth(float newHealth)
    {
        if (newHealth < 0.0f)
        {
            newHealth = StartingHealth;
        }

        currentHealth = newHealth;
        healthBar.SetHealthPercentage(StartingHealth, currentHealth);
        FirstPersonManager.instance.RefreshHealth();
    }

    public Sprite GetCurrentEnemySprite()
    {
        Character character = characterSO.CharacterList.Find(c => c.CharacterName == currentCharacter);
        return character.CharacterImage(4);
    }
}
