using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CurrentMagicSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image image;
    [SerializeField] MagicTooltip magicTooltip;
    [SerializeField] Text magicNameText;
    public virtual event Action OnClickEvent;

    private Magic magic;
    public Magic Magic
    {
        get { return magic; }
        set
        {
            magic = value;

            if (magic == null)
            {
                image.enabled = false;
                magicNameText.enabled = false;
            }
            else
            {
                image.sprite = magic.icon;
                magicNameText.text = magic.magicName;
                magicNameText.enabled = true;
                image.enabled = true;
            }
        }
    }

    private void Awake ()
    {
        if (image == null)
        {
            image = GetComponent<Image> ();
        }

        if (magicTooltip == null)
        {
            magicTooltip = FindObjectOfType<MagicTooltip> ();
        }

        if (magicNameText == null)
        {
            magicNameText = transform.parent.GetComponentInChildren<Text> ();
        }
    }

    public virtual void OnPointerClick (PointerEventData eventData)
    {

        if (Magic != null && OnClickEvent != null)
        {
            OnClickEvent();
        }
    }

    public virtual void OnPointerEnter (PointerEventData eventData)
    {
        magicTooltip.ShowTooltip (magic);
    }

    public virtual void OnPointerExit (PointerEventData eventData)
    {
        magicTooltip.HideTooltip ();
    }

}
