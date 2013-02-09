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
	[MenuItem("Component/AI/Edit AI")]	
	public static void init ()
	{
		AIEditorWindow window = (AIEditorWindow)EditorWindow.GetWindow (typeof(AIEditorWindow));
	}
	
	void OnGUI ()
	{
		MainWindowWidth = position.width;
		MainWindowHeight = position.height;
		GameObject selectedGameObject = Selection.activeGameObject;
		if (selectedGameObject == null) {
			Debug.LogWarning ("No gameObject is selected.");
			return;
		}
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
		ScrollPosition = EditorGUILayout.BeginScrollView (ScrollPosition, false, true, null);
		AIEditor_List[0].EditUnit();
		foreach(AIEditor aiEditor in AIEditor_List)
		{
		   aiEditor.EditAI();
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

