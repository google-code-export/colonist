using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Language and corresponding font.
/// </summary>
[System.Serializable]
public class LocaleStyle
{
    public SystemLanguage language;
    public GUISkin guiSkin;
    //if only use font = true, the guiSkin is ignore in GUI drawing. But the font will be used.
    public bool OnlyUseFont;
    public Font font = null;
}

/// <summary>
/// Controls game dialogue behavior.
/// </summary>
[ExecuteInEditMode]
public class GameDialogue : MonoBehaviour, I_GameEventReceiver
{
    public string LevelName = "Level Name";
    public SystemLanguage targetLanguage;
    public float ShowDialogueTime = 5;
    public Texture DialogueBackgroundTexture;

    /// <summary>
    /// The rectange position for dialogue text, calculate by DialogueAreaAspect.
    /// </summary>
    public AdaptiveRect DialogueArea = new AdaptiveRect();
    /// <summary>
    /// The rectange position for portrait of the dialogue.
    /// </summary>
    public AdaptiveRect PortraitArea = new AdaptiveRect();
    /// <summary>
    /// The rectange position for dialogue background textue.
    /// </summary>
    public AdaptiveRect BackgroundArea = new AdaptiveRect();

    public Texture displayedImage;

    /// <summary>
    /// The portrait image on the current dialog.
    /// </summary>
    public Texture2D Portrait = null;
    public string SpearkerName = "Speaker";
	
	public ScaleMode scaleMode;
	
	public bool HasDialog = true;
	
    void Awake()
    {
        InitializeGameDialog();
    }

    void SetupDisplayedRect()
    {
        DialogueArea.Bound = DialogueArea.GetBound();
		PortraitArea.Bound = PortraitArea.GetBound();
		BackgroundArea.Bound = BackgroundArea.GetBound();
    }

    void InitializeGameDialog()
    {
        targetLanguage = Persistence.GetPlayerLanguage();
        Localization.InitializeLevelDialogue(this.LevelName, targetLanguage);
        Portrait = null;
        SetupDisplayedRect();
    }

    // Use this for initialization
    void Start()
    {
        //		StartCoroutine ("DisplayDialogue", "0");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnGameEvent(GameEvent _event)
    {
        switch (_event.type)
        {
            case GameEventType.ShowGameDialogue:
                Stop();//Stop any playing dialogue
                string DialogueID = _event.StringParameter;
                StartCoroutine("DisplayDialogue", DialogueID);
                break;
        }
    }

    IEnumerator DisplayDialogueWithDelay(string DialogueID, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine("DisplayDialogue", DialogueID);
    }

    public void Stop()
    {
        StopCoroutine("DisplayDialogue");
    }

    /// <summary>
    /// Displaies the dialogue.
    /// if AnimatedTypingLetter = true, the letters will be typed to screen one by one
    /// if AnimatedTypingLetter = false. the dialog text will be flashed out to screen in one time.
    /// </summary>
    IEnumerator DisplayDialogue(string DialogueID)
    {
        LocalizedDialogue dialog = Localization.GetDialogue(DialogueID);
        HasDialog = true;
        foreach (LocalizedDialogueItem dialogItem in dialog.dialogueItem)
        {
            LocalizeCharacter speaker = Localization.GetCharacter(dialogItem.DialogCharacterID);
            this.Portrait = speaker.PortraitIconTexture;
            this.SpearkerName = speaker.CharacterName;
            float delay = dialogItem.pendAfterFinished.HasValue ? dialogItem.pendAfterFinished.Value : 2;
             if (!string.IsNullOrEmpty(dialogItem.DialogImageName))
                displayedImage = (Texture)Resources.Load(Localization.DialogAssetRootFolder + "/" +
                                                                LevelName + "/" + this.targetLanguage + "/" + dialogItem.DialogImageName,
                                                                typeof(Texture));
            else
                displayedImage = null;
            yield return new WaitForSeconds(delay);
        }
        HasDialog = false;
    }
	
    void OnGUI()
    {
        if (HasDialog)
        {
            if (DialogueBackgroundTexture)
            {
                GUI.DrawTexture(BackgroundArea.GetBound(), DialogueBackgroundTexture, ScaleMode.ScaleAndCrop, true);
            }
            //draw character portrait
            if (this.Portrait != null)
            {
                GUI.DrawTexture(this.PortraitArea.GetBound(), Portrait);
            }
//            if (SpearkerName != null)
//            {
//                Rect speakerNameArea = new Rect(PortraitArea);
//                speakerNameArea.x += speakerNameArea.width + 20;
//            }

            if (displayedImage != null)
            {
                GUI.DrawTexture(this.DialogueArea.GetBound(), displayedImage, scaleMode);
            }
        }
	}
}
