using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public class PickupMeleeWeapons : Actor
{

    public float Gravity = 900f; // Gravity force
    public float MaxFall = -240f; // Maximun fall speed

    private bool Pickable = true;
    private float RespawnTimer = 0f;
    public float recoveryTime = 2.5f;

    public float PickupTime; // Total Time it takes the player to respawn
    private float pickupTimer = 1f; // Variable to store the timer of the respawn
    private Inventory inventory;
    public GameObject itemButton;
	
	[Header("MeleeWeapons")]
	public int MeleeAttackMinDamage = 0;
	public int MeleeAttackMaxDamage = 0;
    public float MeleeAttackCooldownTime = 0.2f;
    public bool CanThirdAttackCriticalDamage = false; // or all attack
    //public bool CanPowerAttackCriticalDamage = false;
    [Range(1, 10)]
    public int CriticalDamageMultiply = 1;
    [Range(0, 99)]
    public int ChanceCriticalDamage = 0;
    [Range(1, 10)]
    public int level = 1;  // Multiply damage
    public int MeleePowerAttackMinDamage = 0;
    public int MeleePowerAttackMaxDamage = 0;

    [Header("Poison")]
	public bool CanPoison = false;
    public bool CanPowerAttackPoison = false;
    public int PoisonDamaged = 0;
	public int PoisonFrequency = 0;
	public int PoisonTick = 0;
	[Header("Fire")]
	public bool CanFire = false;
    public bool CanPowerAttackFire = false;
    public int FireDamaged = 0;
	public int FireFrequency = 0;
	public int FireTick = 0;
    [Header("Freez")]
    public bool CanFreez = false;
    public bool CanPowerAttackFreez = false;
    public int FreezDuration = 0;
    [Header("Push")]
    public bool CanPush = false;
    public bool CanPowerAttackPush = false;
    public int PushDistance = 0;
    [Header("Other")]
	public bool hasBlock = false;
    public bool hasSeries = false;
    public int StepUpAfterHit = 0;
    public bool MeleeTossingUp = false;
    public int MeleePopUpAfterHit = 0;

	public MeleeWeapon Type = MeleeWeapon.Sword;
	
    [SerializeField]
    public SpriteRenderer Sprite;

    public Sprite spriteHighlight;
    public Sprite spriteNormal;
    private bool OnTrigger;
    private Player player;
    private bool OnMouse;
    public float dist = 50;
    void Start()
    {
        OnMouse = false;
        Sprite.enabled = true;
    }
    void OnMouseEnter()
    {
        OnMouse = true;
    }

    void OnMouseExit()
    {
        Sprite.sprite = spriteNormal;
        OnMouse = false;
    }
    new void Awake()
    {
        base.Awake();
        Sprite.enabled = true;
        if (Sprite == null)
        {
            Sprite = GetComponent<SpriteRenderer>();

            if (Sprite == null)
            {
                Debug.Log("This refill has no spriterenderer attached to it");
            }
        }
        player = FindObjectOfType<Player>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    // Update is called once per frame
    new void Update()
    {
        // Update variables
        wasOnGround = onGround;
        onGround = OnGround();
        // Gravity
        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }

        dist = Vector3.Distance(player.transform.position, transform.position);
        if (pickupTimer > 0f)
        {
            pickupTimer -= Time.deltaTime;
        }

        Vector3 Cursor = Input.mousePosition;
        Cursor = Camera.main.ScreenToWorldPoint(Cursor);
        Cursor.z = 0;

        if (dist < 50 && OnMouse)
        {
            Sprite.sprite = spriteHighlight;
            if (inventory.isFull[0] == false && Input.GetKey(KeyCode.E))
            {
                if (player.PickUpMeleeWeapon(Type, MeleeAttackMinDamage * level,MeleeAttackMaxDamage * level, 
                MeleeAttackCooldownTime, CriticalDamageMultiply, ChanceCriticalDamage,
                StepUpAfterHit, hasSeries, hasBlock, CanThirdAttackCriticalDamage,
                CanPoison, CanPowerAttackPoison, PoisonDamaged, PoisonFrequency, PoisonTick, 
                CanFire, CanPowerAttackFire, FireDamaged, FireFrequency, FireTick,
                CanFreez, CanPowerAttackFreez, FreezDuration,
                CanPush, CanPowerAttackPush, PushDistance, 
                MeleePowerAttackMinDamage, MeleePowerAttackMaxDamage, MeleeTossingUp, MeleePopUpAfterHit))
                {
                    this.RespawnTimer = recoveryTime;

                    // Screenshake
                    if (PixelCameraController.instance != null)
                    {
                        PixelCameraController.instance.Shake(0.1f);
                    }

                    inventory.isFull[0] = true;
                    Pickable = false;
                    // Disable
                    Sprite.enabled = false;
                    Sprite.sprite = spriteNormal;
                    Instantiate(itemButton, inventory.slots[0].transform, false);

                    Destroy(gameObject);
                }
            }

            else
            {
                if (Input.GetKey(KeyCode.E) && pickupTimer <= 0f)
                {
                    pickupTimer = PickupTime;

                    inventory.isFull[0] = false;
                    player.GetComponent<Inventory>().slots[0].GetComponent<Slot>().DropItem();
                    player.DropMeleeWeapon();
                }
            }
        }
    }

    void LateUpdate()
    {
        // Vertical movement
        var movev = base.MoveV(Speed.y * Time.deltaTime);
        if (movev)
            Speed.y = 0;
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Pickable)
        {
            Sprite.sprite = spriteHighlight;
            var playercomponent = other.GetComponent<Player>();
            if (playercomponent != null)
            {
                OnPlayerTrigger(playercomponent);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Pickable)
        {
            Sprite.sprite = spriteNormal;
        }
    }

    public void OnPlayerTrigger(Player player)
    {
        inventory = FindObjectOfType<Inventory>();
        if (inventory.isFull[0] == false && player.CompareTag("Player"))
                {
                    if (player.PickUpMeleeWeapon(Type, MeleeAttackMinDamage * level, MeleeAttackMaxDamage * level,
                    MeleeAttackCooldownTime, CriticalDamageMultiply, ChanceCriticalDamage,
                    StepUpAfterHit, hasSeries, hasBlock, CanThirdAttackCriticalDamage,
                    CanPoison, CanPowerAttackPoison, PoisonDamaged, PoisonFrequency, PoisonTick,
                    CanFire, CanPowerAttackFire, FireDamaged, FireFrequency, FireTick,
                    CanFreez, CanPowerAttackFreez, FreezDuration,
                    CanPush, CanPowerAttackPush, PushDistance,
                    MeleePowerAttackMinDamage, MeleePowerAttackMaxDamage, MeleeTossingUp, MeleePopUpAfterHit))
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
                else
                {
                    if (Input.GetKey(KeyCode.E) && pickupTimer <= 0f)
                    {
                        pickupTimer = PickupTime;

                        inventory.isFull[0] = false;
                        player.GetComponent<Inventory>().slots[0].GetComponent<Slot>().DropItem();
                        player.DropMeleeWeapon();
                    }
                }
    }

    public void OnPlayer(Player player)
    {
        inventory = FindObjectOfType<Inventory>();
        if (player.PickUpMeleeWeapon(Type, MeleeAttackMinDamage * level, MeleeAttackMaxDamage * level,
            MeleeAttackCooldownTime, CriticalDamageMultiply, ChanceCriticalDamage,
            StepUpAfterHit, hasSeries, hasBlock, CanThirdAttackCriticalDamage,
            CanPoison, CanPowerAttackPoison, PoisonDamaged, PoisonFrequency, PoisonTick,
            CanFire, CanPowerAttackFire, FireDamaged, FireFrequency, FireTick,
            CanFreez, CanPowerAttackFreez, FreezDuration,
            CanPush, CanPowerAttackPush, PushDistance,
            MeleePowerAttackMinDamage, MeleePowerAttackMaxDamage, MeleeTossingUp, MeleePopUpAfterHit))
            {
                inventory.isFull[0] = true;
                Instantiate(itemButton, inventory.slots[0].transform, false);
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
