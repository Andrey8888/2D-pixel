#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using UnityEngine.UI;

public class WormEnemy : Actor
{
    [Header("Movement Variables")]
    // Gravity, Maximun fall speed & fastfall Speed
    public float Gravity = 900f;                // Speed at which you are pushed down when you are on the air
    public float MaxFall = -160f;               // Maximun common speed at which you can fall
    public float RunReduce = 400f;              // Horizontal Acceleration when you're already when your horizontal speed is higher or equal to the maximun
    public float MaxRun = 90f;                  // Maximun Horizontal Run Speed
    public float RunAccel = 1000f;              // Horizontal Acceleration Speed
    public float AirMult = 0.65f;               // Multiplier for the air horizontal movement (friction) the higher the more air control you'll have

    public GameObject PointParent;              // Объект предок всех точек патрулирования
    private Transform[] PointMassive;           // Массив точек обхода для режима патруль
    private float[] PointDelayMassive;          // Массив задержек на точке
    public int PointID = 1;                     // № точки, к которой идет враг
    public float MinDist = 0.5f;                // Допустимое расстояние, при котором враг переключается на следующую точку

    // Helper private Variables
    private int moveX;                          // Variable to store the horizontal Input each frame

    //[Header("Facing Direction")]
    //public Facings Facing;  // Facing Direction

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
    public Animator bowAnimator;

    [Header("Стрельба")]
    public float angle;
    public GameObject bow;                      // Лук
    public Transform GunBarrel;                 // Позиция точки выстрела
    private bool InVisibilityZone = false;      // Проверка на нахождение в зоне видимости
    private bool InImpactArea = false;          // Проверка на нахождение в зоне поражения
    public float ArrowVel = 400;                 // Начальная скорость арбалетного болта         
    public float BowAttackCooldownTime = 4f;    // Задержка по атаке      
    private float bowAttackCooldownTimer = 0f;  // Таймер до атаки
    private Vector3 PlayerPos;                  // Позиция игрока, необходима для расчета угла стрельбы
    public Vector3 PlayerPosGlobal;             // Позиция игрока на карте
    // States for the state machine
    public enum States
    {
        Normal,
        BowAttack,
        Death
    }

    public enum Attack
    {
        Infighting,                         // Ближний бой
        Outfighting                         // Дальний бой
    }
    public Attack AttackType = Attack.Infighting;

    public enum Behaivour
    {
        Following,                          // Преследование
        Patroling,                          // Патрулирование
        FreeWalking,                        // Свободное движение до преграды
        Idle                                // Покой
    }
    public Behaivour BehaivourType = Behaivour.Idle;

    public GameObject ImpactZone;
    public GameObject VisibilityZone;

    private EnemyVisibility EnemyVisibility;// Скрипт видимости
    private EnemyImpact EnemyImpact;        // Скрипт зоны нападения
    public float PatrolTimer = 0;

