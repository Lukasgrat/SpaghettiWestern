using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameObject player;
    public int health = 30;
    bool isDead = false;
    public float shootingCooldown = 1;
    float shootingTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) { return; }
        var playerPos = player.transform.position;
        playerPos.y = transform.position.y;
        transform.LookAt(playerPos);
        if (InSights(Physics.RaycastAll(transform.position, transform.forward, 20)) == player)
        {
            if (shootingTime <= 0) 
            {
                ShootPlayer();
            }
            if (shootingTime > 0)
            {
                shootingTime -= Time.deltaTime;
            }
        }
        else
        {
            if (shootingTime > shootingCooldown / 4)
            {
                shootingTime -= Time.deltaTime;
            }
            else if(shootingTime < shootingCooldown / 4) 
            {
                shootingTime += Time.deltaTime / 2;
            }
        }
    }

    /// <summary>
    /// Receives the given amount of damage. If the enemy's health goes below 0, it will fling itself backwards and be considered "dead"
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage) 
    {
        if (isDead) { return; }
        health -= damage;
        if (!isDead && health < 0) 
        {
            health = 0;
            this.transform.Rotate(-30f, 0f, 0f);
            this.GetComponent<Rigidbody>().AddForce((transform.forward + Vector3.down) * -500);
            isDead = true;
        }
    }

    /// <summary>
    /// Returns the closest gameobject in the given list of hits, returning null if no collisions are made
    /// </summary>
    private GameObject InSights(RaycastHit[] hits)
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

    private void ShootPlayer() 
    {

        this.GetComponent<AudioSource>().Play();
        if (Random.Range(0, 5) < 3)
        {
            player.GetComponent<PlayerController>().TakeDamage(20);
        }
        shootingTime = shootingCooldown;
    }
}
