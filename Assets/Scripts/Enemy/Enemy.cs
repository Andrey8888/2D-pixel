#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using UnityEngine.UI;

public class Enemy : Actor
{
    [Header("Movement Variables")]
    // Gravity, Maximun fall speed & fastfall Speed
    public float Gravity = 900f;                // Speed at which you are pushed down when you are on the air
    public float MaxFall = -160f;               // Maximun common speed at which you can fall
    public float RunReduce = 400f;              // Horizontal Acceleration when you're already when your horizontal speed is higher or equal to the maximun
    public float MaxRun = 30f;                  // Maximun Horizontal Run Speed
    public float RunAccel = 1000f;              // Horizontal Acceleration Speed
    public float AirMult = 0.65f;               // Multiplier for the air horizontal movement (friction) the higher the more air control you'll have
    public float TurnCooldownTime = 1f;        
    private float turnCooldownTimer = 0f;
    public float StunedCooldownTime = 0.25f;
    private float stunedCooldownTimer = 0f;
    public bool OnStun = false;

    public float MinDist = 0.5f;                // Допустимое расстояние, при котором враг переключается на следующую точку

    // Helper private Variables
    private int moveX;                          // Variable to store the horizontal Input each frame

    [Header("Facing Direction")]
    public Facings Facing;  // Facing Direction

    //[Header("HealthBar")]
    //public Canvas MyGUI;                      // UI на котором будем отображать  
    //public Slider EnemyHP;                    // полоска здоровья врага на экране
    //public Transform metka;
    //public Camera Mycamera;

    [Header("Squash & Stretch")]
    public Transform SpriteHolder;              // Reference to the transform of the child object which holds the sprite renderer of the player
    public Vector2 SpriteScale = Vector2.one;   // The current X and Y scale of the sprite holder (used for Squash & Stretch)

    [Header("Animator")]
    public Animator animator;                   // Reference to the animator

    [Header("Particles")]
    public GameObject BloodParticle;

    [Header("Attacks")]
    public int MeleeAttackDamage = 1;
    public Transform GunBarrel;                  // Позиция точки выстрела
    private bool InVisibilityZone = false;       // Проверка на нахождение в зоне видимости
    private bool InDetectionZone = false;           // Проверка на нахождение в зоне обнаружения        
    public float MeleeAttackCooldownTime = 2f; // Задержка по ближней атаке      
    public float BowAttackCooldownTime = 4f;     // Задержка по выстрелу     
    public float CastAttackCooldownTime = 2f;     // Задержка по выстрелу  
    private float meleeAttackCooldownTimer = 0f; // Таймер до ближней атаки
    private float bowAttackCooldownTimer = 0f;   // Таймер до выстрела
    private float castAttackCooldownTimer = 0f;   // Таймер до выстрела
                                                 //private Vector3 PlayerPos;                  // Позиция игрока, необходима для расчета угла стрельбы
                                                 //public Vector3 PlayerPosGlobal;             // Позиция игрока на карте

    // States for the state machine
    public enum States
    {
        Normal,
        BowAttack,
        Attack,
        Hit,
        Stun,
        Death, 
        Cast
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
        FreeWalking,                        // Свободное движение до преграды
        Idle                                // Покой
    }
    public Behaivour BehaivourType = Behaivour.Idle;

    public GameObject DetectionZone;
    public GameObject VisibilityZone;

    private EnemyVisibility EnemyVisibility;// Скрипт видимости
    private EnemyDetection EnemyDetection;// Скрипт обнаружения
    public float PatrolTimer = 0;

    public bool CanShoot
    {
        get
        {
            return AttackType == Attack.Archery && EnemyVisibility.InVisibilityZone && castAttackCooldownTimer <= 0f;
        }
    }

    public bool CanCast
    {
        get
        {
            return AttackType == Attack.Magic && EnemyVisibility.InVisibilityZone && bowAttackCooldownTimer <= 0f;
        }
    }

    public bool CanAttack
    {
        get
        {
            return AttackType == Attack.Melee && EnemyVisibility.InVisibilityZone && ((CheckColAtPlace(Vector2.right * 18, player_layer)) || (CheckColAtPlace(Vector2.left * 18, player_layer)))
                && meleeAttackCooldownTimer <= 0f;
        }
    }

