using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;


/// <summary>
/// Base AI.
/// AI manages the beheavior of the NPC.
/// </summary>
[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Unit))]
[RequireComponent(typeof(CharacterController))]
public class AI : MonoBehaviour, I_AIBehaviorHandler {
	/// <summary>
	/// The name of this AI.
	/// </summary>
	public string Name; 
	
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
    [HideInInspector]
    public bool Halt = false;
    /// <summary>
    /// When current time >= ResetHaltTime, AI should reset Halt to false.
    /// </summary>
    public float ResetHaltTime = 0;

    [HideInInspector]
    public Transform CurrentTarget = null;
    /// <summary>
    /// canSeeTarget & refreshCanSeeTargetInterval - defines how long the daemon routine refresh the canSeeTarget variable.
    /// TargetDistance = Distance between AI and target.
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
    /// The current executing behavior
    /// </summary>
    protected AIBehavior CurrentBehavior = null;
    /// <summary>
    /// The previous behavior
    /// </summary>
    //protected AIBehavior PreviousBehavior = null;

    public Transform TestToPos = null;

    public float AlterBehaviorInterval = 1;
    /// <summary>
    /// Define the beheviors of the AI
    /// </summary>
    public AIBehavior[] Behaviors = new AIBehavior[] {};

    /// <summary>
    /// Sort the prefab Behaviors and save in the list from higher to lower priority
    /// </summary>
    protected IList<AIBehavior> BehaviorList_SortedPriority = new List<AIBehavior>();

    protected CharacterController controller;
    protected Seeker seeker;
	
	/// <summary>
	/// The switch to control print debug message.
	/// </summary>
	public bool PrintDebugMessage = false;
	
    public virtual void Awake()
    {
        StartCoroutine(InitAI());
    }

    public virtual void Start()
    {
        StartCoroutine(StartAI());
    }

    public virtual void Update()
    {
        if (Time.time >= ResetHaltTime && Halt == true)
        {
            Halt = false;
        }
    }

    public virtual void FixedUpdate()
    {
        //Refresh CurrentTarget in every Time.fixDeltaTime seconds
        FindTarget(DetectiveRange);
    }

    public virtual IEnumerator StartAI()
    {
        StartCoroutine("AlterBehavior", AlterBehaviorInterval);
        yield break;
    }

#region initialization
    /// <summary>
    /// 1. Register this AI to LevelManager
    /// 2. Initalize behavior list. Sort the behavior from higher to lower priorty.
    /// Offspring should call InitAI() at Awake() 
    /// 3. Start RefreshCanSeeTarget() daemon coroutine
    /// </summary>
    public virtual IEnumerator InitAI()
    {
        seeker = GetComponent<Seeker>();
        this.Unit = GetComponent<Unit>();
        controller = GetComponent<CharacterController>();

        StartAStarPathfind();

        LevelManager.RegisterAI(this);
        //Put the behavior into a sort list first, which sort the beheavior priority from lower to higher 
        SortedList<int,AIBehavior> tempList = new SortedList<int,AIBehavior>();
        foreach (AIBehavior beheavior in Behaviors)
        {
            tempList.Add(beheavior.Priority,beheavior);
        }
        //Then insert the behavior from higher to lower priority
        for (int i = tempList.Count-1; i>=0; i--)
        {
            BehaviorList_SortedPriority.Add(tempList.Values[i]);
        }
        yield break;
    }

