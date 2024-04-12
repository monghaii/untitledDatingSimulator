using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject mainPausePanel;
    [SerializeField] private GameObject optionsPausePanel;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider mouseSensitivitySlider;
    
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        mainPausePanel.SetActive(true);
        optionsPausePanel.SetActive(false);
        masterVolumeSlider.value = AudioListener.volume;
        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        pauseMenu.SetActive(true);
        mainPausePanel.SetActive(true);
        //Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        if(GameManager.instance.GetFpsLoaded())
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void OptionsPanel()
    {
        mainPausePanel.SetActive(false);
        optionsPausePanel.SetActive(true);
    }
    
    public void PausePanel()
    {
        mainPausePanel.SetActive(true);
        optionsPausePanel.SetActive(false);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
        PlayerPrefs.Save();
    }
}
