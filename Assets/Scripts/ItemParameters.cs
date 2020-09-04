using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemParameters : MonoBehaviour
{

    [Header("Movement Variables")]

    public Artifacts Type;
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
    public float RollSpeed = 240f; // Скорость переката 
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

    #region WeaponParams

    [Header("Hands")]
    public int HandAttackMinDamage = 1;//new int[2] {1, 0, 0};
    public int HandAttackMaxDamage = 2;//new int[2] {2, 0, 0};
    public int HandStepUpAfterHit = 20; //new int[2] {20,0,0};

    [Header("MeleeAttacks")]
    public string MeleeWeaponType;
    private MeleeWeapon MeleeWeaponClass;

    public int MeleeAttackMaxDamage;
    public int MeleeAttackMinDamage;
    public int MeleeCriticalDamageMultiply;
    public int MeleeCriticalDamageChance;
    private float MeleeAttackSpeed = 1f;
    public int StepUpAfterHit;
    public int PopUpAfterHit = 0;
    public float MeleeAttackCooldownTime = 0.8f;
    public float SpeedOnSwordAttack = 20;
    [HideInInspector]
    public bool ChangeCollider = false;
    public bool hasSeries = false;
    private bool hasBlock = false;
    public bool MeleeCanThirdAttackCriticalDamage;

    [Header("MeleePowerAttack")]
    public int MeleePowerAttackMinDamage = 0;
    public int MeleePowerAttackMaxDamage = 0;
    public float MeleePowerAttackCooldownTime = 0.8f;
    [Range(1, 10)]
    public int MeleePowerCriticalDamageMultiply = 1;
    [Range(0, 99)]
    public int MeleePowerChanceCriticalDamage = 0;

    [Header("Poison")]
    public bool MeleeAttackCanPoison = false;
    public bool MeleePowerAttackCanPoison = false;
    public int MeleePoisonDamaged = 0;
    public int MeleePoisonFrequency = 0;
    public int MeleePoisonTick = 0;
    public int MeleePoisonChance = 100;
    [Header("Fire")]
    public bool MeleeAttackCanFire = false;
    public bool MeleePowerAttackCanFire = false;
    public int MeleeFireDamaged = 0;
    public int MeleeFireFrequency = 0;
    public int MeleeFireTick = 0;
    public int MeleeFireChance = 100;
    [Header("Freez")]
    public bool MeleeAttackCanFreez = false;
    public bool MeleePowerAttackCanFreez = false;
    public int MeleeFreezDuration = 0;
    public int MeleeFreezChance = 100;
    [Header("Push")]
    public bool MeleeAttackCanPush = false;
    public bool MeleePowerAttackCanPush = false;
    public int MeleePushDistance = 0;
    [Header("Other")]
    public float HandAttackCooldownTime = 0.8f;
    public float SecondSwordAttackCooldownTime = 0.4f;
    public float ThirdSwordAttackCooldownTime = 0.4f;
    public float MeleeBlockCooldownTime = 0.5f;
    public bool MeleeAttackCanThirdAttackCriticalDamage;


    [Header("RangedAttacks")]
    public string RangedWeaponType;
    public int RangedCriticalDamage = 0;
    public int RangedCriticalDamageMultiply = 1;
    public int RangedCriticalDamageChance = 0;
    public int RangedAttackMinDamage = 1;//new int[2] {1, 0, 0};
    public int RangedAttackMaxDamage = 2;//new int[2] {2, 0, 0};
    public int ShellsCount = 0;
    public float RangedAttackCooldownTime = 1f;

    [Header("RangedPowerAttack")]
    public int RangedPowerAttackMinDamage = 0;
    public int RangedPowerAttackMaxDamage = 0;
    public float RangedPowerAttackCooldownTime = 10f;
    [Range(1, 10)]
    public int RangedPowerCriticalDamageMultiply = 1;
    [Range(0, 99)]
    public int RangedPowerChanceCriticalDamage = 0;
    public int RangedPowerShellsCount = 0;

    [Header("Poison")]
    public bool RangedAttackCanPoison = false;
    public bool RangedPowerAttackCanPoison = false;
    public int RangedPoisonDamaged = 0;
    public int RangedPoisonFrequency = 0;
    public int RangedPoisonTick = 0;
    public int RangedPoisonChance = 100;

    [Header("Fire")]
    public bool RangedAttackCanFire = false;
    public bool RangedPowerAttackCanFire = false;
    public int RangedFireDamaged = 0;
    public int RangedFireFrequency = 0;
    public int RangedFireTick = 0;
    public int RangedFireChance = 100;

    [Header("Freez")]
    public bool RangedAttackCanFreez = false;
    public bool RangedPowerAttackCanFreez = false;
    public int RangedFreezDuration = 0;
    public int RangedFreezChance = 100;

    [Header("Push")]
    public bool RangedAttackCanPush = false;
    public bool RangedPowerAttackCanPush = false;
    public int RangedPushDistance = 0;

    [Header("Other")]
    public bool RangedAttackCanThroughShoot = false;
    public bool RangedTossingUp = false;
    public bool RangedAttackAiming = false;
    public int PowerAttackPopUpAfterHit = 0;
    public int RangedStepUpAfterHit = 0;
    public bool RangedAiming = false;

    [Header("PowerAttackOther")]
    public int RangedPowerAttackPopUpAfterHit = 0;
    public bool RangedPowerAttackAiming = false;
    public bool RangedPowerAttackCanThroughShoot = false;
    #endregion

    [Header("Artifacts Stacks")]
    public Artifacts[] ArtifactsType = null; // TODO с чем стакается предмет
    public ItemParameters[] StacksArtifacts;
}
