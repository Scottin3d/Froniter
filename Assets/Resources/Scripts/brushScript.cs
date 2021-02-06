using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class brushScript : MonoBehaviour
{
    public LayerMask ignore;
    public bool IsOn = false;
    private Vector3 inputPostion;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) {
            IsOn = !IsOn;
        }

        GetComponent<MeshRenderer>().enabled = IsOn;

        if (IsOn) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, ~ignore)) {
                Debug.Log(hit.point);
                HandleMovement(hit.point);
            }
        }
            
    }

    private void HandleMovement(Vector3 inputPostion) {
        inputPostion.y += 0.01f;
        transform.position = inputPostion;
    }
}
