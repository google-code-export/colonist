using UnityEngine;
using System.Collections;

/// <summary>
/// A fixed position RageBar, which is displayed at screen, as a HUD.
/// </summary>
[ExecuteInEditMode]
public class PredatorRageBar : HUD
{

    public Texture foregoundTexture;
    public Texture backgoundTexture;
	
	/// <summary>
	/// The rage bar location.
	/// </summary>
    public AdaptiveRect RageBarLocation = new AdaptiveRect();
	/// <summary>
	/// The max rage hint will be displayed when rage reaches max rage value.
	/// </summary>
	public AdaptiveIcon MaxRageHint = new AdaptiveIcon();
	
    public Rect TextCoord = new Rect(0,0,1,1);
    public AnimationCurve alphaCurve = new AnimationCurve();
	
	
	
    Predator3rdPersonalUnit unit;

    public Color startColor = Color.green;

    public Color endColor = Color.red;

    public bool show = true;
    bool wink = false;
    float startWinkTime;

    float thevalue;
    float currentAlpha = 0;
    private float MaxRage ;

    Color theColor = Color.gray;
    Rect RectOfHealthRealtime;
    Rect theTextCoord;
	Rect LocationRect;
	bool IsMaxRage = false;
    void Awake()
    {
        unit = transform.root.GetComponentInChildren<Predator3rdPersonalUnit>();
		MaxRage = unit.MaxRage;
		MaxRageHint.realtimeRect = MaxRageHint.adaptiveRect.GetBound();
    }

    // Update is called once per frame
    void Update()
    {
        thevalue = unit.Rage;

        float percentage = Mathf.Clamp01(thevalue / MaxRage);
        LocationRect = RageBarLocation.GetBound();
        RectOfHealthRealtime = new Rect(LocationRect);
        RectOfHealthRealtime.width *= percentage;
        theTextCoord = new Rect(TextCoord);
        theTextCoord.width *= percentage;
        theColor = Color.Lerp(startColor, endColor, percentage);
        if (Mathf.Approximately(percentage,1f))
        {
            if (!wink)
            {
                startWinkTime = Time.time;
                wink = true;
            }

            float time = Time.time - startWinkTime;
            currentAlpha = alphaCurve.Evaluate(time);
            theColor.a = currentAlpha;
			IsMaxRage = true;
        }
        else
        {
            wink = false;
			IsMaxRage = false;
        }
    }

    void OnGUI()
    {
        GUI.DrawTexture(LocationRect, backgoundTexture);
		Color oldColor = GUI.color;
        GUI.color = theColor;
        GUI.DrawTextureWithTexCoords(RectOfHealthRealtime, foregoundTexture, theTextCoord);
		
		if(IsMaxRage)
		{
			GUI.DrawTexture(MaxRageHint.realtimeRect, MaxRageHint.texture, MaxRageHint.scaleMode);
		}
		GUI.color = oldColor;
    }
}
