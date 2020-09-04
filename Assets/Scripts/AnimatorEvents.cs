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

    public void PlayerBackToDuckingState()
    {
        var player = GetComponentInParent<Player>();

        if (player != null)
        {
            var health = GetComponentInParent<Health>();
            health.block = false;
            player.fsm.ChangeState(Player.States.Ducking, MonsterLove.StateMachine.StateTransition.Overwrite);
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

    public void EnemyBlink()
    {
        var enemy = GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            //enemy.OnStun = false;
            enemy.Blink();
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

    public void PlayerAim()
    {
        var player = GetComponentInParent<Player>();

        if (player != null)
        {
            player.canAim = true;
        }
    }

    public void PlayerSwordAttackMove()
    {
        var player = GetComponentInParent<Player>();

        if (player != null)
        {
            player.OnAttackMove = true;
        }
    }

    public void PlayerSwordAttackMoveEnd()
    {
        var player = GetComponentInParent<Player>();

        if (player != null)
        {
            player.OnAttackMove = false;
        }
    }

    public void CanSecondSwordAttack()
    {
        var player = GetComponentInParent<Player>();

        if (player != null)
        {
            player.secondSwordAttackCooldownTimer = player.SecondSwordAttackCooldownTime;
            Debug.Log("CanSecondSwordAttack");
        }
    }

    public void CanThirdSwordAttack()
    {
        var player = GetComponentInParent<Player>();

        if (player != null)
        {
            player.thirdSwordAttackCooldownTimer = player.ThirdSwordAttackCooldownTime;
            Debug.Log("CanThirdSwordAttack");
        }
    }
}
