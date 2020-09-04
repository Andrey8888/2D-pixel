using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{

    [Header("Speed")]
    public Vector2 Speed;


    [Header("Layers")]
    public LayerMask solid_layer;

    [HideInInspector]
    public Health owner; // owner of the projectile
    private Vector2 Position; // Current position
    private Vector2 movementCounter = Vector2.zero;  // Counter for subpixel movement
    private Rigidbody2D rb2D; // Cached Rigidbody2D attached to the projectile

    private bool damageShow = false;
    [SerializeField]
    public Transform PopUpDamage;          // Всплывающий текст с уроном по монстру

    List<Health> healthsDamaged = new List<Health>(); // List to store healths damaged

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // keeping everything Pixel perfect
        Position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
        transform.position = Position;
    }

    void Update()
    {
        // Particle Pixel Perfect Movement
        movementCounter += Speed;

        var moveh = (int)Mathf.Round(this.movementCounter.x);
        if (moveh != 0)
        {
            movementCounter.x -= (float)moveh;
            Position.x += moveh;
        }

        var movev = (int)Mathf.Round(this.movementCounter.y);
        if (movev != 0)
        {
            movementCounter.y -= (float)movev;
            Position.y += movev;
        }

        // Move the rigidbody
        rb2D.MovePosition(Position);
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // if the projectile hit's a solid object, destroy it
        if (col.gameObject.layer == (int)Mathf.Log(solid_layer.value, 2))
        {
            DestroyMe();
            return;
        }

        var component = col.GetComponent<Health>();
        // If the target the hitbox collided with has a health component and it is not our owner and it is not on the already on the list of healths damaged by the current hitbox
        if (component != null && component != owner && !healthsDamaged.Contains(component) && component.health > 0)
        {
            // Add the health component to the list of damaged healths
            healthsDamaged.Add(component);

            var PlayerComponent = FindObjectOfType<Player>();
            var didDamage = false;

            if (PlayerComponent.PowerRangedAttack == false)
            {
                if (Random.Range(0, 100) < PlayerComponent.RangedCriticalDamageChance)
                {
                    CriticalDamage(PlayerComponent, component);
                    didDamage = true;
                }
                else
                {
                    Damage(PlayerComponent, component);
                    didDamage = true;
                }
            }
            else
            {
                if (Random.Range(0, 100) < PlayerComponent.RangedPowerChanceCriticalDamage)
                {
                    CriticalPowerDamage(PlayerComponent, component);
                    didDamage = true;
                }
                else
                {
                    PowerDamage(PlayerComponent, component);
                    didDamage = true;
                }
            }
            // Apply the damage
            //var didDamage = component.TakeDamage(DamageOnHit, false, 0, 0, 0, false, 0, 0, 0, false, 0, false, 0);
            // Destroy the projectile after applying damage
            if (didDamage)
            {
                DestroyMe();
            }
        }
    }

    private void Damage(Player PlayerComponent, Health component)
    {
        int dmg = (Random.Range(PlayerComponent.RangedAttackMinDamage, PlayerComponent.RangedAttackMaxDamage));
        component.TakeDamage(dmg, PlayerComponent.RangedAttackCanPoison, PlayerComponent.RangedPoisonDamaged,
        PlayerComponent.RangedPoisonFrequency, PlayerComponent.RangedPoisonTick, PlayerComponent.RangedPoisonChance, PlayerComponent.RangedAttackCanFire,
        PlayerComponent.RangedFireDamaged, PlayerComponent.RangedFireFrequency, PlayerComponent.RangedFireTick, PlayerComponent.RangedFireChance,
        PlayerComponent.RangedAttackCanPush, PlayerComponent.RangedPushDistance, PlayerComponent.RangedAttackCanFreez,
        PlayerComponent.RangedFreezDuration, PlayerComponent.RangedFreezChance);
        if (!damageShow)
        {
            Transform damagePopupTransform = Instantiate(PopUpDamage, transform.position, Quaternion.identity);
            DamagePopUp damagePopUp = damagePopupTransform.GetComponent<DamagePopUp>();
            damagePopUp.Setup(dmg, false);
            damageShow = true;
        }
    }

    private void CriticalDamage(Player PlayerComponent, Health component)
    {
        int dmg = (Random.Range(PlayerComponent.RangedAttackMinDamage, PlayerComponent.RangedAttackMaxDamage))
        * PlayerComponent.RangedCriticalDamageMultiply;
        component.TakeDamage(dmg, PlayerComponent.RangedAttackCanPoison, PlayerComponent.RangedPoisonDamaged,
        PlayerComponent.RangedPoisonFrequency, PlayerComponent.RangedPoisonTick, PlayerComponent.RangedPoisonChance, PlayerComponent.RangedAttackCanFire,
        PlayerComponent.RangedFireDamaged, PlayerComponent.RangedFireFrequency, PlayerComponent.RangedFireTick, PlayerComponent.RangedFireChance,
        PlayerComponent.RangedAttackCanPush, PlayerComponent.RangedPushDistance, PlayerComponent.RangedAttackCanFreez,
        PlayerComponent.RangedFreezDuration, PlayerComponent.RangedFreezChance);
        if (!damageShow)
        {
            Transform damagePopupTransform = Instantiate(PopUpDamage, transform.position, Quaternion.identity);
            DamagePopUp damagePopUp = damagePopupTransform.GetComponent<DamagePopUp>();
            damagePopUp.Setup(dmg, true);
            damageShow = true;
        }
    }

    private void PowerDamage(Player PlayerComponent, Health component)
    {
        int dmg = (Random.Range(PlayerComponent.RangedPowerAttackMinDamage, PlayerComponent.RangedPowerAttackMaxDamage));
        component.TakeDamage(dmg, PlayerComponent.RangedPowerAttackCanPoison, PlayerComponent.RangedPoisonDamaged,
        PlayerComponent.RangedPoisonFrequency, PlayerComponent.RangedPoisonTick, PlayerComponent.RangedPoisonChance, PlayerComponent.RangedPowerAttackCanFire,
        PlayerComponent.RangedFireDamaged, PlayerComponent.RangedFireFrequency, PlayerComponent.RangedFireTick, PlayerComponent.RangedFireChance,
        PlayerComponent.RangedPowerAttackCanPush, PlayerComponent.RangedPushDistance, PlayerComponent.RangedPowerAttackCanFreez,
        PlayerComponent.RangedFreezDuration, PlayerComponent.RangedFreezChance);
        if (!damageShow)
        {
            Transform damagePopupTransform = Instantiate(PopUpDamage, transform.position, Quaternion.identity);
            DamagePopUp damagePopUp = damagePopupTransform.GetComponent<DamagePopUp>();
            damagePopUp.Setup(dmg, false);
            damageShow = true;
        }
    }

    private void CriticalPowerDamage(Player PlayerComponent, Health component)
    {
        int dmg = (Random.Range(PlayerComponent.RangedPowerAttackMinDamage, PlayerComponent.RangedPowerAttackMaxDamage))
        * PlayerComponent.RangedPowerCriticalDamageMultiply;
        component.TakeDamage(dmg, PlayerComponent.RangedPowerAttackCanPoison, PlayerComponent.RangedPoisonDamaged,
        PlayerComponent.RangedPoisonFrequency, PlayerComponent.RangedPoisonTick, PlayerComponent.RangedPoisonChance, PlayerComponent.RangedPowerAttackCanFire,
        PlayerComponent.RangedFireDamaged, PlayerComponent.RangedFireFrequency, PlayerComponent.RangedFireTick, PlayerComponent.RangedFireChance,
        PlayerComponent.RangedPowerAttackCanPush, PlayerComponent.RangedPushDistance, PlayerComponent.RangedPowerAttackCanFreez,
        PlayerComponent.RangedFreezDuration, PlayerComponent.RangedFreezChance);
        if (!damageShow)
        {
            Transform damagePopupTransform = Instantiate(PopUpDamage, transform.position, Quaternion.identity);
            DamagePopUp damagePopUp = damagePopupTransform.GetComponent<DamagePopUp>();
            damagePopUp.Setup(dmg, false);
            damageShow = true;
        }
    }
}

