using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Скрипт по обновлению положения объектов
//
// Принцип работы:
// void Start
// В момент запуска игры, скрипт ищет все игровые объекты с тэгом "Box" и запоминает их положение
// void ResetPosition
// Необходимо объявление в скрипте Игрока
// В случае гибели Игрока, возвращает запомненные объекты в их исходное положение
// 
// Скрипт готов к работе
// Рекомендуется сделать чтение тэгов, с которыми может работать Скрипт, более удобным
// Сделать это можно через объявление в Инспекторе Unity

public class ResetState : MonoBehaviour
{
    //public string[] Tages;         // Не работает, выдает ошибку - Tag: System.String[] is not defined
    private Vector3[] InitPosition;
    private GameObject[] ResetGOs;

    void Start() 
    {
        ResetGOs = GameObject.FindGameObjectsWithTag("Box");
        InitPosition = new Vector3[ResetGOs.Length];
        for (int i = 0; i < ResetGOs.Length; ++i)
        {
            {
                InitPosition[i] = ResetGOs[i].transform.position;
            }
        }
    }
    public void ResetPosition()
    {
        for (int i = 0; i < ResetGOs.Length; ++i)
        {
            {
                ResetGOs[i].transform.position = InitPosition[i];
            }
        }
    }
}