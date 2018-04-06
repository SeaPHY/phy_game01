using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public float inverseMoveTime = 0.1f;
    public Coordinates coordinates;
    TileMapManager tileMapManager;

    private void Start ()
    {
        tileMapManager = GameManager.Instance.tileMapManager;
    }

    private void Update ()
    {
        transform.Rotate (Vector3.forward , 3f , Space.Self);
    }

    public void GetItemPlayer ()
    {
        GameManager.Instance.inventory.AddItem (GameManager.Instance.GetRandomEquippableItem ());
        tileMapManager.currentObject.Remove (coordinates);
        tileMapManager.currentItems.Remove (coordinates);

        Destroy (this.gameObject);
        //this.gameObject.SetActive (false);
    }

    public void RemoveItemObject ()
    {
        tileMapManager.currentObject.Remove (coordinates);
        tileMapManager.currentItems.Remove (coordinates);
        Destroy (this.gameObject);
    }

}
