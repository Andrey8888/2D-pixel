#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using UnityEngine.UI;

public class Enemy : Actor
{
    public enum Type
    {
        MeleeSceleton,
        ArcherySceleton,
        MagicSceleton,
        Snake,
		SandDemon,
		Bat
    }

    public Type TypeEnemy = Type.MeleeSceleton;

    [Header("Movement Variables")]
    // Gravity, Maximun fall speed & fastfall Speed
    public float Gravity = 900f;                // Speed at which you are pushed down when you are on the air
    public float MaxFall = -160f;               // Maximun common speed at which you can fall
    public float RunReduce = 400f;              // Horizontal Acceleration when you're already when your horizontal speed is higher or equal to the maximun
    public float WalkSpeed = 30f;
    private float walkSpeed = 0;
    public float RunAccel = 1000f;              // Horizontal Acceleration Speed
    public float AirMult = 0.65f;               // Multiplier for the air horizontal movement (friction) the higher the more air control you'll have
    public float TurnCooldownTime = 1f;
    private float AggressionCooldownTime = 10f;
	private float BlinkCooldownTime = 10f;
    private float ExpectationCooldownTime = 2f;
    private float turnCooldownTimer = 0f;
    private float aggressionCooldownTimer = 0f;
	private float blinkCooldownTimer = 0f;
    private float expectationCooldownTimer = 0f;
    //public float StunedCooldownTime = 0.25f;
    private float stunedCooldownTimer = 0f;
    public bool OnStun = false;
    public bool OnAggression = false;
	public bool OnBlink = false;
	
    // Helper private Variables
    private int moveX;                          // Variable to store the horizontal Input each frame

    [Header("Facing Direction")]
    public Facings Facing;  // Facing Direction

    [Header("Squash & Stretch")]
    public Transform SpriteHolder;              // Reference to the transform of the child object which holds the sprite renderer of the player
    public Vector2 SpriteScale = Vector2.one;   // The current X and Y scale of the sprite holder (used for Squash & Stretch)

    [Header("Animator")]
    public Animator animator;                   // Reference to the animator

    [Header("Particles")]
    public GameObject BloodParticle;


    [Header("MeleeAttacks")]
    public int MeleeCriticalDamage = 0;
    public float MeleeAttackCooldownTime = 2f; // Задержка по ближней атаке  
    public int MeleeAttackMaxDamage;
    public int MeleeAttackMinDamage;
    public int MeleeCriticalDamageMultiply;
    public int MeleeCriticalDamageChance;

    [Header("Poison")]
    public bool MeleeAttackCanPoison = false;
    public int MeleePoisonDamaged = 0;
    public int MeleePoisonFrequency = 0;
    public int MeleePoisonTick = 0;
    public int MeleePoisonChance = 100;
    [Header("Fire")]
    public bool MeleeAttackCanFire = false;
    public int MeleeFireDamaged = 0;
    public int MeleeFireFrequency = 0;
    public int MeleeFireTick = 0;
    public int MeleeFireChance = 100;
    [Header("Freez")]
    public bool MeleeAttackCanFreez = false;
    public int MeleeFreezDuration = 0;
    public int MeleeFreezChance = 100;
    [Header("Push")]
    public bool MeleeAttackCanPush = false;
    public int MeleePushDistance = 0;

    [Header("RangedAttacks")]
    public float BowAttackCooldownTime = 4f;     // Задержка по выстрелу     
    public float CastAttackCooldownTime = 2f;     // Задержка по выстрелу  
    public int RangedCriticalDamage = 0;
    public int RangedAttackMinDamage = 1;
    public int RangedAttackMaxDamage = 1;
    public int RangedCriticalDamageMultiply;
    public int RangedCriticalDamageChance;
    public Transform GunBarrel;                  // Позиция точки выстрела

