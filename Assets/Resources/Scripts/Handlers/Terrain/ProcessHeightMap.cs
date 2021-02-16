using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProcessHeightMap 
{
    public static Vector3[] ProcessMap(Texture2D heightmap) {
        Vector3[] processedHeightmap = new Vector3[heightmap.width * heightmap.height];
        Color[] colorMap = new Color[heightmap.width * heightmap.height];

        colorMap = heightmap.GetPixels();
        for (int i = 0; i < colorMap.Length; i++) {
            int x = (i % heightmap.width);
            int z = (i / heightmap.height) ;
            processedHeightmap[i] = new Vector3(x, colorMap[i].grayscale, z);
        }
        return processedHeightmap;
    }
}
