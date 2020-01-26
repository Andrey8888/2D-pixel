using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

	public Transform SpawnPoint;

	private bool Pickable = true;
	public Animator myAnimator;

	void Awake () {
		if (SpawnPoint == null) {
			SpawnPoint = transform;
			Debug.Log ("This Check point has no spawnpoint transform attached to it and it will be set to it's own transform");
		}

		if (myAnimator == null) {
			myAnimator = GetComponentInChildren<Animator> ();
			if (myAnimator == null) {
				Debug.Log ("There is no animator assigned to this checkpoint");
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
		if (GameManager.instance != null) {
			Pickable = false;

			SetAsActive ();

		} else {
			Debug.Log ("There is no instance of the GameManager so the checkpoint won't work");
		}
	}

	public void SetActivableCheckpoint () {
		if (!myAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Disabled")) {
			myAnimator.Play ("Disabled");
		}
	}

	public void SetAsActive () {
		if (!myAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Enabled")) {
			myAnimator.Play ("Enabled");
		}

		// Set the gamemanager's spawn point to this checkpoint's spawn point
		GameManager.instance.LevelSpawnPoint = SpawnPoint;

		// Screenshake
		if (PixelCameraController.instance != null) {
			PixelCameraController.instance.DirectionalShake(Vector2.right, 0.1f);
		}

		// Enable all the other checkpoints, make them pickable again. (Only use this part if you want previously used checkpoints to be activable again)
		var checkpoints = GameObject.FindObjectsOfType<Checkpoint>();
		foreach (Checkpoint s in checkpoints) {
			if (s != null && s != this) {
				s.SetActivableCheckpoint ();
			}
		}
	}

}
