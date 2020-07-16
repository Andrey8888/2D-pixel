using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupRangedWeapons : MonoBehaviour
{
    private bool Pickable = true;
    private float RespawnTimer = 0f;
    public float recoveryTime = 2.5f;

    public float PickupTime; // Total Time it takes the player to respawn
    private float pickupTimer = 1f; // Variable to store the timer of the respawn
    private Inventory inventory;
    public GameObject itemButton;
	
	[Header("RangedWeapons")]
	public int RangedAttackMinDamage = 2;
	public int RangedAttackMaxDamage = 5;
	public float RangedAttackCooldownTime = 1;
    [Range(1, 10)]
    public int CriticalDamageMultiply = 1;
    [Range(0, 99)]
    public int ChanceCriticalDamage = 0;
    public int StepUpAfterHit = 0;
	[Range(1, 10)]
	public int level = 1;  // Multiply damage
	
	[Header("Poison")]
	public bool CanPoison = false;
	public int PoisonDamaged = 0;
	public int PoisonFrequency = 0;
	public int PoisonTick = 0;
	[Header("Fire")]
	public bool CanFire = false;
	public int FireDamaged = 0;
	public int FireFrequency = 0;
	public int FireTick = 0;
	[Header("Other")]
	public bool CanPush = false;
    public bool CanThroughShoot = false;

    public enum RangedWeapon
    {
		Bow,
		Staff,
		Staff2
    }
	public RangedWeapon Type = RangedWeapon.Bow;
	
	
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

    public void OnPlayerTrigger(Player player)
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

                if (inventory.isFull[1] == false && player.CompareTag("Player"))
                {
                if (player.PickUpRangedWeapon(Type.ToString(), RangedAttackMinDamage, RangedAttackMaxDamage, 
				CanPoison, PoisonDamaged, PoisonFrequency, PoisonTick,
				CanFire, FireDamaged, FireFrequency, FireTick,
				CanPush, CanThroughShoot))
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
                    if (gameObject != null)
                        Destroy(gameObject);
                    }
                }
                else
                {
                    if (Input.GetKey(KeyCode.E) && pickupTimer <= 0f)
                    {
                        pickupTimer = PickupTime;

                        inventory.isFull[1] = false;
                        player.GetComponent<Inventory>().slots[1].GetComponent<Slot>().DropItem();
                        player.DropRangedWeapon();
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
