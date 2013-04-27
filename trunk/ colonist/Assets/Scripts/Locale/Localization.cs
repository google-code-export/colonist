using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


/// <summary>
/// LocalizeCharacter wrap data on the portrait of dialogue.
/// </summary>
public class LocalizeCharacter
{
	public string CharacterID = "";
	public string CharacterName = "";
	/// <summary>
	/// The character icon path. Normally it's something like "Portrait\William"
	/// </summary>
	public string CharacterIconPath = "";
	/// <summary>
	/// The icon texture loaded from CharacterIconPath.
	/// </summary>
	public Texture2D IconTexture = null;
}

/// <summary>
/// LocalizedDialogueItem wrap data on a dialog script.
/// </summary>
public class LocalizedDialogueItem
{	
	public string DialogText = "";
	/// <summary>
	/// The character ID who speak the dialog. Mapped to Localization_Character.CharacterID
	/// </summary>
	public string DialogCharacterID = "";
	
	/// <summary>
	/// typeSpeed = How many letters will be typed in one second.
	/// if typeSpeed = null, use default setting in GameDialogue.
	/// </summary>
	public int? typeSpeed = null;
	
	/// <summary>
	/// pendAfterFinished = after dialog typed complete, how many seconds should be waited.
	/// if pendAfterFinished = null, use default setting in GameDialogue.
	/// </summary>
	public float? pendAfterFinished = null;
}

/// <summary>
/// LocalizedDialogue wrap data on a dialog.It contains several LocalizedDialogueItem, and display the LocalizedDialogueItem one by one.
/// </summary>
public class LocalizedDialogue
{
	public string DialogID = "";
	public IList<LocalizedDialogueItem> dialogueItem = new List<LocalizedDialogueItem>();
}

/// <summary>
/// Localization handles the localization business.
/// Current support lanuage: 
///   1. Chinese, 2. English, 
/// </summary>
public class Localization {

    const string DialogAssetRootFolder = "Language/Dialog";
	const string CharacterAssetRootFolder = "Language/Character/Character";
	const string SupportLanguage = "English;Chinese;";
	
	static IDictionary<string, LocalizeCharacter> CharacterDict = new Dictionary<string, LocalizeCharacter>();
	static IDictionary<string, LocalizedDialogue> DialogueDict = new Dictionary<string, LocalizedDialogue>();
	
	static SystemLanguage TargetLanguage;
	
	/// <summary>
	/// Initialize level dialogue by the specified language.
	/// if
	/// </summary>
	public static void InitializeLevelDialogue(string LevelName, SystemLanguage? _language)
	{
	   TargetLanguage = _language == null ? SystemLanguage.English :
			             (SupportLanguage.Contains(_language.Value.ToString())? _language.Value : SystemLanguage.English);
		
	   string DialogFilePath = DialogAssetRootFolder + "/" + LevelName + "/" + TargetLanguage.ToString();
	   TextAsset dialogFile = Resources.Load(DialogFilePath, typeof(TextAsset)) as TextAsset;
	   TextAsset characterXMLFile = Resources.Load(CharacterAssetRootFolder, typeof(TextAsset)) as TextAsset;
	   CharacterDict.Clear();
	   DialogueDict.Clear();
		
	   CharacterDict = ParseCharacterLocalizationXMLFile (ParseTextAssetToXMLDocument(characterXMLFile));
	   DialogueDict = ParseDialogLocalizationXMLFile(ParseTextAssetToXMLDocument(dialogFile));
	}
	
