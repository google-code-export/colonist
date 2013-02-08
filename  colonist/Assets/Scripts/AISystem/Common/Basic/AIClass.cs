using UnityEngine;
using System.Collections;

/// <summary>
/// BeheaviorCondition describes the condition of starting beheavior, or ending beheavior
/// </summary>
[System.Serializable]
public class ConditionData
{
    /// <summary>
    /// The type of the condition:
    /// Boolean - LeftValue Is/Is Not True ?
    /// ValueComparision - LeftValue eq/ge/gt/le/lt RightValue
    /// And - conjunct Condition1 and Condition2
    /// Or - conjunct Condition1 Or Condition2
    /// </summary>
    public AIBehaviorConditionType ConditionType;
    
    /// <summary>
    /// Used when ConditionType = Boolean
    /// </summary>
    public AIBooleanConditionEnum BooleanCondition;
    public BooleanComparisionOperator BooleanOperator;
    /// <summary>
    /// Used only when ConditionType = InArea
    /// </summary>
    public Collider[] CheckAreaes;

    /// <summary>
    /// Used when ConditionType = ValueComparision
    /// </summary>
    public AIValueComparisionCondition ValueComparisionCondition;
    public ValueComparisionOperator ValueOperator;
    public float RightValueForComparision;
    /// <summary>
    /// Used only when ConditionType = Boolean, and BooleanCondition = CurrentTargetInLayer
    /// </summary>
    public LayerMask LayerMaskForComparision;
	
	/// <summary>
	/// The string value.
	/// Used when ConditionType = Boolean, and BooleanCondition = LatestBehaviorName
	/// </summary>
	public string StringValue = "";
}

[System.Serializable]
public class AIBehaviorCondition
{
    public ConditionData ConditionData1 = new ConditionData();
    public LogicConjunction Conjunction = LogicConjunction.None;
    public ConditionData ConditionData2 = new ConditionData();
}

/// <summary>
/// Base class of AI Beheavior
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
    /// <summary>
    /// beheavior start condition
    /// </summary>
    public AIBehaviorCondition StartCondition = new AIBehaviorCondition();
    /// <summary>
    /// beheavior end condition
    /// </summary>
    public AIBehaviorCondition EndCondition = new AIBehaviorCondition();
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
    /// Define the attack data name of this attack behavior
    /// </summary>
    public string AttackDataName;
#endregion

#region variables for BehaviorType = HoldPosition
    public float HoldRadius = 3.5f;
    
#endregion

    /// <summary>
    /// SendMessage() at behavior start/end
    /// </summary>
    public string[] MessageAtStart;
    public string[] MessageAtEnd;

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