    [Header("Poison")]
    public bool RangedAttackCanPoison = false;
    public int RangedPoisonDamaged = 0;
    public int RangedPoisonFrequency = 0;
    public int RangedPoisonTick = 0;
    public int RangedPoisonChance = 100;
    [Header("Fire")]
    public bool RangedAttackCanFire = false;
    public int RangedFireDamaged = 0;
    public int RangedFireFrequency = 0;
    public int RangedFireTick = 0;
    public int RangedFireChance = 100;
    [Header("Freez")]
    public bool RangedAttackCanFreez = false;
    public int RangedFreezDuration = 0;
    public int RangedFreezChance = 100;
    [Header("Push")]
    public bool RangedAttackCanPush = false;
    public int RangedPushDistance = 0;


    private bool InVisibilityZone = false;       // Проверка на нахождение в зоне видимости
    private bool InDetectionZone = false;           // Проверка на нахождение в зоне обнаружения   
    private float meleeAttackCooldownTimer = 0f; // Таймер до ближней атаки
    private float bowAttackCooldownTimer = 0f;   // Таймер до выстрела
    private float castAttackCooldownTimer = 0f;   // Таймер до выстрела
    private Player Player;



    //private Vector3 PlayerPos;                  // Позиция игрока, необходима для расчета угла стрельбы
    //public Vector3 PlayerPosGlobal;             // Позиция игрока на карте

    [Header("HP Bar")]
    private GameObject MyGUI;                    // UI на котором будем отображать  
    public Slider EnemyHP;                  // полоска здоровья врага на экране
    public Transform metka;
    private Slider ShowHP;
    // States for the state machine
    public enum States
    {
        Normal,
        BowAttack,
        Attack,
        Hit,
        Stun,
        Death,
        Cast,
		Blink
    }

    public enum Attack
    {
        Melee,                         // Ближний бой
        Archery,                       // Дальний бой
        Magic
    }
    public Attack AttackType = Attack.Melee;

    public enum Behaivour
    {
        Following,                          // Преследование
        Patroling,                          // Патрулирование
        FreeWalking,                        // патрулирование без преследования
        Idle,                                // Покой
        Aggression
    }
    public Behaivour BehaivourType = Behaivour.Idle;

    public GameObject DetectionZone;
    public GameObject VisibilityZone;

    private EnemyVisibility EnemyVisibility;// Скрипт видимости
    private EnemyDetection EnemyDetection;  // Скрипт обнаружения
	private EnemyAttacking EnemyAttacking;// Скрипт зоны для атаки
    public float PatrolTimer = 0;

    public bool CanShoot
    {
        get
        {
            return AttackType == Attack.Archery && EnemyVisibility.InVisibilityZone && bowAttackCooldownTimer <= 0f;
        }
    }

    public bool CanCast
    {
        get
        {
            return AttackType == Attack.Magic && EnemyVisibility.InVisibilityZone && castAttackCooldownTimer <= 0f;
        }
    }

    public bool CanAttack
    {
        get
        {
            return AttackType == Attack.Melee && EnemyVisibility.InVisibilityZone &&
            ((CheckColAtPlace(Vector2.right * 18, player_layer)) || CheckColAtPlace(Vector2.left * 18, player_layer)) &&
             meleeAttackCooldownTimer <= 0f;
        }
    }

    public bool CanIdle
    {
        get
        {
            return ((AttackType == Attack.Melee && (CheckColAtPlace(Vector2.right * 18, player_layer) || CheckColAtPlace(Vector2.left * 18, player_layer)) || CheckColInDir(new Vector2(moveX, 0), player_layer))
                && meleeAttackCooldownTimer >= 0f) || ((AttackType == Attack.Archery || AttackType == Attack.Magic) && EnemyVisibility.InVisibilityZone);
        }
    }

    public bool CanAggression
    {
        get
        {
            return !(TypeEnemy == Type.Snake) && OnAggression; 
        }
    }

    public bool CanStuned
    {
        get
        {
            return OnStun && stunedCooldownTimer <= 0f;
        }
    }

	public bool CanBlink
    {
        get
        {
            return (TypeEnemy == Type.SandDemon) && OnBlink && blinkCooldownTimer <= 0f; 
        }
    }
	
    // State Machine
    public StateMachine<States> fsm;

    new void Awake()
    {
        base.Awake();
        fsm = StateMachine<States>.Initialize(this);
        moveX = 1;
    }

