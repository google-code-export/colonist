using UnityEngine;
using System.Collections;

public class JoybuttonStealth : JoyButton {

    void Awake()
    {
        this.JoyButtonName = "Stealth_Predator";
    }

	// Use this for initialization
	void Start () {
	   
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
    }

}
