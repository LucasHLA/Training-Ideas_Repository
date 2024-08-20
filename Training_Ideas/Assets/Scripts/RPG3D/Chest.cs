using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator anim;

    [Header("Chest opening variables")]
    public float colliderRadius;
    public bool isOpen;

    public List<Item> items = new List<Item>();
    
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
                    if (Input.GetKeyDown(KeyCode.E))
                        OpenChest();
                }
            }
        }
    }

    //executa quando pega os iten de dentro do baú
    void OpenChest()
    {
        if (items.Count > 0)
        {
            int randomI = Random.Range(0, items.Count);

            Inventory.instance.CreateItem(items[randomI]);

            anim.SetTrigger("open");
            switch (items[randomI].itemType.ToString())
            {
                case "Shield":
                    items.RemoveAt(randomI);
                    break;
                case "Sword":
                    items.RemoveAt(randomI);
                    break;
                case "Axe":
                    items.RemoveAt(randomI);
                    break;
                case "Chest":
                    items.RemoveAt(randomI);
                    break;
            }

            isOpen = true;
        }
        else
        {
            return;
        }
    }


}
