using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Voxel
{
    public bool solid;

    public Color color;
    
    public Voxel(Color color)
    {
        this.color = color;
        solid = true;
    }

    public Vector2 GetUV0()
    {
        return new Vector2(color.r, color.g);
    }

    public Vector2 GetUV1()
    {
        return new Vector2(color.g, color.a);
    }
}
