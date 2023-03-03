using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.UIElements;

[ScriptedImporter(1, "serializedmesh")]
public class MeshImporter : ScriptedImporter
{

    public override void OnImportAsset(AssetImportContext ctx)
    {
        Mesh mesh = B83.MeshTools.MeshSerializer.DeserializeMesh(File.ReadAllBytes(ctx.assetPath));
        if (mesh == null)
        {
            Debug.Log("Invalid Mesh File");
        }
        else
        {
            ctx.AddObjectToAsset("main obj", mesh);
            ctx.SetMainObject(mesh);
        }
    }
}
