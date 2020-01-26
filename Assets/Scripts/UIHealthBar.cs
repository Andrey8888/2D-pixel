using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour {

	public Sprite redHeart, greyHeart;
	public Image[] hearts;
	public CanvasGroup myCG;

	public static UIHealthBar instance = null;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		if (myCG == null) {
			myCG = GetComponent<CanvasGroup> ();

			if (myCG == null) {
				Debug.Log ("There is no canvas group attached to the healthbar");
			}
		}
	}

	public void setHealthBar (int amount) {
		for (int i = 0; i < hearts.Length; i++) {
			if (amount > i) {
				var rend = hearts [i];

				if (rend != null) {
					if (rend.sprite != redHeart)
						rend.sprite = redHeart;
				} 
			} else {
				var rend = hearts [i];

				if (rend != null) {
					if (rend.sprite != greyHeart)
						rend.sprite = greyHeart;
				} 
			}
		}
	}

	public void SetVisible () {
		if (myCG != null) {
			myCG.alpha = 1;
		}
	}

	public void SetInVisible () {
		if (myCG != null) {
			myCG.alpha = 0;
		}
	}

}
