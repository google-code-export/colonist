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
	
	Ragdoll ragdollToEdited = null;
	
	void OnGUI ()
	{
		MainWindowWidth = position.width;
		MainWindowHeight = position.height;
		GameObject selectedGameObject = Selection.activeGameObject;
		if (selectedGameObject == null) {
			Debug.LogWarning ("No gameObject is selected.");
			return;
		}
		
		if(Selection.activeObject != null &&
			Selection.activeGameObject.GetComponent<Ragdoll>() != null && GUILayout.Button("Use selected gameobject"))
		{
			ragdollToEdited = Selection.activeGameObject.GetComponent<Ragdoll>();
		}
		ragdollToEdited = (Ragdoll)EditorGUILayout.ObjectField(ragdollToEdited, typeof (Ragdoll));
		
		if(ragdollToEdited == null)
			return;
		
		if (GUILayout.Button ("Save object")) {
			EditorUtility.SetDirty (ragdollToEdited);
			EditorUtility.SetDirty (ragdollToEdited.gameObject);
		}
		
		selectedRagdoll = ragdollToEdited;
		ScrollPosition = EditorGUILayout.BeginScrollView (ScrollPosition, false, true, null);
		EditRagdollBase();
		//EditEffectData();
		if (EnableEditEffectData = EditorGUILayout.BeginToggleGroup ("---Edit Effect Data---", EnableEditEffectData)) {
		    selectedRagdoll.EffectData = EditorCommon.EditEffectData(selectedRagdoll.EffectData);
		}
		EditorGUILayout.EndToggleGroup();
		EditDecalData();
		EditRagdollJointData();
		EditorGUILayout.EndScrollView();
	}
	
	void EditRagdollBase()
	{
		EditorGUILayout.LabelField ("-------------------------Edit Ragdoll---------------");
		selectedRagdoll.Name = EditorGUILayout.TextField(new GUIContent("Name:",""), selectedRagdoll.Name);
//		selectedRagdoll.RagdollCenter = (Transform)EditorGUILayout.ObjectField(new GUIContent("Ragdoll center:",""),selectedRagdoll.RagdollCenter, typeof(Transform));
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
				RagdollJointData.JointGameObjectInitialActive = EditorGUILayout.Toggle(new GUIContent("Initial active:","If false, the gameobject will be deactivate when ragdoll created"),
					                                                                   RagdollJointData.JointGameObjectInitialActive);
				RagdollJointData.Detach = EditorGUILayout.Toggle(new GUIContent ("Detach joint?", "Detach this joint object from parent?"),
					RagdollJointData.Detach);

				RagdollJointData.SelfcontrolDestruction = EditorGUILayout.Toggle(new GUIContent ("Self control destruction?", "The joint control its destruction itself?"),
						RagdollJointData.SelfcontrolDestruction);
				if(RagdollJointData.SelfcontrolDestruction)
				{
				   RagdollJointData.SelfDestructionTime = EditorGUILayout.FloatField("Self destuction time:", RagdollJointData.SelfDestructionTime);
				}
				
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
	
	
	public static void CopyRagdollData(Ragdoll _from, Ragdoll to, bool Override)
	{
		IList<RagdollJointData> jointDataList = new List<RagdollJointData> ();
		foreach(RagdollJointData jointData in _from.RagdollJointData)
		{
			 RagdollJointData cloned = jointData.GetClone();
			 if(Override==false)
			 { 
				Util.AddToArray<RagdollJointData>(cloned, to.RagdollJointData);
			 }
			else 
			{
				jointDataList.Add(cloned);
			}
		}
		if(Override==true)
		{
			to.RagdollJointData = jointDataList.ToArray();
		}
	}
}
