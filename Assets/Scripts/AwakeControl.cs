using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakeControl : MonoBehaviour
{
    public GameObject MainCamera, UI_Canvas;
    // Start is called before the first frame update
    void Awake()
    {
        if (MainCamera != null) { MainCamera.SetActive(true); } else { Debug.Log(">> No set MainCamera!"); }
        if (UI_Canvas != null) { UI_Canvas.SetActive(true); } else { Debug.Log(">> No set UI-Canvas!"); }
    }
}
