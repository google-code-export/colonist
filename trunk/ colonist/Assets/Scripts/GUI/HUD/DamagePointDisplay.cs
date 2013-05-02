using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Behaves to display damage texture.
/// </summary>
public class DamagePointDisplay : MonoBehaviour, I_GameEventReceiver
{
	/// <summary>
	/// Defines an array of fade setting.
	/// </summary>
	public DynamicNumberTextureFadeSetting[] FadeSettingArray;
	
	/// <summary>
	/// GUITextureDataList is populated at runtime, when a dynamic floating number being display on screen, it's actually a record in this list.
	/// </summary>
	IList<DynamicGUINumberTextureData> GUITextureDataList = new List<DynamicGUINumberTextureData> ();
	
	/// <summary>
	/// When NPC receives a normal damage from Player, the name of DynamicNumberTextureFadeSetting will be used.
	/// </summary>
	public string NormalNPCDamagePointFadeSettingName = "";
	DynamicNumberTextureFadeSetting NormalNPCDamagePointFadeSetting;
	
	/// <summary>
	/// When NPC receives a critical damage from Player, the name of CritcalNPCDamagePointFadeSettingName will be used.
	/// </summary>
	public string CritcalNPCDamagePointFadeSettingName = "";
	DynamicNumberTextureFadeSetting CritcalNPCDamagePointFadeSetting;
	
	/// <summary>
	/// When Player receives a damage, the name of PlayerDamagePointFadeSettingName will be used.
	/// </summary>
	public string PlayerDamagePointFadeSettingName = "";
	DynamicNumberTextureFadeSetting PlayerDamagePointFadeSetting;
	
