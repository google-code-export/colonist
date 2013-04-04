using UnityEngine;
using UnityEditor;
using System.Collections;

public class CopyTransformWindow : EditorWindow {

	[MenuItem("Window/CopyTransform")]	
	public static void init ()
	{
		CopyTransformWindow window = (CopyTransformWindow)EditorWindow.GetWindow (typeof(CopyTransformWindow));
	}
	GameObject copyFromObject = null;
	GameObject copyToObject = null;
	
	void OnGUI ()
	{
		copyFromObject = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Copy transform from game object:", ""),
			copyFromObject, typeof(GameObject) );
		copyToObject = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Copy transform to game object:", ""),
			copyToObject, typeof(GameObject) );
		if(GUILayout.Button(new GUIContent("copy transform","")))
		{
			Util.CopyTransform(copyFromObject.transform, copyToObject.transform);
		}
	}
}
