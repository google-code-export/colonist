using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIEditor
{
	
	public AI AI;
	bool EnableEditUnit = false, EnableEditIdleData = false, EnableEditAttackData = false,
         EnableEditMoveData = false, EnableEditEffectData = false, 
	     EnableEditReceiveDamageData = false, EnableEditRotateData = false,
         EnableEditDecalData = false, EnableEditDeathData = false, EnableEditAIBehavior = false,
	     EnableEditStartCondition = false, EnableEditEndCondition = false, EnableEditAudioData = false;
	IDictionary<string, bool> AIBehaviorEnableEditFlags = new Dictionary<string,bool> ();
	
	public AIEditor (AI AI)
	{
		this.AI = AI;
		if (AI.Unit == null) {
			AI.Unit = AI.GetComponent<Unit> ();
		}
	}
	
	public void Dispose ()
	{
		//When close the window, reset all variables
		EnableEditUnit = false;
		EnableEditIdleData = false;
		EnableEditAttackData = false;
		EnableEditMoveData = false;
		EnableEditEffectData = false;
		EnableEditReceiveDamageData = false;
		EnableEditDecalData = false;
		EnableEditDeathData = false;
		EnableEditAIBehavior = false;
		AIBehaviorEnableEditFlags.Clear ();
	}
	
	public void EditUnit ()
	{
		if (AI.Unit == null) {
			AI.Unit = AI.GetComponent<Unit> ();
		}
		
#region Edit Unit
		EnableEditUnit = EditorGUILayout.BeginToggleGroup ("Edit Unit : " + AI.Unit.Name, EnableEditUnit);
		if (EnableEditUnit) {
			//EditBasicUnitProperty ();
			AI.Unit = (Unit)EditorCommon.EditBasicUnitProperty (AI.Unit);
			//Edit Start AI
			AbstractAI[] AllAI = AI.gameObject.GetComponents<AbstractAI> ();
			AI.Unit.StartAIName = EditorCommon.EditPopup ("Unit Start AI:", AI.Unit.StartAIName, AllAI.Select (x => x.Name).ToArray ());
			//Edit Idle Data 
			if (EnableEditIdleData = EditorGUILayout.BeginToggleGroup ("---Edit Idle Data", EnableEditIdleData)) {
				AI.Unit.IdleData = EditorCommon.EditIdleDataArray (AI.Unit.gameObject,
				                                              AI.Unit.IdleData);
			}
			EditorGUILayout.EndToggleGroup ();
			
			//Edit rotate data
			if (EnableEditRotateData = EditorGUILayout.BeginToggleGroup ("---Edit Rotate Data", EnableEditRotateData)) {
				AI.Unit.RotateData = EditorCommon.EditRotateDataArray (AI.Unit.gameObject,
				                                              AI.Unit.RotateData);
			}
			EditorGUILayout.EndToggleGroup ();
			
			//Edit Move Data 
			if (EnableEditMoveData = EditorGUILayout.BeginToggleGroup ("---Edit Move Data", EnableEditMoveData)) {
				AI.Unit.MoveData = EditorCommon.EditMoveDataArray (AI.Unit.gameObject,
				                                              AI.Unit.MoveData);
			}
			EditorGUILayout.EndToggleGroup ();

			//Edit attack data
			if (EnableEditAttackData = EditorGUILayout.BeginToggleGroup ("---Edit Attack Data---", EnableEditAttackData)) {
				AI.Unit.AttackData = EditorCommon.EditAttackDataArray (AI.Unit,
				                                                AI.Unit.AttackData);
			}
			EditorGUILayout.EndToggleGroup ();
			
			

			//Edit Effect Data
			if (EnableEditEffectData = EditorGUILayout.BeginToggleGroup ("---Edit Effect Data---", EnableEditEffectData)) {
				AI.Unit.EffectData = EditorCommon.EditEffectData (AI.Unit.EffectData);
			}
			EditorGUILayout.EndToggleGroup ();
			
			//Edit Decal data
			if (EnableEditDecalData = EditorGUILayout.BeginToggleGroup ("---Edit Decal Data---", EnableEditDecalData)) {
				AI.Unit.DecalData = EditorCommon.EditDecalData (AI.Unit.DecalData);
			}
			EditorGUILayout.EndToggleGroup ();
			//Edit receive damage data:
			if (EnableEditReceiveDamageData = EditorGUILayout.BeginToggleGroup ("---Edit ReceiveDamage Data---", EnableEditReceiveDamageData)) {
				AI.Unit.ReceiveDamageData = EditorCommon.EditReceiveDamageData (AI.Unit,
				                                                              AI.Unit.ReceiveDamageData);
			}
			EditorGUILayout.EndToggleGroup ();
			

			//Edit death data
			if (EnableEditDeathData = EditorGUILayout.BeginToggleGroup ("---Edit Death Data---", EnableEditDeathData)) {
				AI.Unit.DeathData = EditorCommon.EditDeathData (AI.Unit, AI.Unit.DeathData);
			}
			EditorGUILayout.EndToggleGroup ();
			
			//Edit audio data
			if(EnableEditAudioData = EditorGUILayout.BeginToggleGroup ("---Edit Audio Data---", EnableEditAudioData)) 
			{
				AI.Unit.AudioData = EditorCommon.EditAudioData (AI.Unit, AI.Unit.AudioData);
			}
			EditorGUILayout.EndToggleGroup ();
		}
		EditorGUILayout.EndToggleGroup ();
#endregion
	}
	
	public void EditAI ()
	{

//		EnableEditAIBehavior = EditorGUILayout.BeginToggleGroup ("Edit AI : " + AI.Name, EnableEditAIBehavior);
//		if (EnableEditAIBehavior) {
			EditBaseAIProperty (AI);
			EditorGUILayout.LabelField ("-------------------------Edit AI behaviors---------------");
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
//		}
//		EditorGUILayout.EndToggleGroup ();

	}
	
	public void EditUnitAndAI ()
	{
		if (GUILayout.Button ("Save object")) {
			EditorUtility.SetDirty (AI);
		}
		EditUnit ();
		EditAI ();
	}
	
    #region Edit AI Behavior property

	public virtual void EditBaseAIProperty (AI AI)
	{
		EditorGUILayout.LabelField (new GUIContent ("------------- AI Base property", ""));
		AI.Name = EditorGUILayout.TextField (new GUIContent ("AI Name", "Name of this AI component."), AI.Name);
		AI.Description = EditorGUILayout.TextField (new GUIContent ("AI Description", "Description:."), AI.Description);
		AI.OffensiveRange = EditorGUILayout.FloatField (new GUIContent ("AI Offensive range", "当敌人进入Offsensive range, AI会主动发起进攻."), AI.OffensiveRange);
		AI.DetectiveRange = EditorGUILayout.FloatField (new GUIContent ("AI Detective range", "当敌人进入Detective range, AI会监测到这个敌人.DetectiveRange应该大于Offensive Range."), AI.DetectiveRange);
		AI.DetectiveRange = AI.DetectiveRange >= AI.OffensiveRange ? AI.DetectiveRange : AI.OffensiveRange;
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label (new GUIContent ("Attack Obstacle:", "攻击障碍物层,如果目标和AI之间存在该层的碰撞器,则AI无法攻击到目标."));
		AI.AttackObstacle = EditorGUILayoutx.LayerMaskField ("", AI.AttackObstacle);
		EditorGUILayout.EndHorizontal ();
//		AI.AlterBehaviorInterval = EditorGUILayout.FloatField (new GUIContent ("Behavior alternation time",
//            "Interval to alter behavior."), AI.AlterBehaviorInterval);
		
		AIBehavior[] AIBehaviors = this.AI.Behaviors; 
		this.AI.FirstBehavior = EditorCommon.EditPopup ("First Behavior:", this.AI.FirstBehavior , AIBehaviors.Select (x => x.Name).ToArray ());
	}

	public virtual void EditAIBehavior (AIBehavior behavior)
	{
		behavior.Name = EditorGUILayout.TextField (new GUIContent ("Behavior Name:", ""), behavior.Name);
		
		if (AIBehaviorEnableEditFlags.ContainsKey (behavior.Name) == false) {
			AIBehaviorEnableEditFlags [behavior.Name] = false;
		}
		AIBehaviorEnableEditFlags [behavior.Name] = EditorGUILayout.BeginToggleGroup (new GUIContent (
			string.Format ("------------- Edit AI Behavior: {0} ----------------------", behavior.Name), ""),
			AIBehaviorEnableEditFlags [behavior.Name]);
		
		if (AIBehaviorEnableEditFlags [behavior.Name]) {
			behavior.Type = (AIBehaviorType)EditorGUILayout.EnumPopup (new GUIContent ("Behavior type:", ""), behavior.Type);
			
            AIBehavior[] AIBehaviors = this.AI.Behaviors; 
			
			behavior.AlterBehaviorInterval = EditorGUILayout.FloatField("Alter behavior interval:" , behavior.AlterBehaviorInterval);
			
			behavior.SelectTargetRule = (SelectTargetRule)EditorGUILayout.EnumPopup (new GUIContent ("Select enemy rule:", "当这个行为生效的时候,选择敌人的规则, 默认是Closest,也就是选择最近的敌人做为当前目标."), behavior.SelectTargetRule);
			//Edit behavior data
			EditAIBehaviorData (behavior);
			
			//Edit End Condition Wrapper, for behavior type = SwitchToAI, it's not necessary to edit end condition.
			if (behavior.Type != AIBehaviorType.SwitchToAI) {
				EditorGUILayout.Space();
				if(GUILayout.Button("Edit alternate behavior data"))
				{
					AlternateBehaviorEditorWindow.DisplayConditionEditorWindow (this, behavior);
				}
				EditorGUILayout.Space();
			}

			if (GUILayout.Button ("Delete " + behavior.Type.ToString () + " behavior: " + behavior.Name)) {
				IList<AIBehavior> l = AI.Behaviors.ToList<AIBehavior> ();
				l.Remove (behavior);
				AI.Behaviors = l.ToArray<AIBehavior> ();
			}
			
			//Start and End message:
			behavior.MessageAtStart = EditorCommon.EditStringArray ("Message sent when behavior start", behavior.MessageAtStart);
			behavior.MessageAtEnd = EditorCommon.EditStringArray ("Message sent when behavior end", behavior.MessageAtEnd);
		}
		EditorGUILayout.EndToggleGroup ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
	}

	public virtual void EditAIBehaviorData (AIBehavior behavior)
	{
		string[] IdleDataNameArray = this.AI.Unit.IdleData.Select (x => x.Name).ToArray<string> ();
		string[] AttackDataNameArray = this.AI.Unit.AttackData.Select (x => x.Name).ToArray<string> ();
		string[] MoveDataNameArray = this.AI.Unit.MoveData.Select (x => x.Name).ToArray<string> ();
		string[] SceneWaypointNameArray = Object.FindObjectsOfType(typeof(WayPoint)).Select(x=>x.name).ToArray();
		SceneWaypointNameArray.OrderBy(x=>x.ToString());
		
		int idx = 0;
		switch (behavior.Type) {
		case AIBehaviorType.Idle:
			if (IdleDataNameArray == null || IdleDataNameArray.Length == 0) {
				EditorGUILayout.LabelField ("!!!There is no Idle Data defined in this Unit!!!");
			} else {
				idx = IndexOfArray<string> (IdleDataNameArray, behavior.IdleDataName);
				idx = EditorGUILayout.Popup ("Use Idle data:", idx, IdleDataNameArray);
				behavior.IdleDataName = IdleDataNameArray [idx];
			}
			break;
		case AIBehaviorType.MoveToTransform:
			if (MoveDataNameArray == null || MoveDataNameArray.Length == 0) {
				EditorGUILayout.LabelField ("!!!There is no Move Data defined in this Unit!!!");
			} else {
				idx = IndexOfArray<string> (MoveDataNameArray, behavior.MoveDataName);
				idx = EditorGUILayout.Popup ("Use Move data:", idx, MoveDataNameArray);
				behavior.MoveDataName = MoveDataNameArray [idx];
				behavior.MoveToTarget = (Transform)EditorGUILayout.ObjectField (new GUIContent ("Move to target", ""),
                        behavior.MoveToTarget, typeof(Transform));
			}
			break;
		case AIBehaviorType.MoveAtDirection:
			if (MoveDataNameArray == null || MoveDataNameArray.Length == 0) {
				EditorGUILayout.LabelField ("!!!There is no Move Data defined in this Unit!!!");
			} else {
				idx = IndexOfArray<string> (MoveDataNameArray, behavior.MoveDataName);
				idx = EditorGUILayout.Popup ("Use Move data:", idx, MoveDataNameArray);
				behavior.MoveDataName = MoveDataNameArray [idx];
				behavior.MoveDirection = EditorGUILayout.Vector3Field ("Move at direction", behavior.MoveDirection);
				behavior.IsWorldDirection = EditorGUILayout.Toggle (new GUIContent ("Is world direction?", "Move at Direction 指定的方向,是世界方向还是局部方向?"), behavior.IsWorldDirection);
			}
			break;
		case AIBehaviorType.MoveToCurrentTarget:
			if (MoveDataNameArray == null || MoveDataNameArray.Length == 0) {
				EditorGUILayout.LabelField ("!!!There is no Move Data defined in this Unit!!!");
			} else {
				idx = IndexOfArray<string> (MoveDataNameArray, behavior.MoveDataName);
				idx = EditorGUILayout.Popup ("Use Move data:", idx, MoveDataNameArray);
				behavior.MoveDataName = MoveDataNameArray [idx];
			}
			break;
		case AIBehaviorType.Attack:
		case AIBehaviorType.AttackToPosition:
		case AIBehaviorType.AttackToDirection:
		case AIBehaviorType.HoldPosition:
			if (AttackDataNameArray == null || AttackDataNameArray.Length == 0) {
				EditorGUILayout.LabelField ("!!!There is no Attack Data defined in this Unit!!!");
				return;
			}
			if (MoveDataNameArray == null || MoveDataNameArray.Length == 0) {
				EditorGUILayout.LabelField ("!!!There is no Move Data defined in this Unit!!!");
				return;
			}
			
            //Attack Data:
			behavior.UseRandomAttackData = EditorGUILayout.Toggle (new GUIContent ("Use random attack data", ""), behavior.UseRandomAttackData);
			if (behavior.UseRandomAttackData) {
				behavior.AttackDataNameArray = EditorCommon.EditStringArray ("Attack data:", behavior.AttackDataNameArray, AttackDataNameArray);
			} else {
				behavior.AttackDataName = EditorCommon.EditPopup ("Attack data:", behavior.AttackDataName, AttackDataNameArray);
			}
            // Move data:
			idx = IndexOfArray<string> (MoveDataNameArray, behavior.MoveDataName);
			idx = EditorGUILayout.Popup ("Move data:", idx, MoveDataNameArray);
			behavior.MoveDataName = MoveDataNameArray [idx];

			if (behavior.Type == AIBehaviorType.AttackToPosition) {
				behavior.MoveToTarget = (Transform)EditorGUILayout.ObjectField (new GUIContent ("Move to target", ""),
                        behavior.MoveToTarget, typeof(Transform));
			}
			if (behavior.Type == AIBehaviorType.AttackToDirection) {
				behavior.MoveDirection = EditorGUILayout.Vector3Field ("Move at direction", behavior.MoveDirection);
				behavior.IsWorldDirection = EditorGUILayout.Toggle (new GUIContent ("Is world direction?", "Move at Direction in world space or local space?"), behavior.IsWorldDirection);
			}
			if (behavior.Type == AIBehaviorType.HoldPosition) {
				behavior.HoldRadius = EditorGUILayout.FloatField (new GUIContent ("Hold Position:", "The position transform."), behavior.HoldRadius);
			}
			
			//Idle data:
			behavior.AttackInterrupt = EditorGUILayout.Toggle("Attack interrupt:", behavior.AttackInterrupt);
			if(behavior.AttackInterrupt)
			{
				behavior.IdleDataName = EditorCommon.EditPopup ("Interval Idle data:", behavior.IdleDataName, IdleDataNameArray);
				behavior.AttackIntervalMin = EditorGUILayout.FloatField("Min attack interval:" , behavior.AttackIntervalMin );
				behavior.AttackIntervalMax = EditorGUILayout.FloatField("Max attack interval:" , behavior.AttackIntervalMax );
			}
			break;
		case AIBehaviorType.SwitchToAI:
			AbstractAI[] AllAI = this.AI.Unit.GetComponents<AbstractAI> (); 
			behavior.SwitchToAIName = EditorCommon.EditStringArray ("Switch to next AI:", behavior.SwitchToAIName, AllAI.Select (x => x.Name).ToArray ());
			break;
		case AIBehaviorType.MoveToWaypoint:
		    // Move data:
			idx = IndexOfArray<string> (MoveDataNameArray, behavior.MoveDataName);
			idx = EditorGUILayout.Popup ("Move data:", idx, MoveDataNameArray);
			behavior.MoveDataName = MoveDataNameArray [idx];
			behavior.WaypointNames = EditorCommon.EditStringArray("Select scene waypoint:", behavior.WaypointNames, SceneWaypointNameArray);
			break;
		}
	}

#endregion
	
	#region Edit condition functions

	/// <summary>
	/// Composites the condition to string description.
	/// </summary>
	public string GetCompositeConditionDescription (CompositeCondition compositeCondition, 
		                                           CompositeConditionWrapper compositeConditionWrapper)
	{
		string LeftEntityDescription = "";
		string RightEntityDescription = "";
	  	
		string refId = ""; 
		switch (compositeCondition.Entity1.EntityType) {
		case ConditionEntityType.AtomCondition:
			refId = compositeCondition.Entity1.EntityReferenceId;
			IEnumerable<AtomConditionData> AllAtomConditionData = compositeConditionWrapper.atomConditionDataArray.Where (x => x.Id == refId);
			if (AllAtomConditionData.Count () > 0) {
				AtomConditionData atomCondition = AllAtomConditionData.First ();
				LeftEntityDescription = atomCondition.GetDescription ();
			}
			break;
		case ConditionEntityType.ReferenceToComposite:
			refId = compositeCondition.Entity1.EntityReferenceId;
			IEnumerable<CompositeCondition> AllCompositeConditionData = compositeConditionWrapper.CompositeConditionArray.Where (x => x.Id == refId);
			if (AllCompositeConditionData.Count () > 0) {
				CompositeCondition referComposite = AllCompositeConditionData.Where (x => x.Id == refId).First ();
				if (referComposite == compositeCondition) {
					LeftEntityDescription = "Error !!! CompositeCondition - Id:" + compositeCondition.Id + " is referring to itself!";
				} else {
					LeftEntityDescription = GetCompositeConditionDescription (referComposite, compositeConditionWrapper);
				}
			}
			break;
		}
	  
		switch (compositeCondition.Entity2.EntityType) {
		case ConditionEntityType.AtomCondition:
			refId = compositeCondition.Entity2.EntityReferenceId;
			IEnumerable<AtomConditionData> AllAtomConditionData = compositeConditionWrapper.atomConditionDataArray.Where (x => x.Id == refId);
			if (AllAtomConditionData.Count () > 0) {
				AtomConditionData atomCondition = AllAtomConditionData.First ();
				RightEntityDescription = atomCondition.GetDescription ();
			}
			break;
		case ConditionEntityType.ReferenceToComposite:
			refId = compositeCondition.Entity2.EntityReferenceId;
			IEnumerable<CompositeCondition> AllCompositeConditionData = compositeConditionWrapper.CompositeConditionArray.Where (x => x.Id == refId);
			if (AllCompositeConditionData.Count () > 0) {
				CompositeCondition referComposite = AllCompositeConditionData.First ();
				if (referComposite == compositeCondition) {
					RightEntityDescription = "Error !!! CompositeCondition - Id:" + compositeCondition.Id + " is referring to itself!";
				} else {
					RightEntityDescription = GetCompositeConditionDescription (referComposite, compositeConditionWrapper);
				}
			}
			break;
		}
		
		string ret = "({0} {1} {2})";
		switch (compositeCondition.Operator) {
		case LogicConjunction.None:
			ret = string.Format (ret, LeftEntityDescription, "", "");
			break;
		case LogicConjunction.And:
		case LogicConjunction.Or:
			ret = string.Format (ret, LeftEntityDescription, compositeCondition.Operator.ToString (), RightEntityDescription);
			break;
		}
		return ret;
	}
	
	public virtual void EditCompositeCondition (CompositeCondition compositeCondition, CompositeConditionWrapper conditionWrapper)
	{
		EditorGUILayout.LabelField (" -------------- Composite condition:" + compositeCondition.Id + " ------------ ");
		compositeCondition.Id = EditorGUILayout.TextField ("Id:", compositeCondition.Id);
		compositeCondition.Operator = (LogicConjunction)EditorGUILayout.EnumPopup (new GUIContent ("Operator ", ""), compositeCondition.Operator);
		if (compositeCondition.Operator == LogicConjunction.None) {
			EditCompositeConditionEntity ("Entity 1:", compositeCondition.Entity1, conditionWrapper);
		} else {
			EditCompositeConditionEntity ("Entity 1:", compositeCondition.Entity1, conditionWrapper);
			EditCompositeConditionEntity ("Entity 2:", compositeCondition.Entity2, conditionWrapper);
		}
		EditorGUILayout.LabelField ("Condition description:");
		EditorGUILayout.LabelField (GetCompositeConditionDescription (compositeCondition, conditionWrapper));
	}
	
	public virtual void EditCompositeConditionEntity (string Label, ConditionEntity entity, 
		                                             CompositeConditionWrapper compositionConditionWrapper)
	{
		EditorGUILayout.LabelField (Label);
		entity.EntityType = (ConditionEntityType)EditorGUILayout.EnumPopup (new GUIContent ("Entity type ", ""), entity.EntityType);
		string label = "";
		switch (entity.EntityType) {
		case ConditionEntityType.AtomCondition:
			label = "Choose one of atom condition id:";
			entity.EntityReferenceId = EditorCommon.EditPopup (label,
			                                                  entity.EntityReferenceId,
			                                                  compositionConditionWrapper.atomConditionDataArray.Select (x => x.Id).ToArray ());
			break;
		case ConditionEntityType.ReferenceToComposite:
			label = "Choose one of composite condition id:";
			entity.EntityReferenceId = EditorCommon.EditPopup (label,
			                                                  entity.EntityReferenceId,
			                                                  compositionConditionWrapper.CompositeConditionArray.Select (x => x.Id).ToArray ());
			break;
		}
		

	}

	public virtual void EditAtomConditionData (AtomConditionData ConditionData)
	{
		EditorGUILayout.LabelField (" -------------- Atom condition:" + ConditionData.Id + "------------");
		ConditionData.ConditionType = (AIBehaviorConditionType)EditorGUILayout.EnumPopup (new GUIContent ("Condition type:", ""), ConditionData.ConditionType);
		ConditionData.Id = EditorGUILayout.TextField ("AtomCondition ID:", ConditionData.Id);
		switch (ConditionData.ConditionType) {
		case AIBehaviorConditionType.Boolean:
			EditBooleanConditionData (ConditionData);
			break;
		case AIBehaviorConditionType.ValueComparision:
			EditValueComparisionConditionData (ConditionData);
			break;
		}
	}

	public virtual void EditBooleanConditionData (AtomConditionData ConditionData)
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
			ConditionData.CheckArea = (Collider)EditorGUILayout.ObjectField("Check against collider:", ConditionData.CheckArea, typeof(Collider));
			break;
		case AIBooleanConditionEnum.LatestBehaviorNameIs:
			string[] AllBehaviorName = AI.Behaviors.Select (x => x.Name).ToArray ();
			ConditionData.StringValue = EditorCommon.EditPopup ("behavior name:", ConditionData.StringValue
				, AllBehaviorName);
			break;
		case AIBooleanConditionEnum.LastestBehaviorNameIsOneOf:
			AllBehaviorName = AI.Behaviors.Select (x => x.Name).ToArray ();
			EditorGUILayout.BeginVertical ();
			ConditionData.StringValueArray = EditorCommon.EditStringArray ("behavior name:", ConditionData.StringValueArray
				, AllBehaviorName);
			EditorGUILayout.EndVertical ();
			break;
		}
		EditorGUILayout.EndHorizontal ();
	}

	public virtual void EditValueComparisionConditionData (AtomConditionData ConditionData)
	{
		EditorGUILayout.BeginHorizontal ();
		ConditionData.ValueComparisionCondition = (AIValueComparisionCondition)EditorGUILayout.EnumPopup (ConditionData.ValueComparisionCondition);
		ConditionData.ValueOperator = (ValueComparisionOperator)EditorGUILayout.EnumPopup (ConditionData.ValueOperator);
		switch (ConditionData.ValueComparisionCondition) {
		case AIValueComparisionCondition.BehaviorLastExecutionInterval:
		case AIValueComparisionCondition.BehaveTime:
		case AIValueComparisionCondition.CurrentTagetDistance:
		case AIValueComparisionCondition.FarestEnemyDistance:
		case AIValueComparisionCondition.NearestEnemyDistance:
		case AIValueComparisionCondition.LastConditionMatchTimeInterval:
		case AIValueComparisionCondition.WaypointDistance:
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
		case AIValueComparisionCondition.AttackCount:
		case AIValueComparisionCondition.DoDamageCount:
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
