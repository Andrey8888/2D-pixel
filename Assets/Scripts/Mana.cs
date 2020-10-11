using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mana : MonoBehaviour
{

    public UnityEvent OnTakeExpenseEvent;
    public UnityEvent OnTakeRefreshEvent;

    [Header("Max/Starting Mana")]
    public int maxMana;
    [Header("Current Health")]
    public int mana;

    void Start()
    {
        mana = maxMana;
    }

    public bool TakeExpense(int amount)
    {
        mana = Mathf.Max(0, mana - amount);

        if (OnTakeExpenseEvent != null)
            OnTakeExpenseEvent.Invoke();

        return true;
    }

    public bool TakeRefresh(int amount)
    {

        mana = Mathf.Min(maxMana, mana + amount);

        if (OnTakeRefreshEvent != null)
            OnTakeRefreshEvent.Invoke();

        return true;
    }

    public void SetUIManaBar()
    {
        if (UIHealthBar.instance != null)
        {
            UIHealthBar.instance.setHealthBar(mana);
        }
    }

    public void SetUIHealthBarFadeDamage()
    {
        if (ManaBarFade.instance != null)
        {
            ManaBarFade.instance.Cast(mana);
        }
    }

    public void SetUIHealthBarFadeHeal()
    {
        if (ManaBarFade.instance != null)
        {
            ManaBarFade.instance.Refill(mana);
        }
    }
}
