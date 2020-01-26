using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script has been added on Pixel Platformer Engine V1.2
public class PushBlock : Actor {

	public float Gravity = 900f; // Gravity force
	public float MaxFall = -240f; // Maximun fall speed

	new void Awake () {
		base.Awake();
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

	void LateUpdate () {
		// Vertical movement
		var movev = base.MoveV (Speed.y * Time.deltaTime);
		if (movev)
			Speed.y = 0;
	}

}
