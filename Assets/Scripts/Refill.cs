using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refill : MonoBehaviour {
	private bool Pickable = true;
	private float RespawnTimer = 0f;
	[SerializeField]
	private SpriteRenderer Sprite;

	void Awake () {
		if (Sprite == null) {
			Sprite = GetComponent<SpriteRenderer> ();

			if (Sprite == null) {
				Debug.Log ("This refill has no spriterenderer attached to it");
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (this.RespawnTimer > 0f)
		{
			this.RespawnTimer -= Time.deltaTime;
			if (this.RespawnTimer <= 0f)
			{
				this.Respawn();
			}
		}
	}

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
		if (player.UseRefillDash ()) {
			Pickable = false;
			// Disable
			Sprite.enabled = false;
			this.RespawnTimer = 2.5f;

			// Screenshake
			if (PixelCameraController.instance != null) {
				PixelCameraController.instance.Shake (0.1f);
			}
		}
	}

	private void Respawn()
	{
		if (!Pickable) {
			Pickable = true;
			Sprite.enabled = true;
		}
	}
}
