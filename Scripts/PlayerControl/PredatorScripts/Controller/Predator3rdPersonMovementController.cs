using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PredatorPlayerStatus))]
[RequireComponent(typeof(Predator3rdPersonalJumpController))]
public class Predator3rdPersonMovementController : MonoBehaviour {

    public Camera workingCamera = null;
    public float ForwardMovingSpeed = 3f;
    public float BackwardMovingSpeed = 1.5f;
    public float RightwardMovingSpeed = 2f;
    public float RotationAnglarSpeed = 10;

    public string idleAnimation = "idle";
    public string WalkAnimation = "walk";
    public string WalkRightwardAnimation = "rightward";
    public string WalkLeftwardAnimation = "leftward";
    public string WalkRightForwardAnimation = "right_forward";
    public string WalkLeftForwardAnimation = "left_forward";
    public string WalkBackwardAnimation = "walkbackward";
    public string SlowWalkAnimation = "slowwalk";

    /// <summary>
    /// MoveForwardModifier, MoveRightModifier & RotateRightModifier
    /// are used in CameraRelative mode(CS style)
    /// </summary>
    [HideInInspector]
    public float MoveForwardModifier = 0;
    [HideInInspector]
    public float MoveRightModifier = 0;
    [HideInInspector]
    public float RotateRightModifier = 0;

	
    /// <summary>
    /// MoveDirection is used CharacterRelative mode(diablo style)
    /// </summary>
    [HideInInspector]
    public Vector3 MoveDirection_CharacterRelative = new Vector3();

    private CharacterController characterController = null;

    private string[] movementAnimation = null;
    private string[] rotateAnimation = null;

    private int movementAnimationLayer = 0;
    private PredatorPlayerStatus playerStatus = null;
    private MovementControlMode MovementControlMode = MovementControlMode.CameraRelative;
    private Predator3rdPersonalJumpController JumpController;
	/// <summary>
	/// The deceleration factor. When DecelerationFactor > 0, the movement speed = speed * (1-DecelerationFactor)
	/// </summary>
	private float DecelerationFactor = 0;
		/// <summary>
	/// The acceleration factor. When DecelerationFactor > 0, DecelerationFactor will be decreased by accelerationFactor per second
	/// </summary>
	private float accelerationFactor = 0.25f;
	
    private void InitAnimation()
    {
        InitWalkAnimation();
    }
	
    private void InitWalkAnimation()
    {
        animation[idleAnimation].layer = -1;
        animation[idleAnimation].AddMixingTransform(GetComponent<PredatorPlayerStatus>().body);
        movementAnimation = new string[]
        {
            WalkAnimation,WalkBackwardAnimation,WalkLeftwardAnimation,WalkRightwardAnimation,
            WalkLeftForwardAnimation, WalkRightForwardAnimation
        };
  
        foreach (string ani in movementAnimation)
        {
            animation[ani].layer = movementAnimationLayer;
        }
        animation.SyncLayer(movementAnimationLayer);
    }

	// Use this for initialization
	void Awake () {
        characterController = this.GetComponent<CharacterController>();
        JumpController = GetComponent<Predator3rdPersonalJumpController>();
        //Use the main camera by default
        if (workingCamera == null)
        {
            workingCamera = Camera.main;
        }
        InitAnimation();
        playerStatus = GetComponent<PredatorPlayerStatus>();
        MovementControlMode = playerStatus.PlayerControlMode;
	}

    void Start()
    {
        //SendMessage("TestWalk");
    }

    void FixedUpdate()
    {

    }

	// Update is called once per frame
	void Update () {
        //If predator is attacking, stop moving
        //and DisableUserMovement must be false  to allow user commanding the movement
        if (PredatorPlayerStatus.IsJumping==false && PredatorPlayerStatus.IsAttacking == false && playerStatus.DisableUserMovement == false)
        {
            switch (MovementControlMode)
            {
                    //C-S Style:
                case MovementControlMode.CameraRelative:
                    CameraRelativeMoving();
                    Rotating();
                    AnimateWalking_CameraRelative();
                    break;
                    //Diablo style:
                case MovementControlMode.CharacterRelative:
                    CharacterRelativeMoving();
                    AnimateWalking_CharacterRelative();
                    break;
            }
        }

		//if DecelerationFactor > 0, gradually decrease it
		if(DecelerationFactor > 0)
		{
			DecelerationFactor -= accelerationFactor * Time.deltaTime;
			//Debug.Log("DecelerationFactor:" + DecelerationFactor);
			if(DecelerationFactor < 0)
				DecelerationFactor = 0;
		}
	}
	
	void Decelerate(float decelerationFactor)
	{
		DecelerationFactor = decelerationFactor;
	}
	
