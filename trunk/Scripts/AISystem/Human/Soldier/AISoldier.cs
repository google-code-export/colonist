using UnityEngine;
using System.Collections;

public class AISoldier : MonoBehaviour {
    
    public float MovementSpeed = 2f;
    public float FallbackSpeed = 2f;
    public float SprintSpeed = 2f; 
    /// <summary>
    /// the range the unit can attack in
    /// </summary>
    public float AttackRadius = 3;
    /// <summary>
    /// the range the unit will start alert mode.
    /// - player enter the range then the unit will "alert" to find a way to escape away from the player
    /// </summary>
    public float AlertRadius = 5;
    /// <summary>
    /// the range the unit will start escaping from threat nearby
    /// </summary>
    public float CriticalRadius = 3;

    public float AttackInterval = 1;

    /// <summary>
    /// About how soon should redirect the fallback direction ?
    /// </summary>
    public float RedirectFallbackTimeout = 0.8f;

    public string WalkAnimation = "walk";
    public string WalkBackwardAnimation = "walk_backward";
    public string EscapeAnimation = "sprint";

    public string RunAnimation = "runWithPistol";
    public string ShootAnimation = "shootPistol";
    public string MixedShootAnimation = "shootPistol_mix";

    public bool CanEscape = true;

    public LayerMask targetLayer;
    public LayerMask terrainLayer;
    public LayerMask WallLayer;

    public Transform Spine1 = null;
    public Transform Root = null;
    public Transform LLeg = null;
    public Transform RLeg = null;

    [HideInInspector]
    public Transform currentTarget;
    /// <summary>
    /// StartWaypoint is routed when spawning the NPC
    /// </summary>
    public WayPoint StartWaypoint;

    /// <summary>
    /// PartolWaypoint is routed when the target can't be seen
    /// </summary>
    [HideInInspector]
    public WayPoint PartolWaypoint;
	
	/// <summary>
	/// The flag to halt an AI.
	/// true = Halt
	/// false = No halt
	/// </summary>
	[HideInInspector]
	public bool Halt = false;
	
	
    private WayPoint currentWaypoint;

    private CharacterController controller;

    // fallback parameters:
    private float enemyDistance = 9999;
    private float LastTimeRedirectionFallback = -1;
    private Vector3 fallBackDirection = new Vector3();

    private float HitWallTimeLength = 0;
    private bool BlockByWall = false;
    /// <summary>
    /// lastShootTime is used when mixing shooting and walking(backward) animation.
    /// cooperates with AttackInterval to perform a shoot in timing function.
    /// </summary>
    private float lastShootTime = -1;
    private float lastHitWallTime = 0;

    void InitAnimation()
    {
        animation[MixedShootAnimation].AddMixingTransform(Spine1);
        animation[MixedShootAnimation].layer = 10;

        animation[WalkBackwardAnimation].AddMixingTransform(Root);
        animation[WalkBackwardAnimation].AddMixingTransform(Spine1);
        animation[WalkBackwardAnimation].RemoveMixingTransform(Spine1);
    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        InitAnimation();
    }

	// Use this for initialization
	IEnumerator Start () {
        if (StartWaypoint != null)
        {
            yield return StartCoroutine(WayPoint.AutoRoute(this, StartWaypoint, controller, MovementSpeed, WalkAnimation ));
        }
        SendMessage("SearchAndKill");
	}

    void SetStartWaypoint(WayPoint wp)
    {
        StartWaypoint = wp;
    }
	
	// Update is called once per frame
	void Update () {
        if (this.currentTarget != null && IsAnimatingAttack())
        {
            Util.RotateToward(transform, currentTarget.position, false, 0);
        }
        if (currentTarget && currentTarget.GetComponent<GetHP>().HP <= 0)
        {
            currentTarget = null;
        }
        //We need to check, whether the npc who blocked by wall, is still being blocked or not.
        //If the last lastHitWallTime is 1/3 second ago, then it means the npc has not been blocked anymore
        //so we reset the HitWallTimeLength & BlockByWall
        if (BlockByWall && (Time.time - lastHitWallTime >= 1f))
        {
            BlockByWall = false;
            HitWallTimeLength = 0;
        }
	}

    void FixedUpdate()
    {
        CalculateFallbackParameter();
        if (HitWallTimeLength >= 1f)
        {
            BlockByWall = true;
        }
    }
    
    /// <summary>
    /// Calculate the direction for fallback. 
    /// Basically the direction is away from currentTarget, but have a random angular offset (-30,30)
    /// </summary>
    private void CalculateFallbackParameter()
    {
        if (currentTarget)
        {
            enemyDistance = currentTarget.GetComponent<CharacterController>() ?
                    Util.DistanceOfCharactersXZ(controller, currentTarget.GetComponent<CharacterController>()) :
                    Util.Distance_XZ(currentTarget.position, transform.position);
            if (enemyDistance <= AlertRadius)
            {
                //Redirect fallback direction in every <RedirectFallbackTimeout> seconds
                if (LastTimeRedirectionFallback <=0 || (Time.time - LastTimeRedirectionFallback) >= RedirectFallbackTimeout)
                {
                    Vector3 direction = transform.position - currentTarget.transform.position;
                    //plus random factor
                    Quaternion rotation = Quaternion.LookRotation(direction);
                    rotation = Util.RotateAngleYAxis(rotation, Random.Range(-30f, 30f));
                    fallBackDirection = rotation * Vector3.forward;
                    Debug.DrawRay(transform.position, fallBackDirection, Color.red);
                    //Debug.Log("Redirect at:" + Time.time); 
                    LastTimeRedirectionFallback = Time.time;
                }
            }
        }
    }

