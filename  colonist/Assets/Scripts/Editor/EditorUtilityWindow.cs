using UnityEngine;
using System.Collections;
using UnityEditor;

public enum AnimationParameterType
{
	IntParam = 0,
	StringParam = 1,
	FloatParam = 2,
}

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
	bool MeasureTwoObjectRelativePosition = false;
    Transform MeasureTransformA = null;
	Transform MeasureTransformB = null;
	Transform MeasureTransformC = null;
	
	bool copyAnimation = false;
	AnimationClip CopyFromClipA = null;
	AnimationClip CopyFromClipB = null;
	bool AddAnimationEvent = false;
	string AnimationNameToEdit = "";
	string AnimationFunctionName = "";
	string AnimationParam = "";
	AnimationParameterType animationParameterType = AnimationParameterType.FloatParam;
	bool EnableCopyRagdollJointData = false;
	Ragdoll copyRagdollFrom = null;
	Ragdoll copyRagdollTo = null;
	bool EnableCreateRagdollFromTemplate = false;
	GameObject ragdollTemplate = null;
	GameObject newRagdollObject = null;
	bool EnableEditLevelCheckpoint = false;
	string checkPointLevel;
	string checkPointName;
	bool EnableCopyPoseByAnimationFrame = false;
	float copyPoseOfTime = 0;
	AnimationClip PoseSourceClip = null;
	GameObject PoseToCharacter = null;
	
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
		
		#region print position and rotation
		
		if (GUILayout.Button ("Print selection active position + rotation, and audio source status.")) {
			string msg = string.Format ("Position:{0}, Rotation:{1}, AudioSource:{2}", 
				                       Selection.activeGameObject.transform.position, 
				                       Selection.activeGameObject.transform.rotation,
				                       Selection.activeGameObject.GetComponent<AudioSource> () != null ? Selection.activeGameObject.GetComponent<AudioSource> ().isPlaying.ToString () : "no audio"
				                      );
			Debug.Log (msg);
		}
		#endregion

#region measure two objects relative position
		MeasureTwoObjectRelativePosition = EditorGUILayout.BeginToggleGroup ("Measure two object relative position", MeasureTwoObjectRelativePosition);
		if (MeasureTwoObjectRelativePosition) {
			MeasureTransformA = (Transform)EditorGUILayout.ObjectField ("Measured A:",MeasureTransformA, typeof(Transform));
			MeasureTransformB = (Transform)EditorGUILayout.ObjectField ("Measured B:",MeasureTransformB, typeof(Transform));
			MeasureTransformC = (Transform)EditorGUILayout.ObjectField (new GUIContent("Measured C:", "C can be null, then world space"),MeasureTransformC, typeof(Transform));
			if (GUILayout.Button ("Measure object A relative position to Object B,in relative space of C")) {
				Vector3 posA = MeasureTransformA.transform.position - (MeasureTransformC != null ? MeasureTransformC.position : Vector3.zero);
				Vector3 posB = MeasureTransformB.transform.position - (MeasureTransformC != null ? MeasureTransformC.position : Vector3.zero);
				Debug.Log("PositionB - PositionA = " + (posB - posA).ToString());
			}
		}
		EditorGUILayout.EndToggleGroup ();
#endregion
		
#region copy animation events from one aniamtion asset to another animation asset
		copyAnimation = EditorGUILayout.BeginToggleGroup ("Copy animation event", copyAnimation);
		if (copyAnimation) {
			CopyFromClipA = (AnimationClip)EditorGUILayout.ObjectField (CopyFromClipA, typeof(AnimationClip));
			CopyFromClipB = (AnimationClip)EditorGUILayout.ObjectField (CopyFromClipB, typeof(AnimationClip));
			if (GUILayout.Button ("Copy animation event from A to B")) {
				EditorCommon.CopyAnimationEvents (CopyFromClipA, CopyFromClipB);
			}
		}
		EditorGUILayout.EndToggleGroup ();
#endregion
		
#region add animation event
		AddAnimationEvent = EditorGUILayout.BeginToggleGroup ("Add animation event", AddAnimationEvent);
		if (AddAnimationEvent) {
			if (Selection.activeGameObject != null) {
				int index = 0;
				string[] array = EditorCommon.GetAnimationNames (Selection.activeGameObject, AnimationNameToEdit, out index);
				if (index == -1) {
					index = 0;
				}
				index = EditorGUILayout.Popup ("Animation:", index, array);
			   
				AnimationNameToEdit = array [index];
				AnimationFunctionName = EditorGUILayout.TextField ("Function name:", AnimationFunctionName);
				EditorGUILayout.BeginHorizontal ();
				animationParameterType = (AnimationParameterType)EditorGUILayout.EnumPopup ("Evnet param:", animationParameterType);
				AnimationParam = EditorGUILayout.TextField ("Animation param:", AnimationParam);
				EditorGUILayout.EndHorizontal ();
				if (GUILayout.Button ("Add animation")) {
					AnimationClip clip = Selection.activeGameObject.animation.GetClip (AnimationNameToEdit);
					EditorCommon.AddAnimationEvent (clip, AnimationFunctionName, AnimationParam, animationParameterType);
				}
			}
		}
		EditorGUILayout.EndToggleGroup ();
