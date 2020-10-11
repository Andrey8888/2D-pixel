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
	//public GameObject tooltip;
	//public Text tooltipDescription;

    [Header("MeleeWeapons")]
    public int MeleeAttackMinDamage = 0;
    public int MeleeAttackMaxDamage = 0;
    public float MeleeAttackCooldownTime = 0.2f;
    public bool CanThirdAttackCriticalDamage = false; // or all attack
    //public bool CanPowerAttackCriticalDamage = false;
    [Range(1, 10)]
    public int MeleeCriticalDamageMultiply = 1;
    [Range(0, 99)]
    public int MeleeChanceCriticalDamage = 0;
    [Range(1, 10)]
    public int level = 1;  // Multiply damage
    [Range(0, 3)]
    public int Rarity = 1;

    [Header("PowerAttack")]
    public int MeleePowerAttackMinDamage = 0;
    public int MeleePowerAttackMaxDamage = 0;
    [Range(1, 10)]
    public int MeleePowerCriticalDamageMultiply = 1;
    [Range(0, 99)]
    public int MeleePowerChanceCriticalDamage = 0;
    public float MeleePowerAttackCooldownTime = 0.2f;


    [Header("Poison")]
    public bool CanPoison = false;
    public bool CanPowerAttackPoison = false;
    public int PoisonDamaged = 0;
    public int PoisonFrequency = 0;
    public int PoisonTick = 0;
    [Range(0, 99)]
    public int PoisonChance = 0;
    [Header("Fire")]
    public bool CanFire = false;
    public bool CanPowerAttackFire = false;
    public int FireDamaged = 0;
    public int FireFrequency = 0;
    public int FireTick = 0;
    [Range(0, 99)]
    public int FireChance = 0;
    [Header("Freez")]
    public bool CanFreez = false;
    public bool CanPowerAttackFreez = false;
    public int FreezDuration = 0;
    [Range(0, 99)]
    public int FreezChance = 0;
    [Header("Push")]
    public bool CanPush = false;
    public bool CanPowerAttackPush = false;
    public int PushDistance = 0;
	[Header("PushUp")]
    public bool CanPushUp = false;
    public bool CanPowerAttackPushUp = false;
    public int PushUpDistance = 0;
	[Header("Stun")]
    public bool CanStun = false;
    public bool CanPowerAttackStun = false;
    public int StunDuration = 0;
	[Range(0, 99)]
    public int StunChance = 100;
    [Header("Other")]
    public bool hasBlock = false;
    public bool hasSeries = false;
    public bool hasPowerAttackShell = false;
    public int StepUpAfterHit = 0;
	public int PowerStepUpAfterHit = 0;
    public bool MeleeTossingUp = false;
    public int MeleePopUpAfterHit = 0;
    public int MeleePowerPopUpAfterHit = 0;
	public int ManaCost = 0;
	public int PowerManaCost = 0;

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
		
		//Vector3 tooltipPos = Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(0, 50, 0));
		//tooltip.transform.position = tooltipPos;
		
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

        Vector3 Cursor = Input.mousePosition;
        Cursor = Camera.main.ScreenToWorldPoint(Cursor);
        Cursor.z = 0;

        if (pickupTimer <= 0f && dist < 50 && OnMouse)
        {
            Sprite.sprite = spriteHighlight;
            if (inventory.isFull[0] == true && Input.GetKey(KeyCode.E) )
            {
                pickupTimer = PickupTime;
                inventory.isFull[0] = false;
                //if(gameObject.activeSelf)
                {
                    player.MeleeWeapon.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + 20f);
                    player.MeleeWeapon.Sprite.sprite = player.MeleeWeapon.spriteNormal;
                    player.MeleeWeapon.gameObject.SetActive(true);
                }
                inventory.GetComponent<Inventory>().slots[0].GetComponent<Slot>().DropItem();
                player.DropMeleeWeapon();
            }

            if (inventory.isFull[0] == false && Input.GetKey(KeyCode.E))
            {
                if (player.PickUpMeleeWeapon(this, Type, MeleeAttackMinDamage * level, MeleeAttackMaxDamage * level,
                MeleeAttackCooldownTime, MeleeCriticalDamageMultiply, MeleeChanceCriticalDamage,
                MeleePowerAttackMinDamage, MeleePowerAttackMaxDamage,
                MeleePowerAttackCooldownTime, MeleePowerCriticalDamageMultiply, MeleePowerChanceCriticalDamage,
                StepUpAfterHit, hasSeries, hasBlock, CanThirdAttackCriticalDamage,
                CanPoison, CanPowerAttackPoison, PoisonDamaged, PoisonFrequency, PoisonTick, PoisonChance,
                CanFire, CanPowerAttackFire, FireDamaged, FireFrequency, FireTick, FireChance,
                CanFreez, CanPowerAttackFreez, FreezDuration, FreezChance,
                CanPush, CanPowerAttackPush, PushDistance, MeleeTossingUp, MeleePopUpAfterHit,
				CanPushUp, CanPowerAttackPushUp, PushUpDistance,
				CanFreez, CanPowerAttackFreez, FreezDuration, FreezChance, ManaCost, PowerManaCost, hasPowerAttackShell))
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
                    Sprite.sprite = spriteNormal;
                    Instantiate(itemButton, inventory.slots[0].transform, false);

                    gameObject.SetActive(false);
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

    void OnTriggerEnter2D(Collider2D other)
    {
        //if ((other.tag == "Player") && tooltip != null)
        //    tooltip.SetActive(true);
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
        //if ((other.tag == "Player") && tooltip != null)
        //    tooltip.SetActive(false);
    }

    public void OnPlayerTrigger(Player player)
    {
        if (inventory.isFull[0] == true && Input.GetKey(KeyCode.E) && pickupTimer <= 0f)
        {
            pickupTimer = PickupTime;

            inventory.isFull[0] = false;
            //if(gameObject.activeSelf)
            {
                player.MeleeWeapon.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + 20f);
                player.MeleeWeapon.Sprite.sprite = player.MeleeWeapon.spriteNormal;
                player.MeleeWeapon.gameObject.SetActive(true);
            }
            inventory.GetComponent<Inventory>().slots[0].GetComponent<Slot>().DropItem();
            player.DropMeleeWeapon();
        }

        if (inventory.isFull[0] == false && player.CompareTag("Player") && Input.GetKey(KeyCode.E))
        {
            if (player.PickUpMeleeWeapon(this, Type, MeleeAttackMinDamage * level, MeleeAttackMaxDamage * level,
                MeleeAttackCooldownTime, MeleeCriticalDamageMultiply, MeleeChanceCriticalDamage,
                MeleePowerAttackMinDamage, MeleePowerAttackMaxDamage,
                MeleePowerAttackCooldownTime, MeleePowerCriticalDamageMultiply, MeleePowerChanceCriticalDamage,
                StepUpAfterHit, hasSeries, hasBlock, CanThirdAttackCriticalDamage,
                CanPoison, CanPowerAttackPoison, PoisonDamaged, PoisonFrequency, PoisonTick, PoisonChance,
                CanFire, CanPowerAttackFire, FireDamaged, FireFrequency, FireTick, FireChance,
                CanFreez, CanPowerAttackFreez, FreezDuration, FreezChance,
                CanPush, CanPowerAttackPush, PushDistance, MeleeTossingUp, MeleePopUpAfterHit,
				CanPushUp, CanPowerAttackPushUp, PushUpDistance,
				CanFreez, CanPowerAttackFreez, FreezDuration, FreezChance, ManaCost, PowerManaCost, hasPowerAttackShell))
            {
		
                Pickable = false;
                // Disable
                this.RespawnTimer = recoveryTime;

                // Screenshake
                if (PixelCameraController.instance != null)
                {
                    PixelCameraController.instance.Shake(0.1f);
                }

                inventory.isFull[0] = true;
                Instantiate(itemButton, inventory.slots[0].transform, false);

                gameObject.SetActive(false);
            }
        }
    }

    public void OnPlayer(Player player)
    {
        inventory = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Inventory>();
        inventory.isFull[0] = true;
        if (player.PickUpMeleeWeapon(this, Type, MeleeAttackMinDamage * level, MeleeAttackMaxDamage * level,
            MeleeAttackCooldownTime, MeleeCriticalDamageMultiply, MeleeChanceCriticalDamage,
            MeleePowerAttackMinDamage, MeleePowerAttackMaxDamage,
            MeleePowerAttackCooldownTime, MeleePowerCriticalDamageMultiply, MeleePowerChanceCriticalDamage,
            StepUpAfterHit, hasSeries, hasBlock, CanThirdAttackCriticalDamage,
            CanPoison, CanPowerAttackPoison, PoisonDamaged, PoisonFrequency, PoisonTick, PoisonChance,
            CanFire, CanPowerAttackFire, FireDamaged, FireFrequency, FireTick, FireChance,
            CanFreez, CanPowerAttackFreez, FreezDuration, FreezChance,
            CanPush, CanPowerAttackPush, PushDistance, MeleeTossingUp, MeleePopUpAfterHit,
			CanPushUp, CanPowerAttackPushUp, PushUpDistance,
			CanFreez, CanPowerAttackFreez, FreezDuration, FreezChance, ManaCost, PowerManaCost, hasPowerAttackShell))
        {

        }
        Instantiate(itemButton, inventory.slots[0].transform, false);
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
