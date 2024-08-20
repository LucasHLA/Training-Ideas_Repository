using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public static GameObject itemBeginDragged;
    private Vector3 startPos;
    private Transform startParent;

    public Item item;

    void Start()
    {
        GetComponent<Image>().sprite = item.icon;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        itemBeginDragged = gameObject;
        startPos = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemBeginDragged = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (transform.parent == startParent)
        {
            transform.position = startPos;
        }
    }

    public void SetParent(Transform slotTransform, Slots slot)
    {
        if(item.slotsType.ToString() == slot.slotType.ToString())
        {
            transform.SetParent(slotTransform);
            item.ItemAction();
        }
        else if (slot.slotType.ToString() == "Inventory")
        {
            transform.SetParent(slotTransform);
            item.RemoveItemStats();
        }
    }

    public void ClickableAction()
    {
        if(item.itemType.ToString() == "Potion" || item.itemType.ToString() == "Elixir" || item.itemType.ToString() == "Crystal")
        {
            item.ItemAction();
            Destroy(this.gameObject);
        }
    }
}
