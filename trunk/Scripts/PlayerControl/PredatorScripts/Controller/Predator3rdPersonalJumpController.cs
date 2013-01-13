using UnityEngine;
using System.Collections;

[@RequireComponent(typeof (PredatorPlayerStatus))]
[@RequireComponent(typeof(Predator3rdPersonVisualEffectController))]
public class Predator3rdPersonalJumpController : MonoBehaviour {

    public string JumpToGroundAnimation = "jumpToGround";
    public string JumpingAnimation = "jumping";
    public string PrejumpAnimation = "prejump";
    public int JumpAnimationLayer = 3;

    public float JumpOverSpeed = 10f;
    public float JumpoverCheckDistance = 3;
    
    public float ForwardJumpTime = 0.5f;
    public float ForwardJumpSpeed = 12f;
    [HideInInspector]
    public bool IsJumping = false;

    [HideInInspector]
    public bool checkJump = true;

    private CharacterController controller;
    private PredatorPlayerStatus PredatorStatus;
    private LayerMask GroundLayer;
    private LayerMask JumpOverObstacleLayer;
    private Predator3rdPersonVisualEffectController ClawEffectController;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animation[PrejumpAnimation].layer = JumpAnimationLayer;
        PredatorStatus = GetComponent<PredatorPlayerStatus>();
        ClawEffectController = GetComponent<Predator3rdPersonVisualEffectController>();
        GroundLayer = PredatorStatus.GroundLayer;
        JumpOverObstacleLayer = PredatorStatus.JumpoverObstacleLayer;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    bool JumpToGroundTrigger = false;
	float JumpValue = 0;
	Vector3 JumpDirection = Vector3.zero;
	void Update () {
	}

    void Grounding()
    {
        animation.CrossFade(JumpToGroundAnimation); 
        checkJump = false;
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

        ClawEffectController.ShowBothClawVisualEffects();

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
            yield return StartCoroutine(JumpForward());
        }
        ClawEffectController.HideBothClawTrailRenderEffect();
    }

    /// <summary>
    /// Jumping forward at speed = ForwardJumpSpeed, in ForwardJumpTime
    /// </summary>
    /// <returns></returns>
    IEnumerator JumpForward()
    {
        IsJumping = true;
        Vector3 jumpDirection = transform.forward;
        //animation.CrossFade(PrejumpAnimation);
        animation.Play(PrejumpAnimation);
        yield return new WaitForSeconds(animation[PrejumpAnimation].length);
        float _time = Time.time;
        while ((Time.time - _time) <= ForwardJumpTime)
        {
            animation.Play(JumpingAnimation);
            Vector3 jumpVelocity = jumpDirection * ForwardJumpSpeed * Time.deltaTime;
            //gravity = 9.8
            jumpVelocity.y -= 9.8f * Time.deltaTime;
            controller.Move(jumpVelocity);
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
        animation.Play(JumpToGroundAnimation);
        yield return new WaitForSeconds(animation[JumpToGroundAnimation].length);
        Debug.Log("JumpForwardEnd");
    }

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
        animation.CrossFade(JumpToGroundAnimation);
    }

    /// <summary>
    /// Move up to a height position, then move to GroundPoint, at JumpingSpeed
    /// </summary>
    /// <param name="HeightPoint"></param>
    /// <param name="GroundPoint"></param>
    /// <returns></returns>
    IEnumerator JumpOver(Vector3 HeightPoint, Vector3 GroundPoint)
    {
        //Play prejump
        IsJumping = true;
        animation.Play(PrejumpAnimation);
        yield return new WaitForSeconds(animation[PrejumpAnimation].length);
        //Before jumping up, adjust facing direction:
        transform.LookAt(new Vector3(GroundPoint.x, transform.position.y, GroundPoint.z));
        //jump to height point
        Vector3 distanceUp = HeightPoint - transform.position;
        Vector3 jumpUpVelocity = distanceUp.normalized * JumpOverSpeed;
        float overtimeUp = distanceUp.magnitude / JumpOverSpeed;
        float _time = Time.time;
        animation.CrossFade(JumpingAnimation);
        while ((Time.time - _time) <= overtimeUp)
        {
            transform.position += jumpUpVelocity * Time.deltaTime;
            yield return null;
        }
        //Then down to GroundPoint
        Vector3 distanceDown = GroundPoint - transform.position;
        Vector3 jumpDownVelocity = distanceDown.normalized * JumpOverSpeed;
        float overtimeDown = distanceDown.magnitude / JumpOverSpeed;
        _time = Time.time;
        animation.CrossFade(JumpingAnimation);
        while ((Time.time - _time) <= overtimeDown)
        {
            transform.position += jumpDownVelocity * Time.deltaTime;
            yield return null;
        }
        //In case the contoller grounding lower than terrain collider, force the position a little bit upper 
        transform.position = GroundPoint + Vector3.up * 0.3f;

        IsJumping = false;
        //Grounding animation is optional - player may skip this part 
        animation.CrossFade(JumpToGroundAnimation);
        //yield return new WaitForSeconds(animation[JumpToGroundAnimation].length);
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
            Debug.Log("Ground at:" + hitInfo.point);
            transform.position = hitInfo.point;
        }
    }
}
