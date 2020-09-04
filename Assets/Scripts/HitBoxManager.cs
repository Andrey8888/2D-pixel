using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxManager : MonoBehaviour
{

    // Set these in the editor
    public PolygonCollider2D[] attack;
    public PolygonCollider2D[] powerAttack;
    public MeleeWeapon MeleeWeaponType;

    // Used for organization
    private PolygonCollider2D[] colliders;

    // Collider on this game object
    private PolygonCollider2D localCollider;

    private bool damageShow = false;
    [SerializeField]
    public Transform PopUpDamage;          // Всплывающий текст с уроном по монстру

    // We say box, but we're still using polygons.
    public enum hitBoxes
    {
        AttackHitBox, // Add more of these here and on line 8 and 37 to add more possible hitboxes per character
        clear // special case to remove all boxes
    }

    [SerializeField]
    private Health owner;

    List<Health> healthsDamaged = new List<Health>();

    void Start()
    {
        if (owner == null)
        {
            owner = GetComponentInParent<Health>();
            if (owner == null)
            {
                Debug.Log("There is no owner health component asigned to this HitBoxManager");
            }
        }
        // Set up an array so our script can more easily set up the hit boxes
        colliders = new PolygonCollider2D[] { attack[0] };
        // Create a polygon collider
        localCollider = gameObject.AddComponent<PolygonCollider2D>();
        localCollider.isTrigger = true; // Set as a trigger so it doesn't collide with our environment
        localCollider.pathCount = 0; // Clear auto-generated polygons
    }

    public void ChangeCollider(int i)
    {
        if (owner == null)
        {
            owner = GetComponentInParent<Health>();
            if (owner == null)
            {
                Debug.Log("There is no owner health component asigned to this HitBoxManager");
            }
        }
        colliders = new PolygonCollider2D[] { attack[i] };
        // Create a polygon collider
        localCollider = gameObject.AddComponent<PolygonCollider2D>();
        localCollider.isTrigger = true; // Set as a trigger so it doesn't collide with our environment
        localCollider.pathCount = 0; // Clear auto-generated polygons
    }

    public void ChangePowerCollider(int i)
    {
        if (owner == null)
        {
            owner = GetComponentInParent<Health>();
            if (owner == null)
            {
                Debug.Log("There is no owner health component asigned to this HitBoxManager");
            }
        }
        colliders = new PolygonCollider2D[] { powerAttack[i] };
        // Create a polygon collider
        localCollider = gameObject.AddComponent<PolygonCollider2D>();
        localCollider.isTrigger = true; // Set as a trigger so it doesn't collide with our environment
        localCollider.pathCount = 0; // Clear auto-generated polygons
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        var component = col.GetComponent<Health>();
        // If the target the hitbox collided with has a health component and it is not our owner and it is not on the already on the list of healths damaged by the current hitbox
        if (component != null && component != owner && !healthsDamaged.Contains(component) && component.health > 0)
        {
            // Try to Apply the damage
            var PlayerComponent = GetComponentInParent<Player>();
            var didDamage = false;

            if (PlayerComponent.PowerMeleeAttack == false)
            {
                if (PlayerComponent.MeleeCanThirdAttackCriticalDamage == true)
                {
                    if (PlayerComponent.hitCount == 3)
                    {
                        if (Random.Range(0, 100) < PlayerComponent.MeleeCriticalDamageChance)
                        {
                            CriticalDamage(PlayerComponent, component, didDamage);
                        }
                    }
                    else
                    {
                        Damage(PlayerComponent, component, didDamage);
                    }
                }
                else
                {
                    if (Random.Range(0, 100) < PlayerComponent.MeleeCriticalDamageChance)
                    {
                        CriticalDamage(PlayerComponent, component, didDamage);
                    }
                    else
                    {
                        Damage(PlayerComponent, component, didDamage);
                    }
                }
                if (didDamage)
                {
                    // Add the health component to the list of damaged healths
                    healthsDamaged.Add(component);
                }
            }
            else
            {
                if (Random.Range(0, 100) < PlayerComponent.MeleePowerChanceCriticalDamage)
                {
                    PowerCriticalDamage(PlayerComponent, component, didDamage);
                }
                else
                {
                    PowerDamage(PlayerComponent, component, didDamage);
                }
            }
        }
        damageShow = false;
    }

    private void Damage(Player PlayerComponent, Health component, bool didDamage)
    {
        int dmg = (Random.Range(PlayerComponent.MeleeAttackMinDamage, PlayerComponent.MeleeAttackMaxDamage + 1));
        didDamage = component.TakeDamage(dmg, PlayerComponent.MeleeAttackCanPoison, PlayerComponent.MeleePoisonDamaged,
        PlayerComponent.MeleePoisonFrequency, PlayerComponent.MeleePoisonTick, PlayerComponent.MeleePoisonChance, PlayerComponent.MeleeAttackCanFire,
        PlayerComponent.MeleeFireDamaged, PlayerComponent.MeleeFireFrequency, PlayerComponent.MeleeFireTick, PlayerComponent.MeleeFireChance,
        PlayerComponent.MeleeAttackCanPush, PlayerComponent.MeleePushDistance, PlayerComponent.MeleeAttackCanFreez,
        PlayerComponent.MeleeFreezDuration, PlayerComponent.MeleeFreezChance);

        if (!damageShow)
        {
            Transform damagePopupTransform = Instantiate(PopUpDamage, transform.position, Quaternion.identity);
            DamagePopUp damagePopUp = damagePopupTransform.GetComponent<DamagePopUp>();
            damagePopUp.Setup(dmg, false);
            damageShow = true;
        }
    }

    private void CriticalDamage(Player PlayerComponent, Health component, bool didDamage)
    {
        int dmg = (Random.Range(PlayerComponent.MeleeAttackMinDamage, PlayerComponent.MeleeAttackMaxDamage + 1))
        * PlayerComponent.MeleeCriticalDamageMultiply;
        didDamage = component.TakeDamage(dmg, PlayerComponent.MeleeAttackCanPoison, PlayerComponent.MeleePoisonDamaged,
        PlayerComponent.MeleePoisonFrequency, PlayerComponent.MeleePoisonTick, PlayerComponent.MeleePoisonChance, PlayerComponent.MeleeAttackCanFire,
        PlayerComponent.MeleeFireDamaged, PlayerComponent.MeleeFireFrequency, PlayerComponent.MeleeFireTick, PlayerComponent.MeleeFireChance,
        PlayerComponent.MeleeAttackCanPush, PlayerComponent.MeleePushDistance, PlayerComponent.MeleeAttackCanFreez,
        PlayerComponent.MeleeFreezDuration, PlayerComponent.MeleeFreezChance);

        if (!damageShow)
        {
            Transform damagePopupTransform = Instantiate(PopUpDamage, transform.position, Quaternion.identity);
            DamagePopUp damagePopUp = damagePopupTransform.GetComponent<DamagePopUp>();
            damagePopUp.Setup(dmg, true);
            damageShow = true;
        }
    }

    private void PowerDamage(Player PlayerComponent, Health component, bool didDamage) // TODO Power ко всему доделать
    {
        int dmg = (Random.Range(PlayerComponent.MeleePowerAttackMinDamage, PlayerComponent.MeleePowerAttackMaxDamage + 1));
        didDamage = component.TakeDamage(dmg, PlayerComponent.MeleePowerAttackCanPoison, PlayerComponent.MeleePoisonDamaged,
        PlayerComponent.MeleePoisonFrequency, PlayerComponent.MeleePoisonTick, PlayerComponent.MeleePoisonChance,
        PlayerComponent.MeleePowerAttackCanFire, PlayerComponent.MeleeFireDamaged,
        PlayerComponent.MeleeFireFrequency, PlayerComponent.MeleeFireTick, PlayerComponent.MeleeFireChance,
        PlayerComponent.MeleePowerAttackCanPush, PlayerComponent.MeleePushDistance, PlayerComponent.MeleePowerAttackCanFreez,
        PlayerComponent.MeleeFreezDuration, PlayerComponent.MeleeFreezChance);

        if (!damageShow)
        {
            Transform damagePopupTransform = Instantiate(PopUpDamage, transform.position, Quaternion.identity);
            DamagePopUp damagePopUp = damagePopupTransform.GetComponent<DamagePopUp>();
            damagePopUp.Setup(dmg, false);
            damageShow = true;
        }
    }

    private void PowerCriticalDamage(Player PlayerComponent, Health component, bool didDamage) // TODO Power ко всему доделать
    {
        int dmg = (Random.Range(PlayerComponent.MeleePowerAttackMinDamage, PlayerComponent.MeleePowerAttackMaxDamage + 1))
        * PlayerComponent.MeleePowerCriticalDamageMultiply;
        didDamage = component.TakeDamage(dmg, PlayerComponent.MeleeAttackCanPoison, PlayerComponent.MeleePoisonDamaged,
        PlayerComponent.MeleePoisonFrequency, PlayerComponent.MeleePoisonTick, PlayerComponent.MeleePoisonChance, PlayerComponent.MeleeAttackCanFire,
        PlayerComponent.MeleeFireDamaged, PlayerComponent.MeleeFireFrequency, PlayerComponent.MeleeFireTick, PlayerComponent.MeleeFireChance,
        PlayerComponent.MeleePowerAttackCanPush, PlayerComponent.MeleePushDistance, PlayerComponent.MeleeAttackCanFreez,
        PlayerComponent.MeleeFreezDuration, PlayerComponent.MeleeFreezChance);

        if (!damageShow)
        {
            Transform damagePopupTransform = Instantiate(PopUpDamage, transform.position, Quaternion.identity);
            DamagePopUp damagePopUp = damagePopupTransform.GetComponent<DamagePopUp>();
            damagePopUp.Setup(dmg, false);
            damageShow = true;
        }
    }

    public void setHitBox(hitBoxes val)
    {
        healthsDamaged.Clear(); // Clear the list of damaged healths everytime we start a new attack

        // Set the polygon collider to be equal as the target one
        if (val != hitBoxes.clear)
        {
            localCollider.SetPath(0, colliders[(int)val].GetPath(0));
            return;
        }

        // If the value is Clear, set the pathcount of the polygoncollider2D to 0 (No Collisions)
        localCollider.pathCount = 0;
    }
}
