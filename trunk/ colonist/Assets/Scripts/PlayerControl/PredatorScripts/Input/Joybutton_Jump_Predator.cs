using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Predator3rdPersonalJumpController))]
[RequireComponent(typeof(PredatorPlayerStatus))]
public class Joybutton_Jump_Predator : JoyButton {

    private Predator3rdPersonalJumpController JumpController;
    public Color BaseColor;
    private float LastJumpTime = 0;

    void Awake()
    {
        JumpController = this.GetComponent<Predator3rdPersonalJumpController>();
        ValueOffsetModifier = 5;
    }

	// Use this for initialization
	void Start () {
        JoyButtonBound = GameGUIHelper.GetSquareOnGUICoordinate(Location, JoyButtonSize);
        JoyButtonBound.x += JoyButtonScreenOffset.x;
        JoyButtonBound.y += JoyButtonScreenOffset.y;
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
        if (PredatorPlayerStatus.IsJumping == false)
        {
            base.onTouchBegin(touch);
        }
    }
    /// <summary>
    /// Call when touch.phase = Move
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchMove(Touch touch)
    {
    }
    /// <summary>
    /// Call when touch.phase = Move
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchStationary(Touch touch)
    {
    }

    /// <summary>
    /// Call when touch.phase = End
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchEnd(Touch touch)
    {
        base.onTouchEnd(touch);
        StartCoroutine(JumpController.Jump());
    }

    void OnGUI()
    {
        GUI.color = this.BaseColor;
		JoyButtonBound = GameGUIHelper.GetSquareOnGUICoordinate (Location, JoyButtonSize);
		JoyButtonBound.x += JoyButtonScreenOffset.x;
		JoyButtonBound.y += JoyButtonScreenOffset.y;
		Rect r = new Rect (JoyButtonBound.x + JoyButtonRuntimeOffset.x, JoyButtonBound.y + JoyButtonRuntimeOffset.y,
            JoyButtonSize, JoyButtonSize);
		GUI.DrawTexture (r, ButtonTexture);
    }
}
