using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Basic Variables")]

    public float totalHealth = 100f;
    public float currentHealth;
    public float attackDMG;
    public float speed;

    [Header("Animation variables")]
    private Animator anim;
    private CapsuleCollider capsule;
    private enum State { Idle, Walk, Attack01, Attack02, Hit, Death };
    private State state = State.Idle;

    void Start()
    {
        anim = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider>();
        currentHealth = totalHealth;
    }


    void Update()
    {
        anim.SetInteger("state", (int)state);
    }

    public void GetHit(float dmg)
    {
        currentHealth -= dmg;

        if (currentHealth > 0)
        {
            state = State.Hit;
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
        yield return new WaitForSeconds(0.5f);
        state = State.Idle;
    }
}
