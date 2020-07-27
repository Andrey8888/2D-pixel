using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemy;
    void Awake()
    {
        if (Random.Range(0, 100) < 50)
        {
            var rnd = Random.Range(0, enemy.Length);
            var p = Instantiate(enemy[rnd], gameObject.transform.position, Quaternion.identity);
        }
    }
}
