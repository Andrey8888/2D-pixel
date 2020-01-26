using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour {

	private Vector2 Position;

	[Header ("Direction")]
	public Vector2 Speed;
	public bool randomRangeXSpeed = false;
	public bool randomRangeYSpeed = false;
	public Vector2 movementCounter = Vector2.zero;  // Counter for subpixel movement

	[Header ("Life Time")]
	public float Life;
	private float startingLife;

	[Header ("Color Lerp")]
	public Color startingColor, finalColor;

	[Header ("Sprite Renderer")]
	public SpriteRenderer spriteRenderer;

	void Awake () {
		if (spriteRenderer == null) {
			spriteRenderer = GetComponent<SpriteRenderer> ();
		}
		startingLife = Life;
	}

	// Use this for initialization
	void Start () {
		Position = new Vector2 (Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
		transform.position = Position;

		if (randomRangeXSpeed) {
			Speed.x = Random.Range (-Speed.x, Speed.x);
		}

		if (randomRangeYSpeed) {
			Speed.y = Random.Range (-Speed.y, Speed.y);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Reduce the lifetime of the particle
		if (Life > 0f) {
			Life -= Time.deltaTime;

			// If life is lower or equal to 0 destroy the particle
			if (Life <= 0f)
			{
				DestroyMe ();
				return;
			}
		}

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

		//Position += Speed;
		transform.position = Position;	

		// Color
		var targetcolor = Color.Lerp(finalColor, startingColor, Life / startingLife);
		if (spriteRenderer.color != targetcolor) {
			spriteRenderer.color = targetcolor;
		}
			
	}
		

	public void DestroyMe () {
		Destroy (gameObject);
	}
}