    void CharacterRelativeMoving()
    {
        if (Mathf.Approximately(MoveDirection_CharacterRelative.magnitude, 0))
        {
			return;
        }
        //Vector3 direction = Vector3.forward * MoveForwardModifier + Vector3.right * MoveRightModifier;
        //direction = workingCamera.transform.TransformDirection(direction);
        //direction.y = 0;
        Vector3 direction = MoveDirection_CharacterRelative.normalized;
        direction.y = 0;
        Util.MoveTowards(transform, transform.position + direction, characterController, true, true, ForwardMovingSpeed * (1 - DecelerationFactor), RotationAnglarSpeed);
        //characterController.SimpleMove(direction * 1 * (1 - DecelerationFactor));
    }

    void CameraRelativeMoving()
    {
        Vector3 forwardVelocity = new Vector3(0, 0, 0);
        Vector3 rightwardVelocity = new Vector3(0, 0, 0);
        Vector3 totalVelocity = new Vector3();
        if (MoveForwardModifier > 0)
        {
            Vector3 forward = workingCamera.transform.forward;
            forward.y = 0;
            forward *= Mathf.Abs(MoveForwardModifier);
            forwardVelocity += forward;        
        }
        else if (MoveForwardModifier < 0)
        {
            Vector3 backward = workingCamera.transform.TransformDirection(Vector3.back);
            backward.y = 0;
            backward *= Mathf.Abs(MoveForwardModifier);
            forwardVelocity += backward;//new Vector3(0, 0, MoveForwardModifier * MovingSpeed);
        }
        if (MoveRightModifier > 0)
        {
            Vector3 right = workingCamera.transform.right;
            right.y = 0;
            right *= Mathf.Abs(MoveRightModifier );
            rightwardVelocity += right;//new Vector3(MoveRightModifier * MovingSpeed, 0, 0);
        }
        else if (MoveRightModifier < 0)
        {
            Vector3 left = workingCamera.transform.TransformDirection(Vector3.left);
            left.y = 0;
            left *= Mathf.Abs(MoveRightModifier);
            rightwardVelocity += left;//new Vector3(MoveRightModifier * MovingSpeed, 0, 0);
        }
 
        if ( MoveForwardModifier != 0 || MoveRightModifier != 0)
        {
           // transform.LookAt(this.transform.position + velocity * 5);
            forwardVelocity = MoveForwardModifier > 0 ? forwardVelocity.normalized * ForwardMovingSpeed
                                                      : forwardVelocity.normalized * BackwardMovingSpeed; 
            rightwardVelocity = rightwardVelocity.normalized * RightwardMovingSpeed;
            totalVelocity = forwardVelocity + rightwardVelocity;
            //Debug.Log("Moving speed :" + velocity.magnitude + " Moving speed:" + MovingSpeed + " deltaTime:" + Time.deltaTime + " Frame:" + Time.frameCount);
            characterController.SimpleMove(totalVelocity);
            //PredatorPlayerStatus.isMoving = true;
        }
    }

    void Rotating()
    {
        if (!Mathf.Approximately(0, RotateRightModifier))
        {
            transform.RotateAroundLocal(Vector3.up, RotateRightModifier * Time.deltaTime);
        }
    }

    void OnPauseWalking()
    {
        if (PredatorPlayerStatus.IsMoving == false)
        {
            animation.Stop(WalkAnimation);
        }
    }

    void AnimateWalking_CharacterRelative()
    {
        if (Mathf.Approximately(0, MoveDirection_CharacterRelative.magnitude))
        {
            return;
        }
        animation.CrossFade(WalkAnimation);
    }

    /// <summary>
    /// AnimateWalking_CameraRelative() is much more complicate than 
    /// AnimateWalking_CharacterRelative() 
    /// It's like a C-S style moving animation, has move-left , move-right, move-back..etc
    /// </summary>
    void AnimateWalking_CameraRelative()
    {
        if (Mathf.Approximately(0, MoveForwardModifier) && Mathf.Approximately(0, MoveRightModifier) && Mathf.Approximately(0, RotateRightModifier))
        {
            return;
        }
        string _ani = "";
        //Right/Left
        if (Mathf.Approximately(0, MoveForwardModifier) && !Mathf.Approximately(0, MoveRightModifier))
        {
            _ani = MoveRightModifier > 0 ? WalkRightwardAnimation : WalkLeftwardAnimation;
        }
        //Forward/Backward
        else if (!Mathf.Approximately(0, MoveForwardModifier) && Mathf.Approximately(0, MoveRightModifier))
        {
            _ani = MoveForwardModifier > 0 ? WalkAnimation : WalkBackwardAnimation;
        }
        //Right-Forward
        else if (MoveForwardModifier > 0 && MoveRightModifier > 0)
        {
            _ani = WalkRightForwardAnimation;
        }
        //Left-Forward
        else if (MoveForwardModifier > 0 && MoveRightModifier < 0)
        {
            _ani = WalkLeftForwardAnimation;
        }
        //Rotate(last priority):
        else if (!Mathf.Approximately(0, RotateRightModifier))
        {
            _ani = WalkAnimation;
        }
        //Back_left or back_right
        else
        {
            _ani = WalkBackwardAnimation;
        }
        //Debug.Log("Ani:" + _ani);
        animation.CrossFade(_ani, 0.1f);
    }
}
