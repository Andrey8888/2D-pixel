using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;
using Sirenix.OdinInspector;

public class Button_NewOne : MonoBehaviour
{ 
#region Params
    void Start()
    {
        LoadActivators();
    }
   
    public enum TypeOfSearch
    {
        Auto,
        Manual
    }

    [BoxGroup("Activators Array"), HideLabel, EnumToggleButtons]
    public TypeOfSearch HowSetActivators = TypeOfSearch.Auto;
    [BoxGroup("Activators Array"), HideLabel]
    public Activator[] Activators;
    [FoldoutGroup("Params", expanded: true)]
    public bool Active = false;
    [FoldoutGroup("Params", expanded: true)]
    private float Lola_Timer = 0;
    [FoldoutGroup("Params", expanded: true), MinValue(0)]
    public float ReloadTimer = 5;
    #endregion

    #region Mechanics
    private void LoadActivators()
    {
        switch (HowSetActivators)
        {
            case TypeOfSearch.Auto:
                int X = 0;
                for (int i = 0; i <= this.transform.childCount - 1; i++)
                {
                    if (this.transform.GetChild(i).GetComponent<Activator>() != null)
                    {
                        X++;
                    }
                }
                Activators = new Activator[X];
                for (int i = 0; i <= this.transform.childCount - 1; i++)
                {
                    var Joker = this.transform.GetChild(i).GetComponent<Activator>();
                    if (Joker != null)
                    {
                        Activators[X - 1] = Joker;
                        X--;
                    }
                }
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
			Activation();
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (!Active)
        {
            if (col.CompareTag("Player") && col.CompareTag("Enemy"))
            {
                Active = true;
            }
        }
    }
	
    void OnTriggerExit2D(Collider2D col)
    {
		if (col.CompareTag("Enemy"))
		{
			Active = false;
		}
	}
	
    private void Activation()
    {
        for (int i = 0; i <= Activators.Length - 1; i++)
        {
            //Activators[i].enabled = true;
        }
    }
    #endregion
}