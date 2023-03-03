using B83.MeshTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VisualScripting;
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


}

public static class VoxelExtensions
{
    public static void WriteVoxel(this BinaryWriter writer, Voxel v)
    {
        writer.Write(v.solid);
        writer.WriteColor32(v.color);
    }

    public static Voxel ReadVoxel(this BinaryReader reader)
    {
        Voxel v = new Voxel();
        v.solid = reader.ReadBoolean();
        v.color = reader.ReadColor32();
        return v;
    }

    public static void WriteVector3Int (this BinaryWriter writer, Vector3Int vec)
    {
        writer.Write(vec.x); writer.Write(vec.y); writer.Write(vec.z);
    }

    public static Vector3Int ReadVector3Int(this BinaryReader reader)
    {
        return new Vector3Int(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
    }
}
