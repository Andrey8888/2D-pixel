using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyImpact : MonoBehaviour
{
    public bool InImpactZone;
    public Collider2D col;

    [Header("Damage To Player")]
    public int DamageOnTouch = 1;
    public float SlowPlayerSpeed = 10f;

    public enum ImpactZoneAction
    {
        Damage,
        Slowdown
    }

    public ImpactZoneAction ImpactZoneActionType = ImpactZoneAction.Damage;
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            col = collision;
            InImpactZone = true;
        }
        if (collision.CompareTag("Player") && !gameObject.GetComponentInParent<Health>().dead && ImpactZoneActionType == ImpactZoneAction.Damage)
        {
            var playercomponent = collision.GetComponent<Player>();
            if (playercomponent != null)
            {
                OnPlayerTrigger(playercomponent);
            }
        }

        if (collision.CompareTag("Player") && !gameObject.GetComponentInParent<Health>().dead && ImpactZoneActionType == ImpactZoneAction.Slowdown)
        {
            var playercomponent = collision.GetComponent<Player>();
            if (playercomponent != null)
            {
                OnPlayerSlow(playercomponent);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            col = collision;
            InImpactZone = true;
        }
        if (collision.CompareTag("Player") && !gameObject.GetComponentInParent<Health>().dead && ImpactZoneActionType == ImpactZoneAction.Damage)
        {
            var playercomponent = collision.GetComponent<Player>();
            if (playercomponent != null)
            {
                OnPlayerTrigger(playercomponent);
            }
        }

        if (collision.CompareTag("Player") && !gameObject.GetComponentInParent<Health>().dead && ImpactZoneActionType == ImpactZoneAction.Slowdown)
        {
            var playercomponent = collision.GetComponent<Player>();
            if (playercomponent != null)
            {
                StartCoroutine(OnPlayerSlow(playercomponent));
            }
        }
        

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            col = null;
            InImpactZone = false;
        }
        if (collision.CompareTag("Player") && !gameObject.GetComponentInParent<Health>().dead && ImpactZoneActionType == ImpactZoneAction.Slowdown)
        {
            var playercomponent = collision.GetComponent<Player>();
            if (playercomponent != null)
            {
                OnPlayerNormalSpeed(playercomponent);
            }
        }

        //if (collision.CompareTag("DestructibleObjects") && !gameObject.GetComponentInParent<Health>().dead)
        //{
        //    var boxcomponent = collision.GetComponent<HitablePushBlock>();
        //    if (boxcomponent != null)
        //    {
        //        boxcomponent.Die();
        //    }
        //}
    }

    // Function to deal damage to the player
    void OnPlayerTrigger(Player player)
    {
        player.GetComponent<Health>().TakeDamage(DamageOnTouch, false, 0, 0, 0, false, 0, 0, 0, false, 0, false, 0);
    }

    IEnumerator OnPlayerSlow(Player player)
    {
        player.GetComponent<Player>().MaxRun = SlowPlayerSpeed;
        yield return new WaitForSeconds(1);
        player.GetComponent<Player>().MaxRun = player.GetComponent<Player>().curRun;
    }

    void OnPlayerNormalSpeed(Player player)
    {
        player.GetComponent<Player>().MaxRun = player.GetComponent<Player>().curRun;
    }
}
