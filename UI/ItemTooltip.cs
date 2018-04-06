using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] Text ItemNameText;
    [SerializeField] Text ItemTextText;
    [SerializeField] Text ItemModifiersText;

    private StringBuilder stringBuilder = new StringBuilder ();

    private void Awake ()
    {
        if (ItemNameText == null || ItemTextText == null || ItemModifiersText == null)
        {
            Text[] text = GetComponentsInChildren<Text> ();
            ItemNameText = text[0];
            ItemTextText = text[1];
            ItemModifiersText = text[2];
        }
    }

    private void Start ()
    {
        HideTooltip ();
    }

    public void ShowTooltip (Item item)
    {
        ItemNameText.text = item.Name;
        ItemTextText.text = item.Text;

        stringBuilder.Length = 0;
        SetItemModifiersText (item);

        ItemModifiersText.text = stringBuilder.ToString ();

        gameObject.SetActive (true);
    }

    public void HideTooltip ()
    {
        gameObject.SetActive (false);
    }

    private void SetItemModifiersText (Item item)
    {
        if (item is Portion)
        {
            Portion portion = item as Portion;

            if (portion.HpPoint != 0)
            {
                stringBuilder.Append ($"Hp +{portion.HpPoint}");
            }

            if (portion.MpPoint != 0)
            {
                StringBuilderAppendLineLine ();
                stringBuilder.Append ($"Hp +{portion.MpPoint}");
            }

            if (portion.HpPercent != 0)
            {
                StringBuilderAppendLineLine ();
                stringBuilder.Append ($"Hp {portion.HpPercent * 100}%");
            }

            if (portion.MpPercent != 0)
            {
                StringBuilderAppendLineLine ();
                stringBuilder.Append ($"Mp {portion.MpPercent * 100}%");
            }
        }
        else if (item is Scroll)
        {
            Scroll scroll = item as Scroll;
            SetMagicModifiersText (scroll.magic);
        }
        else if (item is EquippableItem)
        {
            EquippableItem equippableItem = item as EquippableItem;

            AddStat (equippableItem.PowerBonus , "Power");
            AddStat (equippableItem.RangeBonus , "Range");
            AddStat (equippableItem.DefanseBonus , "Defans");

            AddStat ((float)equippableItem.PowerPercentBonus , "Power" , isPercent: true);
            AddStat ((float)equippableItem.RangePercentBonus , "Range" , isPercent: true);
            AddStat ((float)equippableItem.DefansePercentBonus , "Defans" , isPercent: true);
        }

        ItemModifiersText.text = stringBuilder.ToString ();

    }

    private void AddStat (float value , string statName , bool isPercent = false)
    {
        if (value != 0)
        {
            if (stringBuilder.Length > 0)
            {
                stringBuilder.AppendLine ();
            }

            if (value > 0)
            {
                stringBuilder.Append ("+");
            }

            if (isPercent)
            {
                stringBuilder.Append (value * 100);
                stringBuilder.Append ("% ");
            }
            else
            {
                stringBuilder.Append (value);
                stringBuilder.Append (" ");
            }
            stringBuilder.Append (statName);
        }
    }

    public void SetMagicModifiersText (Magic magic)
    {
        stringBuilder.Append ($"MagicName: {magic.magicName}\n");
        stringBuilder.Append ($"MagicText: {magic.magicText}\n");
        stringBuilder.Append ($"MagicType: {magic.type}\n");
        if (magic.targeting)
            stringBuilder.Append ("Targeting \n");

        stringBuilder.Append ($"MagicDamage: {magic.damage}\n");

        if (magic.range > 0)
        {
            stringBuilder.Append ($"MagicRange: {magic.range}\n");
        }

        stringBuilder.Append ($"Casting: {magic.castingCount}\n");
        stringBuilder.Append ($"MpCost: {magic.magicPointCost}\n");
    }


    void StringBuilderAppendLineLine ()
    {
        if (stringBuilder.Length != 0)
        {
            stringBuilder.AppendLine ();
        }
    }

}
