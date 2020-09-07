using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupRangedWeapons : Actor
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

    [Header("RangedWeapons")]
    public int RangedAttackMinDamage = 0;
    public int RangedAttackMaxDamage = 0;
    public float RangedAttackCooldownTime = 1;
    [Range(1, 10)]
    public int RangedAttackCriticalDamageMultiply = 1;
    [Range(0, 99)]
    public int RangedAttackChanceCriticalDamage = 0;
    public int ShellsCount = 0;
    [Range(1, 10)]
    public int level = 1;  // Multiply damage
    [Range(0, 3)]
    public int Rarity = 1;

    [Header("PowerAttack")]
    public int RangedPowerAttackMinDamage = 0;
    public int RangedPowerAttackMaxDamage = 0;
    public float RangedPowerAttackCooldownTime = 10f;
    [Range(1, 10)]
    public int RangedPowerCriticalDamageMultiply = 1;
    [Range(0, 99)]
    public int RangedPowerChanceCriticalDamage = 0;
    public int PowerShellsCount = 0;

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
    public bool CanPowerAttackPush = false;
    public int PushDistance = 0;
    [Header("Other")]
    public bool CanThroughShoot = false;
    public bool TossingUp = false;
    public int UpAfterHit = 0;
    public bool Aiming = false;
    public int StepUpAfterHit = 0;
    public int RangedUpAfterHit = 0;
    [Header("PowerAttackOther")]
    public int PowerAttackPopUpAfterHit = 0;
    public bool CanPowerAttackThroughShoot = false;
    public bool PowerAttackAiming = false;


    public RangedWeapon Type = RangedWeapon.Bow;

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
        player = FindObjectOfType<Player>();
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
        inventory = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Inventory>();
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

        if (pickupTimer <= 0f && dist < 50 && OnMouse)
        {
            Sprite.sprite = spriteHighlight;

            if (inventory.isFull[1] == true && Input.GetKey(KeyCode.E) )
            {
                pickupTimer = PickupTime;
                inventory.isFull[1] = false;
                inventory.GetComponent<Inventory>().slots[1].GetComponent<Slot>().DropItem();
                player.DropRangedWeapon();
            }

            if (inventory.isFull[1] == false && Input.GetKey(KeyCode.E))
            {
                if (player.PickUpRangedWeapon(Type, RangedAttackMinDamage, RangedAttackMaxDamage, ShellsCount,
                    RangedAttackCooldownTime, RangedAttackCriticalDamageMultiply, RangedAttackChanceCriticalDamage,
                    RangedPowerAttackMinDamage, RangedPowerAttackMaxDamage, PowerShellsCount,
                    RangedPowerAttackCooldownTime, RangedPowerCriticalDamageMultiply, RangedPowerChanceCriticalDamage,
                    CanPoison, CanPowerAttackPoison, PoisonDamaged, PoisonFrequency, PoisonTick, PoisonChance,
                    CanFire, CanPowerAttackFire, FireDamaged, FireFrequency, FireTick, FireChance,
                    CanFreez, CanPowerAttackFreez, FreezDuration, FreezChance,
                    CanPush, CanPowerAttackPush, PushDistance,
                    CanThroughShoot, RangedUpAfterHit, CanThroughShoot, CanPowerAttackThroughShoot,
                    Aiming, StepUpAfterHit, PowerAttackPopUpAfterHit, PowerAttackAiming))
                {
                    this.RespawnTimer = recoveryTime;

                    // Screenshake
                    if (PixelCameraController.instance != null)
                    {
                        PixelCameraController.instance.Shake(0.1f);
                    }

                    inventory.isFull[1] = true;
                    Pickable = false;
                    // Disable
                    Sprite.enabled = false;
                    Sprite.sprite = spriteNormal;
                    Instantiate(itemButton, inventory.slots[1].transform, false);

                    Destroy(gameObject);
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
        if (inventory.isFull[1] == true && Input.GetKey(KeyCode.E) && pickupTimer <= 0f)
        {
            pickupTimer = PickupTime;

            inventory.isFull[1] = false;
            inventory.GetComponent<Inventory>().slots[1].GetComponent<Slot>().DropItem();
            player.DropRangedWeapon();
        }

        if (inventory.isFull[1] == false && player.CompareTag("Player"))
        {
            if (player.PickUpRangedWeapon(Type, RangedAttackMinDamage, RangedAttackMaxDamage, ShellsCount,
                RangedAttackCooldownTime, RangedAttackCriticalDamageMultiply, RangedAttackChanceCriticalDamage,
                RangedPowerAttackMinDamage, RangedPowerAttackMaxDamage, PowerShellsCount,
                RangedPowerAttackCooldownTime, RangedPowerCriticalDamageMultiply, RangedPowerChanceCriticalDamage,
                CanPoison, CanPowerAttackPoison, PoisonDamaged, PoisonFrequency, PoisonTick, PoisonChance,
                CanFire, CanPowerAttackFire, FireDamaged, FireFrequency, FireTick, FireChance,
                CanFreez, CanPowerAttackFreez, FreezDuration, FreezChance,
                CanPush, CanPowerAttackPush, PushDistance,
                CanThroughShoot, RangedUpAfterHit, CanThroughShoot, CanPowerAttackThroughShoot,
                Aiming, StepUpAfterHit, PowerAttackPopUpAfterHit, PowerAttackAiming))
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
    }

    public void OnPlayer(Player player)
    {
        inventory = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Inventory>();
        inventory.isFull[1] = true;
        if (player.PickUpRangedWeapon(Type, RangedAttackMinDamage, RangedAttackMaxDamage, ShellsCount,
            RangedAttackCooldownTime, RangedAttackCriticalDamageMultiply, RangedAttackChanceCriticalDamage,
            RangedPowerAttackMinDamage, RangedPowerAttackMaxDamage, PowerShellsCount,
            RangedPowerAttackCooldownTime, RangedPowerCriticalDamageMultiply, RangedPowerChanceCriticalDamage,
            CanPoison, CanPowerAttackPoison, PoisonDamaged, PoisonFrequency, PoisonTick, PoisonChance,
            CanFire, CanPowerAttackFire, FireDamaged, FireFrequency, FireTick, FireChance,
            CanFreez, CanPowerAttackFreez, FreezDuration, FreezChance,
            CanPush, CanPowerAttackPush, PushDistance,
            CanThroughShoot, RangedUpAfterHit, CanThroughShoot, CanPowerAttackThroughShoot,
            Aiming, StepUpAfterHit, PowerAttackPopUpAfterHit, PowerAttackAiming))
        {

        }
        Instantiate(itemButton, inventory.slots[1].transform, false);
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
