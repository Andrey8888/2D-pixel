using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

	[Header ("Speed")]
	public Vector2 Speed;

	[Header ("Damage")]
	public int DamageOnHit;

	[Header ("Layers")]
	public LayerMask solid_layer;

	[HideInInspector]
	public Health owner; // owner of the projectile
	private Vector2 Position; // Current position
	private Vector2 movementCounter = Vector2.zero;  // Counter for subpixel movement
	private Rigidbody2D rb2D; // Cached Rigidbody2D attached to the projectile

	List<Health> healthsDamaged = new List<Health>(); // List to store healths damaged

	void Awake () {
		rb2D = GetComponent<Rigidbody2D> ();
	}

	void Start () {
		// keeping everything Pixel perfect
		Position = new Vector2 (Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
		transform.position = Position;
	}

	void Update () {
		// Particle Pixel Perfect Movement
		movementCounter += Speed;
	
		var moveh = (int)Mathf.Round(this.movementCounter.x);
		if (moveh != 0) {
			movementCounter.x -= (float)moveh;
			Position.x += moveh;
		}

		var movev = (int)Mathf.Round(this.movementCounter.y);
		if (movev != 0) {
			movementCounter.y -= (float)movev;
			Position.y += movev;
		}

		// Move the rigidbody
		rb2D.MovePosition (Position);	
	}

	void DestroyMe () {
		Destroy (gameObject);
	}

	void OnTriggerEnter2D (Collider2D col) {
		// if the projectile hit's a solid object, destroy it
		if (col.gameObject.layer ==  (int)Mathf.Log(solid_layer.value, 2)) {
			DestroyMe ();
			return;
		}

		var component = col.GetComponent<Health> ();
		// If the target the hitbox collided with has a health component and it is not our owner and it is not on the already on the list of healths damaged by the current hitbox
		if (component != null && component != owner && !healthsDamaged.Contains(component)) {
			// Add the health component to the list of damaged healths
			healthsDamaged.Add (component);

			// Apply the damage
			var didDamage = component.TakeDamage (DamageOnHit);
			// Destroy the projectile after applying damage
			if (didDamage) {
				DestroyMe ();
			}
		}
	}
}