    public bool CanIdle
    {
        get
        {
            return (AttackType == Attack.Melee && ((CheckColAtPlace(Vector2.right * 18, player_layer)) || (CheckColAtPlace(Vector2.left * 18, player_layer))) && meleeAttackCooldownTimer >= 0f) 
                || (AttackType == Attack.Archery && EnemyVisibility.InVisibilityZone);
        }
    }

    public bool CanStuned
    {
        get
        {
            return OnStun && stunedCooldownTimer <= 0f;
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
    }

    new void Update()
    {
        base.Update();
        // Update all collisions here
        wasOnGround = onGround;
        onGround = OnGround();

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

        if (turnCooldownTimer > 0f)
        {
            turnCooldownTimer -= Time.deltaTime;
        }

        if (stunedCooldownTimer > 0f)
        {
            stunedCooldownTimer -= Time.deltaTime;
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
                health.TakeDamage(9999);
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
            stunedCooldownTimer = StunedCooldownTime;
            fsm.ChangeState(States.Hit, StateTransition.Overwrite);
            return;
        }

        if (CanIdle)
        {
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            BehaivourType = Behaivour.Idle;
            return;
        }
        else
        {
            if (AttackType == Attack.Melee)
            BehaivourType = Behaivour.Patroling;

            if (AttackType == Attack.Archery)
            {
                BehaivourType = Behaivour.FreeWalking;
            }
        }

        float num = onGround ? 1f : 0.65f;

        if (BehaivourType == Behaivour.FreeWalking)
        {
            Speed.x = Calc.Approach(Speed.x, moveX * MaxRun, RunReduce * num * Time.deltaTime);

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

        if (BehaivourType == Behaivour.Patroling)
        {
            // преследование
            if (InDetectionZone)
            {
                MaxRun = 90f;               
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

                Speed.x = Calc.Approach(Speed.x, moveX * MaxRun, RunReduce * num * Time.deltaTime);

                if (!onGround)
                {
                    float target = MaxFall;
                    Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
                }
            }

            else if (!InVisibilityZone)
               {

                MaxRun = 30f;
                Speed.x = Calc.Approach(Speed.x, moveX * MaxRun, RunReduce * num * Time.deltaTime);

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

                Speed.x = Calc.Approach(Speed.x, moveX * MaxRun, RunReduce * num * Time.deltaTime);

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
            stunedCooldownTimer = StunedCooldownTime;
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
            stunedCooldownTimer = StunedCooldownTime;
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
            stunedCooldownTimer = StunedCooldownTime;
            fsm.ChangeState(States.Hit, StateTransition.Overwrite);
            return;
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
            stunedCooldownTimer = StunedCooldownTime;
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

        if (AttackType == Attack.Melee)
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
                if (InDetectionZone)
                {
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
                        // Run Animation
                        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                        {
                            animator.Play("Run");
                        }
                    }
                }
                else
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
        }

        if (AttackType == Attack.Archery)
        {

            if (fsm.State == States.Death)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowDeath"))
                {
                    animator.Play("BowDeath");
                }
            }
            else if (fsm.State == States.Hit)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowHurt"))
                {
                    animator.Play("BowHurt");
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
                if (InDetectionZone)
                {
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
                        // Run Animation
                        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BowRun"))
                        {
                            animator.Play("BowRun");
                        }
                    }
                }
                else
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
        }

        if (AttackType == Attack.Magic)
        {

            if (fsm.State == States.Death)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("MageDeath"))
                {
                    animator.Play("MageDeath");
                }
            }
            else if (fsm.State == States.Hit)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("MageHurt"))
                {
                    animator.Play("MageHurt");
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
                if (InDetectionZone)
                {
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
                        // Run Animation
                        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("MageRun"))
                        {
                            animator.Play("MageRun");
                        }
                    }
                }
                else
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
        }

    }

    public void Hit()
    {
        //int random = Random.Range(0, 99);
        //Debug.Log(random + "/99 ");
        //if (random < 50)
        //{
            fsm.ChangeState(States.Stun, StateTransition.Overwrite);
        //}
    }

    void Stun_Update()
    {

        Debug.Log("Stun");
        // Horizontal Speed Update Section
        float num = onGround ? 1f : 0.65f;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
    }

    public void Die()
    {
        var playercomponent = GetComponent<Player>();
        if (playercomponent != null)
        {
            playercomponent.GetComponent<Player>().MaxRun = 140;
        }
        fsm.ChangeState(States.Death, StateTransition.Overwrite);
        //Destroy(ImpactZone);
        //Destroy(VisibilityZone);
    }
}
