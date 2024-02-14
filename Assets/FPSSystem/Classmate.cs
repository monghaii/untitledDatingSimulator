using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Classmate : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask groundMask, playerMask;
    public Damageable health;
    public GameObject winState;
    public GameManager gameManager;
    public Slider healthBar;
    
    // patrolling
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;
    
    // attacking
    public float timeBetweenAttacks;
    public GameObject projectilePrefab;
    private bool alreadyAttacked;
    
    // states
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    
    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        healthBar.maxValue = health.maxHealth;
        healthBar.value = health.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (health.isDead && !winState.activeSelf)
        {
            gameManager.EndFPS();
            Cursor.lockState = CursorLockMode.None;
            winState.SetActive(true);
            gameObject.SetActive(false);
            return;
        }

        if (health.isDead)
            return;

        healthBar.value = health.currentHealth;
        
        // check if player in range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);


        if (!playerInAttackRange)
        {
            Chasing();
        }
        else if (playerInAttackRange)
        {
            Attacking();
        }
    }

    private void SearchWalkPoint()
    {
        // calc random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundMask))
        {
            walkPointSet = true;
        }

    }

    private void Patrolling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }
        else
        {
            agent.SetDestination(walkPoint);
        }
        
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void Chasing()
    {
        agent.SetDestination(player.position);
    }

    private void Attacking()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();
            projectileRB.AddForce(transform.forward * 32f, ForceMode.Impulse);
            projectileRB.AddForce(transform.up * 8f, ForceMode.Impulse);
            Destroy(projectile, 2f);
            
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
