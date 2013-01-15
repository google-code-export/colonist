﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIEditor : EditorWindow
{

	[MenuItem("Component/AI/AIEditor")]	
	public static void EditAI ()
	{
		EditorWindow.GetWindow (typeof(AIEditor));
	}

	AI AI;
	bool EnableEditUnit = false, EnableEditIdleData = false, EnableEditAttackData = false,
         EnableEditMoveData = false, EnableEditEffectData = false, EnableEditReceiveDamageData = false,
         EnableEditDecalData = false, EnableEditDeathData = false, EnableEditAIBehavior = false;
	float MainWindowWidth, MainWindowHeight;
	Vector2 ScrollPosition = Vector2.zero;

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

		if (GUILayout.Button ("Save object")) {
			EditorUtility.SetDirty (AI);
		}

		AI = selectedGameObject.GetComponent<AI> ();
		if (AI.Unit == null) {
			AI.Unit = AI.GetComponent<Unit> ();
		}
		ScrollPosition = EditorGUILayout.BeginScrollView (ScrollPosition, false, true, null);
#region Edit Unit
		EnableEditUnit = EditorGUILayout.BeginToggleGroup ("Edit Unit", EnableEditUnit);
		if (EnableEditUnit) {
			//EditBasicUnitProperty ();
			AI.Unit = (Unit)EditorCommon.EditBasicUnitProperty(AI.Unit);
			//Edit Idle Data 
			if(EnableEditIdleData = EditorGUILayout.BeginToggleGroup ("---Edit Idle Data", EnableEditIdleData))
			{
			   AI.Unit.IdleData = EditorCommon.EditIdleDataArray(AI.Unit.gameObject,
				                                              AI.Unit.IdleData);
			}
			EditorGUILayout.EndToggleGroup();
			
			//Edit Move Data 
			if(EnableEditMoveData = EditorGUILayout.BeginToggleGroup ("---Edit Move Data", EnableEditMoveData))
			{
			   AI.Unit.MoveData = EditorCommon.EditMoveDataArray(AI.Unit.gameObject,
				                                              AI.Unit.MoveData);
			}
			EditorGUILayout.EndToggleGroup();

			//Edit attack data
			if(EnableEditAttackData = EditorGUILayout.BeginToggleGroup ("---Edit Attack Data---", EnableEditAttackData))
			{
			   AI.Unit.AttackData = EditorCommon.EditAttackData(AI.Unit,
				                                                AI.Unit.AttackData);
			}
			EditorGUILayout.EndToggleGroup();

			//Edit Effect Data
			if(EnableEditEffectData = EditorGUILayout.BeginToggleGroup ("---Edit Effect Data---", EnableEditEffectData))
			{
			   AI.Unit.EffectData = EditorCommon.EditEffectData(AI.Unit.EffectData);
			}
			EditorGUILayout.EndToggleGroup();
			
			//Edit Decal data
			if(EnableEditDecalData = EditorGUILayout.BeginToggleGroup ("---Edit Decal Data---", EnableEditDecalData))
			{
				 AI.Unit.DecalData = EditorCommon.EditDecalData(AI.Unit.DecalData);
			}
			EditorGUILayout.EndToggleGroup();
			//Edit receive damage data:
			if(EnableEditReceiveDamageData = EditorGUILayout.BeginToggleGroup ("---Edit ReceiveDamage Data---", EnableEditReceiveDamageData))
			{
			   AI.Unit.ReceiveDamageData = EditorCommon.EditReceiveDamageData(AI.Unit,
				                                                              AI.Unit.ReceiveDamageData);
			}
			EditorGUILayout.EndToggleGroup();
			

			//Edit death data
			if(EnableEditDeathData = EditorGUILayout.BeginToggleGroup ("---Edit Death Data---", EnableEditDeathData))
			{
			   AI.Unit.DeathData = EditorCommon.EditDeathData(AI.Unit, AI.Unit.DeathData);
			}
			EditorGUILayout.EndToggleGroup();
		}
		EditorGUILayout.EndToggleGroup ();
        #endregion
        
#region Edit AI
		EnableEditAIBehavior = EditorGUILayout.BeginToggleGroup ("Edit AI", EnableEditAIBehavior);
		if (EnableEditAIBehavior) {
			EditBaseAIProperty ();
			EditorGUILayout.LabelField ("-------------------------Edit AI behavior---------------");
			if (GUILayout.Button ("Add new AI behavior")) {
				AIBehavior AIBehavior = new AIBehavior ();
				IList<AIBehavior> l = AI.Behaviors.ToList<AIBehavior> ();
				l.Add (AIBehavior);
				AI.Behaviors = l.ToArray<AIBehavior> ();
			}
			for (int i = 0; i < AI.Behaviors.Length; i++) {
				AIBehavior behavior = AI.Behaviors [i];
				EditAIBehavior (behavior);
			}
		}
		EditorGUILayout.EndToggleGroup ();
#endregion
		EditorGUILayout.EndScrollView ();
	}

    #region Edit AI Behavior property

	public virtual void EditBaseAIProperty ()
	{
		EditorGUILayout.LabelField (new GUIContent ("------------- AI Base property", ""));
		AI.OffensiveRange = EditorGUILayout.FloatField (new GUIContent ("AI Offensive range", "当敌人进入Offsensive range, AI会主动发起进攻."), AI.OffensiveRange);
		AI.DetectiveRange = EditorGUILayout.FloatField (new GUIContent ("AI Detective range", "当敌人进入Detective range, AI会监测到这个敌人.DetectiveRange应该大于Offensive Range."), AI.DetectiveRange);
		AI.DetectiveRange = AI.DetectiveRange >= AI.OffensiveRange ? AI.DetectiveRange : AI.OffensiveRange;
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label (new GUIContent ("Attack Obstacle:", "攻击障碍物层,如果目标和AI之间存在该层的碰撞器,则AI无法攻击到目标."));
		AI.AttackObstacle = EditorGUILayoutx.LayerMaskField ("", AI.AttackObstacle);
		EditorGUILayout.EndHorizontal ();
		AI.AlterBehaviorInterval = EditorGUILayout.FloatField (new GUIContent ("Behavior alternation time",
            "AIÐÐÎªÂÖÌæÖÜÆÚ,²»ÍÆŒöÐÞžÄÕâžöÖµ."), AI.AlterBehaviorInterval);
	}

	public virtual void EditAIBehavior (AIBehavior behavior)
	{
		EditorGUILayout.LabelField (new GUIContent ("------------- Edit AI Behavior: " + behavior.Name + " ----------------------", ""));
		behavior.Name = EditorGUILayout.TextField (new GUIContent ("Behavior Name:", ""), behavior.Name);
		behavior.Type = (AIBehaviorType)EditorGUILayout.EnumPopup (new GUIContent ("Behavior type:", ""), behavior.Type);
		behavior.Priority = EditorGUILayout.IntField (new GUIContent ("Priority:", " 行为优先级,每个行为必须有独立的优先级,优先级不能冲突."), behavior.Priority);
		if (AI.Behaviors.Where (x => x.Priority == behavior.Priority).Count () > 1) {
			EditorGUILayout.LabelField (new GUIContent ("!!! You can not have more than one behavior in priority:" + behavior.Priority));
		}
		behavior.SelectTargetRule = (SelectTargetRule)EditorGUILayout.EnumPopup (new GUIContent ("Select enemy rule:", "当这个行为生效的时候,选择敌人的规则, 默认是Closest,也就是选择最近的敌人做为当前目标."), behavior.SelectTargetRule);
		//Edit behavior data
		EditAIBehaviorData (behavior);

		//Edit Start condition
		EditorGUILayout.LabelField (new GUIContent (" --- Edit Start Condition of behavior - " + behavior.Name, ""));
		EditAIBehaviorCondition (behavior, behavior.StartCondition);
		EditorGUILayout.Space ();

		//Edit End condition
		EditorGUILayout.LabelField (new GUIContent (" --- Edit End Condition of behavior - " + behavior.Name, ""));
		EditAIBehaviorCondition (behavior, behavior.EndCondition);
		if (GUILayout.Button ("Delete " + behavior.Type.ToString () + " behavior: " + behavior.Name)) {
			IList<AIBehavior> l = AI.Behaviors.ToList<AIBehavior> ();
			l.Remove (behavior);
			AI.Behaviors = l.ToArray<AIBehavior> ();
		}
		EditorGUILayout.Space ();
	}

	public virtual void EditAIBehaviorData (AIBehavior behavior)
	{
		string[] IdleDataName = AI.Unit.IdleData.Select (x => x.Name).ToArray<string> ();
		string[] AttackDataName = AI.Unit.AttackData.Select (x => x.Name).ToArray<string> ();
		string[] MoveDataName = AI.Unit.MoveData.Select (x => x.Name).ToArray<string> ();
		int idx = 0;
		switch (behavior.Type) {
		case AIBehaviorType.Idle:
			if (IdleDataName == null || IdleDataName.Length == 0) {
				EditorGUILayout.LabelField ("!!!There is no Idle Data defined in this Unit!!!");
			} else {
				idx = IndexOfArray<string> (IdleDataName, behavior.IdleDataName);
				idx = EditorGUILayout.Popup ("Use Idle data:", idx, IdleDataName);
				behavior.IdleDataName = IdleDataName [idx];
			}
			break;
		case AIBehaviorType.MoveToTransform:
			if (MoveDataName == null || MoveDataName.Length == 0) {
				EditorGUILayout.LabelField ("!!!There is no Move Data defined in this Unit!!!");
			} else {
				idx = IndexOfArray<string> (MoveDataName, behavior.MoveDataName);
				idx = EditorGUILayout.Popup ("Use Move data:", idx, MoveDataName);
				behavior.MoveDataName = MoveDataName [idx];
				behavior.MoveToTarget = (Transform)EditorGUILayout.ObjectField (new GUIContent ("Move to target", ""),
                        behavior.MoveToTarget, typeof(Transform));
			}
			break;
		case AIBehaviorType.MoveAtDirection:
			if (MoveDataName == null || MoveDataName.Length == 0) {
				EditorGUILayout.LabelField ("!!!There is no Move Data defined in this Unit!!!");
			} else {
				idx = IndexOfArray<string> (MoveDataName, behavior.MoveDataName);
				idx = EditorGUILayout.Popup ("Use Move data:", idx, MoveDataName);
				behavior.MoveDataName = MoveDataName [idx];
				behavior.MoveDirection = EditorGUILayout.Vector3Field ("Move at direction", behavior.MoveDirection);
				behavior.IsWorldDirection = EditorGUILayout.Toggle (new GUIContent ("Is world direction?", "Move at Direction 指定的方向,是世界方向还是局部方向?"), behavior.IsWorldDirection);
			}
			break;
		case AIBehaviorType.Attack:
		case AIBehaviorType.AttackToPosition:
		case AIBehaviorType.AttackToDirection:
		case AIBehaviorType.HoldPosition:
			if (AttackDataName == null || AttackDataName.Length == 0) {
				EditorGUILayout.LabelField ("!!!There is no Attack Data defined in this Unit!!!");
				return;
			}
			if (MoveDataName == null || MoveDataName.Length == 0) {
				EditorGUILayout.LabelField ("!!!There is no Move Data defined in this Unit!!!");
				return;
			}
                //Attack Data:
			idx = IndexOfArray<string> (AttackDataName, behavior.AttackDataName);
			idx = EditorGUILayout.Popup ("Attack data:", idx, AttackDataName);
			behavior.AttackDataName = AttackDataName [idx];
                // Move data:
			idx = IndexOfArray<string> (MoveDataName, behavior.MoveDataName);
			idx = EditorGUILayout.Popup ("Move data:", idx, MoveDataName);
			behavior.MoveDataName = MoveDataName [idx];

			if (behavior.Type == AIBehaviorType.AttackToPosition) {
				behavior.MoveToTarget = (Transform)EditorGUILayout.ObjectField (new GUIContent ("Move to target", ""),
                        behavior.MoveToTarget, typeof(Transform));
			}
			if (behavior.Type == AIBehaviorType.AttackToDirection) {
				behavior.MoveDirection = EditorGUILayout.Vector3Field ("Move at direction", behavior.MoveDirection);
				behavior.IsWorldDirection = EditorGUILayout.Toggle (new GUIContent ("Is world direction?", "Move at Direction Öž¶šµÄ·œÏò,ÊÇÊÀœç·œÏò»¹ÊÇŸÖ²¿·œÏò?"), behavior.IsWorldDirection);
			}
			if (behavior.Type == AIBehaviorType.HoldPosition) {
				behavior.HoldRadius = EditorGUILayout.FloatField (new GUIContent ("Hold Position:", "ŒáÊØÕóµØµÄ·¶Î§"), behavior.HoldRadius);
			}
			break;
		}
	}

	public virtual void EditAIBehaviorCondition (AIBehavior behavior, AIBehaviorCondition Condition)
	{
		EditorGUILayout.BeginHorizontal ();
		Condition.Conjunction = (LogicConjunction)EditorGUILayout.EnumPopup (new GUIContent ("ConditionData1 ", ""), Condition.Conjunction);
		if (Condition.Conjunction == LogicConjunction.Or || Condition.Conjunction == LogicConjunction.And) {
			EditorGUILayout.LabelField ("ConditionData2");
		}
		EditorGUILayout.EndHorizontal ();
      
		EditorGUILayout.LabelField ("----ConditionData1");
		EditAIBehaviorConditionData (Condition.ConditionData1);

		if (Condition.Conjunction == LogicConjunction.And || Condition.Conjunction == LogicConjunction.Or) {
			EditorGUILayout.LabelField ("----ConditionData2");
			EditAIBehaviorConditionData (Condition.ConditionData2);
		}
	}

	public virtual void EditAIBehaviorConditionData (ConditionData ConditionData)
	{
		ConditionData.ConditionType = (AIBehaviorConditionType)EditorGUILayout.EnumPopup (new GUIContent ("Condition type:", ""), ConditionData.ConditionType);
		switch (ConditionData.ConditionType) {
		case AIBehaviorConditionType.Boolean:
			EditBooleanConditionData (ConditionData);
			break;
		case AIBehaviorConditionType.ValueComparision:
			EditValueComparisionConditionData (ConditionData);
			break;
		}
	}

	public virtual void EditBooleanConditionData (ConditionData ConditionData)
	{
		EditorGUILayout.BeginHorizontal ();
		ConditionData.BooleanCondition = (AIBooleanConditionEnum)EditorGUILayout.EnumPopup (ConditionData.BooleanCondition);
		ConditionData.BooleanOperator = (BooleanComparisionOperator)EditorGUILayout.EnumPopup (ConditionData.BooleanOperator);
		switch (ConditionData.BooleanCondition) {
		case AIBooleanConditionEnum.AlwaysTrue:
			break;
		case AIBooleanConditionEnum.CurrentTargetInLayer:
			ConditionData.LayerMaskForComparision = EditorGUILayoutx.LayerMaskField ("Current target in layermask:", ConditionData.LayerMaskForComparision);
			break;
		case AIBooleanConditionEnum.EnemyInDetectiveRange:
			break;
		case AIBooleanConditionEnum.EnemyInOffensiveRange:
			break;
		case AIBooleanConditionEnum.InArea:
			EditorGUILayout.LabelField (new GUIContent ("Use inspector to assign Area !", "AIEditor 暂不支持编辑这个字段."));
			break;
		}
		EditorGUILayout.EndHorizontal ();
	}

	public virtual void EditValueComparisionConditionData (ConditionData ConditionData)
	{
		EditorGUILayout.BeginHorizontal ();
		ConditionData.ValueComparisionCondition = (AIValueComparisionCondition)EditorGUILayout.EnumPopup (ConditionData.ValueComparisionCondition);
		ConditionData.ValueOperator = (ValueComparisionOperator)EditorGUILayout.EnumPopup (ConditionData.ValueOperator);
		switch (ConditionData.ValueComparisionCondition) {
		case AIValueComparisionCondition.BehaviorLastExecutionInterval:
		case AIValueComparisionCondition.CurrentTagetDistance:
		case AIValueComparisionCondition.FarestEnemyDistance:
		case AIValueComparisionCondition.NearestEnemyDistance:
			ConditionData.RightValueForComparision = EditorGUILayout.FloatField (ConditionData.RightValueForComparision);
			break;
		case AIValueComparisionCondition.RandomValue:
			ConditionData.RightValueForComparision = EditorGUILayout.Slider (ConditionData.RightValueForComparision, 0, 100);
			break;
		case AIValueComparisionCondition.CurrentTargetHPPercentage:
		case AIValueComparisionCondition.HPPercentage:
			ConditionData.RightValueForComparision = EditorGUILayout.Slider (ConditionData.RightValueForComparision, 0, 1);
			break;
		case AIValueComparisionCondition.ExeuctionCount:
			ConditionData.RightValueForComparision = EditorGUILayout.IntField ((int)ConditionData.RightValueForComparision);
			break;
		}
		EditorGUILayout.EndHorizontal ();
	}

    #endregion

    #region Helper functions
	public static string[] GetAnimationNames (GameObject gameObject, string CurrentAnimationName, out int index)
	{
		IList<string> AnimationList = new List<string> ();
		foreach (AnimationState state in gameObject.animation) {
			AnimationList.Add (state.name);
		}
		index = AnimationList.IndexOf (CurrentAnimationName);
		return AnimationList.ToArray<string> ();
	}

	public static string[] GetAnimationNames (GameObject gameObject)
	{
		IList<string> AnimationList = new List<string> ();
		foreach (AnimationState state in gameObject.animation) {
			AnimationList.Add (state.name);
		}
		return AnimationList.ToArray<string> ();
	}

	/// <summary>
	/// Return element index in the array.
	/// If not exists, return 0
	/// </summary>
	public static int IndexOfArray<T> (T[] array, T element)
	{
		int index = 0;
		if (array != null && array.Length > 0) {
			for (int i = 0; i < array.Length; i++) {
				if (array [i].Equals (element)) {
					index = i;
					break;
				}
			}
			return index;
		}
		return index;
	}

	public static Object[] EditObjectArray (string label, Object[] Array)
	{
		EditorGUILayout.LabelField (label);
		Object[] newArray = Array;
		if (GUILayout.Button ("Add new object element")) {
			Object element = new Object ();
			newArray = Util.AddToArray<Object> (element, newArray);
		}

		for (int i = 0; i < Array.Length; i++) {
			EditorGUILayout.BeginHorizontal ();
			Object element = Array [i];
			element = EditorGUILayout.ObjectField (element, typeof(Object));
			if (GUILayout.Button ("Remove")) {
				newArray = Util.CloneExcept<Object> (newArray, i);
				break;
			}
			Array [i] = element;
			EditorGUILayout.EndHorizontal ();
		}
		return newArray;
	}

	/// <summary>
	/// Edit a string array.
	/// array - the array to edit.
	/// displayOption - the popup group to let user select.
	/// </summary>
	/// <param name="label"></param>
	/// <param name="array"></param>
	/// <param name="displayOption"></param>
	/// <returns></returns>
	public string[] EditStringArray (string label, string[] array, string[] displayOption)
	{
		EditorGUILayout.LabelField (label);
		if (GUILayout.Button ("Add new string element")) {
			string element = "";
			array = Util.AddToArray<string> (element, array);
		}
        
		for (int i = 0; i < array.Length; i++) {
			EditorGUILayout.BeginHorizontal ();
			string element = array [i];
			int index = IndexOfArray<string> (displayOption, element);
			int oldIndex = index;
			index = EditorGUILayout.Popup ("Choose one of :", index, displayOption);
            
			element = displayOption [index];
			array [i] = element;
                
            
			if (GUILayout.Button ("Remove")) {
				array = Util.CloneExcept<string> (array, i);
				break;
			}
			EditorGUILayout.EndHorizontal ();
		}
		return array;
	}

    #endregion
}