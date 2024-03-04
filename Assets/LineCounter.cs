using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineCounter : MonoBehaviour
{
    private Button button;
    private GameManager gameManager;

    void Start()
    {
        // Find the button component attached to this GameObject
        button = GetComponent<Button>();
        // Add a listener to the button's onClick event
        button.onClick.AddListener(OnButtonClick);
        // find gameManager
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("GameManager not found.");
        }
    }

    private void OnButtonClick()
    {
        gameManager.Checkpoint();
    }
}