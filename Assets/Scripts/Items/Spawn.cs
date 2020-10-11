using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{

    public GameObject item;
    private Transform player;
    private GameObject spawnItem = null;

    public void SpawnDroppedItem()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (spawnItem == null)
        {
            //Vector2 playerPos = new Vector2(player.position.x, player.position.y + 20f);
			
            //spawnItem = Instantiate(item, playerPos, Quaternion.identity);	
        }
    }
}
