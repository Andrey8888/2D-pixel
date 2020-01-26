using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour {

	// Helper Variables & Timer
	[Header ("Speed/Direction of the Movement")]
	public Vector2 Speed;
	public Vector2 movementCounter = Vector2.zero; 

	[Header ("Squash & Stretch")]
	public Transform SpriteHolder;
	public Vector2 SpriteScale = Vector2.one;

	[Header ("Bumper Layer")]
	public LayerMask bumper_layer; // Layer of the bumper which makes this platform move
	public LayerMask entities_layer;

	Collider2D myCollider; // The collider

	private List<Actor> EntitiesOnTop = new List<Actor> ();
	private List<Actor> EntitiesOnBot = new List<Actor> ();
	private List<Actor> EntitiesOnRight = new List<Actor> ();
	private List<Actor> EntitiesOnLeft = new List<Actor> ();


	void Awake () {
		// Get the Collider
		myCollider = GetComponent<Collider2D> ();
	}

	void LateUpdate () {
		// Check for entities on all sides
		EntitiesOnTop = GetEntitiesOnDir(Vector2.up, entities_layer);
		EntitiesOnBot = GetEntitiesOnDir(Vector2.down, entities_layer);
		EntitiesOnLeft = GetEntitiesOnDir(Vector2.left, entities_layer);
		EntitiesOnRight = GetEntitiesOnDir(Vector2.right, entities_layer);

		// Horizontal Movement
		if (Speed.x != 0) {
			var MoveH = MoveHPlatform (Speed.x * Time.deltaTime);
			if (MoveH) {
				Speed.x *= -1f;
			}
		}

		// Vertical Movement
		if (Speed.y != 0) {
			var MoveV = MoveVPlatform (Speed.y * Time.deltaTime);
			if (MoveV) {
				Speed.y *= -1f;
			}
		}
	}

	void UpdateSprite () {
		// Approch the normal sprite scale at a set rate
		SpriteScale.x = Calc.Approach (SpriteScale.x, 1f, 0.04f/*1.75f * Time.deltaTime*/);
		SpriteScale.y = Calc.Approach (SpriteScale.y, 1f, 0.04f/*1.75f * Time.deltaTime*/);

		// Set the SpriteHolder scale to the target scale
		var targetSpriteHolderScale = new Vector3 (SpriteScale.x, SpriteScale.y, 1f);
		if (SpriteHolder.localScale != targetSpriteHolderScale) {
			SpriteHolder.localScale = targetSpriteHolderScale;
		}
	}

	List<Actor> GetEntitiesOnDir (Vector2 dir, LayerMask layer) {
		// Create a new list to store the result
		var returnList = new List<Actor> ();

		// Check for all the collisions on top of this platform on the entities layer & store the colliders on a list
		var collidersInDir = CheckColsInDirAll (dir, layer);

		// Iterate through the colliders list and check if they have an Actor component, if it does add it to the list
		if (collidersInDir.Length > 0) {
			foreach (Collider2D s in collidersInDir) {
				var component = s.GetComponent<Actor> ();
				if (component && !returnList.Contains(component)) {
					returnList.Add (component);
				}
			}
		}

		return returnList;
	}

	public bool MoveHPlatform(float moveH) {
		this.movementCounter.x = this.movementCounter.x + moveH;
		int num = (int)Mathf.Round(this.movementCounter.x);
		if (num != 0)
		{
			this.movementCounter.x = this.movementCounter.x - (float)num;
			return this.MoveHExactPlatform(num);
		}
		return false;
	}

	public bool MoveVPlatform(float moveV) {
		this.movementCounter.y = this.movementCounter.y + moveV;
		int num = (int)Mathf.Round(this.movementCounter.y);
		if (num != 0)
		{
			this.movementCounter.y = this.movementCounter.y - (float)num;
			return this.MoveVExactPlatform(num);
		}
		return false;
	}

	public bool MoveHExactPlatform(int moveH) {
		int num = (int)Mathf.Sign((float)moveH);
		while (moveH != 0) {
			bool solid = CheckColInDir(Vector2.right * (float)num, bumper_layer);
			if (solid) {
				this.movementCounter.x = 0f;
				return true;
			}
			moveH -= num;
			// Move the platform
			transform.position = new Vector2 (transform.position.x + (float)num, transform.position.y);

			// Move the entities on top of the platform
			if (EntitiesOnTop.Count > 0) {
				foreach (Actor s in EntitiesOnTop) {
					s.MoveHExact (num);
				}
			}

			// If the block moved left
			if (num < 0) {
				// Move the entities on the left of the block
				if (EntitiesOnLeft.Count > 0) {
					foreach (Actor s in EntitiesOnLeft) {
						s.MoveHExact (num);
					}
				}
				// If the block moved right
			} else if (num > 0) {
				// Move the entities on the right of the block
				if (EntitiesOnRight.Count > 0) {
					foreach (Actor s in EntitiesOnRight) {
						s.MoveHExact (num);
					}
				}
			} 
		}
		return false;
	}

	public bool MoveVExactPlatform(int moveV) {
		int num = (int)Mathf.Sign((float)moveV);
		while (moveV != 0) {
			bool solid = CheckColInDir(Vector2.up * (float)num, bumper_layer);
			if (solid) {
				this.movementCounter.y = 0f;
				return true;
			}
			moveV -= num;
			// Move the platform
			transform.position = new Vector2 (transform.position.x, transform.position.y + (float)num);

			// Move the entities on top of the platform
			if (EntitiesOnTop.Count > 0) {
				foreach (Actor s in EntitiesOnTop) {
					s.MoveVExact (num);
				}
			}
			// If the block moved down
			if (num < 0) {
				// Move the entities on the bottom of the block
				if (EntitiesOnBot.Count > 0) {
					foreach (Actor s in EntitiesOnBot) {
						s.MoveVExact (num);
					}
				}
			}
		}
		return false;
	}

	public bool CheckColInDir (Vector2 dir, LayerMask layer) {
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

		return Physics2D.OverlapArea(leftcorner, rightcorner, layer);
	}

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
}