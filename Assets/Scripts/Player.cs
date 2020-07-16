#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public class Player : Actor
{

    [Header("Movement Variables")]
    // Gravity, Maximun fall speed & fastfall Speed
    public float Gravity = 900f; // Скорость, с которой игрок падает, пока находится в воздухе
    public float MaxFall = -160f; // Максимальная общая скорость, с которой вы можете упасть
    public float FastFall = -240f; // Максимальная скорость падения при быстром падении
    // Скорость бега и ускорение
    public float MaxRun = 90f; //  Максимальная скорость бега по горизонтали

    public float RunAccel = 1000f; // Скорость горизонтального ускорения
    public float RunReduce = 400f; // Горизонтальное ускорение, когда ваша горизонтальная скорость выше или равна максимальной
    // Множитель в воздухе
    public float AirMult = 0.65f; // Множитель для горизонтального перемещения воздуха (трения), чем выше, тем больше контроля в воздухе
    // Переменные для прыжка
    public float JumpSpeed = 135f; // Скорость вертикального прыжка / Сила
    public float JumpHBoost = 40f; // Повышение скорости по горизонтали при прыжке
    public float VariableJumpTime = 0.2f; // Время после выполнения прыжка, при котором вы можете удерживать клавишу прыжка, чтобы прыгнуть выше
    public float SpringJumpSpeed = 275f; // Скорость вертикального прыжка / Сила прыжков на пружине
    public float SpringJumpVariableTime = 0.05f; // Время после выполнения прыжка с пружины, при котором вы можете удерживать клавишу прыжка, чтобы прыгнуть выше
    // Переменные для прыжков от стен
    public float WallJumpForceTime = 0.16f; // Время, когда горизонтальное движение возобновляется / форсируется после прыжка со стены (если он слишком низок, игрок может взобраться на стену)
    public float WallJumpHSpeed = 130f; // Увеличение скорости по горизонтали при выполнении прыжка со стены
    public int WallJumpCheckDist = 2; // Расстояние, на котором мы проверяем стены перед выполнением прыжка со стены (рекомендуется 2-4)
    // Переменные для скольжения по стене
    public float WallSlideStartMax = -20f; //  Начальная вертикальная скорость, когда вы скользите по стене
    public float WallSlideTime = 1.2f; // Максимальное время, когда вы можете скользить по стене, прежде чем снова набрать полную скорость падения
    // Параметры для рывка и переката
    public float DashSpeed = 240f; // Скорость / Сила рывка (dash)
    public float EndDashSpeed = 160f; // Повышенная скорость, когда рывок заканчивается (рекомендуется 2/3 скорости рывка)
    public float EndRollSpeed = 160f; // Повышение скорости при окончании переката (рекомендуется 2/3 скорости переката)
    public float EndDashUpMult = 0.75f; // Множитель применяется к скорости после окончания рывка, если направление, в котором вы летели, было вверх
    public float EndRollUpMult = 0.75f; // Множитель применяется к скорости после окончания переката, если направление, в котором вы катились, было вверх
    public float DashTime = 0.15f; // Общее время, которое длится рывок
    public float RollTime = 0.15f; // Общее время, которое длится перекат
    public float DashCooldown = 0.4f; // Время восстановления рывка
    public float RollCooldown = 0.4f; // Время восстановления переката
    // Другие переменные, используемые для адаптивного движения
    public float clingTime = 0.125f;  // Время после прикосновения к стене, где вы не можете покинуть стену (чтобы избежать непреднамеренного ухода от стены при попытке выполнить прыжок с стены)
    public float JumpGraceTime = 0.1f; // Время отсрочки прыжка после того, как вы покинули землю без прыжка, на котором вы все еще можете сделать прыжок
    public float JumpBufferTime = 0.1f; // Если игрок ударяет об землю в течение этого времени после нажатия кнопки прыжка, прыжок будет выполнен, как только он коснется земли
                                        // Лестничные переменные
    public float LadderClimbSpeed = 60f;

    [Header("Hands")]
    public int HandAttackMinDamage = 1;//new int[2] {1, 0, 0};
    public int HandAttackMaxDamage = 2;//new int[2] {2, 0, 0};
    public int HandStepUpAfterHit = 20; //new int[2] {20,0,0};

    [Header("MeleeAttacks")]
	public string MeleeWeaponType;
    public int MeleeCriticalDamage = 0;
    public int MeleeAttackMaxDamage;
    public int MeleeAttackMinDamage;
    public int MeleeCriticalDamageMultiply;
    public int MeleeCriticalDamageChance;
    public float MeleeAttackSpeed = 1f;
    private int StepUpAfterHit;
    private float MeleeAttackCooldownTime = 0.8f;
    private float MeleePowerAttackCooldownTime = 0.8f;
    public int MeleePowerAttackMaxDamage = 0;
    public int MeleePowerAttackMinDamage = 0;

    //public bool MeleeAttackCanPowerAttackCriticalDamage; // использовать для особой атаки (не реализовано)
    [HideInInspector]
    public bool MeleeAttackCanThirdAttackCriticalDamage; 

    public bool MeleeAttackCanPoison = false;
	[HideInInspector]
	public int MeleePoisonDamaged = 0;
	[HideInInspector]
	public int MeleePoisonFrequency = 0;
	[HideInInspector]
	public int MeleePoisonTick = 0;
    [HideInInspector]
    public bool MeleeAttackCanFire = false;
	[HideInInspector]
	public int MeleeFireDamaged = 0;
	[HideInInspector]
	public int MeleeFireFrequency = 0;
	[HideInInspector]
	public int MeleeFireTick = 0;
    [HideInInspector]
    public bool MeleeAttackCanPush = false;
    [HideInInspector]
    public int MeleePushDistance = 0;
    [HideInInspector]
    public bool MeleeAttackCanFreez = false;
    [HideInInspector]
    public int MeleeFreezDuration = 0;
    [HideInInspector]
    public float HandAttackCooldownTime = 0.8f;
    [HideInInspector]
    public float SecondSwordAttackCooldownTime = 0.2f;
    [HideInInspector]
    public float ThirdSwordAttackCooldownTime = 0.2f;
    [HideInInspector]
    public float MeleeBlockCooldownTime = 0.5f;

    [Header("RangedAttacks")]
	public string RangedWeaponType;
    public int RangedAttackDamage = 0;
    public int RangedCriticalDamage = 0;
    public int RangedCriticalDamageMultiply;
    public int RangedCriticalDamageChance;
    public int RangedAttackMinDamage = 1;//new int[2] {1, 0, 0};
    public int RangedAttackMaxDamage = 2;//new int[2] {2, 0, 0};
    public float RangedAttackCooldownTime = 1f;
    public float RangedPowerAttackCooldownTime = 10f;

	[HideInInspector]
	public bool RangedAttackCanPoison = false;
	[HideInInspector]
	public int RangedPoisonDamaged = 0;
	[HideInInspector]
	public int RangedPoisonFrequency = 0;
	[HideInInspector]
	public int RangedPoisonTick = 0;
	[HideInInspector]
	public bool RangedAttackCanFire = false;
	[HideInInspector]
	public int RangedFireDamaged = 0;
	[HideInInspector]
	public int RangedFireFrequency = 0;
	[HideInInspector]
	public int RangedFireTick = 0;
	[HideInInspector]
	public bool RangedAttackCanPush = false;
    [HideInInspector]
    public bool RangedAttackCanThroughShoot = false;

    [Header("Facing Direction")]
    public Facings Facing;  // Facing Direction
    [Header("Wall Slide Direction")]
    public int wallSlideDir;    // Wall Slide Direction
    [Header("Ladder")]
    public bool isInLadder = false;

    [Header("Respawn")]
    public float RespawnTime; // Total Time it takes the player to respawn
    private float respawnTimer = 0f; // Variable to store the timer of the respawn
    private Vector2 respawnPosition; // Variable to store the position you respawn at

    [Header("Ducking & Colliders")]
    public BoxCollider2D myNormalCollider; // Collider used while you're on any state except for the ducking state
    public BoxCollider2D myDuckingCollider; // Collider used while in the ducking state
    public float DuckingFrictionMultiplier = 0.5f; // The friction multiplier for when you're ducking 0.4-0.6 recommended (0 is no friction, 1 is the normal friction)

    // Helper private Variables
    private int moveX; // Variable to store the horizontal Input each frame
    private int moveY; // Variable to store the vectical Input each frame
    [HideInInspector]
    public float curRun = 0;
    private int oldMoveY; // Variable to store the he vertical Input for the last frame
    private float varJumpSpeed; // Vertical Speed to apply on each frame of the variable jump
    private float varJumpTimer = 0f; // Variable to store the time left on the variable jump
    private int forceMoveX; // Used to store the forced horizontal movement input
    private float forceMoveXTimer = 0f; // Used to store the time left on the force horizontal movement
    private float maxFall; // Variable to store the current maximun fall speed
    private float wallSlideTimer = 0f; // Used to store the time left on the wallslide
    private Vector2 DashDir; // Here we store the direction in which we are dashing
    private Vector2 RollDir; // Here we store the direction in which we are rolling
    private float dashCooldownTimer = 0f; // Timer to store how much cooldown has the dash
    private float rollCooldownTimer = 0f; // Timer to store how much cooldown has the roll
    private bool canStick = false; // Helper variable for the wall sticking functionality
    private bool sticking = false; // Variable to store if the player is currently sticking to a wall
    private float stickTimer = 0f; // Timer to store the time left sticking to a wall 
    private float jumpGraceTimer = 0f; // Timer to store the time left to perform a jump after leaving a platform/solid
    private float jumpBufferTimer = 0f; // Timer to store the time left in the JumpBuffer timer
    private bool jumpIsInsideBuffer = false;
    private float meleeAttackCooldownTimer = 0f; // Timer to store the cooldown left to use the melee attack
    private float meleePowerAttackCooldownTimer = 0f; // Timer to store the cooldown left to use the melee attack
    private float meleeBlockCooldownTimer = 0f;
    private float secondSwordAttackCooldownTimer = 0f; // Timer to store the cooldown left to use the melee attack
    private float thirdSwordAttackCooldownTimer = 0f; // Timer to store the cooldown left to use the melee attack
    private float rangedAttackCooldownTimer = 0f; // Timer to store cooldown left on the bow attak
    private float rangedPowerAttackCooldownTimer = 0f; // Timer to store cooldown left on the bow attak
    private float moveToRespawnPositionAfter = .5f; // Time to wait before moving to the respawn position
    private float moveToRespawnPosTimer = 0f; // Timer to store how much time is left before moving to the respawn position
    private bool rollAgain = false;
    private float ledgeClimbTime = 1f; // Total time it takes to climb a wall
    private float ledgeClimbTimer = 0f; // Timer to store the current time passed in the ledgeClimb state
    private Vector2 extraPosOnClimb = new Vector2(10, 16); // Extra position to add to the current position to the end position of the climb animation matches the start position in idle state
    public int hitCount = 0;
    private bool hasBow = false;
    private bool hasSword = false;
	
    private bool hasSeries = false;
    private bool hasBlock = false;

    [SerializeField]
    private Health owner;

    List<Health> healthsDamaged = new List<Health>();
	public List<ItemParameters> parameters = new List<ItemParameters>();

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
            return !onGround && Input.GetButtonDown("Dash") && dashCooldownTimer <= 0f;
        }
    }

    public bool CanRoll
    {
        get
        {
            return (onGround && rollCooldownTimer <= 0f && moveX != 0 && moveY < 0 && !jumpIsInsideBuffer) || rollAgain;
        }
    }

    public bool CanHandAttack
    {
        get
        {
            return activeWeapon == ActiveWeapon.Hands && Input.GetButtonDown("Attack") && meleeAttackCooldownTimer <= 0f;
        }
    }

    public bool CanSwordAttack
    {
        get
        {
            return hitCount == 0 && activeWeapon == ActiveWeapon.Sword && Input.GetButtonDown("Attack") && meleeAttackCooldownTimer <= 0f;
        }
    }

    public bool CanSecondSwordAttack
    {
        get
        {
            return (hasSeries && hitCount == 1 && activeWeapon == ActiveWeapon.Sword) && Input.GetButtonDown("Attack") && meleeAttackCooldownTimer <= 0f;
        }
    }

    public bool CanThirdSwordAttack
    {
        get
        {
            return hasSeries && hitCount == 2 && activeWeapon == ActiveWeapon.Sword && Input.GetButtonDown("Attack") && meleeAttackCooldownTimer <= 0f;
        }
    }
    public bool CanPowerSwordAttack
    {
        get
        {
            return !hasBlock && activeWeapon == ActiveWeapon.Sword && Input.GetButtonDown("Attack2") && meleePowerAttackCooldownTimer <= 0f;
        }
    }
    public bool CanBlock
    {
        get
        {
            return hasBlock && activeWeapon == ActiveWeapon.Sword && Input.GetButtonDown("Attack2") && meleeBlockCooldownTimer <= 0f;
        }
    }
    public bool CanShoot
    {
        get
        {
            return activeWeapon == ActiveWeapon.Bow && Input.GetButtonDown("Attack") && rangedAttackCooldownTimer <= 0f;
        }
    }
    public bool CanPowerShoot
    {
        get
        {
            return activeWeapon == ActiveWeapon.Bow && Input.GetButtonDown("Attack2") && rangedPowerAttackCooldownTimer <= 0f;
        }
    }

    [Header("Squash & Stretch")]
    public Transform SpriteHolder; // Reference to the transform of the child object which holds the sprite renderer of the player
    public Vector2 SpriteScale = Vector2.one; // The current X and Y scale of the sprite holder (used for Squash & Stretch)

    [Header("Animator")]
    public Animator animator; // Reference to the animator

    [Header("Particles")]
    public GameObject DustParticle;
    public GameObject DashDustParticle;
    public GameObject SparkParticle;
    public GameObject BloodParticle;

    // States for the state machine
    public enum States
    {
        Normal,
        Ducking,
        LadderClimb,
        Dash,
        Respawn,
        LedgeGrab,
        LedgeClimb,
        Attack,
        SwordAttack,
        SecondSwordAttack,
        ThirdSwordAttack,
        SwordBlock,
        BowAttack,
        BowPowerAttack,
        Roll,
        Action,
        PowerSwordAttack
    }

    // State Machine
    public StateMachine<States> fsm;

    public enum ActiveWeapon
    {
        Hands,         // Руки
        Bow,           // Лук 
        Sword          // Меч
    }
    [Header("Initial weapon")]
    public ActiveWeapon activeWeapon; // Активное оружие

    public GameObject InitialWeapon = null;
    new void Awake()
    {
        base.Awake();
        fsm = StateMachine<States>.Initialize(this);

        // This code piece is only neccesary for the ducking functionality
        //Ducking & Normal Colliders Assignment
        if (myNormalCollider == null && myDuckingCollider != null)
        {
            Debug.Log("The player has no Collider attached to it for the normal state");
        }
        else if (myDuckingCollider != null && myNormalCollider != null)
        {
            // Only assign the collider if the ducking collider has been assigned hence only do it if you're planning to use the ducking functionality
            myCollider = myNormalCollider;
        }

    }

    // Use this for initialization
    void Start()
    {
        fsm.ChangeState(States.LadderClimb);
        ChangeWeapon(activeWeapon);
        if (InitialWeapon.GetComponent<PickupRangedWeapons>() != null)
            InitialWeapon.GetComponent<PickupRangedWeapons>().OnPlayerTrigger(this);
        if (InitialWeapon.GetComponent<PickupMeleeWeapons>() != null)
            InitialWeapon.GetComponent<PickupMeleeWeapons>().OnPlayerTrigger(this);
        if (activeWeapon == ActiveWeapon.Hands)
        {
            MeleeAttackMaxDamage = HandAttackMaxDamage;
            MeleeAttackMinDamage = HandAttackMinDamage;
        }
        curRun = MaxRun;

    }

    private void ChangeWeapon(ActiveWeapon activeWeapon) // удалить вместе с вызовом
    {
        if (activeWeapon == ActiveWeapon.Hands)
        {
            MeleeAttackCooldownTime = HandAttackCooldownTime;
            MeleeCriticalDamageMultiply = 0;
            MeleeCriticalDamageChance = 0;
            StepUpAfterHit = HandStepUpAfterHit;

            hasSeries = false;
            hasBlock = false;
        }
        if (activeWeapon == ActiveWeapon.Bow)
        {

        }
        if (activeWeapon == ActiveWeapon.Sword)
        {

        }
    }
    private void SelectWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && hasBow && hasSword)
        {
            switch (activeWeapon)
            {
                case ActiveWeapon.Sword:
                    activeWeapon = ActiveWeapon.Bow;
                    break;
                case ActiveWeapon.Bow:
                    activeWeapon = ActiveWeapon.Sword;
                    break;
            }
            ChangeWeapon(activeWeapon);
        }
        if (Input.GetKeyDown(KeyCode.Tab) && !hasSword && hasBow)
        {
            switch (activeWeapon)
            {
                case ActiveWeapon.Hands:
                    activeWeapon = ActiveWeapon.Bow;
                    break;
                case ActiveWeapon.Bow:
                    activeWeapon = ActiveWeapon.Hands;
                    break;
            }
            ChangeWeapon(activeWeapon);
        }
    }
    // Update is called once per frame
    new void Update()
    {

        base.Update();
        SelectWeapon();

        if (fsm.State == States.Respawn)
            return;

        // Update all collisions here
        wasOnGround = onGround;
        onGround = OnGround();

        // Handle Variable Jump Timer
        if (varJumpTimer > 0f)
        {
            varJumpTimer -= Time.deltaTime;
        }

        // Handle Wall Slide Timer
        if (wallSlideDir != 0)
        {
            wallSlideTimer = Mathf.Max(this.wallSlideTimer - Time.deltaTime, 0f);
            wallSlideDir = 0;
        }

        // Reduce the cooldown of the dash
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Reduce the cooldown of the roll
        if (rollCooldownTimer > 0f)
        {
            rollCooldownTimer -= Time.deltaTime;
        }

        // If on the ground at the start of this frame
        if (onGround)
        {
            wallSlideTimer = WallSlideTime; // Reset the wall slide timer when on the ground
            jumpGraceTimer = JumpGraceTime;
        }
        else if (jumpGraceTimer > 0f) // if not on the ground reduce the jumpgracetimer
        {
            jumpGraceTimer -= Time.deltaTime;
        }

        // Reset the wall cling
        if (onGround || (!CheckColInDir(Vector2.right * 1f, solid_layer) && !CheckColInDir(Vector2.right * -1f, solid_layer)))
        {
            sticking = false;
            canStick = true;
        }

        // Set sticking to false when the timer has expired
        if (stickTimer > 0f && sticking)
        {
            stickTimer -= Time.deltaTime;

            if (stickTimer <= 0f)
            {
                sticking = false;
            }
        }

        // Jump buffer Timer handling
        if (jumpIsInsideBuffer)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        //Jump Input Buffering
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = JumpBufferTime;
        }

        // Check if the jump buffer timer has ran off if so set the jump to false
        if (jumpBufferTimer > 0)
        {
            jumpIsInsideBuffer = true;
        }
        else
        {
            jumpIsInsideBuffer = false;
        }

        // just in case the jumpbuffertime has been set to 0 or less just, just use the old jump input
        if (JumpBufferTime <= 0)
        {
            jumpIsInsideBuffer = Input.GetButtonDown("Jump");
        }

        // Update the moveX Variable depending wether the movement is force or not
        if (forceMoveXTimer > 0f)
        {
            forceMoveXTimer -= Time.deltaTime;
            moveX = forceMoveX;
        }
        else
        {
            moveX = (int)Input.GetAxisRaw("Horizontal");
        }

        // Update the moveY Variable and assign the current vertical input for this frame 
        oldMoveY = moveY;
        moveY = (int)Input.GetAxisRaw("Vertical");

        //Bow Attack timer
        if (rangedAttackCooldownTimer > 0f)
        {
            rangedAttackCooldownTimer -= Time.deltaTime;
        }

        //Bow Power Attack timer
        if (rangedPowerAttackCooldownTimer > 0f)
        {
            rangedPowerAttackCooldownTimer -= Time.deltaTime;
        }

        //Melee Attack timer
        if (meleeAttackCooldownTimer > 0f)
        {
            meleeAttackCooldownTimer -= Time.deltaTime;
        }

        //Second Attack timer
        if (secondSwordAttackCooldownTimer > 0f)
        {
            secondSwordAttackCooldownTimer -= Time.deltaTime;
            hitCount = 1;
        }

        if (secondSwordAttackCooldownTimer < 0f && thirdSwordAttackCooldownTimer < 0f)
        {
            hitCount = 0;
        }
        //Third Attack timer
        if (thirdSwordAttackCooldownTimer > 0f)
        {
            thirdSwordAttackCooldownTimer -= Time.deltaTime;
            hitCount = 2;
        }

        //Melee Power Attack timer
        if (meleePowerAttackCooldownTimer > 0f)
        {
            meleePowerAttackCooldownTimer -= Time.deltaTime;
        }

        //Melee Block timer
        if (meleeBlockCooldownTimer > 0f)
        {
            meleeBlockCooldownTimer -= Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        //Debug.Log(fsm.State);
        if (fsm.State == States.Respawn)
            return;

        // Landed squash
        if (onGround && !wasOnGround && Speed.y <= 0)
        {
            SpriteScale.x = 1.15f;
            SpriteScale.y = 0.85f;
        }

        // Do all the movement on the actor (base)
        // Horizontal
        var moveh = base.MoveH(Speed.x * Time.deltaTime);
        if (moveh)
        {
            Speed.x = 0;
        }

        // Vertical
        var movev = base.MoveV(Speed.y * Time.deltaTime);
        if (movev)
        {
            Speed.y = 0;
        }

        // Update the sprite
        UpdateSprite();

        // kill Box
        //if (Mathf.Abs(transform.position.x) > 192 || Mathf.Abs(transform.position.y) > 108) {
        //	Die ();
        //}

        // Get Crushed by block if we are collisioning with the solid layer
        if (CollisionSelf(box_layer))
        {
            Die();
        }
    }

    void Normal_Update()
    {
        // Ducking over here
        if (CanDuck)
        {
            fsm.ChangeState(States.Ducking, StateTransition.Overwrite);
            return;
        }

        // Dash over here
        if (CanDash)
        {
            fsm.ChangeState(States.Dash, StateTransition.Overwrite);
            return;
        }

        // Roll over here
        if (CanRoll)
        {
            fsm.ChangeState(States.Roll, StateTransition.Overwrite);
            rollAgain = false;
            return;
        }

        // Melee Attack over here
        if (CanHandAttack)
        {
            meleeAttackCooldownTimer = MeleeAttackCooldownTime;
            fsm.ChangeState(States.Attack, StateTransition.Overwrite);
            return;
        }

        // Melee PowerAttack over here
        if (CanPowerSwordAttack)
        {
            meleePowerAttackCooldownTimer = MeleePowerAttackCooldownTime;
            fsm.ChangeState(States.PowerSwordAttack, StateTransition.Overwrite);
            return;
        }

        // Melee Attack over here
        if (CanSwordAttack)
        {
            meleeAttackCooldownTimer = MeleeAttackCooldownTime;
            fsm.ChangeState(States.SwordAttack, StateTransition.Overwrite);
            return;
        }

        // Bottom Attack over here
        if (CanSecondSwordAttack)
        {
            secondSwordAttackCooldownTimer = SecondSwordAttackCooldownTime;
            fsm.ChangeState(States.SecondSwordAttack, StateTransition.Overwrite);
            return;
        }

        // Powerfull Attack over here
        if (CanThirdSwordAttack)
        {
            thirdSwordAttackCooldownTimer = ThirdSwordAttackCooldownTime;
            fsm.ChangeState(States.ThirdSwordAttack, StateTransition.Overwrite);
            return;
        }

        // Sword Block over here
        if (CanBlock)
        {
            meleeBlockCooldownTimer = MeleeBlockCooldownTime;
            fsm.ChangeState(States.SwordBlock, StateTransition.Overwrite);
            return;
        }

        // Bow Attack over here
        if (CanShoot)
        {
            rangedAttackCooldownTimer = RangedAttackCooldownTime;
            fsm.ChangeState(States.BowAttack, StateTransition.Overwrite);
            return;
        }

        // Bow Power Attack over here
        if (CanPowerShoot)
        {
            rangedPowerAttackCooldownTimer = RangedPowerAttackCooldownTime;
            fsm.ChangeState(States.BowPowerAttack, StateTransition.Overwrite);
            return;
        }

        // Cling to wall
        if (((moveX > 0 && CheckColInDir(Vector2.right * -1f, solid_layer)) || (moveX < 0 && CheckColInDir(Vector2.right, solid_layer))) && canStick && !onGround)
        {
            stickTimer = clingTime;
            sticking = true;
            canStick = false;
        }

        // Horizontal Speed Update Section
        float num = onGround ? 1f : AirMult;

        if (!sticking)
        {
            if (Mathf.Abs(Speed.x) > MaxRun && Mathf.Sign(Speed.x) == moveX)
            {
                Speed.x = Calc.Approach(Speed.x, MaxRun * moveX, RunReduce * num * Time.deltaTime);
            }
            else
            {
                Speed.x = Calc.Approach(Speed.x, MaxRun * moveX, RunAccel * num * Time.deltaTime);
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
        if (moveY != 0 && moveY != oldMoveY && CheckColAtPlace(Vector2.up * moveY * 2, ladder_layer))
        {
            var myX = (int)myCollider.bounds.center.x;
            var myExtentX = (int)myCollider.bounds.extents.x;
            var extraDistanceX = 1; // 2-4 recommended when tile size is 16 pixels, adjust accordingly
            for (int i = -myExtentX - extraDistanceX; i <= myExtentX + extraDistanceX; i++)
            {
                if (CanClimbLadder((int)myX + i, moveY))
                {
                    ClimbLadder((int)myX + i, moveY);
                    return;
                }
            }
        }
        //зацепка за уступы
        // Ledge grab
        if (!onGround && Speed.y <= 0 && moveX != 0 && moveY != -1 && CheckColAtPlace(Vector2.right * moveX * 2, box_layer))
        {
            var myY = myCollider.bounds.center.y;
            var myExtentY = 1;


            var extraDistanceY = 0; // 2-4 recommended when tile size is 16 pixels, adjust accordinly

            for (int i = 0; i < myExtentY + extraDistanceY; i++)
            {
                if (CanGrabLedge((int)myY + i, moveX))
                {
                    //Debug.Log((int)myY );
                    GrabLedge((int)myY, moveX);
                    return;
                }
            }
        }

        // Wall Slide
        if (!onGround)
        {
            float target = maxFall;
            // Wall Slide
            if ((moveX == (int)Facing) && moveY != -1)
            {
                if (Speed.y <= 0f && wallSlideTimer > 0f && CheckColInDir(Vector2.right * (int)Facing, box_layer))
                {
                    wallSlideDir = (int)Facing;
                }
                if (wallSlideDir != 0)
                {
                    target = Mathf.Lerp(MaxFall, WallSlideStartMax, wallSlideTimer / WallSlideTime);
                }
            }

            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }

        // Handle facing direction
        if (wallSlideDir == 0)
        {
            // Handle Facings
            if (moveX != 0)
            {
                Facings facings = (Facings)moveX;
                Facing = facings;
            }
        }
        else
        {
            // Dust particles
            if (Random.value < 0.35f && GameManager.instance != null)
            {
                GameManager.instance.Emit(DashDustParticle, 1, new Vector2(transform.position.x, transform.position.y) + new Vector2(wallSlideDir * 2.5f, 4), Vector2.one * 1f);
            }
        }

        // Handle variable jumping 
        if (varJumpTimer > 0f)
        {
            if (Input.GetButton("Jump"))
            {
                Speed.y = Mathf.Max(Speed.y, varJumpSpeed);
            }
            else
            {
                varJumpTimer = 0f;
            }
        }

        // Drop down from a one way platform
        if (onGround && moveY < 0f && jumpIsInsideBuffer && CheckColInDir(Vector2.down, oneway_layer) && !CheckColInDir(Vector2.down, solid_layer))
        {
            onGround = false;
            jumpGraceTimer = 0f;
            jumpIsInsideBuffer = false;
            jumpBufferTimer = 0f;
            transform.position += new Vector3(0, -1, 0);
        }

        // Jump
        if (jumpIsInsideBuffer)
        {
            if (onGround || jumpGraceTimer > 0f)
            {
                // Jump Normally
                Jump();
            }
            else
            {
                // Wall Jump Left
                if (WallJumpCheck(-1))
                {
                    WallJump(1);
                    // Wall Jump Right
                }
                else if (WallJumpCheck(1))
                {
                    WallJump(-1);
                }
            }
        }
    }

    private void Ducking_Enter()
    {
        // Use the ducking hitbox
        myDuckingCollider.enabled = true;
        myNormalCollider.enabled = false;
        myCollider = myDuckingCollider;

        SpriteScale = new Vector2(1.15f, 0.85f);
    }

    private void Ducking_Exit()
    {
        // Use the normal hitbox
        myNormalCollider.enabled = true;
        myDuckingCollider.enabled = false;
        myCollider = myNormalCollider;

        SpriteScale = new Vector2(0.85f, 1.15f);
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

        if (moveY != -1 || !onGround && !CheckColAtPlace(Vector2.up * 10, solid_layer))
        {
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            return;
        }

        // Horizontal Speed Update Section, I change grounded friction to .5 so the ducking state also works like a slide
        float num = DuckingFrictionMultiplier;

        Speed.x = 0;

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
    }

    private void LadderClimb_Enter()
    {
        jumpIsInsideBuffer = false;
        jumpBufferTimer = 0f;
        jumpGraceTimer = 0f;
    }

    private void LadderClimb_Exit()
    {
        // Add extra feel when leaving the ladder state
        SpriteScale = new Vector2(1.1f, 0.9f);
    }

    private void LadderClimb_Update()
    {
        // If jump buttom is being pressed and pressing down just fall
        if (moveY < 0 && jumpIsInsideBuffer)
        {
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            return;
            // Jump when the jump buttom is pressed
        }
        else if ((moveX != 0 && jumpIsInsideBuffer) || jumpIsInsideBuffer)
        {
            Jump();
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            return;
            // Land when touching the ground and press down
        }
        else if (onGround && moveY < 0)
        {
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            return;
        }

        // Up and Down (Vertical)
        Speed.y = Calc.Approach(Speed.y, LadderClimbSpeed * moveY, 10000 * Time.deltaTime);

        // Drop when there is no more ladder below and pressing down or there is no collision with ladder
        if ((moveY < 0 && !CanClimbLadder((int)transform.position.x, -1)) || !CanClimbLadder((int)transform.position.x, 0))
        {
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            return;
            // Jump if you've reached the top of the ladder and pressing UP
        }
        else if (moveY > 0 && !CanClimbLadder((int)transform.position.x, 1))
        {
            Jump();
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            return;
        }
    }

    private void Roll_Enter()
    {
        var health = GetComponent<Health>();
        health.invincible = true;

        // Use the ducking hitbox
        myDuckingCollider.enabled = true;
        myNormalCollider.enabled = false;
        myCollider = myDuckingCollider;

        SpriteScale = new Vector2(1.15f, 0.85f);

        rollCooldownTimer = RollCooldown;
        Speed = Vector2.zero;
        RollDir = Vector2.zero;

        Vector2 value = new Vector2(moveX, moveY);
        if (value == Vector2.zero)
        {
            value = new Vector2((int)Facing, 0f);
        }
        else if (value.x == 0 && value.y > 0 && onGround)
        {
            value = new Vector2((int)Facing, value.y);
        }
        value.Normalize();
        Vector2 vector = value * DashSpeed;
        Speed = vector;
        RollDir = value;
        if (RollDir.x != 0f)
        {
            Facing = (Facings)Mathf.Sign(RollDir.x);
        }
        if (RollDir.y < 0 && onGround)
        {
            RollDir.y = 0;
            RollDir.x = Mathf.Sign(RollDir.x);
            Speed.x *= 2f;
        }

        //// Squash & Stretch
        //if (DashDir.x != 0 && DashDir.y == 0)
        //{
        //    SpriteScale = new Vector2(1.2f, 0.8f);
        //}
        //else if (DashDir.x == 0 && DashDir.y != 0)
        //{
        //    SpriteScale = new Vector2(.8f, 1.2f);
        //}

        // Screenshake
        if (PixelCameraController.instance != null)
        {
            PixelCameraController.instance.DirectionalShake(RollDir);
        }

        if (RollDir.y >= 0f)
        {
            Speed = RollDir * EndRollSpeed;
        }
        if (Speed.y > 0f)
        {
            Speed.y = Speed.y * EndRollUpMult;

        }
    }

    private void Roll_Exit()
    {
        var health = GetComponent<Health>();
        health.invincible = false;

        if (CheckColAtPlace(Vector2.up * 10, solid_layer) && CheckColAtPlace(Vector2.left * 10, solid_layer))
        {
            Facings facings = (Facings)(1);
            Facing = facings;
            rollAgain = true;
            return;
        }

        if (CheckColAtPlace(Vector2.up * 10, solid_layer) && CheckColAtPlace(Vector2.right * 10, solid_layer))
        {
            Facings facings = (Facings)(-1);
            Facing = facings;
            rollAgain = true;
            return;
        }

        if (CheckColAtPlace(Vector2.up * 13, solid_layer))
        {
            rollAgain = true;
            return;
        }

        // Use the normal hitbox
        myNormalCollider.enabled = true;
        myDuckingCollider.enabled = false;
        myCollider = myNormalCollider;

        SpriteScale = new Vector2(0.85f, 1.15f);
    }

    private void Roll_Update()
    {
        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }

        if (Input.GetButton("Jump"))
        {
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            return;
        }

        // Dust particles
        if (Random.value < 0.85f && GameManager.instance != null)
        {
            GameManager.instance.Emit(DashDustParticle, Random.Range(1, 3), new Vector2(transform.position.x, transform.position.y) + new Vector2(0f, -5), Vector2.one * 3f);
        }

    }

    private IEnumerator Dash_Enter()
    {
        dashCooldownTimer = DashCooldown;
        Speed = Vector2.zero;
        RollDir = Vector2.zero;

        Vector2 value = new Vector2(moveX, moveY);
        if (value == Vector2.zero)
        {
            value = new Vector2((int)Facing, 0f);
        }
        else if (value.x == 0 && value.y > 0 && onGround)
        {
            value = new Vector2((int)Facing, value.y);
        }
        value.Normalize();
        Vector2 vector = value * DashSpeed;
        Speed = vector;
        RollDir = value;
        if (RollDir.x != 0f)
        {
            Facing = (Facings)Mathf.Sign(RollDir.x);
        }
        if (RollDir.y < 0 && onGround)
        {
            RollDir.y = 0;
            RollDir.x = Mathf.Sign(RollDir.x);
            Speed.y = 0f;
            Speed.x *= 2f;
        }

        //// Squash & Stretch
        //if (DashDir.x != 0 && DashDir.y == 0)
        //{
        //    SpriteScale = new Vector2(1.2f, 0.8f);
        //}
        //else if (DashDir.x == 0 && DashDir.y != 0)
        //{
        //    SpriteScale = new Vector2(.8f, 1.2f);
        //}

        // Screenshake
        if (PixelCameraController.instance != null)
        {
            PixelCameraController.instance.DirectionalShake(RollDir);
        }

        yield return new WaitForSeconds(RollTime);

        // Wait one extra frame
        yield return null;

        if (RollDir.y >= 0f)
        {
            Speed = RollDir * EndDashSpeed;
        }
        if (Speed.y > 0f)
        {
            Speed.y = Speed.y * EndDashUpMult;

        }
        fsm.ChangeState(States.Normal, StateTransition.Overwrite);
        yield break;
    }

    private void Dash_Update()
    {
        // Dust particles
        if (Random.value < 0.85f && GameManager.instance != null)
        {
            GameManager.instance.Emit(DashDustParticle, Random.Range(1, 3), new Vector2(transform.position.x, transform.position.y) + new Vector2(0f, -5), Vector2.one * 3f);
        }
    }

    void Respawn_Enter()
    {
        if (GameManager.instance != null)
        {
            respawnPosition = GameManager.instance.LevelSpawnPoint.position;
        }
        else
        {
            respawnPosition = Vector2.zero;
        }
        GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;
        animator.Play("Fall");
        moveToRespawnPosTimer = 0f;
    }

    void Respawn_Exit()
    {
        respawnTimer = 0f;
        transform.position = respawnPosition;
        GetComponentInChildren<SpriteRenderer>().sortingOrder = -1;
        SpriteScale = new Vector2(1.15f, 0.85f);

        // Trigger the Respawn Events on the gamemanager
        if (GameManager.instance != null)
        {
            GameManager.instance.PlayerRespawn();
        }

        //activeWeapon = ActiveWeapon.Hands;
        var health = GetComponent<Health>();
        health.dead = false;
        health.TakeHeal(health.maxHealth);
    }

    void Respawn_Update()
    {
        if (moveToRespawnPositionAfter > 0f)
        {
            moveToRespawnPosTimer += Time.deltaTime;

            if (moveToRespawnPosTimer < moveToRespawnPositionAfter)
            {
                return;
            }
        }


        respawnTimer += Time.deltaTime;
        var t = respawnTimer / RespawnTime;

        if (t >= 1f)
        {
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            return;
        }

        transform.position = Vector2.Lerp(transform.position, respawnPosition, t);
    }

    void Attack_Update()
    {
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
        //Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);
        if (!sticking && !CheckColAtPlace(Vector2.right * (int)Facing, solid_layer))
        {
            var MoveH = MoveHPlatform(StepUpAfterHit * Time.deltaTime);
        }
        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
    }

    void PowerSwordAttack_Update()
    {
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
        //Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);
        if (!sticking && !CheckColAtPlace(Vector2.right * (int)Facing, solid_layer))
        {
            var MoveH = MoveHPlatform(StepUpAfterHit * Time.deltaTime);
        }
        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
    }

    void SwordAttack_Update()
    {
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
        //Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);
        if (!sticking && !CheckColAtPlace(Vector2.right * (int)Facing, solid_layer))
        {
            var MoveH = MoveHPlatform(StepUpAfterHit * Time.deltaTime);
        }
        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
        secondSwordAttackCooldownTimer = SecondSwordAttackCooldownTime;
    }

    void SecondSwordAttack_Update()
    {
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
        //Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);
        if (!sticking && !CheckColAtPlace(Vector2.right * (int)Facing, solid_layer))
        {
            var MoveH = MoveHPlatform(StepUpAfterHit * Time.deltaTime);
        }
        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
        thirdSwordAttackCooldownTimer = ThirdSwordAttackCooldownTime;
    }

    void ThirdSwordAttack_Update()
    {
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
        //Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);
        if (!sticking && !CheckColAtPlace(Vector2.right * (int)Facing, solid_layer))
        {
            var MoveH = MoveHPlatform(StepUpAfterHit * Time.deltaTime);
        }
        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
    }

    void ThirdSwordAttack_Exit()
    {
        hitCount = 0;
    }

    public bool MoveHPlatform(float moveH)
    {
        this.movementCounter.x = this.movementCounter.x + moveH;
        int num = (int)Mathf.Round(this.movementCounter.x);
        if (num != 0)
        {
            this.movementCounter.x = this.movementCounter.x - (float)num;
            return this.MoveHExactPlatform(num);
        }
        return false;
    }

    public bool MoveHExactPlatform(int moveH)
    {
        int num = (int)Mathf.Sign((float)moveH);
        while (moveH != 0)
        {
            bool solid = CheckColInDir(Vector2.right * (float)num * (int)Facing, solid_layer);
            if (solid)
            {
                this.movementCounter.x = 0f;
                return true;
            }
            moveH -= num;
            // Move the platform
            transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x + (float)num * (int)Facing, transform.position.y), 0.1f);
        }
        return false;
    }

    void SwordBlock_Update()
    {
        var health = GetComponent<Health>();
        health.block = true;

        // Horizontal Speed Update Section
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (health.block == false)
        {
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
        }

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }

        //if (Input.GetButtonUp("Attack2"))
        //{
        //    fsm.ChangeState(States.Normal, StateTransition.Overwrite);
        //    health.invincible = false;
        //    return;
        //}
        // автоматический выход из анимации, через 60 кадров выход в самой анимации
    }


    //IEnumerator ActivationInvincible()
    //{
    //    yield return new WaitForSeconds(0.2f);
    //    var health = GetComponent<Health>();
    //    health.invincible = true;
    //    yield return new WaitForSeconds(0.2f);
    //    health.invincible = false;
    //}

    void BowAttack_Update()
    {
        // Horizontal Speed Update Section
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
    }

    void BowPowerAttack_Update()
    {
        // Horizontal Speed Update Section
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
    }

    void Action_Update()
    {
        // Horizontal Speed Update Section
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
    }
    void LedgeGrab_Enter()
    {
        Speed = Vector2.zero;
        if (wallSlideDir != 0)
            wallSlideDir = 0;
    }

    void LedgeGrab_Update()
    {
        // If pressing down or the other direction which is not the ledgegrab direction
        if (moveY < 0 || moveX != (int)Facing)
        {
            jumpGraceTimer = JumpGraceTime;
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            return;
        }

        if (jumpIsInsideBuffer)
        {

            // Ledge Climb
            if (moveX == (int)Facing || moveY > 0)
            {
                // Swap to the ledgeclimb state
                fsm.ChangeState(States.LedgeClimb, StateTransition.Overwrite);
                return;
            }
            else if (moveY != -1)
            {
                Jump();
            }
            else if (WallJumpCheck((int)Facing))
            { // Wall Jump
                WallJump(-(int)Facing);
            }
            // Swap back to the normal state
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            return;
        }
    }

    void LedgeClimb_Enter()
    {
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

    void LedgeClimb_Exit()
    {
        // Add the extra position so our new position matches the end position on the animation
        transform.position = new Vector2(transform.position.x + (extraPosOnClimb.x * (int)Facing), transform.position.y + extraPosOnClimb.y + 28);
        // Enable back again the collider
        myCollider.enabled = true;
        // This is done so the on land squash doesn't happen
        onGround = true;
        wasOnGround = true;

        //if (CheckColAtPlace(Vector2.up * 10, solid_layer))
        //{
        //    rollAgain = true;
        //    return;
        //}

        // Start the idle animation 
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            animator.Play("Idle");
        }

    }

    void LedgeClimb_Update()
    {
        ledgeClimbTimer += Time.deltaTime;

        // Check if the timer is equal or higher than the total climb time
        if (ledgeClimbTimer >= ledgeClimbTime)
        {
            ledgeClimbTimer = 0f;
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            return;
        }
    }

    // Jump Function
    public void Jump()
    {
        wallSlideTimer = WallSlideTime;
        jumpGraceTimer = 0f;
        jumpBufferTimer = 0f;
        varJumpTimer = VariableJumpTime;
        Speed.x = Speed.x + JumpHBoost * (float)moveX;
        Speed.y = JumpSpeed;
        varJumpSpeed = Speed.y;


        // Dust particles
        if (GameManager.instance != null)
        {
            GameManager.instance.Emit(DustParticle, 5, new Vector2(transform.position.x, transform.position.y) + new Vector2(0f, -5), Vector2.one * 3f);
        }
    }

    // Perform a wall jump in a set direction (left == -1 or right == 1)
    private void WallJump(int dir)
    {
        wallSlideTimer = WallSlideTime;
        jumpGraceTimer = 0f;
        jumpBufferTimer = 0f;
        varJumpTimer = VariableJumpTime;
        if (moveX != 0)
        {
            forceMoveX = dir;
            forceMoveXTimer = WallJumpForceTime;
        }

        Speed.x = WallJumpHSpeed * (float)dir;
        Speed.y = JumpSpeed;
        varJumpSpeed = Speed.y;
        SpriteScale = new Vector2(0.85f, 1.15f);

        // Dust particles
        if (GameManager.instance != null)
        {
            if (dir == -1)
            {
                GameManager.instance.Emit(DustParticle, 5, new Vector2(transform.position.x, transform.position.y) + new Vector2(2f, -5), Vector2.one * 3f);
            }
            else if (dir == 1)
            {
                GameManager.instance.Emit(DustParticle, 5, new Vector2(transform.position.x, transform.position.y) + new Vector2(-2f, -5), Vector2.one * 3f);
            }
        }
    }

    // Function to check for collisions with walls at a custom check distance
    private bool WallJumpCheck(int dir)
    {
        Vector2 leftcorner = Vector2.zero;
        Vector2 rightcorner = Vector2.zero;

        if (dir == -1)
        {
            leftcorner = new Vector2(myCollider.bounds.center.x - myCollider.bounds.extents.x - .5f - (float)WallJumpCheckDist, myCollider.bounds.center.y + myCollider.bounds.extents.y - 1f);
            rightcorner = new Vector2(myCollider.bounds.center.x - myCollider.bounds.extents.x, myCollider.bounds.center.y - myCollider.bounds.extents.y + 1f);
        }
        else if (dir == 1)
        {
            leftcorner = new Vector2(myCollider.bounds.center.x + myCollider.bounds.extents.x, myCollider.bounds.center.y + myCollider.bounds.extents.y - 1f);
            rightcorner = new Vector2(myCollider.bounds.center.x + myCollider.bounds.extents.x + .5f + (float)WallJumpCheckDist, myCollider.bounds.center.y - myCollider.bounds.extents.y + 1f);
        }

        return Physics2D.OverlapArea(leftcorner, rightcorner, solid_layer);
    }

    // Spring jump/bounce function
    public void SpringBounce()
    {
        varJumpTimer = SpringJumpVariableTime; // Set the amount of time people can hold the jump button to reach higher
        wallSlideTimer = WallSlideTime; // reset the wall slide timer
        jumpGraceTimer = 0f; // Avoid people from jumping in the air after performing a spring bounce/jumping off a spring
        Speed.x = 0f; // Set the horizontal speed to 0
        Speed.y = SpringJumpSpeed; // Set the Y speed to the desired one 
        varJumpSpeed = Speed.y; // Simply set the variable jumping speed (hold to jump higher)

        // Screenshake
        if (PixelCameraController.instance != null)
        {
            PixelCameraController.instance.DirectionalShake(Vector2.up, 0.1f);
        }

        // Change the sprite scale to add that extra "juice"
        SpriteScale = new Vector2(0.85f, 1.15f);
    }

    // This function checks wether or not there is a ledge to grab in the target Y position and in the target horizontal direction (right or left)
    private bool CanGrabLedge(int targetY, int direction)
    {
        // This is just in case the game manager hasn't been assigned we use a default tilesize value of 16
        var tileSize = Vector2.one * 16;
        return !CollisionAtPlace(new Vector2(transform.position.x + (direction * (tileSize.x / 2)), targetY + 20), box_layer) &&
            CollisionAtPlace(new Vector2(transform.position.x + (direction * (tileSize.x / 2)), targetY + 26), box_layer);
    }

    private void GrabLedge(int targetY, int direction)
    {
        // Set the facing direction
        Facing = (Facings)direction;
        // The collider's extents on the Y axis (half of height)
        var extentsY = myCollider.bounds.extents.y + 1;
        // Set the Y position to the desired one according to our collider's size and the targetY
        transform.position = new Vector2(transform.position.x, (float)(targetY - extentsY + 8));
        // Set the vertical speed to 0
        Speed.y = 0f;

        // We check for collisions with the ledge on the set direction
        while (!CheckColAtPlace(Vector2.right * direction, box_layer))
        {
            transform.position = new Vector2(transform.position.x + direction, transform.position.y);
        }

        // Change the sprite scale to add extra feel
        SpriteScale = new Vector2(1.15f, 0.85f);
        // Change the state to the ledge grabbing state
        fsm.ChangeState(States.LedgeGrab, StateTransition.Overwrite);
    }

    private bool CanClimbLadder(int targetX, int direction)
    {
        // This is just in case the game manager hasn't been assigned we use a default tilesize value of 16
        var tileSize = GameManager.instance != null ? GameManager.instance.TileSize : Vector2.one * 16;

        return CollisionAtPlace(new Vector2((float)targetX, transform.position.y + direction * (tileSize.y / 2)), ladder_layer) &&
            !CollisionAtPlace(new Vector2((float)targetX - ((tileSize.x / 2) + 2), transform.position.y + direction * (tileSize.y / 2)), ladder_layer) &&
            !CollisionAtPlace(new Vector2((float)targetX + ((tileSize.x / 2) + 2), transform.position.y + direction * (tileSize.y / 2)), ladder_layer) &&
            !CollisionAtPlace(new Vector2((float)targetX, transform.position.y + direction * (tileSize.y / 2)), solid_layer);

    }

    private void ClimbLadder(int targetX, int direction)
    {
        // Set the vertical speed to 0
        Speed.y = 0;

        //var colliders = checkcol

        // Set the position to the desired one
        transform.position = new Vector2((float)targetX, transform.position.y);

        // Change the sprite scale to add extra feel
        SpriteScale = new Vector2(0.9f, 1.1f);
        // Change the state to the ladder climbing state
        fsm.ChangeState(States.LadderClimb, StateTransition.Overwrite);
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

    //public void WithoutWeapons() //????
    //{
    //    MeleeAttackMinDamage[0] = HandAttackMinDamage;
    //    MeleeAttackMaxDamage[0] = HandAttackMaxDamage;
    //    MeleeAttackCooldownTime = HandAttackCooldownTime; // 0,2

    //    CriticalDamageMultiply = new int[3] { 0, 0, 0 };
    //    CriticalDamageMultiplyChance = new int[3] { 0, 0, 0 };
    //    StepUpAfterHit = new int[3] { 0, 0, 0 };
    //    hasSeries = false;
    //    hasBlock = false;
    //}

    public bool PickUpMeleeWeapon(string type, int minDamage, int maxDamage, 
    float attackCooldown, int criticalDamage, int critMultiply, int critChance, int stepUpAfterHit, 
    bool series, bool block, bool hasThirdAttackCriticalDamage, 
    bool hasPoison, bool hasPowerAttackPoison, int poisonAmount, int poisonFrequency, int poisonTick,
	bool hasFire, bool hasPowerAttackFire, int fireAmount, int fireFrequency, int fireTick,
    bool hasFreez, bool hasPowerAttackFreez, int freezDuration,  
    bool hasPush, bool hasPowerAttackPush, int pushDistance,
    int meleePowerAttackMinDamage, int meleePowerAttackMaxDamage) // TODO не доделано
    {
        if (!hasSword)
        {
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    TakeItem(item);
                }
            }

            fsm.ChangeState(States.Normal, StateTransition.Overwrite);

			MeleeWeaponType = type;
            MeleeAttackMinDamage = minDamage;
            MeleeAttackMaxDamage = maxDamage;
            MeleeCriticalDamage = criticalDamage;
            MeleeAttackCooldownTime = attackCooldown;
            MeleeCriticalDamageMultiply = critMultiply;
            MeleeCriticalDamageChance = critChance;
            StepUpAfterHit = stepUpAfterHit;
            MeleeAttackCanThirdAttackCriticalDamage = hasThirdAttackCriticalDamage;
            hasSeries = series;
            hasBlock = block;
			
			MeleeAttackCanPoison = hasPoison;
		    MeleePoisonDamaged = poisonAmount;
		    MeleePoisonFrequency = poisonFrequency;
		    MeleePoisonTick = poisonTick;
		    MeleeAttackCanFire = hasFire;
		    MeleeFireDamaged = fireAmount;
		    MeleeFireFrequency = fireFrequency;
		    MeleeFireTick = fireTick;
		    MeleeAttackCanPush = hasPush;
            MeleePushDistance = pushDistance;
            MeleeAttackCanFreez = hasFreez;
            MeleeFreezDuration = freezDuration;

            activeWeapon = ActiveWeapon.Sword;
            hasSword = true;
            return false;
        }
        return true;
    }
    public bool PickUpRangedWeapon(string type, int minDamage, int maxDamage, 
	bool hasPoison, int poisonAmount, int poisonFrequency, int poisonTick,
	bool hasFire, int fireAmount, int fireFrequency, int fireTick,
	bool hasPush, bool hasThroughShoot)
    {
        if (!hasBow)
        {
            if (parameters != null)
            { 
                foreach (var item in parameters)
                {
                    TakeItem(item);
                }
            }
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);

			RangedWeaponType = type;
            RangedAttackMinDamage = minDamage;
            RangedAttackMaxDamage = maxDamage;
            //RangedAttackCooldownTime = attackCooldown;
            //RangedCriticalDamageMultiply = critMultiply;
            //RangedCriticalDamageMultiplyChance = critChance;
			
			RangedAttackCanPoison = hasPoison;
		    RangedPoisonDamaged = poisonAmount;
		    RangedPoisonFrequency = poisonFrequency;
		    RangedPoisonTick = poisonTick;
		    RangedAttackCanFire = hasFire;
		    RangedFireDamaged = fireAmount;
		    RangedFireFrequency = fireFrequency;
		    RangedFireTick = fireTick;
		    RangedAttackCanPush = hasPush;
            RangedAttackCanThroughShoot = hasThroughShoot;

            activeWeapon = ActiveWeapon.Bow;
            hasBow = true;
            return false;
        }
        return true;
    }

    public void DropMeleeWeapon()
    {
        foreach (var item in parameters) // перебор всех предметов и вычитание хар-тик
        {
            DropItem(item);
        }

        foreach (var item in parameters) // перебор всех предметов и прибавление хар-тик
        {
            TakeItem(item);
        }

        if (hasBow)
        {
            activeWeapon = ActiveWeapon.Bow;
        }
        else activeWeapon = ActiveWeapon.Hands;
        hasSword = false;
    }

    public void DropRangedWeapon()
    {
        foreach (var item in parameters) // перебор всех предметов и вычитание хар-тик
        {
            DropItem(item);
        }

        foreach (var item in parameters) // перебор всех предметов и прибавление хар-тик
        {
            TakeItem(item);
        }

        if (hasSword)
        {
            activeWeapon = ActiveWeapon.Sword;
        }
        else activeWeapon = ActiveWeapon.Hands;
        hasBow = false;
    }
	
	public bool TakeItem(ItemParameters item) // TODO переделать логику
    {
	    Gravity = Gravity + item.Gravity;
        MaxFall = MaxFall + item.MaxFall;
        FastFall = FastFall + item.FastFall;
        curRun = MaxRun + item.MaxRun;
        MaxRun = MaxRun + item.MaxRun;
        RunAccel = RunAccel + item.RunAccel;
        RunReduce = RunReduce + item.RunReduce;
        AirMult = AirMult + item.AirMult;
        JumpSpeed = JumpSpeed + item.JumpSpeed;
        JumpHBoost = JumpHBoost + item.JumpHBoost;
	    VariableJumpTime = VariableJumpTime + item.VariableJumpTime;
	    SpringJumpSpeed = SpringJumpSpeed + item.SpringJumpSpeed;
	    SpringJumpVariableTime = SpringJumpVariableTime + item.SpringJumpVariableTime;
	    WallJumpForceTime = WallJumpForceTime + item.WallJumpForceTime;
	    WallJumpHSpeed = WallJumpHSpeed + item.WallJumpHSpeed;
	    WallJumpCheckDist = WallJumpCheckDist + item.WallJumpCheckDist;
	    WallSlideStartMax = WallSlideStartMax + item.WallSlideStartMax;
	    WallSlideTime = WallSlideTime + item.WallSlideTime;
	    DashSpeed = DashSpeed + item.DashSpeed;
	    EndDashSpeed = EndDashSpeed + item.EndDashSpeed;
	    EndRollSpeed = EndRollSpeed + item.EndRollSpeed;
	    EndDashUpMult = EndDashUpMult + item.EndDashUpMult;
	    EndRollUpMult = EndRollUpMult + item.EndRollUpMult;
	    DashTime = DashTime + item.DashTime;
	    RollTime = RollTime + item.RollTime;
	    DashCooldown = DashCooldown + item.DashCooldown;
	    RollCooldown = RollCooldown + item.RollCooldown;
	    clingTime = RollCooldown + item.clingTime;
	    JumpGraceTime = JumpGraceTime + item.JumpGraceTime;
	    JumpBufferTime = JumpBufferTime + item.JumpBufferTime;
	    LadderClimbSpeed = LadderClimbSpeed + item.LadderClimbSpeed;

        MeleeCriticalDamage = MeleeCriticalDamage + item.MeleeCriticalDamage;
        MeleeAttackMaxDamage = MeleeAttackMaxDamage+ item.MeleeAttackMaxDamage;
        MeleeAttackMinDamage = MeleeAttackMinDamage+ item.MeleeAttackMinDamage;
        MeleeCriticalDamageMultiply = MeleeCriticalDamageMultiply + item.MeleeCriticalDamageMultiply;
        MeleeCriticalDamageChance = MeleeCriticalDamageChance + item.MeleeCriticalDamageChance;
        StepUpAfterHit = StepUpAfterHit+ item.StepUpAfterHit;
        MeleeAttackCooldownTime = MeleeAttackCooldownTime+ item.MeleeAttackCooldownTime;
        MeleePowerAttackMaxDamage = MeleePowerAttackMaxDamage+ item.MeleePowerAttackMaxDamage;
        MeleePowerAttackMinDamage = MeleePowerAttackMinDamage+ item.MeleePowerAttackMinDamage;

        //MeleeAttackCanPowerAttackCriticalDamage = item.MeleeAttackCanPowerAttackCriticalDamage;
        MeleeAttackCanThirdAttackCriticalDamage = item.MeleeAttackCanThirdAttackCriticalDamage;
        MeleeAttackCanPoison = item.MeleeAttackCanPoison;
        MeleePoisonDamaged = MeleePoisonDamaged+ item.MeleePoisonDamaged;
        MeleePoisonFrequency = MeleePoisonFrequency+ item.MeleePoisonFrequency;
        MeleePoisonTick = MeleePoisonTick+ item.MeleePoisonTick;
        MeleeAttackCanFire = item.MeleeAttackCanFire;
        MeleeFireDamaged = MeleeFireDamaged+ item.MeleeFireDamaged;
        MeleeFireFrequency = MeleeFireFrequency+ item.MeleeFireFrequency;
        MeleeFireTick = MeleeFireTick+ item.MeleeFireTick;
        MeleeAttackCanPush = item.MeleeAttackCanPush;
        MeleeAttackCanFreez = item.MeleeAttackCanFreez;
        MeleeFreezDuration = MeleeFreezDuration+ item.MeleeFreezDuration;
        MeleePushDistance = MeleePushDistance+ item.MeleePushDistance;
        MeleeBlockCooldownTime = MeleeBlockCooldownTime+ item.MeleeBlockCooldownTime;

        RangedAttackMinDamage = RangedAttackMinDamage+ item.RangedAttackMinDamage;
        RangedAttackMaxDamage = RangedAttackMaxDamage+ item.RangedAttackMaxDamage;
        RangedAttackCooldownTime = RangedAttackCooldownTime+ item.RangedAttackCooldownTime;
        RangedPowerAttackCooldownTime = RangedPowerAttackCooldownTime+ item.RangedPowerAttackCooldownTime;
        RangedCriticalDamageMultiply = RangedCriticalDamageMultiply + item.RangedCriticalDamageMultiply;
        RangedCriticalDamageChance = RangedCriticalDamageChance + item.RangedCriticalDamageChance;
        RangedCriticalDamage = RangedCriticalDamage + item.RangedCriticalDamage;

        RangedAttackCanPoison = item.RangedAttackCanPoison;
        RangedPoisonDamaged = RangedPoisonDamaged+ item.RangedPoisonDamaged;
        RangedPoisonFrequency = RangedPoisonFrequency+ item.RangedPoisonFrequency;
        RangedPoisonTick = RangedPoisonTick+ item.RangedPoisonTick;
        RangedAttackCanFire = item.RangedAttackCanFire;
        RangedFireDamaged = RangedFireDamaged+ item.RangedFireDamaged;
        RangedFireFrequency = RangedFireFrequency+ item.RangedFireFrequency;
        RangedFireTick = RangedFireTick+ item.RangedFireTick;
        RangedAttackCanPush = item.RangedAttackCanPush;
        RangedAttackCanThroughShoot = item.RangedAttackCanThroughShoot;

		return true;
    }

        public void DropItem(ItemParameters item) // TODO переделать логику
    {
        Gravity = Gravity - item.Gravity;
        MaxFall = MaxFall - item.MaxFall;
        FastFall = FastFall - item.FastFall;
        curRun = MaxRun - item.MaxRun;
        MaxRun = MaxRun - item.MaxRun;
        RunAccel = RunAccel - item.RunAccel;
        RunReduce = RunReduce - item.RunReduce;
        AirMult = AirMult - item.AirMult;
        JumpSpeed = JumpSpeed - item.JumpSpeed;
        JumpHBoost = JumpHBoost - item.JumpHBoost;
        VariableJumpTime = VariableJumpTime - item.VariableJumpTime;
        SpringJumpSpeed = SpringJumpSpeed - item.SpringJumpSpeed;
        SpringJumpVariableTime = SpringJumpVariableTime - item.SpringJumpVariableTime;
        WallJumpForceTime = WallJumpForceTime - item.WallJumpForceTime;
        WallJumpHSpeed = WallJumpHSpeed - item.WallJumpHSpeed;
        WallJumpCheckDist = WallJumpCheckDist - item.WallJumpCheckDist;
        WallSlideStartMax = WallSlideStartMax - item.WallSlideStartMax;
        WallSlideTime = WallSlideTime - item.WallSlideTime;
        DashSpeed = DashSpeed - item.DashSpeed;
        EndDashSpeed = EndDashSpeed - item.EndDashSpeed;
        EndRollSpeed = EndRollSpeed - item.EndRollSpeed;
        EndDashUpMult = EndDashUpMult - item.EndDashUpMult;
        EndRollUpMult = EndRollUpMult - item.EndRollUpMult;
        DashTime = DashTime - item.DashTime;
        RollTime = RollTime - item.RollTime;
        DashCooldown = DashCooldown - item.DashCooldown;
        RollCooldown = - item.RollCooldown;
        clingTime = RollCooldown - item.clingTime;
        JumpGraceTime = JumpGraceTime - item.JumpGraceTime;
        JumpBufferTime = JumpBufferTime - item.JumpBufferTime;
        LadderClimbSpeed = LadderClimbSpeed - item.LadderClimbSpeed;

        MeleeCriticalDamage = MeleeCriticalDamage - item.MeleeCriticalDamage;
        MeleeAttackMaxDamage = MeleeAttackMaxDamage - item.MeleeAttackMaxDamage;
        MeleeAttackMinDamage = MeleeAttackMinDamage - item.MeleeAttackMinDamage;
        MeleeCriticalDamageMultiply = MeleeCriticalDamageMultiply - item.MeleeCriticalDamageMultiply;
        MeleeCriticalDamageChance = MeleeCriticalDamageChance - item.MeleeCriticalDamageChance;
        StepUpAfterHit = StepUpAfterHit - item.StepUpAfterHit;
        MeleeAttackCooldownTime = MeleeAttackCooldownTime - item.MeleeAttackCooldownTime;
        MeleePowerAttackMaxDamage = MeleePowerAttackMaxDamage - item.MeleePowerAttackMaxDamage;
        MeleePowerAttackMinDamage = MeleePowerAttackMinDamage - item.MeleePowerAttackMinDamage;

        //MeleeAttackCanPowerAttackCriticalDamage = false;
        MeleeAttackCanThirdAttackCriticalDamage = false;
        MeleeAttackCanPoison = false;
        MeleePoisonDamaged = MeleePoisonDamaged - item.MeleePoisonDamaged;
        MeleePoisonFrequency = MeleePoisonFrequency - item.MeleePoisonFrequency;
        MeleePoisonTick = MeleePoisonTick - item.MeleePoisonTick;
        MeleeAttackCanFire = false;
        MeleeFireDamaged = MeleeFireDamaged - item.MeleeFireDamaged;
        MeleeFireFrequency = MeleeFireFrequency - item.MeleeFireFrequency;
        MeleeFireTick = MeleeFireTick - item.MeleeFireTick;
        MeleeAttackCanPush = false;
        MeleeAttackCanFreez = false;
        MeleeFreezDuration = MeleeFreezDuration - item.MeleeFreezDuration;
        MeleePushDistance = MeleePushDistance - item.MeleePushDistance;
        MeleeBlockCooldownTime = MeleeBlockCooldownTime - item.MeleeBlockCooldownTime;

        RangedAttackMinDamage = RangedAttackMinDamage - item.RangedAttackMinDamage;
        RangedAttackMaxDamage = RangedAttackMaxDamage - item.RangedAttackMaxDamage;
        RangedAttackCooldownTime = RangedAttackCooldownTime - item.RangedAttackCooldownTime;
        RangedPowerAttackCooldownTime = RangedPowerAttackCooldownTime - item.RangedPowerAttackCooldownTime;
        RangedCriticalDamageMultiply = RangedCriticalDamageMultiply - item.RangedCriticalDamageMultiply;
        RangedCriticalDamageChance = RangedCriticalDamageChance - item.RangedCriticalDamageChance;
        RangedCriticalDamage = RangedCriticalDamage - item.RangedCriticalDamage;

        RangedAttackCanPoison = false;
        RangedPoisonDamaged = RangedPoisonDamaged - item.RangedPoisonDamaged;
        RangedPoisonFrequency = RangedPoisonFrequency - item.RangedPoisonFrequency;
        RangedPoisonTick = RangedPoisonTick - item.RangedPoisonTick;
        RangedAttackCanFire = false;
        RangedFireDamaged = RangedFireDamaged - item.RangedFireDamaged;
        RangedFireFrequency = RangedFireFrequency - item.RangedFireFrequency;
        RangedFireTick = RangedFireTick - item.RangedFireTick;
        RangedAttackCanPush = false;
        RangedAttackCanThroughShoot = false;

    }

    // Function to update the sprite scale, facing direction and animations 
    void UpdateSprite()
    {
        // Approch the normal sprite scale at a set rate
        if (fsm.State != States.Dash)
        {
            SpriteScale.x = Calc.Approach(SpriteScale.x, 1f, 0.04f);
            SpriteScale.y = Calc.Approach(SpriteScale.y, 1f, 0.04f);
        }

        // Set the SpriteHolder scale to the target scale
        var targetSpriteHolderScale = new Vector3(SpriteScale.x, SpriteScale.y, 1f);
        if (SpriteHolder.localScale != targetSpriteHolderScale)
        {
            SpriteHolder.localScale = targetSpriteHolderScale;
        }

        // Set the x scale to the current facing direction
        var targetLocalScale = new Vector3((int)Facing, transform.localScale.y, transform.localScale.z);
        if (transform.localScale != targetLocalScale)
        {
            transform.localScale = new Vector3((int)Facing, transform.localScale.y, transform.localScale.z);
        }

        if (activeWeapon == ActiveWeapon.Hands)
        {

            if (fsm.State == States.LedgeClimb)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("LedgeClimb"))
                {
                    animator.Play("LedgeClimb");
                }
                // If on the ledge grab state
            }
            else if (fsm.State == States.LedgeGrab)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("LedgeHang"))
                {
                    animator.Play("LedgeHang");
                }
                // If on the Ladder Climbing state
            }
            else if (fsm.State == States.Action)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Action"))
                {
                    animator.Play("Action");
                }
                // If on the Ladder Climbing state
            }
            else if (fsm.State == States.LadderClimb)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("LadderClimb"))
                {
                    animator.Play("LadderClimb");
                }

                //animator.speed = Mathf.Abs(moveY);
                animator.SetFloat("LadderSpeed", moveY);
                // If on the attack state
            }
            else if (fsm.State == States.BowAttack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
                {
                    animator.Play("Shoot");
                }
                // If on the attack state
            }
            else if (fsm.State == States.Attack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    animator.Play("Attack");
                }
                // If on the dash state
            }
            else if (fsm.State == States.Dash)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Dash"))
                {
                    animator.Play("Dash");
                }
                // If on the ground
            }
            else if (fsm.State == States.Roll)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
                {
                    animator.Play("Roll");
                }
                // If on the ground
            }
            else if (onGround)
            {
                if (fsm.State == States.Ducking)
                {
                    // Idle Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Ducking"))
                    {
                        animator.Play("Ducking");
                    }
                    // If the is nohorizontal movement input
                }
                else if (moveX == 0)
                {
                    // Idle Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                    {
                        animator.Play("Idle");
                    }
                    // If there is horizontal movement input
                }
                else if (pushing)
                {
                    // Push Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Push"))
                    {
                        animator.Play("Push");
                    }
                }
                else
                {
                    // Run Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                    {
                        animator.Play("Run");
                    }
                }
                // If not on the ground
            }
            else
            {
                // If the wall slide direction is not 0 (0 equals not sliding)
                if (wallSlideDir != 0)
                {
                    // Wall Slide Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("WallSlide"))
                    {
                        animator.Play("WallSlide");
                    }
                    // If not sliding and speed is upward
                }
                else if (Speed.y > 0)
                {
                    // Jump Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
                    {
                        animator.Play("Jump");
                    }
                    // if speed is not upwards then it is downward
                }
                else if (Speed.y <= 0)
                {
                    // Fall Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
                    {
                        animator.Play("Fall");
                    }
                }
            }
        }

        if (activeWeapon == ActiveWeapon.Sword)
        {

            if (fsm.State == States.LedgeClimb)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "LedgeClimb"))
                {
                    animator.Play((string)MeleeWeaponType + "LedgeClimb");
                }
                // If on the ledge grab state
            }
            else if (fsm.State == States.LedgeGrab)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "LedgeHang"))
                {
                    animator.Play((string)MeleeWeaponType + "LedgeHang");
                }
                // If on the Ladder Climbing state
            }
            else if (fsm.State == States.Action)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "Action"))
                {
                    animator.Play((string)MeleeWeaponType + "Action");
                }
                // If on the Ladder Climbing state
            }
            else if (fsm.State == States.LadderClimb)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "LadderClimb"))
                {
                    animator.Play((string)MeleeWeaponType + "LadderClimb");
                }

                //animator.speed = Mathf.Abs(moveY);
                animator.SetFloat("LadderSpeed", moveY);

                // If on the attack state
            }
            else if (fsm.State == States.BowAttack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "Shoot"))
                {
                    animator.Play((string)MeleeWeaponType + "Shoot");
                }
                // If on the attack state
            }
            else if (fsm.State == States.SwordAttack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "Attack"))
                {
                    //animator.SetFloat("AttackSpeed", MeleeAttackSpeed);
                    animator.Play((string)MeleeWeaponType + "Attack");
                }

                // If on the dash state
            }
            else if (fsm.State == States.SecondSwordAttack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "SecondAttack"))
                {
                    animator.Play((string)MeleeWeaponType + "SecondAttack");
                }
                // If on the dash state
            }
            else if (fsm.State == States.ThirdSwordAttack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "ThirdAttack"))
                {
                    animator.Play((string)MeleeWeaponType + "ThirdAttack");
                }
                // If on the dash state
            }
            else if (fsm.State == States.PowerSwordAttack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "PowerAttack"))
                {
                    animator.Play((string)MeleeWeaponType + "PowerAttack");
                }
                // If on the dash state
            }
            else if (fsm.State == States.SwordBlock)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "Block"))
                {
                    animator.Play((string)MeleeWeaponType + "Block");
                }
                // If on the dash state
            }
            else if (fsm.State == States.Dash)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "Dash"))
                {
                    animator.Play((string)MeleeWeaponType + "Dash");
                }
                // If on the ground
            }
            else if (fsm.State == States.Roll)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "Roll"))
                {
                    animator.Play((string)MeleeWeaponType + "Roll");
                }
                // If on the ground
            }
            else if (onGround)
            {
                if (fsm.State == States.Ducking)
                {
                    // Idle Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "Ducking"))
                    {
                        animator.Play((string)MeleeWeaponType + "Ducking");
                    }
                    // If the is nohorizontal movement input
                }
                else if (moveX == 0)
                {
                    // Idle Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "Idle"))
                    {
                        animator.Play((string)MeleeWeaponType + "Idle");
                    }
                    // If there is horizontal movement input
                }
                else if (pushing)
                {
                    // Push Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "Push"))
                    {
                        animator.Play((string)MeleeWeaponType + "Push");
                    }
                }
                else
                {
                    // Run Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "Run"))
                    {
                        animator.Play((string)MeleeWeaponType + "Run");
                    }
                }
                // If not on the ground
            }
            else
            {
                // If the wall slide direction is not 0 (0 equals not sliding)
                if (wallSlideDir != 0)
                {
                    // Wall Slide Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "WallSlide"))
                    {
                        animator.Play((string)MeleeWeaponType + "WallSlide");
                    }
                    // If not sliding and speed is upward
                }
                else if (Speed.y > 0)
                {
                    // Jump Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "Jump"))
                    {
                        animator.Play((string)MeleeWeaponType + "Jump");
                    }
                    // if speed is not upwards then it is downward
                }
                else if (Speed.y <= 0)
                {
                    // Fall Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "Fall"))
                    {
                        animator.Play((string)MeleeWeaponType + "Fall");
                    }
                }
            }
        }

        if (activeWeapon == ActiveWeapon.Bow)
        {

            if (fsm.State == States.LedgeClimb)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowLedgeClimb"))
                {
                    animator.Play("BowLedgeClimb");
                }
                // If on the ledge grab state
            }
            else if (fsm.State == States.LedgeGrab)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowLedgeHang"))
                {
                    animator.Play("BowLedgeHang");
                }
                // If on the Ladder Climbing state
            }
            else if (fsm.State == States.Action)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowAction"))
                {
                    animator.Play("BowAction");
                }
                // If on the Ladder Climbing state
            }
            else if (fsm.State == States.LadderClimb)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowLadderClimb"))
                {
                    animator.Play("BowLadderClimb");
                }

                //animator.speed = Mathf.Abs(moveY);
                animator.SetFloat("LadderSpeed", moveY);

                // If on the attack state
            }
            else if (fsm.State == States.BowAttack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowShoot"))
                {
                    animator.Play("BowShoot");
                }
                // If on the attack state
            }

            else if (fsm.State == States.BowPowerAttack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowPowerShoot"))
                {
                    animator.Play("BowPowerShoot");
                }
                // If on the attack state
            }

            else if (fsm.State == States.Attack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowAttack"))
                {
                    animator.Play("BowAttack");
                }
                // If on the dash state
            }
            else if (fsm.State == States.Dash)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowDash"))
                {
                    animator.Play("BowDash");
                }
                // If on the ground
            }
            else if (fsm.State == States.Roll)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowRoll"))
                {
                    animator.Play("BowRoll");
                }
                // If on the ground
            }
            else if (onGround)
            {
                if (fsm.State == States.Ducking)
                {
                    // Idle Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowDucking"))
                    {
                        animator.Play("BowDucking");
                    }
                    // If the is nohorizontal movement input
                }
                else if (moveX == 0)
                {
                    // Idle Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowIdle"))
                    {
                        animator.Play("BowIdle");
                    }
                    // If there is horizontal movement input
                }
                else if (pushing)
                {
                    // Push Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowPush"))
                    {
                        animator.Play("BowPush");
                    }
                }
                else
                {
                    // Run Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowRun"))
                    {
                        animator.Play("BowRun");
                    }
                }
                // If not on the ground
            }
            else
            {
                // If the wall slide direction is not 0 (0 equals not sliding)
                if (wallSlideDir != 0)
                {
                    // Wall Slide Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowWallSlide"))
                    {
                        animator.Play("BowWallSlide");
                    }
                    // If not sliding and speed is upward
                }
                else if (Speed.y > 0)
                {
                    // Jump Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowJump"))
                    {
                        animator.Play("BowJump");
                    }
                    // if speed is not upwards then it is downward
                }
                else if (Speed.y <= 0)
                {
                    // Fall Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowFall"))
                    {
                        animator.Play("BowFall");
                    }
                }
            }
        }
    }

    // Die Function
    public void Die()
    {
        // Set the speed to 0
        Speed = Vector2.zero;
        // Trigget the respawn state
        fsm.ChangeState(States.Respawn, StateTransition.Overwrite);
        // Trigger the Dead Events on the gamemanager
        if (GameManager.instance != null)
        {
            GameManager.instance.PlayerDead();
        }
    }
    public void Action()
    {
        fsm.ChangeState(States.Action, StateTransition.Overwrite);
    }
	
    public void DamageBlocked()
    {
        GameManager.instance.Emit(SparkParticle, Random.Range(5, 8), new Vector2(transform.position.x, transform.position.y + 10) + new Vector2(wallSlideDir * 2.5f, 4), Vector2.one * 1f);
    }

		public void Pushed()
    {
		transform.position = new Vector2(transform.position.x + Speed.x * 2, transform.position.y);	
    }
	
    void OnTriggerEnter2D(Collider2D col)
    {
        var component = col.GetComponent<Health>();
        var worm = col.GetComponent<WormEnemy>();
        // If the target the hitbox collided with has a health component and it is not our owner and it is not on the already on the list of healths damaged by the current hitbox
        if (component != null && component != owner && !healthsDamaged.Contains(component) && worm != null && fsm.State == States.Roll)
        {
            // Try to Apply the damage
            var didDamage = component.TakeDamage(3, false, 0, 0, 0, false, 0, 0, 0, false, 0, false, 0);

            if (didDamage)
            {
                // Add the health component to the list of damaged healths
                healthsDamaged.Add(component);
            }
        }
    }
}