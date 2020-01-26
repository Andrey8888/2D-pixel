using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvents : MonoBehaviour {

	public void PlayerBackToNormalState () {
		var player = GetComponentInParent<Player> ();

		if (player != null ) {
			player.fsm.ChangeState (Player.States.Normal, MonsterLove.StateMachine.StateTransition.Overwrite);
		}
	}

}
