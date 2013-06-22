using UnityEngine;
using System.Collections;

/// <summary>
/// LocaleButtonParameter wraps locale button parameter corresponding to language.
/// </summary>
[System.Serializable]
public class LocaleButtonParameter
{
	public SystemLanguage Language;
	public string text;
	public GUIStyle style;
	public Texture texture;
}

/// <summary>
/// LocaleButton is a JoyButton that supports multi-language.
/// </summary>
/// 
[ExecuteInEditMode]
public class LocaleButton : JoyButton
{
	
	public LocaleButtonParameter[] LocaleButtonParameterArray = new LocaleButtonParameter[]{};
	public GameEvent[] event_on_touch = new GameEvent[]{};
	LocaleButtonParameter CurrentLocale = null;
	
	void Awake ()
	{
		GUIBound = this.GetAdaptiveBound ();
	}
	
	// Use this for initialization
	void Start ()
	{
		foreach (LocaleButtonParameter locale in LocaleButtonParameterArray) {
			if (locale.Language == Persistence.GetPlayerLanguage ()) {
				CurrentLocale = locale;
				break;
			}
		}
		if (CurrentLocale == null) {
			foreach (LocaleButtonParameter locale in LocaleButtonParameterArray) {
				if (locale.Language == SystemLanguage.English) {
					CurrentLocale = locale;
					break;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	public override void onTouchBegin (Touch touch)
	{
		base.onTouchBegin (touch);
	}
	
	public override void onTouchMove (Touch touch)
	{
	}
	/// <summary>
	/// Call when touch.phase = Move
	/// </summary>
	/// <param name="touch"></param>
	public override void onTouchStationary (Touch touch)
	{
	}
	/// <summary>
	/// Call when touch.phase = End
	/// </summary>
	/// <param name="touch"></param>
	public override void onTouchEnd (Touch touch)
	{
		base.onTouchEnd (touch);
		foreach (GameEvent e in event_on_touch) {
			LevelManager.OnGameEvent (e , this);
		}
	}
	public ScaleMode _scaleMode;
	void OnGUI ()
	{
		GUIBound = base.GetAdaptiveBound();
		if (CurrentLocale != null) {
			if (CurrentLocale.texture != null) {
				GUI.DrawTexture (GUIBound, CurrentLocale.texture);
			}
			if (CurrentLocale.text != null && CurrentLocale.text != string.Empty) {
				GUI.Label (GUIBound, CurrentLocale.text, CurrentLocale.style);
			}
		}
		if (ButtonTexture != null) {
			GUI.DrawTexture (GUIBound, ButtonTexture,_scaleMode);
		}
	}
}
