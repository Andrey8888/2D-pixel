using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Range(0, 99)]
    public int Chance;
    public GameObject[] enemy;
    void Awake()
    {
        if (Random.Range(0, 100) < Chance)
        {
            var rnd = Random.Range(0, enemy.Length);
            var p = Instantiate(enemy[rnd], gameObject.transform.position, Quaternion.identity);
        }
    }
}
