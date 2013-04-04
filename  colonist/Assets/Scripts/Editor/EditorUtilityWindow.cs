using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorUtilityWindow : EditorWindow {
	[MenuItem("Window/Utility")]	
	public static void init ()
	{
		EditorUtilityWindow window = (EditorUtilityWindow)EditorWindow.GetWindow (typeof(EditorUtilityWindow)); 
	}
	
	bool measureTwoObjects = false;
	GameObject MeasureObjectA = null;
	GameObject MeasureObjectB = null;
	void OnGUI ()
	{
	    measureTwoObjects = EditorGUILayout.BeginToggleGroup("Measure two characters",measureTwoObjects);
		MeasureObjectA = (GameObject)EditorGUILayout.ObjectField(MeasureObjectA, typeof(GameObject));
		MeasureObjectB = (GameObject)EditorGUILayout.ObjectField(MeasureObjectB, typeof(GameObject));
		if(GUILayout.Button("Measure the distance of two game object"))
		{
			float distanceOfCharacter = Util.DistanceOfCharacters(MeasureObjectA, MeasureObjectB);
//			float distanceOfCharacter_XZ = Util.DistanceOfCharactersXZ(MeasureObjectA, MeasureObjectB);
			float distanceOfTransform = Vector3.Distance(MeasureObjectA.transform.position, MeasureObjectB.transform.position);
			Debug.Log(string.Format("Distance of character:{0}; Distance of transform:{1};", distanceOfCharacter, distanceOfTransform));
		}
		EditorGUILayout.EndToggleGroup();
	}
}
