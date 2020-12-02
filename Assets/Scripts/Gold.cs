using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gold : MonoBehaviour {

    public int countMoney;
	private GameObject countMoneyText;
	
	public void Start()
	{
		countMoney = 0;
	}

    public bool Take_Money(int ammount)
    {
        countMoney += ammount;
        countMoneyText.GetComponent<Text>().text = countMoney.ToString();
        return true;
    }

    public bool Spend_Money(int ammount)
    {
        if (countMoney >= ammount && countMoney > 0)
        {
            countMoney -= ammount;
            countMoneyText.GetComponent<Text>().text = countMoney.ToString();
            return true;
        }
        return false;
    }
}
