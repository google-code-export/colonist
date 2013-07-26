using UnityEngine;
using System.Collections;


[@RequireComponent(typeof(Predator3rdPersonVisualEffectController))]
[RequireComponent(typeof(Predator3rdPersonalUnit))]
public class Predator3rdPersonalJumpController : MonoBehaviour {

    [HideInInspector]
    public bool IsJumping = false;

    [HideInInspector]
    public bool checkJump = true;
	
	/// <summary>
	/// ForwardJumpTime defines max time of the predator jump forward.
	/// </summary>
    private float ForwardJumpTime = 0.5f;
	/// <summary>
	/// ForwardJumpSpeed jump speed defines speed of the predator jump forward.
	/// The final jumpDistance = ForwardJumpTime * ForwardJumpSpeed
	/// </summary>
    private float ForwardJumpSpeed = 12f;
	
	/// <summary>
	/// JumpOverSpeed defines when under jump-over-obstacle mode, the speed during jump over.
	/// </summary>
    private float JumpOverSpeed = 10f;
	
	/// <summary>
	/// The JumpoverCheckDistance defines when under jump-over-obstacle mode, the distance for the predator to detect a jump-over-obstacle.
	/// </summary>
    private float JumpoverCheckDistance = 3;
	
	/// <summary>
	/// The jumping animation name.
	/// </summary>
    private string JumpingAnimation = "jumping";
	
	/// <summary>
	/// The grounding animation name.
	/// </summary>
    private string GroundingAnimation = "jumpToGround";
	
	/// <summary>
	/// The prejump animation name.
	/// </summary>
    private string PrejumpAnimation = "prejump";
	
	/// <summary>
	/// Runtime-persist value, record the last jump time.
	/// </summary>
	private float LastJumpTime = 9999;
	
	/// <summary>
	/// The CharacterController of the predator.
	/// </summary>
    private CharacterController controller;
	
	/// <summary>
	/// The ground layer.
	/// </summary>
    private LayerMask GroundLayer;
	
	/// <summary>
	/// The jump over obstacle layer.Used to detect the jump-over obstacle ahead.
	/// </summary>
    private LayerMask JumpOverObstacleLayer;
    
	/// <summary>
	/// The predator player unit.
	/// </summary>
    private Predator3rdPersonalUnit PredatorPlayerUnit = null;
	
	private PredatorPlayerStatus predatorPlayerStatus = null;
	
	/// <summary>
	/// The safe trigger. In case jumping status takes over-long (During jumping, player is NOT allowed any input!), after that time, the jump status will be set false.
	/// </summary>
	private float ResetJumpInterval;
	
	/// <summary>
	/// When jump forward, the max height of the parabola line.
	/// </summary>
	public float JumpForwardMaxHeight = 5;
	
	
    void Awake()
    {
        controller = GetComponent<CharacterController>();
//        ClawEffectController = GetComponent<Predator3rdPersonVisualEffectController>();
        PredatorPlayerUnit = this.GetComponent<Predator3rdPersonalUnit>();
		
		predatorPlayerStatus = this.GetComponent<PredatorPlayerStatus>();
		
        GroundLayer = PredatorPlayerUnit.GroundLayer;
        JumpOverObstacleLayer = PredatorPlayerUnit.JumpData.ObstacleToJumpOver;

        PrejumpAnimation = PredatorPlayerUnit.JumpData.PreJumpAnimation;
        GroundingAnimation = PredatorPlayerUnit.JumpData.GroundingAnimation;
        JumpingAnimation = PredatorPlayerUnit.JumpData.JumpingAnimation;

        JumpOverSpeed = PredatorPlayerUnit.JumpData.JumpOverSpeed;
        JumpoverCheckDistance = PredatorPlayerUnit.JumpData.JumpOverCheckDistance;

        ForwardJumpTime = PredatorPlayerUnit.JumpData.JumpForwardTime;
        ForwardJumpSpeed = PredatorPlayerUnit.JumpData.JumpForwardSpeed;
		
		ResetJumpInterval = PredatorPlayerUnit.JumpData.ResetJumpInterval;
    }
	
