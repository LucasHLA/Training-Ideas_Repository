using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("Gate Opening Variables")]
    public float colliderRadius;
    public bool isOpen;
    public Animator anim;
    public List<BoxCollider> colliders = new List<BoxCollider>();
    [Header("Gate Canvas Variables")]
    public GameObject E;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        GetPlayer();
    }

    void GetPlayer()
    {
        if (!isOpen)
        {
            foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
            {
                if (c.gameObject.tag == "Player")
                {
                    E.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        OpenGate();
                        E.SetActive(false);
                    }
                }
                else
                {
                    E.SetActive(false);
                }
            }
        }
        else
        {
            E.SetActive(false);
        }
    }

    void OpenGate()
    {
        anim.SetBool("DoorOpen", true);
        isOpen = true;
        StartCoroutine(DisableDoorColliders());
    }

    IEnumerator DisableDoorColliders()
    {
        yield return new WaitForSeconds(1.5f);
        foreach(BoxCollider BC in colliders)
        {
            BC.enabled = false;
        }
    }
}
