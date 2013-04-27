using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Language and corresponding font.
/// </summary>
[System.Serializable]
public class LocaleFont
{
	public SystemLanguage language;
	public Font font;
}

/// <summary>
/// Controls game dialogue behavior.
/// </summary>
[ExecuteInEditMode]
public class GameDialogue : MonoBehaviour, I_GameEventReceiver
{
	public SystemLanguage targetLanguage;

	public float ShowDialogueTime = 5;
	
	public Texture DialogueBackgroundTexture;
	
	/// <summary>
	/// The dialogue area aspect relatived to current screen size.
	/// DialogueAreaAspect.x = Width in percentage from screen right to text start point.x 
	/// DialogueAreaAspect.y = Height in percentage from screen top to text start point.y
	/// </summary>
	public Vector2 DialogueAreaAspect = Vector2.zero;
	
	/// <summary>
	/// The portrait area aspect. Same to DialogueAreaAspect, controls the portrait area.
	/// </summary>
	public Vector2 PortraitAreaAspect = Vector2.zero;
	
	/// <summary>
	/// The background area aspect. Same to DialogueAreaAspect, controls the Background Area.
	/// </summary>
	public Vector2 BackgroundAreaAspect = Vector2.zero;
	
	/// <summary>
	/// The rectange position for dialogue text, calculate by DialogueAreaAspect.
	/// </summary>
	Rect DialogueArea = new Rect ();
	/// <summary>
	/// The rectange position for portrait of the dialogue.
	/// </summary>
	Rect PortraitArea = new Rect ();
	/// <summary>
	/// The rectange position for dialogue background textue.
	/// </summary>
	Rect BackgroundArea = new Rect ();
	
	/// <summary>
	/// The width of the text, in letter count.
	/// </summary>
	public int textWidth = 60;
	
    public LocaleFont[] LocaleFontArray = new LocaleFont[] { };
	
	/// <summary>
	/// After display a dialogue, delay N seconds.
	/// </summary>
	public float DefaultDelayPerDialog;
	
	/// <summary>
	/// If AnimatedTypingLetter = true, the text will be typed to screen letter by letter,
	/// if AnimatedTypingLetter = false, the text will be displayed to screen in one time.
	/// </summary>
	public bool AnimatedTypingLetter = true;
	/// <summary>
	/// Used when animated = true, how many character be displayed in one second ?
	/// The large this value the faster the text to be typed to screen.
	/// Use as default setting, only used when the custom setting is not defined in dialog XML.
	/// </summary>
	public int DefaultTypeLetterSpeed = 48;
	
	
	public bool Completed { get { return finished; } }

	public bool Active { get { return animating; } }
	
	List<string> lines;
	bool animating = false;
	bool finished = false;
	/// <summary>
	/// The displayed text.
	/// </summary>
	public string displayedText = "Displayed text";

	/// <summary>
	/// The portrait image on the current dialog.
	/// </summary>
	public Texture2D Portrait = null;
	
	/// <summary>
	/// if breakOnWholeWords = true, when line space not enough,
	/// word will be displayed integrallty at next line, 
	/// </summary>
	bool breakOnWholeWords = true;
	Font customFont = null;
	
	void Awake ()
	{
		Localization.InitializeLevelDialogue (LevelManager.Instance.LevelName, targetLanguage);
		displayedText = "";
		Portrait = null;
		//Pick custom font for the target language, if exists.
		foreach(LocaleFont lf in LocaleFontArray)
		{
			if(lf.language == targetLanguage)
			{
			   this.customFont = lf.font;
			   break;
			}
		}
		SetupDisplayedRect();
	}
	
	void SetupDisplayedRect()
	{
		DialogueArea.x = Screen.width * DialogueAreaAspect.x;
		DialogueArea.y = Screen.height * DialogueAreaAspect.y;
		DialogueArea.width = Screen.width - DialogueArea.x - 10; //10 unit offset
		DialogueArea.height = Screen.height - DialogueArea.y - 5;//5 unit offseet
		
		PortraitArea.x = Screen.width * PortraitAreaAspect.x;
		PortraitArea.y = Screen.height * PortraitAreaAspect.y;
		PortraitArea.width = DialogueArea.x - PortraitArea.x - 10; //10 unit offset
		PortraitArea.height = Screen.height - PortraitArea.y - 5;//5 unit offseet
		
		BackgroundArea.x = 0;
		BackgroundArea.y = Screen.height * BackgroundAreaAspect.y;
		BackgroundArea.width = Screen.width + 10;
		BackgroundArea.height = Screen.height - BackgroundArea.y + 10;
	}
	
