using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour {

	// Different directions/Orientations a spike can have
	public enum Directions
	{
		Up,
		Down,
		Left,
		Right
	}

	[SerializeField]
	private Directions direction; // The direction this spike is/should be facing
	//public bool checkForSurroundingFloors; // Should check for surrounding floors or not
	private bool isSetup = false; // Wether or not the spike has been setup
	public SpriteRenderer Sprite; // The sprite renderer of this spike
	//private Vector2 position; // Helper variable to be used later to auto-check for surrounding solid tiles

	void Awake () {
		// If a sprite renderer has not been assigned try to find it on the base
		if (Sprite == null) {
			Sprite = GetComponent<SpriteRenderer> ();
			if (Sprite == null) {
				Debug.Log ("There is no sprite renderer attached/Assigned to this spike");
			}
		}
	}

	// Use this for initialization
	void Start () {
		// Round the tranform's position to the closest integer (just in case it's been placed on a floating/decimal value)
		transform.position = new Vector3 (Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 1f);
		// Assign the position value 
		//position = new Vector2 (transform.position.x, transform.position.y);

		// Invoke the delayed start function after 100 milliseconds
		Invoke ("DelayedStart", 0.1f);
	}

	// This function is used to check for floors surrounding this spike and set a direction automatically
	public void DelayedStart () {
		if (!isSetup) {
			// Else just set the spike up with the set direction on the inspector
			SetUp (direction);
		}
	}

	// Function to setup the spike on the set direction (sets the correct rotation
	public void SetUp (Directions dir) {
		switch (dir)
		{
		case Directions.Up:
			direction = Directions.Up;
			isSetup = true;
			break;
		case Directions.Down:
			direction = Directions.Down;
			Sprite.flipY = true;
			isSetup = true;
			break;
		case Directions.Left:
			direction = Directions.Left;
			transform.localRotation = Quaternion.Euler (new Vector3 (0f, 0f, 90f));
			isSetup = true;
			break;
		case Directions.Right:
			direction = Directions.Right;
			transform.localRotation = Quaternion.Euler (new Vector3 (0f, 0f, 270f));
			isSetup = true;
			break;
		default:
			return;
		}
	}
		
	void OnTriggerEnter2D (Collider2D other) {
		// Check if the tag of the object is Player
		if (other.CompareTag ("Player")) {
			// try to find the Player component
			var playercomponent = other.GetComponent<Player> ();
			if (playercomponent != null) {
				// Trigget the OnPlayerTrigger Function
				OnPlayerTrigger (playercomponent);
			}
		}
	}

	// Function to check the direction of the spike and the speed of the player and kill the player if the requirements are met
	void OnPlayerTrigger (Player player) {
		switch (direction)
		{
		case Directions.Up:
			// If the spike is facing Up and the player is falling
			if (player.Speed.y < 0f)
			{
				player.Die();
				return;
			}
			break;
		case Directions.Down:
			// If the spike is facing Down and the player is going up
			if (player.Speed.y > 0f)
			{
				player.Die();
				return;
			}
			break;
		case Directions.Left:
			// If the spike is facing Left and the player is going right
			if (player.Speed.x > 0f)
			{
				player.Die();
				return;
			}
			break;
		case Directions.Right:
			// If the spike is facing Right and the player is going left
			if (player.Speed.x < 0f)
			{
				player.Die();
			}
			break;
		default:
			return;
		}
	}
}