	static XmlDocument ParseTextAssetToXMLDocument(TextAsset textasset)
	{
		XmlDocument xmlDoc = new XmlDocument();
		//because of annoying feature of Unity, that the way to read UTF-8 XML, we need to skip BOM(byte order mark)
		//but for XML without UTF-8 character, we MUST NOT skip first character.
		//so, we firstly not skip BOM, try to load XML, if it fail, then try skip BOM to parse again.
		bool parseOK = false;
		//1. not SKIP first character
		try{
			xmlDoc.LoadXml(textasset.text);
			parseOK = true;
			return xmlDoc;
		}
		catch(System.Exception exc)
		{
			Debug.Log("It seems we need to skip BOM at XML:" + textasset.name + "\n" + exc.StackTrace);
			parseOK = false;
		}
		//if 1. fail, skip BOM, and parse again.
		if(parseOK == false)
		{
			System.IO.StringReader stringReader = new System.IO.StringReader(textasset.text);
			stringReader.Read(); // skip BOM
			System.Xml.XmlReader reader = System.Xml.XmlReader.Create(stringReader);
			xmlDoc.Load(reader);
			reader.Close();
			stringReader.Close();
		}
		return xmlDoc;
	}
	
	/// <summary>
	/// Parses the character localization XML file.
	/// </summary>
	static IDictionary<string, LocalizeCharacter> ParseCharacterLocalizationXMLFile(XmlDocument xmlDoc)
	{
		IDictionary<string, LocalizeCharacter> ret = new Dictionary<string, LocalizeCharacter>();
	    XmlElement root = xmlDoc.DocumentElement;
		XmlNodeList xmlNodeList = root.GetElementsByTagName("character");
		foreach(XmlNode node in xmlNodeList)
		{
			XmlElement characterElement = (XmlElement)node;
			LocalizeCharacter localization_Character = new LocalizeCharacter();
			localization_Character.CharacterID = characterElement.GetAttribute("id");;
			localization_Character.CharacterName = characterElement.GetAttribute("Chinese");
			localization_Character.CharacterIconPath = characterElement.GetAttribute("imagepath");
			localization_Character.IconTexture = (Texture2D)Resources.Load(localization_Character.CharacterIconPath, typeof(Texture2D));
			ret.Add(localization_Character.CharacterID, localization_Character);
		}
		return ret;
	}
	
	/// <summary>
	/// Parses the dialog localization XML file.
	/// </summary>
	static IDictionary<string, LocalizedDialogue> ParseDialogLocalizationXMLFile(XmlDocument xmlDoc)
	{
		IDictionary<string, LocalizedDialogue> ret = new Dictionary<string, LocalizedDialogue>();
	    XmlElement root = xmlDoc.DocumentElement;
		XmlNodeList xmlNodeList = root.GetElementsByTagName("dialog");
		foreach(XmlNode dialogNode in xmlNodeList)
		{
			XmlElement dialogElement = (XmlElement)dialogNode;
			LocalizedDialogue localizedDialogue = new LocalizedDialogue();
			localizedDialogue.DialogID = dialogElement.GetAttribute("id");
			foreach(XmlNode dialogItemNode in  dialogElement.ChildNodes)
			{
				XmlElement dialogItemElement = (XmlElement)dialogItemNode;
				LocalizedDialogueItem localizedDialogueItem = new LocalizedDialogueItem();
			    localizedDialogueItem.DialogCharacterID = dialogItemElement.GetAttribute("character");
			    localizedDialogueItem.DialogText = dialogItemElement.GetAttribute("text");
				
				if(dialogItemElement.HasAttribute("speed"))
				{
					localizedDialogueItem.typeSpeed = int.Parse(dialogItemElement.GetAttribute("speed"));
				}
				if(dialogItemElement.HasAttribute("pend"))
				{
					localizedDialogueItem.pendAfterFinished = float.Parse(dialogItemElement.GetAttribute("pend"));
				}
				
				localizedDialogue.dialogueItem.Add(localizedDialogueItem);
			}
			ret.Add(localizedDialogue.DialogID, localizedDialogue);
		}
		return ret;
	}
	
	public static LocalizedDialogue GetDialogue(string DialogID)
	{
		return DialogueDict[DialogID];
	}
	
	public static LocalizeCharacter GetCharacter(string CharacterID)
	{
		return CharacterDict[CharacterID];
	}
}
