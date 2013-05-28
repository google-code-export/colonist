using UnityEngine;
using System.Collections;


public class DynamicGUITextData
{
	public string text = "";

	/// <summary>
	/// Text start screen position.
	/// </summary>
	public Vector2 StartScreenPosition = Vector2.zero;
	
	public Vector2 DisplayedScreenPosition = Vector2.zero;
	
	public float width = 100, height = 30;
	public int fontSize = 15;
	
	public Rect GetDisplayRect()
	{
		Rect r = new Rect(DisplayedScreenPosition.x, DisplayedScreenPosition.y, width, height);
		return r;
	}
	
	public DynamicGUITextData() { 
	}
	public DynamicGUITextData(string text) { 
		this.text = text; 
	}
	public DynamicGUITextData(string text, Vector2 screenPosition) 
	{ 
		this.text = text; 
		this.StartScreenPosition = screenPosition;
	}
}

/// <summary>
/// Display Dynamic GUI text. The text can be:
/// 1. Damage points to the unit.
/// </summary>
public class DynamicGUIText : MonoBehaviour {
	
	public GUIStyle style;
	DynamicGUITextData dynamicGUITextData = new DynamicGUITextData("- 3 5", new Vector2(Screen.width/2, Screen.height/2));
	/// <summary>
	/// Fade in-out total time length
	/// </summary>
	float fadeTimeLength = 3;
	
	/// <summary>
	/// Fading from StartColor to EndColor.
	/// </summary>
	public Color StartColor = Color.red;
	public Color EndColor = Color.white;
	
	public int StartFontSize = 20;
	public int EndFontSize = 30;
	
	/// <summary>
	/// The text fadeout screen offset.
	/// </summary>
	public Vector2 TextFadeOutOffset = new Vector2(0,-50);
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//	    if(Input.GetKeyDown(KeyCode.A))
//		{
//			SendMessage("FadeDynamicText", dynamicGUITextData);
//		}
	}
	
	IEnumerator FadeDynamicText(DynamicGUITextData textData)
	{
		
		//cache the start time
		float _startTime = Time.time;
		float fadePercentage = 0;
		while((Time.time - fadeTimeLength) <= fadeTimeLength)
		{
			fadePercentage = (Time.time - _startTime)/fadeTimeLength;
			style.normal.textColor = Color.Lerp(StartColor, EndColor, fadePercentage);
			textData.DisplayedScreenPosition = textData.StartScreenPosition + TextFadeOutOffset * fadePercentage;
			textData.fontSize = Mathf.RoundToInt(Mathf.Lerp(StartFontSize, EndFontSize, fadePercentage));
//			style.fontSize = textData.fontSize;
			yield return null;
		}
//		yield break;
	}
	
	void OnGUI()
	{
//		GUI.skin.font = textfont;
//		GUI.skin.font.material.SetColor("_Color", dynamicGUITextData.color);
		GUI.Label(dynamicGUITextData.GetDisplayRect(), dynamicGUITextData.text,style);
	}
}
