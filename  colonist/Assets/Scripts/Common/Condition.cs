using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// CompositeConditionWrapper wraps a full set of AtomCondition and CompositeCondition together in one class.
/// The nature of CompositeConditionWrapper == CompositeCondition. It's just a TOP-MOST CompositeCondition.
/// </summary>
[System.Serializable]
public class CompositeConditionWrapper
{
	public CompositeCondition[] CompositeConditionArray = new CompositeCondition[] {};
	public IDictionary<string, CompositeCondition> CompositeConditionDict = new Dictionary<string, CompositeCondition>();
	
	public AtomConditionData[] atomConditionDataArray = new AtomConditionData[]{};
	public IDictionary<string, AtomConditionData> AtomConditionDataDict = new Dictionary<string, AtomConditionData>();
	/// <summary>
	/// The root composite condition.
	/// </summary>
	public CompositeCondition RootCompositeCondition = new CompositeCondition();
	
	/// <summary>
	/// initialize the dictionary - put the atomCondition and compositeCondition into corresponding dictionary.
	/// </summary>
	public void Init()
	{
		foreach(CompositeCondition compositeCondition in CompositeConditionArray)
		{
			CompositeConditionDict.Add(compositeCondition.Id, compositeCondition);
		}
		foreach(AtomConditionData atomConditionData in atomConditionDataArray)
		{
			AtomConditionDataDict.Add(atomConditionData.Id, atomConditionData);
		}
	}
	
	public CompositeConditionWrapper GetClone()
	{
		CompositeConditionWrapper clone = new CompositeConditionWrapper();
		clone.RootCompositeCondition = this.RootCompositeCondition.GetClone();
		foreach(CompositeCondition compositeCondition in this.CompositeConditionArray)
		{
			CompositeCondition cloneCompositiobCondition = compositeCondition.GetClone();
			clone.CompositeConditionArray = Util.AddToArray<CompositeCondition>(cloneCompositiobCondition, clone.CompositeConditionArray);
		}
		foreach(AtomConditionData atomCondition in this.atomConditionDataArray)
		{
			AtomConditionData cloneAtomCondition = atomCondition.GetClone();
			clone.atomConditionDataArray = Util.AddToArray<AtomConditionData>(cloneAtomCondition, clone.atomConditionDataArray);
		}
		return clone;
	}
}

public enum ConditionEntityType
{
	AtomCondition,
	ReferenceToComposite,
}

/// <summary>
/// Condition entity.
/// In composite condition, there are at least two entities. One is left , another right, and logical operator join the two.
/// </summary>
/// 
[System.Serializable]
public class ConditionEntity
{
	/// <summary>
	/// The type of the left entity.
	/// When AtomCondition, the EntityReferenceId is the ID of AtomCondition.
	/// When ReferenceToComposite, the EntityReferenceId is the ID of another CompositeCondition.
	/// </summary>
	public ConditionEntityType EntityType = ConditionEntityType.AtomCondition;
	public string EntityReferenceId = "";
}



/// <summary>
/// Wrapper of a composite condition.
/// A condition could be one of the case:
/// 1. One single condition. e.g: Enemy in sign = True
/// 2. Two condition, conjoint of logical operator: 
///   Condition ID = 001 : Condition 002 AND|OR Condition 003 
///   Condition ID = 002 : Enemy in area001 = True)
///   Condition ID = 003 : Enemy in sign = True
/// </summary>
[System.Serializable]
public class CompositeCondition
{
	public string Id = "";
	
	/// <summary>
	/// Logical operator, can be NONE, AND , OR
	/// When NONE, only LeftEntity is concerned.
	/// When AND , OR, join the Left and Right Entity.
	/// </summary>
	public LogicConjunction Operator = LogicConjunction.None;
	
	public ConditionEntity Entity1 = new ConditionEntity();
	
	public ConditionEntity Entity2 = new ConditionEntity();
	
	public CompositeCondition GetClone()
	{
		CompositeCondition clone = new CompositeCondition();
		clone.Id = this.Id;
		clone.Operator = this.Operator;
		clone.Entity1 = new ConditionEntity();
		clone.Entity1.EntityReferenceId = this.Entity1.EntityReferenceId;
		clone.Entity1.EntityType = this.Entity1.EntityType;
		return clone;
	}
	
}



/// <summary>
/// ConditionData_AIBehaviorSwitching describes the condition of starting beheavior, or ending beheavior.
/// Note : the behavior switching is inside AI.
/// </summary>
[System.Serializable]
public class AtomConditionData
{
	/// <summary>
	/// The identifier.
	/// </summary>
	public string Id = "";
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
    public Collider CheckArea;

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
	/// Used in some condition case. for example, when ConditionType = Boolean, and BooleanCondition = LatestBehaviorNameIs
	/// </summary>
	public string StringValue = "";
	
	/// <summary>
	/// The string array.
	/// Used in some condition case. for example, when ConditionType = Boolean, and BooleanCondition = LatestBehaviorNameIsOneOf
	/// </summary>
	public string[] StringValueArray = new string[] {};
	
	/// <summary>
	/// The latest time when the condition is true.
	/// </summary>
	[HideInInspector]
	public float PreviousConditionTrueTime = 0;
	public string GetDescription()
	{
		string ret = "";
		switch(ConditionType)
		{
		case AIBehaviorConditionType.Boolean:
			ret = BooleanCondition.ToString() + " " + BooleanOperator.ToString();
			break;
		case AIBehaviorConditionType.ValueComparision:
			ret = ValueComparisionCondition.ToString() + " " + ValueOperator.ToString() + " " + RightValueForComparision;
			break;
		}
		return ret;
	}
	
	public AtomConditionData GetClone()
	{
		AtomConditionData clone = new AtomConditionData();
		clone.Id = this.Id;
		clone.ConditionType = this.ConditionType;
		clone.BooleanOperator = this.BooleanOperator;
		clone.BooleanCondition = this.BooleanCondition;
		clone.CheckArea = this.CheckArea;
		clone.ValueComparisionCondition = this.ValueComparisionCondition;
		clone.ValueOperator = this.ValueOperator;
		clone.RightValueForComparision = this.RightValueForComparision;
		clone.LayerMaskForComparision = this.LayerMaskForComparision;
		clone.StringValue = this.StringValue;
		clone.StringValueArray = this.StringValueArray;		
		return clone;
	}

}
