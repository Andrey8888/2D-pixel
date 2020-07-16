using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopUp : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    public void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }
    public void Setup(int damagePopUp, bool isCriticalHit)
    {
        textMesh.SetText(damagePopUp.ToString());
        if (!isCriticalHit)
        {
            // normal hit
            textMesh.fontSize = 90;
            textColor = Utilities.GetColorFromString("D9D9D9");
        }
        else
        {
            // critical hit
            textMesh.fontSize = 110;
            textColor = Utilities.GetColorFromString("FF0000");
        }
        textMesh.color = textColor;
        disappearTimer = 60f;
        moveVector = new Vector3(4, 2) * 3f;
    }

    public void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 1f * Time.deltaTime;
        disappearTimer = -Time.deltaTime;

        if (disappearTimer < 0)
        {
            float disappearSpeed = 1f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }

}
