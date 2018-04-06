using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : InventorySlot
{
    public EquipmentType EquipmentType;

    protected void Start ()
    {
        gameObject.name = EquipmentType.ToString () + " Slot";
    }
}
