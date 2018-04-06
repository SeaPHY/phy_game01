using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Stat _stat;
    public Stat Stat
    {
        get { return _stat; }
        set
        {
            _stat = value;
            UpdateStatValue ();
        }
    }

    public string _name;
    public string Name
    {
        get { return _name; }
        set
        {
            _name = value;
            nameText.text = _name.ToLower ();
        }
    }

    [SerializeField] Text nameText;
    [SerializeField] Text valueText;
    [SerializeField] StatTooltip tooltip;

    private void Awake ()
    {
        Text[] texts = GetComponentsInChildren<Text> ();
        nameText = texts[0];
        valueText = texts[1];

        if (tooltip == null)
        {
            tooltip = FindObjectOfType<StatTooltip> ();
        }
    }

    public void OnPointerEnter (PointerEventData eventData)
    {
        tooltip.ShowTooltip (Stat , Name);
    }

    public void OnPointerExit (PointerEventData eventData)
    {
        tooltip.HideTooltip ();
    }

    public void UpdateStatValue ()
    {
        valueText.text = _stat.Value.ToString ();
    }
}
