using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorUtilityWindow : EditorWindow
{
	[MenuItem("Window/Utility")]	
	public static void init ()
	{
		EditorUtilityWindow window = (EditorUtilityWindow)EditorWindow.GetWindow (typeof(EditorUtilityWindow)); 
	}
	
	bool measureTwoObjects = false;
	GameObject MeasureObjectA = null;
	GameObject MeasureObjectB = null;
	bool copyUnitAndAIComponents = false;
	Unit CopyUnitObjectA = null;
	GameObject CopyUnitObjectB = null;
	bool copyAnimation = false;
	AnimationClip CopyFromClipA = null;
	AnimationClip CopyFromClipB = null;
	
	
	void OnGUI ()
	{
#region measure distance between two objects
		measureTwoObjects = EditorGUILayout.BeginToggleGroup ("Measure two characters", measureTwoObjects);
		if (measureTwoObjects) {
			MeasureObjectA = (GameObject)EditorGUILayout.ObjectField (MeasureObjectA, typeof(GameObject));
			MeasureObjectB = (GameObject)EditorGUILayout.ObjectField (MeasureObjectB, typeof(GameObject));
			if (GUILayout.Button ("Measure the distance of two game object")) {
				float distanceOfCharacter = Util.DistanceOfCharacters (MeasureObjectA, MeasureObjectB);
//			float distanceOfCharacter_XZ = Util.DistanceOfCharactersXZ(MeasureObjectA, MeasureObjectB);
				float distanceOfTransform = Vector3.Distance (MeasureObjectA.transform.position, MeasureObjectB.transform.position);
				Debug.Log (string.Format ("Distance of character:{0}; Distance of transform:{1};", distanceOfCharacter, distanceOfTransform));
			}
		}
		EditorGUILayout.EndToggleGroup ();
#endregion

#region copy components from object to another object
		copyUnitAndAIComponents = EditorGUILayout.BeginToggleGroup ("Copy unit and AI components", copyUnitAndAIComponents);
		if (copyUnitAndAIComponents) {
			CopyUnitObjectA = (Unit)EditorGUILayout.ObjectField (CopyUnitObjectA, typeof(Unit));
			CopyUnitObjectB = (GameObject)EditorGUILayout.ObjectField (CopyUnitObjectB, typeof(GameObject));
			if (GUILayout.Button ("Copy component from object A to Object B")) {
				 EditorCommon.CopyUnitAndAIComponent(CopyUnitObjectA, CopyUnitObjectB);
			}
		}
		EditorGUILayout.EndToggleGroup ();
#endregion
		
#region copy animation events from one aniamtion asset to another animation asset
		copyAnimation = EditorGUILayout.BeginToggleGroup ("Copy animation", copyAnimation);
		if (copyAnimation) {
			CopyFromClipA = (AnimationClip)EditorGUILayout.ObjectField (CopyFromClipA, typeof(AnimationClip));
			CopyFromClipB = (AnimationClip)EditorGUILayout.ObjectField (CopyFromClipB, typeof(AnimationClip));
			if (GUILayout.Button ("Copy animation event from A to B")) {
				 EditorCommon.CopyAnimationEvents(CopyFromClipA, CopyFromClipB);
			}
		}
		EditorGUILayout.EndToggleGroup ();
#endregion
	}
}
