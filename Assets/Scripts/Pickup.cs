using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private bool Pickable = true;
    private float RespawnTimer = 0f;
    public float recoveryTime = 2.5f;

    public float PickupTime; // Total Time it takes the player to respawn
    private float pickupTimer = 1f; // Variable to store the timer of the respawn
    private Inventory inventory;
    public GameObject itemButton;


    public enum Action
    {
        DashPotion,
        MeleeWeapons,       // Меч
        RangedWeapons        // Лук
    }
    public Action action; // Активное оружие
    [SerializeField]
    private SpriteRenderer Sprite;

    void Awake()
    {
        if (Sprite == null)
        {
            Sprite = GetComponent<SpriteRenderer>();

            if (Sprite == null)
            {
                Debug.Log("This refill has no spriterenderer attached to it");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pickupTimer > 0f)
        {
            pickupTimer -= Time.deltaTime;
        }

        if (this.RespawnTimer > 0f)
        {
            this.RespawnTimer -= Time.deltaTime;
            if (this.RespawnTimer <= 0f)
            {
                this.Respawn();
            }
        }
    }

    //void OnTriggerEnter2D (Collider2D other) {
    //	if (other.CompareTag ("Player") && Pickable) {
    //		var playercomponent = other.GetComponent<Player> ();
    //		if (playercomponent != null) {
    //			OnPlayerTrigger (playercomponent);
    //		}
    //	}
    //}

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Pickable)
        {
            var playercomponent = other.GetComponent<Player>();
            if (playercomponent != null)
            {
                OnPlayerTrigger(playercomponent);
            }
        }
    }

    void OnPlayerTrigger(Player player)
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        if (action == Action.DashPotion)
        {
            if (player.UseRefillDash())
            {
                Pickable = false;
                // Disable
                Sprite.enabled = false;
                this.RespawnTimer = recoveryTime;

                // Screenshake
                if (PixelCameraController.instance != null)
                {
                    PixelCameraController.instance.Shake(0.1f);
                }
            }
        }
        if (action == Action.MeleeWeapons)
        {
            if (player.CompareTag("Player"))
            {
                if (inventory.isFull[0] == false)
                {
                    if (player.PickUpSword())
                    {
                        {
                            Pickable = false;
                            // Disable
                            Sprite.enabled = false;
                            this.RespawnTimer = recoveryTime;

                            // Screenshake
                            if (PixelCameraController.instance != null)
                            {
                                PixelCameraController.instance.Shake(0.1f);
                            }

                            inventory.isFull[0] = true;
                            Instantiate(itemButton, inventory.slots[0].transform, false);

                            Destroy(gameObject);
                        }
                    }
                }
                else
                {
                    if (Input.GetKey(KeyCode.E) && pickupTimer <= 0f)
                    {
                        pickupTimer = PickupTime;
                        player.GetComponent<Inventory>().slots[0].GetComponent<Slot>().DropItem();
                        player.DropSword();
                    }
                }
            }
        }
        if (action == Action.RangedWeapons)
        {
            if (player.CompareTag("Player"))
            {
                if (inventory.isFull[1] == false)
                {
                    if (player.PickUpBow())
                    {
                        Pickable = false;
                        // Disable
                        Sprite.enabled = false;
                        this.RespawnTimer = recoveryTime;

                        // Screenshake
                        if (PixelCameraController.instance != null)
                        {
                            PixelCameraController.instance.Shake(0.1f);
                        }

                        inventory.isFull[1] = true;
                        Instantiate(itemButton, inventory.slots[1].transform, false);

                        Destroy(gameObject);
                    }
                }
                else
                {
                    if (Input.GetKey(KeyCode.E) && pickupTimer <= 0f)
                    {
                        pickupTimer = PickupTime;
                        player.GetComponent<Inventory>().slots[1].GetComponent<Slot>().DropItem();
                        player.DropBow();
                    }
                }
            }
        }
    }

    private void Respawn()
    {
        if (!Pickable)
        {
            Pickable = true;
            Sprite.enabled = true;
        }
    }
}