	void Update()
	{
		//in case the IsJumping not being set back, we need a per frame check !
		if(IsJumping == true && ((Time.time - LastJumpTime) > ResetJumpInterval))
		{
			IsJumping = false;
			Debug.Log(Time.time  + " reset is jumping to false, LastJumpTime:" + LastJumpTime + " forwardJumpTime:" + ForwardJumpTime);
		}
	}

    /// <summary>
    /// Trigger a Jump beheavior.
    /// if there is a jump over obstacle ahead, jump over it.
    /// else , jump forward.
    /// </summary>
    /// <param name='Power'>
    /// Power - a value to indicate the jump power, if power = 1, means the predator will jump max forward distance. 
    /// Note: the power is eventually converted to time : 
    ///  - if power = 1, the jumpforward time = ForwardJumpTime.
    ///  - if power = 0 (which is not possible, beacuse min-power = 0.2f, but in theory), means the jumpforward time = 0.
    /// Note: power is only applicable for jump forward case.
    /// </param>
    public IEnumerator Jump(float Power)
    {
		if(IsJumping)
		{
			yield break;
		}
//		Debug.Log("Jump at frame:" + Time.frameCount + " isJumping:" + IsJumping + " at time:" + Time.time + "at jump power:" + Power);
        JumpOverObstacle obstacle = null;
        bool HasObstacle = CheckJumpOverObstacle(out obstacle);
		Debug.Log("Check obstacle:" + HasObstacle);
        //If there is obstacle, jump over it
        if (HasObstacle)
        {
            Vector3 HeightPoint , GroundPoint;
            obstacle.GetJumpOverTrack(transform, out HeightPoint, out GroundPoint);
            yield return StartCoroutine(JumpOverSmoothly(HeightPoint, GroundPoint));
        }
        //Else, jump forward
        else
        {
            yield return StartCoroutine(JumpStraightForward(Power));
        }
    }

    /// <summary>
    /// Jumping straight forward at speed = ForwardJumpSpeed, in ForwardJumpTime
    /// </summary>
    /// <returns></returns>
    IEnumerator JumpStraightForward(float power)
    {
        IsJumping = true;
        Vector3 jumpDirection = transform.forward;

        LastJumpTime = Time.time;//record the current time as last jump time.
		
		//rising time = 1/2 of ForwardJumpTime
		float _jumpTime = ForwardJumpTime * power;
//		Debug.Log("Jump time:" + _jumpTime + " max jump forward time:" + ForwardJumpTime);
		float RisingTime = _jumpTime * 0.5f;
		float gravity = (float)(this.JumpForwardMaxHeight / (1.5f * RisingTime * RisingTime));
		float upwardInitalSpeed = gravity * RisingTime;
		
		Vector3 jumpForwardVelocity = jumpDirection * this.ForwardJumpSpeed;
		jumpForwardVelocity.y = 0;
		Vector3 upwardVelocity = new Vector3(0, upwardInitalSpeed, 0);
        while ((Time.time - LastJumpTime) <= _jumpTime)
        {
            animation.Play(JumpingAnimation);
//            Vector3 jumpVelocity = jumpDirection * ForwardJumpSpeed * Time.deltaTime;
            upwardVelocity.y -= gravity * Time.deltaTime; 
			Vector3 forwardVelocity = jumpForwardVelocity * Time.deltaTime + upwardVelocity;
			
			controller.Move(forwardVelocity);

            //Util.MoveTowards(transform, transform.forward, controller, ForwardJumpSpeed);
            yield return null;
        }
        //If not ground, force grounding at once
        if (controller.isGrounded == false)
        {
            GroundAtOnce();
        }
        IsJumping = false;
        //Grounding animation is optional - player may skip this part 
        animation.Play(GroundingAnimation);
        yield return new WaitForSeconds(animation[GroundingAnimation].length);
    }
	
	
	/// <summary>
	/// A more real jumps forward function. The predator jumps forward in parabola line.
	/// </summary>
	/// <returns>
	/// The forward real.
	/// </returns>
	IEnumerator JumpForwardRealistic(Vector3 StartPosition, Vector3 TargetPosition, 
		                             float Speed, float Overtime, float Radian)
	{
		//Calculate the jump forward to position.

		
		float Distance = Vector3.Distance(TargetPosition, StartPosition);
        float totalTime = Distance / Speed;
        float Height = Distance * Radian;
        float RisingTime = totalTime / 2;
        float upwardInitalSpeed, gravity = 0;//V = gravity * RisingTime
        //gravity * RisingTime * RisingTime + 0.5 * gravity * RisingTime * RisingTime = Height
        gravity =(float)(Height / (1.5 * RisingTime * RisingTime));
        upwardInitalSpeed = gravity * RisingTime;

        Vector3 forwardVelocity = (TargetPosition - StartPosition).normalized * Speed;
        Vector3 velocity = forwardVelocity;
        velocity.y = upwardInitalSpeed;
        float StartTime = Time.time;
        while ((Time.time - StartTime) <= totalTime)
        {
            transform.LookAt(transform.position + velocity);
            transform.position += velocity * Time.deltaTime;
			
            velocity.y -= gravity * Time.deltaTime;
            yield return null;
        }
	}

