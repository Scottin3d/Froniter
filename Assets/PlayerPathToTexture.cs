using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPathToTexture : MonoBehaviour
{
    public MeshRenderer meshRender;
    public Transform player;
    public Material material;
    public Vector3 pPos;
    public float pathStep;
    public int textureSize;
    Texture2D texture;
    Color[] pixelColors;

    float[,] playerPathValues;
    // Start is called before the first frame update
    void Start()
    {
        texture = new Texture2D(textureSize, textureSize);
        meshRender = GetComponent<MeshRenderer>();
        meshRender.material = material;
        material.mainTexture = texture;
        pixelColors = new Color[textureSize * textureSize];
        playerPathValues = new float[textureSize, textureSize];

    }

    private void FixedUpdate() {
        pPos = player.position;
        if (player.position.x < textureSize && player.position.z < textureSize) { 
        
            playerPathValues[Mathf.RoundToInt(pPos.x), Mathf.RoundToInt(pPos.z)] = 1;
            UpdateMask();
        }
    }

    private void UpdateMask() {
        //playerPathValues[Mathf.RoundToInt(player.position.x), Mathf.RoundToInt(player.position.z)] += pathStep;

        
        for (int z = 0; z < textureSize; z++) {
            for (int x = 0; x < textureSize; x++) {
                pixelColors[z * textureSize + x] = Color.Lerp(Color.black, Color.white, playerPathValues[x, z]);
            }
        }

        texture.SetPixels(pixelColors);
        texture.Apply();
    }
}
