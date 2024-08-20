using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControllerRPG3D : MonoBehaviour
{
    [Header("Inventory variables")]
    public GameObject inventoryButton;
    public GameObject itemPrefab;
    public Button itemPrefabButton;
    public static GameControllerRPG3D instance;
    public Item item;

    private void Awake()
    {
        instance = this;
    }
    public void ActiveGameObject(GameObject go)
    {
        go.SetActive(true);
        inventoryButton.SetActive(false);
    }
    public void DisableGameObject(GameObject go)
    {
        go.SetActive(false);
        inventoryButton.SetActive(true);
    }
}
