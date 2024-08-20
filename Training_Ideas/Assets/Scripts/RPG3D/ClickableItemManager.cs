using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableItemManager : MonoBehaviour
{
    public GameControllerRPG3D controller;
    public DragItem dragItem;
    public Item item;
    void Start()
    {
        dragItem = GameObject.FindGameObjectWithTag("drag").GetComponent<DragItem>();
        item = controller.item;
    }

    public void ClickableItemAction()
    {
        Debug.Log(item.itemType.ToString());
        //if(item.itemType.ToString() == "Potion")
        //{
        //    Debug.Log("Potion");
        //}
    }
}
