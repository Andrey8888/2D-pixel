using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvents : MonoBehaviour {

    public void PlayerBackToNormalState()
    {
        var player = GetComponentInParent<Player>();

        if (player != null)
        {
            var health = GetComponentInParent<Health>();
            health.block = false;
            player.fsm.ChangeState(Player.States.Normal, MonsterLove.StateMachine.StateTransition.Overwrite);
        }
    }

    public void MeleeEnemyBackToNormalState()
    {
        var enemy = GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            //enemy.OnStun = false;
            enemy.fsm.ChangeState(Enemy.States.Normal, MonsterLove.StateMachine.StateTransition.Overwrite);
        }
    }

    public void HitablePushBlockBackToNormalState()
    {
        var pushblock = GetComponentInParent<HitablePushBlock>();

        if (pushblock != null)
        {
            pushblock.fsm.ChangeState(HitablePushBlock.States.Normal, MonsterLove.StateMachine.StateTransition.Overwrite);
        }
    }
}
