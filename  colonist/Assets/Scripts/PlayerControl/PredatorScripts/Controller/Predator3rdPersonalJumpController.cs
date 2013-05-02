using UnityEngine;
using System.Collections;


[@RequireComponent(typeof(Predator3rdPersonVisualEffectController))]
[RequireComponent(typeof(Predator3rdPersonalUnit))]
public class Predator3rdPersonalJumpController : MonoBehaviour {

    [HideInInspector]
    public bool IsJumping = false;

    [HideInInspector]
    public bool checkJump = true;

    private float ForwardJumpTime = 0.5f;
    private float ForwardJumpSpeed = 12f;
    private float JumpOverSpeed = 10f;
    private float JumpoverCheckDistance = 3;
    private string JumpingAnimation = "jumping";
    private string GroundingAnimation = "jumpToGround";
    private string PrejumpAnimation = "prejump";

    private CharacterController controller;
    private LayerMask GroundLayer;
    private LayerMask JumpOverObstacleLayer;
//    private Predator3rdPersonVisualEffectController ClawEffectController;
    private Predator3rdPersonalUnit PredatorPlayerUnit = null;
	
	public float JumpForwardMaxHeight = 5;
	
	
    void Awake()
    {
        controller = GetComponent<CharacterController>();
//        ClawEffectController = GetComponent<Predator3rdPersonVisualEffectController>();
        PredatorPlayerUnit = this.GetComponent<Predator3rdPersonalUnit>();

        GroundLayer = PredatorPlayerUnit.GroundLayer;
        JumpOverObstacleLayer = PredatorPlayerUnit.JumpData.ObstacleToJumpOver;

        PrejumpAnimation = PredatorPlayerUnit.JumpData.PreJumpAnimation;
        GroundingAnimation = PredatorPlayerUnit.JumpData.GroundingAnimation;
        JumpingAnimation = PredatorPlayerUnit.JumpData.JumpingAnimation;

        JumpOverSpeed = PredatorPlayerUnit.JumpData.JumpOverSpeed;
        JumpoverCheckDistance = PredatorPlayerUnit.JumpData.JumpOverCheckDistance;

        ForwardJumpTime = PredatorPlayerUnit.JumpData.JumpForwardTime;
        ForwardJumpSpeed = PredatorPlayerUnit.JumpData.JumpForwardSpeed;
    }

    /// <summary>
    /// Trigger a Jump beheavior.
    /// if there is a jump over obstacle ahead, jump over it
    /// else , jump forward.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Jump()
    {
        JumpOverObstacle obstacle = null;
        bool HasObstacle = CheckJumpOverObstacle(out obstacle);

//        ClawEffectController.ShowBothClawVisualEffects();

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
            yield return StartCoroutine(JumpStraightForward());
			
        }
//        ClawEffectController.HideBothClawTrailRenderEffect();
    }

    /// <summary>
    /// Jumping straight forward at speed = ForwardJumpSpeed, in ForwardJumpTime
    /// </summary>
    /// <returns></returns>
    IEnumerator JumpStraightForward()
    {
        IsJumping = true;
        Vector3 jumpDirection = transform.forward;
        //animation.CrossFade(PrejumpAnimation);
        animation.Play(PrejumpAnimation);
        yield return new WaitForSeconds(animation[PrejumpAnimation].length);
        float _time = Time.time;
		
		//rising time = 1/2 of ForwardJumpTime
		float RisingTime = ForwardJumpTime * 0.5f;
		float gravity = (float)(this.JumpForwardMaxHeight / (1.5f * RisingTime * RisingTime));
		float upwardInitalSpeed = gravity * RisingTime;
		
		Vector3 jumpForwardVelocity = jumpDirection * ForwardJumpSpeed;
		jumpForwardVelocity.y = 0;
		Vector3 upwardVelocity = new Vector3(0, upwardInitalSpeed, 0);
        while ((Time.time - _time) <= ForwardJumpTime)
        {
            animation.Play(JumpingAnimation);
//            Vector3 jumpVelocity = jumpDirection * ForwardJumpSpeed * Time.deltaTime;
            upwardVelocity.y -= gravity * Time.deltaTime; 
			Vector3 forwardVelocity = jumpForwardVelocity * Time.deltaTime + upwardVelocity;
			
//            controller.Move(jumpVelocity);
			controller.Move(forwardVelocity);
			
//			transform.position -= new Vector3(0, gravity * Time.deltaTime, 0);
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
