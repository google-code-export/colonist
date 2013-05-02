using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class HealthBar : HUD {
	
	public Texture foregoundTexture;
	public Texture backgoundTexture;
	public Rect LocationRect;
	public Rect TextCoord;
	public ScaleMode scaleMode;
	public bool AplhaBlend;

    UnitBase unit;
	
	public Color startColor = Color.green;
	
	public Color endColor = Color.red;
	
	/// <summary>
	/// The wink value.
	/// When health below 30%, the health bar will wink.
	/// </summary>
	public float winkValue = 0.3f;
	
	bool wink = false;
	public bool show = true;
	float lastChangeShowHideTime = 0;
	
	float thevalue;
	float theMaxValue;
	Color theColor = Color.gray;
	Rect theRect;
	Rect theTextCoord;
	void Awake()
	{
		unit = transform.root.GetComponentInChildren<UnitBase>();
		theMaxValue = unit.GetMaxHP();
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    thevalue = unit.GetCurrentHP();
		float percentage = Mathf.Clamp01(thevalue / theMaxValue);
		theRect = new Rect(LocationRect);
		theRect.width *= percentage;
		theTextCoord = new Rect(TextCoord);
		theTextCoord.width *= percentage;	
		theColor = Color.Lerp(endColor ,startColor, percentage);
		if(wink == false && percentage <= winkValue)
		{
			wink = true;
		}
		if(wink == true && percentage > winkValue)
		{
			wink = false;
			show = true;
		}
		if(wink)
		{
			if((Time.time - lastChangeShowHideTime)>=0.1f)
			{
//				Debug.Log("winking:"+Time.frameCount);
				show = !show;
				lastChangeShowHideTime = Time.time;
			}
		}
	}
	
    IEnumerator ApplyDamage(DamageParameter param)
	{
		yield return new WaitForEndOfFrame();
		GameEvent _e = new GameEvent(GameEventType.DisplayDamageParameterOnPlayer);
		_e.ObjectParameter = param;
		_e.Vector2Parameter = new Vector2(theRect.x + theRect.width, theRect.y + theRect.height);
		//send gameEvent to display player damage number
 		SendMessage("OnGameEvent", _e);
	}
	
	float lastHideTime = 0;
	void OnGUI()
	{
		GUI.DrawTexture( LocationRect, backgoundTexture,scaleMode, AplhaBlend);
		if(show)
		{
		  GUI.color = theColor;
		  GUI.DrawTextureWithTexCoords(  theRect, foregoundTexture, theTextCoord, AplhaBlend);
//			GUI.DrawTexture (LocationRect, foregoundTexture, scaleMode, AplhaBlend, imageAspect);
		}	
		
	}
}
