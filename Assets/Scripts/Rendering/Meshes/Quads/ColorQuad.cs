﻿using UnityEngine;

public class ColorQuad : BaseQuad
{
    public void Init(Material material = null)
    {
        InitQuadMesh();

        //init the array of colors
        Color[] colors = new Color[4];
        GetComponent<MeshFilter>().sharedMesh.colors = colors;

        GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    /**
     * Set the one color for this quad
     * **/
    public void SetColor(Color color)
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        Color[] colors = mesh.colors;
        for (int i = 0; i != 4; i++)
        {
            colors[i] = color;
        }

        mesh.colors = colors;
    }
}
