using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainMenuCamera : MonoBehaviour
{
    public Transform lookat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(lookat, Vector3.up);
    }
}
