using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public MouseLook Camera;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject gameObject = inSights(Physics.RaycastAll(transform.position, transform.forward, 150));
            if (gameObject != null && gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(10);
            }
            Camera.iniateRecoil();
            this.GetComponent<AudioSource>().Play();
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
