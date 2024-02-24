using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonManager : MonoBehaviour
{
    public GameManager gamemanager;
    
    [Header("Health")] 
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isDead = false;
    // also need to connect this to the UI
    public HealthBar healthBar;
    
    // Start is called before the first frame update
    void Start()
    {
        // Todo: have it read from the game manager instead??
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            //Todo: death behavior and switching back to dating sim mode?
        }
        healthBar.SetHealthPercentage(maxHealth, currentHealth);
    }
    
    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0f)
        {
            isDead = true;
        }
    }
}
