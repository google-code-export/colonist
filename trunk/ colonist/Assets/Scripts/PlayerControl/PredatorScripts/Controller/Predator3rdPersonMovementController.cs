using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PredatorPlayerStatus))]
[RequireComponent(typeof (Predator3rdPersonalUnit))]
public class Predator3rdPersonMovementController : MonoBehaviour {

    /// <summary>
    /// MoveDirection is used CharacterRelative mode(diablo style)
    /// </summary>
    [HideInInspector]
    public Vector3 MoveDirection = new Vector3();

    private CharacterController characterController = null;

    private string[] movementAnimation = null;
    private string[] rotateAnimation = null;

    private int movementAnimationLayer = 0;
    private PredatorPlayerStatus playerStatus = null;
	/// <summary>
	/// The deceleration factor. When DecelerationFactor > 0, the movement speed = speed * (1-DecelerationFactor)
	/// </summary>
	private float DecelerationFactor = 0;
		/// <summary>
	/// The acceleration factor. When DecelerationFactor > 0, DecelerationFactor will be decreased by accelerationFactor per second
	/// </summary>
	private float accelerationFactor = 0.25f;

    private Predator3rdPersonalUnit PredatorPlayerUnit;

	// Use this for initialization
	void Awake () {
        characterController = this.GetComponent<CharacterController>();
        PredatorPlayerUnit = this.GetComponent<Predator3rdPersonalUnit>();
        playerStatus = GetComponent<PredatorPlayerStatus>();
	}

	// Update is called once per frame
	void Update () {
        //If predator is attacking, stop moving
        //and DisableUserMovement must be false  to allow user commanding the movement
        if (PredatorPlayerStatus.IsJumping == false && 
            PredatorPlayerStatus.IsAttacking == false && 
            playerStatus.DisableUserMovement == false)
        {
            CharacterRelativeMoving();
            AnimateWalking_CharacterRelative();
        }
//		Debug.Log("Is Jumping:" + PredatorPlayerStatus.IsJumping + " IsAttacking:" + PredatorPlayerStatus.IsAttacking + " playerStatus:" + playerStatus.DisableUserMovement + " IsJumping:" + GetComponent<Predator3rdPersonalJumpController>().IsJumping);
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
        if (Mathf.Approximately(MoveDirection.magnitude, 0))
        {
            return;
        }
        Vector3 direction = MoveDirection.normalized;
        direction.y = 0;
        Util.MoveTowards(transform, transform.position + direction,
                         characterController, true, true,
                         PredatorPlayerUnit.MoveData.MoveSpeed * (1 - DecelerationFactor),
                         PredatorPlayerUnit.MoveData.RotateAngularSpeed);
    }

    void AnimateWalking_CharacterRelative()
    {
        if (Mathf.Approximately(0, MoveDirection.magnitude))
        {
            return;
        }
        animation.CrossFade(PredatorPlayerUnit.MoveData.AnimationName);
    }

}
