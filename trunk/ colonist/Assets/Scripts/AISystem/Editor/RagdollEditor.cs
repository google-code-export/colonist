using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RagdollEditor : EditorWindow {

	[MenuItem("Component/AI/Ragdoll Editor")]	
	public static void EditRagdollWindow ()
	{
		EditorWindow.GetWindow (typeof(RagdollEditor));
	}
	
	float MainWindowWidth, MainWindowHeight;
	Vector2 ScrollPosition = Vector2.zero;
	Ragdoll selectedRagdoll;
	bool EnableEditDecalData = false, EnableEditEffectData = false, EnableEditRagdollData = false;
	
	void OnGUI ()
	{
		MainWindowWidth = position.width;
		MainWindowHeight = position.height;
		GameObject selectedGameObject = Selection.activeGameObject;
		if (selectedGameObject == null) {
			Debug.LogWarning ("No gameObject is selected.");
			return;
		}
		//Attach Ragdoll script button
		if (selectedGameObject.GetComponent<Ragdoll> () == null) {
			Rect newRagdollScriptButton = new Rect (0, 0, MainWindowWidth - 10, 30);
			if (GUI.Button (newRagdollScriptButton, "Attach Ragdoll script")) {
				selectedGameObject.AddComponent<Ragdoll> ();
			}
			return;
		}
		
		if (GUILayout.Button ("Save object")) {
			EditorUtility.SetDirty (selectedGameObject);
		}
		
		selectedRagdoll = selectedGameObject.GetComponent<Ragdoll>();
		ScrollPosition = EditorGUILayout.BeginScrollView (ScrollPosition, false, true, null);
		EditRagdollBase();
		//EditEffectData();
		selectedRagdoll.EffectData = EditorCommon.EditEffectData (selectedRagdoll.EffectData);
		EditDecalData();
		EditRagdollJointData();
		EditorGUILayout.EndScrollView();
	}
	
	void EditRagdollBase()
	{
		EditorGUILayout.LabelField ("-------------------------Edit Ragdoll---------------");
		selectedRagdoll.Name = EditorGUILayout.TextField(new GUIContent("Name:",""), selectedRagdoll.Name);
		selectedRagdoll.RagdollCenter = (Transform)EditorGUILayout.ObjectField(new GUIContent("Ragdoll center:",""),selectedRagdoll.RagdollCenter, typeof(Transform));
		EditorGUILayout.BeginHorizontal();
		selectedRagdoll.AutoDestory = EditorGUILayout.Toggle(new GUIContent("Auto destory in lifetime:",""),selectedRagdoll.AutoDestory);
		if(selectedRagdoll.AutoDestory)
		{
			selectedRagdoll.LifeTime = EditorGUILayout.FloatField("life time:", selectedRagdoll.LifeTime);
		}
		EditorGUILayout.EndHorizontal();
	}
	
	public virtual void EditDecalData ()
	{
		EnableEditDecalData = EditorGUILayout.BeginToggleGroup ("---Edit Decal Data", EnableEditDecalData);
		if (EnableEditDecalData) {
			if (GUILayout.Button ("Add Decal data")) {
				DecalData DecalData = new DecalData ();
				IList<DecalData> l = selectedRagdoll.DecalData.ToList<DecalData> ();
				l.Add (DecalData);
				selectedRagdoll.DecalData = l.ToArray<DecalData> ();
			}
			for (int i = 0; i < selectedRagdoll.DecalData.Length; i++) {
				DecalData DecalData = selectedRagdoll.DecalData [i];
				EditorGUILayout.LabelField ("------------------------ " + DecalData.Name);
				DecalData.Name = EditorGUILayout.TextField (new GUIContent ("Name", ""), DecalData.Name); 
				DecalData.UseGlobalDecal = EditorGUILayout.Toggle (new GUIContent ("UseGlobalDecal?", "使用全局Decal设定?"), DecalData.UseGlobalDecal);
				if (DecalData.UseGlobalDecal) {
					DecalData.GlobalType = (GlobalDecalType)EditorGUILayout.EnumPopup (new GUIContent ("Global decal type:", ""), DecalData.GlobalType);
				} else {
					DecalData.DestoryInTimeOut = EditorGUILayout.Toggle (new GUIContent ("Destory this decal in timeout?", "Decal是否有Lifetime?"), DecalData.DestoryInTimeOut);
					if (DecalData.DestoryInTimeOut) {
						DecalData.DestoryTimeOut = EditorGUILayout.FloatField (new GUIContent ("Destory Timeout:", ""), DecalData.DestoryTimeOut);
					}
					DecalData.ProjectDirection = (HorizontalOrVertical)EditorGUILayout.EnumPopup (new GUIContent ("Decal Project Direction", "创建Decal的投射方向,对于地上的Decal,投射方向是Vertical,对于墙上的Decal,投射方向是Horizontal."), DecalData.ProjectDirection);
					DecalData.ApplicableLayer = EditorGUILayoutx.LayerMaskField ("ApplicableLayer", DecalData.ApplicableLayer);
					DecalData.DecalObjects = AIEditor.EditObjectArray ("--------------Edit Decal object ------", DecalData.DecalObjects);
					DecalData.ScaleRate = EditorGUILayout.FloatField (new GUIContent ("Scale rate:", "Final scale = initial scale * ScaleRate"), DecalData.ScaleRate);
					//Delete this DecalData
					if (GUILayout.Button ("Delete DecalData:" + DecalData.Name)) {
						IList<DecalData> l = selectedRagdoll.DecalData.ToList<DecalData> ();
						l.Remove (DecalData);
						selectedRagdoll.DecalData = l.ToArray<DecalData> ();
					}
				}
				EditorGUILayout.Space ();
			}
		}
		EditorGUILayout.EndToggleGroup ();
	}
	
	public virtual void EditEffectData ()
	{
		EnableEditEffectData = EditorGUILayout.BeginToggleGroup ("---Edit Effect Data", EnableEditEffectData);
		if (EnableEditEffectData) {
			if (GUILayout.Button ("Add Effect data")) {
				EffectData EffectData = new EffectData ();
				IList<EffectData> l = selectedRagdoll.EffectData.ToList<EffectData> ();
				l.Add (EffectData);
				selectedRagdoll.EffectData = l.ToArray<EffectData> ();
			}
			for (int i = 0; i < selectedRagdoll.EffectData.Length; i++) {
				EffectData EffectData = selectedRagdoll.EffectData [i];
				EditorGUILayout.LabelField ("------------------------ " + EffectData.Name);
				EffectData.Name = EditorGUILayout.TextField (new GUIContent ("Name", ""), EffectData.Name);
				
				EffectData.UseGlobalEffect = EditorGUILayout.Toggle (new GUIContent ("Use global effect?", "是否使用全局Effect?"), EffectData.UseGlobalEffect);
				if(EffectData.UseGlobalEffect)
				{
					EffectData.GlobalType = (GlobalEffectType)EditorGUILayout.EnumPopup(new GUIContent ("Global effect type", "是全局Effect类型"),
						EffectData.GlobalType);
				}
				else {
				  EffectData.EffectObject = (GameObject)EditorGUILayout.ObjectField (new GUIContent ("Effect object", ""), EffectData.EffectObject, typeof(GameObject));
				  EffectData.Anchor = (Transform)EditorGUILayout.ObjectField (new GUIContent ("Effect creation anchor", ""), EffectData.Anchor, typeof(Transform));
				  EffectData.DestoryInTimeOut = EditorGUILayout.Toggle (new GUIContent ("Destory this effect in timeout?", "是否在N秒内删除这个效果?"), EffectData.DestoryInTimeOut);
				  if (EffectData.DestoryInTimeOut) {
				    	EffectData.DestoryTimeOut = EditorGUILayout.FloatField (new GUIContent ("Destory Timeout:", ""), EffectData.DestoryTimeOut);
				  }
				  if (GUILayout.Button ("Delete EffectData:" + EffectData.Name)) {
  					  IList<EffectData> l = selectedRagdoll.EffectData.ToList<EffectData> ();  
					  l.Remove (EffectData);
				  	  selectedRagdoll.EffectData = l.ToArray<EffectData> ();
				  }
				}
				EditorGUILayout.Space ();
			}
		}
		EditorGUILayout.EndToggleGroup ();
	}
	
	
	public virtual void EditRagdollJointData()
	{
		EnableEditRagdollData = EditorGUILayout.BeginToggleGroup ("---Edit RagdollJointData", EnableEditRagdollData);
		if (EnableEditRagdollData) {
			if (GUILayout.Button ("Add RagdollJointData")) {
				RagdollJointData RagdollJointData = new RagdollJointData ();
				IList<RagdollJointData> l = selectedRagdoll.RagdollJointData.ToList<RagdollJointData> ();
				l.Add (RagdollJointData);
				selectedRagdoll.RagdollJointData = l.ToArray<RagdollJointData> ();
			}
			for (int i = 0; i < selectedRagdoll.RagdollJointData.Length; i++) {
				RagdollJointData RagdollJointData = selectedRagdoll.RagdollJointData[i];
				EditorGUILayout.LabelField("---------------- Edit RagdollJointData:" + RagdollJointData.Name + "--------");
				RagdollJointData.Name = EditorGUILayout.TextField (new GUIContent ("Name", ""), RagdollJointData.Name);
				
				RagdollJointData.Joint = (Rigidbody)EditorGUILayout.ObjectField(new GUIContent("Joint","关节对象"),
					                                         RagdollJointData.Joint, typeof(Rigidbody));
				RagdollJointData.Detach = EditorGUILayout.Toggle(new GUIContent ("Detach joint?", "把关节分离?"),
					RagdollJointData.Detach);
				RagdollJointData.DestoryJoint = EditorGUILayout.Toggle(new GUIContent ("Destory joint?", "销毁ChracterJoint组件?"),
					RagdollJointData.DestoryJoint);
				
				RagdollJointData.CreateForce = EditorGUILayout.Toggle(new GUIContent ("Create force?", "Use force?"),
					RagdollJointData.CreateForce);
				if(RagdollJointData.CreateForce)
				{
					RagdollJointData.CreateForceDelay = EditorGUILayout.FloatField(new GUIContent("Create force delay",""),
						RagdollJointData.CreateForceDelay);
					RagdollJointData.MinForceMagnitude = EditorGUILayout.FloatField(new GUIContent("Min force:",""),
						RagdollJointData.MinForceMagnitude);
					RagdollJointData.MaxForceMagnitude = EditorGUILayout.FloatField(new GUIContent("Max force:",""),
						RagdollJointData.MaxForceMagnitude);
					RagdollJointData.IsGlobalDirection = EditorGUILayout.Toggle(new GUIContent ("Global direction?", ""),
						RagdollJointData.IsGlobalDirection);
					RagdollJointData.ForceRandomDirection = EditorGUILayout.Toggle(new GUIContent ("Create force at random direction?", ""),
						RagdollJointData.ForceRandomDirection);
					if(RagdollJointData.ForceRandomDirection)
					{
						RagdollJointData.ForceRandomDirectionFrom = EditorGUILayout.Vector3Field("Force random direction from:",RagdollJointData.ForceRandomDirectionFrom);
						RagdollJointData.ForceRandomDirectionTo = EditorGUILayout.Vector3Field("Force random direction to:",RagdollJointData.ForceRandomDirectionTo);
					}
					else 
					{
						RagdollJointData.ForceDirection = EditorGUILayout.Vector3Field("Force direction:",RagdollJointData.ForceDirection);
					}
				}
				
				if (GUILayout.Button ("Delete RagdollJointData:" + RagdollJointData.Name)) {
  					  IList<RagdollJointData> l = selectedRagdoll.RagdollJointData.ToList<RagdollJointData> ();  
					  l.Remove (RagdollJointData);
				  	  selectedRagdoll.RagdollJointData = l.ToArray<RagdollJointData> ();
				  }
			}
		}
	}
}
