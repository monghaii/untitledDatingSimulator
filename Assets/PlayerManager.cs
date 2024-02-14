using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Damageable playerHealth;
    public GameManager gameManager;
    public GameObject winState;
    public GameObject failState;
    private bool showedEndState = false;
    public Slider healthBar;
    
    // Start is called before the first frame update
    void Start()
    {
        winState.SetActive(false);
        failState.SetActive(false);
        healthBar.maxValue = playerHealth.maxHealth;
        healthBar.value = playerHealth.currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth.isDead)
        {
            if (showedEndState)
            {
                return;
            }
            
            failState.SetActive(true);
            Time.timeScale = 0f;
            gameManager.EndFPS();
        }

        healthBar.value = playerHealth.currentHealth;
    }
}
