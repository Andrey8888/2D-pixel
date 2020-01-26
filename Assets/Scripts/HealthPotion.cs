using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour {
	
	private bool Pickable = true; // To avoid players

	[Header ("Heal Amount")]
	public int healAmount = 1;


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
		if (healthcomp != null && healthcomp.TakeHeal(healAmount)) {
			Pickable = false;

			// Screenshake
			if (PixelCameraController.instance != null) {
				PixelCameraController.instance.Shake (0.1f);
			}

			// Destroy the potion
			Destroy(gameObject);
		}
	}
}
