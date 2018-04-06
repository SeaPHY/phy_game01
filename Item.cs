using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemRank
{
    Common,
    Uncommon,
    Rare,
    Epic,
}

/// <summary>
/// 아이템 
/// </summary>
/// 
[System.Serializable]
public class Item
{
    
    public string Name;
    public string Text;  // 설명
    public string Icon;
    public ItemRank Rank;

    public Item () { }

    public Item (string name , string text , string icon , ItemRank rank)
    {
        Name = name;
        Text = text;
        Icon = icon;
        Rank = rank;
    }
}

/// <summary>
/// 1회용 아이템 - 포션
/// </summary>
[System.Serializable]
public class Portion : Item
{
    public int HpPoint;
    public int MpPoint;
    public double HpPercent;
    public double MpPercent;

    public Portion () { }

    public Portion (string name , string text , string icon , ItemRank rank, 
        int hpPoint , int mpPoint , double hpPercent , double mpPercent) 
            : base (name, text, icon, rank)
    {
        HpPoint = hpPoint;
        MpPoint = mpPoint;
        HpPercent = hpPercent;
        MpPercent = mpPercent;
    }
}

/// <summary>
/// 1회용 아이템 -  마법 스크롤
/// </summary>
public class Scroll : Item
{
    public Magic magic;

    public Scroll (string name , string text , string icon , ItemRank rank, Magic magic)
        : base (name , text , icon , rank)
    {
        this.magic = magic;
    }

}

public enum EquipmentType
{
    Helmet,
    Chest,
    Gloves,
    Boots,
    Weapon1,
    Weapon2,
    Accessory1,
    Accessory2,
}

/// <summary>
/// 장비 아이템
/// </summary>
[System.Serializable]
public class EquippableItem : Item
{
    public EquipmentType Type;

    public int PowerBonus;
    public int DefanseBonus;
    public int RangeBonus;

    public double PowerPercentBonus;
    public double DefansePercentBonus;
    public double RangePercentBonus;

    public EquippableItem () { }

    public EquippableItem (string name , string text , string icon , ItemRank rank ,
        EquipmentType type, int powerBonus , int defanseBouns , int rangeBonus ,
         double powerPercentBonus , double defansePercentBonus , double rangePercentBonus)
          : base (name , text , icon , rank)
    {
        Type = type;

        PowerBonus = powerBonus;
        DefanseBonus = defanseBouns;
        RangeBonus = rangeBonus;

        PowerPercentBonus = powerPercentBonus;
        DefansePercentBonus = defansePercentBonus;
        RangePercentBonus = rangePercentBonus;
    }

    // 장착 - 스텟 추가
    public void Equip (Player p)
    {
        if (PowerBonus != 0)
            p.Power.AddModifier (new StatModifier (PowerBonus , StatModType.Flat , this));
        if (DefanseBonus != 0)
            p.Defanse.AddModifier (new StatModifier (DefanseBonus , StatModType.Flat , this));
        if (RangeBonus != 0)
            p.Range.AddModifier (new StatModifier (RangeBonus , StatModType.Flat , this));

        if (PowerPercentBonus != 0)
            p.Power.AddModifier (new StatModifier ((float)PowerPercentBonus , StatModType.PercentMult , this));
        if (DefansePercentBonus != 0)
            p.Defanse.AddModifier (new StatModifier ((float)DefansePercentBonus , StatModType.PercentMult , this));
        if (RangePercentBonus != 0)
            p.Range.AddModifier (new StatModifier ((float)RangePercentBonus , StatModType.PercentMult , this));

        p.SetStats ();
    }

    // 해체 - 스텟 제거
    public void Unequip (Player p)
    {
        p.Power.RemoveAllModifiersFromSource (this);
        p.Defanse.RemoveAllModifiersFromSource (this);
        p.Range.RemoveAllModifiersFromSource (this);
        p.SetStats ();
    }

}
