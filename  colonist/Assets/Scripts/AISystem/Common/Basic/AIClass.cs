using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
	/// <summary>
	/// The priority , 0 = lowest.
	/// </summary>
	public int priority = 0;
	/// <summary>
	/// The alternate condition, if matches, switch condition
	/// </summary>
	public CompositeConditionWrapper AlternateCondition = new CompositeConditionWrapper ();
	public string NextBehaviorName = "";
	
	/// <summary>
	/// Initialize this AlternateBehaviorData.
	/// </summary>
	public void Init()
	{
		AlternateCondition.Init();
	}
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

	/// <summary>
	/// The alternate behavior condition array. Sorted by priority.
	/// </summary>
	public AlternateBehaviorData[] alternateBehaviorConditionArray = new AlternateBehaviorData[] { } ;
	public float AlterBehaviorInterval = 0.3333f;

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
	/// The game event to be sent at behavior start.
	/// For method that do not need parameters, use MessageAtStart/MessageAtEnd is more simpler.
	/// </summary>
	public GameEvent[] GameEventAtStart = new GameEvent[]{};
	/// <summary>
	/// The game event to be sent at behavior end.
	///  For method that do not need parameters, use MessageAtStart/MessageAtEnd is more simpler.
	/// </summary>
	public GameEvent[] GameEventAtEnd = new GameEvent[]{};

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
	
	/// <summary>
	/// The waypoint names. It's used in type = MoveToWaypoint behavior.
	/// </summary>
	public string[] WaypointNames = new string[]{};
	
	/// <summary>
	/// Runtime variables, cache the selected waypoint.
	/// </summary>
	[HideInInspector]
	public WayPoint selectedWaypoint = null;
	
	public AIBehavior GetClone ()
	{
		AIBehavior clone = new AIBehavior ();
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
	/// Inits the AIBehavior variable.
	/// call this method in Awake of AI class.
	/// </summary>
	public void InitBehavior ()
	{
		//sort the alternateBehaviorConditionArray by priority, from lower to higher ( 0 = least priority)
		this.alternateBehaviorConditionArray = this.alternateBehaviorConditionArray.OrderByDescending(x=>x.priority).ToArray();
		foreach(AlternateBehaviorData alterBehaviorData in alternateBehaviorConditionArray)
		{
			alterBehaviorData.Init();
		}
	}
}
