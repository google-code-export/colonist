using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Game global manager
/// </summary>
[ExecuteInEditMode]
public class LevelManager : MonoBehaviour {
	public string LevelName = "Level00";
    public static LevelManager Instance;
	public LayerMask GroundLayer;

	public Transform ControlDirectionPivot;
	public string PlayerTag = "Player";
	
    [HideInInspector]
    public IList<Unit> Units = new List<Unit>();
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
	
	CheckPoint checkPointManager = null;
	
    void Awake()
    {
        Instance = this;
		player = GameObject.FindGameObjectWithTag(PlayerTag);
		gameDialogueObject = FindObjectOfType(typeof (GameDialogue)) as GameDialogue;
		scenarioControlObject = FindObjectOfType(typeof (ScenarioControl)) as ScenarioControl;
		backGroundMusicPlayer = FindObjectOfType(typeof(BackgroundMusicPlayer)) as BackgroundMusicPlayer;
		checkPointManager = FindObjectOfType(typeof(CheckPoint)) as CheckPoint;
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnEnable()
	{
		Instance = this;
	}
	
	/// <summary>
	/// Sends a GameEvent.
	/// </summary>
    public static void OnGameEvent(GameEvent gameEvent)
    {
		if(gameEvent.delaySend > 0)
		{
			Instance.StartCoroutine("GameEventWithDelay", gameEvent);
		}
		else 
		{
			Instance.ProcessGameEvent(gameEvent);
		}
    }
	
    IEnumerator GameEventWithDelay(GameEvent gameEvent)
	{
		yield return new WaitForSeconds(gameEvent.delaySend);
		ProcessGameEvent(gameEvent);
	}
	
	void ProcessGameEvent(GameEvent gameEvent)
	{
	    switch(gameEvent.type)
		{
			//GameDialogue object display dialogue text by dialog id.
		    case GameEventType.ShowGameDialogue:
			  this.gameDialogueObject.OnGameEvent(gameEvent);
			  break;
		    case GameEventType.WhiteInScenarioCamera:
		    case GameEventType.WhiteOutScenarioCamera:
			case GameEventType.StartScenario:
			case GameEventType.ScenarioComplete:
		    case GameEventType.ScenarioCameraOff:
			case GameEventType.ScenarioCameraOn:
            case GameEventType.PlayerCameraOn:
			case GameEventType.PlayerCameraOff:
            case GameEventType.PlayerCameraAudioListenerOff:
			case GameEventType.PlayerCameraAudioListenerOn:
			case GameEventType.ScenarioCameraAudioListenerOn:
			case GameEventType.ScenarioCameraAudioListenerOff:
		    case GameEventType.StartDocking:
		    case GameEventType.PlayerCameraSlowMotionOnFixedPoint:
			case GameEventType.PlayerCameraSlowMotionOnTransform:
			  this.scenarioControlObject.OnGameEvent(gameEvent);
			  break;
		    case GameEventType.PlayerSetToActive:
			  player.transform.root.gameObject.SetActiveRecursively(true);
			  break;
			case GameEventType.PlayerSetToInactive:
			  player.transform.root.gameObject.SetActiveRecursively(false);
			  break;
		    case GameEventType.PlayerControlOn:
			case GameEventType.PlayerControlOff:
		    case GameEventType.WhiteInPlayerCamera:
			case GameEventType.WhiteOutPlayerCamera:
			  player.transform.root.BroadcastMessage("OnGameEvent", gameEvent, SendMessageOptions.DontRequireReceiver);
			  break;
		    case GameEventType.NPCPlayAnimation:
			  gameEvent.receiver.animation.CrossFade(gameEvent.StringParameter);
			  break;
		    case GameEventType.NPCFaceToPlayer:
			  gameEvent.sender.transform.LookAt(new Vector3(player.transform.position.x, gameEvent.sender.transform.position.y, player.transform.position.z));
			  break;
		    case GameEventType.NPCStopPlayingAnimation:
			  gameEvent.receiver.animation.Stop(gameEvent.StringParameter);
			  break;
		    case GameEventType.NPCStartAI:
			case GameEventType.NPCStartDefaultAI:
			  gameEvent.receiver.SendMessage("OnGameEvent", gameEvent);
			  break;
		    case GameEventType.LevelAreaStartSpawn:
			  LevelArea.GetArea(gameEvent.StringParameter).StartSpawn();
			  break;
		    case GameEventType.DeactivateGameObject:
			  Util.DeactivateRecurrsive(gameEvent.receiver);
			  break;
			case GameEventType.DestroyGameObject:
			  Destroy(gameEvent.receiver, gameEvent.FloatParameter);
			  break;
		    case GameEventType.ActivateGameObject:
			   Util.ActivateRecurrsive(gameEvent.receiver);
			   break;
		    case GameEventType.SpecifiedSpawn:
			  gameEvent.receiver.SendMessage("OnGameEvent", gameEvent);
			  break;
		    case GameEventType.InvokeMethod:
			  if(gameEvent.parameterType == ParameterType.None)
			     gameEvent.receiver.SendMessage(gameEvent.CustomMessage);
			  else if(gameEvent.parameterType == ParameterType.Int)
				 gameEvent.receiver.SendMessage(gameEvent.CustomMessage, gameEvent.IntParameter);
			  else if(gameEvent.parameterType == ParameterType.Float)
				 gameEvent.receiver.SendMessage(gameEvent.CustomMessage, gameEvent.FloatParameter);
			  else if(gameEvent.parameterType == ParameterType.String)
				 gameEvent.receiver.SendMessage(gameEvent.CustomMessage, gameEvent.StringParameter);
			  else if(gameEvent.parameterType == ParameterType.Bool)
				 gameEvent.receiver.SendMessage(gameEvent.CustomMessage, gameEvent.BoolParameter);
			  break;
		     case GameEventType.SetLanguage:
			     SystemLanguage systemlanguage = (SystemLanguage)System.Enum.Parse(typeof(SystemLanguage),gameEvent.StringParameter);
			     if(Persistence.GetPlayerLanguage() != systemlanguage)
			     {
			        Persistence.SetPlayerLanguage(systemlanguage);
			        Debug.Log("Change language:" + systemlanguage);
			     }
			     break;
			 case GameEventType.SetQuality:
			     Persistence.SetPlayerQualityLevel(gameEvent.IntParameter);
			     break;
		     case GameEventType.LoadLevel:
			     LoadLevel(gameEvent);
			     break;
		     case GameEventType.Mute:
			     Persistence.Mute();
			     break;
		     case GameEventType.UnMute:
			     Persistence.Unmute();
			     break;
		     case GameEventType.ForceToPhysicsCharacter:
			     gameEvent.receiver.SendMessage("OnGameEvent", gameEvent);
			     break;
		     case GameEventType.PlayBackgroundMusic:
			 case GameEventType.StopBackgroundMusic:
			     backGroundMusicPlayer.OnGameEvent(gameEvent);
			     break;
		}
	}
	
	void LoadLevel(GameEvent gameEvent)
	{
		switch(gameEvent.parameterType)
		{
		case ParameterType.Int:
			Application.LoadLevel(gameEvent.IntParameter);
			break;
		case ParameterType.String:
			Application.LoadLevel(gameEvent.StringParameter);
			break;
		}
	}
	
    public static void SendAIMessage(string message, object parameter)
    {
        for (int i=0; i<Instance.Units.Count; i++)
        {
            Unit unit = Instance.Units[i];
            if (unit == null)
            {
                Instance.Units.RemoveAt(i);
            }
            else
            {
                unit.SendMessage(message, parameter);
            }
        }
    }
}
