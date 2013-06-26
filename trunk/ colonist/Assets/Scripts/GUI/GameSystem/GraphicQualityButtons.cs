using UnityEngine;
using System.Collections;

/// <summary>
/// Graphic quality buttons.
/// </summary>
[ExecuteInEditMode]
public class GraphicQualityButtons : MonoBehaviour {
	
	public AdaptiveRect AnchorRect = new AdaptiveRect();
	public AdaptiveRect ConfirmedButtonRect = new AdaptiveRect();
	
	public Texture FastestTexure = null;
	public Texture FastTexture = null;
	public Texture SimpleTexture = null;
	public Texture GoodTexture = null;
	public Texture BeautifulTexture = null;
	public Texture FantasticTexture = null;
	public Texture ConfirmButtonTexture = null;
	
	int CurrentQualitySetting;
	Texture CurrentQualityTexture = null;
	Rect CurrentDisplayedBound;
	
	Rect ConfirmedButtonBound;
	
	
	const int MaxQualityLevelCount = 6;
	bool showConfirmedButton = false;
	// Use this for initialization
	void Start () {
		CurrentQualitySetting = QualitySettings.GetQualityLevel();
		ConfirmedButtonBound = ConfirmedButtonRect.GetBound();
		Debug.Log("Current Quality setting:" + CurrentQualitySetting);
	}
	
	// Update is called once per frame
	void Update () {
		CurrentQualityTexture = GetDisplayedTexture(CurrentQualitySetting);
		CurrentDisplayedBound = GetDisplayedRect(CurrentQualitySetting);
		if((Input.touchCount > 0) && Input.GetTouch(0).phase == TouchPhase.Ended &&
			(CurrentDisplayedBound.Contains(GameGUIHelper.ConvertScreenTouchCoordToGUICoord(Input.GetTouch(0).position))))
		{
			CurrentQualitySetting = (CurrentQualitySetting + 1) % MaxQualityLevelCount;
		}
		
		if((Input.touchCount > 0) && Input.GetTouch(0).phase == TouchPhase.Ended &&
			(ConfirmedButtonBound.Contains(GameGUIHelper.ConvertScreenTouchCoordToGUICoord(Input.GetTouch(0).position))))
		{
			Persistence.SetPlayerQualityLevel(CurrentQualitySetting);
		}
		
		showConfirmedButton = (CurrentQualitySetting !=  QualitySettings.GetQualityLevel());
	}
	
	Texture GetDisplayedTexture(int qualitylevel)
	{
		switch(qualitylevel)
		{
		    case 0: return FastestTexure;
			case 1: return FastTexture;
			case 2: return SimpleTexture;
			case 3: return GoodTexture;
			case 4: return BeautifulTexture;
			case 5: default:
			return FantasticTexture;
		}
	}
	
	Rect GetDisplayedRect(int qualitylevel)
	{
		Rect rect = AnchorRect.GetBound();
		switch(qualitylevel)
		{
		    case 0: return rect;
			case 1: 
			        rect.width *= 2;
			        return rect;
			case 2: 
			        rect.width *= 3;
			        return rect;
			case 3: 
			        rect.width *= 4;
			        return rect;
			case 4: 
			        rect.width *= 5;
			        return rect;
			case 5:
		    default:
			        rect.width *= 6;
			        return rect;
		}
	}
	
	void OnGUI()
	{
		GUI.DrawTexture(CurrentDisplayedBound, CurrentQualityTexture);
		if(showConfirmedButton)
			GUI.DrawTexture(ConfirmedButtonBound, ConfirmButtonTexture);
	}
}
