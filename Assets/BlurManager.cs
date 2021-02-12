using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlurManager : MonoBehaviour
{
    public Camera blurCamera;
    public Material blurMateiral;
    void Start()
    {
        if (blurCamera.targetTexture != null) {
            blurCamera.targetTexture.Release();
        }

        blurCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32,1);
        blurMateiral.SetTexture("_RenTex", blurCamera.targetTexture);
    }
    private void Update() {
        
    }
}
