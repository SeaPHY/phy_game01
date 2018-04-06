using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EquipmentPanel : MonoBehaviour
{
    [SerializeField] EquipmentSlot[] equipmentSlots;
    public Transform equipmentSlotsParent;
    public event Action<Item> OnItemClickEnvet;

    private void Start ()
    {
        if (equipmentSlotsParent == null)
        {
            equipmentSlotsParent = GameObject.Find ("equipmentSlotsParent").GetComponent<Transform> ();
        }

        equipmentSlots = equipmentSlotsParent.GetComponentsInChildren<EquipmentSlot> ();


        for (int i = 0 ; i < equipmentSlots.Length ; i++)
        {
            equipmentSlots[i].OnClickEvent += OnItemClickEnvet;
        }
    }

    /// <summary>
    /// 아이템 추가, 장비 아이템 패널에 장착
    /// </summary>
    /// <param name="item">장착할 아이템</param>
    /// <param name="previousItem">장착 했던 아이템</param>
    /// <returns></returns>
    public bool AddItem (EquippableItem item , out EquippableItem previousItem)
    {
        for (int i = 0 ; i < equipmentSlots.Length ; i++)
        {
            // 슬롯 타입과 장착할 아이템 타입이 같으면 장착하고 장착했던 아이템을 out
            if (equipmentSlots[i].EquipmentType == item.Type)
            {
                previousItem = (EquippableItem)equipmentSlots[i].Item;
                equipmentSlots[i].Item = item;
                return true;
            }
        }
        previousItem = null;
        return false;
    }

    /// <summary>
    /// 아이템 제거, 장착 해체
    /// </summary>
    /// <param name="item">제거할 아이템</param>
    /// <returns></returns>
    public bool RemoveItem (EquippableItem item)
    {
        for (int i = 0 ; i < equipmentSlots.Length ; i++)
        {
            if (equipmentSlots[i].Item == item)
            {
                equipmentSlots[i].Item = null;
                return true;
            }
        }
        return false;
    }

    public List<EquippableItem> EquipmentSlotsReset ()
    {
        List<EquippableItem> remUnequipItem = new List<EquippableItem> ();

        for (int i = 0 ; i < equipmentSlots.Length ; i++)
        {
            if (equipmentSlots[i].Item != null)
            {
                remUnequipItem.Add (equipmentSlots[i].Item as EquippableItem);
                equipmentSlots[i].Item = null;
            }
        }

        if (remUnequipItem.Count > 0)
        {
            return remUnequipItem;
        }
        else
        {
            return null;
        }

        
    }


}
