#pragma warning disable 0649

using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using UnityEngine.UI;
using Pathfinding;

public class Enemy : Actor
{
    public enum Type
    {
        MeleeSceleton,
        ArcherySceleton,
        MagicSceleton,
        Snake,
        SandDemon,
        Bat,
        Golem,
        Beholder
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
    private float blinkCooldownTimer = 0f;
    private float expectationCooldownTimer = 0f;
    //public float StunedCooldownTime = 0.25f;
    private float stunedCooldownTimer = 0f;
    public bool OnStun = false;
    public bool OnAggression = false;
    public bool OnBlink = false;
    public bool OnSpecial = false;
    public bool firstHit = true;
    public int DpsTestTime = 30;


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


    [Header("MeleeWeapons")]
    [TabGroup("MeleeWeapons")]
    public int MeleeCriticalDamage = 0;
    [TabGroup("MeleeWeapons")]
    public float MeleeAttackCooldownTime = 2f; // Задержка по ближней атаке 
    [TabGroup("MeleeWeapons")]
    public int MeleeAttackMaxDamage;
    [TabGroup("MeleeWeapons")]
    public int MeleeAttackMinDamage;
    [TabGroup("MeleeWeapons")]
    public int MeleeCriticalDamageMultiply;
    [TabGroup("MeleeWeapons")]
    public int MeleeCriticalDamageChance;
    [TabGroup("MeleeWeapons")]
    public int MeleeDistanceForAttack = 18;

    [Header("RangedWeapons")]
    [TabGroup("RangedWeapons")]
    public float BowAttackCooldownTime = 4f;     // Задержка по выстрелу
    [TabGroup("RangedWeapons")]
    public float CastAttackCooldownTime = 2f;     // Задержка по выстрелу
    [TabGroup("RangedWeapons")]
    public int RangedCriticalDamage = 0;
    [TabGroup("RangedWeapons")]
    public int RangedAttackMinDamage = 1;
    [TabGroup("RangedWeapons")]
    public int RangedAttackMaxDamage = 1;
    [TabGroup("RangedWeapons")]
    public int RangedCriticalDamageMultiply;
    [TabGroup("RangedWeapons")]
    public int RangedCriticalDamageChance;
    [TabGroup("RangedWeapons")]
    public Transform GunBarrel;                  // Позиция точки выстрела

    [Header("SpecialAttack")]
    [TabGroup("SpecialAttack")]
    public bool hasFirstSpecial = false;
    [TabGroup("SpecialAttack")]
    public bool hasSecondSpecial = false;
    [TabGroup("SpecialAttack")]
    public bool hasThirdSpecial = false;
    [TabGroup("SpecialAttack")]
    public float FirstSpecialAttackCooldownTime = 10f;
    [TabGroup("SpecialAttack")]
    public float SecondSpecialAttackCooldownTime = 10f;
    [TabGroup("SpecialAttack")]
    public float ThirdSpecialAttackCooldownTime = 10f;
    [TabGroup("SpecialAttack")]
    public float FirstSpecialTimer = 10f;
    [TabGroup("SpecialAttack")]
    public float SecondSpecialTimer = 10f;
    [TabGroup("SpecialAttack")]
    public float ThirdSpecialTimer = 10f;
    [TabGroup("SpecialAttack")]
    public int DistanceForFirstSpecial = 30;
    [TabGroup("SpecialAttack")]
    public int DistanceForSecondSpecial = 0;
    [TabGroup("SpecialAttack")]
    public int DistanceForThirdSpecial = 0;

    [TabGroup("Tab Group 2", "Poison")]
    [Header("Poison")]
    public bool MeleeAttackCanPoison = false;
    [TabGroup("Tab Group 2", "Poison")]
    public int MeleePoisonDamaged = 0;
    [TabGroup("Tab Group 2", "Poison")]
    public int MeleePoisonFrequency = 0;
    [TabGroup("Tab Group 2", "Poison")]
    public int MeleePoisonTick = 0;
    [TabGroup("Tab Group 2", "Poison")]
    public int MeleePoisonChance = 100;

