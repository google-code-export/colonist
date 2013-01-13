using UnityEngine;
using UnityEditor;
using System.Collections;

public class BulkChangeAssetImport : EditorWindow {

    static ArrayList AssetList = new ArrayList();
    static string ScaleFactor = "0.005";
    [MenuItem("Assets/Change Mesh Import Scale Factor")]
    public static void Init()
    {
        AssetList.Clear();
        Object[] selections = Selection.objects;
        foreach (Object obj in selections)
        {
            string szPath = AssetDatabase.GetAssetPath(obj);
            ModelImporter imported = AssetImporter.GetAtPath(szPath) as ModelImporter;
            if (imported == null)
            {
                Debug.Log("Selected object is not an Model, Object name:" + obj.name + " Type:" + obj.GetType().Name);
                return;
            }
            AssetList.Add(imported);
        }
        EditorWindow.GetWindow<BulkChangeAssetImport>().Show();

    }

    void OnGUI()
    {
        ScaleFactor = GUILayout.TextField(ScaleFactor.ToString());
        if (GUILayout.Button("Change scale factor"))
        {
            ChangeScaleFactorAndReimport();
        }
    }

    void ChangeScaleFactorAndReimport()
    {
        foreach (object obj in AssetList)
        {
            ModelImporter model = (ModelImporter)obj;
            //string szPath = AssetDatabase.GetAssetPath(obj);
            //ModelImporter imported = AssetImporter.GetAtPath(szPath) as ModelImporter;
            model.globalScale = float .Parse(ScaleFactor);
            AssetDatabase.ImportAsset(model.assetPath);
        }
    }
}
