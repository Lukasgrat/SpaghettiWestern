using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamiteLogic : MonoBehaviour
{
    public ParticleSystem explosionPrefab;
    public float delay = 2f;
    float timer = 0;
    public float explosionRadius = 8;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > delay)
        {
            RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, explosionRadius, transform.forward, .1f);
            foreach (RaycastHit hit in hits) 
            {
                if(hit.collider.gameObject.TryGetComponent(out EnemyImproved enemy)) 
                {
                    enemy.TakeDamage(20);
                }
            }
            ParticleSystem explosion = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("Particles").transform);
            Destroy(explosion.gameObject, 1);
            Destroy(gameObject);
        }
        else 
        {
            timer += Time.deltaTime;
        }
    }
}
