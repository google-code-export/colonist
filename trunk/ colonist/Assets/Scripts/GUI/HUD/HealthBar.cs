using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class HealthBar : MonoBehaviour {
	
	public Texture foregoundTexture;
	public Rect LocationRect;
	public Rect TextCoord;
	public ScaleMode scaleMode;
	public bool AplhaBlend;
	public float imageAspect = 0;
	public UnitBase unit;
	
	protected float thevalue;
	protected float theMaxValue;
	
	void Awake()
	{
		theMaxValue = unit.GetMaxHP();
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    thevalue = unit.GetCurrentHP();
	}
	
	void OnGUI()
	{
		float percentage = Mathf.Clamp01(thevalue / theMaxValue);
		
		Rect positionRect = new Rect(LocationRect);
		positionRect.width *= percentage;
		
		Rect textCoordRect = new Rect(TextCoord);
		textCoordRect.width *= percentage;		
		
		GUI.DrawTextureWithTexCoords(  positionRect, foregoundTexture, textCoordRect, AplhaBlend);
//			GUI.DrawTexture (LocationRect, foregoundTexture, scaleMode, AplhaBlend, imageAspect);
			
		
	}
}
