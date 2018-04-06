using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StatTooltip : MonoBehaviour
{
    [SerializeField] Text StatNameText;
    [SerializeField] Text StatModifierText;

    private StringBuilder stringBuilder = new StringBuilder ();

    private void Start ()
    {
        if (StatNameText == null)
        {
            Text[] text = GetComponentsInChildren<Text> ();
            text[0] = StatNameText;
            text[1] = StatModifierText;
        }

        HideTooltip ();
    }

    public void ShowTooltip (Stat stat , string statName)
    {
        StatNameText.text = GetStatTopText (stat , statName);

        StatModifierText.text = GetStatModifiersText (stat);

        gameObject.SetActive (true);
    }

    public void HideTooltip ()
    {
        gameObject.SetActive (false);
    }


    private string GetStatTopText (Stat stat , string statName)
    {
        stringBuilder.Length = 0;
        stringBuilder.Append (statName);
        stringBuilder.Append (" ");
        stringBuilder.Append (stat.Value);

        if (stat.Value != stat.BaseValue)
        {
            stringBuilder.Append (" (");
            stringBuilder.Append (stat.BaseValue);

            if (stat.Value > stat.BaseValue)
            {
                stringBuilder.Append ("+");
            }

            stringBuilder.Append (System.Math.Round (stat.Value - stat.BaseValue , 4));
            stringBuilder.Append (")");
        }

        return stringBuilder.ToString ();
    }

    private string GetStatModifiersText (Stat stat)
    {
        stringBuilder.Length = 0;

        foreach (StatModifier mod in stat.StatModifiers)
        {
            if (stringBuilder.Length > 0)
            {
                stringBuilder.AppendLine ();
            }

            if (mod.Value > 0)
            {
                stringBuilder.Append ("+");
            }

            if (mod.Type == StatModType.Flat)
            {
                stringBuilder.Append (mod.Value);
            }
            else
            {
                stringBuilder.Append (mod.Value * 100);
                stringBuilder.Append ("%");
            }


            EquippableItem item = mod.Source as EquippableItem;

            if (item != null)
            {
                stringBuilder.Append (" ");
                stringBuilder.Append (item.Name);
            }
            else
            {
                Debug.LogError ("Modifier is not an EquippableItem!");
            }
        }

        return stringBuilder.ToString ();
    }
}
