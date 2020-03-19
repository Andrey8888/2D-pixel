using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {
    
    public enum ButtonSort
    {
        OnWallButton,   // Настенная (на пейзаже)
        OnBorderButton, // Боковая (на объектах)
        Lever
    }
    [Header("Параметры кнопки")]
    public ButtonSort ButtonType = ButtonSort.OnWallButton;
    public enum Interact
    {
        ByPlayer,       // Только игрок
        ByPlayerAndBox, // Только игрок и ящики
        ByBox,          // Только ящики
        ByArrow         // Только стрела
    }
    public Interact InteractType = Interact.ByPlayer;
    public enum HowWork
    {
        OnlyPress,      // Одиночное нажатие
        PolyPress,      // Множество нажатий
        Holding         // Удержание
    }

    public enum TypeAction
    {
        moveAction,      
        appearAction,
        disappearAction
    }

    public HowWork WorkType = HowWork.OnlyPress;
    public TypeAction typeAction = TypeAction.moveAction;
    public float ActiveTime = 5;
    public Sprite[] sprites = new Sprite[2];
    [Header("Управляемые объекты")]
    public GameObject[] DependObjects;
    [Header("Нерегулируемые параметры")]
    public bool Active = false;
    public float Timer = 0;
    // Use this for initialization
    void Start ()
    {
        Timer = ActiveTime;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Timer < ActiveTime)
        {
            Timer += Time.deltaTime;
        }
	}
    
    void OnTriggerStay2D(Collider2D col)
    {
        if (ButtonType == ButtonSort.OnBorderButton)
        {            
            if (WorkType == HowWork.OnlyPress)
            {
                if (InteractType == Interact.ByPlayer)
                {
                    if (col.CompareTag("Player"))
                    {
                        Active = true;
                        Activation(DependObjects, Active);
                    }
                }

                if (InteractType == Interact.ByPlayerAndBox)
                {
                    if (col.CompareTag("Player") || col.CompareTag("Box"))
                    {
                        Active = true;
                        Activation(DependObjects, Active);
                    }
                }

                if (InteractType == Interact.ByBox)
                {
                    if ( col.CompareTag("Box"))
                    {
                        Active = true;
                        Activation(DependObjects, Active);
                    }
                }
            }
        }

        if (ButtonType == ButtonSort.OnWallButton)
        {
            if (WorkType == HowWork.Holding)
            {
                if (InteractType == Interact.ByPlayer)
                {
                    if ((col.CompareTag("Player")) && Input.GetKey(KeyCode.E))
                    {
                        Active = true;
                        Activation(DependObjects, Active);
                    }
                    else
                    {
                        Active = false;
                        Activation(DependObjects, Active);
                    }
                }
            }
            
            if (WorkType == HowWork.OnlyPress)
            {
                if (InteractType == Interact.ByPlayer)
                {
                    if ((col.CompareTag("Player")) && Input.GetKey(KeyCode.E))
                    {
                        Active = true;
                        Activation(DependObjects, Active);
                    }
                }
            }

            if (WorkType == HowWork.PolyPress)
            {
                if (InteractType == Interact.ByPlayer)
                {
                    if ((Timer >= ActiveTime) && (col.CompareTag("Player")) && Input.GetKeyDown(KeyCode.E))
                    {
                        Active = !Active;
                        Timer = 0;
                        Activation(DependObjects, Active);
                    }
                }
            }
        }

        if (ButtonType == ButtonSort.Lever)
        {
            if (WorkType == HowWork.Holding)
            {
                if (InteractType == Interact.ByPlayer)
                {
                    if ((col.CompareTag("Player")) && (Input.GetKey(KeyCode.E)))
                    {
                        Active = true;
                        Activation(DependObjects, Active);
                    }
                    else
                    {
                        Active = false;
                        Activation(DependObjects, Active);
                    }
                }
            }

            if (WorkType == HowWork.OnlyPress)
            {
                if (InteractType == Interact.ByPlayer)
                {
                    if ((col.CompareTag("Player")) && (Input.GetKeyDown(KeyCode.E)))
                    {
                        Active = true;
                        Activation(DependObjects, Active);
                    }
                }
            }

            if (WorkType == HowWork.PolyPress)
            {
                if (InteractType == Interact.ByPlayer)
                {
                    if ((Timer >= ActiveTime) && (col.CompareTag("Player")) && (Input.GetKeyDown(KeyCode.E)))
                    {
                        Active = !Active;
                        Timer = 0;
                        Activation(DependObjects, Active);
                    }
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if ((Active == false) && (WorkType == HowWork.OnlyPress) && (InteractType == Interact.ByArrow) && (col.tag.CompareTo("Arrow") == 0))
        {
            Active = !Active;
            Activation(DependObjects, Active);
        }

        if (ButtonType == ButtonSort.OnBorderButton)
        {
            if (WorkType == HowWork.Holding)
            {
                if (InteractType == Interact.ByPlayer)
                {
                    if (col.CompareTag("Player"))
                    {
                        Active = true;
                        Activation(DependObjects, Active);
                    }
                }

                if (InteractType == Interact.ByBox)
                {
                    if  (col.CompareTag("Box"))
                    {
                        Active = true;
                        Activation(DependObjects, Active);
                    }
                }

                if (InteractType == Interact.ByPlayerAndBox)
                {
                    if (col.CompareTag("Player") || col.CompareTag("Box"))
                    {
                        Active = true;
                        Activation(DependObjects, Active);
                    }
                }
            }

            if (WorkType == HowWork.OnlyPress)
            {
                if (InteractType == Interact.ByArrow)
                {
                    if (col.CompareTag("Arrow"))
                    {
                        Active = true;
                        Activation(DependObjects, Active);
                    }
                }
            }

            if (WorkType == HowWork.PolyPress)
            {
                if (InteractType == Interact.ByPlayer)
                {
                    if (col.CompareTag("Player"))
                    {
                        Active = !Active;
                        Activation(DependObjects, Active);
                    }
                }

                if (InteractType == Interact.ByPlayerAndBox)
                {
                    if (col.CompareTag("Player") || col.CompareTag("Box"))
                    {
                        Active = !Active;
                        Activation(DependObjects, Active);
                    }
                }

                if (InteractType == Interact.ByBox)
                {
                    if (col.CompareTag("Box"))
                    {
                        Active = !Active;
                        Activation(DependObjects, Active);
                    }
                }

                if (InteractType == Interact.ByArrow)
                {
                    if (col.CompareTag("Arrow"))
                    {
                        Active = !Active;
                        Activation(DependObjects, Active);
                    }
                }
            }

        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (ButtonType == ButtonSort.OnBorderButton)
        {
            if (WorkType == HowWork.Holding)
            {
                if (InteractType == Interact.ByPlayer)
                {
                    if (col.CompareTag("Player"))
                    {
                        Active = false;
                        Activation(DependObjects, Active);
                    }
                }

                if (InteractType == Interact.ByBox)
                {
                    if (col.CompareTag("Box"))
                    {
                        Active = false;
                        Activation(DependObjects, Active);
                    }
                }

                if (InteractType == Interact.ByPlayerAndBox)
                {
                    if (col.CompareTag("Player") || col.CompareTag("Box"))
                    {
                        Active = false;
                        Activation(DependObjects, Active);
                    }
                }
            }
        }
    }

    public void Activation(GameObject[] GameObjects, bool Active)
    {
        int i;
        DoorPlatform SetWork;
        for (i = 0; i < GameObjects.Length; i++)
        {
            if (typeAction == TypeAction.disappearAction)
            {
                GameObjects[i].SetActive(!Active);
            }
            else if (typeAction == TypeAction.appearAction)
            {
                GameObjects[i].SetActive(Active);
            }
            else {
 
                SetWork = GameObjects[i].GetComponentInChildren<DoorPlatform>();
                SetWork.Active = !SetWork.Active;
            }
        }
        ButtonMove(Active);
    }

    public void ButtonMove(bool Active)
    {
        if (Active)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
            // Screenshake
            if (PixelCameraController.instance != null)
            {
                PixelCameraController.instance.DirectionalShake(Vector2.right, 0.1f);
            }
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
            // Screenshake
            if (PixelCameraController.instance != null)
            {
                PixelCameraController.instance.DirectionalShake(Vector2.right, 0.1f);
            }
        }
    }
}