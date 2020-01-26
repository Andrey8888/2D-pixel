#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public class WormEnemy : Actor {

	[Header ("Damage To Player")]
	public int DamageOnTouch = 1;

	[Header ("Movement Variables")]
	// Gravity, Maximun fall speed & fastfall Speed
	public float Gravity = 900f; // Speed at which you are pushed down when you are on the air
	public float MaxFall = -160f; // Maximun common speed at which you can fall
	public float RunReduce = 400f; // Horizontal Acceleration when you're already when your horizontal speed is higher or equal to the maximun
	// Run Speed & Acceleration
	public float MaxRun = 90f; // Maximun Horizontal Run Speed
	public float RunAccel = 1000f; // Horizontal Acceleration Speed
	// Air value multiplier
	public float AirMult = 0.65f; // Multiplier for the air horizontal movement (friction) the higher the more air control you'll have

	// Helper private Variables
	private int moveX; // Variable to store the horizontal Input each frame

	[Header ("Facing Direction")]
	public Facings Facing; 	// Facing Direction

	[Header ("Squash & Stretch")]
	public Transform SpriteHolder; // Reference to the transform of the child object which holds the sprite renderer of the player
	public Vector2 SpriteScale = Vector2.one; // The current X and Y scale of the sprite holder (used for Squash & Stretch)

	[Header ("Animator")]
	public Animator animator; // Reference to the animator

	// States for the state machine
	public enum States {
		Normal, 
		Death
	}

	// State Machine
	public StateMachine<States> fsm;

	new void Awake () {
		base.Awake ();
		fsm = StateMachine<States>.Initialize(this);
	}

	// Use this for initialization
	void Start () {
		fsm.ChangeState(States.Normal);
	}

	new void Update () {
		base.Update ();
		// Update all collisions here
		wasOnGround = onGround;
		onGround = OnGround();
	}

	void LateUpdate () {
		// Do all the movement on the actor (base)
		// Horizontal
		var moveh = base.MoveH (Speed.x * Time.deltaTime);
		if (moveh) {
			Speed.x = 0;
		}

		// Vertical
		var movev = base.MoveV (Speed.y * Time.deltaTime);
		if (movev) {
			Speed.y = 0;
		}

		// Update the sprite
		UpdateSprite ();

		// Get Crushed by block if we are collisioning with the solid layer
		if (CollisionSelf(solid_layer)) {
			var health = GetComponent<Health>();
			if (health != null) {
				health.TakeDamage (9999);
			}
		}
	}

	void Normal_Update () {
		// This is just in case the game manager hasn't been assigned we use a default tilesize value of 16
		var tileSize = GameManager.instance != null ? GameManager.instance.TileSize : Vector2.one * 16; 
		var extraXToCheck = 2; // This depends on the size of your enemy sprite so it doesn't turn around when half or more of the sprite is beyond the platform

		if (moveX == 0) {
			moveX = Random.Range (-1, 2);
		}

		if (moveX != 0 && CheckColInDir (new Vector2(moveX, 0), solid_layer)) {
			moveX *= -1;
		} else if (moveX != 0 && (!CollisionAtPlace(new Vector2(transform.position.x + ((tileSize.x/2 + extraXToCheck) * moveX), transform.position.y - tileSize.y), solid_layer) &&
			!CollisionAtPlace(new Vector2(transform.position.x + ((tileSize.x/2 + extraXToCheck) * moveX), transform.position.y - tileSize.y), oneway_layer))) {
			moveX *= -1;
		}

		// Horizontal Speed Update Section
		float num = onGround ? 1f : 0.65f;

		Speed.x = Calc.Approach (Speed.x, moveX * MaxRun, RunReduce * num * Time.deltaTime);

		if (!onGround) {
			float target = MaxFall;
			Speed.y = Calc.Approach (Speed.y, target, Gravity * Time.deltaTime);
		}
	}

	void Death_Update () {
		// Horizontal Speed Update Section
		float num = onGround ? 1f : 0.65f;

		Speed.x = Calc.Approach (Speed.x, 0f, RunReduce * num * Time.deltaTime);

		if (!onGround) {
			float target = MaxFall;
			Speed.y = Calc.Approach (Speed.y, target, Gravity * Time.deltaTime);
		}
	}

	// Function to detect collision with the player
	void OnTriggerEnter2D (Collider2D other) {
		if (other.CompareTag ("Player") && !GetComponent<Health>().dead) {
			var playercomponent = other.GetComponent<Player> ();
			if (playercomponent != null) {
				OnPlayerTrigger (playercomponent);
			}
		}
	}

	// Function to detect collision with the player
	void OnTriggerStay2D (Collider2D other) {
		if (other.CompareTag ("Player") && !GetComponent<Health>().dead) {
			var playercomponent = other.GetComponent<Player> ();
			if (playercomponent != null) {
				OnPlayerTrigger (playercomponent);
			}
		}
	}

	// Function to deal damage to the player
	void OnPlayerTrigger (Player player) {
		player.GetComponent<Health> ().TakeDamage (DamageOnTouch);
	}

	// Function to update the sprite scale, facing direction and animations 
	void UpdateSprite () {
		// Approch the normal sprite scale at a set rate
		SpriteScale.x = Calc.Approach (SpriteScale.x, 1f, 0.04f);
		SpriteScale.y = Calc.Approach (SpriteScale.y, 1f, 0.04f);

		// Set the SpriteHolder scale to the target scale
		var targetSpriteHolderScale = new Vector3 (SpriteScale.x, SpriteScale.y, 1f);
		if (SpriteHolder.localScale != targetSpriteHolderScale) {
			SpriteHolder.localScale = targetSpriteHolderScale;
		}

		// Set the x scale to the current facing direction
		var targetLocalScale = new Vector3 (((int)Facing) * -1f, transform.localScale.y, transform.localScale.z);
		if (transform.localScale != targetLocalScale) {
			transform.localScale = targetLocalScale;
		}


		if (fsm.State == States.Death) {
			if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Death")) {
				animator.Play ("Death");
			}
			// If on the ground
		} else if (onGround) {
			// If the is nohorizontal movement input
			if (moveX == 0) {
				// Idle Animation
				if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Idle")) {
					animator.Play ("Idle");
				}
				// If there is horizontal movement input
			} else {
				// Run Animation
				if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Run")) {
					animator.Play ("Run");
				}
			}
		}
	}

	public void Die () {
		fsm.ChangeState (States.Death, StateTransition.Overwrite);
	}
}