    public bool CanShoot
    {
        get
        {
            return AttackType == Attack.Outfighting && EnemyVisibility.InVisibilityZone && bowAttackCooldownTimer <= 0f;
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
        SetPatrol();
    }

    private void SetPatrol()
    {
        if (PointParent != null)
        {
            int i = PointParent.GetComponentInChildren<Transform>().childCount;
            PointMassive = new Transform[i];
            PointDelayMassive = new float[i];
            for (int j = 0; j < i; j++)
            {
                PointMassive[j] = PointParent.GetComponentInChildren<Transform>().GetChild(j);
                if (PointMassive[j].GetComponentInChildren<PointParameters>())
                {
                    PointDelayMassive[j] = PointMassive[j].GetComponentInChildren<PointParameters>().DelayTime;
                }
                else
                {
                    PointDelayMassive[j] = 0;
                }
            }
        }
        else
        {
            BehaivourType = Behaivour.Idle;
        }
    }

    new void Update()
    {
        base.Update();
        // Update all collisions here
        wasOnGround = onGround;
        onGround = OnGround();

        EnemyVisibility = VisibilityZone.GetComponentInChildren<EnemyVisibility>();
        InVisibilityZone = EnemyVisibility.InVisibilityZone;
        PlayerPos = EnemyVisibility.PlayerPos;
        EnemyImpact = ImpactZone.GetComponentInChildren<EnemyImpact>();
        InImpactArea = EnemyImpact.InImpactZone;

        //Bow Attack timer
        if (bowAttackCooldownTimer > 0f)
        {
            bowAttackCooldownTimer -= Time.deltaTime;
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
        if (CollisionSelf(solid_layer))
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

        float num = onGround ? 1f : 0.65f;

        if (!onGround)
        {
            float target = MaxFall;
            Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
        }

        if (BehaivourType == Behaivour.FreeWalking)
        {
            if (moveX != 0 && CheckColInDir(new Vector2(moveX, 0), solid_layer))
            {
                moveX *= -1;
                SpriteScale = new Vector2(-SpriteScale.x, SpriteScale.y);
            }
        }

        if (BehaivourType == Behaivour.Patroling)
        {
            if (!InVisibilityZone)
            {
                if ((PointMassive.Length - 1) >= 1)
                {
                    if (moveX != 0 && CheckColInDir(new Vector2(moveX, 0), solid_layer))
                    {
                        moveX *= -1;
                        SpriteScale = new Vector2(-SpriteScale.x, SpriteScale.y);
                        if (PointID < (PointMassive.Length - 1))
                        {
                            PointID++;
                        }
                        else
                        {
                            PointID = 0;
                        }
                        PatrolTimer = 0;
                    }

                    if (Mathf.Sign(SpriteScale.x) == Mathf.Sign(transform.position.x - PointMassive[PointID].position.x))
                    {
                        moveX *= -1;
                        SpriteScale = new Vector2(-SpriteScale.x, SpriteScale.y);
                    }
                    if (Vector3.Distance(PointMassive[PointID].position, transform.position) > MinDist)
                    {
                        Speed.x = Calc.Approach(Speed.x, moveX * MaxRun, RunReduce * num * Time.deltaTime);
                    }
                    else
                    {
                        if (PatrolTimer > PointDelayMassive[PointID])
                        {
                            if (PointID < (PointMassive.Length - 1))
                            {
                                PointID++;
                            }
                            else
                            {
                                PointID = 0;
                            }
                            PatrolTimer = 0;
                        }
                        else
                        {
                            PatrolTimer += Time.deltaTime;
                        }
                    }
                }
            }
            else
            {
                if (InVisibilityZone && (Vector3.Distance(PlayerPos, transform.position) > MinDist))
                {
                    Speed.x = Calc.Approach(Speed.x, moveX * MaxRun, RunReduce * num * Time.deltaTime);
                }
            }
        }
        if (BehaivourType == Behaivour.Following)
        {
            if (InVisibilityZone && (Vector3.Distance(PlayerPos, transform.position) > MinDist))
            {
                Speed.x = Calc.Approach(Speed.x, moveX * MaxRun, RunReduce * num * Time.deltaTime);
            }
        }
        if (BehaivourType == Behaivour.Idle)
        {

        }
    }
    void BowAttack_Update()
    {
       bow.transform.rotation = (Quaternion.Euler(0, 0, angle + 45));
       angle = Vector3.Angle(Vector3.right, (-transform.position + PlayerPos));
       if ((PlayerPos.y - transform.position.y) < 0)
       {
        angle = -angle;
       }
    }

    void Death_Update()
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

    // Function to update the sprite scale, facing direction and animations 
    void UpdateSprite()
    {
        //Approch the normal sprite scale at a set rate
        // SpriteScale.x = Calc.Approach(SpriteScale.x, 1f, 0.04f);
        // SpriteScale.y = Calc.Approach(SpriteScale.y, 1f, 0.04f);

        // //Set the x scale to the current facing direction

        //var targetLocalScale = new Vector3(((int)Facing) * -1f, transform.localScale.y, transform.localScale.z);
        // if (transform.localScale != targetLocalScale)
        // {
        //    transform.localScale = targetLocalScale;
        //}

        //Set the SpriteHolder scale to the target scale
        var targetSpriteHolderScale = new Vector3(SpriteScale.x, SpriteScale.y, 1f);
        if (SpriteHolder.localScale != targetSpriteHolderScale)
        {
            SpriteHolder.localScale = targetSpriteHolderScale;
        }

        if (fsm.State == States.Death)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
                animator.Play("Death");
            } 
        }
        else if (fsm.State == States.BowAttack)
        {
            if (!bowAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shoot"))
            {
                bowAnimator.Play("Shoot");
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
                // Run Animation
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                {
                    animator.Play("Run");
                }
            }
        }
    }

    public void Die()
    {
        bow.SetActive(false);
        fsm.ChangeState(States.Death, StateTransition.Overwrite);
    }
}
