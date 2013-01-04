using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Predator3rdPersonalStealthController))]
[ExecuteInEditMode]
public class JoybuttonStealth : JoyButton {

    public GameGUIHelper.RectPosition Location = GameGUIHelper.RectPosition.BottomLeft;

    private Predator3rdPersonalStealthController StealthController;

    void Awake()
    {
        this.JoyButtonName = "Stealth_Predator";
        StealthController = GetComponent<Predator3rdPersonalStealthController>();
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

    public override void onTouchEnd(Touch touch)
    {
        base.onTouchEnd(touch);
        StealthController.ToggleStealth();
    }
    void OnGUI()
    {
        Rect r = new Rect(JoyButtonBound.x + JoyButtonBoundOffset.x,
            JoyButtonBound.y + JoyButtonBoundOffset.y,
            JoyButtonSize, JoyButtonSize);
        GUI.DrawTexture(r, ButtonTexture);
    }
}
