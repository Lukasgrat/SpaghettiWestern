using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;


public class Level1Enemy : MonoBehaviour
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
    public bool isDead = false;
    public bool canShoot = false; // New variable to control shooting
    public float shootingCooldown = 1;
    float shootingTime = 0;
    public float seeingRadius = 5;
    public float hearingRadius = 10;
    public float fieldOfView = 45f;
    public Transform head;
    public string displayName;
    public FSMStates currentState = FSMStates.idle;
    public GameObject[] wanderPoints;
    public GameObject gun;
    //Due to additive scenes causing the start function to go horribly wrong, forced to write 
    // a manual start
    bool hasStarted = false;

    NavMeshAgent agent;

    Animator playerAnimator;
    Vector3 nextDestination;
    int currentDestinationIndex = 0;
    float distanceToPlayer;

    static bool cardGamePlayed = false; // Flag to track card game completion
    static bool playerWonCardGame = false; // Flag to track card game winner


    // Start is called before the first frame update
    void Start()
    {
        curhealth = maxHealth;
        hasStarted = true;
        //currentState = FSMStates.idle;
        player = GameObject.FindGameObjectWithTag("Player");
        if (wanderPoints.Length > 1)
        {
            agent = GetComponent<NavMeshAgent>();
            currentState = FSMStates.patrol;
            FindNextPoint();
        }
        else 
        {
            TryGetComponent<NavMeshAgent>(out NavMeshAgent nav);
            if (nav != null)
            {
                nav.enabled = false;
            }
        }
        playerAnimator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!cardGamePlayed) // Don't process enemy states until card game is played
            return;

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
        Debug.Log(playerAnimator);
        playerAnimator.SetInteger("animState", 0);
        gun.SetActive(false);
        if (distanceToPlayer <= seeingRadius && InSights(Physics.RaycastAll(head.transform.position,
            player.transform.position - head.transform.position)).CompareTag("Player")&& !playerWonCardGame)
        {
            currentState = FSMStates.shooting;
        }
    }


    void UpdatePatrolState()
    {
        playerAnimator.SetInteger("animState", 1);
        gun.SetActive(false);

        if(Vector3.Distance(transform.position, nextDestination) < 3)
        {
            FindNextPoint();
        }
        else if (distanceToPlayer <= seeingRadius 
        //&& InSights(Physics.RaycastAll(head.transform.position, player.transform.position)).CompareTag("Player") 
        && IsPlayerInClearFOV()) 
        {
            currentState = FSMStates.shooting;
        }

        FaceTarget(nextDestination);
        agent.SetDestination(nextDestination);
        //transform.position = Vector3.MoveTowards(transform.position, nextDestination, 1.5f * Time.deltaTime);

    }

    // Updates Shooting FSM State
    void UpdateShootState()
    {

        gun.SetActive(true);
        playerAnimator.SetInteger("animState", 5);
        var playerPos = player.transform.position;
        playerPos.y = transform.position.y;
        transform.LookAt(playerPos);
        transform.Rotate(0, 30, 0);

        if (head != null)
        {
            head.transform.LookAt(player.transform.position);
        }

        if (Vector3.Distance(player.transform.position, transform.position) > seeingRadius
            && !IsPlayerInClearFOV()
            //!InSights(Physics.RaycastAll(head.transform.position, head.transform.forward, hearingRadius)).CompareTag("Player")) 
        )
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

    // Updates Dead FSM State
    void UpdateDeadState()
    {
        isDead = true;
        if (playerAnimator.GetInteger("animState") != 2)
        {
            playerAnimator.SetInteger("animState", 2);
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            FindAnyObjectByType<EnemyManager>().enemyDied();
        }
    }

    // Sets the enemy's destination to be the next wanderpoint in the array
    void FindNextPoint()
    {
        nextDestination = wanderPoints[currentDestinationIndex].transform.position;

        currentDestinationIndex = (currentDestinationIndex + 1) % wanderPoints.Length;

        agent.SetDestination(nextDestination);
    }

    // Rotates the Enemy to face the given target
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
            FaceTarget(player.transform.position);
            if (InSights(Physics.RaycastAll(transform.position, transform.forward,hearingRadius)).TryGetComponent(out PlayerController PC))
            {
                currentState = FSMStates.shooting;
            }
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

    bool IsPlayerInClearFOV()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = player.transform.position - head.position;
        
        if (Vector3.Angle(directionToPlayer, head.forward) <= fieldOfView)
        {
            if(Physics.Raycast(head.position, directionToPlayer, out hit, seeingRadius))
            {
                if(hit.collider.CompareTag("Player"))
                {
                    print("Player in sight");
                    return true;
                }

                return false;
            }
            return false;
        }
        return false;
    }

    public static void SetCardGamePlayed(bool win) // Function to signal the card game is played and result (win/lose)
    {
        cardGamePlayed = true;
        playerWonCardGame = win;
    }
}
