using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupRangedWeapons : MonoBehaviour
{
#region Params
    [Header("RangedWeapons")]
    [HorizontalGroup("base", LabelWidth = 80)]
    [VerticalGroup("base/column 1")]
	public RangedWeapon Type = RangedWeapon.Bow;
	[VerticalGroup("base/column 1")]
    public SpriteRenderer Sprite;
	[VerticalGroup("base/column 1")]
	public bool IsSell = false;
	[VerticalGroup("base/column 1")]
	public int cost = 0;
    [VerticalGroup("base/column 1")]
    public int Rarity = 1;
    [VerticalGroup("base/column 1")]
    [Range(1, 10)]
    public int level = 1;  // Multiply damage

    private Inventory inventory;

    [Header("NormalAttack")]
    [TabGroup("NormalAttack")]
    public int RangedAttackMinDamage = 0;
    [TabGroup("NormalAttack")]
    public int RangedAttackMaxDamage = 0;
    [TabGroup("NormalAttack")]
    public float RangedAttackCooldownTime = 1;
    [Range(1, 10)]
    [TabGroup("NormalAttack")]
    public int RangedAttackCriticalDamageMultiply = 1;
    [TabGroup("NormalAttack")]
    [Range(0, 99)]
    public int RangedAttackChanceCriticalDamage = 0;
	[TabGroup("NormalAttack")]
    public float RangedAttackSpeed = 1f;
    [HideInInspector]
    public int RangedShellsCountCurent = 0;
    [TabGroup("NormalAttack")]
    public int ShellsCount = 0;
    [TabGroup("NormalAttack")]
    public int ManaCost = 0;

    [Header("PowerAttack")]
    [TabGroup("PowerAttack")]
    public int RangedPowerAttackMinDamage = 0;
    [TabGroup("PowerAttack")]
    public int RangedPowerAttackMaxDamage = 0;
    [TabGroup("PowerAttack")]
    public float RangedPowerAttackCooldownTime = 10f;
    [TabGroup("PowerAttack")]
    [Range(1, 10)]
    public int RangedPowerCriticalDamageMultiply = 1;
    [TabGroup("PowerAttack")]
    [Range(0, 99)]
    public int RangedPowerChanceCriticalDamage = 0;
	[TabGroup("PowerAttack")]
    public float RangedPowerAttackSpeed = 1f;
    [HideInInspector]
    public int RangedPowerShellsCountCurent = 0;
    [TabGroup("PowerAttack")]
    public int PowerShells = 0;
    [TabGroup("PowerAttack")]
    public int PowerManaCost = 0;


    [TabGroup("Tab Group 2", "Poison")]
    [Header("Poison")]
    public bool CanPoison = false;
    [TabGroup("Tab Group 2", "Poison")]
    public bool CanPowerAttackPoison = false;
    [TabGroup("Tab Group 2", "Poison")]
    public int PoisonDamaged = 0;
    [TabGroup("Tab Group 2", "Poison")]
    public int PoisonFrequency = 0;
    [TabGroup("Tab Group 2", "Poison")]
    public int PoisonTick = 0;
    [TabGroup("Tab Group 2", "Poison")]
    [Range(0, 99)]
    public int PoisonChance = 100;

    [TabGroup("Tab Group 2", "Fire")]
    [Header("Fire")]
    public bool CanFire = false;
    [TabGroup("Tab Group 2", "Fire")]
    public bool CanPowerAttackFire = false;
    [TabGroup("Tab Group 2", "Fire")]
    public int FireDamaged = 0;
    [TabGroup("Tab Group 2", "Fire")]
    public int FireFrequency = 0;
    [TabGroup("Tab Group 2", "Fire")]
    public int FireTick = 0;
    [TabGroup("Tab Group 2", "Fire")]
    [Range(0, 99)]
    public int FireChance = 100;

    [TabGroup("Tab Group 2", "Freez")]
    [Header("Freez")]
    public bool CanFreez = false;
    [TabGroup("Tab Group 2", "Freez")]
    public bool CanPowerAttackFreez = false;
    [TabGroup("Tab Group 2", "Freez")]
    public int FreezDuration = 0;
    [TabGroup("Tab Group 2", "Freez")]
    [Range(0, 99)]
    public int FreezChance = 100;

    [Header("Push")]
    [TabGroup("Tab Group 2", "Push")]
    public bool CanPush = false;
    [TabGroup("Tab Group 2", "Push")]
    public bool CanPowerAttackPush = false;
    [TabGroup("Tab Group 2", "Push")]
    public int PushDistance = 0;
    [TabGroup("Tab Group 2", "Push")]
    public bool CanPushUp = false;
    [TabGroup("Tab Group 2", "Push")]
    public bool CanPowerAttackPushUp = false;
    [TabGroup("Tab Group 2", "Push")]
    public int PushUpDistance = 0;

    [Header("Stun")]
    [TabGroup("Tab Group 2", "Stun")]
    public bool CanStun = false;
    [TabGroup("Tab Group 2", "Stun")]
    public bool CanPowerAttackStun = false;
    [TabGroup("Tab Group 2", "Stun")]
    public int StunDuration = 0;
    [TabGroup("Tab Group 2", "Stun")]
    [Range(0, 99)]
    public int StunChance = 100;

    [Header("Other")]
    [TabGroup("Tab Group 2", "Other")]
    public bool CanThroughShoot = false;
    [TabGroup("Tab Group 2", "Other")]
    public bool TossingUp = false;
    [TabGroup("Tab Group 2", "Other")]
    public int UpAfterHit = 0;
    [TabGroup("Tab Group 2", "Other")]
    public bool Aiming = false;
    [TabGroup("Tab Group 2", "Other")]
    public int StepUpAfterHit = 0;
    [TabGroup("Tab Group 2", "Other")]
    public int RangedUpAfterHit = 0;

    [Header("Power")]
    [TabGroup("Tab Group 2", "Power")]
    public int PowerAttackPopUpAfterHit = 0;
    [TabGroup("Tab Group 2", "Power")]
    public bool CanPowerAttackThroughShoot = false;
    [TabGroup("Tab Group 2", "Power")]
    public int PowerAttackStepUpAfterHit = 0;
    [TabGroup("Tab Group 2", "Power")]
    public bool PowerAttackAiming = false;
    [TabGroup("Tab Group 2", "Power")]
    public bool RangedBlink = false;
    [TabGroup("Tab Group 2", "Power")]
    public bool RangedHeal = false;
    [TabGroup("Tab Group 2", "Power")]
    public int RangedHealCount = 0;
    #endregion

    public Sprite spriteHighlight;
    public Sprite spriteNormal;
	
	public GameObject itemButton;
	
	public Text tooltip;
    public string tooltipDescription;
	
    private bool OnTrigger;
    private Player player;
    private bool OnMouse;
    private float dist = 50;

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
    void Awake()
    {
        RangedShellsCountCurent = ShellsCount;
        RangedPowerShellsCountCurent = PowerShells;
        Sprite.enabled = true;

        //tooltipDescription = ("RangedWeapon " + Type + "; Level " + level + "; Min Damage: " + RangedAttackMinDamage + "' Max Damage: " + RangedAttackMinDamage);
		//tooltip.text = tooltipDescription;
		
		
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
    void Update()
    {
        dist = Vector3.Distance(player.transform.position, transform.position);

		if(tooltip != null)
		{
			Vector3 tooltipPos = Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(0, 50, 0));
			tooltip.transform.position = tooltipPos;
		}		

        Vector3 Cursor = Input.mousePosition;
        Cursor = Camera.main.ScreenToWorldPoint(Cursor);
        Cursor.z = 0;

        if (dist < 50 && OnMouse)
        {
            Sprite.sprite = spriteHighlight;

            if (inventory.isFull[1] == true && Input.GetKey(KeyCode.E))
            {
                inventory.isFull[1] = false;
                //if(gameObject.activeSelf)
                {
                    player.RangedWeapon.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + 20f);
                    player.RangedWeapon.Sprite.sprite = player.RangedWeapon.spriteNormal;
                    player.RangedWeapon.gameObject.SetActive(true);
                }
                inventory.GetComponent<Inventory>().slots[1].GetComponent<Slot>().DropItem();
                player.DropRangedWeapon();
            }

            if (inventory.isFull[1] == false && Input.GetKey(KeyCode.E))
            {
                if (player.PickUpRangedWeapon(this))
                {
                    //// Screenshake
                    //if (PixelCameraController.instance != null)
                    //{
                    //    PixelCameraController.instance.Shake(0.1f);
                    //}

                    inventory.isFull[1] = true;
					
                    // Disable
                    Sprite.sprite = spriteNormal;
                    Instantiate(itemButton, inventory.slots[1].transform, false);

                    gameObject.SetActive(false);
                }
            }
        }
    }

    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    if ((other.tag == "Player") && tooltip != null)
      //tooltip.SetActive(true);
    //}	
	
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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
        if (other.CompareTag("Player"))
        {
            Sprite.sprite = spriteNormal;
        }
		//if ((other.tag == "Player") && tooltip != null)
	        //tooltip.SetActive(false);
    }

    public void OnPlayerTrigger(Player player)
    {
        if (inventory.isFull[1] == true && Input.GetKey(KeyCode.E))
        {
			if(IsSell)
			{
				if(!Sell(player))
				return;
			}
			
            inventory.isFull[1] = false;
            //if(gameObject.activeSelf)
            {
                player.RangedWeapon.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + 20f);
                player.RangedWeapon.Sprite.sprite = player.RangedWeapon.spriteNormal;
                player.RangedWeapon.gameObject.SetActive(true);
            }
            inventory.GetComponent<Inventory>().slots[1].GetComponent<Slot>().DropItem();
            player.DropRangedWeapon();
        }

        if (inventory.isFull[1] == false && player.CompareTag("Player"))
        {
            if (player.PickUpRangedWeapon(this))
            {

                //// Screenshake
                //if (PixelCameraController.instance != null)
                //{
                //    PixelCameraController.instance.Shake(0.1f);
                //}

                inventory.isFull[1] = true;
                Instantiate(itemButton, inventory.slots[1].transform, false);

                gameObject.SetActive(false);
            }
        }
    }

	public bool Sell(Player player)
	{
		var money = player.GetComponent<Gold>();
		if(money.Spend_Money(cost))
			return true;
			else return false;
	}
	
    public void OnPlayer(Player player)
    {
        inventory = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Inventory>();
        inventory.isFull[1] = true;
        RangedShellsCountCurent = ShellsCount;
        RangedPowerShellsCountCurent = PowerShells;
        if (player.PickUpRangedWeapon(this))
        {
            RangedShellsCountCurent = ShellsCount;
            RangedPowerShellsCountCurent = PowerShells;
        }
        Instantiate(itemButton, inventory.slots[1].transform, false);
    }
}
