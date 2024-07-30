using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header ("Basic Moviment variables")]
    public float speed;
    public float originalSpeed;
    public float rotSpeed;
    private float rotation;
    public float gravity;
    Vector3 moveDirection;

    CharacterController charController;

    [Header("Animation Variables")]
    Animator anim;
    private enum State { Idle, Walk, Attack01, Run, Attack02, Defend };
    private State state = State.Idle;

    [Header ("Logical Booleans")]
    public bool attackIsReady;
    public bool isRunning;

    [Header ("Attack Related")]
    List<Transform> EnemiesList = new List<Transform>();
    public float colliderRadius;
    public float damage = 25f;


    void Start()
    {
        charController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        originalSpeed = speed;
    }

    
    void Update()
    {
        Move();
        GetMouseInput();
        anim.SetInteger("state", (int)state);
    }

    public void Move()
    {
        if (charController.isGrounded)
        {
            if (Input.GetKey(KeyCode.W))
            {
                if (!anim.GetBool("Attacking") && !anim.GetBool("Defending"))
                {
                    anim.SetBool("Walking", true);
                    state = State.Walk;
                    moveDirection = Vector3.forward * speed;
                    moveDirection = transform.TransformDirection(moveDirection);

                    if(Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                    {
                        state = State.Run;
                        if (!isRunning)
                        {
                            originalSpeed = speed;
                            isRunning = true;
                            speed = speed * 2;
                        }
                    }

                    if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
                    {
                        isRunning = false;
                        state = State.Walk;
                        speed = originalSpeed;
                    }
                }
                else
                {
                    anim.SetBool("Walking", false);
                    moveDirection = Vector3.zero;
                    StartCoroutine(Attack((int)state));
                    StartCoroutine(secondAttack((int)state));
                }
            }

            if(Input.GetKeyUp (KeyCode.W))
            {
                anim.SetBool("Walking", false);
                state = State.Idle;
                moveDirection = Vector3.zero;
            }
        }

        rotation += Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, rotation, 0);

        moveDirection.y -= gravity * Time.deltaTime;

        charController.Move(moveDirection * Time.deltaTime);
    }

    void GetMouseInput()
    {
        if (charController.isGrounded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if(anim.GetBool("Walking"))
                {
                    anim.SetBool("Walking", false);
                    state = State.Idle;
                }

                if (!anim.GetBool("Walking"))
                {
                    StartCoroutine(Attack((int)state));
                }
            }

            if(Input.GetMouseButtonDown(1))
            {
                if (anim.GetBool("Walking"))
                {
                    anim.SetBool("Walking", false);
                    state = State.Idle;
                }

                if (!anim.GetBool("Walking"))
                {
                    StartCoroutine(secondAttack((int)state));
                }
            }

            if (Input.GetMouseButton(2))
            {
                if (anim.GetBool("Walking") /*|| anim.GetBool("Running")*/)
                {
                    anim.SetBool("Walking", false);
                    state = State.Idle;
                }

                if (!anim.GetBool("Walking"))
                {
                    anim.SetBool("Defending", true);
                    state = State.Defend;
                }
            }

            if (Input.GetMouseButtonUp(2))
            {
                anim.SetBool("Defending", false);
                state = State.Idle;
            }
        }
    }
    IEnumerator Attack(int stateValue)
    {
        if (!attackIsReady)
        {
            attackIsReady = true;
            anim.SetBool("Attacking", true);
            state = State.Attack01;
            yield return new WaitForSeconds(0.4f);

            GetEnemiesInRange();

            foreach (Transform enemies in EnemiesList)
            {
                Enemy enemy = enemies.GetComponent<Enemy>();

                if (enemy != null)
                {
                    enemy.GetHit(damage);
                }
            }


            yield return new WaitForSeconds(0.34f);
            state = (State)stateValue;
            anim.SetBool("Attacking", false);
            attackIsReady = false;
        }
    }

    IEnumerator secondAttack(int stateValue)
    {
        if (!attackIsReady)
        {
            attackIsReady = true;
            anim.SetBool("Attacking", true);
            state = State.Attack02;
            yield return new WaitForSeconds(0.4f);

            GetEnemiesInRange();

            foreach (Transform enemies in EnemiesList)
            {
                Enemy enemy = enemies.GetComponent<Enemy>();

                if (enemy != null)
                {
                    enemy.GetHit(damage * 2);
                }
            }


            yield return new WaitForSeconds(0.34f);
            state = (State)stateValue;
            anim.SetBool("Attacking", false);
            yield return new WaitForSeconds(1f);
            attackIsReady = false;
        }
    }

    void GetEnemiesInRange()
    {
        EnemiesList.Clear();
        foreach(Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            if(c.gameObject.tag == "Enemy")
            {
                EnemiesList.Add(c.transform);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, colliderRadius);
    }
}
