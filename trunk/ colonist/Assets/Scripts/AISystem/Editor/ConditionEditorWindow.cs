using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class ConditionEditorWindow : EditorWindow
{
	public CompositeConditionWrapper compositionConditionWrapper = null;
	public AIEditor aiEditor = null;
	public string WindowLabel = "";
	public bool EnableEditAtomCondition = true, EnableEditCompositeCondition = true, EnableEditRootCompositeCondition = true;
	
	private Vector2 ScrollPos = Vector2.zero;
	
//	[MenuItem("Component/AI/EditCondition")]	
//	public static void init ()
//	{
//		ConditionEditorWindow window = (ConditionEditorWindow)EditorWindow.GetWindow (typeof(ConditionEditorWindow));
//		window.title = "Root composite condition";
//	}
	
	public static void DisplayConditionEditorWindow (AIEditor aiEditor,
		CompositeConditionWrapper compositionConditionWrapper)
	{
		ConditionEditorWindow window = (ConditionEditorWindow)EditorWindow.GetWindow (typeof(ConditionEditorWindow));
		window.compositionConditionWrapper = compositionConditionWrapper;
		window.aiEditor = aiEditor;
		window.Show ();
	}
	
	public void OnGUI ()
	{
		if (compositionConditionWrapper == null || aiEditor == null) {
			EditorGUILayout.LabelField ("No condition wrapper or AIEditor!!");
			return;
		}
		
		if(GUILayout.Button("Save"))
		{
			EditorUtility.SetDirty (aiEditor.AI);
		}
		
		ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos,false, true, null);
		
		//Edit Atom condition
		EnableEditAtomCondition = EditorGUILayout.BeginToggleGroup ("Edit atom condition", EnableEditAtomCondition);
		if (EnableEditAtomCondition) {
			if (GUILayout.Button ("Add atom condition")) {
				AtomConditionData atomCondition = new AtomConditionData ();
				compositionConditionWrapper.atomConditionDataArray = Util.AddToArray<AtomConditionData> (atomCondition, compositionConditionWrapper.atomConditionDataArray);
			}
			foreach (AtomConditionData atomCondition in compositionConditionWrapper.atomConditionDataArray) {
				this.aiEditor.EditAtomConditionData (atomCondition);
				if (GUILayout.Button ("Delete atom condition:" + atomCondition.Id)) {
					compositionConditionWrapper.atomConditionDataArray =
				      Util.CloneExcept<AtomConditionData> (compositionConditionWrapper.atomConditionDataArray, atomCondition);
				}
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
			}
		}
		EditorGUILayout.EndToggleGroup ();
		
		//Edit composite condition
		EnableEditCompositeCondition = EditorGUILayout.BeginToggleGroup ("Edit composite condition", EnableEditCompositeCondition);
		if(EnableEditCompositeCondition)
		{
			if (GUILayout.Button ("Add composite condition")) {
				CompositeCondition compositeCondition = new CompositeCondition ();
				compositionConditionWrapper.CompositeConditionArray = Util.AddToArray<CompositeCondition> (compositeCondition, compositionConditionWrapper.CompositeConditionArray);
			}
			foreach (CompositeCondition compositeCondition in compositionConditionWrapper.CompositeConditionArray) {
				this.aiEditor.EditCompositeCondition (compositeCondition, compositionConditionWrapper);
				if (GUILayout.Button ("Delete composite condition:" + compositeCondition.Id)) {
					compositionConditionWrapper.CompositeConditionArray =
				      Util.CloneExcept<CompositeCondition> (compositionConditionWrapper.CompositeConditionArray, 
							                                compositeCondition);
				}
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
			}
		}
		EditorGUILayout.EndToggleGroup();
		
		//Edit root composite condition
		EnableEditRootCompositeCondition = EditorGUILayout.BeginToggleGroup("Edit root composite condition", EnableEditRootCompositeCondition);
		if(EnableEditRootCompositeCondition)
		{
			this.aiEditor.EditCompositeCondition (compositionConditionWrapper.RootCompositeCondition, compositionConditionWrapper);
		}
		
		EditorGUILayout.EndScrollView();
	}
	
	void OnDestroy ()
	{
		
	}
}
