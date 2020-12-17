#pragma warning disable 0649
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using UnityEngine.UI;
using PowerTools;
using Pathfinding;

public class Player : Actor
{
    #region Params
    [Header("Movement Variables")]
    [FoldoutGroup("Movement Variables")]
    // Gravity, Maximun fall speed & fastfall Speed
    public float Gravity = 900f; // Скорость, с которой игрок падает, пока находится в воздухе
    [FoldoutGroup("Movement Variables")]
    public float MaxFall = -160f; // Максимальная общая скорость, с которой вы можете упасть
    [FoldoutGroup("Movement Variables")]
    public float FastFall = -240f; // Максимальная скорость падения при быстром падении
    [FoldoutGroup("Movement Variables")]
    // Скорость бега и ускорение
    public float MaxRun = 140f; //  Максимальная скорость бега по горизонтали
    [FoldoutGroup("Movement Variables")]
    public float RunAccel = 1000f; // Скорость горизонтального ускорения
    [FoldoutGroup("Movement Variables")]
    public float RunReduce = 400f; // Горизонтальное ускорение, когда ваша горизонтальная скорость выше или равна максимальной
    // Множитель в воздухе
    [FoldoutGroup("Movement Variables")]
    public float AirMult = 0.65f; // Множитель для горизонтального перемещения воздуха (трения), чем выше, тем больше контроля в воздухе
    // Переменные для прыжка
    [FoldoutGroup("Movement Variables")]
    public float JumpSpeed = 135f; // Скорость вертикального прыжка / Сила
    [FoldoutGroup("Movement Variables")]
    public float JumpHBoost = 40f; // Повышение скорости по горизонтали при прыжке
    [FoldoutGroup("Movement Variables")]
    public float VariableJumpTime = 0.2f; // Время после выполнения прыжка, при котором вы можете удерживать клавишу прыжка, чтобы прыгнуть выше
    [FoldoutGroup("Movement Variables")]
    public float SpringJumpSpeed = 275f; // Скорость вертикального прыжка / Сила прыжков на пружине
    [FoldoutGroup("Movement Variables")]
    public float SpringJumpVariableTime = 0.05f; // Время после выполнения прыжка с пружины, при котором вы можете удерживать клавишу прыжка, чтобы прыгнуть выше
    // Переменные для прыжков от стен
    [FoldoutGroup("Movement Variables")]
    public float WallJumpForceTime = 0.16f; // Время, когда горизонтальное движение возобновляется / форсируется после прыжка со стены (если он слишком низок, игрок может взобраться на стену)
    [FoldoutGroup("Movement Variables")]
    public float WallJumpHSpeed = 130f; // Увеличение скорости по горизонтали при выполнении прыжка со стены
    [FoldoutGroup("Movement Variables")]
    public int WallJumpCheckDist = 2; // Расстояние, на котором мы проверяем стены перед выполнением прыжка со стены (рекомендуется 2-4)
    // Переменные для скольжения по стене
    [FoldoutGroup("Movement Variables")]
    public float WallSlideStartMax = -20f; //  Начальная вертикальная скорость, когда вы скользите по стене
    [FoldoutGroup("Movement Variables")]
    public float WallSlideTime = 1.2f; // Максимальное время, когда вы можете скользить по стене, прежде чем снова набрать полную скорость падения
    // Параметры для рывка и переката
    [FoldoutGroup("Movement Variables")]
    public float DashSpeed = 240f; // Скорость / Сила рывка (dash)
    [FoldoutGroup("Movement Variables")]
    public float RollSpeed = 240f; // Скорость переката 
    [FoldoutGroup("Movement Variables")]
    public float EndDashSpeed = 160f; // Повышенная скорость, когда рывок заканчивается (рекомендуется 2/3 скорости рывка)
    [FoldoutGroup("Movement Variables")]
    public float EndRollSpeed = 160f; // Повышение скорости при окончании переката (рекомендуется 2/3 скорости переката)
    [FoldoutGroup("Movement Variables")]
    public float EndDashUpMult = 0.75f; // Множитель применяется к скорости после окончания рывка, если направление, в котором вы летели, было вверх
    [FoldoutGroup("Movement Variables")]
    public float EndRollUpMult = 0.75f; // Множитель применяется к скорости после окончания переката, если направление, в котором вы катились, было вверх
    [FoldoutGroup("Movement Variables")]
    public float DashTime = 0.15f; // Общее время, которое длится рывок
    [FoldoutGroup("Movement Variables")]
    public float RollTime = 0.15f; // Общее время, которое длится перекат
    [FoldoutGroup("Movement Variables")]
    public float DashCooldown = 0.4f; // Время восстановления рывка
    [FoldoutGroup("Movement Variables")]
    public float RollCooldown = 0.4f; // Время восстановления переката
    [FoldoutGroup("Movement Variables")]
    // Другие переменные, используемые для адаптивного движения
    public float clingTime = 0.125f;  // Время после прикосновения к стене, где вы не можете покинуть стену (чтобы избежать непреднамеренного ухода от стены при попытке выполнить прыжок с стены)
    [FoldoutGroup("Movement Variables")]
    public float JumpGraceTime = 0.1f; // Время отсрочки прыжка после того, как вы покинули землю без прыжка, на котором вы все еще можете сделать прыжок
    [FoldoutGroup("Movement Variables")]
    public float JumpBufferTime = 0.1f; // Если игрок ударяет об землю в течение этого времени после нажатия кнопки прыжка, прыжок будет выполнен, как только он коснется земли
    [FoldoutGroup("Movement Variables")]                                 // Лестничные переменные
    public float LadderClimbSpeed = 60f;
    #endregion

    #region WeaponParams
    [Header("Hands")]
    public int HandAttackMinDamage = 1;//new int[2] {1, 0, 0};
    public int HandAttackMaxDamage = 2;//new int[2] {2, 0, 0};
    public int HandStepUpAfterHit = 20; //new int[2] {20,0,0};
    public float HandAttackCooldownTime = 0.8f;
    public float HandAttackSpeed = 1;

    #region MeleeWeapons
    [Header("MeleeWeapons")]
    [HorizontalGroup("base1")]
    public string MeleeWeaponType;
    private MeleeWeapon MeleeWeaponClass;

    [Header("NormalAttack")]
    [TabGroup("NormalAttack")]
    public int MeleeAttackMaxDamage;
    [TabGroup("NormalAttack")]
    public int MeleeAttackMinDamage;
    [TabGroup("NormalAttack")]
    public int MeleeCriticalDamageMultiply;
    [TabGroup("NormalAttack")]
    public int MeleeCriticalDamageChance;
    [TabGroup("NormalAttack")]
    public float MeleeAttackCooldownTime = 0.8f;
    [TabGroup("NormalAttack")]
    public float SecondSwordAttackCooldownTime = 0.4f;
    [TabGroup("NormalAttack")]
    public float ThirdSwordAttackCooldownTime = 0.4f;
    [TabGroup("NormalAttack")]
    private float MeleeAttackSpeed = 1f;
    [TabGroup("NormalAttack")]
    private float SecondSwordAttackSpeed = 1f;
    [TabGroup("NormalAttack")]
    private float ThirdSwordAttackSpeed = 1f;
    [TabGroup("NormalAttack")]
    public int MeleeManaCost = 0;

    [Header("PowerAttack")]
    [TabGroup("PowerAttack")]
    public int MeleePowerAttackMinDamage = 0;
    [TabGroup("PowerAttack")]
    public int MeleePowerAttackMaxDamage = 0;
    [TabGroup("PowerAttack")]
    public float MeleePowerAttackCooldownTime = 0.8f;
    [TabGroup("PowerAttack")]
    [Range(1, 10)]
    public int MeleePowerCriticalDamageMultiply = 1;
    [TabGroup("PowerAttack")]
    [Range(0, 99)]
    public int MeleePowerChanceCriticalDamage = 0;
    [TabGroup("PowerAttack")]
    private float MeleePowerAttackSpeed = 1f;
    [TabGroup("PowerAttack")]
    public int MeleePowerManaCost = 0;
    [TabGroup("PowerAttack")]
    public int MeleePowerShellsCount = 0;

    [TabGroup("Tab Group 2", "Poison")]
    [Header("Poison")]
    public bool MeleeAttackCanPoison = false;
    [TabGroup("Tab Group 2", "Poison")]
    public bool MeleePowerAttackCanPoison = false;
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
    public bool MeleePowerAttackCanFire = false;
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
    public bool MeleePowerAttackCanFreez = false;
    [TabGroup("Tab Group 2", "Freez")]
    public int MeleeFreezDuration = 0;
    [TabGroup("Tab Group 2", "Freez")]
    public int MeleeFreezChance = 100;

    [Header("Push")]
    [TabGroup("Tab Group 2", "Push")]
    public bool MeleeAttackCanPush = false;
    [TabGroup("Tab Group 2", "Push")]
    public bool MeleePowerAttackCanPush = false;
    [TabGroup("Tab Group 2", "Push")]
    public int MeleePushDistance = 0;
    [TabGroup("Tab Group 2", "Push")]
    public bool MeleeAttackCanPushUp = false;
    [TabGroup("Tab Group 2", "Push")]
    public bool MeleePowerAttackCanPushUp = false;
    [TabGroup("Tab Group 2", "Push")]
    public int MeleePushUpDistance = 0;

    [Header("Stun")]
    [TabGroup("Tab Group 2", "Stun")]
    public bool MeleeAttackCanStun = false;
    [TabGroup("Tab Group 2", "Stun")]
    public bool MeleePowerAttackCanStun = false;
    [TabGroup("Tab Group 2", "Stun")]
    public int MeleeStunDuration = 0;
    [TabGroup("Tab Group 2", "Stun")]
    [Range(0, 99)]
    public int MeleeStunChance = 100;

    [Header("Other")]
    [TabGroup("Tab Group 2", "Other")]
    public int StepUpAfterHit;
    [TabGroup("Tab Group 2", "Other")]
    public int PopUpAfterHit = 0;
    [TabGroup("Tab Group 2", "Other")]
    public float SpeedOnSwordAttack = 20;
    [HideInInspector]
    public bool ChangeCollider = false;
    [TabGroup("Tab Group 2", "Other")]
    public bool meleeHasSeries = false;
    [TabGroup("Tab Group 2", "Other")]
    public bool hasBlock = false;
    [TabGroup("Tab Group 2", "Other")]
    public float MeleeBlockCooldownTime = 0.5f;
    [TabGroup("Tab Group 2", "Other")]
    public bool hasPowerAttackShell = false;
    [TabGroup("Tab Group 2", "Other")]
    public bool MeleeCanThirdAttackCriticalDamage;

    #endregion

    #region RangedWeapons
    [Header("RangedWeapons")]
    [HorizontalGroup("base2")]
    public string RangedWeaponType;
    private RangedWeapon RangedWeaponClass;