#endregion
		
#region print playing animation
		string logStr_animation = "Playing animation:";
		if (GUILayout.Button ("Print selection active animation status")) {
			foreach (AnimationState ani in Selection.activeGameObject.animation) {
				bool isplaying = Selection.activeGameObject.animation.IsPlaying (ani.name);
				if (isplaying)
					logStr_animation += ani.name + "; ";
			}
			Debug.Log (logStr_animation);
		}
#endregion		
		
#region Copy ragdollJoint from one unit to another
		if (EnableCopyRagdollJointData = EditorGUILayout.BeginToggleGroup ("Copy ragdoll joint data", EnableCopyRagdollJointData)) {
			copyRagdollFrom = (Ragdoll)EditorGUILayout.ObjectField ("Copy ragdoll from:", copyRagdollFrom, typeof(Ragdoll));
			copyRagdollTo = (Ragdoll)EditorGUILayout.ObjectField ("Copy ragdoll to:", copyRagdollTo, typeof(Ragdoll));
			if (GUILayout.Button ("Copy ragdoll from-to")) {
				RagdollEditor.CopyRagdollData (copyRagdollFrom, copyRagdollTo);
			}
		}
		EditorGUILayout.EndToggleGroup ();
#endregion
		
		#region print child transform path
		if (GUILayout.Button ("Print child path")) {
			string path = Util.GetChildPath (Selection.activeGameObject.transform.root, Selection.activeGameObject.transform);
			Transform s = Selection.activeGameObject.transform.root.Find (path);
			Debug.Log (s);
			Debug.Log (path);
		}
		#endregion
		
#region copy ragdoll
		if (EnableCreateRagdollFromTemplate = EditorGUILayout.BeginToggleGroup ("Create ragdoll from template", EnableCreateRagdollFromTemplate)) {
			EditorGUILayout.BeginHorizontal ();
			ragdollTemplate = (GameObject)EditorGUILayout.ObjectField ("Ragdoll template:", ragdollTemplate, typeof(GameObject));
			newRagdollObject = (GameObject)EditorGUILayout.ObjectField ("New ragdoll:", newRagdollObject, typeof(GameObject));
			if (GUILayout.Button ("Create ragdoll from template")) {
				EditorCommon.CreateRagdollFromTemplate (ragdollTemplate, newRagdollObject);
			}
			EditorGUILayout.EndHorizontal ();
		}
		EditorGUILayout.EndToggleGroup ();
#endregion
		
#region edit level checkpoint
		if (EnableEditLevelCheckpoint = EditorGUILayout.BeginToggleGroup ("Edit level checkpoint", EnableEditLevelCheckpoint)) {
			bool hasCheckPoint = Persistence.GetLastCheckPoint (out checkPointLevel, out checkPointName);
			if (hasCheckPoint) {
				checkPointLevel = EditorGUILayout.TextField ("check point level", checkPointLevel);
				checkPointName = EditorGUILayout.TextField ("check point name", checkPointName);
				if (GUILayout.Button ("Delete checkpoint")) {
					Persistence.ClearCheckPoint ();
				}
			} else {
				checkPointLevel = EditorGUILayout.TextField ("check point level", checkPointLevel);
				checkPointName = EditorGUILayout.TextField ("check point name", checkPointName);
				if (GUILayout.Button ("Save checkpoint")) {
					Persistence.SaveCheckPoint (checkPointLevel, checkPointName); 
				}
			}
		}
		EditorGUILayout.EndToggleGroup ();
#endregion
		
		
#region copy character pose by animation clip frame
		EnableCopyPoseByAnimationFrame = EditorGUILayout.BeginToggleGroup ("Copy pose by animation clip", EnableCopyPoseByAnimationFrame);
		if (EnableCopyPoseByAnimationFrame) {
			PoseSourceClip = (AnimationClip)EditorGUILayout.ObjectField ("Animation clip source:", PoseSourceClip, typeof(AnimationClip));
			PoseToCharacter = (GameObject)EditorGUILayout.ObjectField ("Pose to object:", PoseToCharacter, typeof(GameObject));
			if (PoseSourceClip != null && PoseToCharacter != null) {
				copyPoseOfTime = EditorGUILayout.FloatField ("Copy pose at time of the curve:", copyPoseOfTime);
				if (GUILayout.Button ("Copy pose at time")) {
					AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves (PoseSourceClip, true);
					string logMsg = "";
					for (int i=curveDatas.Length-1; i>=0; i--) {
						AnimationClipCurveData curveData = curveDatas[i];
//						logMsg += string.Format("Path:" + curveData.path + " property:" + curveData.propertyName + " value:" + curveData.curve.Evaluate(copyPoseOfTime) + "\r\n");
						EditorCommon.CopyCurveDataIntoTransform(PoseToCharacter.transform, curveData, copyPoseOfTime);
					}
//					Debug.Log(logMsg);
				}
			}
		}
		
#endregion
	}
}
