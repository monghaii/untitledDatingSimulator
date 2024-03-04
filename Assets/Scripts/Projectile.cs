using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Enemy enemy;
    private void OnTriggerEnter(Collider other)
    {
        FirstPersonManager player = other.gameObject.GetComponent<FirstPersonManager>();
        if (player)
        {
            player.TakeDamage(enemy.damage);
            Destroy(gameObject);
        }
    }
}
