using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Combat hint.
/// The HUD at screen left top.
/// To display uesr gesture: 
/// dot, slice, tick
/// </summary>
[ExecuteInEditMode]
public class CombatHint : MonoBehaviour {
    public Texture Tap;
	public Texture Slice;
	public Texture Tick;
	
	public float width = 32;
	public float height = 32;
	
	private IList<GestureType> GestureList = new List<GestureType>();
	private int MaxCount = 4;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
    void NewHint(GestureType gestureType)
	{
		if(GestureList.Count == MaxCount)
		{
			GestureList.Clear();
		}
		GestureList.Add(gestureType);
	}
	
	
    void OnGUI()
    {
		//From left to right
		for(int i=0; i<GestureList.Count; i++)
		{
			Rect area1 = new Rect(Screen.width - width * (5 - i), 0, width, height);
			GestureType gestureType = GestureList[i];
		    GUI.DrawTexture(area1, GestureToTexture(gestureType), ScaleMode.ScaleToFit, true);
		}
	}
	
	Texture GestureToTexture(GestureType gestureType)
	{
		switch(gestureType)
		{
		case GestureType.Single_Slice:
			return Slice;
		case GestureType.Single_Curve:
			return Tick;
	    case GestureType.Single_Tap:
		default:
			return Tap;
		}
	}
}
