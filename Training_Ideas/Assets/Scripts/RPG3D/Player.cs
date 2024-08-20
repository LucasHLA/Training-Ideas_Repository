using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private enum State { Idle, Walk, Attack01, Run, Attack02, Defend, Hit, Death };
    private State state = State.Idle;

    [Header ("Logical Booleans")]
    public bool attackIsReady;
    public bool isRunning;
    public bool isAlive;

    [Header ("Attack Variables")]
    List<Transform> EnemiesList = new List<Transform>();
    public float colliderRadius;
    public float damage = 25f;
    public List<GameObject> weaponList = new List<GameObject>();

    [Header ("Life variables")]
    public float totalHealth = 100f;
    public float currentHealth;

    void Start()
    {
        charController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        originalSpeed = speed;
        currentHealth = totalHealth;
        isAlive = true;
    }

    
    void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == null)
        {
            Move();
            GetMouseInput();
        }
        
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
                    StartCoroutine("Attack");
                    StartCoroutine("secondAttack");
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
                    StartCoroutine("Attack");
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
                    StartCoroutine("secondAttack");
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
    IEnumerator Attack()
    {
        if (!attackIsReady && !anim.GetBool("Hiting") && !anim.GetBool("Defending"))
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
            state =State.Idle;
            anim.SetBool("Attacking", false);
            attackIsReady = false;
        }

    }

    IEnumerator secondAttack()
    {
        if (!attackIsReady && !anim.GetBool("Hiting") && !anim.GetBool("Defending"))
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
            state = State.Idle;
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

    public void GetHit(float dmg)
    {
        if (currentHealth > 0)
        {
            if (!anim.GetBool("Defending"))
            {
                currentHealth -= dmg;
            }
            else
            {
                currentHealth -= dmg - (dmg / 2);
            }

            StopCoroutine("Attack");
            StopCoroutine("secondAttack");
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
        if (currentHealth <= 0)
        {
            state = State.Death;
            isAlive = false;
            //capsule.enabled = false;
            //Destroy(this.gameObject, 10f);
        }
    }

    IEnumerator RecoverFromHit()
    {
        yield return new WaitForSeconds(0.7f);
        state = State.Idle;
        anim.SetBool("Hiting", false);
        attackIsReady = false;
        anim.SetBool("Attacking", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, colliderRadius);
    }

    public void IncreaseStats(float health, float increaseSpeed, float dmg)
    {
        currentHealth += health;
        speed += increaseSpeed;
        damage += dmg;
    }

    public void DecreaseStats(float health, float increaseSpeed, float dmg)
    {
        currentHealth -= health;
        speed -= increaseSpeed;
        damage -= dmg;
    }

    public void EquipWeapon(int weaponNumber)
    {
        switch(weaponNumber)
        {
            case 0:
                weaponList[0].SetActive(true);
                weaponList[1].SetActive(false);
                weaponList[2].SetActive(false);
                break;

            case 1:
                weaponList[0].SetActive(false);
                weaponList[1].SetActive(true);
                weaponList[2].SetActive(false);
                break;

            case 2:
                weaponList[0].SetActive(false);
                weaponList[1].SetActive(false);
                weaponList[2].SetActive(true);
                break;

            default:
                weaponList[0].SetActive(true);
                weaponList[1].SetActive(false);
                weaponList[2].SetActive(false);
                break;
        }
    }
}
