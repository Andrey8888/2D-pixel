#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using UnityEngine.UI;
using System;

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
    public float TurnCooldownTime = 1f;
    private float turnCooldownTimer = 0f;

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

    [Header("Attacks")]
    private bool InVisibilityZone = false;       // Проверка на нахождение в зоне видимости     
    private Vector3 PlayerPos;                  // Позиция игрока, необходима для расчета угла стрельбы
    public Vector3 PlayerPosGlobal;             // Позиция игрока на карте


    // States for the state machine
    public enum States
    {
        Normal,
        Death
    }

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


        if (turnCooldownTimer > 0f)
        {
            turnCooldownTimer -= Time.deltaTime;
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
                health.TakeDamage(9999, false, 0, 0, 0, false, 0, 0, 0, false, 0, false, 0);
            }
        }
    }

    void Normal_Update()
    {
        if (BehaivourType == Behaivour.FreeWalking)
        {
            if (moveX != 0 && CheckColInDir(new Vector2(moveX, 0), solid_layer))
            {
                moveX *= -1;
                CharacterRotation();
            }
        }

        float num = onGround ? 1f : 0.65f;

        if (BehaivourType == Behaivour.Patroling)
        {
            if (!InVisibilityZone)
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
        }
        else
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
            // преследование
            else if (InVisibilityZone && (Vector3.Distance(PlayerPos, transform.position) > MinDist))
            {
                Speed.x = Calc.Approach(Speed.x, moveX * MaxRun, RunReduce * num * Time.deltaTime);

                if (!onGround)
                {
                    float target = MaxFall;
                    Speed.y = Calc.Approach(Speed.y, target, Gravity * Time.deltaTime);
                }
            }
        }



        if (BehaivourType == Behaivour.Following)
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
            else if (InVisibilityZone && (Vector3.Distance(PlayerPos, transform.position) > MinDist))
            {
                Speed.x = Calc.Approach(Speed.x, moveX * MaxRun, RunReduce * num * Time.deltaTime);
            }
        }


        if (BehaivourType == Behaivour.Idle)
        {
            // пусто
        }
    }

    void CharacterRotation()
    {
        Facings facings = (Facings)moveX;
        Facing = facings;
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

        if (fsm.State == States.Death)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
                animator.Play("Death");
            }
        }
         // If on the ground
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
        fsm.ChangeState(States.Death, StateTransition.Overwrite);
        //Destroy(ImpactZone);
        //Destroy(VisibilityZone);
    }
}
