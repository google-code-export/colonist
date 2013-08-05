using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Game global manager
/// </summary>
public class LevelManager : MonoBehaviour
{
	public string LevelName = "Level00";
	
	static LevelManager _instance = null;
	
	public static LevelManager Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = FindObjectOfType(typeof(LevelManager)) as LevelManager;
			}
			return _instance;
		}
	}
	
	public LayerMask GroundLayer;
	public Transform ControlDirectionPivot;
	public string PlayerTag = "Player";
	[HideInInspector]
	public IList<Unit> Units = new List<Unit> ();
	[HideInInspector]
	public GameObject player = null;
	
	/// <summary>
	/// The GameDialogue object which controls the dialogue show/hide.
	/// </summary>
	GameDialogue gameDialogueObject = null;
	/// <summary>
	/// The ScenarioControl object which control the scenario camera/scenario character behavior.
	/// </summary>
	ScenarioControl scenarioControlObject = null;
	BackgroundMusicPlayer backGroundMusicPlayer = null;
	CheckPointManager checkPointManager = null;
	PlayerCameraRuntimeConfig playerRuntimeCameraConfig = null;
	
	void Awake ()
	{
		_instance = this;
		player = GameObject.FindGameObjectWithTag (PlayerTag);
		gameDialogueObject = FindObjectOfType (typeof(GameDialogue)) as GameDialogue;
		scenarioControlObject = FindObjectOfType (typeof(ScenarioControl)) as ScenarioControl;
		backGroundMusicPlayer = FindObjectOfType (typeof(BackgroundMusicPlayer)) as BackgroundMusicPlayer;
		checkPointManager = FindObjectOfType (typeof(CheckPointManager)) as CheckPointManager;
		playerRuntimeCameraConfig = FindObjectOfType (typeof(PlayerCameraRuntimeConfig)) as PlayerCameraRuntimeConfig;
	}

	// Use this for initialization
	void Start ()
	{
		string level = "";
		string checkpoint = "";
		
		//try to load checkpoint, if checkpointManager exists in scene.
		if (checkPointManager != null) {
			checkPointManager.StartGame();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	void OnEnable ()
	{
		_instance = this;
	}
	
	/// <summary>
	/// Sends a GameEvent.
	/// </summary>
	public static void OnGameEvent (GameEvent gameEvent, Object caller)
	{
		if (gameEvent.delaySend > 0) {
			Instance.StartCoroutine ("GameEventWithDelay", gameEvent);
		} else {
			try {
				Instance.ProcessGameEvent (gameEvent);
			} catch (System.Exception exc) {
				Debug.LogError ("GameEvent has exception, caller:" + caller + " type:" + caller.GetType ().ToString () + 
					"\n" + "Event name:" + gameEvent.Name + "\n" + "Event type:" + gameEvent.type +
					"\n" + exc.Message + "\n" + exc.StackTrace);
			}
		}
	}
	
	IEnumerator GameEventWithDelay (GameEvent gameEvent)
	{
		yield return new WaitForSeconds(gameEvent.delaySend);
		ProcessGameEvent (gameEvent);
	}
	
	void ProcessGameEvent (GameEvent gameEvent)
	{
		switch (gameEvent.type) {
		case GameEventType.LevelPause:
			PauseGame();
			break;
		case GameEventType.ResumeLevel:
			ResumeGame();
			break;
		//GameDialogue object display dialogue text by dialog id.
		case GameEventType.ShowGameDialogue:
			this.gameDialogueObject.OnGameEvent (gameEvent);
			break;
		case GameEventType.WhiteInScenarioCamera:
		case GameEventType.WhiteOutScenarioCamera:
		case GameEventType.TimeScaleOn:
		case GameEventType.TimeScaleOff:
		case GameEventType.ScenarioCameraOff:
		case GameEventType.ScenarioCameraOn:
		case GameEventType.PlayerCameraOn:
		case GameEventType.PlayerCameraOff:
		case GameEventType.PlayerCameraAudioListenerOff:
		case GameEventType.PlayerCameraAudioListenerOn:
		case GameEventType.ScenarioCameraAudioListenerOn:
		case GameEventType.ScenarioCameraAudioListenerOff:
		case GameEventType.StartDocking:
		case GameEventType.StartDockingOnRuntimeTarget:
		case GameEventType.PlayerCameraSlowMotionOnFixedPoint:
		case GameEventType.PlayerCameraSlowMotionOnTransform:
		case GameEventType.ShiftToPlayerMode:
		case GameEventType.ShiftToScenarioMode:
			this.scenarioControlObject.OnGameEvent (gameEvent);
			break;
		case GameEventType.ApplyTopdownCameraParameter:
		case GameEventType.ResetTopdownCameraParameter:	
			playerRuntimeCameraConfig.OnGameEvent(gameEvent);
		    break;
		case GameEventType.PlayerSetToActive:
			player.transform.root.gameObject.SetActiveRecursively (true);
			break;
		case GameEventType.PlayerSetToInactive:
			player.transform.root.gameObject.SetActiveRecursively (false);
			break;
		case GameEventType.PlayerControlOn:
		case GameEventType.PlayerControlOff:
		case GameEventType.WhiteInPlayerCamera:
		case GameEventType.WhiteOutPlayerCamera:
			player.transform.root.BroadcastMessage ("OnGameEvent", gameEvent, SendMessageOptions.DontRequireReceiver);
			break;
		case GameEventType.NPCPutToGround:
			Util.PutToGround (gameEvent.receiver.transform, this.GroundLayer, 0.1f);
			break;
		case GameEventType.NPCPlayAnimation:
			gameEvent.receiver.animation.Rewind(gameEvent.StringParameter);
			gameEvent.receiver.animation.Play (gameEvent.StringParameter);
			break;
		case GameEventType.NPCPlayQueueAnimation:
			foreach (string ani in gameEvent.StringParameter.Split(new char[] { ';' })) {
				gameEvent.receiver.animation.CrossFadeQueued (ani);
			}
			break;
		case GameEventType.NPCFaceToPlayer:
			gameEvent.receiver.transform.LookAt (new Vector3 (player.transform.position.x, gameEvent.receiver.transform.position.y, player.transform.position.z));
			break;
		case GameEventType.NPCStopPlayingAnimation:
			gameEvent.receiver.animation.Stop (gameEvent.StringParameter);
			break;
		case GameEventType.NPCStartAI:
		case GameEventType.NPCStartDefaultAI:
			gameEvent.receiver.SendMessage ("OnGameEvent", gameEvent);
			break;
		case GameEventType.DeactivateGameObject:
			Util.DeactivateRecurrsive (gameEvent.receiver);
			break;
		case GameEventType.DestroyGameObject:
			Destroy (gameEvent.receiver, gameEvent.FloatParameter);
			break;
		case GameEventType.ActivateGameObject:
			Util.ActivateRecurrsive (gameEvent.receiver);
			break;
		case GameEventType.ActivateGameObjectIgnoreChildren:
			gameEvent.receiver.active = true;
			break;
		case GameEventType.DeactivateGameObjectIgnoreChildren:
			gameEvent.receiver.active = false;
			break;
		case GameEventType.SpecifiedSpawn:
			gameEvent.receiver.SendMessage ("OnGameEvent", gameEvent);
			break;
		case GameEventType.InvokeMethod:
			//in case the receiver is destroyed in runtime.
			if (gameEvent.receiver == null) {
				break;
			} else {
				if (gameEvent.parameterType == ParameterType.None)
					gameEvent.receiver.SendMessage (gameEvent.CustomMessage);
				else if (gameEvent.parameterType == ParameterType.Int)
					gameEvent.receiver.SendMessage (gameEvent.CustomMessage, gameEvent.IntParameter);
				else if (gameEvent.parameterType == ParameterType.Float)
					gameEvent.receiver.SendMessage (gameEvent.CustomMessage, gameEvent.FloatParameter);
				else if (gameEvent.parameterType == ParameterType.String)
					gameEvent.receiver.SendMessage (gameEvent.CustomMessage, gameEvent.StringParameter);
				else if (gameEvent.parameterType == ParameterType.Bool)
					gameEvent.receiver.SendMessage (gameEvent.CustomMessage, gameEvent.BoolParameter);
				break;
			}
		case GameEventType.SetLanguage:
			SystemLanguage systemlanguage = (SystemLanguage)System.Enum.Parse (typeof(SystemLanguage), gameEvent.StringParameter);
			if (Persistence.GetPlayerLanguage () != systemlanguage) {
				Persistence.SetPlayerLanguage (systemlanguage);
				Debug.Log ("Change language:" + systemlanguage);
			}
			break;
		case GameEventType.SetQuality:
			Persistence.SetPlayerQualityLevel (gameEvent.IntParameter);
			break;
		case GameEventType.LoadLevel:
			LoadLevel (gameEvent);
			break;
		case GameEventType.LoadNextLevel:
			LoadNextLevel();
			break;
		case GameEventType.ContinueLastCheckPoint:
			ContinueLastCheckpoint();
			break;
		case GameEventType.ReloadLevelWithCheckpoint:
			//Reload the current level with checkpoint
			ReloadCurrentLevel(true);
			break;
		case GameEventType.ReloadLevel:
			//Reload the current level
			ReloadCurrentLevel(false);
			break;
			
		case GameEventType.Mute:
			Persistence.Mute ();
			break;
		case GameEventType.UnMute:
			Persistence.Unmute ();
			break;
		case GameEventType.ForceToPhysicsCharacter:
			gameEvent.receiver.SendMessage ("OnGameEvent", gameEvent);
			break;
		case GameEventType.PlayBackgroundMusic:
		case GameEventType.StopBackgroundMusic:
			backGroundMusicPlayer.OnGameEvent (gameEvent);
			break;
		case GameEventType.SaveCheckPoint:
			checkPointManager.CheckPointReach (gameEvent.StringParameter);
			break;
		case GameEventType.AttachObjectToSpecifiedParent:
			gameEvent.receiver.transform.parent = gameEvent.GameObjectParameter.transform;
			break;
		case GameEventType.DetachObjectFromParent:
			gameEvent.receiver.transform.parent = null;
			break;
		case GameEventType.SetPlayerControlDirectionPivot:
			this.ControlDirectionPivot = gameEvent.GameObjectParameter.transform;
			break;
		case GameEventType.AlignObjectToSpecifiedParent:
			gameEvent.receiver.transform.position = gameEvent.GameObjectParameter.transform.position;
			gameEvent.receiver.transform.rotation = gameEvent.GameObjectParameter.transform.rotation;
			break;
		}
	}
	
	void LoadNextLevel()
	{
		int currentLevel = Application.loadedLevel;
		LoadLevelByNumber(currentLevel + 1);
	}
	
	/// <summary>
	/// Loads the level completely, ignores the previous played checkpoint.
	/// </summary>
	void LoadLevel (GameEvent gameEvent)
	{

		switch (gameEvent.parameterType) {
		    case ParameterType.Int:
			  LoadLevelByNumber(gameEvent.IntParameter);
			  break;
		    case ParameterType.String:
			  LoadLevelByName(gameEvent.StringParameter);
			  break;
		}
	}
	
	/// <summary>
	/// Passes the checkpoint object to next level.
	/// loadLastCheckPoint - true of false to indicate if the previous checkpoint is loaded 
	/// checkPointName - only used when loadLastCheckPoint == true
	/// </summary>
	void PassCheckpointObjectToNextLevel(bool loadLastCheckPoint, string checkPointName)
	{
		GameObject checkPointObject = new GameObject("CheckPoint");
		CheckPoint checkPoint = checkPointObject.AddComponent<CheckPoint>();
		checkPoint.loadCheckPointType = loadLastCheckPoint ? CheckPoint.LoadCheckPointType.LoadLastCheckpoint : CheckPoint.LoadCheckPointType.NewLevelStart;
		if(checkPoint.loadCheckPointType == CheckPoint.LoadCheckPointType.LoadLastCheckpoint)
		{
			checkPoint.LoadCheckPointName = checkPointName;
		}
		//leave the checkPointObject undestroyed to next scene.
		Object.DontDestroyOnLoad(checkPointObject);
	}
	
	/// <summary>
	/// Reloads the current level.
	/// withLastCheckpoint indicates If any checkpoint exists in previous part of current level, should we load the checkpoint ?
	/// </summary>
	void ReloadCurrentLevel(bool withLastCheckpoint)
	{
		//if withLastCheckpoint = true, load current level with checkpoint
		if(withLastCheckpoint)
		{
			//check if there's checkpoint , and if the checkpoint level = current level
			string lastCheckPointLevel = "";
		    string lastCheckPointName = "";
			bool hasCheckPoint = checkPointManager.GetLastCheckpoint(out lastCheckPointLevel, out lastCheckPointName);
			//if has checkpoint and the checkpoint level = this level
			if(hasCheckPoint && lastCheckPointLevel == Application.loadedLevelName)
			{
				ContinueLastCheckpoint();
			}
			else 
			{
				LoadLevelByNumber(Application.loadedLevel);
			}
		}
		else 
		//simply reload the current level
		{
			LoadLevelByNumber(Application.loadedLevel);
		}
	}
	
	/// <summary>
	/// Loads the level by name, and start the level completely new.
	/// </summary>
	void LoadLevelByName(string levelName)
	{
		PassCheckpointObjectToNextLevel(false, "");
		Application.LoadLevel (levelName);
	}
	/// <summary>
	/// Loads the level by number, and start the level completely new.
	/// </summary>
	void LoadLevelByNumber(int number)
	{
		PassCheckpointObjectToNextLevel(false, "");
		Application.LoadLevel (number);
	}
	/// <summary>
	/// Loads the last played level and continue with last played checkpoint.
	/// </summary>
	void ContinueLastCheckpoint()
	{
		string lastCheckPointLevel = "";
		string lastCheckPointName = "";
		bool hasCheckPoint = checkPointManager.GetLastCheckpoint(out lastCheckPointLevel, out lastCheckPointName);
		if(hasCheckPoint)
		{
			PassCheckpointObjectToNextLevel(true, lastCheckPointName);
			Application.LoadLevel (lastCheckPointLevel);
		}
		//if no last checkpoint, start the new game
		else 
		{
			LoadLevelByName("Level00");//yes, this is hardcode ...
		}
	}
	
	
	public static void SendAIMessage (string message, object parameter)
	{
		for (int i=0; i<Instance.Units.Count; i++) {
			Unit unit = Instance.Units [i];
			if (unit == null) {
				Instance.Units.RemoveAt (i);
			} else {
				unit.SendMessage (message, parameter);
			}
		}
	}
	
	public static void PauseGame()
	{
		Time.timeScale = 0;
	}
	
	public static void ResumeGame()
	{
		Time.timeScale = 1;
	}
}
