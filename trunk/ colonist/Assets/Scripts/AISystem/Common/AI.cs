using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;


/// <summary>
/// Base AI.
/// AI manages the a set of beheavior of the NPC.
/// Note that one game object can contains more than one AI component, however, there
/// is usually one AI running at one time, so if you want to define more than one AI
/// component in game object ,you need to use Character class to coordinate the AI components.
/// </summary>
[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Unit))]
[RequireComponent(typeof(CharacterController))]
public class AI : MonoBehaviour, I_AIBehaviorHandler {
	/// <summary>
	/// The name of this AI.
	/// </summary>
	public string Name = ""; 
	
    [HideInInspector]
    public Unit Unit;
    /// <summary>
    /// The obstacle layer which can blocks attack
    /// </summary>
    public LayerMask AttackObstacle;
    
    /// <summary>
    /// The distance of Unit should start offensing enemy
    /// </summary>
    public float OffensiveRange = 10;

    /// <summary>
    /// The distance of awareness nearby enemy.
    /// DetectiveRange should greater than OffensiveRange.
    /// </summary>
    public float DetectiveRange = 30;

    /// <summary>
    /// The flag to halt an AI. When AI is halted, it can't move, it can't attack, but it can apply damage or die.
    /// true = Halt
    /// false = No halt
    /// </summary>
    public bool Halt 
	{
		get
		{
			return this.Unit.Halt;
		}
	}
	
    /// <summary>
    /// When current time >= ResetHaltTime, AI should reset Halt to false.
    /// </summary>
    [HideInInspector]
	public float ResetHaltTime = 0;

    [HideInInspector]
    public Transform CurrentTarget = null;
    /// <summary>
    /// CanSeeCurrentTarget is refreshed in FixUpdate().FindTarget() routine.
    /// </summary>
    [HideInInspector]
    public bool CanSeeCurrentTarget = false;
    [HideInInspector]
    public float CurrentTargetDistance = 0;
    /// <summary>
    /// LastAttackTime = the time last attacking.
    /// </summary>
    [HideInInspector]
    public float LastAttackTime = -999;
	
	/// <summary>
	/// The flag to indicate if this frame need to alternate AIBehavior, ignore the scanning interval.
	/// Usually, this flag is set true by events.
	/// </summary>
	[HideInInspector]
	public bool AlternateBehaviorFlag = false;
	
    /// <summary>
    /// The current executing behavior
    /// </summary>
    protected AIBehavior CurrentBehavior = null;
    /// <summary>
    /// The previous behavior
    /// </summary>
    //protected AIBehavior PreviousBehavior = null;

    public Transform TestToPos = null;

    public float AlterBehaviorInterval = 0.25f;
    /// <summary>
    /// Define the beheviors of the AI
    /// </summary>
    public AIBehavior[] Behaviors = new AIBehavior[] {};

    /// <summary>
    /// Sort the prefab Behaviors and save in the list from higher to lower priority
    /// </summary>
    protected IList<AIBehavior> BehaviorList_SortedPriority = new List<AIBehavior>();

    protected CharacterController controller;
//    protected Seeker seeker;
	protected Navigator navigator;
	
	/// <summary>
	/// The switch to control print debug message.
	/// </summary>
	public bool PrintDebugMessage = false;
	
    void Awake()
    {
        InitAI();
    }

    void Start()
    {
        StartCoroutine(StartAI());
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        //Refresh CurrentTarget in every Time.fixDeltaTime seconds
        FindTarget(DetectiveRange);
		this.Unit.CurrentTarget = this.CurrentTarget;
    }
	
	void OnEnable()
	{
		Unit.CurrentAI = this;
	}
	
	void OnDisable()
	{
		StopAI();
	}
	
#region initialization
    /// <summary>
    /// 
    /// 1. Initialize variable.
    /// 2. Initalize behavior list. Sort the behavior from higher to lower priorty.
    /// 
    /// Offspring should call InitAI() at Awake() 
    /// </summary>
    public virtual void InitAI()
    {
        this.Unit = GetComponent<Unit>();
        controller = GetComponent<CharacterController>();
		navigator = GetComponent<Navigator>();
        //Put the behavior into a sort list first, which sort the beheavior priority from lower to higher 
        SortedList<int,AIBehavior> tempList = new SortedList<int,AIBehavior>();
        foreach (AIBehavior beheavior in Behaviors)
        {
            tempList.Add(beheavior.Priority,beheavior);
			beheavior.StartConditionWrapper.InitDictionary();
			beheavior.EndConditionWrapper.InitDictionary();
        }
        //Then insert the behavior from higher to lower priority
        for (int i = tempList.Count-1; i>=0; i--)
        {
            BehaviorList_SortedPriority.Add(tempList.Values[i]);
        }        
    }
	
