using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointZombieAI : MonoBehaviour
{
    public enum ZombieState { Walk, Chase, Attack, Dead }
    public Animator animator;
    public ZombieState currentState = ZombieState.Walk;
    public Transform player;
    public float chaseDistance = 10f;
    public float attackDistance = 2f;
    public float attackCooldown = 2f;
    public float attackDelay = 1.5f;
    public NavMeshAgent navAgent;
    private bool isAttacking;
    private bool isChasing;
    private float lastAttackTime;
    public int damage = 10;
    public int health = 100;
    private CapsuleCollider capsuleCollider;
    public GameObject BloodScreen;
    private GameObject instantiatedObject;

    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;

        }
        else
        {
            Debug.Log("Player object not found!");
        }
        capsuleCollider = GetComponent<CapsuleCollider>();
        navAgent = GetComponent<NavMeshAgent>();
        lastAttackTime = -attackCooldown;
        animator = GetComponent<Animator>();

    }

     void Update()
    {
      switch(currentState)
        {
            case ZombieState.Walk:
                if (!isChasing || navAgent.remainingDistance < 0.1f) {
                    Walk();
                }
                if (IsPlayerInRange(chaseDistance))
                {
                    currentState = ZombieState.Chase;
                }
                break;

            case ZombieState.Chase:
                ChasePlayer();
                if (IsPlayerInRange(attackDistance))
                {
                    currentState = ZombieState.Attack;

                }
                break;

            case ZombieState.Attack:
               
                AttackPlayer();
                if (IsPlayerInRange(attackDistance))
                {
                    currentState = ZombieState.Chase;
                }
                break;

            case ZombieState.Dead:
                animator.SetBool("isChasing", false);
                animator.SetBool("IsAttacking", false);
                animator.SetBool("IsDead", true);
                navAgent.enabled = false;
                capsuleCollider.enabled = false;
                enabled = false;

                Debug.Log("Dead");

                break;
        }  
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.position) <= range; 
    }

    private void Walk()
    {
        navAgent.speed = 0.3f;
        Vector3 randomposition = RandomNavMeshPosition();
        navAgent.SetDestination(randomposition);
        isChasing = true;
    }

    private Vector3 RandomNavMeshPosition()
    {
        Vector3 randomdirection = Random.insideUnitSphere * 10f;
        randomdirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomdirection, out hit, 10f, NavMesh.AllAreas);
        return hit.position;
    }

    private void ChasePlayer()
    {
        animator.SetBool("isChasing", true);
        animator.SetBool("IsAttacking", false);
        navAgent.speed = 3f;
        navAgent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        animator.SetBool("isChasing", false);
        animator.SetBool("IsAttacking", true);
        navAgent.SetDestination(player.position);
        if(!isAttacking || Time.time - lastAttackTime >= attackCooldown)
        {
            StartCoroutine(AttackWithDelay());
            StartCoroutine(ActivateBloodScreenEffect());
            lastAttackTime = Time.time;

            Movement playerMovement = player.GetComponent<Movement>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(damage);
            }
        }
    }

    private IEnumerator AttackWithDelay()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
    }

    private IEnumerator ActivateBloodScreenEffect()
    {
        InstantiateObject();
        yield return new WaitForSeconds(attackDelay);
        DeleteObject();
    }

    void InstantiateObject()
    {
        instantiatedObject = Instantiate(BloodScreen);
    }

    void DeleteObject()
    {
        if (instantiatedObject != null)
        {
            Destroy(instantiatedObject);

            instantiatedObject = null;
        }

    }



    public void TakeDamage(int damageAmount)
    {
        if (currentState == ZombieState.Dead)
        {
            return;
        }
        health -= damageAmount;
        if (health <= 0)
        {
            health = 0;
            Die();

        }
    }

    private void Die()
    {
        currentState = ZombieState.Dead;

    }
}

