using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public enum Gunplay
{
    Reloading,
    Readied,
    Firing,
    Dead,
}

public interface IGUN 
{
    public void ShootingLogic();
    public void Initialize(MouseLook PS, TMP_Text ammotext);

    public void UpdateAmmoText();
}

public class PlayerShooting : MonoBehaviour
{
    public Animator gunAnimator;
    public float coolDownDynamiteTime = 5f;
    public float throwingForce = 100f;
    float cooldownDynamiteTimer = 0f;
    public Gunplay curState;
    public TMP_Text ammoText;
    public GameObject dynamite;
    public Slider dynamiteCooldown;
    IGUN currentGun;
    int currentGunIndex = 0;
    public List<GameObject> availableGuns;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject gun in availableGuns) 
        {
            gun.GetComponent<IGUN>().Initialize(this.GetComponent<MouseLook>(), ammoText);
            gun.SetActive(false);
        }
        availableGuns[currentGunIndex].gameObject.SetActive(true);
        currentGun = availableGuns[currentGunIndex].GetComponent<IGUN>();
        curState = Gunplay.Readied;
        if (ammoText == null)
        {
            throw new System.Exception("Ammo counter is not linked up");
        }
    }

    //Update is called once per frame
    void Update()
    {
        currentGun.ShootingLogic();
        SightHandler();
        ThrowingHandler();
        ShootSwapHandler();
    }

    void ShootSwapHandler() 
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            foreach (GameObject gun in availableGuns) 
            {
                gun.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1)) 
            {
                currentGunIndex = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                currentGunIndex = 1;
            }

            availableGuns[currentGunIndex].SetActive(true);
            currentGun = availableGuns[currentGunIndex].GetComponent<IGUN>();
            currentGun.UpdateAmmoText();
        }
    }

    void ThrowingHandler() 
    {
        if (cooldownDynamiteTimer > 0)
        {
            cooldownDynamiteTimer -= Time.deltaTime;
        }
        else if(Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
        {
            GameObject newDynamite = Instantiate(dynamite, this.transform.position + transform.forward.normalized * .25f, this.transform.rotation, GameObject.FindGameObjectWithTag("Particles").transform);
            newDynamite.GetComponent<Rigidbody>().AddForce(
                Quaternion.AngleAxis(0, Vector3.left) * transform.forward * throwingForce, ForceMode.Force);
            cooldownDynamiteTimer = coolDownDynamiteTime;
            newDynamite.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.right * 20);
        }
        dynamiteCooldown.value = cooldownDynamiteTimer / coolDownDynamiteTime;

    }

    void SightHandler()
    {
        GameObject sightedObject = inSights(Physics.RaycastAll(transform.position, transform.forward, 600));
        if (sightedObject != null && sightedObject.TryGetComponent(out EnemyImproved target))
        {
            target.HealthDisplay();
        }
        else if (sightedObject != null && sightedObject.TryGetComponent(out EnemyHead head))
        {
            head.enemy.HealthDisplay();
        }
        else
        {
            GameObject.FindGameObjectWithTag("EnemyDisplayName").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.FindGameObjectWithTag("EnemyHealthSlider").GetComponent<CanvasGroup>().alpha = 0;
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
}