	/// <summary>
	/// 1. Start A* pathfind daemon routines.
	/// 2. Start AlterBehavior daemon routine.
	/// </summary>
    public virtual IEnumerator StartAI()
    {
        StartCoroutine("AlterBehavior", AlterBehaviorInterval);
        yield break;
    }

#endregion

#region basic AI supporting functions
    /// <summary>
    /// Stop AI.
    /// </summary>
    public virtual void StopAI()
    {
		StopAllCoroutines();
    }
	
    /// <summary>
    /// Find Target in given range.
    /// 1. set CurrentTarget variable
    /// 2. set CanSeeCurrentTarget variable
    /// 3. set CurrentTargetDistance variable
    /// Return false if no target is found.
    /// Note: the rule of setting Current Target 
    /// </summary>
    /// <returns></returns>
    public virtual bool FindTarget(float Range)
    {
        Collider[] colliders = null;
        FindEnemyAround(Range, out colliders);
        //If there're some enemy's around, select one of them, by rule
        if (colliders != null && colliders.Length > 0)
        {
            if (colliders.Length == 1)
            {
                CurrentTarget = colliders[0].transform;
                CurrentTargetDistance = Util.DistanceOfCharacters(gameObject, CurrentTarget.gameObject);
                CanSeeCurrentTarget = !Physics.Linecast(transform.position, CurrentTarget.transform.position, AttackObstacle);
                return true;
            }
            else
            {
                GameObject _target = SelectTargetByRule(CurrentBehavior.SelectTargetRule, colliders);
                CurrentTarget = _target.transform;
                CurrentTargetDistance = Util.DistanceOfCharacters(gameObject, CurrentTarget.gameObject);
                CanSeeCurrentTarget = !Physics.Linecast(transform.position, CurrentTarget.transform.position, AttackObstacle);
                return true;
            }
        }
        //if no enemy's around, or the current target is dead, reset the current target vaiables
        else if (CurrentTarget == null || CurrentTarget.GetComponent<UnitHealth>().GetCurrentHP() <= 0)
        {
            CurrentTarget = null;
            CurrentTargetDistance = 0;
            CanSeeCurrentTarget = false;
            return false;
        }
        else 
        {
            //Don't touch CurrentTarget, just set CurrentTargetDistance & CanSeeCurrentTarget
            if (CurrentTarget != null)
            {
                CurrentTargetDistance = Util.DistanceOfCharacters(gameObject, CurrentTarget.gameObject);
                CanSeeCurrentTarget = !Physics.Linecast(transform.position, CurrentTarget.transform.position, AttackObstacle);
                return true;
            }
            else
            {
                CurrentTargetDistance = 0;
                CanSeeCurrentTarget = false;
                return true;
            }
        }
    }

    /// <summary>
    /// Select target by rules, if no target can be selected by the given rule,
    /// default rule is applied, default rule = always select closest enemy.
    /// </summary>
    /// <param name="rule"></param>
    /// <param name="Colliders"></param>
    /// <returns></returns>
    public virtual GameObject SelectTargetByRule(SelectTargetRule rule, Collider[] Colliders)
    {
        GameObject _target = null;
        switch (rule)
        {
            case SelectTargetRule.Farest:
                _target = Util.findFarest(transform.position, Colliders).gameObject;
                break;
            case SelectTargetRule.Default:
            case SelectTargetRule.Closest:
            default:
                _target = Util.findClosest(transform.position, Colliders).gameObject;
                break;
        }
        return _target;
    }

