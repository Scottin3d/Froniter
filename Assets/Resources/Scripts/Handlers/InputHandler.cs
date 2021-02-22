using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputHandler : MonoBehaviour
{
    public static InputHandler current;

    public Action OnPress_i;
    public Action OnPress_l;
    public Action OnPress_g;
    public Action OnPress_r;
    public Action OnPress_t;

    // Start is called before the first frame update
    void Awake()
    {
        current = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) {
            OnPress_g?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.I)) {
            OnPress_i?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            OnPress_l?.Invoke();
        }

        
        if (Input.GetKeyDown(KeyCode.R)) {
            OnPress_r?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.T)) {
            OnPress_t?.Invoke();
        }
    }
}
