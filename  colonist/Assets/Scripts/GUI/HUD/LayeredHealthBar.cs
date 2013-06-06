using UnityEngine;
using System.Collections;

/// <summary>
/// LayerHealthBarGUIData wraps :
/// 1. Healthbar texture.
/// 2. Start color.
/// 3. End color.
/// </summary>
[System.Serializable]
public class LayerHealthBarGUIData
{
	public Texture foregoundTexture;
	public Color FullHealthColor = Color.white;
	public Color ZeroHealthColor = Color.red;
	
	/// <summary>
	/// The color of the realtime.
	/// </summary>
	[HideInInspector]
	public Color realtimeColor = Color.white;
	
	/// <summary>
	/// The realtime health percentage.
	/// </summary>
	[HideInInspector]
	public float realtimeHealthPercentage;
}

/// <summary>
/// LayeredHealthBar is usually applied to BOSS unit.
/// For example, there is a boss unit, he has 10,000 health point.
/// In this case, if you use normal healthbar to display the BOSS's HP, it could be a big unconfortable as his health is too large.
/// So, LayeredHealthBar is targeted for this. with LayeredHealthBar, you can define many layers, each layer's color can be different :
/// Layer 0 = 9,000 - 10,000 , color = green.
/// Layer 1 = 8,000 - 8,999 , color = grey
/// Layer 2 = 7,000 - 7,999, color = red 
/// ... etc
/// </summary>
[ExecuteInEditMode]
public class LayeredHealthBar : MonoBehaviour {
	
	/// <summary>
	/// You can define more than one LayerHealthBarGUIData here.
	/// For example, if you define 4, for a unit that has 1000 HP:
	/// 0 - 250 = Texture[0]
	/// 250 - 499 = Texture[1]
	/// 500 - 749 = Texture[2]
	/// 750 - 1000 = Texture[3]
	/// </summary>
	public LayerHealthBarGUIData[] LayerHealthBarGUIDataArray = new LayerHealthBarGUIData[]{};
	public AdaptiveIcon[] adaptiveIcons = new AdaptiveIcon[]{};
	public Texture backgoundTexture;
	public AdaptiveRect HealthBarLocation = new AdaptiveRect();
	public UnitBase unit;
	
	/// <summary>
	/// The top layered healthbar. Which will be drawed on topest layer.
	/// </summary>
	LayerHealthBarGUIData topLayeredHealthbar;
	/// <summary>
	/// The bottom layed healthbar.Which will be drawed at the bottom (Z-Axis) of topLayeredHealthbar.
	/// </summary>
	LayerHealthBarGUIData bottomLayedHealthbar;
	
	Rect LocationRect;
	Rect RectOfHealthRealtime;
	
//	public AdaptiveIcon 
	
	void Awake()
	{
		topLayeredHealthbar = LayerHealthBarGUIDataArray[LayerHealthBarGUIDataArray.Length - 1];
		bottomLayedHealthbar = LayerHealthBarGUIDataArray.Length == 1 ? topLayeredHealthbar : LayerHealthBarGUIDataArray[LayerHealthBarGUIDataArray.Length - 2];
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    float currentHP = unit.GetCurrentHP();
		float maxHP = unit.GetMaxHP();
		float HPPercentage = currentHP / maxHP;
		int TextureNumber = LayerHealthBarGUIDataArray.Length;
		float divisor = 1f / (float)TextureNumber;
		int currentForegroundTextureIndex = HPPercentage == 1 ? TextureNumber - 1 : (int)(HPPercentage * 100) / (int)(divisor * 100);
		
		topLayeredHealthbar = LayerHealthBarGUIDataArray[currentForegroundTextureIndex];
		if(currentForegroundTextureIndex != 0)
		{
			bottomLayedHealthbar = LayerHealthBarGUIDataArray[currentForegroundTextureIndex-1];
		}
		else 
		{
			bottomLayedHealthbar = null;
		}
		float divisorHP = divisor * maxHP;
		
		topLayeredHealthbar.realtimeHealthPercentage = (currentHP % divisorHP == 0) ? 1 : (currentHP % divisorHP) / divisorHP;
		
		topLayeredHealthbar.realtimeColor = Color.Lerp(LayerHealthBarGUIDataArray[currentForegroundTextureIndex].ZeroHealthColor, 
			                                                LayerHealthBarGUIDataArray[currentForegroundTextureIndex].FullHealthColor, 
			                                                topLayeredHealthbar.realtimeHealthPercentage);
		
		LocationRect = HealthBarLocation.GetBound();
		RectOfHealthRealtime = new Rect(LocationRect);
		RectOfHealthRealtime.width *= topLayeredHealthbar.realtimeHealthPercentage;
		foreach(AdaptiveIcon adaptiveIcon in adaptiveIcons)
		{
			adaptiveIcon.realtimeRect = adaptiveIcon.adaptiveRect.GetBound();
		}
		
	}
	
	void OnGUI()
	{
		GUI.DrawTexture(LocationRect, backgoundTexture);
		if(bottomLayedHealthbar != null)
		{
			GUI.color = bottomLayedHealthbar.FullHealthColor;
			GUI.DrawTexture(LocationRect, bottomLayedHealthbar.foregoundTexture);
		}
	    GUI.color = topLayeredHealthbar.realtimeColor;
		GUI.DrawTextureWithTexCoords(RectOfHealthRealtime, topLayeredHealthbar.foregoundTexture, RectOfHealthRealtime);
		//Draw icon at top
		foreach(AdaptiveIcon adaptiveIcon in adaptiveIcons)
		{
			GUI.color = adaptiveIcon.color;
			GUI.DrawTexture(adaptiveIcon.realtimeRect, adaptiveIcon.texture);
		}
	}
}

