using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slots : MonoBehaviour, IDropHandler
{
    [System.Serializable]
    public enum SlotsType
    {
        Inventory,
        Sword,
        Chest,
        Shield
    }

    public SlotsType slotType;
    public GameObject item
    {
        get
        {
            if(transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }

            return null;
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        if(!item)
        {
            DragItem.itemBeginDragged.GetComponent<DragItem>().SetParent(transform, this);
        }
    }
}