    /// <summary>
    /// Find cloest enemy around in %range%. Output if exists.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public virtual bool FindClosestEnemyAround(float range, out Transform enemy)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, Unit.EnemyLayer);
        if (colliders != null && colliders.Length > 0)
        {
            enemy = Util.findClosest(transform.position, colliders).transform;
            return true;
        }
        else
        {
            enemy = null;
            return false;
        }
    }

    /// <summary>
    /// Find farest enemy around in %range%. Output if exists.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public virtual bool FindFarestEnemyAround(float range, out Transform enemy)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, Unit.EnemyLayer);
        if (colliders != null && colliders.Length > 0)
        {
            enemy = Util.findFarest(transform.position, colliders).transform;
            return true;
        }
        else
        {
            enemy = null;
            return false;
        }
    }

    /// <summary>
    /// Find if enemy exists in given range
    /// return true if exists.
    /// Differs to FindTarget, FindEnemyAround would not set CurrentTarget variable
    /// </summary>
    /// <returns></returns>
    public virtual bool FindEnemyAround(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, Unit.EnemyLayer);
        return colliders != null && colliders.Length > 0;
    }

    /// <summary>
    /// Find if enemy exists in given range
    /// return true if exists.
    /// Output enemyCollider.
    /// Differs to FindTarget, FindEnemyAround would not set CurrentTarget variable
    /// </summary>
    /// <returns></returns>
    public virtual bool FindEnemyAround(float range, out Collider[] enemyCollider)
    {
        enemyCollider = Physics.OverlapSphere(transform.position, range, Unit.EnemyLayer);
        return enemyCollider != null && enemyCollider.Length > 0;
    }

    public virtual void ResetTarget()
    {
        CurrentTarget = null;
    }

    public virtual void ResetTarget(int layer)
    {
        if (CurrentTarget != null && CurrentTarget.gameObject.layer == layer)
            CurrentTarget = null;
    }
    /// <summary>
    /// Return false if:
    /// 1. Distance between AI and Target > MaxRange
    /// Or
    /// 2. Colliders exists between AI and Target.
    /// Performance issue - avoid calling this method per frame, as it depends on Physics.Linecast()!!
    /// </summary>
    /// <param name="Target"></param>
    /// <param name="MaxRange"></param>
    /// <returns></returns>
    public virtual bool CanSeeTarget(GameObject Target, float MaxRange)
    {
        if (Target==null || Util.DistanceOfCharacters(gameObject, Target) > MaxRange)
        {
            return false;
        }
        if (Target==null || Physics.Linecast(transform.position, Target.transform.position, AttackObstacle))
        {
            return false;
        }
        return true;
    }

    public virtual bool CanSeeTarget(GameObject Target, float MaxRange, out float Distance)
    {
        if ((Distance = Util.DistanceOfCharacters(gameObject, Target.gameObject)) > MaxRange)
        {
            return false;
        }
        if (Physics.Linecast(transform.position, CurrentTarget.position, AttackObstacle))
        {
            return false;
        }
        return true;
    }
	
	/// <summary>
	/// Helper functions - navigating AI to transform.
	/// The function returns when distance to target transform lesser than BreakDistance
	/// </summary>
	public virtual IEnumerator NavigateToTransform(Transform TargetTransform, 
		                               MoveData moveData, 
		                               float BreakDistance)
	{
		float _lastNavigationTime = Time.time;
		StartNavigation(CurrentTarget, true, moveData);
		while (TargetTransform != null &&
			Vector3.Distance(transform.position,TargetTransform.position) > BreakDistance)
		{
			//we need to refresh the navigation path in a fixed time
             if ((Time.time - _lastNavigationTime) >= moveData.RedirectTargetInterval)
             {
                 StartNavigation(TargetTransform, true, moveData);
                 _lastNavigationTime = Time.time;
             }
             yield return null;
        }
        StopNavigation();
	}
	
	/// <summary>
	/// Fallback in the specified time.
	/// target is optional, if target is null, fallback direction is back direction in local space.
	/// </summary>
	public IEnumerator Fallback(float timelength, MoveData movedata, Transform target)
	{
		float time = Time.time;
		animation.CrossFade(movedata.AnimationName);
		while((Time.time - time ) <= timelength)
		{
			if(target != null)
			{
				Util.RotateToward(transform,target.position, movedata.SmoothRotate, movedata.RotateAngularSpeed);
			}
			controller.SimpleMove(transform.TransformDirection(Vector3.back) * movedata.MoveSpeed);
			yield return null;
		}
	}
	
#endregion

#region Navigation
	
    /// <summary>
    /// Call this routine to start navigation.
    /// Target - the target navigate to
    /// IsMovingTarget - Is the target a static target? If yes, the path wouldn't be refreshed during navigation
    /// MoveData - the moving data(animations, speed)
    /// </summary>
    /// <param name="target"></param>
    /// <param name="IsMovingTarget"></param>
    public void StartNavigation(Transform target, bool IsMovingTarget, MoveData MoveData)
    {
		navigator.StartNavigation(target, IsMovingTarget, MoveData);
    }

    /// <summary>
    /// Call this routine to stop navigation immediately.
    /// </summary>
    public void StopNavigation()
    {
        navigator.StopNavigation();
    }

#endregion

