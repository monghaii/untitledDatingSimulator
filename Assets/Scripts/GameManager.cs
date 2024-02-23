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
    private bool fpsLoaded = false;
    
    [Header("Characters")] 
    public Image characterImage;
    public Characters characterSO;

    // This should be populated in Unity Editor
    [Header("EndScreens")] public GameObject winningScreen;


    [Header("Background")]
    public Image backgroundImage;
    public Backgrounds backgroundSO;

    private IEnumerator Start()
    {
        var loaded = false;
        var loadedLevel = Application.LoadLevelAdditiveAsync("UI");
        yield return loadedLevel;
        loaded = true;
        datingSimInterface = GameObject.Find("DatingCanvas").GetComponent<Canvas>();
        characterImage = GameObject.Find("CharacterSprite").GetComponent<Image>();
        backgroundImage = GameObject.Find("BackgroundSprite").GetComponent<Image>();
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
    }
    public void EndFPS()
    {
        fpsLoaded = false;
        
        // enable dating sim ui
        datingSimInterface.enabled = true;
        characterImage.enabled = true;

        // handles event system control back
        datingSimEventSystem.enabled = true;
        
        // unload fps scene
        Cursor.lockState = CursorLockMode.None;
        SceneManager.UnloadSceneAsync("FPSScene");
    }
    
    // boilerplate to expose a method to yarn runtime
    // https://docs.yarnspinner.dev/using-yarnspinner-with-unity/creating-commands-functions
    [YarnCommand("TestYarnUnityIntegration")]
    public static void TestYarnUnityIntegration() {
        Debug.Log($"I am called from yarn :)");
    }

    [YarnCommand("SetSprite")]
    public void SetSprite(string characterName, int spriteIndex = 0)
    {
        Debug.Log($"Switching to {characterName}");
        // Find the character in the CharacterList by name
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

    // Function that can be called by Yarn to change game view
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

    // This is a Yarn-callable function that shows screen if existed by name
    [YarnCommand("ShowEndScreen")]
    public void ShowEndScreen(string screenName)
    {
        if (screenName == "winning")
        {
            characterImage.enabled = false;
            winningScreen.SetActive(true);
        }
    }
    
    [YarnCommand("SetBackground")]
    public void SetBackground(string backgroundName)
    {
        Debug.Log($"Switching to {backgroundName}");
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
