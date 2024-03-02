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
    [Header("Scene Management")] 
    public Canvas datingSimInterface;
    public EventSystem datingSimEventSystem;
    public DialogueRunner dialogueRunnerInstance;
    private bool fpsLoaded = false;
    
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
    public float currentHealth = 100.0f;
    public float currentLikability = 50.0f;

    [Header("Background")]
    public Image backgroundImage;
    public Backgrounds backgroundSO;

    private IEnumerator Start()
    {
        var loaded = false;
        var loadedLevel = SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
        yield return loadedLevel;
        loaded = true;
        datingSimInterface = GameObject.Find("DatingCanvas").GetComponent<Canvas>();
        characterImage = GameObject.Find("CharacterSprite").GetComponent<Image>();
        backgroundImage = GameObject.Find("BackgroundSprite").GetComponent<Image>();
        
        // This may not be the best practice...
        dialogueRunnerInstance = FindAnyObjectByType<DialogueRunner>();
        MusicManager.Instance.PlayMusic(MusicManager.Instance.music_classroom);
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
        datingSimInterface.enabled = false;
        characterImage.enabled = false;
        
        // handles event system control over to FPS scene
        datingSimEventSystem.enabled = false;

        // load in fps scene
        Cursor.lockState = CursorLockMode.Locked; // also done in the FirstPersonCamera script but here again just in case
        SceneManager.LoadScene("FPSScene", LoadSceneMode.Additive);
        
        //switch music
        MusicManager.Instance.PlayMusic(MusicManager.Instance.music_FPS);
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
}
