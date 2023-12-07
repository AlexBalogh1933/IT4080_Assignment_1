using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScore : MonoBehaviour
{
    private Canvas canvas;
    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            canvas.enabled = !canvas.enabled;
        }
    }
}
