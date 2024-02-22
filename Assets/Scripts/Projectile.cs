using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        FirstPersonManager player = other.gameObject.GetComponent<FirstPersonManager>();
        if (player)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
