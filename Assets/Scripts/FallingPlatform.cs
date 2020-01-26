using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Added in Update V1.1.0
public class FallingPlatform : MonoBehaviour {

	public bool isTriggered = false;

	public float timeBeforeFalling = 0.5f; 

	public float destroyAfterTime = 0.5f;

	public BoxCollider2D myCollider;
	public Animator myAnimator;

	void Awake () {
		if (myCollider == null) {
			myCollider = GetComponent<BoxCollider2D> ();

			if (myCollider == null) {
				Debug.Log ("This Falling Platform has no Collider Attached to it");
			}
		}

		if (myAnimator == null) {
			myAnimator = GetComponentInChildren<Animator> ();

			if (myAnimator == null) {
				Debug.Log ("This falling platform has no animator attached to it");
			}
		}
	}

	void Update () {
		if (!isTriggered) {
			if (CheckColAtPlace(Vector2.up, 1)) {
				var cols = CheckColsInDirAll (Vector2.up, 1);

				foreach (Collider2D col in cols) {
					if (col.GetComponent<Actor> () != null && col.GetComponent<Actor>().onGround) {
						isTriggered = true;
						startFalling ();
						break;
					}
				}
			}
		}
	}

	[ContextMenu("Start Falling")]
	void startFalling () {
		StartCoroutine (FallRoutine ());
	}

	IEnumerator FallRoutine () {
		// wait the set time before setting the collider off
		yield return new WaitForSeconds (timeBeforeFalling);

		// Play the fall animation on the animator
		if (myAnimator != null) {
			myAnimator.Play ("Fall");
		}

		// Desactivate the Collider
		if (myCollider.enabled)
			myCollider.enabled = false;

		// Wait the set time before destroying the object (to play the animation)
		yield return new WaitForSeconds (destroyAfterTime);

		// Destroy the object
		//Destroy (gameObject);
	}

	[ContextMenu("Reset Platform")]
	public void ResetPlatform () {
		isTriggered = false;

		// Play the fall animation on the animator
		if (myAnimator != null) {
			myAnimator.Play ("Idle");
		}

		// Desactivate the Collider
		if (!myCollider.enabled)
			myCollider.enabled = true;
	}

	// The same as CheckColInDir but it returns a Collider2D array of the colliders you're collisioning with
	public Collider2D[] CheckColsInDirAll (Vector2 dir, LayerMask layer) {
		Vector2 leftcorner = Vector2.zero; 
		Vector2 rightcorner = Vector2.zero;

		if (dir.x > 0) {
			leftcorner = new Vector2 (myCollider.bounds.center.x + myCollider.bounds.extents.x, myCollider.bounds.center.y + myCollider.bounds.extents.y - .1f);
			rightcorner = new Vector2 (myCollider.bounds.center.x + myCollider.bounds.extents.x + .5f, myCollider.bounds.center.y - myCollider.bounds.extents.y + .1f);
		} else if (dir.x < 0) {
			leftcorner = new Vector2 (myCollider.bounds.center.x - myCollider.bounds.extents.x - .5f, myCollider.bounds.center.y + myCollider.bounds.extents.y - .1f);
			rightcorner = new Vector2 (myCollider.bounds.center.x - myCollider.bounds.extents.x, myCollider.bounds.center.y - myCollider.bounds.extents.y + .1f);
		} else if (dir.y > 0) {
			leftcorner = new Vector2 (myCollider.bounds.center.x - myCollider.bounds.extents.x + .1f, myCollider.bounds.center.y + myCollider.bounds.extents.y + .5f);
			rightcorner = new Vector2 (myCollider.bounds.center.x + myCollider.bounds.extents.x - .1f, myCollider.bounds.center.y + myCollider.bounds.extents.y);
		} else if (dir.y < 0) {
			leftcorner = new Vector2 (myCollider.bounds.center.x - myCollider.bounds.extents.x + .1f, myCollider.bounds.center.y - myCollider.bounds.extents.y);
			rightcorner = new Vector2 (myCollider.bounds.center.x + myCollider.bounds.extents.x - .1f, myCollider.bounds.center.y - myCollider.bounds.extents.y - .5f);
		}

		return Physics2D.OverlapAreaAll(leftcorner, rightcorner, layer);
	}

	// Helper function to check if there is any collision within a given layer in a set direction (only use up, down, left, right)
	public bool CheckColAtPlace (Vector2 extraPos, LayerMask layer) {
		Vector2 leftcorner = Vector2.zero; 
		Vector2 rightcorner = Vector2.zero;

		leftcorner = new Vector2 (myCollider.bounds.center.x - myCollider.bounds.extents.x + .1f, myCollider.bounds.center.y + myCollider.bounds.extents.y - .1f) + extraPos;
		rightcorner = new Vector2 (myCollider.bounds.center.x + myCollider.bounds.extents.x - .1f, myCollider.bounds.center.y - myCollider.bounds.extents.y + .1f) + extraPos;

		return Physics2D.OverlapArea(leftcorner, rightcorner, layer);
	}

}
