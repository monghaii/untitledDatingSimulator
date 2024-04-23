using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.CommandLine.Parsing;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject levelSelect;
    private string levelToLoad = "";
    [SerializeField]
    private GameObject eventSystem;
    private int selectedDay = 1;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LevelSelect()
    {
        levelSelect.SetActive(true);
    }

    public string GetSelectedLevel()
    {
        return levelToLoad;
    }

    public int GetSelectedDay()
    {
        return selectedDay;
    }
    public void LoadSelectedLevel(string name)
    {
        selectedDay = int.Parse(name);
        if(selectedDay == 1)
        {
            levelToLoad = "GoToClass";
        }
        else if(selectedDay == 3)
        {
            levelToLoad = "day3_otheranimegirl";
        }
        else
        {
            levelToLoad = "dialogueDay" + name;
        }
        eventSystem.SetActive(false);
        SceneManager.LoadScene("Main", LoadSceneMode.Additive);
    }

    public void Back()
    {
        levelSelect.SetActive(false);
    }

    public void StartGame()
    {
        eventSystem.SetActive(false);
        SceneManager.LoadScene("Main", LoadSceneMode.Additive);
    }
}
