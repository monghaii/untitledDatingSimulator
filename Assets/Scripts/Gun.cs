using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Camera fpsCam;

    [Header("Stats")]
    public float damage = 10f;
    public float range = 100f;
    public float knockbackForce = 30f;
    public float fireRate = 15f;
    private float nextTimeToFire = 0f;
    private List<GameObject> bulletPool;    //this is for if we want actual bullets, rn its just playing an effect at hit location :')
    
    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.pauseMenu || FirstPersonManager.isFpsPaused)
        {
            return;
        }
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        //play sfx
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfx_GunFire, this.gameObject);
        
        // play effects
        muzzleFlash.Play();
        
        // check if hit target
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            // apply damage
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.TakeDamage(damage);
            }
            
            // optional knockback
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(hit.normal * knockbackForce);
            }

            // more effects
            GameObject vfxImpact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(vfxImpact, 0.2f);
        }

    }
}
