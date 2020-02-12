using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyImpact : MonoBehaviour
{
    public bool InImpactZone;
    public Collider2D col;

    [Header("Damage To Player")]
    public int DamageOnTouch = 1;

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            col = collision;
            InImpactZone = true;
        }
        if (collision.CompareTag("Player") && !gameObject.GetComponentInParent<Health>().dead)
        {
            var playercomponent = collision.GetComponent<Player>();
            if (playercomponent != null)
            {
                OnPlayerTrigger(playercomponent);
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
        if (collision.CompareTag("Player") && !gameObject.GetComponentInParent<Health>().dead)
        {
            var playercomponent = collision.GetComponent<Player>();
            if (playercomponent != null)
            {
                OnPlayerTrigger(playercomponent);
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
    }

    // Function to deal damage to the player
    void OnPlayerTrigger(Player player)
    {
        player.GetComponent<Health>().TakeDamage(DamageOnTouch);
    }
}