    [TabGroup("Tab Group 2", "Fire")]
    [Header("Fire")]
    public bool MeleeAttackCanFire = false;
    [TabGroup("Tab Group 2", "Fire")]
    public int MeleeFireDamaged = 0;
    [TabGroup("Tab Group 2", "Fire")]
    public int MeleeFireFrequency = 0;
    [TabGroup("Tab Group 2", "Fire")]
    public int MeleeFireTick = 0;
    [TabGroup("Tab Group 2", "Fire")]
    public int MeleeFireChance = 100;


    [TabGroup("Tab Group 2", "Freez")]
    [Header("Freez")]
    public bool MeleeAttackCanFreez = false;
    [TabGroup("Tab Group 2", "Freez")]
    public int MeleeFreezDuration = 0;
    [TabGroup("Tab Group 2", "Freez")]
    public int MeleeFreezChance = 100;


    [Header("Push")]
    [TabGroup("Tab Group 2", "Push")]
    public bool MeleeAttackCanPush = false;
    [TabGroup("Tab Group 2", "Push")]
    public int MeleePushDistance = 0;
    [TabGroup("Tab Group 2", "Push")]
    public bool MeleeAttackCanPushUp = false;
    [TabGroup("Tab Group 2", "Push")]
    public int MeleePushUpDistance = 0;

    [Header("Stun")]
    [TabGroup("Tab Group 2", "Stun")]
    public bool MeleeAttackCanStun = false;
    [TabGroup("Tab Group 2", "Stun")]
    public int MeleeStunDuration = 0;
    [TabGroup("Tab Group 2", "Stun")]
    [Range(0, 99)]
    public int MeleeStunChance = 100;



    [TabGroup("Tab Group 3", "Poison")]
    [Header("Poison")]
    public bool RangedAttackCanPoison = false;
    [TabGroup("Tab Group 3", "Poison")]
    public int RangedPoisonDamaged = 0;
    [TabGroup("Tab Group 3", "Poison")]
    public int RangedPoisonFrequency = 0;
    [TabGroup("Tab Group 3", "Poison")]
    public int RangedPoisonTick = 0;
    [TabGroup("Tab Group 3", "Poison")]
    public int RangedPoisonChance = 100;


    [TabGroup("Tab Group 3", "Fire")]
    [Header("Fire")]
    public bool RangedAttackCanFire = false;
    [TabGroup("Tab Group 3", "Fire")]
    public int RangedFireDamaged = 0;
    [TabGroup("Tab Group 3", "Fire")]
    public int RangedFireFrequency = 0;
    [TabGroup("Tab Group 3", "Fire")]
    public int RangedFireTick = 0;
    [TabGroup("Tab Group 3", "Fire")]
    public int RangedFireChance = 100;


    [TabGroup("Tab Group 3", "Freez")]
    [Header("Freez")]
    public bool RangedAttackCanFreez = false;
    [TabGroup("Tab Group 3", "Freez")]
    public int RangedFreezDuration = 0;
    [TabGroup("Tab Group 3", "Freez")]
    public int RangedFreezChance = 100;


    [Header("Push")]
    [TabGroup("Tab Group 3", "Push")]
    public bool RangedAttackCanPush = false;
    [TabGroup("Tab Group 3", "Push")]
    public int RangedPushDistance = 0;
    [TabGroup("Tab Group 3", "Push")]
    public bool RangedAttackCanPushUp = false;
    [TabGroup("Tab Group 3", "Push")]
    public int RangedPushUpDistance = 0;


    [Header("Stun")]
    [TabGroup("Tab Group 3", "Stun")]
    public bool RangedAttackCanStun = false;
    [TabGroup("Tab Group 3", "Stun")]
    public int RangedStunDuration = 0;
    [TabGroup("Tab Group 3", "Stun")]
    [Range(0, 99)]
    public int RangedStunChance = 100;


    private bool InVisibilityZone = false;       // Проверка на нахождение в зоне видимости
    private bool InDetectionZone = false;           // Проверка на нахождение в зоне обнаружения   
    private float meleeAttackCooldownTimer = 0f; // Таймер до ближней атаки
    private float firstSpecialAttackCooldownTimer = 0f; // Таймер до спец приема
    private float secondSpecialAttackCooldownTimer = 0f; // Таймер до спец приема
    private float thirdSpecialAttackCooldownTimer = 0f; // Таймер до спец приема
    private float bowAttackCooldownTimer = 0f;   // Таймер до выстрела
    private float castAttackCooldownTimer = 0f;   // Таймер до выстрела
    private Transform Player;



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
        Blink,
        FirstSpecial,
        SecondSpecial,
        ThirdSpecial
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
        Flying,
        Idle,                               // Покой для лет тоже
        Aggression
    }
    public Behaivour BehaivourType = Behaivour.Idle;

