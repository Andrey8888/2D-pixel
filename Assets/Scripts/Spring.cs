using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour {

	[SerializeField]
	private bool playerCanUse = true; // Reference to wether the player can use the spring or not (migh be usefull for other functionallity)

	[Header ("Animator")]
	public Animator animator;

	void OnTriggerEnter2D (Collider2D other) {
		// Check if the tag of the collider is "Player"
		if (other.CompareTag ("Player")) {
			// Try to get the player component
			var playercomponent = other.GetComponent<Player> ();
			// If there is such component call the OnPlayerTrigger Function
			if (playercomponent != null) {
				OnPlayerTrigger (playercomponent);
			}
		}
	}

	void OnPlayerTrigger (Player player) {
		// If cannot be used by the player return
		if (!playerCanUse)
			return;

		// If the vertical speed of the player is downwards
		if (player.Speed.y < 0f)
		{
			// Call the bounce function on the player
			player.SpringBounce ();
			animator.Play ("Spring-Bounce");
			return;
		}
	}
}