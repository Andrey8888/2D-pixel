using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : MonoBehaviour {
	
	private bool Pickable = true; // To avoid players

	[Header ("Amount")]
	public int Amount = 1;

	public Ammo AmmoType = Ammo.healAmount;

	void OnTriggerEnter2D (Collider2D other) {
		if (other.CompareTag ("Player") && Pickable) {
			var playercomponent = other.GetComponent<Player> ();
			if (playercomponent != null) {
				OnPlayerTrigger (playercomponent);
			}
		}
	}

	void OnTriggerStay2D (Collider2D other) {
		if (other.CompareTag ("Player") && Pickable) {
			var playercomponent = other.GetComponent<Player> ();
			if (playercomponent != null) {
				OnPlayerTrigger (playercomponent);
			}
		}
	}

	void OnPlayerTrigger (Player player) {
		var healthcomp = player.GetComponent<Health> ();
		if (AmmoType == Ammo.healAmount && healthcomp != null && healthcomp.TakeHeal(Amount)) {
			Pickable = false;
		}
		
		if (AmmoType == Ammo.rangedShell && player.Take_RangedShell(Amount)) {
			Pickable = false;
		}
		
		if (AmmoType == Ammo.rangedPowerShell && player.Take_RangedPowerShell(Amount)) {
			Pickable = false;
		}
		
		if (AmmoType == Ammo.meleeRune && player.Take_MeleeShell(Amount)) {
			Pickable = false;
		}
		
		//if (AmmoType == Ammo.money && player.Take_Money(Amount)) {
		//	Pickable = false;
		//}

		var manacomp = player.GetComponent<Mana> ();
		if (AmmoType == Ammo.mana && manacomp != null &&  manacomp.TakeRefresh(Amount)) {
			Pickable = false;
		}
	
	
			// Screenshake
		if (PixelCameraController.instance != null) {
			PixelCameraController.instance.Shake (0.1f);
		}
		// Destroy the potion
		Destroy(gameObject);
    }
}
