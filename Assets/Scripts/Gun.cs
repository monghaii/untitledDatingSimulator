using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Camera fpsCam;

    [Header("Stats")]
    public float damage = 10f;
    public float range = 100f;
    public float knockbackForce = 30f;
    public float fireRate = 15f;
    public float maxAmmo = 10f;
    private float ammo;
    public TMP_Text ammoUI;
    public GameObject reloadMsg;
    private float nextTimeToFire = 0f;
    public float reloadTime = 1f;
    private List<GameObject> bulletPool;    //this is for if we want actual bullets, rn its just playing an effect at hit location :')
    
    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    void Start()
    {
        ammo = maxAmmo;
        reloadMsg.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.pauseMenu || FirstPersonManager.isFpsPaused)
        {
            return;
        }
        if (Input.GetButton("Fire1"))
        {
            if (ammo <= 0)
            {
                // play empty gun sfx
                // display reload message
                StartCoroutine(PlayReloadMessage());
            }
            else if (Time.time >= nextTimeToFire)
            {
                // shooting
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            // reload
            ammo = maxAmmo;
            ammoUI.text = "" + ammo;
            nextTimeToFire = reloadTime;
        }
    }

    private void Shoot()
    {
        // decrease ammo
        ammo--;
        ammoUI.text = "" + ammo;
        
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

    IEnumerator PlayReloadMessage()
    {
        reloadMsg.SetActive(true);
        yield return new WaitForSeconds(2f);
        reloadMsg.SetActive(false);
    }
}