    [Header("NormalAttack")]
    [TabGroup("RangedWeapons", "NormalAttack")]
    public int RangedCriticalDamage = 0;
    [TabGroup("RangedWeapons", "NormalAttack")]
    public int RangedCriticalDamageMultiply = 1;
    [TabGroup("RangedWeapons", "NormalAttack")]
    public int RangedCriticalDamageChance = 0;
    [TabGroup("RangedWeapons", "NormalAttack")]
    public int RangedAttackMinDamage = 1;//new int[2] {1, 0, 0};
    [TabGroup("RangedWeapons", "NormalAttack")]
    public int RangedAttackMaxDamage = 2;//new int[2] {2, 0, 0};
    [TabGroup("RangedWeapons", "NormalAttack")]
    private float RangedAttackSpeed = 1f;
    [TabGroup("RangedWeapons", "NormalAttack")]
    public int ShellsCount = 0;
    [TabGroup("RangedWeapons", "NormalAttack")]
    public float RangedAttackCooldownTime = 1f;
    [TabGroup("RangedWeapons", "NormalAttack")]
    public int RangedManaCost = 0;

    [Header("PowerAttack")]
    [TabGroup("RangedWeapons", "PowerAttack")]
    public int RangedPowerAttackMinDamage = 0;
    [TabGroup("RangedWeapons", "PowerAttack")]
    public int RangedPowerAttackMaxDamage = 0;
    [TabGroup("RangedWeapons", "PowerAttack")]
    public float RangedPowerAttackCooldownTime = 10f;
    [TabGroup("RangedWeapons", "PowerAttack")]
    public float RangedPowerAccumulateTime = 1f;
    [TabGroup("RangedWeapons", "PowerAttack")]
    [Range(1, 10)]
    public int RangedPowerCriticalDamageMultiply = 1;
    [TabGroup("RangedWeapons", "PowerAttack")]
    [Range(0, 99)]
    public int RangedPowerChanceCriticalDamage = 0;
    [TabGroup("RangedWeapons", "PowerAttack")]
    private float RangedPowerAttackSpeed = 1f;
    [TabGroup("RangedWeapons", "PowerAttack")]
    public int RangedPowerShellsCount = 0;
    [TabGroup("RangedWeapons", "PowerAttack")]
    public int RangedPowerManaCost = 0;

    [TabGroup("Tab Group 3", "Poison")]
    [Header("Poison")]
    public bool RangedAttackCanPoison = false;
    [TabGroup("Tab Group 3", "Poison")]
    public bool RangedPowerAttackCanPoison = false;
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
    public bool RangedPowerAttackCanFire = false;
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
    public bool RangedPowerAttackCanFreez = false;
    [TabGroup("Tab Group 3", "Freez")]
    public int RangedFreezDuration = 0;
    [TabGroup("Tab Group 3", "Freez")]
    public int RangedFreezChance = 100;

    [Header("Push")]
    [TabGroup("Tab Group 3", "Push")]
    public bool RangedAttackCanPush = false;
    [TabGroup("Tab Group 3", "Push")]
    public bool RangedPowerAttackCanPush = false;
    [TabGroup("Tab Group 3", "Push")]
    public int RangedPushDistance = 0;
    [TabGroup("Tab Group 3", "Push")]
    public bool RangedAttackCanPushUp = false;
    [TabGroup("Tab Group 3", "Push")]
    public bool RangedPowerAttackCanPushUp = false;
    [TabGroup("Tab Group 3", "Push")]
    public int RangedPushUpDistance = 0;

    [Header("Stun")]
    [TabGroup("Tab Group 3", "Stun")]
    public bool RangedAttackCanStun = false;
    [TabGroup("Tab Group 3", "Stun")]
    public bool RangedPowerAttackCanStun = false;
    [TabGroup("Tab Group 3", "Stun")]
    public int RangedStunDuration = 0;
    [TabGroup("Tab Group 3", "Stun")]
    [Range(0, 99)]
    public int RangedStunChance = 100;

    [Header("Other")]
    [TabGroup("Tab Group 3", "Other")]
    public bool rangedHasSeries = false;
    [TabGroup("Tab Group 3", "Other")]
    public bool RangedAttackCanThroughShoot = false;
    [TabGroup("Tab Group 3", "Other")]
    public bool RangedTossingUp = false;
    [TabGroup("Tab Group 3", "Other")]
    public bool RangedAttackAiming = false;
    [TabGroup("Tab Group 3", "Other")]
    public int PowerAttackPopUpAfterHit = 0;
    [TabGroup("Tab Group 3", "Other")]
    public int RangedStepUpAfterHit = 0;
    [TabGroup("Tab Group 3", "Other")]
    public bool RangedAiming = false;

    [Header("Power")]
    [TabGroup("Tab Group 3", "Power")]
    public int RangedPowerAttackPopUpAfterHit = 0;
    [TabGroup("Tab Group 3", "Power")]
    public bool RangedPowerAttackAiming = false;
    [TabGroup("Tab Group 3", "Power")]
    public bool RangedPowerAttackCanThroughShoot = false;
    [TabGroup("Tab Group 3", "Power")]
    public bool RangedHeal = false;
    [TabGroup("Tab Group 3", "Power")]
    public int RangedHealCount = 0;
    [TabGroup("Tab Group 3", "Power")]
    public bool RangedPowerBlink = false;
    #endregion


    public int meleeHitCount = 0;
    public int rangedHitCount = 0;
    public bool hasPrepare = false;
    private GameObject countShellText;
    private GameObject countPowerShellText;
    private GameObject countMeleePowerShellText;

    private bool hasBow = false;
    private bool hasSword = false;
    public bool canAim = false;
    public Sprite AimSpriteGreen; // сделать дял каждого оружия отдельный подгружать из оружия
    public Sprite AimSpriteRed;
    public GameObject aimSprite;
    [HideInInspector]
    public bool OnAttackMove = false;
    public bool OnAttackMoveBack = false;
    [HideInInspector]
    public bool tossingUp = false;
    public bool PowerMeleeAttack = false;
    public bool PowerRangedAttack = false;

    public enum ActiveWeapon
    {
        Hands,         // Руки
        Bow,           // Лук 
        Sword          // Меч
    }

    [Header("Initial weapon")]
    public ActiveWeapon activeWeapon; // Активное оружие

    public GameObject InitialWeapon = null; // TODO проверка на null Odin
    public PickupRangedWeapons RangedWeapon = null;
    public PickupMeleeWeapons MeleeWeapon = null;
    //private Sprite RangedSprite;
    //private Sprite MeleeSprite;
    #endregion

    [Header("Facing Direction")]
    public Facings Facing;  // Facing Direction
    [Header("Wall Slide Direction")]
    public int wallSlideDir;    // Wall Slide Direction
    [Header("Ladder")]
    public bool isInLadder = false;

    [Header("Respawn")]
    public float RespawnTime; // Общее время, необходимое игроку, чтобы возродиться
    private float respawnTimer = 0f; // Переменная для хранения таймера респауна
    private Vector2 respawnPosition; // Переменная для хранения таймера респауна

    [Header("Ducking & Colliders")]
    public BoxCollider2D myNormalCollider; // Коллайдер используется, когда вы находитесь в любом состоянии, кроме состояния приседа
    public BoxCollider2D myDuckingCollider; // Коллайдер используется в состоянии приседа
    public float DuckingFrictionMultiplier = 0.5f; // Множитель трения для уклона в воздухе рекомендуется 0,4-0,6 (0 - отсутствие трения, 1 - нормальное трение)

    [Header("Flying Weapon")]
    public GameObject flyingWeapon;
    public Transform pointWeapon;
    public float maxDist = 20;
    private GameObject FlyingWeapon;
    public float timeDisapear = 5;
    public float TurnCooldownTime = 1f;

    [Header("Other")]
    public GameObject wing; //TODO перенести отсюда

    // Вспомогательные частные переменные
    private int moveX; // Переменная для хранения горизонтального ввода каждого кадра
    private int moveY; // Переменная для хранения векторного ввода каждого кадра
    public float curRun = 140;
    private int oldMoveY; // Переменная для хранения вертикального ввода для последнего кадра
    private float varJumpSpeed; // Вертикальная скорость, применяемая к каждому кадру переменной jump
    private float varJumpTimer = 0f; // Переменная для хранения времени, оставшегося до перехода переменной
    private int forceMoveX; // Используется для сохранения ввода принудительного горизонтального перемещения
    private float forceMoveXTimer = 0f; // Используется для хранения времени, оставшегося на силовое горизонтальное движение
    private float maxFall; // Переменная для сохранения текущей максимальной скорости падения
    private float wallSlideTimer = 0f; // Используется для хранения времени, оставшегося на стене
    private Vector2 DashDir; // Здесь мы сохраняем направление, в котором мы мчимся
    private Vector2 RollDir; // Здесь мы храним направление, в котором катимся
    private float dashCooldownTimer = 0f; // Таймер для хранения времени восстановления рывка
    private float rollCooldownTimer = 0f; // Таймер для хранения времени восстановления броска
    private bool canStick = false; // Вспомогательная переменная для функции приклеивания стены
    public bool sticking = false; // Переменная для сохранения, если игрок в данный момент прилипает к стене
    private float stickTimer = 0f; // Таймер для хранения оставшегося времени прилипания к стене
    private float jumpGraceTimer = 0f; // Таймер для запоминания времени, оставшегося до выполнения прыжка после выхода с платформы / твердого тела
    private float jumpBufferTimer = 0f; // Таймер для хранения оставшегося времени в таймере JumpBuffer(Буфер перехода)
    private bool jumpIsInsideBuffer = false;
    private float meleeAttackCooldownTimer = 0f; // Таймер для сохранения времени восстановления, оставшегося для использования атаки ближнего боя
    private float meleePowerAttackCooldownTimer = 0f; // Таймер для сохранения времени восстановления, оставшегося для использования атаки ближнего боя
    private float meleeBlockCooldownTimer = 0f;
    [HideInInspector]
    public float secondSwordAttackCooldownTimer = 0f; // Таймер для сохранения времени восстановления, оставшегося для использования атаки ближнего боя
    [HideInInspector]
    public float thirdSwordAttackCooldownTimer = 0f; // // Таймер для сохранения времени восстановления, оставшегося для использования атаки ближнего боя
    private float rangedAttackCooldownTimer = 0f; // Таймер для сохранения времени восстановления, оставшегося после атаки лука
    [HideInInspector]
    public float rangedPowerAttackCooldownTimer = 0f; // Таймер для сохранения времени восстановления, оставшегося после атаки лука

