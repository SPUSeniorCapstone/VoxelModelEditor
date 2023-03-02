using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelObject : MonoBehaviour
{
    public int width = 32, height = 32, length = 32;


    Voxel[,,] voxels;

    private void Start()
    {
        voxels = new Voxel[width, height, length];

        int x = (width / 2)-1, y = (height / 2)-1, z = (length / 2)-1;

        for(int i = 0; i <= 1; i++)
        {
            for(int j = 0; j <= 1; j++)
            {
                for(int k = 0; k <= 1; k++)
                {
                    AddVoxel(x + i, y + j, z + k, new Voxel(Color.black));
                }
            }
        }

        ReloadMesh();

    }

    public void AddVoxel(Vector3Int pos, Voxel voxel)
    {
        AddVoxel(pos.x, pos.y, pos.z, voxel);
    }

    public void AddVoxel(int x, int y, int z, Voxel voxel)
    {
        if (!IsValidPosition(x, y, z) || CheckForVoxel(x,y,z))
        {
            return;
        }

        voxels[x,y,z] = voxel;
    }
    
    public void RemoveVoxel(Vector3Int pos)
    {
        RemoveVoxel(pos.x, pos.y, pos.z);
    }

    public void RemoveVoxel(int x, int y, int z)
    {
        if(!IsValidPosition(x, y, z))
        {
            return;
        }

        voxels[x, y, z].solid = false;
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

        if (GetVoxel(x,y,z).solid)
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
        return voxels[x, y, z];
    }

    /// <summary>
    /// Reloads the Chunk Mesh
    /// </summary>
    public void ReloadMesh()
    {
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter.sharedMesh == null)
        {
            meshFilter.sharedMesh = new Mesh();
        }

        int index = 0;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Color> colors = new List<Color>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    DrawVoxel(new Vector3Int(x, y, z), voxels[x,y,z]);
                }
            }
        }

        Mesh mesh = new Mesh();

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetColors(colors);
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;

        void DrawVoxel(Vector3Int pos, Voxel voxel)
        {
            if (!voxel.solid)
            {
                return;
            }

            ///
            /// 
            /// Help from https://github.com/b3agz/Code-A-Game-Like-Minecraft-In-Unity/blob/master/01-the-first-voxel/Assets/Scripts/Chunk.cs
            for (int i = 0; i < 6; i++)
            {
                if (!CheckVoxel(pos + VoxelData.faceChecks[i]))
                {
                    Vector2 u0 = voxel.GetUV0();
                    Vector2 u1 = voxel.GetUV1();

                    vertices.Add(pos + VoxelData.Vertices[VoxelData.Triangles[i, 0]]);
                    vertices.Add(pos + VoxelData.Vertices[VoxelData.Triangles[i, 1]]);
                    vertices.Add(pos + VoxelData.Vertices[VoxelData.Triangles[i, 2]]);
                    vertices.Add(pos + VoxelData.Vertices[VoxelData.Triangles[i, 3]]);
                    colors.Add(voxel.color);
                    colors.Add(voxel.color);
                    colors.Add(voxel.color);
                    colors.Add(voxel.color);
                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);
                    triangles.Add(index + 2);
                    triangles.Add(index + 1);
                    triangles.Add(index + 3);
                    index += 4;
                }
            }
        }
    }

    /// <summary>
    /// Checks if there is a (non-air) Block at the given position 
    /// </summary>
    bool CheckVoxel(Vector3Int pos)
    {
        int x = pos.x;
        int y = pos.y;
        int z = pos.z;

        if (x < 0 || x > width - 1 || y < 0 || y > height - 1 || z < 0 || z > length - 1)
            return false;

        if (!voxels[x,y,z].solid)
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
        if(x < 0 || x >= width)
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
}