    // Use this for initialization
    void Start()
    {
        fsm.ChangeState(States.Normal);
        MyGUI = GameObject.Find("Canvas");
        SetHPBar();
        walkSpeed = WalkSpeed;
        Player = FindObjectOfType<Player>();
    }

    new void Update()
    {
        base.Update();
        // Update all collisions here
        wasOnGround = onGround;
        onGround = OnGround();

        //Debug.Log(BehaivourType.ToString());

        EnemyVisibility = VisibilityZone.GetComponentInChildren<EnemyVisibility>();
        InVisibilityZone = EnemyVisibility.InVisibilityZone;

        EnemyDetection = DetectionZone.GetComponentInChildren<EnemyDetection>();
        InDetectionZone = EnemyDetection.InDetectionZone;

        //Bow Attack timer
        if (bowAttackCooldownTimer > 0f)
        {
            bowAttackCooldownTimer -= Time.deltaTime;
        }

        //Bow Attack timer
        if (castAttackCooldownTimer > 0f)
        {
            castAttackCooldownTimer -= Time.deltaTime;
        }

        //Melee Attack timer
        if (meleeAttackCooldownTimer > 0f)
        {
            meleeAttackCooldownTimer -= Time.deltaTime;
        }

        if (aggressionCooldownTimer > 0f)
        {
            aggressionCooldownTimer -= Time.deltaTime;
        }

		if (blinkCooldownTimer > 0f)
        {
            blinkCooldownTimer -= Time.deltaTime;
        }
		
        if (expectationCooldownTimer > 0f)
        {
            expectationCooldownTimer -= Time.deltaTime;
        }

        if (turnCooldownTimer > 0f)
        {
            turnCooldownTimer -= Time.deltaTime;
        }

        //if (stunedCooldownTimer > 0f)
        //{
        //    stunedCooldownTimer -= Time.deltaTime;
        //}

        if (ShowHP != null)
        {
            // получаем экранные координаты расположения врага
            Vector3 screenPos = metka.transform.position;
            // задаем координаты расположения хп
            ShowHP.transform.position = screenPos;
        }
    }

