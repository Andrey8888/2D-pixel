using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class PickupMeleeWeapons : MonoBehaviour
{
    #region Params
    [Header("MeleeWeapons")]
    [HorizontalGroup("base", LabelWidth = 80)]
    [VerticalGroup("base/column 1")]
    public MeleeWeapon Type = MeleeWeapon.Sword;
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
    public int MeleeAttackMinDamage = 0;
    [TabGroup("NormalAttack")]
    public int MeleeAttackMaxDamage = 0;
    [TabGroup("NormalAttack")]
    public float MeleeAttackCooldownTime = 0.2f;
    [TabGroup("NormalAttack")]
    public bool CanThirdAttackCriticalDamage = false; // or all attack
                                                      //public bool CanPowerAttackCriticalDamage = false;
    [TabGroup("NormalAttack")]
    [Range(1, 10)]
    public int MeleeCriticalDamageMultiply = 1;
    [TabGroup("NormalAttack")]
    [Range(0, 99)]
    public int MeleeChanceCriticalDamage = 0;
    [TabGroup("NormalAttack")]
    public float MeleeAttackSpeed = 1f;
    [TabGroup("NormalAttack")]
    public float SecondSwordAttackSpeed = 1f;
    [TabGroup("NormalAttack")]
    public float ThirdSwordAttackSpeed = 1f;
    [TabGroup("NormalAttack")]
    public int ManaCost = 0;

    [Header("PowerAttack")]
    [TabGroup("PowerAttack")]
    public int MeleePowerAttackMinDamage = 0;
    [TabGroup("PowerAttack")]
    public int MeleePowerAttackMaxDamage = 0;
    [TabGroup("PowerAttack")]
    [Range(1, 10)]
    public int MeleePowerCriticalDamageMultiply = 1;
    [TabGroup("PowerAttack")]
    [Range(0, 99)]
    public int MeleePowerChanceCriticalDamage = 0;
    [TabGroup("PowerAttack")]
    public float MeleePowerAttackCooldownTime = 0.2f;
    [TabGroup("PowerAttack")]
    public float MeleePowerAttackSpeed = 1f;
    [HideInInspector]
    public int MeleePowerShellsCountCurent = 0;
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
    public int PoisonChance = 0;

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
    public int FireChance = 0;

    [TabGroup("Tab Group 2", "Freez")]
    [Header("Freez")]
    public bool CanFreez = false;
    [TabGroup("Tab Group 2", "Freez")]
    public bool CanPowerAttackFreez = false;
    [TabGroup("Tab Group 2", "Freez")]
    public int FreezDuration = 0;
    [TabGroup("Tab Group 2", "Freez")]
    [Range(0, 99)]
    public int FreezChance = 0;

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
    public bool hasBlock = false;
    [TabGroup("Tab Group 2", "Other")]
    public bool hasSeries = false;
    [TabGroup("Tab Group 2", "Other")]
    public bool hasPowerAttackShell = false;
    [TabGroup("Tab Group 2", "Other")]
    public int StepUpAfterHit = 0;
    [TabGroup("Tab Group 2", "Other")]
    public int PowerStepUpAfterHit = 0;
    [TabGroup("Tab Group 2", "Other")]
    public bool MeleeTossingUp = false;
    [TabGroup("Tab Group 2", "Other")]
    public int MeleePopUpAfterHit = 0;
    [TabGroup("Tab Group 2", "Other")]
    public int MeleePowerPopUpAfterHit = 0;
    #endregion

    public Sprite spriteHighlight;
    public Sprite spriteNormal;

    public GameObject itemButton;

    public GameObject tooltip;
    public Text tooltipDescription;

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
        Sprite.enabled = true;
        MeleePowerShellsCountCurent = PowerShells;

        //tooltipDescription = ("MeleeWeapon " + Type + "; Level " + level + "; Min Damage: " + MeleeAttackMinDamage + "' Max Damage: " + MeleeAttackMinDamage )
        //tooltip.Text = tooltipDescription;

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

        //Vector3 tooltipPos = Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(0, 50, 0));
        //tooltip.transform.position = tooltipPos;

        Vector3 Cursor = Input.mousePosition;
        Cursor = Camera.main.ScreenToWorldPoint(Cursor);
        Cursor.z = 0;

        if (dist < 50 && OnMouse)
        {
            Sprite.sprite = spriteHighlight;
            if (inventory.isFull[0] == true && Input.GetKey(KeyCode.E))
            {
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
                if (player.PickUpMeleeWeapon(this))
                {

                    // Screenshake
                    if (PixelCameraController.instance != null)
                    {
                        PixelCameraController.instance.Shake(0.1f);
                    }

                    inventory.isFull[0] = true;
                    // Disable
                    Sprite.sprite = spriteNormal;
                    Instantiate(itemButton, inventory.slots[0].transform, false);

                    gameObject.SetActive(false);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //if ((other.tag == "Player") && tooltip != null)
        //    tooltip.SetActive(true);
    }

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
        //    tooltip.SetActive(false);
    }

    public void OnPlayerTrigger(Player player)
    {
        if (inventory.isFull[0] == true && Input.GetKey(KeyCode.E))
        {
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
            if (IsSell)
            {
                if (!Sell(player))
                    return;
            }

            if (player.PickUpMeleeWeapon(this))
            {
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

    public bool Sell(Player player)
    {
        var money = player.GetComponent<Gold>();
        if (money.Spend_Money(cost))
            return true;
        else return false;
    }

    public void OnPlayer(Player player)
    {
        inventory = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Inventory>();
        inventory.isFull[0] = true;
        MeleePowerShellsCountCurent = PowerShells;

        if (player.PickUpMeleeWeapon(this))
        {
            MeleePowerShellsCountCurent = PowerShells;
        }
        Instantiate(itemButton, inventory.slots[0].transform, false);
    }
}
