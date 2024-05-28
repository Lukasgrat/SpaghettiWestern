using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shootProjectile : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 100;
    public AudioClip fireSFX;
    public Image recticleImage;
    public Color reticleEnemyColor;

    Color originalReticleColor;
    GameObject currentProjectilePrefab;


    // Start is called before the first frame update
    void Start()
    {
        originalReticleColor = recticleImage.color;
        currentProjectilePrefab = projectilePrefab;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && GameObject.FindGameObjectWithTag("Weapon").activeInHierarchy)
        {
            GameObject projectile = Instantiate(currentProjectilePrefab, transform.position + transform.forward, transform.rotation) as GameObject;

            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);

            projectile.transform.SetParent(
                GameObject.FindGameObjectWithTag("ProjectileParent").transform);

            AudioSource.PlayClipAtPoint(fireSFX, transform.position);
        }
    }

    private void FixedUpdate() 
    {
        ReticleEffect();
    }

    void ReticleEffect() 
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            if(hit.collider.CompareTag("Enemy") && GameObject.FindGameObjectWithTag("Weapon").activeInHierarchy)
            {
                currentProjectilePrefab = projectilePrefab;

                recticleImage.color = Color.Lerp(recticleImage.color, reticleEnemyColor, Time.deltaTime * 2);
                recticleImage.transform.localScale = Vector3.Lerp(recticleImage.transform.localScale, new Vector3(1f, 1f, 1), Time.deltaTime * 2);
            }
            else 
            {
                currentProjectilePrefab = projectilePrefab;

                recticleImage.color = Color.Lerp(recticleImage.color, originalReticleColor, Time.deltaTime * 2);
                recticleImage.transform.localScale = Vector3.Lerp(recticleImage.transform.localScale, new Vector3(0.5f, 0.5f, 0.5f), Time.deltaTime * 2);
            }
        }
        else 
        {
            recticleImage.color = Color.Lerp(recticleImage.color, originalReticleColor, Time.deltaTime * 2);
                recticleImage.transform.localScale = Vector3.Lerp(recticleImage.transform.localScale, new Vector3(0.5f, 0.5f, 0.5f), Time.deltaTime * 2);
        }
        
    }
}
