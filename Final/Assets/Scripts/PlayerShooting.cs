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
    MouseLook Camera;
    public Animator gunAnimator;
    public int curAmmo;
    public int maxAmmo = 6;
    public AudioClip reloadingSFX;
    public AudioClip shootingSFX;
    AudioSource audiosource;
    public float reloadTime = 3.1f;
    public float fireTime = .36f;
    float cooldownTime = 0;
    public Gunplay curState;
    public TMP_Text ammoText;

    // Start is called before the first frame update
    void Start()
    {
        Camera = GetComponent<MouseLook>();

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
        SightHandler();
    }

    void SightHandler()
    {
        GameObject sightedObject = inSights(Physics.RaycastAll(transform.position, transform.forward, 600));
        if (sightedObject != null && sightedObject.TryGetComponent(out EnemyImproved target))
        {
            target.HealthDisplay();
        }
        else
        {
            GameObject.FindGameObjectWithTag("EnemyDisplayName").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.FindGameObjectWithTag("EnemyHealthSlider").GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    private void InputHandler()
    {
        if (curState != Gunplay.Readied)
        {
            return;
        }


        if (Input.GetButtonDown("Fire1") && curAmmo > 0)
        {
            gunAnimator.gameObject.transform.localEulerAngles = new Vector3(0, -5, 0);
            gunAnimator.SetInteger("animState", 1);
            curState = Gunplay.Firing;
            StartCoroutine(shootingEffects());
        }
        if (Input.GetKeyDown(KeyCode.R) || (curAmmo == 0 && Input.GetButtonDown("Fire1")))
        {
            gunAnimator.gameObject.transform.localEulerAngles = new Vector3(0, -5, 0);
            curState = Gunplay.Reloading;
            audiosource.clip = reloadingSFX;
            audiosource.Play();
            gunAnimator.SetInteger("animState", 2);
        }
    }

    private void StateHandler()
    {
        if (curState == Gunplay.Readied) return;
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
            gunAnimator.SetInteger("animState", 0);
            gunAnimator.gameObject.transform.localPosition = Vector3.zero;
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

    private IEnumerator shootingEffects()
    {
        yield return new WaitForSeconds(.05f);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObj in enemies)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(10);
                enemy.canShoot = true; // Modify the canShoot property of the enemy script
            }

            EnemyImproved EI = enemyObj.GetComponent<EnemyImproved>();
            if (EI != null)
            {
                EI.OnPlayerFire();
            }
        }
        if (inSights(Physics.RaycastAll(transform.position, transform.forward, 600)).TryGetComponent(out EnemyImproved target))
        {
            target.TakeDamage(10);
        }
        curAmmo -= 1;
        updateAmmoText();
        FindObjectOfType<ReticleLogic>().InitiateReticle(fireTime);
        Camera.iniateRecoil();
        audiosource.clip = shootingSFX;
        audiosource.Play();

    }
}

