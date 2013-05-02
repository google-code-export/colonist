using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[CustomEditor(typeof(AI))]
public class AIInspector : Editor
{
	bool EnableBaseInspector = false , EnableAIInspector = false;
	AIEditor aiEditor = null;
	public AIInspector()
	{
		
	}
	public override void OnInspectorGUI ()
	{
		if(aiEditor == null)
		{
			aiEditor = new AIEditor(target as AI);
		}
		EditorGUILayout.LabelField("Editing AI: " + (target as AI).Name);
		EnableBaseInspector = EditorGUILayout.Toggle(new GUIContent("Base Inspector", ""), EnableBaseInspector);
		if(EnableBaseInspector)
		{
		   base.OnInspectorGUI();
		}
		
		EnableAIInspector = EditorGUILayout.Toggle(new GUIContent("Advanced AI Inspector", ""), EnableAIInspector);
		if(EnableAIInspector)
		{
		   if (GUILayout.Button ("Save object")) {
				EditorUtility.SetDirty (target as AI);
		   }
           aiEditor.EditAI();
		}
	}
}