#region AI Behavior determination

    /// <summary>
    /// ALter current behavior at every %ScanBehaviorInterval% seconds
    /// </summary>
    public virtual IEnumerator AlterBehavior(float Interval)
    {
		float lastScanEndConditionTime = 0;
        while (true)
        {
			if(this.Halt)
			{
				yield return null;
				continue;
			}
			if(((Time.time - lastScanEndConditionTime) < Interval) && AlternateBehaviorFlag == false)
			{
				yield return null;
				continue;
			}
			
			if(AlternateBehaviorFlag)
			{
				AlternateBehaviorFlag = false;
			}
            //if the current behavior is still running, check if it meets end condition.
			// if the current behavior status is not running, it means it can stop at anytime
            if (CurrentBehavior != null && CurrentBehavior.Phase == AIBehaviorPhase.Running)
            {
				lastScanEndConditionTime = Time.time;
                //If CurrentBehavior's end condition = false, do nothing
				if(CheckConditionWrapper(CurrentBehavior.EndConditionWrapper, CurrentBehavior) == false)
                {
					if(PrintDebugMessage)
					{
                       Debug.Log("Behavior:" + CurrentBehavior.Name + " do not meet end condition, not ended yet.");
					}
                    yield return null;
                    continue;
                }
            }
            AIBehavior behaviorToGo = null;
            //Choose next behavior whose StartCondition = True
            foreach (AIBehavior behavior in BehaviorList_SortedPriority)
            {
                if(behavior.Name=="SwitchToAttack") 
					Debug.DebugBreak();
				if(CheckConditionWrapper(behavior.StartConditionWrapper, behavior))
                {
                    behaviorToGo = behavior;
                    break;
                }
            }
            //if no behavior meets start condition, let it be, print a message and wait for next loop
            if (behaviorToGo == null)
            {
                Debug.LogWarning("No behavior can start - " + gameObject.name);
				yield return null;
                continue;
            }
            //Good, there is a behavior can be started - behaviorToGo
            else
            {
                //Do nothing if the new behavior is already running
                if (CurrentBehavior == behaviorToGo || behaviorToGo.Phase == AIBehaviorPhase.Running)
                {
					if(PrintDebugMessage)
                       Debug.Log("Behavior : " + behaviorToGo.Name + " is already running! No need to start again.");
                }
                else
                {
                    if (CurrentBehavior != null)
                    {
                        this.StopBehavior(CurrentBehavior);
						Debug.Log("Stop behavior:" + CurrentBehavior.Name + " and start behavior:" + behaviorToGo.Name);
                        //Wait one frame to let StopBehavior complete.
                        yield return null;
                    }
                    StartBehavior(behaviorToGo);
                    CurrentBehavior = behaviorToGo;
                }
                //yield return new WaitForSeconds(Interval);
				yield return null;
            }
        }
    }
	
	public virtual bool CheckConditionWrapper(CompositeConditionWrapper compositeConditionWrapper,AIBehavior behavior)
	{
		bool ret = false;
	    ret = IsCompositeConditionMatched(compositeConditionWrapper.RootCompositeCondition, 
			                              compositeConditionWrapper, 
			                              behavior);
		return ret;
	}
	
	public virtual bool IsCompositeConditionMatched(CompositeCondition compositeCondition,
		                                            CompositeConditionWrapper compositeConditionWrapper,
		                                            AIBehavior behavior)
	{
		bool ret = false;
		switch(compositeCondition.Operator)
		{
		case LogicConjunction.None:
			ret = IsConditionEntityMatched(compositeCondition.Entity1,compositeConditionWrapper,behavior);
			break;
		case LogicConjunction.And:
			ret = IsConditionEntityMatched(compositeCondition.Entity1,compositeConditionWrapper,behavior);
			if(ret)
				ret = IsConditionEntityMatched(compositeCondition.Entity2,compositeConditionWrapper,behavior);
			break;
		case LogicConjunction.Or:
			ret = IsConditionEntityMatched(compositeCondition.Entity2,compositeConditionWrapper,behavior); 
			if(ret == false)
				ret = IsConditionEntityMatched(compositeCondition.Entity2,compositeConditionWrapper,behavior);
			break;
		}
		return ret;
	}
	
	private bool IsConditionEntityMatched(ConditionEntity conditionEntity, 
		                                  CompositeConditionWrapper compositeConditionWrapper,
		                                  AIBehavior behavior)
	{
		bool ret = false;
		switch(conditionEntity.EntityType)
		{
		case ConditionEntityType.AtomCondition:
			AtomConditionData atomCondition = compositeConditionWrapper.AtomConditionDataDict[conditionEntity.EntityReferenceId];
			ret = CheckAtomCondition(atomCondition, behavior);
			break;
		case ConditionEntityType.ReferenceToComposite:
			CompositeCondition compositeCondition = compositeConditionWrapper.CompositeConditionDict[conditionEntity.EntityReferenceId];
			ret = IsCompositeConditionMatched(compositeCondition, compositeConditionWrapper, behavior);
			break;
		}
		return ret;
	}

    /// <summary>
    /// Check if the AIBehavior condition is true
    /// </summary>
    /// <param name="AIBehaviorCondition"></param>
    /// <returns></returns>
    public virtual bool CheckAtomCondition(AtomConditionData AIBehaviorCondition, AIBehavior behavior)
    {
        bool ret = false;
        switch (AIBehaviorCondition.ConditionType)
        {
            case AIBehaviorConditionType.Boolean:
                ret = CheckBooleanCondition(AIBehaviorCondition, behavior);
                break;
            case AIBehaviorConditionType.ValueComparision:
                ret = CheckValueComparisionCondition(AIBehaviorCondition, behavior);
                break;
        }
        return ret;
    }

    /// <summary>
    /// Default check boolean condition routine.
    /// Offspring class should override this method to check custom boolean condition.
    /// </summary>
    /// <param name="AIBehaviorCondition"></param>
    /// <returns></returns>
    public virtual bool CheckBooleanCondition(AtomConditionData AIBehaviorCondition, AIBehavior behavior)
    {
        bool ret = false;
        bool LeftValue = false;
        switch (AIBehaviorCondition.BooleanCondition)
        {
            case AIBooleanConditionEnum.AlwaysTrue:
                LeftValue = true;
                break;
            //See if there's any enemy in offensive range?
            case AIBooleanConditionEnum.EnemyInOffensiveRange:
                LeftValue = FindEnemyAround(this.OffensiveRange);
                break;
            //See if there's any enemy in detective range?
            case AIBooleanConditionEnum.EnemyInDetectiveRange:
                LeftValue = FindEnemyAround(this.DetectiveRange);
                break;
            //See if current transform located within the predefined area?
            case AIBooleanConditionEnum.InArea:
                LeftValue = Util.IsTransformInsideBounds(transform, AIBehaviorCondition.CheckAreaes);
                break;
            //See if the CurrentTarget's gameObject layer within the layermask
            case AIBooleanConditionEnum.CurrentTargetInLayer:
                LeftValue = (CurrentTarget == null && FindTarget(OffensiveRange) == false) ?
                    false :
                    Util.CheckLayerWithinMask(CurrentTarget.gameObject.layer, AIBehaviorCondition.LayerMaskForComparision);
                break;
			
		    case AIBooleanConditionEnum.LatestBehaviorNameIs:
			    if(this.CurrentBehavior == null)
				   LeftValue = false;
			    else 
			       LeftValue = (AIBehaviorCondition.StringValue == this.CurrentBehavior.Name);
			    break;
		case AIBooleanConditionEnum.LastestBehaviorNameIsOneOf:
			if(this.CurrentBehavior == null)
			{
				LeftValue = false;
			}
			else {
			    LeftValue = Util.ArrayContains<string>(AIBehaviorCondition.StringValueArray, this.CurrentBehavior.Name);
			}
			break;
            default:
                Debug.LogError("GameObject:" + this.gameObject.name + " - Unsupported boolean condition:" + AIBehaviorCondition.BooleanCondition.ToString());
                break;
        }
        ret = AIBehaviorCondition.BooleanOperator == BooleanComparisionOperator.IsTrue ? LeftValue
            : !LeftValue;
        return ret;
    }

    /// <summary>
    /// Default check value comparision condition routine.
    /// Offspring class should override this method to check custom value comparision condition.
    /// </summary>
    /// <param name="AIBehaviorCondition"></param>
    /// <returns></returns>
    public virtual bool CheckValueComparisionCondition(AtomConditionData AIBehaviorCondition, AIBehavior behavior)
    {
        bool ret = false;
        float LeftValue = 0;
        float RightValue = AIBehaviorCondition.RightValueForComparision;
        bool ShouldCompare = false;
        switch (AIBehaviorCondition.ValueComparisionCondition)
        {
            //Check if any enemy closer than AIBehaviorCondition.RightValueForComparision
            case AIValueComparisionCondition.NearestEnemyDistance:
                Transform NearestEnemy = null;
                ShouldCompare = FindClosestEnemyAround(this.DetectiveRange, out NearestEnemy);
                LeftValue = ShouldCompare ? Util.DistanceOfCharacters(gameObject, NearestEnemy.gameObject) : 0;
                break;
            //Check if any enemy farer than AIBehaviorCondition.RightValueForComparision
            case AIValueComparisionCondition.FarestEnemyDistance:
                Transform FarestEnemy = null;
                ShouldCompare = FindClosestEnemyAround(this.DetectiveRange, out FarestEnemy);
                LeftValue = ShouldCompare ? Util.DistanceOfCharacters(gameObject, FarestEnemy.gameObject) : 0;
                break;
            case AIValueComparisionCondition.HPPercentage:
                ShouldCompare = true;
                LeftValue = Unit.HP / Unit.MaxHP;
                break;
            case AIValueComparisionCondition.CurrentTargetHPPercentage:
                ShouldCompare = CurrentTarget != null || FindTarget(this.DetectiveRange);
                LeftValue = ShouldCompare ? CurrentTarget.GetComponent<UnitHealth>().GetCurrentHP() / CurrentTarget.GetComponent<UnitHealth>().GetMaxHP()
                                          : 0;
			    if (PrintDebugMessage)
                    Debug.Log("Left value:" + LeftValue + " rightValue:" + RightValue);
                break;
            case AIValueComparisionCondition.BehaviorLastExecutionInterval:
			    ShouldCompare = true;
                float LastExecutionTimeInterval = Time.time - behavior.LastExecutionTime;
                LeftValue = LastExecutionTimeInterval;
                break;
            case AIValueComparisionCondition.CurrentTagetDistance:
                ShouldCompare = (CurrentTarget != null || FindTarget(this.DetectiveRange));
                LeftValue = ShouldCompare ? Util.DistanceOfCharacters(gameObject, CurrentTarget.gameObject)
                    : 0;
                break;
            case AIValueComparisionCondition.ExeuctionCount:
                ShouldCompare = true;
                LeftValue = behavior.ExecutionCounter;
                break;
            case AIValueComparisionCondition.RandomValue:
                ShouldCompare = true;
                Random.seed = System.DateTime.Now.Millisecond;
                LeftValue = Random.Range(0, 100);
                break;
		    case AIValueComparisionCondition.BehaveTime:
                ShouldCompare = true;
                LeftValue = Time.time - behavior.StartTime;
                break;
		    case AIValueComparisionCondition.AttackCount:
			    ShouldCompare = true;
			    LeftValue = this.Unit.AttackCounter;
			    break;
		case AIValueComparisionCondition.DoDamageCount:
			    ShouldCompare = true;
			    LeftValue = this.Unit.DoDamageCounter;
			    break;
            default:
                Debug.LogError("GameObject:" + this.gameObject.name + " - unsupported value comparision condition:" + AIBehaviorCondition.ValueComparisionCondition.ToString());
                break;
        }
        if (ShouldCompare)
        {
            ret = Util.CompareValue(LeftValue, AIBehaviorCondition.ValueOperator, RightValue);
        }
        return ret;
    }