    public GameObject DetectionZone;
    public GameObject VisibilityZone;

    private EnemyVisibility EnemyVisibility;// Скрипт видимости
    private EnemyDetection EnemyDetection;  // Скрипт обнаружения
    private EnemyAttacking EnemyAttacking;// Скрипт зоны для атаки
    public float PatrolTimer = 0;

    public float distanceDetecting = 50;

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
            return (AttackType == Attack.Melee && EnemyVisibility.InVisibilityZone &&
            ((CheckColAtPlace(Vector2.right * MeleeDistanceForAttack, player_layer)) || CheckColAtPlace(Vector2.left * MeleeDistanceForAttack, player_layer)) &&
             meleeAttackCooldownTimer <= 0f);

            //(AttackType == Attack.Flying && EnemyVisibility.InVisibilityZone && meleeAttackCooldownTimer <= 0f);

            //(TypeEnemy == Type.SandDemon && EnemyVisibility.InVisibilityZone &&
            //((CheckColAtPlace(-Vector2.right * 45, player_layer)) || CheckColAtPlace(-Vector2.left * 45, player_layer)) && // вынести число расстояния взависимости от типа врага
            // meleeAttackCooldownTimer <= 0f);
        }
    }

    public bool CanFirstSpecial
    {
        get
        {
            return hasFirstSpecial && EnemyVisibility.InVisibilityZone && // если получает x урона
                ((CheckColAtPlace(Vector2.right * DistanceForFirstSpecial, player_layer)) || CheckColAtPlace(Vector2.left * DistanceForFirstSpecial, player_layer)) &&
                firstSpecialAttackCooldownTimer <= 0f; ;
        }
    }

    public bool CanSecondSpecial
    {
        get
        {
            return hasSecondSpecial && EnemyVisibility.InVisibilityZone && // если получает x урона
                ((CheckColAtPlace(Vector2.right * DistanceForSecondSpecial, player_layer)) || CheckColAtPlace(Vector2.left * DistanceForSecondSpecial, player_layer)) &&
                secondSpecialAttackCooldownTimer <= 0f; ;
        }
    }

    public bool CanThirdSpecial
    {
        get
        {
            return hasThirdSpecial && EnemyVisibility.InVisibilityZone && // если получает x урона
                ((CheckColAtPlace(Vector2.right * DistanceForThirdSpecial, player_layer)) || CheckColAtPlace(Vector2.left * DistanceForThirdSpecial, player_layer)) &&
                thirdSpecialAttackCooldownTimer <= 0f; ;
        }
    }

    public bool CanIdle
    {
        get
        {
            return ((AttackType == Attack.Melee && (CheckColAtPlace(Vector2.right * 18, player_layer) || CheckColAtPlace(Vector2.left * 18, player_layer)) || CheckColInDir(new Vector2(moveX, 0), player_layer))
                && meleeAttackCooldownTimer >= 0f) ||
                ((AttackType == Attack.Archery || AttackType == Attack.Magic) && EnemyVisibility.InVisibilityZone);
        }
    }

    public bool CanAggression
    {
        get
        {
            return OnAggression;
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

        //Movement Variables
        walkSpeed = WalkSpeed;

        //HB Bar UI
        MyGUI = GameObject.Find("Canvas");
        SetHPBar();

        //Visibility Zone
        EnemyVisibility = VisibilityZone.GetComponentInChildren<EnemyVisibility>();
        InVisibilityZone = EnemyVisibility.InVisibilityZone;
        //Detection Zone
        EnemyDetection = DetectionZone.GetComponentInChildren<EnemyDetection>();
        InDetectionZone = EnemyDetection.InDetectionZone;

        //Player find to follow
        Player = GameManager.instance.player.transform;

        if (BehaivourType == Behaivour.Flying)
            GetComponent<AIDestinationSetter>().target = Player;
    }

    new void Update()
    {
        base.Update();
        // Update all collisions here
        wasOnGround = onGround;
        onGround = OnGround();

        //Debug.Log(BehaivourType.ToString());

        //if (BehaivourType == Behaivour.Flying)
        //{
        //    float dist = Vector3.Distance(transform.position, Player.transform.position);

        //    if (dist < distanceDetecting)
        //    {
        //        Debug.Log("враг заметил");
        //        //TODO враг в зоне и движется 
        //    }
        //}

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

        if (firstSpecialAttackCooldownTimer > 0f)
        {
            firstSpecialAttackCooldownTimer -= Time.deltaTime;
        }

        if (secondSpecialAttackCooldownTimer > 0f)
        {
            secondSpecialAttackCooldownTimer -= Time.deltaTime;
        }

        if (thirdSpecialAttackCooldownTimer > 0f)
        {
            thirdSpecialAttackCooldownTimer -= Time.deltaTime;
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
                health.TakeDamage(9999, false, 0, 0, 0, 0, false, 0, 0, 0, 0, false, 0, false, 0, 0, false, 0, false, 0, 0);
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
            Debug.Log("attack");
            meleeAttackCooldownTimer = MeleeAttackCooldownTime;
            fsm.ChangeState(States.Attack, StateTransition.Overwrite);
            return;
        }

        if (CanFirstSpecial)
        {
            firstSpecialAttackCooldownTimer = FirstSpecialAttackCooldownTime;
            fsm.ChangeState(States.FirstSpecial, StateTransition.Overwrite);
            return;
        }

        if (CanSecondSpecial)
        {
            secondSpecialAttackCooldownTimer = SecondSpecialAttackCooldownTime;
            fsm.ChangeState(States.SecondSpecial, StateTransition.Overwrite);
            return;
        }

        if (CanThirdSpecial)
        {
            thirdSpecialAttackCooldownTimer = ThirdSpecialAttackCooldownTime;
            fsm.ChangeState(States.ThirdSpecial, StateTransition.Overwrite);
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
            StartCoroutine("PatrolingCor");
            OnAggression = false;
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

        if (BehaivourType == Behaivour.Idle)//if (expectationCooldownTimer >= 0f)
        {
            BehaivourType = Behaivour.Patroling;
        }

        float num = onGround ? 1f : 0.65f;

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
            Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

            if (!(moveX != 0 && CheckColInDir(new Vector2(moveX, 0), solid_layer)))
            {
                Speed.x = Calc.Approach(Speed.x, moveX * walkSpeed, RunReduce * num * Time.deltaTime);
            }

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

        // Special
        if (hasFirstSpecial && hasSecondSpecial && hasThirdSpecial && InDetectionZone)
        {
            int rnd = Random.Range(0, 2);
            Debug.Log("random special attack:" + rnd);
            OnSpecial = false;
            if (rnd == 0)
                StartCoroutine("FirstSpecialCoroutine");
            if (rnd == 1)
                StartCoroutine("SecondSpecialCoroutine");
            if (rnd == 2)
                StartCoroutine("ThirdSpecialCoroutine");
        }

        if (hasFirstSpecial && hasSecondSpecial && InDetectionZone)
        {
            int rnd = Random.Range(0, 1);
            Debug.Log("random special attack:" + rnd);
            OnSpecial = false;
            if (rnd == 0)
                StartCoroutine("FirstSpecialCoroutine");
            if (rnd == 1)
                StartCoroutine("SecondSpecialCoroutine");
        }

        if (hasFirstSpecial && InDetectionZone)
        {
            OnSpecial = false;
            StartCoroutine("FirstSpecialCoroutine");
        }
    }

    IEnumerator PatrolingCor()
    {
        yield return new WaitForSeconds(AggressionCooldownTime);
        BehaivourType = Behaivour.Patroling;
    }

    IEnumerator FirstSpecialCoroutine()
    {
        yield return new WaitForSeconds(FirstSpecialTimer);
        OnSpecial = true;
    }

    IEnumerator SecondSpecialCoroutine()
    {
        yield return new WaitForSeconds(SecondSpecialTimer);
        OnSpecial = true;
    }

    IEnumerator ThirdSpecialCoroutine()
    {
        yield return new WaitForSeconds(ThirdSpecialTimer);
        OnSpecial = true;
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

    void FirstSpecial_Enter()
    {
        var col = GetComponentInChildren<EnemyHitBoxManager>();
        col.ChangeCollider(1);
    }

    void FirstSpecial_Exit()
    {
        var col = GetComponentInChildren<EnemyHitBoxManager>();
        col.ChangeCollider(0);
    }

    void FirstSpecial_Update()
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

    void SecondSpecial_Enter()
    {
        var col = GetComponentInChildren<EnemyHitBoxManager>();
        col.ChangeCollider(2);
    }

    void SecondSpecial_Exit()
    {
        var col = GetComponentInChildren<EnemyHitBoxManager>();
        col.ChangeCollider(0);
    }

    void SecondSpecial_Update()
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

    void ThirdSpecial_Enter()
    {
        var col = GetComponentInChildren<EnemyHitBoxManager>();
        col.ChangeCollider(3);
    }

    void ThirdSpecial_Exit()
    {
        var col = GetComponentInChildren<EnemyHitBoxManager>();
        col.ChangeCollider(0);
    }

    void ThirdSpecial_Update()
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
            else
            {
                // Idle Animation
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("SnakeIdle"))
                {
                    animator.Play("SnakeIdle");
                }
                // If there is horizontal movement input
            }
            // If there is horizontal movement input
        }


        if (TypeEnemy == Type.SandDemon)
        {

            if (fsm.State == States.Death)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
                {
                    animator.Play("Death");
                }
            }

            else if (fsm.State == States.Blink)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Blink"))
                {
                    animator.Play("Blink");
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
            // If there is horizontal movement input
        }


        if (TypeEnemy == Type.Golem)
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
            else if (fsm.State == States.FirstSpecial)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("FirstSpecial"))
                {
                    animator.Play("FirstSpecial");
                }
            }
            else if (fsm.State == States.SecondSpecial)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("SecondSpecial"))
                {
                    animator.Play("SecondSpecial");
                }
            }
            else if (fsm.State == States.ThirdSpecial)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("ThirdSpecial"))
                {
                    animator.Play("ThirdSpecial");
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

        if (TypeEnemy == Type.Bat)
        {
            if (fsm.State == States.Attack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    animator.Play("Attack");
                }
            } // If on the ground

            if (fsm.State == States.Death)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
                {
                    animator.Play("Death");
                }
            }

            else
            {
                // Walk Animation
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Fly"))
                {
                    animator.Play("Fly");
                }
            }
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
        //transform.position = Vector2.Lerp(transform.position, new Vector3(EnemyDetection.PlayerPos.x - 5, EnemyDetection.PlayerPos.y + 25, 0), 2); // домножить на направление игрока 

        //animator.Play("Blink");
    }

    public void Blink()
    {
        var player = GetComponentInParent<Player>();

        if (player != null)
        {
            if (!player.sticking && !player.CheckColAtPlace(Vector2.right * (int)player.Facing * 10, solid_layer)
            && !player.CheckColAtPlace(Vector2.left * 10, solid_layer)
            && !player.CheckColAtPlace(Vector2.up * 10, solid_layer))
            {
                transform.position = Vector2.Lerp(transform.position, new Vector3(EnemyDetection.PlayerPos.x - 5, EnemyDetection.PlayerPos.y + 25, 0), 2); // домножить на направление игрока 
            }
        }
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

    public void UnStun()
    {
        fsm.ChangeState(States.Normal, StateTransition.Overwrite);
    }

    public void Push(int pushDistance)
    {
        if (!CheckColAtPlace(Vector2.right * (int)Facing * pushDistance, solid_layer))
        {
            Debug.Log(Vector2.right * (int)Facing * pushDistance);
            transform.position = new Vector2(transform.position.x + (pushDistance * (int)Facing), transform.position.y);
        }
    }

    public void PushUp(int pushDistance)
    {
        if (!CheckColAtPlace(Vector2.up * pushDistance, solid_layer))
        {
            Debug.Log(Vector2.up * pushDistance);
            transform.position = new Vector2(transform.position.x + pushDistance, transform.position.y + pushDistance);
        }
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

    public void DpsTest()
    {
        if (firstHit)
        {
            StartCoroutine("DpsTestCoroutine");
            firstHit = false;
            Debug.Log("START");
        }
    }

    IEnumerator DpsTestCoroutine()
    {
        var health = GetComponent<Health>();
        yield return new WaitForSeconds(DpsTestTime);
        Debug.Log("STOP");
        Debug.Log(health.maxHealth - health.health + " dmg per " + DpsTestTime + " sec");
        health.health = health.maxHealth;
        firstHit = true;
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
