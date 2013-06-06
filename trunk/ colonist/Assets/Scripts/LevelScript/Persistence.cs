using UnityEngine;
using System.Collections;

/// <summary>
/// Handle persistence.
/// 1. Player Language.
/// 2. Last level.
/// 3. Last checkpoint.
/// 4. Player expereince.
/// 5. Player skill tree.
/// </summary>
public class Persistence : MonoBehaviour {
	
	public const string Language = "Language";
	public const string Quality = "Quality";
	public const string AudioMute = "Mute";
	public const string CheckPointLevel = "CheckpointLevel";
	public const string CheckPointName = "CheckpointName";
	void Awake()
	{
		
	}
	
	// Use this for initialization
	void Start () {
	    InitializePlayerLanguage();
		InitializePlayerQualityLevel();
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void InitializePlayerLanguage()
	{
		if(PlayerPrefs.HasKey(Language) == false)
		{
			//if the default language is one of the supported language:
			if(Localization.SupportLanguage.Contains(Application.systemLanguage.ToString()))
			{
			   PlayerPrefs.SetString(Language, Application.systemLanguage.ToString());
			}
			else 
			{
				//else, set english as default langauge.
				PlayerPrefs.SetString(Language, SystemLanguage.English.ToString());
			}
		}
	}
	
	void InitializePlayerQualityLevel()
	{
		if(PlayerPrefs.HasKey(Quality) == false)
		{
			PlayerPrefs.SetInt(Quality, QualitySettings.GetQualityLevel());
			Debug.Log("Player quality not set!");
		}
		Debug.Log("Current system Quality:" + QualitySettings.GetQualityLevel() + " player pref quality: " + PlayerPrefs.GetInt(Quality));
	}
	
	public static SystemLanguage GetPlayerLanguage()
	{
		SystemLanguage l = (SystemLanguage)System.Enum.Parse(typeof(SystemLanguage), PlayerPrefs.GetString(Language));
		return l;
	}
	
	public static void SetPlayerLanguage(SystemLanguage language)
	{
		PlayerPrefs.SetString(Language, language.ToString());
	}
	
	public static int GetPlayerQualityLevel()
	{
		return PlayerPrefs.GetInt(Quality);
	}
	
	public static void SetPlayerQualityLevel(int level)
	{
		QualitySettings.SetQualityLevel(level, false);
		PlayerPrefs.SetInt(Quality, level);
	}
	
	public static void Mute()
	{
		PlayerPrefs.SetInt(AudioMute, 1);
 
	}
	
	public static void Unmute()
	{
		PlayerPrefs.SetInt(AudioMute, 0);
 
	}
	
	public static bool IsMute()
	{
		bool isMute = PlayerPrefs.HasKey(AudioMute) ? PlayerPrefs.GetInt(AudioMute) == 1 : false;
		return isMute;
	}
	
	public static void SaveCheckPoint(string LevelName, string CheckPointName)
	{
		PlayerPrefs.SetString(Persistence.CheckPointLevel, LevelName);
	    PlayerPrefs.SetString(Persistence.CheckPointName, CheckPointName);
	}
	
	public static bool GetLastCheckPoint(out string LevelName, out string CheckPointName)
	{
		if( !PlayerPrefs.HasKey(Persistence.CheckPointLevel) || !PlayerPrefs.HasKey(Persistence.CheckPointName))
		{
			LevelName =	PlayerPrefs.GetString(Persistence.CheckPointLevel);
	        CheckPointName = PlayerPrefs.GetString(Persistence.CheckPointName);
			return true;
		}
		else 
		{
	        LevelName =	"";
	        CheckPointName = "";
			return false;
		}
	}
}
