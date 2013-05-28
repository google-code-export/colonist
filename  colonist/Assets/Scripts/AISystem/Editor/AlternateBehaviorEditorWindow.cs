using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class AlternateBehaviorEditorWindow : EditorWindow
{
	AIBehavior aiBehavior = null;
	AIEditor aiEditor = null;
	bool EnableEditAtomCondition = true, EnableEditCompositeCondition = true, EnableEditRootCompositeCondition = true;
	IDictionary<string,bool> editableFlagDict = new Dictionary<string,bool> ();
	private Vector2 ScrollPos = Vector2.zero;
	
//	[MenuItem("Component/AI/EditCondition")]	
//	public static void init ()
//	{
//		ConditionEditorWindow window = (ConditionEditorWindow)EditorWindow.GetWindow (typeof(ConditionEditorWindow));
//		window.title = "Root composite condition";
//	}
	
	public static void DisplayConditionEditorWindow (AIEditor aiEditor,
		AIBehavior aiBehavior)
	{
		AlternateBehaviorEditorWindow window = (AlternateBehaviorEditorWindow)EditorWindow.GetWindow (typeof(AlternateBehaviorEditorWindow));
		window.aiBehavior = aiBehavior;
		window.aiEditor = aiEditor;
		window.Show ();
		
	}
	
	public void OnGUI ()
	{
		if (aiBehavior == null || aiEditor == null) {
			EditorGUILayout.LabelField ("No condition wrapper or AIEditor!!");
			return;
		}
		
		EditorGUILayout.LabelField ("Edit AlternateBehaviorData of Behavior:" + aiBehavior.Name);
		
		if (GUILayout.Button ("Save")) {
			EditorUtility.SetDirty (aiEditor.AI);
		}
		
		ScrollPos = EditorGUILayout.BeginScrollView (ScrollPos, false, true, null);
		
		if (GUILayout.Button ("Add AlternateBehaviorData")) {
			AlternateBehaviorData alternateBehaviorData = new AlternateBehaviorData ();
			aiBehavior.alternateBehaviorConditionArray = Util.AddToArray<AlternateBehaviorData> (alternateBehaviorData, aiBehavior.alternateBehaviorConditionArray);
		}
		
		for (int i=0; i<aiBehavior.alternateBehaviorConditionArray.Length; i++) {
			AlternateBehaviorData alternateBehaviorData = aiBehavior.alternateBehaviorConditionArray [i];
			EditorGUILayout.BeginHorizontal();
			alternateBehaviorData.Name = EditorGUILayout.TextField("AlternateBehaviorData name:", alternateBehaviorData.Name);
			if (editableFlagDict.Keys.Contains (alternateBehaviorData.Name) == false) {
				editableFlagDict.Add (alternateBehaviorData.Name, false);
			}
			editableFlagDict [alternateBehaviorData.Name] = EditorGUILayout.Toggle("Enable:" + alternateBehaviorData.Name, editableFlagDict [alternateBehaviorData.Name]);
			EditorGUILayout.EndHorizontal();
			if (editableFlagDict [alternateBehaviorData.Name]) {
				EditAlternateBehaviorData (alternateBehaviorData);
				if (GUILayout.Button ("Remove AlternateBehaviorData:" + alternateBehaviorData.Name)) {
					aiBehavior.alternateBehaviorConditionArray = Util.CloneExcept<AlternateBehaviorData> (aiBehavior.alternateBehaviorConditionArray, alternateBehaviorData);
				}
			}
		}
		
		EditorGUILayout.EndScrollView ();
	}
	
	void EditAlternateBehaviorData (AlternateBehaviorData alternateBehaviorData)
	{
		EditorGUILayout.LabelField ("--------------------------- Edit AlternateBehaviorData: " + alternateBehaviorData.Name);
		alternateBehaviorData.priority = EditorGUILayout.IntField (new GUIContent ("Priority (0=least priority):", ""), alternateBehaviorData.priority);
		string[] allBehaviorName = aiEditor.AI.Behaviors.Select (x => x.Name).ToArray ();
		alternateBehaviorData.NextBehaviorName = EditorCommon.EditPopup ("Next beahvior:", 
			                                                            alternateBehaviorData.NextBehaviorName,
			                                                            allBehaviorName);
		EditConditionWrapper (alternateBehaviorData.AlternateCondition);
	}
	
	void EditConditionWrapper (CompositeConditionWrapper compositionConditionWrapper)
	{
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
		if (EnableEditCompositeCondition) {
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
		EditorGUILayout.EndToggleGroup ();
		
		//Edit root composite condition
		EnableEditRootCompositeCondition = EditorGUILayout.BeginToggleGroup ("Edit root composite condition", EnableEditRootCompositeCondition);
		if (EnableEditRootCompositeCondition) {
			this.aiEditor.EditCompositeCondition (compositionConditionWrapper.RootCompositeCondition, compositionConditionWrapper);
		}
		EditorGUILayout.EndToggleGroup ();
	}
}
