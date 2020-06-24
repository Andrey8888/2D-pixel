using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{

    public Player player;
    public int i;
    public bool dropped = false;

    public void Update()
    {
        if (transform.childCount <= 0)
        {
            player.GetComponent<Inventory>().isFull[i] = false;
        }
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //        DropItem();
        //}
    }

    public void DropItem()
    {
        if (transform.GetComponentInChildren<Spawn>() != null)
        {
            transform.GetComponentInChildren<Spawn>().SpawnDroppedItem();
            GameObject.Destroy(transform.GetComponentInChildren<Spawn>().gameObject);
        }
    }
}