    bool IsAnimatingAttack()
    {
        bool isAttacking = animation.IsPlaying(ShootAnimation);
        return isAttacking;
    }

    private bool FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 9999, targetLayer);
        if (colliders != null && colliders.Length > 0)
        {
            currentTarget = colliders[0].transform;
            return true;
        }
        else
            return false;
    }

    private bool CanSeeTarget()
    {
        if (!controller)
        {
            Debug.Log("Controller is null!");
        }
        if (!currentTarget.GetComponent<CharacterController>())
        {
            Debug.Log("currentTarget.Controller is null: " + currentTarget.name);
        }
        if (Util.DistanceOfCharactersXZ(controller, currentTarget.GetComponent<CharacterController>()) > AttackRadius)
        {
            return false;
        }
        if (Physics.Linecast(transform.position, currentTarget.position, terrainLayer))
        {
            return false;
        }
        return true;
    }

    private void Attack()
    {
        //if have target, but can't see it
        if (!CanSeeTarget())
        {

        }
        //Move to target 
        float distance = Util.DistanceOfCharactersXZ(this.controller, currentTarget.GetComponent<CharacterController>());
        //If distance not close enough to attack
        if (distance > AttackRadius)
        {
            Util.MoveTowards(transform, currentTarget.position, this.controller, true, false, MovementSpeed, 0);
            animation.CrossFade(RunAnimation);
        }
        else
        {
            ShootOnce(false);
        }
    }

    /// <summary>
    /// One shot, one kill!
    /// if isMoving = true , then animate the mixed animation which has a higher layer with much more weight
    /// </summary>
    void ShootOnce(bool isMoving)
    {
		//Debug.Log("ShootOnce:" + isMoving);
        if ((Time.time - lastShootTime) < AttackInterval)
        {
            Util.RotateToward(transform, currentTarget.position, false, 0);
            return;
        }

        if (isMoving)
        {
            animation.Rewind(MixedShootAnimation);
            animation.Play(MixedShootAnimation);
        }
        else
        {
            animation.Rewind(ShootAnimation);
            animation.CrossFade(ShootAnimation);
        }
        SendMessage("Fire", currentTarget);
        lastShootTime = Time.time;
    }

    IEnumerator WalkAround()
    {
        yield return null;
    }

    IEnumerator SearchAndKill()
    {
        while (true)
        {
			if(Halt)
			{
				yield return null;
				continue;
			}
           //if no target is found, do nothing
           if (currentTarget == null && FindTarget() == false)
           {
               yield return null;
               continue;
           }
           //If enemy stays far away is fine to shoot or approaching to shoot
           if (enemyDistance >= AlertRadius)
           {
                Attack();
           }
           //else if emeny comes nearby but not too closed, fallback - but not stop shooting
           else if ((enemyDistance > CriticalRadius && enemyDistance <= AlertRadius) || !CanEscape)
           {
               //Debug.Log("Danager!Now fall back at direction :" + fallBackDirection);
               if(fallBackDirection == Vector3.zero)
               {
                    CalculateFallbackParameter();
				}
                Fallback();
           }
           //fine - it's too close, let's run !
           else
           {
                //Debug.Log("Now escaping!");
                if (fallBackDirection == Vector3.zero)
                {
                    CalculateFallbackParameter();
                }
                yield return StartCoroutine(Escape());
           }
           yield return null;
        }
    }

    /// <summary>
    /// NPC sprint at a direct to escape from threat.
    /// There's a minimum duration the npc has to escape in (to reduce frequent switch between Escape() and Fallback()
    /// </summary>
    /// <returns></returns>
    IEnumerator Escape()
    {
        float _t = Time.time;
        float sprintLength = Random.Range(1, 1.5f);
        while( (Time.time - _t) <= sprintLength)
        {
			if(Halt)
				yield break;
            transform.LookAt(transform.position + fallBackDirection);
            Util.MoveTowards(transform, fallBackDirection, controller, SprintSpeed);
            animation.CrossFade(EscapeAnimation);
            yield return null;
        }
    }

    void Fallback()
    {
        Util.MoveTowards(transform, fallBackDirection, controller, FallbackSpeed);
        animation.CrossFade(WalkBackwardAnimation);
        if (currentTarget)
        {
            ShootOnce(true);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.layer != LayerMask.NameToLayer("walls"))
            return;
        //Debug.DrawLine(hit.point, hit.point + hit.normal, Color.red);
        //Debug.Break();
        HitWallTimeLength = HitWallTimeLength + Time.deltaTime;
        lastHitWallTime = Time.time;
        Debug.Log("Last hit wall time frame:" + lastHitWallTime + " " + Time.frameCount);
    }

    void StopAI()
    {
		Debug.Log("In stopAI!");
        StopAllCoroutines();
        animation.Stop();
        //Remove character controller
        //Destroy(controller);
        //Destory (Controller) caused unexplained error in 64bit U3D version so we try not to destory controller but to disable it
        controller.enabled = false;
        //Remove this monobeheavior
        //Destroy(this);
    }

    void OnCollisionStay(Collision collisionInfo) 
    {
        Debug.Log("Detect collision - " + collisionInfo.gameObject.name + " " + collisionInfo.collider.gameObject.name);
    }
     
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, AlertRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, CriticalRadius);
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, fallBackDirection * 5);
    }
}
