using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyImproved : MonoBehaviour
{

    public enum FSMStates 
    {
        idle,
        patrol,
        shooting,
        dead,
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
    public FSMStates currentState;
    public GameObject[] wanderPoints;

    Animator playerAnimator;
    Vector3 nextDestination;
    int currentDestinationIndex = 0;
    float distanceToPlayer;


    // Start is called before the first frame update
    void Start()
    {
        curhealth = maxHealth;
        //currentState = FSMStates.idle;
        player = GameObject.FindGameObjectWithTag("Player");
        if (wanderPoints.Length > 1) 
        {
            currentState = FSMStates.patrol;
            FindNextPoint();
        }
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        switch(currentState)
        {
            case FSMStates.idle:
                UpdateIdleState();
                break;
            case FSMStates.patrol:
                UpdatePatrolState();
                break;
            case FSMStates.shooting:
                UpdateShootState();
                break;
            case FSMStates.dead:
                UpdateDeadState();
                break;
        }
        
        if (curhealth <= 0)
        {
            currentState = FSMStates.dead;
        }

    }

    void UpdateIdleState()
    {
        playerAnimator.SetInteger("animState", 0);
        if (distanceToPlayer <= seeingRadius && InSights(Physics.RaycastAll(head.transform.position,
            player.transform.position - head.transform.position)).CompareTag("Player") && canShoot)
        {
            currentState = FSMStates.shooting;
        }
    }


    void UpdatePatrolState()
    {

        playerAnimator.SetInteger("animState", 1);

        if(Vector3.Distance(transform.position, nextDestination) < 3)
        {
            FindNextPoint();
        }
        else if (distanceToPlayer <= seeingRadius && InSights(Physics.RaycastAll(head.transform.position, player.transform.position)).CompareTag("Player") && canShoot) 
        {
            currentState = FSMStates.shooting;
        }

        FaceTarget(nextDestination);

        //transform.position = Vector3.MoveTowards(transform.position, nextDestination, 1.5f * Time.deltaTime);

    }

    void UpdateShootState()
    {


        playerAnimator.SetInteger("animState", 5);
        var playerPos = player.transform.position;
        playerPos.y = transform.position.y;
        transform.LookAt(playerPos);
        transform.Rotate(0, 30, 0);

        if (head != null)
        {
            head.transform.LookAt(player.transform.position);
        }

        if ((Vector3.Distance(player.transform.position, transform.position) > seeingRadius
            && !InSights(Physics.RaycastAll(head.transform.position, head.transform.forward, hearingRadius)).CompareTag("Player"))) 
        {
            if (this.wanderPoints.Length > 1)
            {
                currentState = FSMStates.patrol;
            }
            else 
            {
                currentState = FSMStates.idle;
            }
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

    void UpdateDeadState()
    {
        isDead = true;
        if (playerAnimator.GetInteger("animState") != 2)
        {
            playerAnimator.SetInteger("animState", 2);
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    void FindNextPoint()
    {
        nextDestination = wanderPoints[currentDestinationIndex].transform.position;

        currentDestinationIndex = (currentDestinationIndex + 1) % wanderPoints.Length;
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;

        directionToTarget.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10 * Time.deltaTime);
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
            curhealth = 0;
            //this.transform.Rotate(-30f, 0f, 0f);
            //Rigidbody rb = this.GetComponent<Rigidbody>();
            //rb.constraints -= RigidbodyConstraints.FreezeRotationX;
            //rb.constraints -= RigidbodyConstraints.FreezeRotationZ;
            //rb.AddForce((transform.forward + Vector3.down) * -500);
            UpdateDeadState();
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
            if (hit.distance < shortestDistance && hit.collider.gameObject.tag != this.tag)
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
        if (currentState == FSMStates.shooting || isDead) return;

        if (Vector3.Distance(player.transform.position, transform.position) <= hearingRadius) 
        {
            currentState = FSMStates.shooting;
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
