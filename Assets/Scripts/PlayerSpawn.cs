using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject player;
    void Awake()
        {
            var p = Instantiate(player, gameObject.transform.position, Quaternion.identity);
        }
}
