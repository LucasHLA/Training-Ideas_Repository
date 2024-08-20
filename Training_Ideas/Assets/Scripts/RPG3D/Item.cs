using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public Sprite icon;
    public string name;
    public float value;

    [System.Serializable]
    public enum Type
    {
        Potion,
        Elixir,
        Crystal,
        Chestplate,
        Sword,
        Shield,
        Axe
    }

    public Type itemType;

    public enum SlotsType
    {
        Sword,
        Chest,
        Shield
    }

    public SlotsType slotsType;

    public Player player;
    public void ItemAction()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        switch (itemType)
        {
            case Type.Potion:
                player.IncreaseStats(value, 0, 0);
                break;

            case Type.Elixir:
                player.IncreaseStats(0, value, 0);
                break;

            case Type.Crystal:
                player.IncreaseStats(0, 0, value);
                break;

            case Type.Chestplate:
                player.IncreaseStats(value, 0, 0);
                break;

            case Type.Sword:
                player.EquipWeapon(1);
                player.IncreaseStats(0, 0, value);
                break;

            case Type.Shield:
                player.IncreaseStats(value, 0, 0);
                break;

            case Type.Axe:
                player.EquipWeapon(2);
                player.IncreaseStats(0, 0, value);
                break;
        }
    }

    public void RemoveItemStats()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        switch (itemType)
        {
            case Type.Chestplate:
                player.DecreaseStats(value, 0, 0);
                break;

            case Type.Sword:
                player.EquipWeapon(0);
                player.DecreaseStats(0, 0, value);
                break;

            case Type.Shield:
                player.DecreaseStats(value, 0, 0);
                break;

            case Type.Axe:
                player.EquipWeapon(0);
                player.IncreaseStats(0, 0, value);
                break;
        }
    }
}
