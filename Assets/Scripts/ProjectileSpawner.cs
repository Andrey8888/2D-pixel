using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour {


	[Header ("Owner of the projectile Spawner")]
	public Health owner;

	[Header ("Projectile to Shoot")]
	public Projectile projectile;

	[Header ("Place to spawn the projectile at")]
	public Transform gunBarrel;

	void Awake () {
		if (owner == null) {
			owner = GetComponentInParent<Health> ();

			if (owner == null) {
				Debug.Log ("This projectileSpawner has no owner/health attached to it");
			}
		}
	}

	public void InstantiateProjectile () {
		//Instantiate the projectile prefab
		var p = Instantiate (projectile, gunBarrel.position, Quaternion.identity) as Projectile;

		// Shoot based on the X scale of our parent object (base facing), which should be 1 for right and -1 for left 
		var parentXScale = Mathf.Sign (transform.parent.localScale.x);

		// Set the localscale so the projectiles faces the right direction based on the parent object (base)
		p.transform.localScale = new Vector3 (parentXScale * p.transform.localScale.x, p.transform.localScale.y, p.transform.localScale.z);

		if (owner != null) {
			p.owner = owner; // Set it's owner 
		}

		// Change the X speed based on the facing of the parent object
		p.Speed.x *= parentXScale;

		// Do a small screenshake to add a little bit of extra "feel"
		if (PixelCameraController.instance != null) {
			PixelCameraController.instance.DirectionalShake (new Vector2 (parentXScale, 0f), .05f);
		}
	}

}
