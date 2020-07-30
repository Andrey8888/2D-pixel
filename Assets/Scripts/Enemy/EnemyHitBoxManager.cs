using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBoxManager : MonoBehaviour {

	// Set these in the editor
	public PolygonCollider2D attack1; // hands

    // Used for organization
    public PolygonCollider2D[] colliders;

	// Collider on this game object
	private PolygonCollider2D localCollider;

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
		if (owner == null) {
			owner = GetComponentInParent<Health> ();
			if (owner == null) {
				Debug.Log ("There is no owner health component asigned to this HitBoxManager");
			}
		}
		// Set up an array so our script can more easily set up the hit boxes
		colliders = new PolygonCollider2D[]{attack1};

		// Create a polygon collider
		localCollider = gameObject.AddComponent<PolygonCollider2D>();
		localCollider.isTrigger = true; // Set as a trigger so it doesn't collide with our environment
		localCollider.pathCount = 0; // Clear auto-generated polygons
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        var component = col.GetComponent<Health>();
        // If the target the hitbox collided with has a health component and it is not our owner and it is not on the already on the list of healths damaged by the current hitbox
        if (component != null && component != owner && !healthsDamaged.Contains(component))
        {
            // Try to Apply the damage
            var EnemyComponent = GetComponentInParent<Enemy>();
            var didDamage = false;
            if (Random.Range(0, 100) < EnemyComponent.MeleeCriticalDamageChance)
            {
                CriticalDamage(EnemyComponent, component, didDamage);
            }
            else
            {
                Damage(EnemyComponent, component, didDamage);
            }

            if (didDamage)
            {
                // Add the health component to the list of damaged healths
                healthsDamaged.Add(component);
            }
        }
    }

    private void Damage(Enemy EnemyComponent, Health component, bool didDamage)
    {
        int dmg = (Random.Range(EnemyComponent.MeleeAttackMinDamage, EnemyComponent.MeleeAttackMaxDamage + 1));
        didDamage = component.TakeDamage(dmg, EnemyComponent.MeleeAttackCanPoison, EnemyComponent.MeleePoisonDamaged,
        EnemyComponent.MeleePoisonFrequency, EnemyComponent.MeleePoisonTick, EnemyComponent.MeleePoisonChance, EnemyComponent.MeleeAttackCanFire,
        EnemyComponent.MeleeFireDamaged, EnemyComponent.MeleeFireFrequency, EnemyComponent.MeleeFireTick, EnemyComponent.MeleeFireChance,
        EnemyComponent.MeleeAttackCanPush, EnemyComponent.MeleePushDistance, EnemyComponent.MeleeAttackCanFreez,
        EnemyComponent.MeleeFreezDuration, EnemyComponent.MeleeFreezChance);

        //if (!damageShow)
        //{
        //    Transform damagePopupTransform = Instantiate(PopUpDamage, transform.position, Quaternion.identity);
        //    DamagePopUp damagePopUp = damagePopupTransform.GetComponent<DamagePopUp>();
        //    damagePopUp.Setup(dmg, false);
        //    damageShow = true;
        //}
    }

    private void CriticalDamage(Enemy EnemyComponent, Health component, bool didDamage)
    {
        int dmg = (Random.Range(EnemyComponent.MeleeAttackMinDamage, EnemyComponent.MeleeAttackMaxDamage + 1))
        * EnemyComponent.MeleeCriticalDamageMultiply;
        didDamage = component.TakeDamage(dmg, EnemyComponent.MeleeAttackCanPoison, EnemyComponent.MeleePoisonDamaged,
        EnemyComponent.MeleePoisonFrequency, EnemyComponent.MeleePoisonTick, EnemyComponent.MeleePoisonChance, EnemyComponent.MeleeAttackCanFire,
        EnemyComponent.MeleeFireDamaged, EnemyComponent.MeleeFireFrequency, EnemyComponent.MeleeFireTick, EnemyComponent.MeleeFireChance,
        EnemyComponent.MeleeAttackCanPush, EnemyComponent.MeleePushDistance, EnemyComponent.MeleeAttackCanFreez,
        EnemyComponent.MeleeFreezDuration, EnemyComponent.MeleeFreezChance);

        //if (!damageShow)
        //{
        //    Transform damagePopupTransform = Instantiate(PopUpDamage, transform.position, Quaternion.identity);
        //    DamagePopUp damagePopUp = damagePopupTransform.GetComponent<DamagePopUp>();
        //    damagePopUp.Setup(dmg, true);
        //    damageShow = true;
        //}
    }

    public void setHitBox(hitBoxes val)
	{
		healthsDamaged.Clear (); // Clear the list of damaged healths everytime we start a new attack

		// Set the polygon collider to be equal as the target one
		if(val != hitBoxes.clear)
		{
			localCollider.SetPath(0, colliders[(int)val].GetPath(0));
			return;
		}

		// If the value is Clear, set the pathcount of the polygoncollider2D to 0 (No Collisions)
		localCollider.pathCount = 0;
	}
}
