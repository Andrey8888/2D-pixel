#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public class Player : Actor {

	[Header ("Movement Variables")]
	// Gravity, Maximun fall speed & fastfall Speed
	public float Gravity = 900f; // Speed at which you are pushed down when you are on the air
	public float MaxFall = -160f; // Maximun common speed at which you can fall
	public float FastFall = -240f; // Maximun fall speed when you're fast falling
	// Run Speed & Acceleration
	public float MaxRun = 90f; // Maximun Horizontal Run Speed
	public float RunAccel = 1000f; // Horizontal Acceleration Speed
	public float RunReduce = 400f; // Horizontal Acceleration when you're already when your horizontal speed is higher or equal to the maximun
	// Air value multiplier
	public float AirMult = 0.65f; // Multiplier for the air horizontal movement (friction) the higher the more air control you'll have
	// Jump Variables
	public float JumpSpeed = 135f; // Vertical Jump Speed/Force
	public float JumpHBoost = 40f; // Extra Horizontal Speed Boost when jumping
	public float VariableJumpTime = 0.2f; // Time after performing a jump on which you can hold the jump key to jump higher
	public float SpringJumpSpeed = 275f; // Vertical Jump Speed/Force for spring jumps
	public float SpringJumpVariableTime = 0.05f; // ime after performing a spring jump (jumping off a spring) on which you can hold the jump key to jump higher
	// Wall Jump variables
	public float WallJumpForceTime = 0.16f; // Time on which the horizontal movement is restrcited/forced after a wall jump (if too low the player migh be able to climb up a wall)
	public float WallJumpHSpeed = 130f; // Horizontal Speed Boost when performing a wall jump
	public int WallJumpCheckDist = 2; // Distance at which we check for walls before performing a wall jump (2-4 is recommended)
	// Wall Slide Variables
	public float WallSlideStartMax = -20f; // Starting vertical speed when you wall slide
	public float WallSlideTime = 1.2f; // Maximun time you can wall slide before gaining the full fall speed again
	// Dash Variables
	public float DashSpeed = 240f; // Speed/Force which you dash at
	public float EndDashSpeed = 160f; // Extra Speed Boost when the dash ends (2/3 of the dash speed recommended)
	public float EndDashUpMult = 0.75f; // Multiplier applied to the Speed after a dash ends if the direction you dashed at was up
	public float DashTime = 0.15f; // The total time which a dash lasts for
	public float DashCooldown = 0.4f; // The Cooldown time of a dash
	// Other variables used for responsive movement
	public float clingTime   = 0.125f; // Wall Cling/Stick time after touching a wall where you can't leave the wall (to avoid unintentionally leaving the wall when trying to perform a wall jump)
	public float JumpGraceTime = 0.1f; // Jump Grace Time after leaving the ground non-jump on which you can still make a jump
	public float JumpBufferTime = 0.1f; // If the player hits the ground within this time after pressing the the jump button, the jump will be executed as soon as they touch the ground
	// Ladder Variables 
	public float LadderClimbSpeed = 60f;



	[Header ("Attacks")]
	public int MeleeAttackDamage = 1; // The damage done by the sword attack
	public float MeleeAttackCooldownTime = 1f;
	public float BowAttackCooldownTime = 1f;

	[Header ("Facing Direction")]
	public Facings Facing; 	// Facing Direction
	[Header ("Wall Slide Direction")]
	public int wallSlideDir; 	// Wall Slide Direction
	[Header ("Ladder")]
	public bool isInLadder = false;

	[Header ("Respawn")]
	public float RespawnTime; // Total Time it takes the player to respawn
	private float respawnTimer = 0f; // Variable to store the timer of the respawn
	private Vector2 respawnPosition; // Variable to store the position you respawn at

	[Header ("Ducking & Colliders")]
	public BoxCollider2D myNormalCollider; // Collider used while you're on any state except for the ducking state
	public BoxCollider2D myDuckingCollider; // Collider used while in the ducking state
	public float DuckingFrictionMultiplier = 0.5f; // The friction multiplier for when you're ducking 0.4-0.6 recommended (0 is no friction, 1 is the normal friction)

	// Helper private Variables
	private int moveX; // Variable to store the horizontal Input each frame
	private int moveY; // Variable to store the vectical Input each frame
	private int oldMoveY; // Variable to store the he vertical Input for the last frame
	private float varJumpSpeed; // Vertical Speed to apply on each frame of the variable jump
	private float varJumpTimer = 0f; // Variable to store the time left on the variable jump
	private int forceMoveX; // Used to store the forced horizontal movement input
	private float forceMoveXTimer = 0f; // Used to store the time left on the force horizontal movement
	private float maxFall; // Variable to store the current maximun fall speed
	private float wallSlideTimer = 0f; // Used to store the time left on the wallslide
	private Vector2 DashDir; // Here we store the direction in which we are dashing
	private float dashCooldownTimer = 0f; // Timer to store how much cooldown has the dash
	private bool canStick = false; // Helper variable for the wall sticking functionality
	private bool sticking = false; // Variable to store if the player is currently sticking to a wall
	private float stickTimer = 0f; // Timer to store the time left sticking to a wall 
	private float jumpGraceTimer = 0f; // Timer to store the time left to perform a jump after leaving a platform/solid
	private float jumpBufferTimer = 0f; // Timer to store the time left in the JumpBuffer timer
	private bool jumpIsInsideBuffer = false;
	private float meleeAttackCooldownTimer = 0f; // Timer to store the cooldown left to use the melee attack
	private float bowAttackCooldownTimer = 0f; // Timer to store cooldown left on the bow attak
	private float moveToRespawnPositionAfter = .5f; // Time to wait before moving to the respawn position
	private float moveToRespawnPosTimer = 0f; // Timer to store how much time is left before moving to the respawn position
	private float ledgeClimbTime = 1f; // Total time it takes to climb a wall
	private float ledgeClimbTimer = 0f; // Timer to store the current time passed in the ledgeClimb state
	private Vector2 extraPosOnClimb = new Vector2(10, 16); // Extra position to add to the current position to the end position of the climb animation matches the start position in idle state
    private ResetState ResetObj;

	// Check if we should duck (on the ground and moveY is pointing down and moveX is 0)
	public bool CanDuck
	{
		get
		{
			return onGround && moveX == 0 && moveY < 0 && !jumpIsInsideBuffer;
		}
	}

	// Check if we should/can dash (the dash button has been pressed & If the cooldown has been completed)
	public bool CanDash
	{
		get
		{
			return Input.GetButtonDown("Dash") && dashCooldownTimer <= 0f;
		}
	}

	public bool CanAttack
	{
		get
		{
			return Input.GetButtonDown("Attack") && meleeAttackCooldownTimer <= 0f;
		}
	}

	public bool CanShoot
	{
		get
		{
			return Input.GetButtonDown("Shoot") && bowAttackCooldownTimer <= 0f;
		}
	}

	[Header ("Squash & Stretch")]
	public Transform SpriteHolder; // Reference to the transform of the child object which holds the sprite renderer of the player
	public Vector2 SpriteScale = Vector2.one; // The current X and Y scale of the sprite holder (used for Squash & Stretch)

	[Header ("Animator")]
	public Animator animator; // Reference to the animator

	[Header ("Particles")]
	public GameObject DustParticle;
	public GameObject DashDustParticle;

	// States for the state machine
	public enum States {
		Normal,
		Ducking,
		LadderClimb,
		Dash,
		Respawn,
		LedgeGrab,
		LedgeClimb,
		Attack,
		BowAttack
	}

	// State Machine
	public StateMachine<States> fsm;
    private bool onDucking = false;
    private bool OnСeiling = false;

    new void Awake () {
		base.Awake ();
		fsm = StateMachine<States>.Initialize(this);
        ResetObj = FindObjectOfType<ResetState>();
        // This code piece is only neccesary for the ducking functionality
        //Ducking & Normal Colliders Assignment
        if (myNormalCollider == null && myDuckingCollider != null) {
			Debug.Log ("The player has no Collider attached to it for the normal state");
		} else if (myDuckingCollider != null && myNormalCollider != null) {
			// Only assign the collider if the ducking collider has been assigned hence only do it if you're planning to use the ducking functionality
			myCollider = myNormalCollider;
		}

	}

	// Use this for initialization
	void Start () {
		fsm.ChangeState(States.LadderClimb);
	}
	
	// Update is called once per frame
	new void Update () {
		base.Update ();

		if (fsm.State == States.Respawn)
			return;

		// Update all collisions here
		wasOnGround = onGround;
		onGround = OnGround();

		// Handle Variable Jump Timer
		if (varJumpTimer > 0f) {
			varJumpTimer -= Time.deltaTime;
		}

		// Handle Wall Slide Timer
		if (wallSlideDir != 0) {
			wallSlideTimer = Mathf.Max(this.wallSlideTimer - Time.deltaTime, 0f);
			wallSlideDir = 0;
		}

		// Reduce the cooldown of the dash
		if (dashCooldownTimer > 0f) {
			dashCooldownTimer -= Time.deltaTime;
		}

		// If on the ground at the start of this frame
		if (onGround)
		{
			wallSlideTimer = WallSlideTime; // Reset the wall slide timer when on the ground
			jumpGraceTimer = JumpGraceTime;
		} else if (jumpGraceTimer > 0f) // if not on the ground reduce the jumpgracetimer
		{
			jumpGraceTimer -= Time.deltaTime;
		}

		// Reset the wall cling
		if (onGround || (!CheckColInDir (Vector2.right * 1f, solid_layer) && !CheckColInDir (Vector2.right * -1f, solid_layer))) {
			sticking = false;
			canStick = true;
		}

		// Set sticking to false when the timer has expired
		if (stickTimer > 0f && sticking) {
			stickTimer -= Time.deltaTime;

			if (stickTimer <= 0f) {
				sticking = false;
			}
		}

		// Jump buffer Timer handling
		if (jumpIsInsideBuffer) {
			jumpBufferTimer -= Time.deltaTime;
		}

		//Jump Input Buffering
		if (Input.GetButtonDown("Jump")) {
			jumpBufferTimer = JumpBufferTime;
		}

		// Check if the jump buffer timer has ran off if so set the jump to false
		if (jumpBufferTimer > 0) {
			jumpIsInsideBuffer = true;
		} else {
			jumpIsInsideBuffer = false;
		}

		// just in case the jumpbuffertime has been set to 0 or less just, just use the old jump input
		if (JumpBufferTime <= 0) {
			jumpIsInsideBuffer = Input.GetButtonDown ("Jump");
		}

		// Update the moveX Variable depending wether the movement is force or not
		if (forceMoveXTimer > 0f) {
			forceMoveXTimer -= Time.deltaTime;
			moveX = forceMoveX;
		}else {
			moveX = (int)Input.GetAxisRaw("Horizontal");
		}
			
		// Update the moveY Variable and assign the current vertical input for this frame 
		oldMoveY = moveY;
		moveY = (int)Input.GetAxisRaw("Vertical");

		//Melee Attack timer
		if (meleeAttackCooldownTimer > 0f) {
			meleeAttackCooldownTimer -= Time.deltaTime;
		}

		//Bow Attack timer
		if (bowAttackCooldownTimer > 0f) {
			bowAttackCooldownTimer -= Time.deltaTime;
		}

	}

	void LateUpdate () {
		if (fsm.State == States.Respawn)
			return;

		// Landed squash
		if (onGround && !wasOnGround && Speed.y <= 0) {
			SpriteScale.x = 1.35f;
			SpriteScale.y = 0.65f;
		}

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

		// kill Box
		//if (Mathf.Abs(transform.position.x) > 192 || Mathf.Abs(transform.position.y) > 108) {
		//	Die ();
		//}

		// Get Crushed by block if we are collisioning with the solid layer
		if (CollisionSelf(solid_layer)) {
			Die ();
		}
	}

	void Normal_Update () {
		// Ducking over here
		if (CanDuck) {
			fsm.ChangeState(States.Ducking, StateTransition.Overwrite);
			return;
		}

		// Dash over here
		if (CanDash) {
			fsm.ChangeState(States.Dash, StateTransition.Overwrite);
			return;
		}

		// Melee Attack over here
		if (CanAttack) {
			meleeAttackCooldownTimer = MeleeAttackCooldownTime;
			fsm.ChangeState(States.Attack, StateTransition.Overwrite);
			return;
		}

		// Bow Attack over here
		if (CanShoot) {
			bowAttackCooldownTimer = BowAttackCooldownTime;
			fsm.ChangeState(States.BowAttack, StateTransition.Overwrite);
			return;
		}
			
		// Cling to wall
		if (((moveX > 0 && CheckColInDir (Vector2.right * -1f, solid_layer)) || (moveX < 0 && CheckColInDir (Vector2.right, solid_layer))) && canStick && !onGround) {
			stickTimer = clingTime;
			sticking = true;
			canStick = false;
		}

		// Horizontal Speed Update Section
		float num = onGround ? 1f : AirMult;
	
		if (!sticking) {
			if (Mathf.Abs (Speed.x) > MaxRun && Mathf.Sign (Speed.x) == moveX) {
				Speed.x = Calc.Approach (Speed.x, MaxRun * moveX, RunReduce * num * Time.deltaTime);
			} else {
				Speed.x = Calc.Approach (Speed.x, MaxRun * moveX, RunAccel * num * Time.deltaTime);
			}
		}
			
		// Vertical Speed Update & Fast Fall Section
		float num3 = MaxFall;
		//float num4 = 240f;
		/*
		if ((int)Input.GetAxisRaw("Horizontal") < 0 && Speed.y > 0) {
			Debug.Log ("Fast Falling");
			maxFall = Approach(maxFall, num4, 450f * Time.deltaTime);
			float num5 = num3 + (num4 - num3) * 0.5f;
			if (Speed.y >= num5)
			{
				float amount = Mathf.Min(1f, (Speed.y - num5) / (num4 - num5));
				Debug.Log (amount.ToString ());
				SpriteScale.x = Mathf.Lerp(1f, 0.5f, amount);
				SpriteScale.y = Mathf.Lerp(1f, 1.5f, amount);
			}

		} else {
			*/
			maxFall = Calc.Approach(maxFall, num3, 300f * Time.deltaTime);
		//}

		// Ladder climb
		if (moveY != 0 && moveY != oldMoveY && CheckColAtPlace(Vector2.up * moveY * 2, ladder_layer)) {
			var myX = (int)myCollider.bounds.center.x;
			var myExtentX = (int)myCollider.bounds.extents.x;
			var extraDistanceX = 2; // 2-4 recommended when tile size is 16 pixels, adjust accordingly
			for (int i = -myExtentX - extraDistanceX; i <= myExtentX + extraDistanceX; i++) {
				if (CanClimbLadder ((int)myX + i, moveY)) {
					ClimbLadder((int)myX + i, moveY);
					return;
				}
			}
		}


		// Wall Slide
		if (!onGround) {
			float target = maxFall;
			// Wall Slide
			if ((moveX == (int)Facing) && moveY != -1) {
				if (Speed.y <= 0f && wallSlideTimer > 0f && CheckColInDir (Vector2.right * (int)Facing, solid_layer)) {
					wallSlideDir = (int)Facing;
				}
				if (wallSlideDir != 0) {
					target = Mathf.Lerp (MaxFall, WallSlideStartMax, wallSlideTimer / WallSlideTime);
				}
			}

			Speed.y = Calc.Approach (Speed.y, target, Gravity * Time.deltaTime);
		}

		// Handle facing direction
		if (wallSlideDir == 0) {
			// Handle Facings
			if (moveX != 0) {
				Facings facings = (Facings)moveX;
				Facing = facings;
			}
		} else {
			// Dust particles
			if (Random.value < 0.35f && GameManager.instance != null) {
				GameManager.instance.Emit (DashDustParticle, 1, new Vector2 (transform.position.x, transform.position.y) + new Vector2 (wallSlideDir * 2.5f, 4), Vector2.one * 1f);
			}
		}

		// Handle variable jumping 
		if (varJumpTimer > 0f) {
			if (Input.GetButton("Jump")) {
				Speed.y = Mathf.Max(Speed.y, varJumpSpeed);
			} else {
				varJumpTimer = 0f;
			}
		}

		// Drop down from a one way platform
		if (onGround && moveY < 0f && jumpIsInsideBuffer && CheckColInDir(Vector2.down, oneway_layer) && !CheckColInDir(Vector2.down, solid_layer)) {
			onGround = false;
			jumpGraceTimer = 0f;
			jumpIsInsideBuffer = false;
			jumpBufferTimer = 0f;
			transform.position += new Vector3 (0, -1, 0);
		}

		// Jump
		if (jumpIsInsideBuffer) { 
			if (onGround || jumpGraceTimer > 0f) {
				// Jump Normally
				Jump ();
			} else {
				// Wall Jump Left
				if (WallJumpCheck(-1)) {
					WallJump (1);
					// Wall Jump Right
				} else if (WallJumpCheck(1)) {
					WallJump (-1);
				}
			} 
		}
        if (OnСeiling && !CheckColAtPlace(Vector2.up * 7, solid_layer) && onGround)
        {
            myNormalCollider.enabled = true;
            myDuckingCollider.enabled = false;
            myCollider = myNormalCollider;
            onDucking = false;

            SpriteScale = new Vector2(0.7f, 1.3f);
            OnСeiling = false;
        }
    }

    private void Ducking_Enter()
    {
        // Use the ducking hitbox
        myDuckingCollider.enabled = true;
        myNormalCollider.enabled = false;
        myCollider = myDuckingCollider;

        SpriteScale = new Vector2(1.3f, 0.7f);
    }

    private void Ducking_Exit()
    {
        if (!CheckColAtPlace(Vector2.up * 7, solid_layer))
        {
            // Use the normal hitbox
            myNormalCollider.enabled = true;
            myDuckingCollider.enabled = false;
            myCollider = myNormalCollider;
            onDucking = false;

            SpriteScale = new Vector2(0.7f, 1.3f);
        }
        else
        {
            OnСeiling = true;
        }
    }

    private void Ducking_Update()
    {
        // Drop down from a one way platform
        if (onGround && moveY < 0f && jumpIsInsideBuffer && CheckColInDir(Vector2.down, oneway_layer) && !CheckColInDir(Vector2.down, solid_layer))
        {
            onGround = false;
            jumpGraceTimer = 0f;
            jumpIsInsideBuffer = false;
            jumpBufferTimer = 0f;
            transform.position += new Vector3(0, -1, 0);
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            return;
        }

            if (moveY != -1 || !onGround)
        {
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            return;
        }

        if (wallSlideDir == 0)
        {
            // Handle Facings
            if (moveX != 0)
            {
                Facings facings = (Facings)moveX;
                Facing = facings;
            }
        }
        if (Mathf.Abs(Speed.x) > MaxRun && Mathf.Sign(Speed.x) == moveX)
        {
            Speed.x = Calc.Approach(Speed.x / 1.1f, MaxRun * moveX, RunReduce * Time.deltaTime);
        }
        else
        {
            Speed.x = Calc.Approach(Speed.x / 1.1f, MaxRun * moveX, RunAccel * Time.deltaTime);
        }

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
        onDucking = true;
    }

    private void LadderClimb_Enter () {
		jumpIsInsideBuffer = false;
		jumpBufferTimer = 0f;
		jumpGraceTimer = 0f;
	}

	private void LadderClimb_Exit () {
		// Add extra feel when leaving the ladder state
		SpriteScale = new Vector2 (1.2f, 0.8f);
	}

	private void LadderClimb_Update () {
		// If jump buttom is being pressed and pressing down just fall
		if (moveY < 0 && jumpIsInsideBuffer) {
			fsm.ChangeState (States.Normal, StateTransition.Overwrite);
			return;
			// Jump when the jump buttom is pressed
		} else if ((moveX != 0 && jumpIsInsideBuffer) || jumpIsInsideBuffer) {
			Jump ();
			fsm.ChangeState (States.Normal, StateTransition.Overwrite);
			return;
			// Land when touching the ground and press down
		} else if (onGround && moveY < 0) {
			fsm.ChangeState (States.Normal, StateTransition.Overwrite);
			return;
		}

		// Up and Down (Vertical)
		Speed.y = Calc.Approach (Speed.y, LadderClimbSpeed * moveY, 10000 * Time.deltaTime);

		// Drop when there is no more ladder below and pressing down or there is no collision with ladder
		if ((moveY < 0 && !CanClimbLadder ((int)transform.position.x, -1)) || !CanClimbLadder ((int)transform.position.x, 0)) {
			fsm.ChangeState (States.Normal, StateTransition.Overwrite);
			return;
			// Jump if you've reached the top of the ladder and pressing UP
		} else if (moveY > 0 && !CanClimbLadder ((int)transform.position.x, 1)) {
			Jump ();
			fsm.ChangeState (States.Normal, StateTransition.Overwrite);
			return;
		}
	}
		
	private IEnumerator Dash_Enter () {
		dashCooldownTimer = DashCooldown;
		Speed = Vector2.zero;
		DashDir = Vector2.zero;
			
		Vector2 value = new Vector2(moveX, moveY);
		if (value == Vector2.zero) {
			value = new Vector2 ((int)Facing, 0f);
		} else if (value.x == 0 && value.y > 0 && onGround) {
			value = new Vector2 ((int)Facing, value.y);
		}
		value.Normalize();
		Vector2 vector = value * DashSpeed;
		Speed = vector;
		DashDir = value;
		if (DashDir.x != 0f) {
			Facing = (Facings)Mathf.Sign (DashDir.x);
		}
		if (DashDir.y < 0 && onGround) {
			DashDir.y = 0;
			DashDir.x = Mathf.Sign (DashDir.x);
			Speed.y = 0f;
			Speed.x *= 2f;
		}

		// Squash & Stretch
		if (DashDir.x != 0 && DashDir.y == 0) {
			SpriteScale = new Vector2 (1.2f, 0.8f);
		} else if (DashDir.x == 0 && DashDir.y != 0) {
			SpriteScale = new Vector2 (.8f, 1.2f);
		}
			
		// Screenshake
		if (PixelCameraController.instance != null) {
			PixelCameraController.instance.DirectionalShake (DashDir);
		}
			
		yield return new WaitForSeconds (DashTime);

		// Wait one extra frame
		yield return null;
			
		if (DashDir.y >= 0f) {
			Speed = DashDir * EndDashSpeed;
		}
		if (Speed.y > 0f) {
			Speed.y = Speed.y * EndDashUpMult;

		}
		fsm.ChangeState (States.Normal, StateTransition.Overwrite);
		yield break;
	}

	private void Dash_Update () {
		// Dust particles
		if (Random.value < 0.85f && GameManager.instance != null) {
			GameManager.instance.Emit (DashDustParticle, Random.Range(1,3), new Vector2 (transform.position.x, transform.position.y) + new Vector2 (0f, -5), Vector2.one * 3f);
		}
	}
	
	void Respawn_Enter () {
		if (GameManager.instance != null) {
			respawnPosition = GameManager.instance.LevelSpawnPoint.position;
		} else {
			respawnPosition = Vector2.zero;
		}
		GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;
		//animator.Play("Fall");
		moveToRespawnPosTimer = 0f;
	}

	void Respawn_Exit () {
		respawnTimer = 0f;
		transform.position = respawnPosition;
		GetComponentInChildren<SpriteRenderer>().sortingOrder = -1;
		SpriteScale = new Vector2(1.5f, 0.5f);

		// Trigger the Respawn Events on the gamemanager
		if (GameManager.instance != null) {
			GameManager.instance.PlayerRespawn ();
		}

		var health = GetComponent<Health> ();
		health.dead = false;
		health.TakeHeal (health.maxHealth);
	}

	void Respawn_Update () {
		if (moveToRespawnPositionAfter > 0f) {
			moveToRespawnPosTimer += Time.deltaTime;

			if (moveToRespawnPosTimer < moveToRespawnPositionAfter) {
				return;
			}
		}
		 

		respawnTimer += Time.deltaTime;
		var t = respawnTimer / RespawnTime;

		if (t >= 1f) {
		fsm.ChangeState(States.Normal,StateTransition.Overwrite);
		return;
		}

		transform.position = Vector2.Lerp(transform.position, respawnPosition, t);
	}

	void Attack_Update () {
		// Horizontal Speed Update Section
		float num = onGround ? 1f : AirMult;

		Speed.x = Calc.Approach (Speed.x, 0f, RunReduce * num * Time.deltaTime);

		if (!onGround) {
			float target = MaxFall;
			Speed.y = Calc.Approach (Speed.y, target, Gravity * Time.deltaTime);
		}
	}

	void BowAttack_Update () {
		// Horizontal Speed Update Section
		float num = onGround ? 1f : AirMult;

		Speed.x = Calc.Approach (Speed.x, 0f, RunReduce * num * Time.deltaTime);

		if (!onGround) {
			float target = MaxFall;
			Speed.y = Calc.Approach (Speed.y, target, Gravity * Time.deltaTime);
		}
	}

	void LedgeGrab_Enter () {
		Speed = Vector2.zero;
		if (wallSlideDir != 0)
			wallSlideDir = 0;
	}

	void LedgeGrab_Update () {
		// If pressing down or the other direction which is not the ledgegrab direction
		if (moveY < 0 || moveX != (int)Facing) {
			jumpGraceTimer = JumpGraceTime;
			fsm.ChangeState (States.Normal, StateTransition.Overwrite);
			return;
		}

		if (jumpIsInsideBuffer) {

			// Ledge Climb
			if (moveX == (int) Facing || moveY > 0) {
				// Swap to the ledgeclimb state
				fsm.ChangeState (States.LedgeClimb, StateTransition.Overwrite);
				return;
			} else if (moveY != -1) {
				Jump ();
			} else if (WallJumpCheck((int)Facing)) { // Wall Jump
				WallJump (-(int)Facing);
			}
			// Swap back to the normal state
			fsm.ChangeState (States.Normal, StateTransition.Overwrite);
			return;
		}
	}

	void LedgeClimb_Enter () {
		// Disable the collider while on the ledgeclimb state
		myCollider.enabled = false;
		// Safeproo reset the timer just in case
		ledgeClimbTimer = 0f;
		// Set the speed to 0
		Speed = Vector2.zero;
		// Reset the wallslidedir
		if (wallSlideDir != 0)
			wallSlideDir = 0;
	}

	void LedgeClimb_Exit () {
		// Add the extra position so our new position matches the end position on the animation
		transform.position = new Vector2(transform.position.x + (extraPosOnClimb.x * (int)Facing), transform.position.y + extraPosOnClimb.y);
		// Enable back again the collider
		myCollider.enabled = true;
		// This is done so the on land squash doesn't happen
		onGround = true;
		wasOnGround = true;

		// Start the idle animation 
		if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Idle")) {
			animator.Play ("Idle");
		}

	}

	void LedgeClimb_Update () {
		ledgeClimbTimer += Time.deltaTime;

		// Check if the timer is equal or higher than the total climb time
		if (ledgeClimbTimer >= ledgeClimbTime) {
			ledgeClimbTimer = 0f;
			fsm.ChangeState (States.Normal, StateTransition.Overwrite);
			return;
		}
	}

	// Jump Function
	public void Jump () {
		wallSlideTimer = WallSlideTime;
		jumpGraceTimer = 0f;
		jumpBufferTimer = 0f;
		varJumpTimer = VariableJumpTime;
		Speed.x = Speed.x + JumpHBoost * (float)moveX;
		Speed.y = JumpSpeed;
		varJumpSpeed = Speed.y;
		SpriteScale = new Vector2(0.6f, 1.4f);

		// Dust particles
		if (GameManager.instance != null) {
			GameManager.instance.Emit (DustParticle, 5, new Vector2 (transform.position.x, transform.position.y) + new Vector2 (0f, -5), Vector2.one * 3f);
		}
	}

	// Perform a wall jump in a set direction (left == -1 or right == 1)
	private void WallJump(int dir) {
		wallSlideTimer = WallSlideTime;
		jumpGraceTimer = 0f;
		jumpBufferTimer = 0f;
		varJumpTimer = VariableJumpTime;
		if (moveX != 0) {
			forceMoveX = dir;
			forceMoveXTimer = WallJumpForceTime;
		}

		Speed.x = WallJumpHSpeed * (float)dir;
		Speed.y = JumpSpeed;
		varJumpSpeed = Speed.y;
		SpriteScale = new Vector2(0.6f, 1.4f);

		// Dust particles
		if (GameManager.instance != null) {
			if (dir == -1) {
				GameManager.instance.Emit (DustParticle, 5, new Vector2 (transform.position.x, transform.position.y) + new Vector2 (2f, -5), Vector2.one * 3f);
			} else if (dir == 1) {
				GameManager.instance.Emit (DustParticle, 5, new Vector2 (transform.position.x, transform.position.y) + new Vector2 (-2f, -5), Vector2.one * 3f);
			}
		}
	}

	// Function to check for collisions with walls at a custom check distance
	private bool WallJumpCheck(int dir) {
		Vector2 leftcorner = Vector2.zero; 
		Vector2 rightcorner = Vector2.zero;

		if (dir == -1) {
			leftcorner = new Vector2 (myCollider.bounds.center.x - myCollider.bounds.extents.x - .5f - (float)WallJumpCheckDist, myCollider.bounds.center.y + myCollider.bounds.extents.y - 1f);
			rightcorner = new Vector2 (myCollider.bounds.center.x - myCollider.bounds.extents.x, myCollider.bounds.center.y - myCollider.bounds.extents.y + 1f);
		} else if (dir == 1) {
			leftcorner = new Vector2 (myCollider.bounds.center.x + myCollider.bounds.extents.x, myCollider.bounds.center.y + myCollider.bounds.extents.y - 1f);
			rightcorner = new Vector2 (myCollider.bounds.center.x + myCollider.bounds.extents.x + .5f + (float)WallJumpCheckDist, myCollider.bounds.center.y - myCollider.bounds.extents.y + 1f);
		}

		return Physics2D.OverlapArea(leftcorner, rightcorner, solid_layer);
	}

	// Spring jump/bounce function
	public void SpringBounce() {
		varJumpTimer = SpringJumpVariableTime; // Set the amount of time people can hold the jump button to reach higher
		wallSlideTimer = WallSlideTime; // reset the wall slide timer
		jumpGraceTimer = 0f; // Avoid people from jumping in the air after performing a spring bounce/jumping off a spring
		Speed.x = 0f; // Set the horizontal speed to 0
		Speed.y = SpringJumpSpeed; // Set the Y speed to the desired one 
		varJumpSpeed = Speed.y; // Simply set the variable jumping speed (hold to jump higher)

		// Screenshake
		if (PixelCameraController.instance != null) {
			PixelCameraController.instance.DirectionalShake (Vector2.up, 0.1f);
		}

		// Change the sprite scale to add that extra "juice"
		SpriteScale = new Vector2(0.6f, 1.4f);
	}

	// This function checks wether or not there is a ledge to grab in the target Y position and in the target horizontal direction (right or left)


	private void GrabLedge(int targetY, int direction)
	{
		// Set the facing direction
		Facing = (Facings)direction;
		// The collider's extents on the Y axis (half of height)
		var extentsY = myCollider.bounds.extents.y + 1;
		// Set the Y position to the desired one according to our collider's size and the targetY
		transform.position = new Vector2(transform.position.x, (float)(targetY - extentsY));
		// Set the vertical speed to 0
		Speed.y = 0f;

		// We check for collisions with the ledge on the set direction
		while (!CheckColAtPlace(Vector2.right * direction, solid_layer))
		{
			transform.position = new Vector2 (transform.position.x + direction, transform.position.y);
		}

		// Change the sprite scale to add extra feel
		SpriteScale = new Vector2(1.3f, 0.7f);
		// Change the state to the ledge grabbing state
		fsm.ChangeState (States.LedgeGrab, StateTransition.Overwrite);
	}

	private bool CanClimbLadder(int targetX, int direction)
	{		
		// This is just in case the game manager hasn't been assigned we use a default tilesize value of 16
		var tileSize =  Vector2.one * 32; 

		return CollisionAtPlace (new Vector2 ((float)targetX, transform.position.y + direction * (tileSize.y / 2)), ladder_layer) &&
			!CollisionAtPlace (new Vector2 ((float)targetX - ((tileSize.x / 2) + 2), transform.position.y + direction * (tileSize.y / 2)), ladder_layer) &&
			!CollisionAtPlace (new Vector2 ((float)targetX + ((tileSize.x / 2) + 2), transform.position.y + direction * (tileSize.y / 2)), ladder_layer) &&
			!CollisionAtPlace (new Vector2 ((float)targetX, transform.position.y + direction * (tileSize.y / 2)), solid_layer);

	}

	private void ClimbLadder (int targetX, int direction) {
		// Set the vertical speed to 0
		Speed.y = 0;

		//var colliders = checkcol

		// Set the position to the desired one
		transform.position = new Vector2((float)targetX, transform.position.y);

		// Change the sprite scale to add extra feel
		SpriteScale = new Vector2(0.8f, 1.2f);
		// Change the state to the ladder climbing state
		fsm.ChangeState (States.LadderClimb, StateTransition.Overwrite);
	}

	// Dash Refill Function
	public bool UseRefillDash()
	{
		// Instantly reduce the cooldown to 0 (refill)
		if (dashCooldownTimer > 0f)
		{
			dashCooldownTimer = 0f;
			return true;
		}
		return false;

		/* 
		// This version is used when you've a set amount of dashes like in celeste (Only use one version of this function)
		if (Dashes < MaxDashes)
		{
			Dashes = MaxDashes;
			return true;
		}
		return false;
		*/
	}

	// Function to update the sprite scale, facing direction and animations 
	void UpdateSprite () {
		// Approch the normal sprite scale at a set rate
		if (fsm.State != States.Dash) {
			SpriteScale.x = Calc.Approach (SpriteScale.x, 1f, 0.04f);
			SpriteScale.y = Calc.Approach (SpriteScale.y, 1f, 0.04f);
		}

		// Set the SpriteHolder scale to the target scale
		var targetSpriteHolderScale = new Vector3 (SpriteScale.x, SpriteScale.y, 1f);
		if (SpriteHolder.localScale != targetSpriteHolderScale) {
			SpriteHolder.localScale = targetSpriteHolderScale;
		}

		// Set the x scale to the current facing direction
		var targetLocalScale = new Vector3 ((int)Facing, transform.localScale.y, transform.localScale.z);
		if (transform.localScale != targetLocalScale) {
			transform.localScale = new Vector3 ((int)Facing, transform.localScale.y, transform.localScale.z);
		}

		if (fsm.State == States.LedgeClimb) {
			if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("LedgeClimb")) {
				//animator.Play ("LedgeClimb");
			}
			// If on the ledge grab state
		} else if (Speed.x == 0 && onDucking)
        {
            animator.Play("DuckingIdle");
        } else if (fsm.State == States.LedgeGrab) {
			if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("LedgeHang")) {
				//animator.Play ("LedgeHang");
			}
			// If on the Ladder Climbing state
		} else if (fsm.State == States.LadderClimb) {
			if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("LadderClimb")) {
				//animator.Play ("LadderClimb");
			}

			//animator.speed = Mathf.Abs(moveY);
			animator.SetFloat("LadderSpeed", moveY);

			// If on the attack state
		} else if (fsm.State == States.BowAttack) {
			if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Shoot")) {
				animator.Play ("Shoot");
			}
			// If on the attack state
		} else if (fsm.State == States.Attack) {
			if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Attack")) {
				animator.Play ("Attack");
			}
			// If on the dash state
		} else 	if (fsm.State == States.Dash) {
			if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Dash")) {
				//animator.Play ("Dash");
			}
			// If on the ground
		} else if (onGround) {
			if (fsm.State == States.Ducking) {
				// Idle Animation
				if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Ducking")) {
					animator.Play ("Ducking");
				}
				// If the is nohorizontal movement input
			} else if (moveX == 0) {
				// Idle Animation
				if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Idle")) {
					animator.Play ("Idle");
				}
			// If there is horizontal movement input
			} else if (pushing) {
				// Push Animation
				if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Push")) {
					//animator.Play ("Push");
				}
			} else {
				// Run Animation
				if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Run")) {
					animator.Play ("Run");
				}
			}
		// If not on the ground
		} else {
			// If the wall slide direction is not 0 (0 equals not sliding)
			if (wallSlideDir != 0) {
				// Wall Slide Animation
				if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("WallSlide")) {
					//animator.Play ("WallSlide");
				}
			// If not sliding and speed is upward
			} else if (Speed.y > 0) {
				// Jump Animation
				if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Jump")) {
					//animator.Play ("Jump");
				}
			// if speed is not upwards then it is downward
			} else if (Speed.y <= 0) {
				// Fall Animation
				if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("Fall")) {
					//animator.Play ("Fall");
				}
			}
		}
	}

	// Die Function
	public void Die () {
        ResetObj.ResetPosition();
        // Set the speed to 0
        Speed = Vector2.zero;
		// Trigget the respawn state
		fsm.ChangeState (States.Respawn, StateTransition.Overwrite);
		// Trigger the Dead Events on the gamemanager
		if (GameManager.instance != null) {
			GameManager.instance.PlayerDead ();
		}
	}
}