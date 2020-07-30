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
	public int RangedAttackMinDamage = 0;
	public int RangedAttackMaxDamage = 0;
	public float RangedAttackCooldownTime = 1;
    [Range(1, 10)]
    public int CriticalDamageMultiply = 1;
    [Range(0, 99)]
    public int ChanceCriticalDamage = 0;
    public int StepUpAfterHit = 0;
    public int ShellsCount = 0;
	[Range(1, 10)]
	public int level = 1;  // Multiply damage
	[Header("PowerAttack")]
	public int RangedPowerAttackMinDamage = 0;
	public int RangedPowerAttackMaxDamage = 0;
	public float RangedPowerAttackCooldownTime = 10f;
	public bool CanPowerAttackThroughShoot = false;
	
	[Header("Poison")]
	public bool CanPoison = false;
	public bool CanPowerAttackPoison = false;
	public int PoisonDamaged = 0;
	public int PoisonFrequency = 0;
	public int PoisonTick = 0;
	[Range(0, 99)]
	public int PoisonChance = 100;
	[Header("Fire")]
	public bool CanFire = false;
	public bool CanPowerAttackFire = false;
	public int FireDamaged = 0;
	public int FireFrequency = 0;
	public int FireTick = 0;
	[Range(0, 99)]
	public int FireChance = 100;
	[Header("Freez")]
	public bool CanFreez = false;
	public bool CanPowerAttackFreez = false;
	public int FreezDuration = 0;
	[Range(0, 99)]
	public int FreezChance = 100;
	[Header("Push")]
	public bool CanPush = false;
	public bool CanPowerAttackPush  = false;
	public int PushDistance = 0;
	[Header("Other")]
    public bool CanThroughShoot = false;
	public bool RangedTossingUp = false;
	public int RangedUpAfterHit = 0;
	
	
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
        inventory = FindObjectOfType<Inventory>();
        if (inventory.isFull[1] == false && player.CompareTag("Player"))
                {							
                if (player.PickUpRangedWeapon(Type, RangedAttackMinDamage, RangedAttackMaxDamage, ShellsCount, 
				RangedAttackCooldownTime, RangedPowerAttackCooldownTime, CriticalDamageMultiply, ChanceCriticalDamage,
				CanPoison, CanPowerAttackPoison, PoisonDamaged, PoisonFrequency, PoisonTick, PoisonChance,
				CanFire, CanPowerAttackFire, FireDamaged, FireFrequency, FireTick, FireChance,
				CanFreez, CanPowerAttackFreez, FreezDuration, FreezChance,
				CanPush, CanPowerAttackPush, PushDistance,
				RangedPowerAttackMinDamage, RangedPowerAttackMaxDamage,			
				CanThroughShoot, RangedUpAfterHit, CanThroughShoot, CanPowerAttackThroughShoot))
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

                        inventory.isFull[1] = false;
                        player.GetComponent<Inventory>().slots[1].GetComponent<Slot>().DropItem();
                        player.DropRangedWeapon();
                    }
                }
    }

    public void OnPlayer(Player player)
    {
        inventory = FindObjectOfType<Inventory>();
                if (player.PickUpRangedWeapon(Type, RangedAttackMinDamage, RangedAttackMaxDamage, ShellsCount, 
				RangedAttackCooldownTime, RangedPowerAttackCooldownTime, CriticalDamageMultiply, ChanceCriticalDamage,
				CanPoison, CanPowerAttackPoison, PoisonDamaged, PoisonFrequency, PoisonTick, PoisonChance,
				CanFire, CanPowerAttackFire, FireDamaged, FireFrequency, FireTick, FireChance,
				CanFreez, CanPowerAttackFreez, FreezDuration, FreezChance,
				CanPush, CanPowerAttackPush, PushDistance,
				RangedPowerAttackMinDamage, RangedPowerAttackMaxDamage,			
				CanThroughShoot, RangedUpAfterHit, CanThroughShoot, CanPowerAttackThroughShoot))
            {
                inventory.isFull[1] = true;
                Instantiate(itemButton, inventory.slots[1].transform, false);
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
