using UnityEngine;
using System.Collections;
using Pathfinding;

/// <summary>
/// Facade of A* pathfind.
/// </summary>
public class AStarNavigator : Navigator {
	    
	/// <summary>
	/// how long should call A* pathfind again, when navigating to non-static target
	/// </summary>
    public float RefreshPathInterval = 1f; 
	
	/// <summary>
	/// the distance valve to check if the path node point has been reached
	/// </summary>
    public float ReachCheckDistance = 1f; 
	
	public LayerMask Obstacle = new LayerMask();
	
	[HideInInspector]
    public Transform NaivgateToTarget; // the target where A* should navigate to
	[HideInInspector]
	public bool AllowNavigate; //only navigate when AllowNavigate = true
	[HideInInspector]
    public MoveData NavigationMoveData;
	
	Seeker seeker;
	CharacterController controller = null;
    Path AStarPath; //the A* path calculated out
    int CurrentPathNodeIndex = 0; //the current path node index navigating to
	float LastPathingTime = 0; //last time of requesting pathing calculation
    bool AutoRefreshPath = false; //only auto refresh A* pathfind when AutoRefreshPath = true, for non-static target
	Unit unit;
	bool Halt
	{
		get 
		{
			return unit.Halt;
		}
	}
	
//	public Transform testMoveTo;
//	public MoveData movedata;
	void Awake()
	{
		seeker = GetComponent<Seeker>();
		controller = GetComponent<CharacterController>();
		unit = GetComponent<Unit>();
	}
	
	void Start()
	{
		StartCoroutine("AStarNavigate");
        StartCoroutine("RefreshAStarPath");
	}
	
	void Update()
	{

	}
	
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
            if (Vector3.Distance(transform.position, AStarPath.vectorPath[CurrentPathNodeIndex]) >= ReachCheckDistance)
            {
                Vector3 direction = (AStarPath.vectorPath[CurrentPathNodeIndex] - transform.position);
                float Speed = NavigationMoveData.MoveSpeed;
                if (NavigationMoveData.CanRotate)
                {
                    if (NavigationMoveData.SmoothRotate)
                        Util.MoveSmoothly(transform, direction.normalized * Speed, controller, NavigationMoveData.RotateAngularSpeed, NavigationMoveData.UseGravityWhenMoving);
                    else
                        Util.MoveTowards(transform, transform.position + direction, controller, true, false, Speed, 0, NavigationMoveData.UseGravityWhenMoving);
                }
                else
                {
                    Util.MoveTowards(transform, direction, controller, Speed, NavigationMoveData.UseGravityWhenMoving);
                }
				if(NavigationMoveData.UseAnimation)
				{
                   animation.CrossFade(NavigationMoveData.AnimationName);
				}
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
            if (Halt || AutoRefreshPath == false || NaivgateToTarget == null)
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
    public override void StartNavigation(Transform target, bool IsMovingTarget, MoveData MoveData)
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
    public override void StopNavigation()
    {
        AStarPath = null;
        AllowNavigate = false;
        AutoRefreshPath = false;
        NaivgateToTarget = null;
        CurrentPathNodeIndex = 0;
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
        if (Target==null || Util.DistanceOfCharacters(gameObject, Target.gameObject) > MaxRange
			|| Physics.Linecast(transform.position, Target.transform.position, Obstacle))
        {
            return false;
        }
        return true;
    }
	
#region AnimationEvent
	
	/// <summary>
	/// this function is for event invoking.(AnimationEvent, or GameEvent).
	/// Because of the parameter restriction, you will usually need to call _StopNavigationInTime to stop in pair.
	/// </summary>
	public void _NavigateToCurrentTarget(string name)
	{
		this.StartNavigation(this.unit.CurrentTarget, true, this.unit.MoveDataDict[name]);
	}
	
	/// <summary>
	/// After N seconds, stop navigation
	/// </summary>
	public IEnumerator _StopNavigationInTime(float time)
	{
		if(time != 0)
		{
		   yield return new WaitForSeconds(time);
		}
		this.StopNavigation();
	}
	
#endregion
	
	void OnDrawGizmosSelected()
    {
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
