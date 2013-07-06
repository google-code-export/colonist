using UnityEngine;
using System.Collections;

/// <summary>
/// Joybutton_Jump_Predator - controls the jumping input of the predator.
/// The typical jumping scenario: 
/// 1. Player press down the jump button, predator start "PreJump" animation.
/// 2. Player hold the jump button, predator keep playing "PreJump" animation.
/// 3.1 Player release finger on jump button, predator play "jumping" aniamtion and start jumping behavior.
/// 3.2 Player keep holding finger on jump button, and the time reaches %MaxHoldTime%, so, predator starts jumping.
/// 
/// The longer time player holds finger on jump button, the longer distance predator can jump forward.
/// But, for jump over obstacle, the hold finger time takes no difference.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Predator3rdPersonalJumpController))]
[RequireComponent(typeof(PredatorPlayerStatus))]
public class Joybutton_Jump_Predator : JoyButton {

    private Predator3rdPersonalJumpController JumpController;
    private float LastJumpTime = 0;
	/// <summary>
	/// The max hold time.
	/// When palyer holds finger on Jump button longer than the MaxHoldTime, the predator will jump at max power.
	/// </summary>
	public float MaxHoldTime = 0.75f;
    void Awake()
    {
        JumpController = this.GetComponent<Predator3rdPersonalJumpController>();
    }

	// Use this for initialization
	void Start () {
		GUIBound = GetAdaptiveBound();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    /// <summary>
    /// Call when touch.phase = Began
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchBegin(Touch touch)
    {
		//If IsJumping = true, means the predator is already jumping, so not allow to jump again.
        if (JumpController.IsJumping == false)
        {
            base.onTouchBegin(touch);
			//Do PreJump work.
			JumpController.PreJumpBegin();
        }
    }
    /// <summary>
    /// Call when touch.phase = Move
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchMove(Touch touch)
    {
		onTouchStationary(touch);
    }
    /// <summary>
    /// Call when touch.phase = Move
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchStationary(Touch touch)
    {
		float holdFingerTime = Time.time - this.TouchStartTime;
		if(holdFingerTime >= MaxHoldTime)
		{
			Debug.Log("Hold time > maxHoldTime! Force jumping!");
			//if player holding finger on jump button longer than MaxHoldTime, predator will jump anyway.
			this.onTouchEnd(touch);
		}
    }

    /// <summary>
    /// Call when touch.phase = End
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchEnd(Touch touch)
    {
        base.onTouchEnd(touch);
		//calculate the jump power, by holding finger time.
		float holdFingerTime = Time.time - this.TouchStartTime;
		float JumpPower = holdFingerTime / MaxHoldTime;
		//Min Power = 0.2f, Max Power = 1.
		JumpPower = Mathf.Clamp(JumpPower, 0.3f, 1);
		JumpController.PreJumpEnd();
        StartCoroutine(JumpController.Jump(JumpPower));
    }

    void OnGUI()
    {
		GUI.DrawTexture (GUIBound, ButtonTexture);
    }
}
