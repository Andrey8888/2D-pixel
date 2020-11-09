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
    private bool IsDrop = false;
    public Transform dropPoint;

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

    void Update()
    {
        UpdateSprite();
    }
    void OnTriggerStay2D(Collider2D col)
    {
		if (!IsDrop && col.CompareTag("Player") && Input.GetKey(KeyCode.E) && col.GetComponent<Player>() != null)
		{
            IsDrop = true;
            StartCoroutine("ActivationCoroutine");
			col.GetComponent<Player>().Action();
		}
	}
    void Open()
    {
        if (Dropout != null)
        {
            var rnd = Random.Range(0, Dropout.Length);
            var p = Instantiate(Dropout[rnd], dropPoint.transform.position, Quaternion.identity);
        }
    }
	void UpdateSprite()
    {
            if (fsm.State == States.Open)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
                {
                    animator.Play("Death");
                }
            }
    }
	
	IEnumerator ActivationCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
		fsm.ChangeState(States.Open, StateTransition.Overwrite);
        Open();
    }
}