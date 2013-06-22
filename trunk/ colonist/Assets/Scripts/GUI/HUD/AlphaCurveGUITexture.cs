using UnityEngine;
using System.Collections;
/// <summary>
/// Dynamic GUI texture.
/// Use a curve to control the alpha when drawing texture, so the texture is shown/hidden repeatly.
/// </summary>
public class AlphaCurveGUITexture : MonoBehaviour {
	
	public AdaptiveRect adaptiveRect = new AdaptiveRect();
	public AnimationCurve alphaCurve = new AnimationCurve();
	public Texture texture = null;
	/// <summary>
	/// If auto redraw is true, the texture will automatically redraw when out of the alphaCurve's time range,
	/// call Stop() to stop drawing.
	/// </summary>
	public bool AutoRedraw = false;
	bool shouldDrawTexture = false;
	float currentAlpha = 0;
	float startDrawTime = 0;
	Rect drawRect = new Rect();
	
	// Use this for initialization
	void Start () {
	
	}
	
	public void Draw()
	{
		shouldDrawTexture = true;
		startDrawTime = Time.time;
	}
	
	public void Stop()
	{
		shouldDrawTexture = false;
	}
	
	// Update is called once per frame
	void Update () {
	    if(shouldDrawTexture)
		{
		   drawRect = adaptiveRect.GetBound();
		   float time = Time.time - startDrawTime;
			//if time expires and auto-redraw is false
		   if(time > Util.GetCurveMaxTime(alphaCurve))
		   {
			  if(AutoRedraw)
			  {
			     time = Mathf.Repeat(time, Util.GetCurveMaxTime(alphaCurve));
			  }
			  else 
			  {
				 //stop drawing if time expires.
				 shouldDrawTexture = false;
				 return;
			  }
		   }			
		   currentAlpha = alphaCurve.Evaluate(time);
		}
	}
	
	void OnGUI()
	{
		if(shouldDrawTexture)
		{
		    float oldAlpha = GUI.color.a;
		    GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, currentAlpha);
		    GUI.DrawTexture(drawRect, texture);
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, oldAlpha);
		}
	}
}