    /// <summary>
    /// If offspring do not need AStar pathfinding, don't call it at Awake()
    /// </summary>
    public void StartAStarPathfind()
    {
        StartCoroutine("AStarNavigate");
        StartCoroutine("RefreshAStarPath");
    }

#endregion

#region basic AI supporting functions
    /// <summary>
    /// offspring should call StopAI() at offspring.StopAI()
    /// </summary>
    public virtual IEnumerator StopAI()
    {
        LevelManager.UnregisterAI(this);
        yield break;
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
        //if no enemy's around, and the current target is dead, reset the current target vaiables
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
        if (Target==null || Util.DistanceOfCharacters(gameObject, Target.gameObject) > MaxRange)
        {
            return false;
        }
        if (Target==null || Physics.Linecast(transform.position, CurrentTarget.position, AttackObstacle))
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
	/// Given an AttackData, check by its HitTestType, return true/false to indicate if AI has hit the target.
	/// </summary>
	public virtual bool CheckHitCondition(GameObject target, AttackData AttackData)
	{
		bool ShouldSendHitMessage =false;
        switch (AttackData.HitTestType)
        {
            case HitTestType.AlwaysTrue:
                ShouldSendHitMessage = true;
                break;
            case HitTestType.HitRate:
                float randomNumber = Random.Range(0f, 1f);
                ShouldSendHitMessage = (randomNumber <= AttackData.HitRate);
                break;
            case HitTestType.CollisionTest:
                ShouldSendHitMessage = AttackData.HitTestCollider.bounds.Intersects(target.collider.bounds);
                break;
            case HitTestType.DistanceTest:
                float TargetDistance = Util.DistanceOfCharacters(gameObject, CurrentTarget.gameObject); 
                ShouldSendHitMessage = TargetDistance <= AttackData.HitTestDistance;
			    break;
		    case HitTestType.AngleTest:
			    float TargetAngularDiscrepancy = Vector3.Distance(transform.forward, (CurrentTarget.position - transform.position).normalized);
			    ShouldSendHitMessage = TargetAngularDiscrepancy<= AttackData.HitTestAngularDiscrepancy;
                break;
		    case HitTestType.DistanceAndAngleTest:
                TargetDistance = Util.DistanceOfCharacters(gameObject, CurrentTarget.gameObject); 
			    TargetAngularDiscrepancy = Vector3.Distance(transform.forward, (CurrentTarget.position - transform.position).normalized);
                ShouldSendHitMessage = TargetAngularDiscrepancy <= AttackData.HitTestDistance && TargetAngularDiscrepancy<= AttackData.HitTestAngularDiscrepancy;
			    break;
        }
		return ShouldSendHitMessage;
	}
	
    /// <summary>
    /// Send ApplyDamage message, in %delay time% = AttackData.HitTime seconds
    /// </summary>
    /// <param name="target"></param>
    /// <param name="DamageParameter"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public virtual IEnumerator SendHitmessage(GameObject target, AttackData AttackData)
    {
        if (AttackData.HitTime != 0)
        {
			yield return new WaitForSeconds(AttackData.HitTime);
        }
        bool ShouldSendHitMessage = CheckHitCondition(target, AttackData);
        if(ShouldSendHitMessage)
           target.SendMessage("ApplyDamage", AttackData.GetDamageParameter(gameObject));
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

#region A* pathfind navigation functions - thanks to A* pathfinding team

    Transform NaivgateToTarget; // the target where A* should navigate to
    Path AStarPath; //the A* path calculated out
    bool AllowNavigate; //only navigate when AllowNavigate = true
    float LastPathingTime = 0; //last time of requesting pathing calculation
    float AStarNodePathReachCheckDistance = 1f; //the distance valve to check if the path node point has been reached
    int CurrentPathNodeIndex = 0; //the current path node index navigating to
    float RefreshPathInterval = 1f; //how long should call A* pathfind again, when navigating to non-static target
    bool AutoRefreshPath = false; //only auto refresh A* pathfind when AutoRefreshPath = true, for non-static target
    MoveData NavigationMoveData;
    /// <summary>
    /// A daemon routine, as long as AllowNavigate is true and AStarPath is not null, navigate moving along
    /// the AStarPath automatically
    /// </summary>
    /// <returns></returns>
    public IEnumerator AStarNavigate()
    {
        while (true)
        {
            if (Halt == true || AllowNavigate == false || AStarPath == null || 
                AStarPath.vectorPath == null || AStarPath.vectorPath.Length == 0)
            {
                yield return null;
                continue;
            }
            if (Vector3.Distance(transform.position, AStarPath.vectorPath[CurrentPathNodeIndex]) >= AStarNodePathReachCheckDistance)
            {
                Vector3 direction = (AStarPath.vectorPath[CurrentPathNodeIndex] - transform.position);
                float Speed = NavigationMoveData.MoveSpeed;
                if (NavigationMoveData.CanRotate)
                {
                    if (NavigationMoveData.SmoothRotate)
                        Util.MoveSmoothly(transform, direction.normalized * Speed, controller, NavigationMoveData.RotateAngularSpeed);
                    else
                        Util.MoveTowards(transform, transform.position + direction, controller, true, false, Speed, 0);
                }
                else
                {
                    Util.MoveTowards(transform, direction, controller, Speed);
                }
                animation.CrossFade(NavigationMoveData.AnimationName);
            }
            else //reach the current node
            {
                //move to next path point
                CurrentPathNodeIndex++;
                //reach the last node, reset navigation condition, and disable auto refresh pathing condition
                if (CurrentPathNodeIndex >= AStarPath.vectorPath.Length)
                {
                    AStarPath = null;
                    AllowNavigate = false;
                    AutoRefreshPath = false;
                    NaivgateToTarget = null;
                    NavigationMoveData = null;
                }
                continue;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Daemon routine, refresh AStar path at every %RefreshPathInterval% time.
    /// </summary>
    /// <returns></returns>
    public IEnumerator RefreshAStarPath()
    {
        while (true)
        {
            if (AutoRefreshPath == false || NaivgateToTarget == null)
            {
                yield return null;
                continue;
            }
            
            //Refresh path at every %RefreshPathInterval% seconds
            if ((Time.time - LastPathingTime) >= RefreshPathInterval)
            {
                FindPath(NaivgateToTarget);
                LastPathingTime = Time.time;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Callback delegate by A*.Seeker
    /// </summary>
    /// <param name="_path"></param>
    public void OnAStarPathComplete(Path _path)
    {
        AStarPath = _path;
        AllowNavigate = true;
        CurrentPathNodeIndex = 0;
        LastPathingTime = Time.time;
    }
	
	/// <summary>
	/// Finds the path to Target.
	/// If the Target is unseenable, require AStar Pathfind
	/// Else if the target is seeable, build vector path directly.
	/// </summary>
	public void FindPath(Transform Target)
	{
		//Only require A* pathfind when the current target can't be seen
		if(CanSeeTarget(Target.gameObject,999) == false)
		{
			try{
	           seeker.StartPath(transform.position, Target.position, OnAStarPathComplete);
			}
			catch(System.Exception exc)
			{
//				Debug.LogError(exc.Message);
			}
		}
		//the target is seeable, assign 
		else 
		{
			AStarPath = new Path();
			AStarPath.vectorPath = new Vector3[2] { transform.position, Target.position };
			AllowNavigate = true;
			CurrentPathNodeIndex = 0;
		}
	}
	
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
		FindPath(target);
        //If IsMovingTarget = false, means the target is static, no need to auto refresh path
        AutoRefreshPath = IsMovingTarget;
        NaivgateToTarget = target;
        NavigationMoveData = MoveData;
    }

    /// <summary>
    /// Call this routine to stop navigation immediately.
    /// </summary>
    public void StopNavigation()
    {
        AStarPath = null;
        AllowNavigate = false;
        AutoRefreshPath = false;
        NaivgateToTarget = null;
        CurrentPathNodeIndex = 0;
    }

    #endregion

#region AI Behavior determination

    /// <summary>
    /// ALter current behavior at every %ScanBehaviorInterval% seconds
    /// </summary>
    public virtual IEnumerator AlterBehavior(float Interval)
    {
        while (true)
        {
            //if the current behavior is still running, check if it meets end condition.
			// if the current behavior status is not running, it means it can stop at anytime
            if (CurrentBehavior != null && CurrentBehavior.Phase == AIBehaviorPhase.Running)
            {
                //If CurrentBehavior's end condition = false, do nothing
                if (IsConditionMatched(CurrentBehavior, CurrentBehavior.EndCondition) == false)
                {
					if(PrintDebugMessage)
					{
                       Debug.Log("Behavior:" + CurrentBehavior.Name + " do not meet end condition, not ended yet.");
					}
                    yield return new WaitForSeconds(Interval);
                    continue;
                }
            }
            AIBehavior behaviorToGo = null;
            //Choose next behavior whose StartCondition = True
            foreach (AIBehavior behavior in BehaviorList_SortedPriority)
            {
                if (IsConditionMatched(behavior, behavior.StartCondition))
                {
                    behaviorToGo = behavior;
                    break;
                }
            }
            //if no behavior meets start condition, let it be, print a message and wait for next loop
            if (behaviorToGo == null)
            {
                Debug.LogWarning("No behavior can start - " + gameObject.name);
                yield return new WaitForSeconds(Interval);
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
                        //Wait one frame to let StopBehavior complete.
                        yield return null;
                    }
                    StartBehavior(behaviorToGo);
                    CurrentBehavior = behaviorToGo;
                }
                yield return new WaitForSeconds(Interval);
            }
        }
    }

    /// <summary>
    /// Determine if the behavior matches start condition.
    /// Return true/false.
    /// Offspring can override this method.
    /// </summary>
    /// <returns></returns>
    public virtual bool IsConditionMatched(AIBehavior Behavior, AIBehaviorCondition AICondition)
    {
        bool ret = false;
        //switch (Behavior.ConditionConjunction)
        switch (AICondition.Conjunction)
        {
            case LogicConjunction.None:
                ret = CheckCondition(AICondition.ConditionData1, Behavior);
                break;
            case LogicConjunction.And:
                ret = CheckCondition(AICondition.ConditionData1, Behavior);
                if (ret == true)
                {
                    ret = ret && CheckCondition(AICondition.ConditionData2, Behavior);
                }
                break;
            case LogicConjunction.Or:
                ret = CheckCondition(AICondition.ConditionData1, Behavior);
                if (ret == false)
                {
                    ret = ret || CheckCondition(AICondition.ConditionData2, Behavior);
                }
                break;
        }
        return ret;
    }

    /// <summary>
    /// Check if the AIBehavior condition is true
    /// </summary>
    /// <param name="AIBehaviorCondition"></param>
    /// <returns></returns>
    public virtual bool CheckCondition(ConditionData AIBehaviorCondition, AIBehavior behavior)
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
    public virtual bool CheckBooleanCondition(ConditionData AIBehaviorCondition, AIBehavior behavior)
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
			    LeftValue = (AIBehaviorCondition.StringValue == this.CurrentBehavior.Name);
			    break;
		case AIBooleanConditionEnum.LastestBehaviorNameIsOneOf:
			    LeftValue = Util.ArrayContains<string>(AIBehaviorCondition.StringValueArray, this.CurrentBehavior.Name);
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
    public virtual bool CheckValueComparisionCondition(ConditionData AIBehaviorCondition, AIBehavior behavior)
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
		//remember the last start time
        behavior.LastExecutionTime = behavior.StartTime;
		//remember the start time
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
            if (distance <= this.AStarNodePathReachCheckDistance)
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
            if (distance <= this.AStarNodePathReachCheckDistance)
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

        while (true)
        {
            //If no target is found, do nothing
            if (Halt || CurrentTarget == null)
            {
                yield return null;
                continue;
            }
            //Get attack data
            AttackData attackData = Unit.AttackDataDict[behavior.AttackDataName];
            string AttackAnimationName = attackData.AnimationName;
            //If can see target, and target distance <= AttackableRange, do this:
            //1. Face to target
            //2. Check last attack time against attack interval
            //3. if #2 pass, animating, send hit message
            if (this.CanSeeCurrentTarget && CurrentTargetDistance <= attackData.AttackableRange)
            {
                transform.LookAt(new Vector3(CurrentTarget.position.x, transform.position.y, CurrentTarget.position.z));
                DoAttack(attackData);
            }
            else if (animation.IsPlaying(attackData.AnimationName))
            {
                yield return null;
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
        AttackData attackData = Unit.AttackDataDict[behavior.AttackDataName];
        MoveData moveData = Unit.MoveDataDict[behavior.MoveDataName];
        animation.Stop(moveData.AnimationName);
        animation.Stop(attackData.AnimationName);
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

    /// <summary>
    /// Try to perform Attack action. If the timing is not available to perform attack, the method do nothing and return.
    /// It's safe to call this method per frame.
    /// </summary>
    public virtual void DoAttack(AttackData attackData)
    {
        //Don't attack if it's too early.
        if (Halt || Time.time - LastAttackTime <= attackData.AttackInterval)
        {
            return;
        }
        else
        {
            this.LastAttackTime = Time.time;
			animation.CrossFade(attackData.AnimationName);
            Attack(attackData);
        }
    }
	
	/// <summary>
	/// Different to DoAttack(AttackData), this method blocks until attack finish.
	/// </summary>
	public virtual IEnumerator DoAttack_Block(AttackData attackData)
	{
		float _time = Time.time;
		Attack(attackData);
		animation.CrossFade(attackData.AnimationName);
		while((Time.time - _time) <= attackData.AttackInterval)
		{
			yield return null;
		}
	}
	
	/// <summary>
	/// The actual routine to perform attack.
    /// </summary>
    /// <param name="attackData"></param>
	public virtual void Attack(AttackData attackData)
	{
        DamageParameter dp = attackData.GetDamageParameter(this.gameObject);
        switch (attackData.Type)
        {
                case AIAttackType.Instant:
                    StartCoroutine(SendHitmessage(CurrentTarget.gameObject, attackData));
                    break;
                case AIAttackType.Projectile:
                    StartCoroutine(CreateProjectile(attackData, attackData.HitTime));
                    break;
                //Regional attack is bit special, because Regional attack could hurts
                //more than one enemy(not just the CurrentTarget) as long as enemy intersect
                //with the HitTestCollider!
                case AIAttackType.Regional:
                    //scan enemy inside the HitTestDistance range:
                    foreach (Collider c in Physics.OverlapSphere(transform.position, attackData.HitTestDistance, Unit.EnemyLayer))
                    {
                        StartCoroutine(SendHitmessage(c.gameObject, attackData));
                    }
                    break;
                default:
                    Debug.Log("Unsupported attack type:" + attackData.Type.ToString() + " at object:" + gameObject.name);
                    break;
        }
	}
	
#endregion

#region Visual Effect

    /// <summary>
    /// Create a effect object.
    /// Note: the %name% MUST BE an effective name in the key set of Unit.EffectDataDict
    /// </summary>
    /// <param name="name"></param>
    public virtual void CreateEffect(string name)
    {
        if (Unit.EffectDataDict.Keys.Contains(name) == false)
        {
            Debug.LogError("There is no such effect:" + name + " gameobject-" + gameObject.name);
        }
        else
        {
            EffectData data = Unit.EffectDataDict[name];
            Object effectObject = Object.Instantiate(data.EffectObject, data.Anchor.position, data.Anchor.rotation);
            if (data.DestoryInTimeOut)
            {
                Destroy(effectObject, data.DestoryTimeOut);
            }
        }
    }

    

#endregion

#region Audio playing
    
#endregion

#region Receive Damage and Die
    public virtual IEnumerator ApplyDamage(DamageParameter damageParam)
    {
        if (Unit.IsDead)
        {
            yield break;
        }
        //Minus HP:
        Unit.HP -= damageParam.damagePoint;
        if (Unit.HP <= 0)
        {
            StartCoroutine("Die", damageParam);
            yield break;
        }
        else
        {
            StartCoroutine("ReceiveDamage", damageParam);
            yield break;
        }
        
    }

    public virtual IEnumerator ReceiveDamage(DamageParameter damageParam)
    {
        //Get the right ReceiveDamageData
        ReceiveDamageData ReceiveDamageData = null;
        if (Unit.ReceiveDamageDataDict.ContainsKey(damageParam.damageForm))
        {
            if (Unit.ReceiveDamageDataDict[damageParam.damageForm].Count == 1)
            {
                ReceiveDamageData = Unit.ReceiveDamageDataDict[damageParam.damageForm][0];
            }
            else
            {
                int RandomIndex = Random.Range(0, Unit.ReceiveDamageDataDict[damageParam.damageForm].Count);
                ReceiveDamageData = Unit.ReceiveDamageDataDict[damageParam.damageForm][RandomIndex];
            }
        }
        //If ReceiveDamageDataDict[DamageForm.Common] = null or Count ==0, will have error!
        //So make sure you have assign a Common receive damage data!
        else
        {
            ReceiveDamageData = Unit.ReceiveDamageDataDict[DamageForm.Common][0];
        }
        //Create effect data
        if (ReceiveDamageData.EffectDataName != null && ReceiveDamageData.EffectDataName.Length > 0)
        {
            foreach (string effectDataName in ReceiveDamageData.EffectDataName)
            {
                EffectData EffectData = Unit.EffectDataDict[effectDataName];
                GlobalBloodEffectDecalSystem.CreateBloodEffect(transform.position + controller.center, EffectData);
            }
        }
        //Create blood decal:
        if (ReceiveDamageData.DecalDataName != null && ReceiveDamageData.DecalDataName.Length > 0)
        {
            foreach (string decalName in ReceiveDamageData.DecalDataName)
            {
                DecalData DecalData = Unit.DecalDataDict[decalName];
                GlobalBloodEffectDecalSystem.CreateBloodDecal(transform.position + controller.center, DecalData);
            }
        }

        //Halt AI if set true
        if (ReceiveDamageData.HaltAI)
        {
            animation.Rewind(ReceiveDamageData.AnimationName);
            animation.CrossFade(ReceiveDamageData.AnimationName);
            Halt = true;
            ResetHaltTime = Time.time + animation[ReceiveDamageData.AnimationName].length;
            yield return new WaitForSeconds(animation[ReceiveDamageData.AnimationName].length);
        }
    }

    public virtual IEnumerator Die(DamageParameter DamageParameter)
    {
        //Basic death processing.
        controller.enabled = false;
        Unit.IsDead = true;
        StopAllCoroutines();
        animation.Stop();

        //Handle setting - DeathData:
        DeathData DeathData = null;
        if(Unit.DeathDataDict.ContainsKey(DamageParameter.damageForm))
        {
            IList<DeathData> DeathDataList = Unit.DeathDataDict[DamageParameter.damageForm];
            DeathData = Util.RandomFromList(DeathDataList);
        }
        else 
        {
            DeathData = Util.RandomFromList<DeathData>( Unit.DeathDataDict[DamageForm.Common]);
        }

        //Create effect data 
        if (DeathData.EffectDataName != null && DeathData.EffectDataName.Length > 0)
        {
            foreach (string effectDataName in DeathData.EffectDataName)
            {
                EffectData EffectData = Unit.EffectDataDict[effectDataName];
                GlobalBloodEffectDecalSystem.CreateBloodEffect(transform.position + controller.center, EffectData);
            }
        }
        //Create blood decal:
        if (DeathData.DecalDataName != null && DeathData.DecalDataName.Length > 0)
        {
            foreach (string decalName in DeathData.DecalDataName)
            {
                DecalData DecalData = Unit.DecalDataDict[decalName];
                GlobalBloodEffectDecalSystem.CreateBloodDecal(transform.position + controller.center, DecalData);
            }
        }


        if(DeathData.UseDieReplacement)
        {
            if(DeathData.ReplaceAfterAnimationFinish)
            {
                animation.CrossFade(DeathData.AnimationName);
                yield return new WaitForSeconds(animation[DeathData.AnimationName].length);
            }
            GameObject DieReplacement = (GameObject)Object.Instantiate(DeathData.DieReplacement, transform.position, transform.rotation);
            Debug.Log("Creating replacement:" + DieReplacement.name);
            if(DeathData.CopyChildrenTransformToDieReplacement)
            {
                Util.CopyTransform(transform, DieReplacement.transform);
            }
            Destroy(gameObject);
        }
        else 
        {
            animation.CrossFade(DeathData.AnimationName);
        }
        
    }

    public void CreateDecal()
    {
        //Locate the place to create decal:

    }
#endregion

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, OffensiveRange);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, DetectiveRange);
        if (AStarPath != null && AStarPath.vectorPath != null)
        {
            for(int i=0; i<AStarPath.vectorPath.Length; i++)
            {
                if(i!=AStarPath.vectorPath.Length-1)
                   Gizmos.DrawLine(AStarPath.vectorPath[i], AStarPath.vectorPath[i+1]);
            }
        }
    }
}
