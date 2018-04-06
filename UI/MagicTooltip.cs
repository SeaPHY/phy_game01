using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MagicTooltip : MonoBehaviour
{
    [SerializeField] Text MagicNameText;
    [SerializeField] Text MagicTextText;
    [SerializeField] Text MagicModifiersText;

    private StringBuilder stringBuilder = new StringBuilder ();

    private void Awake ()
    {
        if (MagicNameText == null || MagicTextText == null || MagicModifiersText == null)
        {
            Text[] text = GetComponentsInChildren<Text> ();
            MagicNameText = text[0];
            MagicTextText = text[1];
            MagicModifiersText = text[2];
        }
    }

    private void Start ()
    {
        HideTooltip ();
    }

    public void ShowTooltip (Magic magic)
    {
        MagicNameText.text = magic.magicName;
        MagicTextText.text = magic.magicText;

        SetMagicModifiersText (magic);

        MagicModifiersText.text = stringBuilder.ToString ();

        gameObject.SetActive (true);
    }

    public void HideTooltip ()
    {
        gameObject.SetActive (false);
    }

    public void SetMagicModifiersText (Magic magic)
    {
        stringBuilder.Length = 0;

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
}
