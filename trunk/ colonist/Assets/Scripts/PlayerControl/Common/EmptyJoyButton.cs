using UnityEngine;
using System.Collections;

/// <summary>
/// Empty joy button.
/// Draw nothing and do nothing. You can use this class as an anchor.
/// </summary>
public class EmptyJoyButton : JoyButton {
	
	void Start()
	{
		GUIBound = this.GetAdaptiveBound();
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
}
