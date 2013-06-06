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
				IList<RagdollJointData> l = selectedRagdoll.RagdollJointDataArray.ToList<RagdollJointData> ();
				l.Add (RagdollJointData);
				selectedRagdoll.RagdollJointDataArray = l.ToArray<RagdollJointData> ();
			}
			for (int i = 0; i < selectedRagdoll.RagdollJointDataArray.Length; i++) {
				RagdollJointData _RagdollJointData = selectedRagdoll.RagdollJointDataArray[i];
				EditorGUILayout.LabelField("---------------- Edit RagdollJointData:" + _RagdollJointData.Name + "--------");
				_RagdollJointData.Name = EditorGUILayout.TextField (new GUIContent ("Name", ""), _RagdollJointData.Name);
				
				_RagdollJointData.Joint = (Rigidbody)EditorGUILayout.ObjectField(new GUIContent("Joint","关节对象"),
					                                         _RagdollJointData.Joint, typeof(Rigidbody));
				_RagdollJointData.JointGameObjectInitialActive = EditorGUILayout.Toggle(new GUIContent("Initial active:","If false, the gameobject will be deactivate when ragdoll created"),
					                                                                   _RagdollJointData.JointGameObjectInitialActive);
				_RagdollJointData.Detach = EditorGUILayout.Toggle(new GUIContent ("Detach joint?", "Detach this joint object from parent?"),
					_RagdollJointData.Detach);

				_RagdollJointData.SelfcontrolDestruction = EditorGUILayout.Toggle(new GUIContent ("Self control destruction?", "The joint control its destruction itself?"),
						_RagdollJointData.SelfcontrolDestruction);
				if(_RagdollJointData.SelfcontrolDestruction)
				{
				   _RagdollJointData.SelfDestructionTime = EditorGUILayout.FloatField("Self destuction time:", _RagdollJointData.SelfDestructionTime);
				}
				
				_RagdollJointData.DestoryJoint = EditorGUILayout.Toggle(new GUIContent ("Destory joint?", "销毁ChracterJoint组件?"),
					_RagdollJointData.DestoryJoint);
				
				_RagdollJointData.CreateForce = EditorGUILayout.Toggle(new GUIContent ("Create force?", "Use force?"),
					_RagdollJointData.CreateForce);
				if(_RagdollJointData.CreateForce)
				{
					_RagdollJointData.forceMode = (ForceMode)EditorGUILayout.EnumPopup("Force mode:" , _RagdollJointData.forceMode);
					_RagdollJointData.CreateForceDelay = EditorGUILayout.FloatField(new GUIContent("Create force delay",""),
						_RagdollJointData.CreateForceDelay);
					_RagdollJointData.MinForceMagnitude = EditorGUILayout.FloatField(new GUIContent("Min force:",""),
						_RagdollJointData.MinForceMagnitude);
					_RagdollJointData.MaxForceMagnitude = EditorGUILayout.FloatField(new GUIContent("Max force:",""),
						_RagdollJointData.MaxForceMagnitude);
					_RagdollJointData.IsGlobalDirection = EditorGUILayout.Toggle(new GUIContent ("Global direction?", ""),
						_RagdollJointData.IsGlobalDirection);
					_RagdollJointData.ForceRandomDirection = EditorGUILayout.Toggle(new GUIContent ("Create force at random direction?", ""),
						_RagdollJointData.ForceRandomDirection);
					if(_RagdollJointData.ForceRandomDirection)
					{
						_RagdollJointData.ForceRandomDirectionFrom = EditorGUILayout.Vector3Field("Force random direction from:",_RagdollJointData.ForceRandomDirectionFrom);
						_RagdollJointData.ForceRandomDirectionTo = EditorGUILayout.Vector3Field("Force random direction to:",_RagdollJointData.ForceRandomDirectionTo);
					}
					else 
					{
						_RagdollJointData.ForceDirection = EditorGUILayout.Vector3Field("Force direction:",_RagdollJointData.ForceDirection);
					}
				}
				
				if (GUILayout.Button ("Delete RagdollJointData:" + _RagdollJointData.Name)) {
  					  IList<RagdollJointData> l = selectedRagdoll.RagdollJointDataArray.ToList<RagdollJointData> ();  
					  l.Remove (_RagdollJointData);
				  	  selectedRagdoll.RagdollJointDataArray = l.ToArray<RagdollJointData> ();
				  }
			}
		}
	}
	
	/// <summary>
	/// Copies the RagdollJoint data, EffectData from to.
	/// </summary>
	public static void CopyRagdollData(Ragdoll _from, Ragdoll to)
	{
		IList<RagdollJointData> jointDataList = new List<RagdollJointData> ();
		foreach(RagdollJointData jointData in _from.RagdollJointDataArray)
		{
		   RagdollJointData cloned = jointData.GetClone();
		   jointDataList.Add(cloned);
		}
		to.RagdollJointDataArray = jointDataList.ToArray();
		
		IList<EffectData> effectDataList = new List<EffectData> ();
		foreach(EffectData effectData in _from.EffectData)
		{
		   EffectData cloned = effectData.GetClone();
		   effectDataList.Add(cloned);
		}
		to.EffectData = effectDataList.ToArray();
	}
}
