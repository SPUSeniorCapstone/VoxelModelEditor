using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VoxelEditor : MonoBehaviour
{
    public VoxelObject voxelObject;

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
            voxelObject.ReloadMesh();
            changed = false;
        }

        if(mirrorX) xPlane.SetActive(true);
        else xPlane.SetActive(false);

        if (mirrorZ) zPlane.SetActive(true);
        else zPlane.SetActive(false);

        if (mirrorY) yPlane.SetActive(true);
        else yPlane.SetActive(false);
    }


    void PlaceBlock(Vector3Int pos, bool x = false, bool y = false, bool z = false)
    {
        if (!placeBlock.gameObject.activeSelf)
        {
            return;
        }

        voxelObject.AddVoxel(pos, new Voxel(color.color));

        if (mirrorX && !x)
        {
            Vector3Int p = new Vector3Int(voxelObject.width - pos.x - 1, pos.y, pos.z);
            PlaceBlock(p, true, y, z);
        }
        if (mirrorY && !y)
        {
            Vector3Int p = new Vector3Int(pos.x, voxelObject.height - pos.y - 1, pos.z);
            PlaceBlock(p, x, true, z);
        }
        if (mirrorZ && !z)
        {
            Vector3Int p = new Vector3Int(pos.x, pos.y, voxelObject.length - pos.z - 1);
            PlaceBlock(p, x, y, true);
        }

    }

    void BreakBlock(Vector3Int pos, bool x = false, bool y = false, bool z = false)
    {
        if (!highlight.gameObject.activeSelf)
        {
            return;
        }

        voxelObject.RemoveVoxel(pos);

        if (mirrorX && !x)
        {
            Vector3Int p = new Vector3Int(voxelObject.width - pos.x - 1, pos.y, pos.z);
            BreakBlock(p, true, y, z);
        }
        if (mirrorY && !y)
        {
            Vector3Int p = new Vector3Int(pos.x, voxelObject.height - pos.y - 1, pos.z);
            BreakBlock(p, x, true, z);
        }
        if (mirrorZ && !z)
        {
            Vector3Int p = new Vector3Int(pos.x, pos.y, voxelObject.length - pos.z - 1);
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
        if (voxelObject.CheckForVoxel(pos.ToVector3Int()))
        {
            return true;
        }
        return false;
    }
}
