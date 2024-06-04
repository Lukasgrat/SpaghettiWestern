using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public enum Gunplay
{
    Reloading,
    Readied,
    Firing,
    Dead,
}

public class PlayerShooting : MonoBehaviour
{
    public MouseLook Camera;
    public int curAmmo;
    public int maxAmmo = 6;
    public AudioClip reloadingSFX;
    public AudioClip shootingSFX;
    AudioSource audiosource;
    public float reloadTime = 3.1f;
    public float fireTime = .3f;
    float cooldownTime = 0;
    public Gunplay curState;
    public TextMeshProUGUI ammoText;

    // Start is called before the first frame update
    void Start()
    {
        curAmmo = maxAmmo;
        curState = Gunplay.Readied;
        audiosource = GetComponent<AudioSource>();  
        if (ammoText == null) 
        {
            throw new System.Exception("Ammo counter is not linked up");
        }
    }

    //Update is called once per frame
    void Update()
    {
        InputHandler();
        StateHandler();
    }

    private void InputHandler() 
    {
        if (curState != Gunplay.Readied) 
        {
            return;
        }
        
        
        if (Input.GetButtonDown("Fire1") && curAmmo > 0)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemyObj in enemies)
            {
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(10);
                    enemy.canShoot = true; // Modify the canShoot property of the enemy script
                }
            }
            curAmmo -= 1;
            updateAmmoText();

            Camera.iniateRecoil();
            audiosource.clip = shootingSFX;
            audiosource.Play();
            curState = Gunplay.Firing;
        }
        if (Input.GetKeyDown(KeyCode.R) || (curAmmo == 0 && Input.GetButtonDown("Fire1")))
        {
            curState = Gunplay.Reloading;
            audiosource.clip = reloadingSFX;
            audiosource.Play();
            curState = Gunplay.Reloading;
        }
    }

    private void StateHandler() 
    {
        float timer = 0;
        if (curState == Gunplay.Reloading)
        {
            timer = reloadTime;
        }
        else if (curState == Gunplay.Firing) 
        {
            timer = fireTime; 
        }
        if (cooldownTime < timer)
        {
            cooldownTime += Time.deltaTime;
        }
        else 
        {
            if (curState == Gunplay.Reloading) 
            {
                this.curAmmo = this.maxAmmo;
                updateAmmoText();
            }
            cooldownTime = 0;

            curState = Gunplay.Readied;
        }
    }


/// <summary>
/// Returns the closest gameobject in the given list of hits, returning null if no collisions are made
/// </summary>
private GameObject inSights(RaycastHit[] hits) 
    {
        GameObject shortestGameObject = null;
        float shortestDistance = float.MaxValue;
        foreach (RaycastHit hit in hits) 
        {
            if (hit.distance < shortestDistance) 
            {
                shortestDistance = hit.distance;
                shortestGameObject = hit.collider.gameObject;
            }
        }
        return shortestGameObject;
    }

    public void updateAmmoText() 
    {
        this.ammoText.text = curAmmo + " / " + maxAmmo;
    }
}
