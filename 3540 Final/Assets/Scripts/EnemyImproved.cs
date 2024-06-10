using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyImproved : MonoBehaviour
{

    enum EnemyState 
    {
        idle,
        shooting,
    }


    GameObject player;
    public int maxHealth = 30;
    int curhealth = 30;
    bool isDead = false;
    public bool canShoot = false; // New variable to control shooting
    public float shootingCooldown = 1;
    float shootingTime = 0;
    public float seeingRadius = 5;
    public float hearingRadius = 10;
    public Transform head;
    public string displayName;
    EnemyState enemyState;


    // Start is called before the first frame update
    void Start()
    {
        curhealth = maxHealth;
        enemyState = EnemyState.idle;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= seeingRadius) 
        {
            enemyState = EnemyState.shooting;
        }
        if (enemyState == EnemyState.idle) return;
        if (isDead) { return; }
        
        var playerPos = player.transform.position;
        playerPos.y = transform.position.y;
        transform.LookAt(playerPos);

        if (head != null)
        {
            head.transform.LookAt(player.transform.position);
        }

        RaycastHit[] hits;
        if (head != null)
        {
            hits = Physics.RaycastAll(head.transform.position, head.transform.forward, 50);
        }
        else
        {
            hits = Physics.RaycastAll(transform.position, transform.forward, 50);
        }

        if (canShoot && InSights(hits) == player)
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
            if (shootingTime > shootingCooldown / 2)
            {
                shootingTime -= Time.deltaTime;
            }
            else if (shootingTime < shootingCooldown / 2)
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

        curhealth -= damage;
        if (!isDead && curhealth <= 0)
        {
            GameObject.FindGameObjectWithTag("EnemyDisplayName").GetComponent<CanvasGroup>().alpha = 0;
            GameObject.FindGameObjectWithTag("EnemyHealthSlider").GetComponent<CanvasGroup>().alpha = 0;
            curhealth = 0;
            this.transform.Rotate(-30f, 0f, 0f);
            Rigidbody rb = this.GetComponent<Rigidbody>();
            rb.constraints -= RigidbodyConstraints.FreezeRotationX;
            rb.constraints -= RigidbodyConstraints.FreezeRotationZ;
            rb.AddForce((transform.forward + Vector3.down) * -500);
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

        if (Random.Range(0, 5) < 2)
        {
            player.GetComponent<PlayerController>().TakeDamage(15);
        }

        shootingTime = shootingCooldown;
    }

    public void OnPlayerFire() 
    {
        if (enemyState == EnemyState.shooting) return;

        if (Vector3.Distance(player.transform.position, transform.position) <= hearingRadius) 
        {
            enemyState = EnemyState.shooting;
        }
    }

    public void HealthDisplay() 
    {
        if (isDead) return;
        Slider slider = GameObject.FindGameObjectWithTag("EnemyHealthSlider").GetComponent<Slider>();
        slider.gameObject.GetComponent<CanvasGroup>().alpha = 1;
        slider.value = curhealth / (float) maxHealth;
        TMP_Text text = GameObject.FindGameObjectWithTag("EnemyDisplayName").GetComponent<TMP_Text>();
        text.gameObject.GetComponent<CanvasGroup>().alpha = 1;
        text.text = displayName;
    }
}
