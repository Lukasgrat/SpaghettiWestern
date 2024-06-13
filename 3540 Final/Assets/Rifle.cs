using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Rifle : MonoBehaviour, IGUN
{
    public AudioClip reloadingSFX;
    Animator gunAnimator;
    public AudioClip shootingSFX;
    AudioSource shootingSFXSource;
    public float reloadTime = 3f;
    public float fireTime = .75f;
    float coolDownTime = 0f;
    public Gunplay curState;
    public int maxAmmo = 5;
    int curAmmo;
    TMP_Text ammoText;
    MouseLook playerHead;
    // Start is called before the first frame update
    void Start()
    {
        curAmmo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        gunAnimator = transform.GetChild(0).GetComponent<Animator>();
        shootingSFXSource = GetComponent<AudioSource>();
    }

    public void Initialize(MouseLook PS, TMP_Text ammotext)
    {
        playerHead = PS;
        this.ammoText = ammotext;
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
        if (coolDownTime < timer)
        {
            coolDownTime += Time.deltaTime;
        }
        else
        {
            if (curState == Gunplay.Reloading)
            {
                this.curAmmo = this.maxAmmo;
                UpdateAmmoText();
            }
            coolDownTime = 0;
            curState = Gunplay.Readied;
            gunAnimator.SetInteger("animState", 0);
            gunAnimator.gameObject.transform.localPosition = Vector3.zero;
        }
    }

    public void ShootingLogic()
    {
        StateHandler();
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
            shootingSFXSource.clip = reloadingSFX;
            shootingSFXSource.Play();
            gunAnimator.SetInteger("animState", 2);
        }
    }

    public void UpdateAmmoText()
    {
        this.ammoText.text = curAmmo + " / " + maxAmmo;
    }

    private IEnumerator shootingEffects()
    {
        yield return new WaitForSeconds(.1333f);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObj in enemies)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(5);
                enemy.canShoot = true; // Modify the canShoot property of the enemy script
            }

            EnemyImproved EI = enemyObj.GetComponent<EnemyImproved>();
            if (EI != null)
            {
                EI.OnPlayerFire();
            }
        }
        RaycastHit[] hits = Physics.RaycastAll(playerHead.transform.position,
            playerHead.transform.forward, 600);
        GameObject sightedObject = playerHead.inSights(hits);
        if (sightedObject.TryGetComponent(out EnemyImproved target))
        {
            target.TakeDamage(5);
        }
        if (sightedObject.TryGetComponent(out EnemyHead enemyHead))
        {
            enemyHead.enemy.TakeDamage(35);
        }
        else if (sightedObject.TryGetComponent(out DynamiteLogic DL))
        {
            DL.Explode();
        }
        curAmmo -= 1;
        UpdateAmmoText();
        FindObjectOfType<ReticleLogic>().InitiateReticle(fireTime);
        playerHead.iniateRecoil();
        shootingSFXSource.clip = shootingSFX;
        shootingSFXSource.Play();

    }
}