#endregion

#region AI Behavior execution
    /// <summary>
    /// Execute a behavior.
    /// By default, the coroutine to start is the same name to behavior.Type.ToString(), offspring can
    /// override to define a new rule.
    /// </summary>
    /// <param name="behavior"></param>
    /// <returns></returns>
    public virtual void StartBehavior(AIBehavior behavior)
    {
        //Rule: behavior start coroutine = "Start_" + BehaviorType
        string Coroutine = "Start_" + behavior.Type.ToString();
        behavior.ExecutionCounter++;
		//preserve the start time
		behavior.StartTime = Time.time;
		
        //By defaut, set the behavior phase = running at StartBehavior()
        behavior.Phase = AIBehaviorPhase.Running;
        foreach (string startMessage in behavior.MessageAtStart)
        {
            SendMessage(startMessage);
        }
        StartCoroutine(Coroutine, behavior);
    }
    /// <summary>
    /// Stop a behavior.
    /// By default, the coroutine to stop = "Stop_" + behavior.Type.ToString(), offspring can
    /// override to define a new rule.
    /// </summary>
    /// <param name="behavior"></param>
    /// <returns></returns>
    /// 
    public virtual void StopBehavior(AIBehavior behavior)
    {
	    //preserve the last start time
        behavior.LastExecutionTime = Time.time;
        string Coroutine = "Stop_" + behavior.Type.ToString();
        //By defaut, set the behavior phase = sleeping at StopBehavior()
        behavior.Phase = AIBehaviorPhase.Sleeping;
        foreach (string endMessage in behavior.MessageAtEnd)
        {
            SendMessage(endMessage);
        }
        StartCoroutine(Coroutine, behavior);
    }

    /// <summary>
    /// Start Idle. 
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Start_Idle(AIBehavior behavior)
    {
		if(PrintDebugMessage)
           Debug.Log("Start behavior:" + behavior.Name + " .IdleDataName:" + behavior.IdleDataName);
		
        IdleData _IdleData = Unit.IdleDataDict[behavior.IdleDataName];
		
        string IdleAnimationName = _IdleData.AnimationName;
        
        while (true)
        {
            if (Halt)
            {
                animation.Stop(IdleAnimationName);
                yield return null;
                continue;
            }
            else
            {
                animation.CrossFade(IdleAnimationName);
				if(CurrentTarget != null && _IdleData.KeepFacingTarget)
				{
					Vector3 LookAtPosition = new Vector3(CurrentTarget.position.x, transform.position.y, CurrentTarget.position.z);
					if(_IdleData.SmoothRotate)
					{
						RotateData rotateData = Unit.RotateDataDict[_IdleData.RotateDataName];
						//Calculate angle distance of forward direction and face to target direction.
					    Vector3 toTargetDir = CurrentTarget.position - transform.position;
						Vector3 faceDir = transform.forward;
						if(Vector3.Angle(toTargetDir, faceDir) >= rotateData.AngleDistanceToStartRotate)
						{
						   Util.RotateToward(transform, LookAtPosition, true, rotateData.RotateAngularSpeed);
						}
					}
					else 
					{
						transform.LookAt(LookAtPosition);
					}
				}
                yield return null;
            }
        }
    }

    /// <summary>
    /// Stop Idle. 
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Stop_Idle(AIBehavior behavior)
    {
        Debug.Log("Stop behavior:" + behavior.Name + " .IdleDataName:" + behavior.IdleDataName);
        StopCoroutine("Start_Idle");
        string IdleAnimationName = Unit.IdleDataDict[behavior.IdleDataName].AnimationName;
        animation.Stop(IdleAnimationName);
        yield return null;
    }

    /// <summary>
    /// Start behave move to.transform target.
    /// The behavior is terminated by itself, if AI reaches enough close to the target transform.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Start_MoveToTransform(AIBehavior behavior)
    {
        MoveData MoveData = Unit.MoveDataDict[behavior.MoveDataName];
        StartNavigation(behavior.MoveToTarget, true, MoveData);
        while (true)
        {
            if (Halt)
            {
                yield return null;
                continue;
            }
            float distance = Util.DistanceOfCharacters(gameObject, behavior.MoveToTarget.gameObject);
            if (distance <= 1)
            {
                break;
            }
            yield return new WaitForSeconds(0.3333f);
        }
        StopBehavior(behavior);
    }

    /// <summary>
    /// Stop behave stopping move to.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Stop_MoveToTransform(AIBehavior behavior)
    {
        StopNavigation();
        MoveData MoveData = Unit.MoveDataDict[behavior.MoveDataName];
        animation.Stop(MoveData.AnimationName);
        StopCoroutine("Start_MoveToTransform");
        yield return null;
    }
    /// <summary>
    /// Start behave move at direction.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Start_MoveAtDirection(AIBehavior behavior)
    {
        Vector3 direction = behavior.IsWorldDirection ? behavior.MoveDirection : transform.InverseTransformDirection(behavior.MoveDirection);
        MoveData MoveData = Unit.MoveDataDict[behavior.MoveDataName];
        Vector3 velocity = direction.normalized * MoveData.MoveSpeed;
        while (true)
        {
            if (Halt)
            {
                yield return null;
                continue;
            }
            controller.SimpleMove(velocity);
            animation.CrossFade(MoveData.AnimationName);
            yield return null;
        }
    }
	
	public virtual IEnumerator Start_MoveToCurrentTarget(AIBehavior behavior)
	{
		if(Unit.MoveDataDict.ContainsKey(behavior.MoveDataName)==false)
		{
			Debug.LogError("Error key:" + behavior.MoveDataName + " at frame:" + Time.frameCount);
		}
		MoveData MoveData = Unit.MoveDataDict[behavior.MoveDataName];
        float refreshNavigationInterval = 0.3333f;
		float lastNavigationTime = 0;
        while (true)
        {
            if ((Halt) || (CurrentTarget == null))
            {
                yield return null;
                continue;
            }
			if((Time.time - lastNavigationTime)>=refreshNavigationInterval)
			{
			   animation.CrossFade( MoveData.AnimationName);
			   StartNavigation(CurrentTarget, true, MoveData);
			   lastNavigationTime = Time.time;
			}
			
            float distance = Util.DistanceOfCharacters(gameObject, CurrentTarget.gameObject);
            if (distance <= 1)
            {
                break;
            }
            yield return new WaitForSeconds(0.3333f);
        }
        StopBehavior(behavior); 
	}
	
	public virtual IEnumerator Stop_MoveToCurrentTarget(AIBehavior behavior)
	{
        StopNavigation();
        MoveData MoveData = Unit.MoveDataDict[behavior.MoveDataName];
        //animation.Stop(MoveData.AnimationName);
        StopCoroutine("Start_MoveToCurrentTarget");
        yield return null;
	}
	
    /// <summary>
    /// Stop behave move at direction.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Stop_MoveAtDirection(AIBehavior behavior)
    {
        MoveData MoveData = Unit.MoveDataDict[behavior.MoveDataName];
        animation.Stop(MoveData.AnimationName);
        StopCoroutine("Start_MoveAtDirection");
        yield break;
    }

    /// <summary>
    /// Start behave attack
    /// </summary>
    /// <param name="behavior"></param>
    /// <returns></returns>
    public virtual IEnumerator Start_Attack(AIBehavior behavior)
    {
		AttackData attackData = null;
        while (true)
        {
            //If no target is found, do nothing
            if (Halt || CurrentTarget == null)
            {
                yield return null;
                continue;
            }
            //Get attack data
			if(behavior.UseRandomAttackData == false)
			{
			   attackData = Unit.AttackDataDict[behavior.AttackDataName];
			}
			else 
			{
		       string attackDataName = Util.RandomFromArray<string>(behavior.AttackDataNameArray);
			   attackData = Unit.AttackDataDict[attackDataName];
			}
			//Animating attack
            string AttackAnimationName = attackData.AnimationName;
            //If can see target, and target distance <= AttackableRange, do this:
            //1. Face to target
            //2. Check last attack time against attack interval
            //3. if #2 pass, animating, send hit message
            if (this.CanSeeCurrentTarget && CurrentTargetDistance <= attackData.AttackableRange)
            {
				
                transform.LookAt(new Vector3(CurrentTarget.position.x, transform.position.y, CurrentTarget.position.z));
				//If hitTriggerType is HitTriggerType.ByAnimationEvent, the function will be invoked by animation event
				if(attackData.hitTriggerType == HitTriggerType.ByTime)
				{
				   SendMessage("_Attack",attackData.Name);
				}
				animation.CrossFade(attackData.AnimationName);
				yield return new WaitForSeconds(animation[attackData.AnimationName].length);
				//After attack animation complete, set this flag AlternateBehaviorFlag to true, to enable altering to higher priority behavior.
				AlternateBehaviorFlag = true;
				yield return null;
				if(behavior.AttackInterrupt)
				{
					IdleData idleData = this.Unit.IdleDataDict[behavior.IdleDataName];
					float interval = Random.Range(behavior.AttackIntervalMin, behavior.AttackIntervalMax);
					animation.CrossFade(idleData.AnimationName,0.5f);
					yield return new WaitForSeconds(interval);
				}
				else 
				{
				    yield return null;
				}
				continue;
            }
            //else if can't see target, navigating until CanSeeCurrentTarget = true & within AttackableRange
            else
            {
                MoveData moveData = Unit.MoveDataDict[behavior.MoveDataName];
				yield return StartCoroutine(NavigateToTransform(CurrentTarget,
					moveData, attackData.AttackableRange));
                continue;
            }
            yield return null;
        }
    }

    public virtual IEnumerator CreateProjectile(AttackData attackData,float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        GameObject projectile = (GameObject)Object.Instantiate(attackData.Projectile.gameObject, attackData.ProjectileInstantiateAnchor.position, attackData.ProjectileInstantiateAnchor.rotation);
        projectile.GetComponent<Projectile>().Src = gameObject;
        projectile.GetComponent<Projectile>().Target = CurrentTarget.gameObject;
        projectile.GetComponent<Projectile>().DamageParameter = attackData.GetDamageParameter(gameObject);
    }

    public virtual IEnumerator Stop_Attack(AIBehavior behavior)
    {
//        AttackData attackData = Unit.AttackDataDict[behavior.AttackDataName];
//        MoveData moveData = Unit.MoveDataDict[behavior.MoveDataName];
//        animation.Stop(moveData.AnimationName);
//        animation.Stop(attackData.AnimationName);
		
        StopNavigation();
        StopCoroutine("Start_Attack");
        yield return null;
    }

    public virtual IEnumerator Start_HoldPosition(AIBehavior behavior)
    {
        Vector3 HoldPos = transform.position;
        while (true)
        {
            if (Vector3.Distance(transform.position, HoldPos) > behavior.HoldRadius)
            {

            }
            yield return null;
        }
    }

    public virtual IEnumerator Stop_HoldPosition(AIBehavior behavior)
    {
        StopCoroutine("Start_HoldPosition");
        yield return null;
    }
	
	public virtual void Start_SwitchToAI(AIBehavior behavior)
	{
		string NextAIName = Util.RandomFromArray(behavior.SwitchToAIName);
		AI nextAI = this.Unit.AIDict[NextAIName];
		//switch to next AI.
		this.enabled = false;
		nextAI.enabled = true;
	}
	
#endregion

    public void CloneTo(AI _ai)
	{
		_ai.Name = this.Name;
		_ai.AlterBehaviorInterval = this.AlterBehaviorInterval;
		_ai.AttackObstacle = this.AttackObstacle;
		_ai.OffensiveRange = this.OffensiveRange;
		_ai.DetectiveRange = this.DetectiveRange;
		foreach(AIBehavior behavior in this.Behaviors)
		{
 			AIBehavior clone = behavior.GetClone(); 
			_ai.Behaviors = Util.AddToArray<AIBehavior>(clone, _ai.Behaviors);
		}
	}
	
	
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, OffensiveRange);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, DetectiveRange);

    }
}