    public float rangedPowerAccumulateTimer = 0f;
    private float moveToRespawnPositionAfter = .5f; // Пора подождать, прежде чем перейти на позицию респауна
    private float moveToRespawnPosTimer = 0f; // Таймер, чтобы запомнить, сколько времени осталось до перехода в позицию респауна
    private bool rollAgain = false;
    private float ledgeClimbTime = 1f; // Общее время, необходимое, чтобы взобраться на стену
    private float ledgeClimbTimer = 0f; // Таймер для сохранения текущего времени, прошедшего в состоянии ledgeClimb
    private Vector2 extraPosOnClimb = new Vector2(10, 16); // Дополнительная позиция для добавления к текущей позиции в конечную позицию анимации подъема соответствует начальной позиции в состоянии ожидания
    private Inventory inventory;
    private float turnCooldownTimer = 0f;

    #region Aim params
    //[Header("Objects")]
    //// public GameObject Arrow;         // Стрела, которую наш герой выпускает
    //public GameObject AimLine;          // Линия прицеливания

    //[Header("Parameters")]
    //public float Velocity;              // Скорость с которой будет лететь стрела (пересылается в ArrowMovement)    
    //public float deltaVel = 10.0f;      // Приращение скорости
    //public Vector2 mouseMotion;         // Направление мыши
    //public float mouseSens = 5.0f;      // Чувствиельность мыши
    //public float delayTimer = 5.0f;     // Счетчик времени между выстрелами
    //public Vector2 AimAngleDiap =       // Диапазон минимального и максимального 
    //            new Vector2(55, 90);    //          значения угла прицеливания
    //private Vector2[] AimPoints;        // Пара точек для определения угла стрельбы
    //public float curAngle;              // Текущий угол
    //private Vector3 Forward;            // Вектор, смотрящий вперед (нужен для расчета направления прицела)
    //bool ChangeDirection = false;       // Проверка на смену направления
    //public Vector2 Len2Vel =
    //            new Vector2(1, 11);     // значение, при котором скорость стрелы будет максимальной

    //public float minVel = 200;         // Минимальная скорость пуска стрелы
    //public float maxVel = 2000;        // Максимальная скорость пуска стрелы
    //[Range(500, 1000)] public float delVel = 200;        // Скорость изменения скорости пуска стрелы (Скорость натяжения)
    //[Range(1, 15)] public float AttackDelay = 5.0f; // Время перезарядки  
    //private float bowSpriteAngle = 135;
    #endregion

    [SerializeField]
    private Health owner;

    List<Health> healthsDamaged = new List<Health>();
    public List<ItemParameters> parameters = new List<ItemParameters>();

    #region Properties

    // Проверяем, должны ли мы пригнуться (на земле и moveY указывает вниз, а moveX равно 0)
    public bool CanDuck
    {
        get
        {
            return onGround && moveX == 0 && moveY < 0 && !jumpIsInsideBuffer;
        }
    }

