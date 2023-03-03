using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class Voxels
{
    public int width = 32, height = 32, length = 32;
    public int count = 0;

    public Voxel[] voxels;

    public void SetSize(Vector3Int size, bool reset)
    {
        SetSize(size.x, size.y, size.z, reset);
    }

    public void SetSize(int w, int h, int l, bool reset)
    {
        Voxel[] vox = new Voxel[w * h * l];

        if (!reset)
        {
            var X = Mathf.Min(w, width);
            var Y = Mathf.Min(h, height);
            var Z = Mathf.Min(l, length);


            for (int x = 0; x < X; x++)
            {
                for (int y = 0; y < Y; y++)
                {
                    for (int z = 0; z < Z; z++)
                    {
                        vox[x + y * h + z * l] = voxels[GetIndex(x,y,z)];
                    }
                }
            }
        }


        width = w;
        height = h;
        length = l;
        voxels = vox;
    }

    public int GetIndex(int x, int y, int z)
    {
        return x + y * height + z * length;
    }

    public void AddVoxel(Vector3Int pos, Voxel voxel)
    {
        AddVoxel(pos.x, pos.y, pos.z, voxel);
    }

    public void AddVoxel(int x, int y, int z, Voxel voxel)
    {
        if (!IsValidPosition(x, y, z) || CheckForVoxel(x, y, z) || !voxel.solid)
        {
            return;
        }

        voxels[GetIndex(x,y,z)] = voxel;
        count++;
    }

    public void RemoveVoxel(Vector3Int pos)
    {
        RemoveVoxel(pos.x, pos.y, pos.z);
    }

    public void RemoveVoxel(int x, int y, int z)
    {
        if (count == 1)
        {
            Debug.Log("Cannot remove last voxel");
            return;
        }

        if (!IsValidPosition(x, y, z))
        {
            return;
        }

        voxels[GetIndex(x,y,z)].solid = false;
        count--;
    }

    public bool CheckForVoxel(Vector3Int pos)
    {
        return CheckForVoxel(pos.x, pos.y, pos.z);
    }

    public bool CheckForVoxel(int x, int y, int z)
    {
        if (!IsValidPosition(x, y, z))
        {
            return false;
        }

        if (GetVoxel(x, y, z).solid)
        {
            return true;
        }
        return false;
    }

    public Voxel GetVoxel(Vector3Int pos)
    {
        return GetVoxel(pos.x, pos.y, pos.z);
    }

    public Voxel GetVoxel(int x, int y, int z)
    {
        return voxels[GetIndex(x,y,z)];
    }

    /// <summary>
    /// Checks if there is a (non-air) Block at the given position 
    /// </summary>
    public bool CheckVoxel(Vector3Int pos)
    {
        int x = pos.x;
        int y = pos.y;
        int z = pos.z;

        if (x < 0 || x > width - 1 || y < 0 || y > height - 1 || z < 0 || z > length - 1)
            return false;

        if (!GetVoxel(x, y, z).solid)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool IsValidPosition(int x, int y, int z)
    {
        if (x < 0 || x >= width)
        {
            return false;
        }
        if (y < 0 || y >= height)
        {
            return false;
        }
        if (z < 0 || z >= length)
        {
            return false;
        }
        return true;
    }

    public void SaveToFile(string path)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write(fileCheckID);

        // Write size to stream
        writer.WriteVector3Int(new Vector3Int(width, height, length));

        // Write voxels to stream 
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    writer.WriteVoxel(GetVoxel(x, y, z));
                }
            }
        }

        File.WriteAllBytes(path, stream.ToArray());
    }

    public void LoadFromFile(string path)
    {
        var stream = new MemoryStream(File.ReadAllBytes(path));
        BinaryReader reader = new BinaryReader(stream);

        // Primitive way of checking for valid file format
        if (reader.ReadUInt32() != fileCheckID)
        {
            Debug.LogError("Invalid Voxel Object File");
            return;
        }

        var size = reader.ReadVector3Int();


        SetSize(size, true);


        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    AddVoxel(x, y, z, reader.ReadVoxel());
                }
            }
        }

    }

    // Primitive way of checking for valid file format
    const uint fileCheckID = 0x766f7865; // "voxe"  
}
