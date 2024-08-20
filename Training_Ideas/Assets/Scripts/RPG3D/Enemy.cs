using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Diagnostics;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Basic Variables")]

    public float totalHealth = 100f;
    public float currentHealth;
    public float speed;

    [Header("Animation variables")]
    private Animator anim;
    private CapsuleCollider capsule;
    private enum State { Idle, Walk, Attack01, Attack02, Hit, Death };
    private State state = State.Idle;

    [Header("Attack variables")]

    public float attackDMG;
    public float lookRadius = 10f;
    public Transform target;
    private NavMeshAgent navAgent;
    public bool attackIsReady;
    public bool isAttacking;
    public float colliderRadius;
    public bool playerIsAlive;

    void Start()
    {
        anim = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider>();
        navAgent = GetComponent<NavMeshAgent>();
        currentHealth = totalHealth;
        playerIsAlive = true;
    }


    void Update()
    {
        anim.SetInteger("state", (int)state);
        if(currentHealth > 0)
        {
            AttackTarget();
        }
    }

    public void AttackTarget()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        
        if(distance <= lookRadius)
        {
            navAgent.isStopped = false;
            if (!anim.GetBool("Attacking"))
            {
                navAgent.SetDestination(target.position);
                state = State.Walk;
                anim.SetBool("Walking", true);
            }

            if (distance <= navAgent.stoppingDistance)
            {
                LookTarget();
                if (!isAttacking) 
                {
                    StartCoroutine("Attack");
                }
            }

            if (distance >= navAgent.stoppingDistance)
            {
                anim.SetBool("Attacking", false);
            }
        }
        else
        {
            state = State.Idle;
            anim.SetBool("Walking", false);
            anim.SetBool("Attacking", false);
            navAgent.isStopped = true;
        }
    }

    void GetEnemy()
    {
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            if (c.gameObject.tag == "Player")
            {
                c.gameObject.GetComponent<Player>().GetHit(attackDMG);
                playerIsAlive = c.gameObject.GetComponent<Player>().isAlive;
            }
        }
    }
    void LookTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookrotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookrotation, Time.deltaTime * 5f); ;
    }
    public void GetHit(float dmg)
    {
        currentHealth -= dmg;

        if (currentHealth > 0)
        {
            StopCoroutine("Attack");
            state = State.Hit;
            anim.SetBool("Hiting", true);
            StartCoroutine(RecoverFromHit());
        }
        else
        {
            Die();
        }
    }

    public void Die()
    {
        if(currentHealth <= 0)
        {
            state = State.Death;
            capsule.enabled = false;
            Destroy(this.gameObject, 10f);
        }
    }

    IEnumerator RecoverFromHit()
    {
        yield return new WaitForSeconds(0.8f);
        state = State.Idle;
        anim.SetBool("Hiting", false);
        attackIsReady = false;
        isAttacking = false;
    }

    IEnumerator Attack()
    {
        if (!attackIsReady && !isAttacking && playerIsAlive && !anim.GetBool("Hiting"))
        {
            isAttacking = true;
            int number = Random.Range(2, 4);
            attackIsReady = true;
            anim.SetBool("Attacking", true);
            anim.SetBool("Walking", false);
            
            switch (number)
            {
                case 2:
                    state = State.Attack01;
                    break;

                case 3:
                    state = State.Attack02;
                    break;

                default:
                    break;
            }
            yield return new WaitForSeconds(0.8f);
            GetEnemy();
            yield return new WaitForSeconds(0.7f);
            anim.SetBool("Attacking", false);
            attackIsReady = false;
            isAttacking = false;
        }

        if (!playerIsAlive)
        {
            state = State.Idle;
            anim.SetBool("Walking", false);
            anim.SetBool("Attacking", false);
            navAgent.isStopped = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
