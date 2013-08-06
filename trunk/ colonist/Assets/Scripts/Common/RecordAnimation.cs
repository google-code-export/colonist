using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

public class RecordAnimation : MonoBehaviour
{
	Dictionary<Transform, Dictionary<CurvePropertyType, AnimationCurve>> Curves = new Dictionary<Transform, Dictionary<CurvePropertyType, AnimationCurve>> ();
	/// <summary>
	/// The stop record flag. Once user check-off the flag, the recoding is done and animation cliped is generated in asset folder.
	/// </summary>
	public bool StopRecord = false;
	/// <summary>
	/// The sample interval.
	/// In the fixed time, a key frame is generated.
	/// </summary>
	public float SampleInterval = 0.002f;
	bool saved;
	public string clipName = "";
	private static Dictionary<CurvePropertyType, AnimationCurve> NewCurve ()
	{
		var curve = new Dictionary<CurvePropertyType, AnimationCurve> ();

		foreach (CurvePropertyType item in Enum.GetValues(typeof(CurvePropertyType))
                                                 .Cast<CurvePropertyType>()
                                                  .Except(new[]{CurvePropertyType.Unknown,
                                                      //CurvePropertyType.Local_ScaleX,CurvePropertyType.Local_ScaleY,CurvePropertyType.Local_ScaleZ
                                                  }))
			curve.Add (item, new AnimationCurve ());
		return curve;
	}

	void Awake ()
	{
		CreateRecursively (transform);
		Curves.Remove (transform);
	}
	
	void Start()
	{
		StartCoroutine("Recording");
	}
	
	void CreateRecursively (Transform target)
	{
		if (!Curves.ContainsKey (target)) {
			Curves.Add (target, NewCurve ());
			for (int i = 0; i < target.childCount; i++) {
				CreateRecursively (target.GetChild (i));
			}
		}
	}

	// Update is called once per frame
	IEnumerator Recording ()
	{
		while (true) {
			if (saved) {
				yield break;
			} else if (StopRecord) {
				Save ();
				yield break;
			}
		
			foreach (var item in Curves) {
				RecordRecursively (item.Key, item.Value);
			}
			yield return new WaitForSeconds(this.SampleInterval);
		}
	}

	void Save ()
	{
		AnimationClip clip = new AnimationClip ();
		foreach (var info in Curves) {
			var target = info.Key;
			string path = AnimationUtility.CalculateTransformPath (target, target.root);

			foreach (var kv in info.Value) {
				var propName = ConvertToString (kv.Key);
				clip.SetCurve (path, typeof(Transform), propName, kv.Value);
			}
		}
		clip.name = this.clipName + ".anim";
		AssetDatabase.CreateAsset (clip, "Assets/" + clip.name);
		saved = true;
	}

	string ConvertToString (CurvePropertyType type)
	{
		switch (type) {
		case CurvePropertyType.Local_PositionX:
			return "localPosition.x";
		case CurvePropertyType.Local_PositionY:
			return "localPosition.y";
		case CurvePropertyType.Local_PositionZ:
			return "localPosition.z";
		case CurvePropertyType.Local_RotationX:
			return "localRotation.x";
		case CurvePropertyType.Local_RotationY:
			return "localRotation.y";
		case CurvePropertyType.Local_RotationZ:
			return "localRotation.z";
		case CurvePropertyType.Local_RotationW:
			return "localRotation.w";
		case CurvePropertyType.Local_ScaleX:
			return "localScale.x";
		case CurvePropertyType.Local_ScaleY:
			return "localScale.y";
		case CurvePropertyType.Local_ScaleZ:
			return "localScale.z";
		case CurvePropertyType.Unknown:
		default:
			Debug.LogError ("Unknow Type:" + type.ToString ());
			return null;
		}
	}

	void RecordRecursively (Transform target, Dictionary<CurvePropertyType, AnimationCurve> curve)
	{

		curve [CurvePropertyType.Local_PositionX].AddKey (Time.time, target.localPosition.x);
		curve [CurvePropertyType.Local_PositionY].AddKey (Time.time, target.localPosition.y);
		curve [CurvePropertyType.Local_PositionZ].AddKey (Time.time, target.localPosition.z);

		curve [CurvePropertyType.Local_RotationW].AddKey (Time.time, target.localRotation.w);
		curve [CurvePropertyType.Local_RotationX].AddKey (Time.time, target.localRotation.x);
		curve [CurvePropertyType.Local_RotationY].AddKey (Time.time, target.localRotation.y);
		curve [CurvePropertyType.Local_RotationZ].AddKey (Time.time, target.localRotation.z);

		curve [CurvePropertyType.Local_ScaleX].AddKey (Time.time, target.localScale.x);
		curve [CurvePropertyType.Local_ScaleY].AddKey (Time.time, target.localScale.y);
		curve [CurvePropertyType.Local_ScaleZ].AddKey (Time.time, target.localScale.z);
	}
}
