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
	/// The Input type triggered when player tap on the button.
	/// </summary>
	public UserInputType Tap = UserInputType.Button_Left_Claw_Tap;

	public Texture HintTexture;
	public float HintRotateAngluarSpeed = 150;
	
	/// <summary>
	/// The hold detection seconds.
	/// If player hold button longer than HoldDetectionSeconds, the 
	/// </summary>
	private float HoldDetectionSeconds = 0.3333f;
	private Predator3rdPersonalAttackController attackController = null;
	private float holdStart = -1;

	private bool showHint = true;
	private float stopHintTime = 0;

	void Awake ()
	{
		attackController = this.GetComponent<Predator3rdPersonalAttackController> ();
	}

	void Start ()
	{
		GUIBound = this.GetAdaptiveBound();
	}

	void Update ()
	{
		if(showHint && Time.time > stopHintTime)
		{
			showHint = false;
		}
	}
	
	void DontHint()
	{
		
		showHint = false;
	}
	
	IEnumerator ShowButtonHints(UserInputType[] hintTypes)
	{
		yield return new WaitForEndOfFrame();
		foreach(UserInputType t in hintTypes)
		{
			if(t == this.Tap)
			{
				showHint = true;
				stopHintTime = Time.time + 3;
			}
		}
	}

	/// <summary>
	/// Call when touch.phase = Began
	/// </summary>
	/// <param name="touch"></param>
	public override void onTouchBegin (Touch touch)
	{
		base.onTouchBegin (touch);
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
	}
	
	/// <summary>
	/// Call when touch.phase = End
	/// </summary>
	/// <param name="touch"></param>
	public override void onTouchEnd (Touch touch)
	{
		base.onTouchEnd (touch);

	    UserInputData gestInfo = new UserInputData( Tap, null, this.TouchStartTime, Time.time);
	    attackController.NewUserGesture(gestInfo);		
		Debug.Log("Input was send to controller at frame:" + Time.frameCount);
	    SendMessage("DontHint");
	}

	void OnGUI ()
	{
		GUI.DrawTexture (GUIBound, ButtonTexture);
		if(showHint)
		{
			GUIUtility.RotateAroundPivot(Time.time % 360 * HintRotateAngluarSpeed, GUIBound.center);
			GUI.DrawTexture (GUIBound, HintTexture, ScaleMode.ScaleToFit,true);
		}
	}
 
}
