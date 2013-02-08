using UnityEngine;
using UnityEditor;


/// <summary>
/// AI editor window.
/// Menu : Component -> AI -> Editor.
/// Can only edit one AI component.
/// </summary>
public class AIEditorWindow : EditorWindow
{
	float MainWindowWidth, MainWindowHeight;
	Vector2 ScrollPosition = Vector2.zero;
	
	AIEditor AIEditor = null;
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
        
		AI _AI = selectedGameObject.GetComponent<AI> ();
		if(AIEditor == null)
		{
		   AIEditor = new AIEditor(_AI);
		}
		ScrollPosition = EditorGUILayout.BeginScrollView (ScrollPosition, false, true, null);
		AIEditor.EditUnitAndAI();
		EditorGUILayout.EndScrollView ();
	}

    void OnDestroy ()
	{
		if(AIEditor!=null)
		{
			AIEditor.Dispose();
		}
	}
	
}

