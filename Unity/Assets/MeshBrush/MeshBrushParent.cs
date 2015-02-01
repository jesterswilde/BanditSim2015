using UnityEngine;
using System.Collections;

public class MeshBrushParent : MonoBehaviour{

	public void FlagMeshesAsStatic ()
	{
        Component[] meshes = GetComponentsInChildren(typeof(Transform));
		foreach(Transform _t in meshes){
			_t.gameObject.isStatic = true;
		}
	}
	public void UnflagMeshesAsStatic ()
	{
        Component[] meshes = GetComponentsInChildren(typeof(Transform));
		foreach(Transform _t in meshes){
			_t.gameObject.isStatic = false;
		}
	}
    public void DeleteAllMeshes()
    {
        DestroyImmediate(gameObject);
    }

    public void CombinePaintedMeshes()
    {
        Component[] meshFilters = GetComponentsInChildren(typeof(MeshFilter));
        Matrix4x4 myTransform = transform.worldToLocalMatrix;
        Hashtable materialToMesh = new Hashtable();

        for (int i = 0; i < meshFilters.Length; i++)
        {
            MeshFilter filter = (MeshFilter)meshFilters[i];
            Renderer curRenderer = meshFilters[i].renderer;
            MB_MeshCombineUtility.MeshInstance instance = new MB_MeshCombineUtility.MeshInstance();
            instance.mesh = filter.sharedMesh;
            if (curRenderer != null && curRenderer.enabled && instance.mesh != null)
            {
                instance.transform = myTransform * filter.transform.localToWorldMatrix;

                Material[] materials = curRenderer.sharedMaterials;
                for (int m = 0; m < materials.Length; m++)
                {
                    instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);

                    ArrayList objects = (ArrayList)materialToMesh[materials[m]];
                    if (objects != null)
                    {
                        objects.Add(instance);
                    }
                    else
                    {
                        objects = new ArrayList();
                        objects.Add(instance);
                        materialToMesh.Add(materials[m], objects);
                    }
                }

                DestroyImmediate( curRenderer.gameObject );
            }
        }

        foreach (DictionaryEntry de in materialToMesh)
        {
            ArrayList elements = (ArrayList)de.Value;
            MB_MeshCombineUtility.MeshInstance[] instances = (MB_MeshCombineUtility.MeshInstance[])elements.ToArray(typeof(MB_MeshCombineUtility.MeshInstance));

            // We have a maximum of one material, so just attach the mesh to our own game object
            if (materialToMesh.Count == 1)
            {
                // Make sure we have a mesh filter & renderer
                if (GetComponent(typeof(MeshFilter)) == null)
                    gameObject.AddComponent(typeof(MeshFilter));
                if (!GetComponent("MeshRenderer"))
                    gameObject.AddComponent("MeshRenderer");

                MeshFilter filter = (MeshFilter)GetComponent(typeof(MeshFilter));
                filter.mesh = MB_MeshCombineUtility.Combine(instances, false);
                renderer.material = (Material)de.Key;
                renderer.enabled = true;
            }
            // We have multiple materials to take care of, build one mesh / gameobject for each material
            // and parent it to this object
            else
            {
                GameObject go = new GameObject("Combined mesh");
                go.transform.parent = transform;
                go.transform.localScale = Vector3.one;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localPosition = Vector3.zero;
                go.AddComponent(typeof(MeshFilter));
                go.AddComponent("MeshRenderer");
                go.renderer.material = (Material)de.Key;
				go.isStatic = true;
                MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
                filter.mesh = MB_MeshCombineUtility.Combine(instances, false);
            }
        }
		gameObject.isStatic = true;
    }
}

/*
 * 
 * Raphael Beck, 2014
 * 
*/
