using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public int i;
    public bool dropped = false;
	public Image selectImage;
	
    public void Update()
    {
        if (transform.childCount <= 0)
        {
            GetComponentInParent<Inventory>().isFull[i] = false;
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
            //transform.GetComponentInChildren<Spawn>().SpawnDroppedItem();
            GameObject.Destroy(transform.GetComponentInChildren<Spawn>().gameObject);
        }
    }
}