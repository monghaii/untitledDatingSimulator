using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class GameManager : MonoBehaviour
{
    [Header("DatingSim")] 
    public Canvas datingSimMode;
    
    [Header("FPS")] 
    public GameObject fpsMode;

    [Header("CameraManagement")] 
    public Camera datingSimCamera;
    public Camera fpsCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        // dating sim enabled at the beginning
        datingSimCamera.enabled = true;
        
        // fps mode disabled at the beginning
        fpsMode.SetActive(false);
        fpsCamera.enabled = false;
    }
    
    public void StartFPS()
    {
        fpsMode.SetActive(true);
        datingSimMode.enabled = false;
        ShowFPSCamera();
    }
    public void EndFPS()
    {
        fpsMode.SetActive(false);
        datingSimMode.enabled = true;
        ShowDatingSimCamera();
    }
    
    // boilerplate to expose a method to yarn runtime
    [YarnCommand("testYarnUnityIntegration")]
    public static void testYarnUnityIntegration() {
        Debug.Log($"I am called from yarn :)");
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowDatingSimCamera()
    {
        datingSimCamera.enabled = true;
        fpsCamera.enabled = false;
    }

    public void ShowFPSCamera()
    {
        datingSimCamera.enabled = false;
        fpsCamera.enabled = true;
    }
}
