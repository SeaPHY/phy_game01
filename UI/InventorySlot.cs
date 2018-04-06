using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image image;
    [SerializeField] ItemTooltip itemTooltip;
    

    public event Action<Item> OnClickEvent;

    public Item item;
    public Item Item
    {
        get { return item; }
        set
        {
            item = value;

            if (item == null)
            {
                image.enabled = false;
            }
            else
            {
                image.sprite = GameManager.Instance.itemIconDictionaty[item.Icon];
                image.enabled = true;
            }
        }
    }

    private void Awake ()
    {
        if (itemTooltip == null)
        {
            itemTooltip = FindObjectOfType<ItemTooltip> ();
        }

        if (image == null)
        {
            image = GetComponent<Image> ();
        }

        Item = null;
    }


    public void OnPointerClick (PointerEventData eventData)
    {
        if (Item != null && OnClickEvent != null)
        {
            OnClickEvent (Item);
        }
    }

    public void OnPointerEnter (PointerEventData eventData)
    {
        if (Item != null)
        {
            itemTooltip.ShowTooltip (Item);
        }
    }

    public void OnPointerExit (PointerEventData eventData)
    {
        itemTooltip.HideTooltip ();
    }
}
