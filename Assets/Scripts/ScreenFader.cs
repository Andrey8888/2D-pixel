using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour {

	public Image FadeImage;
	public float fadeSpeed = 1.5f;
	//[HideInInspector]
	//public bool sceneStarted = false;
	//private string loadLevelName = "";

	IEnumerator fadeRoutine;

	void Update () {
	}

	public void FadeIn () {
		if (fadeRoutine != null) {
			StopCoroutine (fadeRoutine);
		}

		fadeRoutine = Fade(Color.black);
		StartCoroutine (fadeRoutine);
	}

	public void FadeOut () {
		if (fadeRoutine != null) {
			StopCoroutine (fadeRoutine);
		}

		fadeRoutine = Fade(Color.clear);
		StartCoroutine (fadeRoutine);
	}

	private IEnumerator Fade(Color targetColor) {
		Color originalColor = FadeImage.color;
		for (float t = 0; t < 1; t += Time.deltaTime / fadeSpeed) {
			FadeImage.color = Color.Lerp (originalColor, targetColor, t);
			yield return null;
		}

		FadeImage.color = targetColor;

		/*
		if (FadeImage.color.a == 0) {
			FadeImage.gameObject.SetActive (false);
		} else {
			StartCoroutine (Fade (Color.black));
		}
		*/
	}

	/*
	public void EndScene(string sceneName) {
		FadeImage.gameObject.SetActive (true);
		//loadLevelName = sceneName;
		StartCoroutine (Fade (Color.black));
	}
	*/
}
