using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPlatform : MonoBehaviour
{

    [Header("Массив объектов управления")]
    public GameObject[] ButtonControl;  // Массив объектов управления
    [HideInInspector]
    public Button[] Owner;              // Массив скриптов объектов управления
    [Header("Массив точек перемещения")]
    public GameObject PointParent;      // Предок массива точек
    [HideInInspector]
    public Transform[] PointMassive;    // Массив позиций точек
    [HideInInspector]
    public float[] PointDelayMassive;   // Массив времен задержки на точках
    [Header("Параметры")]
    public GameObject Door;             // Объект, который будет перемещаться
    public enum MoveType
    {
        OneMove,                        // Движение в одну сторону
        CycleMove                       // Движение циклично
    }
    public MoveType Motion;             // Выбранный тип перемещения объекта
    [HideInInspector]
    public bool Active;                 // Проверка на активацию всех объектов управления
    [HideInInspector]
    public int NextPointID = 1;         // ID следующей точки
    [HideInInspector]
    public int BackPointID = 0;         // ID предыдущей точки
    [Range(0.1f, 1)]
    public float NextSpeed = 0.3f;         // Скорость при движении к следующей точке 

    public float MinDist = 0;           // Минимальное расстояние, на которое следует приблизиться к точке
    public float Timer = 0;             // Таймер на задержку в точке


    [Header("Bumper Layer")]
    public LayerMask bumper_layer; // Layer of the bumper which makes this platform move
    public LayerMask entities_layer;

    private int Mnum = 0;


    void Awake()
    {
        // Get the Collider
        SetPoints();
        Owner = new Button[ButtonControl.Length];
        for (int i = 0; i < Owner.Length; i++)
        {
            Owner[i] = ButtonControl[i].GetComponent<Button>();
        }
    }
    private void SetPoints()
    {
        int i = PointParent.GetComponentInChildren<Transform>().childCount;
        PointMassive = new Transform[i];
        PointDelayMassive = new float[i];
        for (int j = 0; j < i; j++)
        {
            PointMassive[j] = PointParent.GetComponentInChildren<Transform>().GetChild(j);
            if (PointMassive[j].GetComponentInChildren<PointParameters>())
            {
                PointDelayMassive[j] = PointMassive[j].GetComponentInChildren<PointParameters>().DelayTime;
            }
            else
            {
                PointDelayMassive[j] = 0;
            }
        }
    }

    void Update()
    {
        CheckAllControls();

        if (Active)
        {
            if (Motion == MoveType.OneMove)
            {
                Move(Active, Motion);
            }

            if (Motion == MoveType.CycleMove)
            {
                Move(Active, Motion);
            }
        }
        else
        {
            if (Motion == MoveType.OneMove)
            {
                Move(Active, Motion);
            }
        }
    }

    private void Move(bool Active, MoveType Motion)
    {
        int ID;
        if (Active)
        {
            ID = NextPointID;
        }
        else
        {
            ID = BackPointID;
        }

        if ((PointMassive.Length - 1) >= 1)
        {
            if (Vector3.Distance(PointMassive[ID].position, Door.transform.position) > MinDist)
            {
                //transform.position = new Vector2 (transform.position.x + (float)Mnum, transform.position.y);
                Door.transform.position = Vector3.MoveTowards(Door.transform.position, PointMassive[ID].position, NextSpeed + (float)Mnum);
                //Door.transform.position = Vector3.MoveTowards(Door.transform.position, PointMassive[ID].position, NextSpeed * Time.deltaTime);
            }
            else
            {
                if (Timer > PointDelayMassive[ID])
                {
                    if (Active)
                    {
                        if (NextPointID < (PointMassive.Length - 1))
                        {
                            NextPointID++; BackPointID++;
                        }
                        if ((Motion == MoveType.CycleMove) && (NextPointID == (PointMassive.Length - 1)))
                        {
                            NextPointID = 0; BackPointID = PointMassive.Length - 1;
                        }
                    }
                    else
                    {
                        if (BackPointID > 0)
                        {
                            NextPointID--; BackPointID--;
                        }
                    }
                    Timer = 0;
                }
                else
                {
                    Timer += Time.deltaTime;
                }
            }
        }
    }


    public void CheckAllControls()
    {
        //Debug.Log(Owner[0].Active + " - " + Owner[1].Active);
        for (int i = 0; i < ButtonControl.Length; i++)
        {
            if (Owner[i].Active == false)
            {
                Active = false;
                break;
            }
            else
            {
                Active = true;
            }
        }
    }
}