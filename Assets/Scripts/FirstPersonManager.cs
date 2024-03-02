using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonManager : MonoBehaviour
{
    public GameManager gm;
    public Enemy enemyInstance;
    
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
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        enemyInstance = GameObject.Find("Enemy").GetComponent<Enemy>();
        if (gm != null)
        {
            currentHealth = gm.currentHealth;
        }
        else
        {
            currentHealth = maxHealth;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //Determine if is dead (enemy or player)
        if (currentHealth <= 0.0f || enemyInstance.currentHealth <= 0.0f)
        {
            isDead = true;
        }
        
        if (isDead)
        {
            //TODO: different death behavior for enemy dead and player dead
            gm.EndFPS();
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
