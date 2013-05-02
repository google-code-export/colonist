using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Defines the data to alternate to another behavior.
/// Includes:
/// 1. Priority
/// 2. CompositeConditionWrapper - when codition = true, alter to another behavior
/// 3. Next behavior name.
/// </summary>
[System.Serializable]
public class AlternateBehaviorData
{
	public string Name = "";
	public int priority = 0;
	/// <summary>
	/// The alternate condition, if matches, switch condition
	/// </summary>
	public CompositeConditionWrapper AlternateCondition = new CompositeConditionWrapper ();
	public string NextBehaviorName = "";
}

/// <summary>
/// Base class of AI Beheavior.
/// A AIBehavior defines a set of data to guide how AI behaves.
/// One AI can contains a couple of AIBehavior. And at one time, only one AIBehavior is functioning. The AI Behavior is switched accordingg to its condition,
/// if start/end condition matches, the AI Behavior should be started / ended.
/// One game object can contains a couple of AI script component, alike AI behavior, AI is switched by Unit class, see SwitchAIData class.
/// </summary>
[System.Serializable]
public class AIBehavior
{
	public string Name = "";
	/// <summary>
	/// Type of the behavior
	/// </summary>
	public AIBehaviorType Type;
	//[HideInInspector]
	public AIBehaviorPhase Phase = AIBehaviorPhase.Sleeping;
	public SelectTargetRule SelectTargetRule = SelectTargetRule.Default;

#region variables for Start and End Condition
	/// <summary>
	/// Scan end condition at every %ScanEndConditionInterval% seconds
	/// </summary>
	public float ScanEndConditionInterval = 0.33333f;
	public CompositeConditionWrapper EndConditionWrapper = new CompositeConditionWrapper ();
	
	
	/// <summary>
	/// The alternate behavior condition array. Sorted by priority.
	/// </summary>
	public AlternateBehaviorData[] alternateBehaviorConditionArray = new AlternateBehaviorData[] { } ;
	
#endregion

#region variables for BehaviorType = Idle
	public string IdleDataName = string.Empty;
#endregion

#region variables for BehaviorType = MoveToTransform/MoveAtDirection/Attack
	/// <summary>
	/// Used when AIBeheaviorType = MoveToTransform/MoveAtDirection, the MoveData name to search the moveData
	/// </summary>
	public string MoveDataName;
    
	/// <summary>
	/// Used when AIBeheaviorType = MoveToTransform/AttackToPosition
	/// </summary>
	public Transform MoveToTarget;
	/// <summary>
	/// Used when AIBehaviorType = MoveAtDirection/AttackToDirection
	/// IsWorldDirection & MoveDirection - is the direction local/global ?
	/// </summary>
	public bool IsWorldDirection = false;
	public Vector3 MoveDirection = Vector3.zero;
#endregion

#region variables for BehaviorType = Attack/AttackToPosition

	/// <summary>
	/// Define the attack data name of this attack behavior.
	/// </summary>
	public string AttackDataName;
	
	/// <summary>
	/// If UseRandomAttackData is true, the attack data is randomly picked in AttackDataNameString.
	/// </summary>
	public bool UseRandomAttackData;
	
	/// <summary>
	/// The attack data name array, which is used when UseRandomAttackData is true.
	/// </summary>
	public string[] AttackDataNameArray = new string[]{};
	/// <summary>
	/// The attack interval minimum and maximum is used when behavior type = Attack and AttackInterrupt = true
	/// During interval, IdleData will be performed.
	/// </summary>
	public bool AttackInterrupt = true;
	public float AttackIntervalMin = 0.5f;
	public float AttackIntervalMax = 1.5f;
#endregion

#region variables for BehaviorType = HoldPosition
	public float HoldRadius = 3.5f;
    
#endregion
	
#region variables for BehaviorType = SwitchToAI
	/// <summary>
	/// The name of the switch to AI.
	/// Randomly pick one AI name, when swtiching
	/// </summary>
	public string[] SwitchToAIName = new string[] {};
#endregion

	/// <summary>
	/// SendMessage() at behavior start/end
	/// </summary>
	public string[] MessageAtStart = new string[]{};
	public string[] MessageAtEnd = new string[]{};

	/// <summary>
	/// Count how many times the behavior has been executed.
	/// </summary>
	[HideInInspector]
	public long ExecutionCounter = 0;
	/// <summary>
	/// The last time of execution the behavior to now.
	/// </summary>
	[HideInInspector]
	public float LastExecutionTime = 0;
	[HideInInspector]
	public float StartTime = 0;
	
	/// <summary>
	/// The name of the next behavior.
	/// This variable is assigned in runtime.
	/// </summary>
	[HideInInspector]
	public string NextBehaviorName = "";
	
	public AIBehavior GetClone ()
	{
		AIBehavior clone = new AIBehavior ();
//		clone.StartConditionWrapper = this.StartConditionWrapper.GetClone();
		clone.EndConditionWrapper = this.EndConditionWrapper.GetClone ();
		clone.Name = this.Name;
		clone.Type = this.Type;
		clone.SelectTargetRule = this.SelectTargetRule;
		clone.IdleDataName = this.IdleDataName;
		clone.MoveDataName = this.MoveDataName;
		clone.MoveToTarget = this.MoveToTarget;
		clone.IsWorldDirection = this.IsWorldDirection;
		clone.MoveDirection = this.MoveDirection;
		clone.AttackDataName = this.AttackDataName;
		clone.UseRandomAttackData = this.UseRandomAttackData;
		clone.AttackDataNameArray = Util.CloneArray<string> (this.AttackDataNameArray);
		clone.HoldRadius = HoldRadius;
		clone.SwitchToAIName = Util.CloneArray<string> (this.SwitchToAIName);
		clone.MessageAtStart = Util.CloneArray<string> (this.MessageAtStart);
		clone.MessageAtEnd = Util.CloneArray<string> (this.MessageAtEnd);
		return clone;
	}
	
	/// <summary>
	/// Ascendent sort AlternateBehaviorData.
	/// </summary>
	int SortAlternateBehaviorData(AlternateBehaviorData x, AlternateBehaviorData y)
	{
		int ret = 0;
		if(x.priority == y.priority)
			ret = 0;//equal
		if(x.priority < y.priority)
			ret = -1;//lesser
		if(x.priority > y.priority)
			ret = 1;//greater
		return ret;
	}
	
	/// <summary>
	/// Descent sort AlternateBehaviorData.
	/// </summary>
	int DescSortAlternateBehaviorData(AlternateBehaviorData x, AlternateBehaviorData y)
	{
		int ret = 0;
		if(x.priority == y.priority)
			ret = 0;//equal
		if(x.priority < y.priority)
			ret = 1;//lesser
		if(x.priority > y.priority)
			ret = -1;//greater
		return ret;
	}
	
	/// <summary>
	/// Inits the AIBehavior variable.
	/// call this method in Awake of AI class.
	/// </summary>
	public void InitBehavior ()
	{
		//sort the alternateBehaviorConditionArray by priority, in descending order.
		List <AlternateBehaviorData> sortedList = new List<AlternateBehaviorData> ();
		
		for (int i=0; i<alternateBehaviorConditionArray.Length; i++) {
			sortedList.Add (alternateBehaviorConditionArray [i]);
			//Initialize the condition wrapper data.
			alternateBehaviorConditionArray[i].AlternateCondition.InitDictionary();
		}
		//Sort priority descendently from higher to lower, so the higher priority always get more chance to be executed.
		sortedList.Sort(DescSortAlternateBehaviorData);
		this.alternateBehaviorConditionArray = sortedList.ToArray();
	}
}
