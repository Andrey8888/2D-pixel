using System.Collections;
using System.Collections.Generic;
using MonsterLove.StateMachine;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator; // Reference to the animator

    // State Machine
    public StateMachine<States> fsm;

    public GameObject[] Dropout;

    public enum States
    {
        Normal,
		Open
    }

	public void Awake () 
    {
		fsm = StateMachine<States>.Initialize(this);
	}
    void Start()
    {
        fsm.ChangeState(States.Normal);
    }
	
	void OnTriggerStay2D(Collider2D col)
    {
		if (col.CompareTag("Player") && Input.GetKey(KeyCode.E) && col.GetComponent<Player>() != null)
		{
			StartCoroutine("ActivationCoroutine");
			col.GetComponent<Player>().Action();
		}
	}
    void Open_Update()
    {
	        if (Dropout != null)
        {
            var rnd = Random.Range(0, Dropout.Length);
            var p = Instantiate(Dropout[rnd], gameObject.transform.position, Quaternion.identity);
        }
	}
	void UpdateSprite()
    {
            if (fsm.State == States.Open)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
                {
                    animator.Play("Open");
                }
            }
    }
	
	IEnumerator ActivationCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
		fsm.ChangeState(States.Open, StateTransition.Overwrite);
    }
}