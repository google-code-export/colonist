using UnityEngine;
using UnityEditor;

using System.Collections;
using System.IO;
using System.Text;


public class PlaceGrassOnGround : EditorWindow{

    [MenuItem("GameObject/Terrain/PlaceGrassOnGround...")]
    static void PlantGrass()
    {
        GameObject selectedObject = Selection.activeGameObject;
        foreach (Transform t in selectedObject.transform)
        {
            if (t.gameObject.layer != LayerMask.NameToLayer("grass"))
            {
                continue;
            }
            else
            {
                _plantgrass(t);
            }
        }
    }
    static void _plantgrass(Transform grassTransform)
    {
        RaycastHit hitInfo = new RaycastHit();
        int layermask = 1 << LayerMask.NameToLayer("Terrain");
        if (Physics.Raycast(grassTransform.position, Vector3.down, out hitInfo, 1000, layermask))
        {
            grassTransform.position = hitInfo.point;
            grassTransform.RotateAround(Vector3.up, Random.Range(0f, 360f));
        }

    }
}