	void Awake ()
	{
		foreach (DynamicNumberTextureFadeSetting setting in this.FadeSettingArray) {
			setting.InitializeFadeSetting ();
			if(setting.SettingName == NormalNPCDamagePointFadeSettingName)
			{
				NormalNPCDamagePointFadeSetting = setting;
			}
			if(setting.SettingName == CritcalNPCDamagePointFadeSettingName)
			{
				CritcalNPCDamagePointFadeSetting = setting;
			}
			if(setting.SettingName == PlayerDamagePointFadeSettingName)
			{
				PlayerDamagePointFadeSetting = setting;
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
        for(int i=0; i<GUITextureDataList.Count; i++)
		{
			if(GUITextureDataList[i].Update() == false)
			{
				GUITextureDataList.RemoveAt(i);
			}
		}
	}
	
	/// <summary>
	/// Call this function to display magical number in given Screen Position.
	/// </summary>
	public void ShowNumber (Vector2 GUIPosition, int Number, DynamicNumberTextureFadeSetting setting)
	{
		Texture2D runtimeTexture = setting.GetNumberTexture (Number);
		DynamicGUINumberTextureData dynamicGUITextureData = new DynamicGUINumberTextureData (runtimeTexture, 
			                                                                     GUIPosition, setting);
		GUITextureDataList.Add (dynamicGUITextureData);
	}
	
	/// <summary>
	/// Shows the number to display magical number over game object.
	/// </summary>
	public void ShowNumberOverGameObject (GameObject gameObject, int Number, DynamicNumberTextureFadeSetting setting)
	{
		Vector3 worldPos = gameObject.transform.position;
		CharacterController controller = gameObject.GetComponent<CharacterController> ();
		if (controller != null) {
			worldPos += new Vector3 (controller.center.x, controller.center.y + controller.height / 2, controller.center.z);
		}
		Camera camera = Camera.allCameras [0];
		Vector3 screenPos = camera.WorldToScreenPoint (worldPos);
		screenPos = GameGUIHelper.ConvertScreenTouchCoordToGUICoord (new Vector2 (screenPos.x, screenPos.y));
		screenPos.y -= 10; //fixed 10 offset
		ShowNumber (screenPos, Number, setting);
	}
	
	/// <summary>
	/// Handles the display damageparameter gameevent. Display damage on NPC or player.
	/// For Player, the start screen position is given in gameEvent.ObjectParameter as Vector2.
	/// For NPC, the DamageParameter is given in gameEvent.ObjectParameter as DamageParameter.
	/// </summary>
	public void OnGameEvent (GameEvent _e)
	{
		DamageParameter dp = _e.ObjectParameter as DamageParameter;
		switch (_e.type) {
		case GameEventType.DisplayDamageParameterOnNPC:
			dp = _e.ObjectParameter as DamageParameter;
			ShowNumberOverGameObject (_e.receiver, Mathf.RoundToInt (dp.damagePoint), NormalNPCDamagePointFadeSetting);
			break;
		case GameEventType.DisplayDamageParameterOnPlayer:
			Vector2 GUIPosition = _e.Vector2Parameter;
			ShowNumber (GUIPosition, Mathf.RoundToInt (dp.damagePoint), PlayerDamagePointFadeSetting);
			break;
		}
	}
	
	void OnGUI ()
	{
		if (GUITextureDataList.Count > 0) {
			foreach (DynamicGUINumberTextureData dynamicTextureData in GUITextureDataList) {
				Color _c = GUI.color;
				GUI.color = dynamicTextureData.DisplayColor;
				GUI.DrawTexture (dynamicTextureData.DisplayRect, dynamicTextureData.texture);
				GUI.color = _c;
			}
		}
	}
}

/// <summary>
/// DynamicGUITextureData represents a dynamic floating number texture on GUI screen.
/// The data is controlled by DynamicNumberTextureFadeSetting, behavior is controlled by DamagePointDisplay
/// </summary>
public class DynamicGUINumberTextureData
{
	public Texture2D texture = null;

	/// <summary>
	/// Text start screen position.
	/// </summary>
	public Vector2 StartScreenPosition = Vector2.zero;
	public Rect DisplayRect;
	public Color DisplayColor;
	public float StartTime;
	public Vector2 screenPositionOffsetStart;
	public Vector2 screenPositionOffsetEnd;
	DynamicNumberTextureFadeSetting FadeSetting;
	
	/// <summary>
	/// Constuctor
	/// </summary>
	/// <param name='texture'>
	/// Texture.
	/// </param>
	/// <param name='screenPosition'>
	/// Screen position where the texture start floating.
	/// </param>
	/// <param name='fromScreenPosition'>
	/// From screen position.
	/// </param>
	/// <param name='toScreenPosition'>
	/// To screen position.
	/// </param>
	public DynamicGUINumberTextureData (Texture2D texture, 
		                                Vector2 screenPosition, 
		                                DynamicNumberTextureFadeSetting fadeSetting)
	{ 
		this.texture = texture; 
		this.StartScreenPosition = screenPosition;
		DisplayRect = new Rect (0, 0, 0, 0);
		StartTime = Time.time;
		this.screenPositionOffsetStart = fadeSetting.screenOffset.GetValueAtPercentage(0);
		this.screenPositionOffsetEnd = fadeSetting.screenOffset.GetValueAtPercentage(1);
		this.FadeSetting = fadeSetting;
	}
	
	/// <summary>
	/// Update this DynamicGUINumberTextureData instance.
	/// Call this function per frame.
	/// If return false, means this instance has been expired and should be removed from memory.
	/// </summary>
	public bool Update ()
	{
		if ((Time.time - this.StartTime) > FadeSetting.TimeLength) {
			return false;
		} 
		else {
			float pasttime = Time.time - this.StartTime;
			float percentage = pasttime / this.FadeSetting.TimeLength;
			//calculate the left-top position
			Vector2 screenOffset = Vector2.Lerp (this.screenPositionOffsetStart, this.screenPositionOffsetEnd, percentage);
			this.DisplayRect.x = this.StartScreenPosition.x + screenOffset.x;
			this.DisplayRect.y = this.StartScreenPosition.y + screenOffset.y;
			//calculate the wide-height size
			float imageSizeRate = FadeSetting.sizeExpandSetting.GetValueAtPercentage (percentage);
			this.DisplayRect.width = this.texture.width * imageSizeRate;
			this.DisplayRect.height = this.texture.height * imageSizeRate;
			//centerize the texture screen position
			this.DisplayRect.x -= this.DisplayRect.width / 2;
			//calculate the color
			this.DisplayColor = FadeSetting.colorCurve.GetValueAtPercentage (percentage);
			return true;
		}
	}
}

/// <summary>
/// Define the data of how to fade the DynamicGUINumberTextureData.
/// Includes:
/// 1. Fade in-out time.
/// 2. Fade in-out color.
/// 3. Fade in-out screen offset.
/// 4. Fade in-out size.
/// </summary>
[System.Serializable]
public class DynamicNumberTextureFadeSetting
{
	public string SettingName = "";
	/// <summary>
	/// The source texture contains the Arabic Number.
	/// Note: the number must be order at 0 1 2 3 4 5 6 7 8 9
	/// </summary>
	public Texture2D SourceTexture;
	
	/// <summary>
	/// fade in-out time length.
	/// </summary>
	public float TimeLength = 3;
	
	/// <summary>
	/// The color curve.
	/// Defines how to color fade from start to end color.
	/// </summary>
	public ColorCurve colorCurve;
	/// <summary>
	/// The size expand setting.
	/// Defines how the size expand, in rate. 1 = initial size, 2 = double size.
	/// </summary>
	public FloatCurve sizeExpandSetting;
	/// <summary>
	/// The screen offset.
	/// Defines how the text float in screen.
	/// </summary>
	public Vector2Curve screenOffset;
	
	/// <summary>
	/// Pixel arrays, one array is a color block of the  arabic number.
	/// ColorBlocks[0] = Color block for Number Zero,
	/// ColorBlocks[1] = Color block for Number One ... etc
	/// </summary>
	[HideInInspector]
	public IList<Color[]> ColorBlocks = new List<Color[]> ();
	[HideInInspector]
	public float numberLetterWidth;
	[HideInInspector]
	public float numberLetterHeight;
	
	/// <summary>
	/// Gets the pixels of number block in a texture where number is ordered at 0123456789
	/// </summary>	
	Color[] GetNumberPixels (Texture2D sourceTexture, int ArabicNumber)
	{
		int x, y = 0;
		x = ArabicNumber * (int)numberLetterWidth;
		Color[] pixels = sourceTexture.GetPixels (x, y, (int)numberLetterWidth, (int)sourceTexture.height);
		return pixels;
	}
	
	/// <summary>
	/// Call this function to get the number texture.
	/// </summary>
	public Texture2D GetNumberTexture (int Number)
	{
		int[] iArray = Util.FormatNumberToNumberArray (Number);
		Texture2D t2d = BuildNumberTexture (iArray);
		return t2d;
	}
	
	/// <summary>
	/// Cut numbers from source texture and build a new texture and return.
	/// </summary>
	Texture2D BuildNumberTexture (int[] Number)
	{
		float textureWidth = numberLetterWidth * Number.Length;
		float textureHeight = numberLetterHeight;
		Texture2D runtimeTexture = new Texture2D (Mathf.RoundToInt (textureWidth), Mathf.RoundToInt (textureHeight), this.SourceTexture.format, false);
		for (int i=0; i<Number.Length; i++) {
			Color[] pixelBlocks = ColorBlocks [Number [i]];
			int x = i * (int)numberLetterWidth;
			int y = 0;
			runtimeTexture.SetPixels (x,
				                     y,
				                     Mathf.RoundToInt (numberLetterWidth), 
				                     Mathf.RoundToInt (numberLetterHeight), 
				                     pixelBlocks);
		}
		runtimeTexture.Apply (false);
		return runtimeTexture;
	}
	
	/// <summary>
	/// Initializes the fade setting. Call this function at Awake().
	/// </summary>
	public void InitializeFadeSetting ()
	{
		numberLetterWidth = SourceTexture.width / 10;
		numberLetterHeight = SourceTexture.height;
		for (int i=0; i<10; i++) {
			this.ColorBlocks.Add (GetNumberPixels (SourceTexture, i));
		}
	}
	
}

