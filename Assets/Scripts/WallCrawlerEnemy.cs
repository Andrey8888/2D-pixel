using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public class WallCrawlerEnemy : Actor {

	[Header ("Damage To Player")]
	public int DamageOnTouch = 1; //

	// Run Speed & Acceleration
	public float MaxRun = 70f; // Maximun Horizontal Run Speed
	public float RunAccel = 1000f; // Horizontal Acceleration Speed

	// Helper private Variables
	private int moveX; // Variable to store the horizontal Input each frame
	private int moveY; // Variable to store the vertical Input each frame

	[Header ("Facing Direction")]
	public Facings Facing; 	// Facing Direction

	[Header ("Squash & Stretch")]
	public Transform SpriteHolder; // Reference to the transform of the child object which holds the sprite renderer of the player
	public Vector2 SpriteScale = Vector2.one; // The current X and Y scale of the sprite holder (used for Squash & Stretch)

	[Header ("Animator")]
	public Animator animator; // Reference to the animator

	// States for the state machine
	public enum States {
		Normal = 1,
		Up = 2,
		Down = 3,
		Right = 0,
		Left = -2,
		Death = 4
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

	// Update is called once per frame
	new void Update () {
		base.Update ();
		// Update the collision
		wasOnGround = onGround;
		//onGround = 
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
		while (moveX == 0) {
			moveX = Random.value < 0.5f ? -1 : 1;
		}

		if (CheckColAtPlace (Vector2.down, solid_layer)) {
			fsm.ChangeState (States.Up, StateTransition.Overwrite);
			return;
		} else if (CheckColAtPlace (Vector2.up, solid_layer)) {
			fsm.ChangeState (States.Down, StateTransition.Overwrite);
			return;
		} else if (CheckColAtPlace (Vector2.right, solid_layer)) {
			fsm.ChangeState (States.Left, StateTransition.Overwrite);
			return;
		} else if (CheckColAtPlace (Vector2.left, solid_layer)) {
			fsm.ChangeState (States.Right, StateTransition.Overwrite);
			return;
		}
	}

	void Up_Update () {

		/* 
		// This is for debugging purposses and makes the wall crawler start moving in the opposite direction when on a ledge
		var tileSize = GameManager.instance.TileSize; 
		var extraXToCheck = 0; // This depends on the size of your enemy sprite so it doesn't turn around when half or more of the sprite is beyond the platform

		if (moveX != 0 && !CollisionAtPlace(new Vector2(transform.position.x + ((tileSize.x/2 + extraXToCheck) * moveX), transform.position.y - (tileSize.y/2)), solid_layer)) {
			moveX *= -1;
		}
		*/

		// Wall crawling
		if (CheckColInDir(Vector2.down, solid_layer) && moveX != 0) {
			// Crawl DOWN a LEDGE
			if (!CheckColAtPlace (Vector2.right * moveX * 2, solid_layer)) {
				var myX = myCollider.bounds.center.x;
				var myExtentX = myCollider.bounds.extents.x;
				var extraDistanceX = -myExtentX + 2; // I set it to minus the X extent so it crawls the ledge when it's about halfway out of it, set it to a lower amount so it doesn't go so far out of it

				for (int i = 0; i <= myExtentX + extraDistanceX; i++) {
					if (CanCrawlVertical ((int)myX + i * moveX, moveX, -1)) {
						//Debug.Log (((int)myX + i * moveX).ToString());
						CrawlVertical ((int)myX + i * moveX, moveX, -1, true);
						return;
					}
				}
				// Crawl UP a WALL
			} else {
				var myX = myCollider.bounds.center.x;
				var myExtentX = myCollider.bounds.extents.x;
				var extraDistanceX = 1; // I set it to 1 so it crawls UP the WALL when it's touching the wall

				for (int i = 0; i <= myExtentX + extraDistanceX; i++) {
					if (CanCrawlVertical ((int)myX + i * moveX, -moveX, 1)) {
						//Debug.Log (((int)myX + i * moveX).ToString());
						CrawlVertical ((int)myX + i * moveX, -moveX, 1);
						return;
					}
				}
			}
		} 
			
		Speed.x = Calc.Approach (Speed.x, moveX * MaxRun, RunAccel * Time.deltaTime);
	}

	void Down_Update () {
		// Wall crawling
		if (CheckColInDir(Vector2.up, solid_layer) && moveX != 0) {
			// Crawl UP a LEDGE
			if (!CheckColAtPlace (Vector2.right * moveX * 2, solid_layer)) {
				var myX = myCollider.bounds.center.x;
				var myExtentX = myCollider.bounds.extents.x;
				var extraDistanceX = -myExtentX + 2; // I set it to minus the X extent so it crawls the ledge when it's about halfway out of it, set it to a lower amount so it doesn't go so far out of it

				for (int i = 0; i <= myExtentX + extraDistanceX; i++) {
					if (CanCrawlVertical ((int)myX + i * moveX, moveX, 1)) {
						//Debug.Log (((int)myX + i * moveX).ToString());
						CrawlVertical ((int)myX + i * moveX, moveX, 1, true);
						return;
					}
				}
				// Crawl DOWN a WALL
			} else {
				var myX = myCollider.bounds.center.x;
				var myExtentX = myCollider.bounds.extents.x;
				var extraDistanceX = 1; // I set it to 1 so it crawls DOWN the WALL when it's touching the wall

				for (int i = 0; i <= myExtentX + extraDistanceX; i++) {
					if (CanCrawlVertical ((int)myX + i * moveX, -moveX, -1)) {
						//Debug.Log (((int)myX + i * moveX).ToString());
						CrawlVertical ((int)myX + i * moveX, -moveX, -1);
						return;
					}
				}
			}
		} 


		Speed.x = Calc.Approach (Speed.x, moveX * MaxRun, RunAccel * Time.deltaTime);
	}

	void Left_Update () {
		// Crawl RIGHT a LEDGE
		if (CheckColInDir(Vector2.right, solid_layer) && moveY != 0) {
			if (!CheckColAtPlace (Vector2.up * moveY * 2, solid_layer)) {
				var myY = myCollider.bounds.center.y;
				var myExtentY = myCollider.bounds.extents.y;
				var extraDistanceY = -myExtentY + 2; // I set it to minus the Y extent so it crawls the ledge when it's about halfway out of it, set it to a lower amount so it doesn't go so far out of it

				for (int i = 0; i <= myExtentY + extraDistanceY; i++) {
					if (CanCrawlHorizontal ((int)myY + i * moveY, 1, moveY)) {
						//Debug.Log (((int)myY + i * moveY).ToString());
						CrawlHorizontal ((int)myY + i * moveY, 1, moveY, true);
						return;
					}
				}
				// Crawl LEFT a WALL
			} else {
				var myY = myCollider.bounds.center.y;
				var myExtentY = myCollider.bounds.extents.y;
				var extraDistanceY = 1; // I set it to 1 so it crawls the WALL when it's touching the wall

				for (int i = 0; i <= myExtentY + extraDistanceY; i++) {
					if (CanCrawlHorizontal ((int)myY + i * moveY, -1, -moveY)) {
						//Debug.Log (((int)myY + i * moveY).ToString());
						CrawlHorizontal ((int)myY + i * moveY, -1, -moveY);
						return;
					}
				}
			}
		}
			
		Speed.y = Calc.Approach (Speed.y, moveY * MaxRun, RunAccel * Time.deltaTime);
	}

	void Right_Update () {
		// Crawl LEFT a LEDGE
		if (CheckColInDir(Vector2.left, solid_layer) && moveY != 0) {
			if (!CheckColAtPlace(Vector2.up * moveY * 2, solid_layer)) {
				var myY = myCollider.bounds.center.y;
				var myExtentY = myCollider.bounds.extents.y;
				var extraDistanceY = -myExtentY + 2; // I set it to minus the Y extent so it crawls the ledge when it's about halfway out of it, set it to a lower amount so it doesn't go so far out of it

				for (int i = 0; i <= myExtentY + extraDistanceY; i++) {
					if (CanCrawlHorizontal((int)myY + i * moveY, -1, moveY)) {
						//Debug.Log (((int)myY + i * moveY).ToString());
						CrawlHorizontal((int)myY + i * moveY, -1, moveY, true);
						return;
					}
				}
				// Crawl RIGHT a WALL
			} else {
				var myY = myCollider.bounds.center.y;
				var myExtentY = myCollider.bounds.extents.y;
				var extraDistanceY = 1; // I set it to 1 so it crawls the WALL when it's touching the wall

				for (int i = 0; i <= myExtentY + extraDistanceY; i++) {
					if (CanCrawlHorizontal((int)myY + i * moveY, 1, -moveY)) {
						//Debug.Log (((int)myY + i * moveY).ToString());
						CrawlHorizontal((int)myY + i * moveY, 1, -moveY);
						return;
					}
				}
			}
		}

		Speed.y = Calc.Approach (Speed.y, moveY * MaxRun, RunAccel * Time.deltaTime);
	}

	// This function checks wether or not there is a ledge to grab in the target Y position and in the target horizontal direction (right or left)
	private bool CanCrawlVertical(int targetX, int hDirection, int vDirection)
	{		
		// This is just in case the game manager hasn't been assigned we use a default tilesize value of 16
		var tileSize = GameManager.instance != null ? GameManager.instance.TileSize : Vector2.one * 16; 

		return !CollisionAtPlace (new Vector2 (targetX + hDirection, transform.position.y + (vDirection * (tileSize.y / 2))), solid_layer) && 
			CollisionAtPlace (new Vector2 (targetX, transform.position.y + (vDirection * (tileSize.y / 2))), solid_layer);

	}

	// This function checks wether or not there is a ledge to grab in the target Y position and in the target horizontal direction (right or left)
	private bool CanCrawlHorizontal(int targetY, int hDirection, int vDirection)
	{		
		// This is just in case the game manager hasn't been assigned we use a default tilesize value of 16
		var tileSize = GameManager.instance != null ? GameManager.instance.TileSize : Vector2.one * 16; 

		return !CollisionAtPlace (new Vector2 (transform.position.x + (hDirection * (tileSize.x/2)), targetY + vDirection), solid_layer) &&
			CollisionAtPlace (new Vector2 (transform.position.x + (hDirection * (tileSize.x/2)), targetY), solid_layer);

	}

	private void CrawlVertical(int targetX, int hDirection, int vDirection, bool ledge = false)
	{

		// The collider's extents on the Y axis (half of height) plus one
		var extentsY = myCollider.bounds.extents.y + 1;
		var extentsX = ledge == true ? myCollider.bounds.extents.x : 1;
		// Set the Y position to the desired one according to our collider's size and the targetY
		transform.position = new Vector2(targetX + extentsY * hDirection, transform.position.y + (extentsX) * vDirection);
		// Set the speed to zero
		Speed = Vector2.zero;
		// Reset the move X
		moveX = 0;
		// Set the moveY To the desired one
		moveY = vDirection;
	
		// Set the rotation
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90 * -hDirection));

		// We check for collisions with the ledge on the set direction
		while (!CheckColAtPlace(Vector2.right * -hDirection, solid_layer))
		{
			transform.position = new Vector2 (transform.position.x + -hDirection, transform.position.y);
		}

		// Change the sprite scale to add extra feel
		SpriteScale = new Vector2(1.2f, 0.8f);
		// Change the state
		var targetState = hDirection == -1 ? States.Left : States.Right;
		fsm.ChangeState (targetState, StateTransition.Overwrite);
	}

	private void CrawlHorizontal(int targetY, int hDirection, int vDirection, bool ledge = false)
	{

		// The collider's extents on the Y axis (half of height) plus one
		var extentsY = myCollider.bounds.extents.y + 1;
		var extentsX = ledge == true ? myCollider.bounds.extents.x : 1;
		// Set the Y position to the desired one according to our collider's size and the targetY
		transform.position = new Vector2(transform.position.x + (extentsX) * hDirection, targetY + extentsY * vDirection);
		// Set the speed to zero
		Speed = Vector2.zero;
		// Reset the move X
		moveY = 0;
		// Set the moveY To the desired one
		moveX = hDirection;

		// Set the rotation
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, vDirection == 1 ? 0 : 180));

		// We check for collisions with the ledge on the set direction
		while (!CheckColAtPlace(Vector2.up * -vDirection, solid_layer))
		{
			transform.position = new Vector2 (transform.position.x, transform.position.y + -vDirection);
		}

		// Change the sprite scale to add extra feel
		SpriteScale = new Vector2(1.2f, 0.8f);
		// Change the state
		var targetState = vDirection == -1 ? States.Down : States.Up;
		fsm.ChangeState (targetState, StateTransition.Overwrite);
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

		// If on the death state
		if (fsm.State == States.Death) {
			// Play the death animation
			if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Death")) {
				animator.Play ("Death");
			}
			// If not death
		} else {
			// Play the normal animation
			if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Normal")) {
				animator.Play ("Normal");
			}
		}
	}

	public void Die () {
		fsm.ChangeState (States.Death, StateTransition.Overwrite);
		myCollider.enabled = false;
		moveX = 0;
		moveY = 0;
		Speed = Vector2.zero;
	}

	// Checks wether the actor is grounded or not 
	public override bool OnGround () {
		// checking if there is a collision on the bottom with the solid layer or if there is a collision with the oneway layer and you are not already collisioning with it
		return CheckColAtPlace (Vector2.down, solid_layer);
	}
}