using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MagicSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image image;
    [SerializeField] MagicTooltip magicTooltip;

    public virtual event Action<Magic> OnClickEvent;

    public Magic magic;
    public Magic Magic
    {
        get { return magic; }
        set
        {
            magic = value;

            if (magic == null)
            {
                image.enabled = false;
            }
            else
            {
                image.sprite = magic.icon;
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
    }

    public virtual void OnPointerClick (PointerEventData eventData)
    {
        if (Magic != null && OnClickEvent != null)
        {
            OnClickEvent (Magic);
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
