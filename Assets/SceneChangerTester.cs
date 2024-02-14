using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerTester : MonoBehaviour
{
    public string sceneToLoad; // Name of the scene you want to load

    private void Start()
    {
        // Start a coroutine to change the scene after 3 seconds
        StartCoroutine(ChangeSceneAfterDelay());
    }

    private IEnumerator ChangeSceneAfterDelay()
    {
        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Load the provided scene
        SceneManager.LoadScene(sceneToLoad);
    }
}