using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Animator animator;

    public Transform firePoint;
    public float fireRate=0.1f;
    public float fireRange = 10f;
    private float nextFireTime = 0f;
    public bool isAuto = false;
    public int maxAmmo = 30;
    private int currentAmmo;
    public float reloadTime = 1.5f;
    private bool isReloading = false;

    public AudioSource soundAudioSource;
    public AudioClip shootingSoundClip;
    public AudioClip reloadSoundClip;

        // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading)
            return;


        if (isAuto == true)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                shoot();
            }
            else
            {
                animator.SetBool("Shoot", false);
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                shoot();
            }
            else
            {
                animator.SetBool("Shoot", false);
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            Reload();
        }
        
    }

    private void shoot()
    {
        if (currentAmmo > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, fireRange))
            {
                Debug.Log(hit.transform.name);
            }
            animator.SetBool("Shoot", true);
            currentAmmo--;
            soundAudioSource.PlayOneShot(shootingSoundClip);
        }
        else
        {
            Reload();
        }
       

    }

    private void Reload()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            animator.SetTrigger("Reload");
            isReloading = true;
            //reload sound
            soundAudioSource.PlayOneShot(reloadSoundClip);
            Invoke("FinishReloading", reloadTime); 

        }

    }
    private void FinishReloading()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        animator.ResetTrigger("Reload");
    }

    
    
}