    void LateUpdate()
    {
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

        // Get Crushed by block if we are collisioning with the solid layer
        if (CheckColAtPlace(Vector2.up * 15, solid_layer) || CollisionSelf(box_layer))
        {
            var health = GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(9999, false, 0, 0, 0, 0, false, 0, 0, 0, 0, false, 0, false, 0, 0);
            }
        }
    }

    void Normal_Update()
    {
        // Bow Attack over here
        if (CanShoot)
        {
            bowAttackCooldownTimer = BowAttackCooldownTime;
            fsm.ChangeState(States.BowAttack, StateTransition.Overwrite);
            return;
        }

        if (CanCast)
        {
            castAttackCooldownTimer = CastAttackCooldownTime;
            fsm.ChangeState(States.Cast, StateTransition.Overwrite);
            return;
        }

        if (CanAttack)
        {
            meleeAttackCooldownTimer = MeleeAttackCooldownTime;
            fsm.ChangeState(States.Attack, StateTransition.Overwrite);
            return;
        }

        if (CanStuned)
        {
            //stunedCooldownTimer = StunedCooldownTime;
            fsm.ChangeState(States.Hit, StateTransition.Overwrite);
            return;
        }

        if (CanAggression)
        {
            BehaivourType = Behaivour.Aggression;
            OnAggression = false;
            aggressionCooldownTimer = AggressionCooldownTime;
        }

        if (CanIdle)
        {
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            BehaivourType = Behaivour.Idle;
            return;
        }
		
		if (CanBlink)
        {
            blinkCooldownTimer = BlinkCooldownTime;
			OnBlink = false;
            fsm.ChangeState(States.Blink, StateTransition.Overwrite);
            return;
        }
		
        else if (BehaivourType == Behaivour.Idle)//if (expectationCooldownTimer >= 0f)
        {
            //expectationCooldownTimer = ExpectationCooldownTime;
            if (AttackType == Attack.Melee && TypeEnemy != Type.Snake)
                BehaivourType = Behaivour.Patroling;
            if (AttackType == Attack.Archery || AttackType == Attack.Magic)
            {
                BehaivourType = Behaivour.FreeWalking;
            }
        }

        float num = onGround ? 1f : 0.65f;

        if (BehaivourType == Behaivour.FreeWalking)
        {
            // преследование
            if (InDetectionZone)
            {
                OnAggression = true;
				OnBlink = true;
            }
            // патрулироавние
            else if (!InVisibilityZone)
            {
                aggressionCooldownTimer = AggressionCooldownTime;
                Speed.x = Calc.Approach(Speed.x, moveX * walkSpeed, RunReduce * num * Time.deltaTime);

                if (!onGround)
                {
                    float target = MaxFall;
                    Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
                }

                if (moveX != 0 && CheckColInDir(new Vector2(moveX, 0), bumper_layer) || CheckColInDir(new Vector2(moveX, 0), solid_layer))
                {
                    if (turnCooldownTimer <= 0f)
                    {
                        CharacterRotation();
                        moveX *= -1;
                        turnCooldownTimer = TurnCooldownTime;
                    }
                }
            }
        }


        if (BehaivourType == Behaivour.Patroling)
        {
            // преследование
            if (InDetectionZone)
            {
                OnAggression = true;
				OnBlink = true;
            }
            // патрулироавние
            else if (!InVisibilityZone)
            {
                aggressionCooldownTimer = AggressionCooldownTime;
                Speed.x = Calc.Approach(Speed.x, moveX * walkSpeed, RunReduce * num * Time.deltaTime);

                if (!onGround)
                {
                    float target = MaxFall;
                    Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
                }

                if (moveX != 0 && CheckColInDir(new Vector2(moveX, 0), bumper_layer) || CheckColInDir(new Vector2(moveX, 0), solid_layer))
                {
                    if (turnCooldownTimer <= 0f)
                    {
                        CharacterRotation();
                        moveX *= -1;
                        turnCooldownTimer = TurnCooldownTime;
                    }
                }
            }
        }


        if (BehaivourType == Behaivour.Following)
        {
            if (InDetectionZone)
            {
                if (moveX != 0 && CheckColInDir(new Vector2(moveX, 0), solid_layer))
                {
                    if (turnCooldownTimer <= 0f)
                    {
                        CharacterRotation();
                        moveX *= -1;
                        turnCooldownTimer = TurnCooldownTime;
                    }
                }
                if (!InVisibilityZone)
                {
                    if (turnCooldownTimer <= 0f)
                    {
                        CharacterRotation();
                        moveX *= -1;
                        turnCooldownTimer = TurnCooldownTime;
                    }
                }

                Speed.x = Calc.Approach(Speed.x, moveX * walkSpeed, RunReduce * num * Time.deltaTime);

                if (!onGround)
                {
                    float target = MaxFall;
                    Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
                }
            }
        }

        if (BehaivourType == Behaivour.Aggression)  // переделать новую зону
        {
            if (AttackType == Attack.Archery || AttackType == Attack.Magic)
            {
                Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

                var diff = Player.transform.position.x - transform.position.x; //позиция

                if (-(int)Facing * diff < 0)  // разворот за игроком
                {
                    if (turnCooldownTimer <= 0f)
                    {
                        CharacterRotation();
                        moveX *= -1;
                        turnCooldownTimer = TurnCooldownTime;
                    }
                }
                if (!onGround)
                {
                    float target = MaxFall;
                    Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
                }
            }

            if (AttackType == Attack.Melee)
            {
                Speed.x = Calc.Approach(Speed.x, moveX * walkSpeed, RunReduce * num * Time.deltaTime);

                var diff = Player.transform.position.x - transform.position.x; //позиция

                if (-(int)Facing * diff < 0)  // разворот за игроком
                {
                    if (turnCooldownTimer <= 0f)
                    {
                        CharacterRotation();
                        moveX *= -1;
                        turnCooldownTimer = TurnCooldownTime;
                    }
                }
                if (!onGround)
                {
                    float target = MaxFall;
                    Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
                }
            }
        }
        if (BehaivourType == Behaivour.Idle)
        {
            // Horizontal Speed Update Section

            Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

            if (!onGround)
            {
                float target = MaxFall;
                Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
            }
        }
    }

    void CharacterRotation()
    {
        Facings facings = (Facings)moveX;
        Facing = facings;
    }

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

        if (CanStuned)
        {
            //stunedCooldownTimer = StunedCooldownTime;
            fsm.ChangeState(States.Hit, StateTransition.Overwrite);
            return;
        }
    }

    void Cast_Update()
    {
        // Horizontal Speed Update Section
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }

        if (CanStuned)
        {
            //stunedCooldownTimer = StunedCooldownTime;
            fsm.ChangeState(States.Hit, StateTransition.Overwrite);
            return;
        }
    }

    void Attack_Update()
    {
        // Horizontal Speed Update Section
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }

        if (CanStuned)
        {
            //stunedCooldownTimer = StunedCooldownTime;
            fsm.ChangeState(States.Hit, StateTransition.Overwrite);
            return;
        }
    }
    void Attack_Exit()
    {
        if (!(CheckColAtPlace(Vector2.right * 26, solid_layer) || (CheckColAtPlace(Vector2.left * 26, solid_layer))))
        {
            float num = onGround ? 1f : AirMult;
            Speed.x = Calc.Approach(Speed.x, moveX * walkSpeed, RunReduce * num * Time.deltaTime);

            if (!onGround)
            {
                float target = MaxFall;
                Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
            }
        }
    }
    void Idle_Update()
    {
        if (!onGround)
        {
            fsm.ChangeState(States.Attack, StateTransition.Overwrite);
        }

        if (CanStuned)
        {
            //stunedCooldownTimer = StunedCooldownTime;
            fsm.ChangeState(States.Hit, StateTransition.Overwrite);
            return;
        }
    }

    void Death_Update()
    {
        // Horizontal Speed Update Section
        float num = onGround ? 1f : 0.65f;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

    }

    void Hit_Update()
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

    [ContextMenu("Reset Enemy HP")]
    public void ResetEnenmyHP()
    {
        var health = GetComponent<Health>();
        health.health = health.maxHealth;
    }

    public void SetHPBar()
    {
        var health = GetComponent<Health>();
        // создаем новый слайдер на основе эталона
        ShowHP = (Slider)Instantiate(EnemyHP);
        //Объявляем что он будет расположен в canvas

        ShowHP.transform.SetParent(MyGUI.transform, true);
        ShowHP.maxValue = health.maxHealth;
        ShowHP.value = health.health;
    }

    // Function to update the sprite scale, facing direction and animations 
    void UpdateSprite()
    {
        //Approch the normal sprite scale at a set rate
        // SpriteScale.x = Calc.Approach(SpriteScale.x, 1f, 0.04f);
        // SpriteScale.y = Calc.Approach(SpriteScale.y, 1f, 0.04f);

        //Set the SpriteHolder scale to the target scale
        var targetSpriteHolderScale = new Vector3(SpriteScale.x, SpriteScale.y, 1f);
        if (SpriteHolder.localScale != targetSpriteHolderScale)
        {
            SpriteHolder.localScale = targetSpriteHolderScale;
        }

        // Set the x scale to the current facing direction
        var targetLocalScale = new Vector3(((int)Facing) * -1f, transform.localScale.y, transform.localScale.z);
        if (transform.localScale != targetLocalScale)
        {
            transform.localScale = targetLocalScale;
        }

        if (TypeEnemy == Type.MeleeSceleton)
        {

            if (fsm.State == States.Death)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
                {
                    animator.Play("Death");
                }
            }
            else if (fsm.State == States.Stun)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Stun"))
                {
                    animator.Play("Stun");
                }
            }
            else if (fsm.State == States.Attack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    animator.Play("Attack");
                }
            } // If on the ground
            else if (onGround)
            {
                // If the is nohorizontal movement input
                if (Speed.x == 0)
                {
                    // Idle Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                    {
                        animator.Play("Idle");
                    }
                    // If there is horizontal movement input
                }
                else
                {
                    // Walk Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                    {
                        animator.Play("Walk");
                    }
                }
            }
        }

        if (TypeEnemy == Type.ArcherySceleton)
        {

            if (fsm.State == States.Death)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowDeath"))
                {
                    animator.Play("BowDeath");
                }
            }
            else if (fsm.State == States.Stun)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowStun"))
                {
                    animator.Play("BowStun");
                }
            }
            else if (fsm.State == States.BowAttack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
                {
                    animator.Play("Shoot");
                }
            } // If on the ground
            else if (onGround)
            {
                // If the is nohorizontal movement input
                if (Speed.x == 0)
                {
                    // Idle Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowIdle"))
                    {
                        animator.Play("BowIdle");
                    }
                    // If there is horizontal movement input
                }
                else
                {
                    // Walk Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowWalk"))
                    {
                        animator.Play("BowWalk");
                    }
                }
            }
        }

        if (TypeEnemy == Type.MagicSceleton)
        {

            if (fsm.State == States.Death)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("MageDeath"))
                {
                    animator.Play("MageDeath");
                }
            }
            else if (fsm.State == States.Stun)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("MageStun"))
                {
                    animator.Play("MageStun");
                }
            }
            else if (fsm.State == States.Cast)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Cast"))
                {
                    animator.Play("Cast");
                }
            } // If on the ground
            else if (onGround)
            {

                // If the is nohorizontal movement input
                if (Speed.x == 0)
                {
                    // Idle Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("MageIdle"))
                    {
                        animator.Play("MageIdle");
                    }
                    // If there is horizontal movement input
                }
                else
                {
                    // Walk Animation
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("MageWalk"))
                    {
                        animator.Play("MageWalk");
                    }
                }

            }
        }

        if (TypeEnemy == Type.Snake)
        {

            if (fsm.State == States.Death)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("SnakeDeath"))
                {
                    animator.Play("SnakeDeath");
                }
            }
            else if (fsm.State == States.Attack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("SnakeAttack"))
                {
                    animator.Play("SnakeAttack");
                }
            }
            // If the is nohorizontal movement input
            else {
                // Idle Animation
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("SnakeIdle"))
                {
                    animator.Play("SnakeIdle");
                }
                // If there is horizontal movement input
            }
            // If there is horizontal movement input
        }

    }

    void Stun_Update()
    {
        // Horizontal Speed Update Section
        float num = onGround ? 1f : 0.65f;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
    }

	void Blink_Update()
    {
        // Horizontal Speed Update Section
        float num = onGround ? 1f : 0.65f;

        Speed.x = Calc.Approach(0, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
    }
	
	void Blink_Enter()
    {
		Debug.Log(blinkCooldownTimer);
        //animator.Play("Blink");

        transform.position = Vector2.Lerp(transform.position, new Vector3(EnemyDetection.PlayerPos.x - 5, EnemyDetection.PlayerPos.y + 25, 0), 2); // домножить на направление игрока 
	}
	
	void Blink_Exit()
    {
        //animator.Play("Reveal");
	}
	
    public void Freez()
    {
        walkSpeed = 0;
    }

    public void UnFreez()
    {
        walkSpeed = WalkSpeed;
    }

    public void Stun()
    {
        fsm.ChangeState(States.Stun, StateTransition.Overwrite);
    }

    public void Hit()
    {
        OnAggression = true;
		OnBlink = true;
        var health = GetComponent<Health>();
        // показываем текущие здоровье на полосе хп
        ShowHP.value = health.health;
        //int random = Random.Range(0, 99);
        //if (random < 50)
        //{
        //fsm.ChangeState(States.Stun, StateTransition.Overwrite);
        //}
    }

    public void Die()
    {
        //var playercomponent = GetComponent<Player>();
        //if (playercomponent != null)
        //{
        //    playercomponent.GetComponent<Player>().MaxRun = playercomponent.curRun;
        //}
        var health = GetComponent<Health>();
        ShowHP.transform.SetParent(transform, true);
        fsm.ChangeState(States.Death, StateTransition.Overwrite);
        //Destroy(ImpactZone);
        //Destroy(VisibilityZone);
    }
}
