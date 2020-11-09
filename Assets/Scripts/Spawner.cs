using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Range(0, 99)]
    public int SpawnChance;
    public GameObject[] SpawnObject;
    void Awake()
    {
        if (Random.Range(0, 100) < SpawnChance)
        {
            var rnd = Random.Range(0, SpawnObject.Length);
            var p = Instantiate(SpawnObject[rnd], gameObject.transform.position, Quaternion.identity);
        }
    }
}
