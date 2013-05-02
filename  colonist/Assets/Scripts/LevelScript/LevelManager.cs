using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Game global manager
/// </summary>
[ExecuteInEditMode]
public class LevelManager : MonoBehaviour {
	
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
	/// <summary>
	/// The MagicalNumberGUITexture object which display the DamageHint number. 
	/// </summary>
	
    void Awake()
    {
        Instance = this;
		player = GameObject.FindGameObjectWithTag(PlayerTag);
		gameDialogueObject = FindObjectOfType(typeof (GameDialogue)) as GameDialogue;
		scenarioControlObject = FindObjectOfType(typeof (ScenarioControl)) as ScenarioControl;
		
    }

	// Use this for initialization
	void Start () {
		GameEvent _e = new GameEvent();
		_e.type = GameEventType.LevelStart;
		_e.receiver = this.gameObject;
        GameEvent(_e);
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
    public static void GameEvent(GameEvent gameEvent)
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
			case GameEventType.ScenarioCameraDockComplete:
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
		    case GameEventType.PlayerCameraWhiteIn:
			case GameEventType.PlayerCameraWhiteOut:
			  player.transform.root.BroadcastMessage("OnGameEvent", gameEvent, SendMessageOptions.DontRequireReceiver);
			  break;
		    case GameEventType.NPCPlayAnimation:
		    case GameEventType.NPCStartAI:
			  gameEvent.receiver.SendMessage("OnGameEvent", gameEvent);
			  break;
		    case GameEventType.LevelAreaStartSpawn:
			  LevelArea.GetArea(gameEvent.StringParameter).StartSpawn();
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
