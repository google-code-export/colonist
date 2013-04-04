using UnityEngine;
using System.Collections;



[System.Serializable]
public class AIBehaviorCondition
{
    public AtomConditionData ConditionData1 = new AtomConditionData();
    public LogicConjunction Conjunction = LogicConjunction.None;
    public AtomConditionData ConditionData2 = new AtomConditionData();
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
    public string Name="";
    /// <summary>
    /// Type of the behavior
    /// </summary>
    public AIBehaviorType Type;
    //[HideInInspector]
    public AIBehaviorPhase Phase = AIBehaviorPhase.Sleeping;

    /// <summary>
    /// When two beheavior simultaneity matches condition, higher priority get executed first.
    /// </summary>
    public int Priority = 0;
	
    public SelectTargetRule SelectTargetRule = SelectTargetRule.Default;

#region variables for Start and End Condition
	
	public CompositeConditionWrapper StartConditionWrapper = new CompositeConditionWrapper();
	public CompositeConditionWrapper EndConditionWrapper = new CompositeConditionWrapper();
	
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
    public string[] MessageAtEnd =  new string[]{};

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
}


/// <summary>
/// Condition data_of AI switching.
/// Note: this class defines how should different AI switch between each other, which is different to ConditionData_AIBehaviorSwitching.
/// </summary>
[System.Serializable]
public class ConditionData_AISwitching
{
	
}

/// <summary>
/// Switch AI data.
/// The class wrap the switch AI data:
/// 1. Switch from AI to AI.
/// 2. Condition of switching.
/// 3. 
/// </summary>
//[System.Serializable]
//public class SwitchAIData
//{
//	/// <summary>
//	/// The name of this switch AI data.
//	/// </summary>
//	public string Name = "";
//	
//	public ConditionData_AIBehaviorSwitching StartConditionData = new ConditionData_AIBehaviorSwitching();
//}
