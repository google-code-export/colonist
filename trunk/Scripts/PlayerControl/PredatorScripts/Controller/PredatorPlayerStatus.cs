using UnityEngine;
using System.Collections;



/// <summary>
/// A class used for its static member to expose current status of the predator protagonist
/// </summary>
[@RequireComponent(typeof(Predator3rdPersonMovementController))]
[@RequireComponent(typeof(Predator3rdPersonalAttackController))]
[@RequireComponent(typeof(Predator3rdPersonalFetchController))]
public class PredatorPlayerStatus : MonoBehaviour{

    public MovementControlMode PlayerControlMode = MovementControlMode.CharacterRelative;

    /// <summary>
    /// Assign the predator bone reference at this beheavior,
    /// </summary>
    public Transform root = null;
    public Transform pelvis = null;
    public Transform spine = null;
    public Transform body = null;
    public Transform frontLeftUpperLeg = null;
    public Transform frontRightUpperLeg = null;
    public Transform rearLeftUpperLeg = null;
    public Transform rearRightUpperLeg = null;
    public Transform leftUpperClaw = null;
    public Transform rightUpperClaw = null;

    public static bool isBusy
    {
        get
        {
            return isFetching || isAttacking || isMoving;
        }
    }
    private static bool isFetching = false;
    private static bool isAttacking = false;
    private static bool isMoving = false;

    public static bool IsFetching
    {
        get
        {
            return isFetching;
        }
    }

    public static bool IsAttacking
    {
        get
        {
            return isAttacking;
        }
    }

    public static bool IsMoving
    {
        get
        {
            return isMoving;
        }
    }

    private Predator3rdPersonMovementController movementController;
    private Predator3rdPersonalAttackController attackController;
    private Predator3rdPersonalFetchController fetchController;

    void Awake()
    {
        movementController = this.GetComponent<Predator3rdPersonMovementController>();
        attackController = this.GetComponent<Predator3rdPersonalAttackController>();
        fetchController = this.GetComponent<Predator3rdPersonalFetchController>();
    }

    private void UpdateStatus()
    {
        isAttacking = attackController.IsPlayingAttack() || fetchController.isPlayingFetchAnimation();
        isMoving =!(Mathf.Approximately(movementController.MoveForwardModifier, 0) &&
                   Mathf.Approximately(movementController.MoveRightModifier, 0) &&
                   Mathf.Approximately(movementController.RotateRightModifier, 0));
        isFetching = fetchController.HasFetchSomething;
    }

    void FixedUpdate()
    {
        UpdateStatus();
    }
}
