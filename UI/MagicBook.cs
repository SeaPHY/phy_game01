using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBook : MonoBehaviour
{
    [SerializeField] List<Magic> magics;
    [SerializeField] Transform MagicBookParent;
    MagicSlot[] magicSlots;

    public event Action<Magic> OnMagicClickEvent;

    private void Awake ()
    {
        magicSlots = MagicBookParent.gameObject.GetComponentsInChildren<MagicSlot> ();
    }

    private void Start ()
    {
        if (OnMagicClickEvent != null && magics.Count >= 1)
        {
            OnMagicClickEvent (magics[0]);
        }
    }

    public void MagicActionSet ()
    {
        for (int i = 0 ; i < magicSlots.Length ; i++)
        {
            magicSlots[i].OnClickEvent += OnMagicClickEvent;
        }
        RefreshUI ();
        //MagicBookOpen ();
    }


    public void MagicBookOpen ()
    {
        MagicBookParent.gameObject.SetActive (!MagicBookParent.gameObject.activeSelf);
    }

    private void RefreshUI ()
    {
        int i = 0;
        for (; i < magics.Count && i < magicSlots.Length ; i++)
        {
            magicSlots[i].Magic = magics[i];
        }

        for (; i < magicSlots.Length ; i++)
        {
            magicSlots[i].Magic = null;
        }
    }

    public bool AddMagic (Magic magic)
    {
        if (IsFull ())
        {
            return false;
        }

        magics.Add (magic);
        RefreshUI ();
        return true;
    }

    public bool RemoveMagic (Magic magic)
    {
        if (magics.Remove (magic))
        {
            RefreshUI ();
            return true;
        }
        return false;
    }

    public bool IsFull ()
    {
        return magics.Count >= magicSlots.Length;
    }

}

