using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvents : MonoBehaviour {

    private Animator bowAnimator;
    private void Start()
    {
        bowAnimator = GetComponent<Animator>();
    }
    public void PlayerBackToNormalState()
    {
        var player = GetComponentInParent<Player>();

        if (player != null)
        {
            player.fsm.ChangeState(Player.States.Normal, MonsterLove.StateMachine.StateTransition.Overwrite);
        }
    }
    public void PlayerBowTension()
    {
        var player = GetComponentInParent<Player>();

        if (player != null)
        {
            player.fsm.ChangeState(Player.States.BowTension, MonsterLove.StateMachine.StateTransition.Overwrite);
        }
    }
    public void PlayerShootBackToNormalState () {
		var player = GetComponentInParent<Player> ();

		if (player != null ) {
			player.fsm.ChangeState (Player.States.Normal, MonsterLove.StateMachine.StateTransition.Overwrite);
            player.isShoot = false;

            if (!bowAnimator.GetCurrentAnimatorStateInfo(0).IsName("BowIdle"))
            {
                bowAnimator.Play("BowIdle");
            }
        }
	}
    public void MeleeEnemyBackToNormalState()
    {
        var enemy = GetComponentInParent<WormEnemy>();

        if (enemy != null)
        {
            enemy.fsm.ChangeState(WormEnemy.States.Normal, MonsterLove.StateMachine.StateTransition.Overwrite);

            if (!bowAnimator.GetCurrentAnimatorStateInfo(0).IsName("BowIdle"))
            {
                bowAnimator.Play("BowIdle");
            }
        }
    }
}
