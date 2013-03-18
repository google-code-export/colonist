using UnityEngine;
using System.Collections;

/// <summary>
/// Stirke button - 
/// When player tap the button, 
/// UserInputData.Type = Tap will be send to attack controller.
/// 
/// When player hold the button,
/// UserInputData.Type = Hold will be send to attack controller.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Predator3rdPersonalAttackController))]
public class Joybutton_Strike_Predator : JoyButton
{
	
	/// <summary>
	/// The Input type triggered bywhenplayer tap on the button.
	/// </summary>
	public UserInputType Tap = UserInputType.Button_Left_Claw_Tap;
	/// <summary>
	/// The Input type triggered when player hold on the button.
	/// If Hold == UserInputType.None, the hold message will not be sent.
	/// </summary>
	public UserInputType Hold = UserInputType.Button_Left_Claw_Hold;
	/// <summary>
	/// The hold detection seconds.
	/// If player hold button longer than HoldDetectionSeconds, the 
	/// </summary>
	private float HoldDetectionSeconds = 0.3333f;
	private Predator3rdPersonalAttackController attackController = null;
	private float holdStart = -1;
	private bool messageSent = false;
	void Awake ()
	{
		attackController = this.GetComponent<Predator3rdPersonalAttackController> ();
	}

	void Start ()
	{
		JoyButtonBound = GameGUIHelper.GetSquareOnGUICoordinate (Location, JoyButtonSize);
	}

	void Update ()
	{
	}

	/// <summary>
	/// Call when touch.phase = Began
	/// </summary>
	/// <param name="touch"></param>
	public override void onTouchBegin (Touch touch)
	{
		base.onTouchBegin (touch);
		messageSent = false;
		holdStart = -1;
	}
	/// <summary>
	/// Call when touch.phase = Move
	/// </summary>
	/// <param name="touch"></param>
	public override void onTouchMove (Touch touch)
	{

	}
	/// <summary>
	/// Call when touch.phase = Move
	/// </summary>
	/// <param name="touch"></param>
	public override void onTouchStationary (Touch touch)
	{
		if (holdStart == -1) {
			holdStart = Time.time;
		}
		if (messageSent==false && Hold!= UserInputType.None && ((Time.time - holdStart)>=HoldDetectionSeconds))
		{
			UserInputData gestInfo = new UserInputData( Hold, null, holdStart, Time.time);
			attackController.SendMessage("NewUserGesture", gestInfo);
			Debug.Log("Hold");
			messageSent = true;
		}
	}
	/// <summary>
	/// Call when touch.phase = End
	/// </summary>
	/// <param name="touch"></param>
	public override void onTouchEnd (Touch touch)
	{
		base.onTouchEnd (touch);
        if(messageSent == false)
		{
			UserInputData gestInfo = new UserInputData( Tap, null, this.TouchStartTime, Time.time);
			attackController.SendMessage("NewUserGesture", gestInfo);			
		}
	}

	void OnGUI ()
	{
		JoyButtonBound = GameGUIHelper.GetSquareOnGUICoordinate (Location, JoyButtonSize);
		JoyButtonBound.x += JoyButtonScreenOffset.x;
		JoyButtonBound.y += JoyButtonScreenOffset.y;
		Rect r = new Rect (JoyButtonBound.x + JoyButtonRuntimeOffset.x, JoyButtonBound.y + JoyButtonRuntimeOffset.y,
            JoyButtonSize, JoyButtonSize);
		GUI.DrawTexture (r, ButtonTexture, ScaleMode.ScaleToFit,true);
	}
 
}
