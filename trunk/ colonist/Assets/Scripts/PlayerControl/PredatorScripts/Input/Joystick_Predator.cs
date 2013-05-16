using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof (Predator3rdPersonMovementController))]
public class Joystick_Predator : JoyButton {
	
	public Texture ForeGroundTexture;
	public float ForeGroundTextureSize = 25;
	public float JoyButtonSize = 50;
    /// <summary>
    /// If FlexibleMode = true, joy sticker is not shown by default
    /// When user leave a finger on touchpad, sticker will dock to finger position automatically and shown
    /// </summary>
    public bool FlexibleMode;
    public ScreenOccupancy ScreenOccupancy = ScreenOccupancy.LeftScreen;
    private Predator3rdPersonMovementController PredatorMovementController = null;
	
	/// <summary>
	/// In OnGUI(), draws the Background text at FlexibleButtonRect.
	/// </summary>
    private Rect FlexibleButtonRect = new Rect();
    /// <summary>
    /// When the button being moved by the JoyButtonBoundOffset, the modifier of the output offset value
    /// </summary>
    public float ValueOffsetModifier = 20;
    void Awake()
    {
        this.JoyButtonName = "Joystick_Predator";
        PredatorMovementController = this.GetComponent<Predator3rdPersonMovementController>();
    }

    void Start()
    {
        if (FlexibleMode == true)
        {
            JoyButtonBound = Util.GetScreenOccupancy(ScreenOccupancy);
        }
        else
        {
            JoyButtonBound = this.GetAdaptiveBound();
        }
    }

    void Update()
    {
    }

    public override void onTouchBegin(Touch t)
    {
        base.onTouchBegin(t);
        FlexibleButtonRect = new Rect(t.position.x - JoyButtonSize/2, Screen.height - t.position.y-JoyButtonSize/2, JoyButtonSize, JoyButtonSize);
    }

    private void SetDirectionInCharacterRelativeMode()
    {
        Vector3 worldDirection = LevelManager.Instance.ControlDirectionPivot.TransformDirection
			(new Vector3(Joybutton_Right_Value, 0, Joybutton_Up_Value));
        PredatorMovementController.MoveDirection = worldDirection.normalized;
    }

    public override void onTouchMove(Touch t)
    {
        Vector2 direction = (t.position - TouchStartPosition).normalized;
        Joybutton_Up_Value = direction.y;
        Joybutton_Right_Value = direction.x;
        //translate input direction to world direction,
        //by LevelManager.ControlDirectionPivot
        Vector3 worldDirection = LevelManager.Instance
                                             .ControlDirectionPivot.TransformDirection(new Vector3(Joybutton_Right_Value, 0, Joybutton_Up_Value));
        PredatorMovementController.MoveDirection = worldDirection.normalized;
        JoyButtonRuntimeOffset.x = direction.x * ValueOffsetModifier;
        JoyButtonRuntimeOffset.y = -direction.y * ValueOffsetModifier;
    }

    public override void onTouchEnd(Touch t)
    {
        //Debug.Log("onTouchEnd");
        base.onTouchEnd(t);
        PredatorMovementController.MoveDirection = Vector3.zero;
        JoyButtonRuntimeOffset.x = 0;
        JoyButtonRuntimeOffset.y = 0;
    }

    public override void onTouchStationary(Touch t)
    {
        this.onTouchMove(t);
    }

    void OnGUI()
    {
        //If flexiblemode == false, draw the ButtionTexture at fix location
        if (FlexibleMode == false)
        {
            Rect backgroundRectArea = JoyButtonBound;
            GUI.DrawTexture(backgroundRectArea, ButtonTexture);

            Rect foregroundRectArea = new Rect(JoyButtonBound.center.x - ForeGroundTextureSize / 2 + JoyButtonRuntimeOffset.x,
                                               JoyButtonBound.center.y - ForeGroundTextureSize / 2 + JoyButtonRuntimeOffset.y,
                                               ForeGroundTextureSize, 
                                               ForeGroundTextureSize);
            GUI.DrawTexture(foregroundRectArea, ForeGroundTexture);
        }
        else if(FlexibleMode && hasFingerOnJoyButton)
        {
            GUI.DrawTexture(FlexibleButtonRect, ButtonTexture);
            Rect foregroundRectArea = new Rect(FlexibleButtonRect.center.x - ForeGroundTextureSize / 2 + JoyButtonRuntimeOffset.x,
                                               FlexibleButtonRect.center.y - ForeGroundTextureSize / 2 + JoyButtonRuntimeOffset.y,
                                               ForeGroundTextureSize,
                                               ForeGroundTextureSize);
            GUI.DrawTexture(foregroundRectArea, ForeGroundTexture);
        }
    }
}
