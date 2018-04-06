using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActiveItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image image;
    [SerializeField] ItemTooltip itemTooltip;
    [SerializeField] Text itemName;
    Player player;

    private Item item;
    public Item Item
    {
        get { return item; }
        set
        {
            item = value;

            if (item == null)
            {
                image.enabled = false;
                itemName.text = "";
            }
            else
            {
                image.sprite = GameManager.Instance.itemIconDictionaty[item.Icon];
                image.enabled = true;
                itemName.text = item.Name;
            }
        }
    }

    private void Start ()
    {
        Item = null;
    }

    private void Awake ()
    {
        if (image == null)
        {
            image = GetComponent<Image> ();
        }

        if (itemTooltip == null)
        {
            itemTooltip = FindObjectOfType<ItemTooltip> ();
        }

        if (itemName == null)
        {
            itemName = transform.parent.GetComponentInChildren<Text> ();
        }

        if (player == null)
        {
            player = FindObjectOfType<Player> ();
        }
    }

    public virtual void OnPointerClick (PointerEventData eventData)
    {
        if (Item != null)
        {
            Item = player.ActiveItem (Item);
        }
    }

    public virtual void OnPointerEnter (PointerEventData eventData)
    {
        if (Item != null)
        {
            itemTooltip.ShowTooltip (item);
        }
    }

    public virtual void OnPointerExit (PointerEventData eventData)
    {
        itemTooltip.HideTooltip ();
    }

}