    /// <summary>
    /// Move over a jump over obstacle along parabola line.
    /// </summary>
    /// <param name="HeightPoint"></param>
    /// <param name="GroundPoint"></param>
    /// <returns></returns>
    IEnumerator JumpOverSmoothly(Vector3 HeightPoint, Vector3 GroundPoint)
    {
        float Distance = Vector3.Distance(GroundPoint, transform.position);
        float totalTime = Distance / JumpOverSpeed;
        float Height = HeightPoint.y - transform.position.y;

        //Debug.DrawLine(transform.position, HeightPoint);
        //Debug.Break();

        float RisingTime = totalTime / 2;
        float upwardInitalSpeed, gravity = 0;//V = gravity * RisingTime
        //gravity * RisingTime * RisingTime + 0.5 * gravity * RisingTime * RisingTime = Height
        gravity = (float)(Height / (1.5 * RisingTime * RisingTime));
        upwardInitalSpeed = gravity * RisingTime;
        Vector3 forwardVelocity = (GroundPoint - transform.position).normalized * JumpOverSpeed;
        Vector3 velocity = forwardVelocity;
        velocity.y = upwardInitalSpeed;
        IsJumping = true;

        animation.Play(PrejumpAnimation);
        yield return new WaitForSeconds(animation[PrejumpAnimation].length);

        float StartTime = Time.time;
        animation.CrossFade(JumpingAnimation);
        while ((Time.time - StartTime) <= totalTime)
        {
            transform.position += velocity * Time.deltaTime;
            velocity.y -= gravity * Time.deltaTime;
            yield return null;
        }
        transform.position = GroundPoint + Vector3.up * 0.3f;
        IsJumping = false;
        //Grounding animation is optional - player may skip this part 
        animation.CrossFade(GroundingAnimation);
    }
	
	/// <summary>
	/// Prepare for jumping.
	/// This method is called when player holding the jump button.
	/// It does:
	/// 1. Mark the jump status to true. (To disable other player input)
	/// 2. Play prejump animation.
	/// </summary>
	public void PreJumpBegin()
	{
        predatorPlayerStatus.DisableUserMovement = true;
		animation.Stop();
		animation.CrossFade(PrejumpAnimation);
	}
	
	public void PreJumpEnd()
	{
		predatorPlayerStatus.DisableUserMovement = false;
		animation.Stop(PrejumpAnimation);
	}
	
    /// <summary>
    /// Check if the predator is facing an obstacle
    /// </summary>
    /// <param name="obstacleHeight">output the height of the obstacle</param>
    /// <param name="obstacleWidth">output the width of the obstacle</param>
    /// <param name="DistanceToObstacle">output the distance from predator to the obstacle</param>
    /// <returns></returns>
    bool CheckJumpOverObstacle(out JumpOverObstacle Obstacle)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward * JumpoverCheckDistance, out hitInfo, JumpoverCheckDistance, JumpOverObstacleLayer))
        {
            JumpOverObstacle obstacle = hitInfo.collider.GetComponent<JumpOverObstacle>();
            Obstacle = obstacle;
            return true;
        }
        else
        {
            Obstacle = null;
            return false;
        }
    }

    void GroundAtOnce()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 999, GroundLayer))
        {
            //Debug.Log("Ground at:" + hitInfo.point);
            transform.position = hitInfo.point;
        }
    }
}
