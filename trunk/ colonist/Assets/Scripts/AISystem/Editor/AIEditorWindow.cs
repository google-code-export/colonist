using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// AI editor window.
/// Menu : Component -> AI -> Editor.
/// Support edit multiple AI component.
/// </summary>
public class AIEditorWindow : EditorWindow
{
	float MainWindowWidth, MainWindowHeight;
	Vector2 ScrollPosition = Vector2.zero;
	
	IList<AIEditor> AIEditor_List = null;
	IDictionary<string, bool> EnableEditAIFlag = new Dictionary<string,bool>();
	Unit UnitToEdited = null;
	
	[MenuItem("Component/AI/Edit Unit and AI")]	
	public static void init ()
	{
		AIEditorWindow window = (AIEditorWindow)EditorWindow.GetWindow (typeof(AIEditorWindow));
	}
	
	void OnGUI ()
	{
		MainWindowWidth = position.width;
		MainWindowHeight = position.height;
		if(Selection.activeGameObject != null &&
			Selection.activeGameObject.GetComponent<Unit>() != null && GUILayout.Button("Use selected gameobject"))
		{
			UnitToEdited = Selection.activeGameObject.GetComponent<Unit>();
		}
		UnitToEdited = (Unit)EditorGUILayout.ObjectField(UnitToEdited, typeof (Unit));
		if (UnitToEdited == null) {
			EditorGUILayout.LabelField ("Select a unit to edit.");
			return;
		}
		
		GameObject selectedGameObject = UnitToEdited.gameObject;
		
		//Attach AI script button
		if (selectedGameObject.GetComponent<AI> () == null) {
			Rect newAIScriptButton = new Rect (0, 0, MainWindowWidth - 10, 30);
			if (GUI.Button (newAIScriptButton, "Attach AI script")) {
				selectedGameObject.AddComponent<AI> ();
			}
			return;
		}
        
		IList<AI> _AIs = selectedGameObject.GetComponents<AI> ().ToList();
		if(AIEditor_List == null || AIEditor_List.Count != _AIs.Count || AIEditor_List.Count(x=>x==null) > 0
			|| AIEditor_List.Count(x=>(_AIs.Contains(x.AI)==false)) > 0  )
		{
		   AIEditor_List = new List<AIEditor>();
		   foreach(AI ai in _AIs)
		   {
			  AIEditor_List.Add(new AIEditor(ai));
		   }
		}

		if (GUILayout.Button ("Save object")) {
			foreach(AIEditor aiEditor in AIEditor_List){
				EditorUtility.SetDirty (aiEditor.AI.Unit);
			    EditorUtility.SetDirty (aiEditor.AI);
			}
		}
		
		ScrollPosition = EditorGUILayout.BeginScrollView (ScrollPosition, false, true, null);
		
		AIEditor_List[0].EditUnit();
		
		foreach(AIEditor aiEditor in AIEditor_List)
		{
		   if(EnableEditAIFlag.Keys.Contains(aiEditor.AI.Name) == false)
		   {
			  EnableEditAIFlag[aiEditor.AI.Name] = false;
		   }
			
		   EnableEditAIFlag[aiEditor.AI.Name] = EditorGUILayout.BeginToggleGroup ("Edit AI : " + aiEditor.AI.Name, EnableEditAIFlag[aiEditor.AI.Name]);
           if (EnableEditAIFlag[aiEditor.AI.Name] ) {
		      aiEditor.EditAI();
		   }
		}
		EditorGUILayout.EndScrollView ();
	}

    void OnDestroy ()
	{
		if(AIEditor_List != null)
		{
		  foreach(AIEditor aiEditor in AIEditor_List)
		  {
		    aiEditor.Dispose();
		  }
		}
	}
	
}

