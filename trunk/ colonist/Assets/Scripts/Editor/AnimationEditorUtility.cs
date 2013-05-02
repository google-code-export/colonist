using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class AnimationEditorUtility
{
	const string duplicatePostfix = "";
	

	
	static void CopyClip (AnimationClip src)
	{
		// Find path of copy
		string importedPath = AssetDatabase.GetAssetPath (src);
		string copyPath = importedPath.Substring (0, importedPath.LastIndexOf ("/"));
		copyPath += "/" + src.name + duplicatePostfix + ".anim";
		
//		AnimationClip src = AssetDatabase.LoadAssetAtPath (importedPath, typeof(AnimationClip)) as AnimationClip;
		AnimationClip newClip = new AnimationClip ();
		newClip.name = src.name + duplicatePostfix;
		AssetDatabase.CreateAsset (newClip, copyPath);
		AssetDatabase.Refresh ();
		AnimationClip copy = AssetDatabase.LoadAssetAtPath (copyPath, typeof(AnimationClip)) as AnimationClip;
		if (copy == null) {
			Debug.Log ("No copy found at " + copyPath);
			return;
		}
		// Copy curves from imported to copy
		AnimationClipCurveData[] curveDatas = AnimationUtility.GetAllCurves (src, true);
		for (int i = 0; i < curveDatas.Length; i++) {
			AnimationUtility.SetEditorCurve (
                copy,
                curveDatas [i].path,
                curveDatas [i].type,
                curveDatas [i].propertyName,
                curveDatas [i].curve
            );
		}
	}

	[MenuItem("Assets/Transfer Clip Curves to Copy")]
	static void CopyCurvesToDuplicate ()
	{
		// Get selected AnimationClip
		AnimationClip imported = Selection.activeObject as AnimationClip;
		if (imported == null) {
			Debug.Log ("Selected object is not an AnimationClip");
			return;
		}

		CopyClip (imported);

		Debug.Log ("Copying curves of " + imported.name + " is done");
	}
	
	
	[MenuItem("Assets/Bulk transfer Clip Curves in selected folder to Copy")]
	static void BulkCopyCurvesToDuplicate ()
	{
		IList<AnimationClip> animationClips = null;
		foreach (Object selection in Selection.objects) {
			System.Type _type = selection.GetType ();
			string AssetPath = AssetDatabase.GetAssetPath (selection);
			Debug.Log ("Selection name:" + selection.name + " type name:" + _type.Name + " path:" + AssetPath);
			if (AssetPath != null && AssetPath != string.Empty) {
				animationClips = GetAnimationClipFromAssetFolder(AssetPath);
			}
		}
		if(animationClips != null)
		{
			foreach(AnimationClip clip in animationClips)
		    {
			  CopyClip(clip);
			  Debug.Log ("Copying curves of " + clip.name + " is done");
		    }
		}
	}
	
	/// <summary>
	/// Populate the animation.
	/// Select an GameObject and a project folder when calling this function, then all animation clip 
	/// under the selected project folder, will be populated to the selected gameobject.
	/// </summary>
	[MenuItem("Component/Animation/Copy animation clips from select asset folder into selected game object")]
	static void PopulateAnimation ()
	{
		IList<AnimationClip> animationClips = null;
		foreach (Object selection in Selection.objects) {
			System.Type _type = selection.GetType ();
			string AssetPath = AssetDatabase.GetAssetPath (selection);
			Debug.Log ("Selection name:" + selection.name + " type name:" + _type.Name + " path:" + AssetPath);
			if (AssetPath != null && AssetPath != string.Empty) {
				animationClips = GetAnimationClipFromAssetFolder(AssetPath);
			}
		}
		if(animationClips != null)
		{
			foreach (Transform t in Selection.transforms)
			{
				foreach(AnimationClip clip in animationClips)
				{
					t.animation.AddClip(clip,clip.name);
				}
			}
		}
	}
	
	/// <summary>
	/// Given an asset path, for example, assets/resources/robots,
	/// output all of the animation clip in the asset path.
	/// It only output the *.anim (Unity native animation file) and animation clips stored in *.FBX file.
	/// </summary>
	public static IList<AnimationClip> GetAnimationClipFromAssetFolder (string AssetDirPath)
	{
		IList<AnimationClip> animList = new List<AnimationClip>();
		//Scan FBX files
		string[] FBXFiles = Directory.GetFiles (AssetDirPath, "*.FBX", SearchOption.AllDirectories);
		foreach (string fbx in FBXFiles) {
			//Debug.Log ("fbx:" + fbx);
			Object[] assetObjects = AssetDatabase.LoadAllAssetsAtPath (fbx);
			foreach (Object assetObject in assetObjects) {
				if (assetObject is UnityEngine.AnimationClip) {
//					Debug.Log ("assetObject:" + assetObject.name + " type:" + assetObject.GetType ().FullName);
					animList.Add(assetObject as AnimationClip);
				}
			}
		}
		
		string[] AnimFiles = Directory.GetFiles (AssetDirPath, "*.anim", SearchOption.AllDirectories);
		foreach (string anim in AnimFiles) {
			//Debug.Log ("anim:" + fbx);
			AnimationClip clip = AssetDatabase.LoadAssetAtPath (anim, typeof(AnimationClip)) as AnimationClip;
			animList.Add(clip);
		}
		
//		foreach(AnimationClip clip in animList)
//		{
//			Debug.Log("clip :" + clip.name);
//		}
		
		return animList;
	}
}