using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlasher : MonoBehaviour {

	public SpriteRenderer spriteRenderer;

	IEnumerator flashRoutine;

	public static float timeToFlashFor = .2f;


	public void FlashMe () {
		if (flashRoutine != null) {
			StopCoroutine (flashRoutine);
		}

		flashRoutine = FlashRoutine ();
		StartCoroutine (flashRoutine);
	}

	IEnumerator FlashRoutine () {
		spriteRenderer.material.SetFloat ("_FlashAmount", 1f);

		yield return new WaitForSeconds (timeToFlashFor);

		spriteRenderer.material.SetFloat ("_FlashAmount", 0f);
	}
}
