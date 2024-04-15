using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Enemy enemy;
    /*private void OnTriggerEnter(Collider other)
    {
        FirstPersonManager player = other.gameObject.GetComponent<FirstPersonManager>();
        if (player)
        {
            player.TakeDamage(enemy.damage);
            Destroy(gameObject);
        }
    }*/

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            FirstPersonManager player = other.gameObject.GetComponent<FirstPersonManager>();
            player.TakeDamage(enemy.damage);
            Destroy(gameObject);
        }
    }
}
