using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        Damageable player = other.gameObject.GetComponent<Damageable>();
        if (player && other.gameObject.tag == "Player")
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
