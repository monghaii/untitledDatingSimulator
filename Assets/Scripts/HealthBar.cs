using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBar;
    public Text healthValue;

    [SerializeField]
    private GameObject buffs;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetHealthPercentage(float maxHealth, float currHealth)
    {
        healthBar.fillAmount = currHealth / maxHealth;
        healthValue.text = currHealth.ToString() + "/" + maxHealth.ToString();
    }

    public void DisplayBuff(string buffname)
    {
        Transform child = buffs.transform.Find(buffname);
        child.gameObject.SetActive(true);
    }
}
