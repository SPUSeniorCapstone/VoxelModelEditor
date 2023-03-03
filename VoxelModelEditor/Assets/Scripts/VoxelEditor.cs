using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using USFB;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelEditor : MonoBehaviour
{
    public Voxels voxels;

    public float checkIncrement = 0.1f;
    public float reach = 8;

    public Transform highlight;
    public Transform placeBlock;

    public ColorPicker color;

    bool changed = false;

    public bool mirrorX,mirrorY,mirrorZ;
    public GameObject xPlane, yPlane, zPlane;

    public void MirrorX(bool x)
    {
        mirrorX = x;
    }
    public void MirrorY(bool y)
    {
        mirrorY = y;
    }
    public void MirrorZ(bool z)
    {
        mirrorZ = z;
    }

    private void Start()
    {
        //if(voxels.voxels == null)
        //{
        //    voxels.voxels = new Voxel[voxels.width, voxels.height, voxels.length];
        //}
        ReloadMesh();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BreakBlock(highlight.transform.position.ToVector3Int());
            changed = true;
        }
        if (Input.GetMouseButtonDown(1))
        {
            PlaceBlock(placeBlock.transform.position.ToVector3Int());
            changed = true;
        }


        SetHighlights();


        if (changed)
        {
            ReloadMesh();
            changed = false;
        }

        if(mirrorX) xPlane.SetActive(true);
        else xPlane.SetActive(false);

        if (mirrorZ) zPlane.SetActive(true);
        else zPlane.SetActive(false);

        if (mirrorY) yPlane.SetActive(true);
        else yPlane.SetActive(false);
    }

    /// <summary>
    /// Reloads the Chunk Mesh
    /// </summary>
    public void ReloadMesh()
    {
        if (voxels == null)
        {
            Debug.LogError("Cannot load null voxel mesh");
            return;
        }

        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter.sharedMesh == null)
        {
            meshFilter.sharedMesh = new Mesh();
        }

        int index = 0;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Color> colors = new List<Color>();

        for (int x = 0; x < voxels.width; x++)
        {
            for (int y = 0; y < voxels.height; y++)
            {
                for (int z = 0; z < voxels.length; z++)
                {
                    DrawVoxel(new Vector3Int(x, y, z), voxels.GetVoxel(x, y, z));
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
                if (!voxels.CheckVoxel(pos + VoxelData.faceChecks[i]))
                {
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
    public void ExportToFile(string path)
    {
        ReloadMesh();
        var bytes = B83.MeshTools.MeshSerializer.SerializeMesh(GetComponent<MeshFilter>().sharedMesh);
        File.WriteAllBytes(path, bytes);
    }

    void PlaceBlock(Vector3Int pos, bool x = false, bool y = false, bool z = false)
    {
        if (!placeBlock.gameObject.activeSelf)
        {
            return;
        }

        voxels.AddVoxel(pos, new Voxel(color.color));

        if (mirrorX && !x)
        {
            Vector3Int p = new Vector3Int(voxels.width - pos.x - 1, pos.y, pos.z);
            PlaceBlock(p, true, y, z);
        }
        if (mirrorY && !y)
        {
            Vector3Int p = new Vector3Int(pos.x, voxels.height - pos.y - 1, pos.z);
            PlaceBlock(p, x, true, z);
        }
        if (mirrorZ && !z)
        {
            Vector3Int p = new Vector3Int(pos.x, pos.y, voxels.length - pos.z - 1);
            PlaceBlock(p, x, y, true);
        }

    }

    void BreakBlock(Vector3Int pos, bool x = false, bool y = false, bool z = false)
    {
        if (!highlight.gameObject.activeSelf)
        {
            return;
        }

        voxels.RemoveVoxel(pos);

        if (mirrorX && !x)
        {
            Vector3Int p = new Vector3Int(voxels.width - pos.x - 1, pos.y, pos.z);
            BreakBlock(p, true, y, z);
        }
        if (mirrorY && !y)
        {
            Vector3Int p = new Vector3Int(pos.x, voxels.height - pos.y - 1, pos.z);
            BreakBlock(p, x, true, z);
        }
        if (mirrorZ && !z)
        {
            Vector3Int p = new Vector3Int(pos.x, pos.y, voxels.length - pos.z - 1);
            BreakBlock(p, x, y, true);
        }
    }

    public void SetHighlights()
    {
        float step = checkIncrement;
        Vector3 last = Camera.main.transform.position;
        Vector3 dir = Camera.main.ScreenPointToRay(Input.mousePosition).direction;

        while (step < reach)
        {
            Vector3 curr = last + dir * checkIncrement;
            if (CheckForVoxel(curr))
            {
                highlight.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);

                highlight.position = curr.ToVector3Int() + new Vector3(0.5f,0.5f,0.5f);
                placeBlock.position = last.ToVector3Int() + new Vector3(0.5f, 0.5f, 0.5f);
                return;
            }
            last = curr;
            step += checkIncrement;
        }

        highlight.gameObject.SetActive(false);
        placeBlock.gameObject.SetActive(false);
    }

    public bool CheckForVoxel(Vector3 pos)
    {
        if (voxels.CheckForVoxel(pos.ToVector3Int()))
        {
            return true;
        }
        return false;
    }

    public string savePath; 

    [Button(nameof(ExportMeshToFile))]
    public bool button_ExportToFile;
    public void ExportMeshToFile()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Save Mesh", "", "", "serializedmesh");
        ExportToFile(path);
    }

    public MeshFilter displayObject;

    [Button(nameof(LoadMeshFromFile))]
    public bool button_LoadMeshFromFile;
    public void LoadMeshFromFile()
    {
        var path = StandaloneFileBrowser.OpenFilePanel("Load Mesh", "", "serializedmesh", false)[0];

        Mesh mesh = B83.MeshTools.MeshSerializer.DeserializeMesh(File.ReadAllBytes(path));
        if(mesh == null)
        {
            Debug.Log("Invalid Mesh File");
        }
        else
        {
            displayObject.sharedMesh = mesh;
        }
    }

    [Button(nameof(LoadVoxelObjectFromFile))]
    public bool button_LoadVoxelObjectFromFile;
    public void LoadVoxelObjectFromFile()
    {
        var path = StandaloneFileBrowser.OpenFilePanel("Load Voxel", "", "voxel", false)[0];

        voxels.LoadFromFile(path);
    }

    [Button(nameof(SaveVoxelObjectToFile))]
    public bool button_SaveVoxelObjectToFile;
    public void SaveVoxelObjectToFile()
    {
        if(savePath == "" || savePath.Substring(savePath.Length - 6) != "voxel")
        {
            savePath = StandaloneFileBrowser.SaveFilePanel("Save Voxel", "", "", "voxel");
        }
        voxels.SaveToFile(savePath);
    }
}
