using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemySpawner : MonoBehaviour
{
    [Header("Стрельба")]
    //public float angle;
    public EnemyProjectile projectile;           // Стрела
    public Transform GunBarrel;             // Позиция точки выстрела
    public void InstantiateProjectile()
    {
        //if ((EnemyVisibility.PlayerPos.y - transform.position.y) < 0)
        //{
        //    angle = -angle;
        //}
        //Instantiate the projectile prefab
        var p = Instantiate(projectile, GunBarrel.position, Quaternion.identity) as EnemyProjectile;

        var parentXScale = - Mathf.Sign(transform.parent.localScale.x);

        // Set the localscale so the projectiles faces the right direction based on the parent object (base)
        p.transform.localScale = new Vector3(parentXScale * p.transform.localScale.x, p.transform.localScale.y, p.transform.localScale.z);

        // Change the X speed based on the facing of the parent object
        p.Speed.x *= parentXScale;
    }
}
