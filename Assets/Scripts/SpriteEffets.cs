using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteEffets : MonoBehaviour {

	public SpriteRenderer spriteRenderer;
    public GameObject ice;

	public void Poison () {

        spriteRenderer.material.color = Color.green;
	}
    public void PoisonEnd()
    {
        spriteRenderer.material.color = Color.white;
    }

    public void Burned()
    {

        spriteRenderer.material.color = Color.red;
    }
    public void BurnedEnd()
    {
        spriteRenderer.material.color = Color.white;
    }

    public void Freezed()
    {
        spriteRenderer.material.color = Color.cyan;
        ice.SetActive(true);
    }
    public void FreezedEnd()
    {
        ice.SetActive(false);
        spriteRenderer.material.color = Color.white;
    }
}
