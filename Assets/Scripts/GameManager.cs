using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {

	[Header ("Configuration")]
	public Vector2 TileSize; // The size of the tiles you're using (in unity Units) our tile's size is 16 pixels and PPU is set to 1 which means size is 16, we always recommend setting PPU to 1 for pixel art games.

	[Header ("Level Spawn Point")]
	public Transform LevelSpawnPoint; // Currently active Spawn Point

	[Header ("Events")]
	// Called from the player Die function
	public UnityEvent OnPlayerDeadEvents;

	// Called from the player Script when exiting the Respawn State
	public UnityEvent OnPlayerRespawnEvents;

	// Called from the Door/Exit Script
	public UnityEvent NextLevelEvents;

	public static GameManager instance = null;

	void Awake () {
		// Singleton-ish
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		// Set the maximun FPS to 60
		Application.targetFrameRate = 60;
	}

	// Update is called once per frame
	void Update () {
		// This is used for debugging purposes, reloads the first scene in Build Settings > Scenes In Build
		//if (Input.GetKeyDown (KeyCode.R)) {
		//	SceneManager.LoadScene (0);
		//}
	}
		
	public void PlayerDead() {
		Debug.Log ("Player Dead");
		if (OnPlayerDeadEvents != null) {
			OnPlayerDeadEvents.Invoke ();
		}
	}

	public void PlayerRespawn() {
		Debug.Log ("Player Respawn");
		if (OnPlayerRespawnEvents != null) {
			OnPlayerRespawnEvents.Invoke ();
		}
	}

	public void NextLevel() {
		Debug.Log ("Next Level");
		if (NextLevelEvents != null) {
			NextLevelEvents.Invoke ();
		}
	}

	public void Emit(GameObject particlePrefab, int amount, Vector2 position, Vector2 positionRange) {
		for (int i = 0; i < amount; i++) {
			this.Emit(particlePrefab, new Vector2(Random.Range(position.x - positionRange.x, position.x + positionRange.x), 
				Random.Range(position.y - positionRange.y, position.y + positionRange.y)));
		}
	}

	public void Emit(GameObject particlePrefab, Vector2 position) {
		Instantiate (particlePrefab, position, Quaternion.identity);
	}

	// Script to reset all the falling platforms back to the base state
	public void ResetFallingPlatforms () {
		var platforms = GameObject.FindObjectsOfType<FallingPlatform> ();

		foreach (FallingPlatform s in platforms) {
			if (s.enabled) {
				s.ResetPlatform ();
			}
		}
	}
}
