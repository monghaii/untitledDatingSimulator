using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,       // TODO: for the random classmates just standing around until aggro-ed??
        Patrol,     // TODO: same as ^
        Chase,
        Attack,
        Rage,
        Dead
    }

    [Header("State Machine")]
    public EnemyState currentState = EnemyState.Chase;
    public bool canSeePlayer = true;
    
    [Header("Movement")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask groundMask, playerMask, obstacleMask;
    public float speed;
    public float rageSpeedMultiplier;
    
    [Header("Patrol")]
    public float walkPointRange;
    private Vector3 walkPoint;
    private bool walkPointSet;

    [Header("Attack")] 
    public float meleeRange;
    public float rangedRange;
    public float timeBetweenAttacks;
    private bool alreadyAttacked;
    private bool playerInMeleeRange, playerInRangedRange;
    public float damage = 10f;
    public List<GameObject> projectilePrefabs;
    public GameObject weapon;

    [Header("Health")] 
    public float maxHealth = 100f;
    public float currentHealth;
    public HealthBar healthBar;

    [Header("Sprite")]
    public GameObject enemySprite;

    
    
    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        enemySprite.GetComponent<SpriteRenderer>().sprite = GameObject.Find("GameManager").GetComponent<GameManager>().GetCurrentEnemySprite();
        EnemyData data = GameObject.Find("GameManager").GetComponent<GameManager>().GetCurrentEnemyData();
        meleeRange = data.meleeRange;
        rangedRange = data.rangedRange;
        timeBetweenAttacks = data.timeBetweenAttacks;
        damage = data.damage;
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
        agent.speed = data.moveSpeed;
        projectilePrefabs = data.projectilePrefabs;
        weapon.SetActive(data.weaponEnabled);
    }
    
    void Update()
    {
        if(GameManager.isGamePaused)
        {
            return;
        }
        // TODO: check for rage mode from game manager???
        
        // check if player in sight
        RaycastHit hit;
        Vector3 directionToCast = player.transform.position - transform.position;
        if (Physics.Raycast(transform.position, directionToCast, out hit, Mathf.Infinity, obstacleMask))
        {
            canSeePlayer = false;
        }
        else
        {
            canSeePlayer = true;
        }
        
        // check ranges
        playerInMeleeRange = Physics.CheckSphere(transform.position, meleeRange, playerMask);
        playerInRangedRange = Physics.CheckSphere(transform.position, rangedRange, playerMask);
        if ((playerInMeleeRange || playerInRangedRange) && canSeePlayer)
        {
            currentState = EnemyState.Attack;
        }
        else
        {
            currentState = EnemyState.Chase;
        }
        
        // go through state machine
        switch (currentState)
        {
            case EnemyState.Chase:
            {
                Chase();
                break;
            }
            case EnemyState.Attack:
            {
                Attack();
                break;
            }
            case EnemyState.Rage:
            {
                Rage();
                break;
            }
            case EnemyState.Dead:
            {
                Dead();
                break;
            }
        }

        //update healthBar
        healthBar.SetHealthPercentage(maxHealth, currentHealth);
    }

    /// <summary>
    /// CHASE FUNCTIONS
    /// </summary>
    void Chase()
    {
        agent.SetDestination(player.position);
    }

    /// <summary>
    /// ATTACK FUNCTIONS
    /// </summary>
    void Attack()
    {
        // stop moving when attacking
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            if (playerInMeleeRange)
            {
                MeleeAttack();
            }
            else if (playerInRangedRange)
            {
                RangedAttack();
            }
        }
    }

    // TODO: melee attack
    void MeleeAttack()
    {
        // punch???????????
        
        // reset
        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }
    
    void RangedAttack()
    {
        // choose projectile
        int projectileIndex = Random.Range(0, projectilePrefabs.Count - 1);
        
        // spawn projectile
        GameObject projectile = Instantiate(projectilePrefabs[projectileIndex], transform.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().enemy = this;
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();
        
        // shoot towards player
        projectileRB.AddForce(transform.forward * 32f, ForceMode.Impulse);
        projectileRB.AddForce(transform.up * 8f, ForceMode.Impulse);
        
        // reset
        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    /// <summary>
    /// RAGE FUNCTIONS
    /// </summary>
    void Rage()
    {
        // TODO: boost attack speed, dmg, and movement speed???
        Attack();
    }

    /// <summary>
    /// DEATH FUNCTIONS
    /// </summary>
    void Dead()
    {
        // TODO: tell game manager that enemy is down...
        // TODO: also some kinda death animation
    }
    
    /// <summary>
    /// HEALTH FUNCTIONS
    /// </summary>
    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfx_HurtVocal, this.gameObject);
        if (currentHealth <= 0f)
        {
            currentState = EnemyState.Dead;
        }
    }

    /// <summary>
    /// PATROL FUNCTIONS
    /// </summary>
    void Patrol()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }
        else
        {
            agent.SetDestination(walkPoint);
        }

        // reached target
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    void SearchWalkPoint()
    {
        // calc random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        bool validWalkpoint = Physics.Raycast(walkPoint, -transform.up, 2f, groundMask);
        if (validWalkpoint)
        {
            walkPointSet = true;
        }
    }
}