	// Use this for initialization
	void Start ()
	{
//		StartCoroutine ("DisplayDialogue", "0");
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	public void OnGameEvent (GameEvent _event)
	{
		if (_event.type == GameEventType.DisplayGUIText) {
			string DialogueID = _event.StringParameter;
			StartCoroutine("DisplayDialogue",DialogueID);
		}
	}

	public void Stop ()
	{
		StopCoroutine ("DisplayDialogue");
		animating = false;
	}
	
	IEnumerator DisplayDialogue (string DialogueID)
	{
		LocalizedDialogue dialog = Localization.GetDialogue (DialogueID);
		
		foreach (LocalizedDialogueItem dialogItem in dialog.dialogueItem) {
			LocalizeCharacter speaker = Localization.GetCharacter(dialogItem.DialogCharacterID);
			this.Portrait = speaker.IconTexture;
			
			string formattedText = FormatText (dialogItem.DialogText);
			int typeSpeed = dialogItem.typeSpeed.HasValue ? dialogItem.typeSpeed.Value : DefaultTypeLetterSpeed;
			float delay = dialogItem.pendAfterFinished.HasValue ? dialogItem.pendAfterFinished.Value : DefaultDelayPerDialog;
			
			if (AnimatedTypingLetter) {
				animating = true;
				int currentLength = 1;
				
				while (animating && currentLength <= formattedText.Length) {
				
					displayedText = formattedText.Substring (0, currentLength);
					// This has to be calculated each time because the rate can change
					// during playback, such as with a "hold button to accelerate text"
					// type script.
					yield return new WaitForSeconds(1/(float)typeSpeed);
					++currentLength;
				}
			} else {
				displayedText = formattedText;
			}
			
			yield return new WaitForSeconds(delay);
		}
		finished = true;
	}
	
	string FormatText (string text)
	{
		int offset = 0;
		finished = false;
		lines = new List<string> ();
		
		if (breakOnWholeWords) {
			int width;
			int adjustedWidth;
			bool endOfText = false;
			
			while (offset < text.Length) {
				width = textWidth;
				if (offset + width > text.Length) {
					width = text.Length - offset;
					endOfText = true;
				}
				
				adjustedWidth = width;
				try{
				while (!endOfText && (offset+adjustedWidth-1 >= 0) && text[offset+adjustedWidth-1] != ' ' && adjustedWidth > 0) {
					--adjustedWidth;
				}
				}catch(System.Exception  exc)
				{
					Debug.LogError(offset+adjustedWidth-1);
					throw exc;
				}
				
				if (0 == adjustedWidth) {
					adjustedWidth = width;
				}
				lines.Add (text.Substring (offset, adjustedWidth));
				
				offset += adjustedWidth;
			}
		} else {
			while (offset < text.Length) {
				if (offset + textWidth > text.Length) {
					lines.Add (text.Substring (offset, text.Length - offset));
				} else {
					lines.Add (text.Substring (offset, textWidth));
				}
				offset += textWidth;
			}
		}
		
		StringBuilder sb = new StringBuilder ();
		foreach (string s in lines) {
			sb.Append (s);
			sb.Append ("\n");
		}
		return sb.Remove (sb.Length - 1, 1).ToString ();
	}
	
	void OnGUI ()
	{
		if(DialogueBackgroundTexture)
		{
			GUI.DrawTexture(BackgroundArea, DialogueBackgroundTexture,ScaleMode.ScaleAndCrop, true);
		}
		if (Portrait != null) {
			GUI.DrawTexture (this.PortraitArea, Portrait,ScaleMode.ScaleToFit, true);
		}
		if (displayedText != "") {
			if(this.customFont != null)
			{
				GUI.skin.font = this.customFont;
			}
			GUI.Label (this.DialogueArea, displayedText);
		}
	}
}
