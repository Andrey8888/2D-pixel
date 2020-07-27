using System.Collections;
using System.Collections.Generic;
using MonsterLove.StateMachine;
using UnityEngine;

// This script has been added on Pixel Platformer Engine V1.2
public class HitablePushBlock : Actor {

	public float Gravity = 900f; // Gravity force
	public float MaxFall = -240f; // Maximun fall speed

    [Header("Animator")]
    public Animator animator; // Reference to the animator

    // State Machine
    public StateMachine<States> fsm;

    public GameObject[] Dropout;

    public enum States
    {
        Normal,
        Hit,
        Fall,
        Death
    }

    public bool CanFall
    {
        get
        {
            return !onGround && Speed.y == MaxFall;
        }
    }
    new void Awake () {
		base.Awake();
        fsm = StateMachine<States>.Initialize(this);
    }
    void Start()
    {
        fsm.ChangeState(States.Normal);
    }
    // Update is called once per frame
    new void Update () {
		// Update variables
		wasOnGround = onGround;
		onGround = OnGround();
		// Gravity
		if (!onGround) {
			float target = MaxFall;
			Speed.y = Calc.Approach (Speed.y, target, Gravity * Time.deltaTime);
        }
    }
    void Normal_Update()
    {
        // Ducking over here
        if (CanFall)
        {
            fsm.ChangeState(States.Fall, StateTransition.Overwrite);
            return;
        }
    }
        void LateUpdate () {
		// Vertical movement
		var movev = base.MoveV (Speed.y * Time.deltaTime);
		if (movev)
			Speed.y = 0;
        UpdateSprite();
    }
    void UpdateSprite()
    {
            if (fsm.State == States.Hit)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
                {
                    animator.Play("Hit");
                }
            }
            if (fsm.State == States.Death)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
                {
                    animator.Play("Death");
                }
            }
    }
    void Fall_Update()
    {
        if (onGround)
            {
            Die();
            }
    }
    public void Hit()
    {
        fsm.ChangeState(States.Hit, StateTransition.Overwrite);
    }
    public void Die()
    {
        fsm.ChangeState(States.Death, StateTransition.Overwrite);
        myCollider.enabled = false;
        if (Dropout != null)
        {
            var rnd = Random.Range(0, Dropout.Length);
            var p = Instantiate(Dropout[rnd], gameObject.transform.position, Quaternion.identity);
        }
    }
}
