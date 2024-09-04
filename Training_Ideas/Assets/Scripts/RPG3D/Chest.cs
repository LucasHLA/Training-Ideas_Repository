using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator anim;

    [Header("Chest opening variables")]
    public float colliderRadius;
    public bool isOpen;

    [Header("Chest Canvas Variables")]
    public GameObject E;

    public List<Item> items = new List<Item>();

    // Lista est�tica para rastrear os itens �nicos que j� foram coletados
    private static List<Item.Type> collectedUniqueItems = new List<Item.Type>();

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
                        OpenChest();
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

    void OpenChest()
    {
        if (items.Count > 0)
        {
            int randomI = Random.Range(0, items.Count);
            Item selectedItem = items[randomI];

            // Verifica se o item � �nico e se j� foi coletado
            if (IsUniqueItem(selectedItem.itemType) && collectedUniqueItems.Contains(selectedItem.itemType))
            {
                return; // O item �nico j� foi coletado, ent�o n�o faz nada
            }

            // Cria o item
            Inventory.instance.CreateItem(selectedItem);

            // Se for um item �nico, marca como coletado
            if (IsUniqueItem(selectedItem.itemType))
            {
                collectedUniqueItems.Add(selectedItem.itemType);
                RemoveItemFromAllChests(selectedItem); // Remove o item de todos os ba�s
            }

            anim.SetTrigger("open");
            isOpen = true;
        }
        else
        {
            Debug.Log("O ba� est� vazio!");
        }
    }

    // Fun��o para verificar se o item � �nico
    bool IsUniqueItem(Item.Type itemType)
    {
        return itemType == Item.Type.Shield || itemType == Item.Type.Sword ||
               itemType == Item.Type.Axe || itemType == Item.Type.Chestplate;
    }

    // Fun��o para remover um item de todos os ba�s na cena
    void RemoveItemFromAllChests(Item itemToRemove)
    {
        // Encontra todos os ba�s ativos na cena
        Chest[] allChests = FindObjectsOfType<Chest>();

        // Remove o item da lista de cada ba�
        foreach (Chest chest in allChests)
        {
            chest.items.Remove(itemToRemove);
        }
    }
}
