using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI와 연결된 인벤토리
public class Inventory : MonoBehaviour
{
    public List<Item> items;
    [SerializeField] Transform inventoryParent;
    [SerializeField] EquipmentPanel equipmentSlotsParent;
    [SerializeField] StatsPanel statsPanel;
    InventorySlot[] inventorySlot;
    Button trashcanButton;
    public bool isItemRemve = false;
    public bool isInventoryOpen;
    public Sprite trashcan;
    public Sprite trashcanOpen;

    public event Action<Item> OnItemClickEvent;

    private void Awake ()
    {
        if (inventoryParent == null)
        {
            inventoryParent = GameObject.FindGameObjectWithTag ("Inventory Parent").transform;
        }

        if (trashcanButton == null)
        {
            trashcanButton = inventoryParent.gameObject.GetComponentInChildren<Button> ();
        }

        inventorySlot = inventoryParent.gameObject.GetComponentsInChildren<InventorySlot> ();
    }

    private void Start ()
    {
        if (equipmentSlotsParent == null)
        {
            equipmentSlotsParent = FindObjectOfType<EquipmentPanel> ();
        }

        if (statsPanel == null)
        {
            statsPanel = FindObjectOfType<StatsPanel> ();
        }

        // OnItemClickEvent += 

        for (int i = 0 ; i < inventorySlot.Length ; i++)
        {
            inventorySlot[i].OnClickEvent += OnItemClickEvent;
        }

        RefreshUI ();
        InventoryOpen ();
    }

    public void InventoryOpen ()
    {
        isInventoryOpen = !inventoryParent.gameObject.activeSelf;
        inventoryParent.gameObject.SetActive (isInventoryOpen);
        equipmentSlotsParent.equipmentSlotsParent.gameObject.SetActive (isInventoryOpen);
        statsPanel.gameObject.SetActive (isInventoryOpen);

        if (!isInventoryOpen)
        {
            isItemRemve = isInventoryOpen;
            trashcanButton.image.sprite = trashcan;
        }
    }

    private void RefreshUI ()
    {
        int i = 0;
        for (; i < items.Count && i < inventorySlot.Length ; i++)
        {
            inventorySlot[i].Item = items[i];
        }

        for (; i < inventorySlot.Length ; i++)
        {
            inventorySlot[i].Item = null;
        }
    }

    // 아이템 제거 버튼 활성화 되면 
    public void RemoveItemswitch ()
    {
        isItemRemve = !isItemRemve;

        if (isItemRemve)
        {
            trashcanButton.image.sprite = trashcanOpen;
        }
        else
        {
            trashcanButton.image.sprite = trashcan;
        }
    }

    public bool AddItem (Item item)
    {
        if (IsFull ())
        {
            return false;
        }

        items.Add (item);
        RefreshUI ();
        return true;
    }

    public bool RemoveItem (Item item)
    {
        if (items.Remove (item))
        {
            RefreshUI ();
            return true;
        }
        return false;
    }

    public bool IsFull ()
    {
        return items.Count >= inventorySlot.Length;
    }

    public void InventroyReset ()
    {
        items.Clear ();
        RefreshUI ();
    }

}
