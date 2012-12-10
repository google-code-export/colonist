using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Predator3rdPersonalAttackController))]
public class GestureHandler_Predator : JoyButton {

    public ThirdPersonFollowCamera_Predator camera_controller = null;
	/// <summary>
	/// This list stores the touches during one user touch
	/// </summary>
	private IList<Touch> OneOffTouchHistory = new List<Touch>();
	private Predator3rdPersonalAttackController attackController;
 	// Use this for initialization
	void Awake () {
        //GestureHandler covers the full screen size, so to detect every touch that not own by other joybutton 
        JoyButtonBound = new Rect(0, 0, Screen.width, Screen.height);
		this.JoyButtonName = "GestureHandler";
		attackController = GetComponent<Predator3rdPersonalAttackController>();
 	}
	
	// Update is called once per frame
	void Update () {
	
	}
	int touchStartFrame = -1;
    /// <summary>
    /// Call when touch.phase = Began
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchBegin(Touch touch)
    {
        base.onTouchBegin(touch);
		touchStartFrame=Time.frameCount;
//		Debug.Log("touchStartFrame:" + touchStartFrame);
		OneOffTouchHistory.Add(touch);
    }

	bool isTickGesture = false;
    /// <summary>
    /// Call when touch.phase = Move
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchMove(Touch touch)
    {
       OneOffTouchHistory.Add(touch);
    }
    /// <summary>
    /// Call when touch.phase = Move
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchStationary(Touch touch)
	{
	}

    /// <summary>
    /// In two finger case, one finger end means whole gesture ends.
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchEnd(Touch touch)
    {
        GestureInfomation gestInfo = ParseSingleTouchGesture(touch);
        attackController.SendMessage("NewUserGesture", gestInfo);
		//attackController.SendMessage("ProcessUserGesture", gestInfo);
		base.onTouchEnd(touch);
		OneOffTouchHistory.Clear();
    }
	
	private bool CheckTick()
	{
		bool ret = false;
		//we need at least three touch to judge tick gesture
		if(OneOffTouchHistory.Count>=3)
		{
		   //Get direction1: second touch <-> first touch
		   Touch touch0 = OneOffTouchHistory[0];
		   Touch touch1 = OneOffTouchHistory[1];
		   Vector2 directionOfFirstMove = touch1.position - touch0.position;
		   //Get direction2: final touch <-> first touch
		   Touch touchZ = OneOffTouchHistory[OneOffTouchHistory.Count-1];
		   Vector2 directionOfFinal = touchZ.position - touch0.position;
		   float angle = Vector2.Angle(directionOfFirstMove, directionOfFinal);
		   ret = angle >=30;
		}
		return ret;
	}

    private GestureInfomation ParseSingleTouchGesture(Touch endTouch)
    {
        float SliceDistance = Vector2.Distance(endTouch.position, this.TouchStartPosition);
        GestureInfomation gestureInfo = null;
        //Tap
        if (Mathf.Approximately(SliceDistance, 0))
        {
            gestureInfo = new GestureInfomation(GestureType.Single_Tap, null, this.TouchStartTime, Time.time);
        }
        //Curve
        else if(CheckTick() == true)
        {
            float _timeDis = Time.time - this.TouchStartTime;
            gestureInfo = new GestureInfomation(GestureType.Single_Curve, null,
                                                this.TouchStartTime, Time.time);
        }
		//Slice
		else 
		{
            float _timeDis = Time.time - this.TouchStartTime;
            float strength = SliceDistance / _timeDis;
            Vector2 sliceDirection = (endTouch.position - TouchStartPosition).normalized;
            gestureInfo = new GestureInfomation(GestureType.Single_Slice, sliceDirection,
                                                this.TouchStartTime, Time.time);
		}
        return gestureInfo;
    }



}
