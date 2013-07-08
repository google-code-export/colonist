using UnityEditor;
using UnityEngine;
using System.Collections;

public class OutputTransfromPath {

    [MenuItem("GameObject/PrintTransformPath #o")]
    public static void ShowTransformPath()
    {
        GameObject selection = UnityEditor.Selection.activeGameObject;
        string path = GetPath(selection.transform);
        Debug.Log(path);
    }

    public static string GetPath(Transform current)
    {
        if (current.parent == null)
            return "/" + current.name;
        return GetPath(current.parent) + "/" + current.name;
    }
}