    // Проверяем, нужно ли / можем ли мы сделать рывок (кнопка рывка была нажата и завершилась ли перезарядка)
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
            return GetComponent<Mana>().mana > MeleeManaCost && meleeHitCount == 0 && activeWeapon == ActiveWeapon.Sword && Input.GetButtonDown("Attack") && meleeAttackCooldownTimer <= 0f;
        }
    }

    public bool CanSecondSwordAttack
    {
        get
        {
            return GetComponent<Mana>().mana > MeleeManaCost && meleeHasSeries && meleeHitCount == 1 && activeWeapon == ActiveWeapon.Sword && Input.GetButtonDown("Attack") && meleeAttackCooldownTimer <= 0f; // TODO secondtimeattck
        }
    }

    public bool CanThirdSwordAttack
    {
        get
        {
            return GetComponent<Mana>().mana > MeleeManaCost && meleeHasSeries && meleeHitCount == 2 && activeWeapon == ActiveWeapon.Sword && Input.GetButtonDown("Attack") && meleeAttackCooldownTimer <= 0f; // TODO secondtimeattck
        }
    }
    public bool CanPowerSwordAttack
    {
        get
        {
            return MeleePowerShellsCount > 0 && GetComponent<Mana>().mana > MeleePowerManaCost &&
            !hasBlock && activeWeapon == ActiveWeapon.Sword && Input.GetButtonDown("Attack2") && meleePowerAttackCooldownTimer <= 0f;
        }
    }
    public bool CanBlock
    {
        get
        {
            return hasBlock && activeWeapon == ActiveWeapon.Sword && Input.GetButtonDown("Attack2") && meleeBlockCooldownTimer <= 0f;
        }
    }
    public bool CanPrepare
    {
        get
        {
            return ShellsCount > 0 && GetComponent<Mana>().mana > RangedManaCost && activeWeapon == ActiveWeapon.Bow && Input.GetButtonDown("Attack")
            && rangedAttackCooldownTimer <= 0f;
        }
    }
    public bool CanShoot
    {
        get
        {
            return activeWeapon == ActiveWeapon.Bow && Input.GetButtonUp("Attack") && rangedPowerAccumulateTimer > 0f;
        }
    }
    public bool CanAccumulateShoot
    {
        get
        {
            return activeWeapon == ActiveWeapon.Bow && Input.GetButtonUp("Attack") && rangedPowerAccumulateTimer <= 0f;
        }
    }
    public bool CanSecondShoot
    {
        get
        {
            return rangedHasSeries && rangedHitCount == 1 && canAim && activeWeapon == ActiveWeapon.Bow && Input.GetButtonUp("Attack");
        }
    }
    public bool CanThirdShoot
    {
        get
        {
            return rangedHasSeries && rangedHitCount == 2 && canAim && activeWeapon == ActiveWeapon.Bow && Input.GetButtonUp("Attack");
        }
    }
    public bool CanPowerShoot
    {
        get
        {
            return activeWeapon == ActiveWeapon.Bow && Input.GetButtonUp("Attack2"); //&& canAim 
        }
    }

    public bool CanPowerShootPrepare
    {
        get
        {
            return RangedPowerShellsCount > 0 && GetComponent<Mana>().mana > RangedPowerManaCost && activeWeapon == ActiveWeapon.Bow
            && Input.GetButtonDown("Attack2") && rangedPowerAttackCooldownTimer <= 0f;
        }
    }


    public bool CanDuckPrepare
    {
        get
        {
            return moveX == 0 && moveY < 0 && !jumpIsInsideBuffer && GetComponent<Mana>().mana > RangedManaCost
            && activeWeapon == ActiveWeapon.Bow && Input.GetButtonDown("Attack") && rangedAttackCooldownTimer <= 0f;
        }
    }

    public bool CanDuckShoot
    {
        get
        {
            return moveX == 0 && moveY < 0 && !jumpIsInsideBuffer && ShellsCount > 0
            && activeWeapon == ActiveWeapon.Bow && Input.GetButtonUp("Attack"); //&& canAim 
        }
    }

    public bool CanDuckPowerShootPrepare
    {
        get
        {
            return moveX == 0 && moveY < 0 && !jumpIsInsideBuffer && GetComponent<Mana>().mana > RangedPowerManaCost && RangedPowerShellsCount > 0
            && activeWeapon == ActiveWeapon.Bow && Input.GetButtonDown("Attack2") && rangedPowerAttackCooldownTimer <= 0f;
        }
    }

    public bool CanDuckPowerShoot
    {
        get
        {
            return moveX == 0 && moveY < 0 && !jumpIsInsideBuffer && activeWeapon == ActiveWeapon.Bow && Input.GetButtonUp("Attack2");
            //&& canAim 
        }
    }
    #endregion

    [Header("Squash & Stretch")]
    public Transform SpriteHolder; // Ссылка на преобразование дочернего объекта, который содержит средство визуализации спрайтов игрока
    public Vector2 SpriteScale = Vector2.one; // Текущие масштабы X и Y держателя спрайта (используются для Squash & Stretch)

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
        Shoot,
        SecondShoot,
        ThirdShoot,
        BowPrepare,
        BowPowerAttack,
        BowPowerAttackPrepare,
        Roll,
        Action,
        PowerSwordAttack,
        DuckShoot,
        DuckPrepare,
        DuckPowerShoot,
        DuckPowerShootPrepare,
        Selection,
        Stun,
        AccumulateShoot
    }

    // State Machine
    public StateMachine<States> fsm;

    new void Awake()
    {
        base.Awake();
        fsm = StateMachine<States>.Initialize(this);
        aimSprite.SetActive(false);

        //AimLine.gameObject.SetActive(false);

        // Этот фрагмент кода необходим только для функции приседания
        // Назначение коллайдеров Ducking и Normal
        if (myNormalCollider == null && myDuckingCollider != null)
        {
            Debug.Log("The player has no Collider attached to it for the normal state");
        }
        else if (myDuckingCollider != null && myNormalCollider != null)
        {
            // Назначайте коллайдер только в том случае, если был назначен коллайдер приседания, поэтому делайте это только в том случае, если вы планируете использовать функцию приглушения
            myCollider = myNormalCollider;
        }

        // Следующее за игроком оружие
        FlyingWeapon = Instantiate(flyingWeapon, pointWeapon.position, Quaternion.identity);
        FlyingWeapon.GetComponent<AIDestinationSetter>().target = pointWeapon;
    }

    void Start()
    {
        fsm.ChangeState(States.LadderClimb);

        //отображение патрон
        countShellText = GameObject.FindGameObjectWithTag("ShellCount");
        countPowerShellText = GameObject.FindGameObjectWithTag("PowerShellCount");
        //countMeleePowerShellText = GameObject.FindGameObjectWithTag("PowerMeleeShellCount"); //TODO доделать

        countShellText.GetComponent<Text>().text = ShellsCount.ToString();
        countPowerShellText.GetComponent<Text>().text = RangedPowerShellsCount.ToString();
        //countMeleePowerShellText.GetComponent<Text>().text = MeleePowerShellsCount.ToString();

        // вызов оружия если оно есть
        if (InitialWeapon.GetComponent<PickupRangedWeapons>() != null)
            InitialWeapon.GetComponent<PickupRangedWeapons>().OnPlayer(this);

        if (InitialWeapon.GetComponent<PickupMeleeWeapons>() != null)
            InitialWeapon.GetComponent<PickupMeleeWeapons>().OnPlayer(this);

        ChangeWeapon(activeWeapon);
        curRun = MaxRun;

        StartCoroutine("ManaRegeneration"); // при старте запускать TODO
        inventory = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Inventory>();
    }

    private void ChangeWeapon(ActiveWeapon activeWeapon) // удалить вместе с вызовом TODO
    {
        if (activeWeapon == ActiveWeapon.Hands)
        {
            MeleeAttackMaxDamage = HandAttackMaxDamage;
            MeleeAttackMinDamage = HandAttackMinDamage;
            MeleeAttackCooldownTime = HandAttackCooldownTime;
            MeleeCriticalDamageMultiply = 0;
            MeleeCriticalDamageChance = 0;
            StepUpAfterHit = HandStepUpAfterHit;
            meleeHasSeries = false;
            hasBlock = false;

            FlyingWeapon.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        if (activeWeapon != ActiveWeapon.Sword)
        {
            FlyingWeapon.GetComponentInChildren<SpriteRenderer>().enabled = true;
        }

    }

    private void SelectWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && hasBow && hasSword)
        {
            switch (activeWeapon)
            {
                case ActiveWeapon.Sword:
                    hasPowerAttackShell = false;
                    FlyingWeapon.GetComponentInChildren<SpriteRenderer>().sprite = RangedWeapon.GetComponent<SpriteRenderer>().sprite;
                    //inventory.GetComponent<Inventory>().slots[0].GetComponent<Slot>().selectImage.SetActive(true);
                    //inventory.GetComponent<Inventory>().slots[1].GetComponent<Slot>().selectImage.SetActive(false);
                    activeWeapon = ActiveWeapon.Bow;
                    break;
                case ActiveWeapon.Bow:
                    hasPowerAttackShell = true;
                    FlyingWeapon.GetComponentInChildren<SpriteRenderer>().sprite = MeleeWeapon.GetComponent<SpriteRenderer>().sprite;
                    //inventory.GetComponent<Inventory>().slots[1].GetComponent<Slot>().selectImage.SetActive(true);
                    //inventory.GetComponent<Inventory>().slots[0].GetComponent<Slot>().selectImage.SetActive(false);
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

    new void Update()
    {
        base.Update();
        SelectWeapon();

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;

        float dist = Vector3.Distance(transform.position, FlyingWeapon.transform.position);

        if (dist > maxDist)
        {
            FlyingWeapon.transform.position = pointWeapon.position;
        }

        aimSprite.transform.position = Camera.main.ScreenToWorldPoint(mousePos);

        if (fsm.State == States.Respawn)
            return;

        // Update all collisions here
        wasOnGround = onGround;
        onGround = OnGround();

        //AimLine.transform.localPosition = new Vector3(transform.position.x, transform.position.y + 28, transform.position.z);

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

        if (turnCooldownTimer > 0f)
        {
            turnCooldownTimer -= Time.deltaTime;
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
        if (CanSecondShoot)
        {
            rangedAttackCooldownTimer = RangedAttackCooldownTime;
            //AimLine.gameObject.SetActive(false);
            fsm.ChangeState(States.SecondShoot, StateTransition.Overwrite);
            return;
        }

        if (CanThirdShoot)
        {
            rangedAttackCooldownTimer = RangedAttackCooldownTime;
            //AimLine.gameObject.SetActive(false);
            fsm.ChangeState(States.SecondShoot, StateTransition.Overwrite);
            return;
        }

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

        if (CanPrepare)
        {
            //rangedAttackCooldownTimer = RangedAttackCooldownTime;
            fsm.ChangeState(States.BowPrepare, StateTransition.Overwrite);
            return;
        }

        // Bow Power Attack over here
        // if (CanPowerShoot)
        //{
        //    rangedPowerAttackCooldownTimer = RangedPowerAttackCooldownTime;
        //    fsm.ChangeState(States.BowPowerAttack, StateTransition.Overwrite);
        //    return;
        //}

        // Bow Power Attack over here
        if (CanPowerShootPrepare)
        {
            rangedPowerAttackCooldownTimer = RangedPowerAttackCooldownTime;
            fsm.ChangeState(States.BowPowerAttackPrepare, StateTransition.Overwrite);
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
            var extraDistanceX = 16; // 2-4 recommended when tile size is 16 pixels, adjust accordingly
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

        if (CanDuckPrepare)
        {
            rangedAttackCooldownTimer = RangedAttackCooldownTime;
            //AimLine.gameObject.SetActive(false);
            fsm.ChangeState(States.DuckPrepare, StateTransition.Overwrite);
            return;
        }

        if (CanDuckPowerShootPrepare)
        {
            rangedAttackCooldownTimer = RangedAttackCooldownTime;
            fsm.ChangeState(States.DuckPowerShootPrepare, StateTransition.Overwrite);
            return;
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
        Vector2 vector = value * RollSpeed;
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

    void SwordAttack_Update()
    {
        // Horizontal Speed Update Section
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        float target = MaxFall;

        if (!onGround)
        {
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }

        if (OnAttackMove)
        {
            if (tossingUp)
                Speed.y = Calc.Approach(PopUpAfterHit, target, Gravity * Time.deltaTime);

            if (!sticking && !CheckColAtPlace(Vector2.right * (int)Facing, solid_layer))
            {
                var MoveH = MoveHPlatform(StepUpAfterHit * Time.deltaTime);
            }
            if (!onGround)
            {
                Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
            }
        }
        //if (meleeHasSeries)
        //    secondSwordAttackCooldownTimer = SecondSwordAttackCooldownTime;

        if (CanSecondSwordAttack)
        {
            secondSwordAttackCooldownTimer = SecondSwordAttackCooldownTime;
            fsm.ChangeState(States.SecondSwordAttack, StateTransition.Overwrite);
            return;
        }
    }

    void SwordAttack_Enter()
    {
        GetComponent<Mana>().TakeExpense(MeleeManaCost);
        FlyingWeapon.SetActive(true);
    }

    void SwordAttack_Exit()
    {
        OnAttackMove = false;
        StartCoroutine("DisapearCoroutine");
    }

    void SecondSwordAttack_Update()
    {
        // Horizontal Speed Update Section
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }

        if (OnAttackMove)
        {

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
        //thirdSwordAttackCooldownTimer = ThirdSwordAttackCooldownTime;

        if (CanThirdSwordAttack)
        {
            thirdSwordAttackCooldownTimer = ThirdSwordAttackCooldownTime;
            fsm.ChangeState(States.ThirdSwordAttack, StateTransition.Overwrite);
            return;
        }
    }

    void SecondSwordAttack_Exit()
    {
        GetComponent<Mana>().TakeExpense(MeleeManaCost);
        OnAttackMove = false;
    }

    void ThirdSwordAttack_Update()
    {
        // Horizontal Speed Update Section
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }

        if (OnAttackMove)
        {

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
    }

    void ThirdSwordAttack_Exit()
    {
        GetComponent<Mana>().TakeExpense(MeleeManaCost);
        meleeHitCount = 0;
        OnAttackMove = false;
    }

    void PowerSwordAttack_Update()
    {
        // Horizontal Speed Update Section
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        float target = MaxFall;

        if (OnAttackMove)
        {
            if (tossingUp)
                Speed.y = Calc.Approach(PopUpAfterHit, target, Gravity * Time.deltaTime);

            if (!sticking && !CheckColAtPlace(Vector2.right * (int)Facing, solid_layer))
            {
                var MoveH = MoveHPlatform(StepUpAfterHit * Time.deltaTime);
            }
            if (!onGround)
            {
                Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
            }
        }

        if (OnAttackMoveBack)
        {
            if (tossingUp)
                Speed.y = Calc.Approach(-PopUpAfterHit, target, Gravity * Time.deltaTime);

            if (!sticking && !CheckColAtPlace(Vector2.left * (int)Facing, solid_layer))
            {
                var MoveH = MoveHPlatform(-StepUpAfterHit * Time.deltaTime);
            }
            if (!onGround)
            {
                Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
            }
        }

        if (meleeHasSeries)
            secondSwordAttackCooldownTimer = SecondSwordAttackCooldownTime;

        if (!onGround)
        {
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
    }
    void PowerSwordAttack_Enter()
    {
        GetComponent<Mana>().TakeExpense(MeleePowerManaCost);
        FlyingWeapon.SetActive(true);
        PowerMeleeAttack = true;
        var col = GetComponentInChildren<HitBoxManager>();
        col.ChangePowerCollider((int)MeleeWeaponClass);
    }

    void PowerSwordAttack_Exit()
    {
        PowerMeleeAttack = false;
        var col = GetComponentInChildren<HitBoxManager>();
        col.ChangeCollider((int)MeleeWeaponClass);

        MeleeWeapon.GetComponent<PickupMeleeWeapons>().MeleePowerShellsCountCurent--;
        MeleePowerShellsCount--;
        StartCoroutine("DisapearCoroutine");
        //countMeleePowerShellText.GetComponent<Text>().text = MeleePowerShellsCount.ToString();
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


    void DuckPrepare_Update()
    {

        //if(canAim) // прицеливание
        //{
        //    AimLine.gameObject.SetActive(true);
        //    //mouseMotion.x += Input.GetAxis("Mouse X") * mouseSens;
        //    //mouseMotion.y += Input.GetAxis("Mouse Y") * mouseSens;
        //    //AimPoints[0] += mouseMotion;

        //    var facingDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - SpriteHolder.position;
        //    var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        //    var aimAngle2 = aimAngle * Mathf.Rad2Deg;
        //    //Debug.Log(aimAngle2);
        //    if (aimAngle < 0f)
        //    {
        //        aimAngle = Mathf.PI * 2 + aimAngle;
        //    }
        //    bool Flip = ((Mathf.Abs(aimAngle2) > 91) && (transform.localScale.x > 0) || (Mathf.Abs(aimAngle2) < 89) && (transform.localScale.x < 0));
        //    if (Flip)
        //    {
        //        if (Facing != (Facings)(-1))
        //            Facing = (Facings)(-1);
        //        else
        //            Facing = (Facings)(1);
        //    }
        //    AimLine.transform.rotation = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg);
        //    var aimAngle3 = aimAngle - 89.6f;
        //    var aimAngle4 = 180;
        //    //Bow.transform.rotation = Quaternion.Euler(aimAngle4, aimAngle4, aimAngle3 * Mathf.Rad2Deg - bowSpriteAngle);
        //    if (delayTimer >= AttackDelay)
        //    {
        //        if (Velocity < maxVel)
        //        {
        //            Velocity += delVel * Time.deltaTime;
        //            //Debug.Log("Скорость: " + Velocity);
        //        }
        //        else
        //        {
        //            Velocity = maxVel;
        //        }
        //    }
        //}
        //else
        //{
        //    /*if (Input.GetMouseButtonUp(0))
        //    {
        //        Instantiate(Arrow, AimPosition, AimLine.transform.rotation);
        //        delayTimer = 0;
        //    }*/

        //    if (ChangeDirection)
        //    {
        //        //AimLine.transform.localScale = new Vector3(-AimLine.transform.localScale.x, AimLine.transform.localScale.y, AimLine.transform.localScale.z);
        //        //AimPoints[1] = -AimPoints[1];
        //        ChangeDirection = !ChangeDirection;
        //    }
        //    AimLine.transform.Rotate(Vector3.forward, -AimLine.transform.localEulerAngles.z);
        //    AimLine.gameObject.SetActive(false);
        //}
        //mouseMotion = Vector2.zero;
        //delayTimer += Time.deltaTime;

        // Bow Attack over here
        if (CanDuckShoot)
        {
            //AimLine.gameObject.SetActive(false);
            fsm.ChangeState(States.DuckShoot, StateTransition.Overwrite);
            return;
        }

        if (!(onGround && moveX == 0 && moveY < 0 && !jumpIsInsideBuffer))
        {
            //AimLine.gameObject.SetActive(false);
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            return;
        }

        // Horizontal Speed Update Section
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
    }

    void DuckPrepare_Enter()
    {
        GetComponent<Mana>().TakeExpense(RangedManaCost);
        FlyingWeapon.SetActive(false);
    }

    void DuckShoot_Update()
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

    void DuckShoot_Exit()
    {
        RangedWeapon.GetComponent<PickupRangedWeapons>().RangedShellsCountCurent--;
        ShellsCount--;
        countShellText.GetComponent<Text>().text = ShellsCount.ToString();
        StartCoroutine("DisapearCoroutine");
        canAim = false;
    }

    void DuckPowerShootPrepare_Update()
    {

        //if(canAim) // прицеливание
        //{
        //    AimLine.gameObject.SetActive(true);
        //    //mouseMotion.x += Input.GetAxis("Mouse X") * mouseSens;
        //    //mouseMotion.y += Input.GetAxis("Mouse Y") * mouseSens;
        //    //AimPoints[0] += mouseMotion;

        //    var facingDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - SpriteHolder.position;
        //    var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        //    var aimAngle2 = aimAngle * Mathf.Rad2Deg;
        //    //Debug.Log(aimAngle2);
        //    if (aimAngle < 0f)
        //    {
        //        aimAngle = Mathf.PI * 2 + aimAngle;
        //    }
        //    bool Flip = ((Mathf.Abs(aimAngle2) > 91) && (transform.localScale.x > 0) || (Mathf.Abs(aimAngle2) < 89) && (transform.localScale.x < 0));
        //    if (Flip)
        //    {
        //        if (Facing != (Facings)(-1))
        //            Facing = (Facings)(-1);
        //        else
        //            Facing = (Facings)(1);
        //    }
        //    AimLine.transform.rotation = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg);
        //    var aimAngle3 = aimAngle - 89.6f;
        //    var aimAngle4 = 180;
        //    //Bow.transform.rotation = Quaternion.Euler(aimAngle4, aimAngle4, aimAngle3 * Mathf.Rad2Deg - bowSpriteAngle);
        //    if (delayTimer >= AttackDelay)
        //    {
        //        if (Velocity < maxVel)
        //        {
        //            Velocity += delVel * Time.deltaTime;
        //            //Debug.Log("Скорость: " + Velocity);
        //        }
        //        else
        //        {
        //            Velocity = maxVel;
        //        }
        //    }
        //}
        //else
        //{
        //    /*if (Input.GetMouseButtonUp(0))
        //    {
        //        Instantiate(Arrow, AimPosition, AimLine.transform.rotation);
        //        delayTimer = 0;
        //    }*/

        //    if (ChangeDirection)
        //    {
        //        //AimLine.transform.localScale = new Vector3(-AimLine.transform.localScale.x, AimLine.transform.localScale.y, AimLine.transform.localScale.z);
        //        //AimPoints[1] = -AimPoints[1];
        //        ChangeDirection = !ChangeDirection;
        //    }
        //    AimLine.transform.Rotate(Vector3.forward, -AimLine.transform.localEulerAngles.z);
        //    AimLine.gameObject.SetActive(false);
        //}
        //mouseMotion = Vector2.zero;
        //delayTimer += Time.deltaTime;

        if (RangedPowerAttackAiming || RangedPowerBlink)
        {
            aimSprite.SetActive(true);

            var facingDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - SpriteHolder.position;
            var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
            var aimAngle2 = aimAngle * Mathf.Rad2Deg;

            bool Flip = ((Mathf.Abs(aimAngle2) > 95) && (transform.localScale.x > 1) || (Mathf.Abs(aimAngle2) < 85) && (transform.localScale.x < -1));
            if (Flip)
            {
                if (Facing != (Facings)(-1))
                    Facing = (Facings)(-1);
                else
                    Facing = (Facings)(1);
            }

            if ((Mathf.Abs(aimAngle2) < 90) && (Mathf.Abs(aimAngle2) > 75))
                Debug.Log("angle 90 - 75");
            if ((Mathf.Abs(aimAngle2) < 75) && (Mathf.Abs(aimAngle2) > 60))
                Debug.Log("angle 75 - 60");
            if ((Mathf.Abs(aimAngle2) < 60) && (Mathf.Abs(aimAngle2) > 45))
                Debug.Log("angle 60 - 45");
            if ((Mathf.Abs(aimAngle2) < 45) && (Mathf.Abs(aimAngle2) > 30))
                Debug.Log("angle 45 - 30");
            if ((Mathf.Abs(aimAngle2) < 30) && (Mathf.Abs(aimAngle2) > 15))
                Debug.Log("angle 30 - 15");
            if ((Mathf.Abs(aimAngle2) < 15) && (Mathf.Abs(aimAngle2) > 0))
                Debug.Log("angle 15 - 0");
        }

        // Bow Attack over here
        if (CanDuckPowerShoot)
        {
            //AimLine.gameObject.SetActive(false);
            fsm.ChangeState(States.DuckPowerShoot, StateTransition.Overwrite);
            return;
        }

        if (!(onGround && moveX == 0 && moveY < 0 && !jumpIsInsideBuffer))
        {
            //AimLine.gameObject.SetActive(false);
            fsm.ChangeState(States.Normal, StateTransition.Overwrite);
            return;
        }

        // Horizontal Speed Update Section
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
    }

    void DuckPowerShootPrepare_Enter()
    {
        GetComponent<Mana>().TakeExpense(RangedPowerManaCost);
        FlyingWeapon.SetActive(false);
    }

    void DuckPowerShoot_Update()
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

    void DuckPowerShoot_Exit()
    {
        RangedWeapon.GetComponent<PickupRangedWeapons>().RangedPowerShellsCountCurent--;
        RangedPowerShellsCount--;
        countPowerShellText.GetComponent<Text>().text = RangedPowerShellsCount.ToString();
        StartCoroutine("DisapearCoroutine");
    }


    void BowPrepare_Update()
    {

        //if(canAim) // прицеливание
        //{
        //    AimLine.gameObject.SetActive(true);
        //    //mouseMotion.x += Input.GetAxis("Mouse X") * mouseSens;
        //    //mouseMotion.y += Input.GetAxis("Mouse Y") * mouseSens;
        //    //AimPoints[0] += mouseMotion;

        //    var facingDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - SpriteHolder.position;
        //    var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        //    var aimAngle2 = aimAngle * Mathf.Rad2Deg;
        //    //Debug.Log(aimAngle2);
        //    if (aimAngle < 0f)
        //    {
        //        aimAngle = Mathf.PI * 2 + aimAngle;
        //    }
        //    bool Flip = ((Mathf.Abs(aimAngle2) > 91) && (transform.localScale.x > 0) || (Mathf.Abs(aimAngle2) < 89) && (transform.localScale.x < 0));
        //    if (Flip)
        //    {
        //        if (Facing != (Facings)(-1))
        //            Facing = (Facings)(-1);
        //        else
        //            Facing = (Facings)(1);
        //    }
        //    AimLine.transform.rotation = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg);
        //    var aimAngle3 = aimAngle - 89.6f;
        //    var aimAngle4 = 180;
        //    //Bow.transform.rotation = Quaternion.Euler(aimAngle4, aimAngle4, aimAngle3 * Mathf.Rad2Deg - bowSpriteAngle);
        //    if (delayTimer >= AttackDelay)
        //    {
        //        if (Velocity < maxVel)
        //        {
        //            Velocity += delVel * Time.deltaTime;
        //            //Debug.Log("Скорость: " + Velocity);
        //        }
        //        else
        //        {
        //            Velocity = maxVel;
        //        }
        //    }
        //}
        //else
        //{
        //    /*if (Input.GetMouseButtonUp(0))
        //    {
        //        Instantiate(Arrow, AimPosition, AimLine.transform.rotation);
        //        delayTimer = 0;
        //    }*/

        //    if (ChangeDirection)
        //    {
        //        //AimLine.transform.localScale = new Vector3(-AimLine.transform.localScale.x, AimLine.transform.localScale.y, AimLine.transform.localScale.z);
        //        //AimPoints[1] = -AimPoints[1];
        //        ChangeDirection = !ChangeDirection;
        //    }
        //    AimLine.transform.Rotate(Vector3.forward, -AimLine.transform.localEulerAngles.z);
        //    AimLine.gameObject.SetActive(false);
        //}
        //mouseMotion = Vector2.zero;
        //delayTimer += Time.deltaTime;

        //aimSprite.SetActive(true);


        //if ((Mathf.Abs(aimAngle2) < 90) && (Mathf.Abs(aimAngle2) > 45))
        //             Debug.Log("angle 90 - 45");
        //         if ((Mathf.Abs(aimAngle2) < 45) && (Mathf.Abs(aimAngle2) > 0))
        //             Debug.Log("angle 45 - 0");
        //         if ((Mathf.Abs(aimAngle2) < 0) && (Mathf.Abs(aimAngle2) > -45))
        //             Debug.Log("angle 0 - -45");
        //if ((Mathf.Abs(aimAngle2) < -45) && (Mathf.Abs(aimAngle2) > -90))
        //             Debug.Log("angle -45 - -90");

        //var facingDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - SpriteHolder.position;
        //var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        //var aimAngle2 = aimAngle * Mathf.Rad2Deg;

        //bool Flip = ((Mathf.Abs(aimAngle2) > 92) && (transform.localScale.x > 1) || (Mathf.Abs(aimAngle2) < 88) && (transform.localScale.x < -1));
        //if (Flip)
        //{
        //    if (Facing != (Facings)(-1))
        //        Facing = (Facings)(-1);
        //    else
        //        Facing = (Facings)(1);
        //}

        //Bow Attack over here
        if (CanShoot)
        {
            rangedAttackCooldownTimer = RangedAttackCooldownTime;
            //AimLine.gameObject.SetActive(false);
            fsm.ChangeState(States.Shoot, StateTransition.Overwrite);
            return;
        }

        if (rangedPowerAccumulateTimer > 0f)
        {
            rangedPowerAccumulateTimer -= Time.deltaTime;
        }

        if (CanAccumulateShoot)
        {
            fsm.ChangeState(States.AccumulateShoot, StateTransition.Overwrite);
            return;
        }

        // Horizontal Speed Update Section
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }
    }

    void BowPrepare_Enter()
    {
        rangedPowerAccumulateTimer = RangedPowerAccumulateTime;
        GetComponent<Mana>().TakeExpense(RangedManaCost);
        FlyingWeapon.SetActive(false);
    }

    void Shoot_Update()
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

    void Shoot_Exit()
    {
        RangedWeapon.GetComponent<PickupRangedWeapons>().RangedShellsCountCurent--;
        ShellsCount--;
        countShellText.GetComponent<Text>().text = ShellsCount.ToString();
        //aimSprite.SetActive(false);
        StartCoroutine("DisapearCoroutine");
        canAim = false;
    }

    void AccumulateShoot_Update()
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

    void AccumulateShoot_Exit()
    {
        Debug.Log("AccumulateShoot");
        RangedWeapon.GetComponent<PickupRangedWeapons>().RangedShellsCountCurent--;
        ShellsCount--;
        countShellText.GetComponent<Text>().text = ShellsCount.ToString();
        //aimSprite.SetActive(false);
        StartCoroutine("DisapearCoroutine");
        canAim = false;
    }

    void SecondShoot_Update()
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

    void SecondShoot_Enter()
    {
        GetComponent<Mana>().TakeExpense(RangedManaCost);
        FlyingWeapon.SetActive(false);
    }

    void SecondShoot_Exit()
    {
        RangedWeapon.GetComponent<PickupRangedWeapons>().RangedShellsCountCurent--;
        ShellsCount--;
        countShellText.GetComponent<Text>().text = ShellsCount.ToString();
        //aimSprite.SetActive(false);
        StartCoroutine("DisapearCoroutine");
        canAim = false;
    }

    void ThirdShoot_Update()
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

    void ThirdShoot_Enter()
    {
        GetComponent<Mana>().TakeExpense(RangedManaCost);
        FlyingWeapon.SetActive(false);
    }

    void ThirdShoot_Exit()
    {
        rangedHitCount = 0;

        RangedWeapon.GetComponent<PickupRangedWeapons>().RangedShellsCountCurent--;
        ShellsCount--;
        countShellText.GetComponent<Text>().text = ShellsCount.ToString();
        //aimSprite.SetActive(false);
        StartCoroutine("DisapearCoroutine");
        canAim = false;
    }

    void BowPowerAttackPrepare_Update()
    {
        // Horizontal Speed Update Section
        float num = onGround ? 1f : AirMult;

        Speed.x = Calc.Approach(Speed.x, 0f, RunReduce * num * Time.deltaTime);

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }

        //if (canAim) // прицеливание
        //{
        //AimLine.gameObject.SetActive(true);
        //mouseMotion.x += Input.GetAxis("Mouse X") * mouseSens;
        //mouseMotion.y += Input.GetAxis("Mouse Y") * mouseSens;
        //AimPoints[0] += mouseMotion;

        if (RangedPowerAttackAiming || RangedPowerBlink)
        {
            aimSprite.SetActive(true);

            var facingDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - SpriteHolder.position;
            var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
            var aimAngle2 = aimAngle * Mathf.Rad2Deg;

            bool Flip = ((Mathf.Abs(aimAngle2) > 95) && (transform.localScale.x > 1) || (Mathf.Abs(aimAngle2) < 85) && (transform.localScale.x < -1));
            if (Flip)
            {
                if (Facing != (Facings)(-1))
                    Facing = (Facings)(-1);
                else
                    Facing = (Facings)(1);
            }

            if ((Mathf.Abs(aimAngle2) < 90) && (Mathf.Abs(aimAngle2) > 75))
            {
                Debug.Log("angle 90 - 75");
                animator.Play((string)RangedWeaponType + "angle75");
            }
            if ((Mathf.Abs(aimAngle2) < 75) && (Mathf.Abs(aimAngle2) > 60))
            {
                Debug.Log("angle 75 - 60");
                animator.Play((string)RangedWeaponType + "angle60");
            }
            if ((Mathf.Abs(aimAngle2) < 60) && (Mathf.Abs(aimAngle2) > 45))
            {
                Debug.Log("angle 60 - 45");
                animator.Play((string)RangedWeaponType + "angle45");
            }
            if ((Mathf.Abs(aimAngle2) < 45) && (Mathf.Abs(aimAngle2) > 30))
            {
                Debug.Log("angle 45 - 30");
                animator.Play((string)RangedWeaponType + "angle30");
            }
            if ((Mathf.Abs(aimAngle2) < 30) && (Mathf.Abs(aimAngle2) > 15))
            {
                Debug.Log("angle 30 - 15");
                animator.Play((string)RangedWeaponType + "angle15");
            }
            if ((Mathf.Abs(aimAngle2) < 15) && (Mathf.Abs(aimAngle2) > 0))
            {
                Debug.Log("angle 15 - 0");
                animator.Play((string)RangedWeaponType + "angle0");
            }
        }
        // Bow Attack over here
        if (CanPowerShoot)
        {
            //AimLine.gameObject.SetActive(false);
            fsm.ChangeState(States.BowPowerAttack, StateTransition.Overwrite);
            return;
        }

    }

    void BowPowerAttackPrepare_Enter()
    {
        GetComponent<Mana>().TakeExpense(RangedManaCost);
        FlyingWeapon.SetActive(false);
        PowerRangedAttack = true;
        if (RangedPowerAttackAiming || RangedPowerBlink)
        {
            aimSprite.GetComponent<SpriteRenderer>().sprite = AimSpriteGreen;
        }
    }

    void BowPowerAttackPrepare_Exit()
    {
        PowerRangedAttack = false;
        aimSprite.SetActive(false);
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

    void BowPowerAttack_Exit()
    {
        RangedWeapon.GetComponent<PickupRangedWeapons>().RangedPowerShellsCountCurent--;
        RangedPowerShellsCount--;
        countPowerShellText.GetComponent<Text>().text = RangedPowerShellsCount.ToString();
        StartCoroutine("DisapearCoroutine");
    }

    IEnumerator DisapearCoroutine()
    {
        FlyingWeapon.SetActive(true);
        yield return new WaitForSeconds(timeDisapear);
        FlyingWeapon.SetActive(false);
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

    void Stun_Update()
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
    //    meleeHasSeries = false;
    //    hasBlock = false;
    //}

    public bool PickUpMeleeWeapon(PickupMeleeWeapons meleeWeapon)
    {
        if (!hasSword)
        {
            fsm.ChangeState(States.Selection, StateTransition.Overwrite); //???
                                                                          // сброс параметров
            meleePowerAttackCooldownTimer = 0; // new

            MeleeWeaponClass = meleeWeapon.Type;
            MeleeAttackMinDamage = meleeWeapon.MeleeAttackMinDamage;
            MeleeAttackMaxDamage = meleeWeapon.MeleeAttackMaxDamage;
            MeleeAttackCooldownTime = meleeWeapon.MeleeAttackCooldownTime;
            MeleeCriticalDamageMultiply = meleeWeapon.MeleeCriticalDamageMultiply;
            MeleeCriticalDamageChance = meleeWeapon.MeleeChanceCriticalDamage;
            StepUpAfterHit = meleeWeapon.StepUpAfterHit;
            MeleeCanThirdAttackCriticalDamage = meleeWeapon.CanThirdAttackCriticalDamage;
            meleeHasSeries = meleeWeapon.hasSeries;
            hasBlock = meleeWeapon.hasBlock;
            hasPowerAttackShell = meleeWeapon.hasPowerAttackShell;

            MeleePowerAttackMinDamage = meleeWeapon.MeleePowerAttackMinDamage;
            MeleePowerAttackMaxDamage = meleeWeapon.MeleePowerAttackMaxDamage;
            MeleePowerAttackCooldownTime = meleeWeapon.MeleePowerAttackCooldownTime;
            MeleePowerCriticalDamageMultiply = meleeWeapon.MeleePowerCriticalDamageMultiply;
            MeleePowerChanceCriticalDamage = meleeWeapon.MeleePowerChanceCriticalDamage;

            MeleeAttackCanPoison = meleeWeapon.CanPoison;
            MeleePowerAttackCanPoison = meleeWeapon.CanPowerAttackPoison;
            MeleePoisonDamaged = meleeWeapon.PoisonDamaged;
            MeleePoisonFrequency = meleeWeapon.PoisonFrequency;
            MeleePoisonTick = meleeWeapon.PoisonTick;
            MeleePoisonChance = meleeWeapon.PoisonChance;

            MeleeAttackCanFire = meleeWeapon.CanFire;
            MeleePowerAttackCanFire = meleeWeapon.CanPowerAttackFire;
            MeleeFireDamaged = meleeWeapon.FireDamaged;
            MeleeFireFrequency = meleeWeapon.FireFrequency;
            MeleeFireTick = meleeWeapon.FireTick;
            MeleeFireChance = meleeWeapon.FireChance;

            MeleeAttackCanFreez = meleeWeapon.CanFreez;
            MeleePowerAttackCanFreez = meleeWeapon.CanPowerAttackFreez;
            MeleeFreezDuration = meleeWeapon.FreezDuration;
            MeleeFreezChance = meleeWeapon.FreezChance;

            MeleeAttackCanPush = meleeWeapon.CanPush;
            MeleePowerAttackCanPush = meleeWeapon.CanPowerAttackPush;
            MeleePushDistance = meleeWeapon.PushDistance;

            MeleeAttackCanPushUp = meleeWeapon.CanPushUp;
            MeleePowerAttackCanPushUp = meleeWeapon.CanPowerAttackPushUp;
            MeleePushUpDistance = meleeWeapon.PushUpDistance;

            //MeleeAttackCanStun = meleeWeapon;
            //MeleePowerAttackCanStun = meleeWeapon;
            //MeleeStunDuration = meleeWeapon;
            //MeleeStunChance = meleeWeapon;

            MeleeManaCost = meleeWeapon.ManaCost;
            MeleePowerManaCost = meleeWeapon.PowerManaCost;
            MeleePowerShellsCount = meleeWeapon.MeleePowerShellsCountCurent;

            MeleeAttackSpeed = meleeWeapon.MeleeAttackSpeed;
            SecondSwordAttackSpeed = meleeWeapon.SecondSwordAttackSpeed;
            ThirdSwordAttackSpeed = meleeWeapon.ThirdSwordAttackSpeed;
            MeleePowerAttackSpeed = meleeWeapon.MeleePowerAttackSpeed;

            tossingUp = meleeWeapon.MeleeTossingUp;
            PopUpAfterHit = meleeWeapon.MeleePopUpAfterHit;

            MeleeWeapon = meleeWeapon;

            activeWeapon = ActiveWeapon.Sword;
            hasSword = true;

            var col = GetComponentInChildren<HitBoxManager>();
            col.ChangeCollider((int)meleeWeapon.Type);

            //col.ChangeCollider((int)typepower); // отдельные колайдеры для power attack // TODO

            FlyingWeapon.GetComponentInChildren<SpriteRenderer>().sprite = meleeWeapon.spriteNormal;

            for (int i = 0; i < parameters.Count; i++)
            {
                DropItem(parameters[i]);
            }

            for (int i = 0; i < parameters.Count; i++)
            {
                TakeItem(parameters[i]);
            }

            MeleeWeaponType = meleeWeapon.Type.ToString();
            return false;
        }
        return true;
    }
    public bool PickUpRangedWeapon(PickupRangedWeapons rangedWeapon)
    {
        if (!hasBow)
        {
            fsm.ChangeState(States.Selection, StateTransition.Overwrite); //???

            RangedWeaponClass = rangedWeapon.Type;

            rangedPowerAttackCooldownTimer = 0; // new

            RangedAttackMinDamage = rangedWeapon.RangedAttackMinDamage;
            RangedAttackMaxDamage = rangedWeapon.RangedAttackMaxDamage;
            ShellsCount = rangedWeapon.RangedShellsCountCurent;
            RangedAttackCooldownTime = rangedWeapon.RangedAttackCooldownTime;
            RangedCriticalDamageMultiply = rangedWeapon.RangedAttackCriticalDamageMultiply;
            RangedCriticalDamageChance = rangedWeapon.RangedAttackChanceCriticalDamage;

            RangedPowerAttackMinDamage = rangedWeapon.RangedPowerAttackMinDamage;
            RangedPowerAttackMaxDamage = rangedWeapon.RangedPowerAttackMaxDamage;
            RangedPowerAttackCooldownTime = rangedWeapon.RangedPowerAttackCooldownTime;
            RangedPowerCriticalDamageMultiply = rangedWeapon.RangedPowerCriticalDamageMultiply;
            RangedPowerChanceCriticalDamage = rangedWeapon.RangedPowerChanceCriticalDamage;
            RangedPowerShellsCount = rangedWeapon.RangedPowerShellsCountCurent;

            RangedAttackCanPoison = rangedWeapon.CanPoison;
            RangedPowerAttackCanPoison = rangedWeapon.CanPowerAttackPoison;
            RangedPoisonDamaged = rangedWeapon.PoisonDamaged;
            RangedPoisonFrequency = rangedWeapon.PoisonFrequency;
            RangedPoisonTick = rangedWeapon.PoisonTick;
            RangedPoisonChance = rangedWeapon.PoisonChance;

            RangedAttackCanFire = rangedWeapon.CanFire;
            RangedPowerAttackCanFire = rangedWeapon.CanPowerAttackFire;
            RangedFireDamaged = rangedWeapon.FireDamaged;
            RangedFireFrequency = rangedWeapon.FireFrequency;
            RangedFireTick = rangedWeapon.FireTick;
            RangedFireChance = rangedWeapon.FireChance;

            RangedAttackCanFreez = rangedWeapon.CanFreez;
            RangedPowerAttackCanFreez = rangedWeapon.CanPowerAttackFreez;
            RangedFreezDuration = rangedWeapon.FreezDuration;
            RangedFreezChance = rangedWeapon.FreezChance;

            RangedAttackCanPush = rangedWeapon.CanPush;
            RangedPowerAttackCanPush = rangedWeapon.CanPowerAttackPush;
            RangedPushDistance = rangedWeapon.PushDistance;

            RangedAttackCanPushUp = rangedWeapon.CanPushUp;
            RangedPowerAttackCanPushUp = rangedWeapon.CanPowerAttackPushUp;
            RangedPushUpDistance = rangedWeapon.PushUpDistance;

            //RangedAttackCanStun = rangedWeapon. ;
            //RangedPowerAttackCanStun = rangedWeapon. ;
            //RangedStunDuration = rangedWeapon. ;
            //RangedStunChance = rangedWeapon. ;

            RangedAttackCanThroughShoot = rangedWeapon.CanThroughShoot;
            RangedPowerAttackCanThroughShoot = rangedWeapon.CanPowerAttackThroughShoot;
            RangedAttackAiming = rangedWeapon.Aiming;
            RangedPowerAttackAiming = rangedWeapon.PowerAttackAiming;
            RangedPowerBlink = rangedWeapon.RangedBlink;
            RangedHeal = rangedWeapon.RangedHeal;
            RangedHealCount = rangedWeapon.RangedHealCount;
            RangedManaCost = rangedWeapon.ManaCost;
            RangedPowerManaCost = rangedWeapon.PowerManaCost;

            RangedAttackSpeed = rangedWeapon.RangedAttackSpeed;
            RangedPowerAttackSpeed = rangedWeapon.RangedPowerAttackSpeed;

            RangedWeapon = rangedWeapon;

            activeWeapon = ActiveWeapon.Bow;
            hasBow = true;
            countShellText.GetComponent<Text>().text = ShellsCount.ToString();
            countPowerShellText.GetComponent<Text>().text = RangedPowerShellsCount.ToString();

            var projectile = GetComponentInChildren<ProjectileSpawner>();
            projectile.ChangeProjectile((int)rangedWeapon.Type);

            FlyingWeapon.GetComponentInChildren<SpriteRenderer>().sprite = rangedWeapon.spriteNormal;

            for (int i = 0; i < parameters.Count; i++)
            {
                DropItem(parameters[i]);
            }

            for (int i = 0; i < parameters.Count; i++)
            {
                TakeItem(parameters[i]);
            }

            RangedWeaponType = rangedWeapon.Type.ToString();
            return false;
        }
        return true;
    }

    public void DropMeleeWeapon()
    {
        for (int i = 0; i < parameters.Count; i++)
        {
            DropItem(parameters[i]);
        }

        for (int i = 0; i < parameters.Count; i++)
        {
            TakeItem(parameters[i]);
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
        RangedWeapon = null;

        for (int i = 0; i < parameters.Count; i++)
        {
            DropItem(parameters[i]);
        }

        for (int i = 0; i < parameters.Count; i++)
        {
            TakeItem(parameters[i]);
        }

        if (hasSword)
        {
            activeWeapon = ActiveWeapon.Sword;
        }
        else activeWeapon = ActiveWeapon.Hands;
        hasBow = false;
    }

    public void PickUpArtifacts(ItemParameters item)
    {
        parameters.Add(item);
        TakeItem(item);

        for (int j = 0; j < item.ArtifactsType.Length; j++)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (item.ArtifactsType.Length != 0)
                    if (parameters[i].Type.ToString() == item.ArtifactsType[j].ToString())
                    {
                        {
                            TakeItem(item.StacksArtifacts[j]);
                            Debug.Log("plus param");
                        }
                    }
            }
        }

    }

    public bool TakeItem(ItemParameters item) // TODO переделать логику
    {
        Gravity = Gravity + item.Gravity;
        MaxFall = MaxFall + item.MaxFall;
        FastFall = FastFall + item.FastFall;
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
        RollSpeed = RollSpeed + item.RollSpeed;
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

        MeleeAttackMaxDamage = MeleeAttackMaxDamage + item.MeleeAttackMaxDamage;
        MeleeAttackMinDamage = MeleeAttackMinDamage + item.MeleeAttackMinDamage;
        MeleeCriticalDamageMultiply = MeleeCriticalDamageMultiply + item.MeleeCriticalDamageMultiply;
        MeleeCriticalDamageChance = MeleeCriticalDamageChance + item.MeleeCriticalDamageChance;
        StepUpAfterHit = StepUpAfterHit + item.StepUpAfterHit;
        MeleeAttackCooldownTime = MeleeAttackCooldownTime + item.MeleeAttackCooldownTime;
        MeleePowerAttackMaxDamage = MeleePowerAttackMaxDamage + item.MeleePowerAttackMaxDamage;
        MeleePowerAttackMinDamage = MeleePowerAttackMinDamage + item.MeleePowerAttackMinDamage;

        //MeleeAttackCanPowerAttackCriticalDamage = item.MeleeAttackCanPowerAttackCriticalDamage;
        MeleeCanThirdAttackCriticalDamage = item.MeleeAttackCanThirdAttackCriticalDamage;
        if (!MeleeAttackCanPoison)
            MeleeAttackCanPoison = item.MeleeAttackCanPoison;
        MeleePoisonDamaged = MeleePoisonDamaged + item.MeleePoisonDamaged;
        MeleePoisonFrequency = MeleePoisonFrequency + item.MeleePoisonFrequency;
        MeleePoisonTick = MeleePoisonTick + item.MeleePoisonTick;
        MeleePoisonChance = item.MeleePoisonChance;
        if (!MeleeAttackCanFire)
            MeleeAttackCanFire = item.MeleeAttackCanFire;
        MeleeFireDamaged = MeleeFireDamaged + item.MeleeFireDamaged;
        MeleeFireFrequency = MeleeFireFrequency + item.MeleeFireFrequency;
        MeleeFireTick = MeleeFireTick + item.MeleeFireTick;
        MeleeFireChance = item.MeleeFireChance;
        if (!MeleeAttackCanPush)
            MeleeAttackCanPush = item.MeleeAttackCanPush;
        if (!MeleeAttackCanFreez)
            MeleeAttackCanFreez = item.MeleeAttackCanFreez;
        MeleeFreezDuration = MeleeFreezDuration + item.MeleeFreezDuration;
        MeleeFreezChance = item.MeleeFreezChance;
        MeleePushDistance = MeleePushDistance + item.MeleePushDistance;
        MeleeBlockCooldownTime = MeleeBlockCooldownTime + item.MeleeBlockCooldownTime;

        RangedAttackMinDamage = RangedAttackMinDamage + item.RangedAttackMinDamage;
        RangedAttackMaxDamage = RangedAttackMaxDamage + item.RangedAttackMaxDamage;
        RangedAttackCooldownTime = RangedAttackCooldownTime + item.RangedAttackCooldownTime;
        RangedPowerAttackCooldownTime = RangedPowerAttackCooldownTime + item.RangedPowerAttackCooldownTime;
        RangedCriticalDamageMultiply = RangedCriticalDamageMultiply + item.RangedCriticalDamageMultiply;
        RangedCriticalDamageChance = RangedCriticalDamageChance + item.RangedCriticalDamageChance;
        RangedCriticalDamage = RangedCriticalDamage + item.RangedCriticalDamage;

        if (!RangedAttackCanPoison)
            RangedAttackCanPoison = item.RangedAttackCanPoison;
        RangedPoisonDamaged = RangedPoisonDamaged + item.RangedPoisonDamaged;
        RangedPoisonFrequency = RangedPoisonFrequency + item.RangedPoisonFrequency;
        RangedPoisonTick = RangedPoisonTick + item.RangedPoisonTick;
        RangedPoisonChance = item.RangedPoisonChance;
        if (!RangedAttackCanFire)
            RangedAttackCanFire = item.RangedAttackCanFire;
        RangedFireChance = item.RangedFireChance;
        RangedFireDamaged = RangedFireDamaged + item.RangedFireDamaged;
        RangedFireFrequency = RangedFireFrequency + item.RangedFireFrequency;
        RangedFireTick = RangedFireTick + item.RangedFireTick;
        if (!RangedAttackCanFreez)
            RangedAttackCanFreez = item.RangedAttackCanFreez;
        RangedFreezDuration = RangedFreezDuration + item.RangedFreezDuration;
        RangedFreezChance = item.RangedFreezChance;
        if (!RangedAttackCanPush)
            RangedAttackCanPush = item.RangedAttackCanPush;
        if (!RangedAttackCanThroughShoot)
            RangedAttackCanThroughShoot = item.RangedAttackCanThroughShoot;

        return true;
    }

    public void DropItem(ItemParameters item) // TODO переделать логику
    {
        Gravity = Gravity - item.Gravity;
        MaxFall = MaxFall - item.MaxFall;
        FastFall = FastFall - item.FastFall;
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
        RollSpeed = RollSpeed - item.RollSpeed;
        EndDashSpeed = EndDashSpeed - item.EndDashSpeed;
        EndRollSpeed = EndRollSpeed - item.EndRollSpeed;
        EndDashUpMult = EndDashUpMult - item.EndDashUpMult;
        EndRollUpMult = EndRollUpMult - item.EndRollUpMult;
        DashTime = DashTime - item.DashTime;
        RollTime = RollTime - item.RollTime;
        DashCooldown = DashCooldown - item.DashCooldown;
        RollCooldown = -item.RollCooldown;
        clingTime = RollCooldown - item.clingTime;
        JumpGraceTime = JumpGraceTime - item.JumpGraceTime;
        JumpBufferTime = JumpBufferTime - item.JumpBufferTime;
        LadderClimbSpeed = LadderClimbSpeed - item.LadderClimbSpeed;


        MeleeAttackMaxDamage = MeleeAttackMaxDamage - item.MeleeAttackMaxDamage;
        MeleeAttackMinDamage = MeleeAttackMinDamage - item.MeleeAttackMinDamage;
        MeleeCriticalDamageMultiply = MeleeCriticalDamageMultiply - item.MeleeCriticalDamageMultiply;
        MeleeCriticalDamageChance = MeleeCriticalDamageChance - item.MeleeCriticalDamageChance;
        StepUpAfterHit = StepUpAfterHit - item.StepUpAfterHit;
        MeleeAttackCooldownTime = MeleeAttackCooldownTime - item.MeleeAttackCooldownTime;
        MeleePowerAttackMaxDamage = MeleePowerAttackMaxDamage - item.MeleePowerAttackMaxDamage;
        MeleePowerAttackMinDamage = MeleePowerAttackMinDamage - item.MeleePowerAttackMinDamage;

        //MeleeAttackCanPowerAttackCriticalDamage = false;
        MeleeCanThirdAttackCriticalDamage = false;
        MeleeAttackCanPoison = false;
        MeleePoisonDamaged = MeleePoisonDamaged - item.MeleePoisonDamaged;
        MeleePoisonFrequency = MeleePoisonFrequency - item.MeleePoisonFrequency;
        MeleePoisonTick = MeleePoisonTick - item.MeleePoisonTick;
        MeleePoisonChance = 0;
        MeleeAttackCanFire = false;
        MeleeFireDamaged = MeleeFireDamaged - item.MeleeFireDamaged;
        MeleeFireFrequency = MeleeFireFrequency - item.MeleeFireFrequency;
        MeleeFireTick = MeleeFireTick - item.MeleeFireTick;
        MeleeFireChance = 0;
        MeleeAttackCanPush = false;
        MeleeAttackCanFreez = false;
        MeleeFreezDuration = MeleeFreezDuration - item.MeleeFreezDuration;
        MeleeFreezChance = 0;
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
        RangedPoisonChance = 0;
        RangedAttackCanFire = false;
        RangedFireChance = 0;
        RangedFireDamaged = RangedFireDamaged - item.RangedFireDamaged;
        RangedFireFrequency = RangedFireFrequency - item.RangedFireFrequency;
        RangedFireTick = RangedFireTick - item.RangedFireTick;
        RangedAttackCanFreez = false;
        RangedFreezChance = 0;
        RangedAttackCanPush = false;
        RangedAttackCanThroughShoot = false;
        RangedPowerAttackAiming = false;
        RangedPowerBlink = false;
        RangedHeal = false;
        RangedHealCount = 0;

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
            if (turnCooldownTimer <= 0f)
            {
                FlyingWeapon.transform.localScale = new Vector3((int)Facing, transform.localScale.y, transform.localScale.z);
                turnCooldownTimer = TurnCooldownTime;
            }
            else TurnCor();
        }

        // PowerWolfAnimator nodes
        SpriteAnimNodes nodes = GetComponentInChildren<SpriteAnimNodes>();
        wing.transform.position = new Vector3(nodes.GetPosition(0).x - 11 * -(int)Facing, nodes.GetPosition(0).y + 9, nodes.GetPosition(0).z);

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
            else if (fsm.State == States.Shoot)
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
                animator.SetFloat("MeleeAttackSpeed", HandAttackSpeed);
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
            else if (fsm.State == States.Selection)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Action"))
                {
                    animator.Play("Action");
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
            else if (fsm.State == States.Shoot)
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
                animator.SetFloat("MeleeAttackSpeed", MeleeAttackSpeed);
                // If on the dash state
            }
            else if (fsm.State == States.SecondSwordAttack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "SecondAttack"))
                {
                    animator.Play((string)MeleeWeaponType + "SecondAttack");
                }
                animator.SetFloat("MeleeAttackSpeed", SecondSwordAttackSpeed);
                // If on the dash state
            }
            else if (fsm.State == States.ThirdSwordAttack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "ThirdAttack"))
                {
                    animator.Play((string)MeleeWeaponType + "ThirdAttack");
                }
                animator.SetFloat("MeleeAttackSpeed", ThirdSwordAttackSpeed);
                // If on the dash state
            }
            else if (fsm.State == States.PowerSwordAttack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)MeleeWeaponType + "PowerAttack"))
                {
                    animator.Play((string)MeleeWeaponType + "PowerAttack");
                }

                animator.SetFloat("MeleeAttackSpeed", MeleePowerAttackSpeed);
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
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Dash"))
                {
                    animator.Play("Dash");
                }
                // If on the ground
            }
            else if (fsm.State == States.Selection)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Action"))
                {
                    animator.Play("Action");
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

        if (activeWeapon == ActiveWeapon.Bow)
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

            else if (fsm.State == States.BowPrepare)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)RangedWeaponType + "Prepare"))
                {
                    animator.Play((string)RangedWeaponType + "Prepare");
                }
                // If on the attack state
            }
            else if (fsm.State == States.DuckPrepare)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)RangedWeaponType + "DuckPrepare"))
                {
                    animator.Play((string)RangedWeaponType + "DuckPrepare");
                }
                // If on the attack state
            }
            else if (fsm.State == States.DuckPowerShootPrepare)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)RangedWeaponType + "DuckPowerShootPrepare"))
                {
                    animator.Play((string)RangedWeaponType + "DuckPowerShootPrepare");
                }
                // If on the attack state
            }
            else if (fsm.State == States.Shoot)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)RangedWeaponType + "Shoot"))
                {
                    animator.Play((string)RangedWeaponType + "Shoot");
                }
                animator.SetFloat("ShootSpeed", RangedAttackSpeed);
                // If on the attack state
            }
            else if (fsm.State == States.AccumulateShoot)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
                {
                    animator.Play((string)RangedWeaponType + "Shoot");
                }
                animator.SetFloat("ShootSpeed", RangedAttackSpeed);
                // If on the attack state
            }
            else if (fsm.State == States.DuckShoot)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)RangedWeaponType + "DuckShoot"))
                {
                    animator.Play((string)RangedWeaponType + "DuckShoot");
                }

                animator.SetFloat("ShootSpeed", RangedAttackSpeed);
                // If on the attack state
            }
            else if (fsm.State == States.DuckPowerShoot)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)RangedWeaponType + "DuckPowerShoot"))
                {
                    animator.Play((string)RangedWeaponType + "DuckPowerShoot");
                }

                animator.SetFloat("ShootSpeed", RangedPowerAttackSpeed);
                // If on the attack state
            }
            else if (fsm.State == States.BowPowerAttack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)RangedWeaponType + "PowerShoot"))
                {
                    animator.Play((string)RangedWeaponType + "PowerShoot");
                }

                animator.SetFloat("ShootSpeed", RangedPowerAttackSpeed);
                // If on the attack state
            }

            else if (fsm.State == States.BowPowerAttackPrepare)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)RangedWeaponType + "PowerAttackPrepare"))
                {
                    animator.Play((string)RangedWeaponType + "PowerAttackPrepare");
                }
                // If on the attack state
            }

            else if (fsm.State == States.Attack)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName((string)RangedWeaponType + "Attack"))
                {
                    animator.Play((string)RangedWeaponType + "Attack");
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
            else if (fsm.State == States.Selection)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Action"))
                {
                    animator.Play("Action");
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


    public void Blink(Vector3 pos)
    {
        transform.position = Vector2.Lerp(transform.position, new Vector3(pos.x, pos.y + 5, 0), 2);
    }

    IEnumerator ManaRegeneration()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            GetComponent<Mana>().TakeRefresh(1);
        }
    }

    IEnumerator TurnCor()
    {
        yield return new WaitForSeconds(1f);
        FlyingWeapon.transform.localScale = new Vector3((int)Facing, transform.localScale.y, transform.localScale.z);
    }


    public void Push(int pushDistance)
    {
        if (!sticking && !CheckColAtPlace(Vector2.right * (int)Facing * pushDistance, solid_layer))
        {
            Debug.Log(Vector2.right * (int)Facing * pushDistance);
            transform.position = new Vector2(transform.position.x + (pushDistance * (int)Facing), transform.position.y);
        }
    }

    public bool Take_RangedShell(int ammount)
    {
        //return false; // если нету такого оржуия TODO
        RangedWeapon.GetComponent<PickupRangedWeapons>().RangedShellsCountCurent += ammount;
        ShellsCount += ammount;
        countShellText.GetComponent<Text>().text = ShellsCount.ToString();

        return true;
    }

    public bool Take_RangedPowerShell(int ammount)
    {
        //return false; // если нету такого оржуия TODO
        RangedWeapon.GetComponent<PickupRangedWeapons>().RangedPowerShellsCountCurent += ammount;
        RangedPowerShellsCount += ammount;
        countPowerShellText.GetComponent<Text>().text = RangedPowerShellsCount.ToString();

        return true;
    }

    public bool Take_MeleeShell(int ammount)
    {
        //return false; // если нету такого оржуия TODO
        MeleeWeapon.GetComponent<PickupMeleeWeapons>().MeleePowerShellsCountCurent += ammount;
        MeleePowerShellsCount += ammount;
        //countMeleePowerShellText.GetComponent<Text>().text = MeleePowerShellsCount.ToString();

        return true;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        var component = col.GetComponent<Health>(); // TODO переделать для другого разбивать вазы
        var worm = col.GetComponent<WormEnemy>();
        // If the target the hitbox collided with has a health component and it is not our owner and it is not on the already on the list of healths damaged by the current hitbox
        if (component != null && component != owner && !healthsDamaged.Contains(component) && worm != null && fsm.State == States.Roll)
        {
            // Try to Apply the damage
            var didDamage = component.TakeDamage(3, false, 0, 0, 0, 0, false, 0, 0, 0, 0, false, 0, false, 0, 0, false, 0, false, 0, 0);

            if (didDamage)
            {
                // Add the health component to the list of damaged healths
                healthsDamaged.Add(component);
            }
        }
    }
}