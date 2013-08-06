using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The ScenarioControl component is responsible for presenting scenario/story to player.
/// Its responsibility includes : 
/// 1. Control scenario camera.
/// 2. Control character (actor) movement.
/// 3. Switch on/off player control.
/// </summary>
public class ScenarioControl : MonoBehaviour {
	
	/// <summary>
	/// The scenario camera. Which should NOT be the player camera.
	/// </summary>
	[HideInInspector]
	public Camera ScenarioCamera;
	[HideInInspector]
	public Camera PlayerCamera;
	[HideInInspector]
	public AudioListener ScenarioAudioListener;
	[HideInInspector]
	public AudioListener PlayerAudioListener;
	[HideInInspector]
	
	public static ScenarioControl Instance = null;
	
	bool isPlayingScenario = false;
	
	public float SkippingTimeScale = 3;
	IDictionary<string, ObjectDock> ObjectDockMonoDict = new Dictionary<string, ObjectDock>();
	
	void Awake()
	{
		Instance = this;
		foreach(Camera c in Object.FindObjectsOfType(typeof(Camera)))
		{
			if(c.tag.ToLower().Contains("player"))
			{
				PlayerCamera = c; 
			}
			if(c.tag.ToLower().Contains("scenario"))
			{
				ScenarioCamera = c;
			}
		}
		foreach(AudioListener listener in Object.FindObjectsOfType(typeof(AudioListener)))
		{
			if(listener.tag.ToLower().Contains("player"))
			{
				PlayerAudioListener = listener;
			}
			if(listener.tag.ToLower().Contains("scenario"))
			{
				ScenarioAudioListener = listener;
			}
		}
		
		foreach(ObjectDock dockMono in FindObjectsOfType(typeof(ObjectDock)))
		{
			ObjectDockMonoDict.Add(dockMono.Name, dockMono);
		}
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(isPlayingScenario)
		{
	      if(Input.touches.Length > 0 || Input.GetKey(KeyCode.S))
		  {
			 Time.timeScale = SkippingTimeScale;
		  }
		  else if( Time.timeScale != 1)
		  {
				Time.timeScale = 1;
		  }
		}
	}
	
	void OnDestroy()
	{
		Instance = null;
	}
	

    public void OnGameEvent(GameEvent gameEvent)
	{
		switch(gameEvent.type)
		{
		case GameEventType.WhiteInScenarioCamera:
			this.ScenarioCamera.SendMessage("WhiteIn");
			break;
		case GameEventType.WhiteOutScenarioCamera:
			this.ScenarioCamera.SendMessage("WhiteOut", gameEvent.BoolParameter);
			break;
        case GameEventType.TimeScaleOn:
			isPlayingScenario = true;
			break;
		case GameEventType.TimeScaleOff:
			isPlayingScenario = false;
			Time.timeScale = 1;
			break;
		case GameEventType.ScenarioCameraOff:
		    this.ScenarioCamera.enabled = false;
			break;
		case GameEventType.ScenarioCameraOn:
		    this.ScenarioCamera.enabled = true;
			if(ScenarioCamera.gameObject.active == false)
			{
				ScenarioCamera.gameObject.active = true;
			}
			break;
		case GameEventType.PlayerCameraOn:
		    this.PlayerCamera.enabled = true;
			break;
        case GameEventType.PlayerCameraOff:		    
			this.PlayerCamera.enabled = false;
			break;
        case GameEventType.ScenarioCameraAudioListenerOn:
		    this.ScenarioAudioListener.enabled = true; 
			break;
		case GameEventType.ScenarioCameraAudioListenerOff:
		    this.ScenarioAudioListener.enabled = false;
			break;
		case GameEventType.PlayerCameraAudioListenerOn:
		    this.PlayerAudioListener.enabled = true;
			break;
        case GameEventType.PlayerCameraAudioListenerOff:		    
			this.PlayerAudioListener.enabled = false;
			break;
		case GameEventType.StartDocking:
			ObjectDockMonoDict[gameEvent.StringParameter].Dock();
			break;
		case GameEventType.StartDockingOnRuntimeTarget:
			ObjectDockMonoDict[gameEvent.StringParameter].Dock(gameEvent.receiver.transform);
			break;
		case GameEventType.PlayerCameraSlowMotionOnFixedPoint:
		case GameEventType.PlayerCameraSlowMotionOnTransform:
			this.PlayerCamera.SendMessage("OnGameEvent", gameEvent);
			break;
		case GameEventType.ShiftToPlayerMode:
			//to shift to Player mode:
			//1. activate player
			//2. deactivate scenario camera + audio listener
			LevelManager.Instance.player.SetActive (true);
			LevelManager.Instance.player.transform.root.gameObject.SetActive(true);
			GameEvent event_player_control_on = new GameEvent(GameEventType.PlayerControlOn);
			LevelManager.OnGameEvent(event_player_control_on, this);
			PlayerCamera.enabled = true;
			PlayerAudioListener.enabled = true;
			this.ScenarioCamera.enabled = false;
			this.ScenarioAudioListener.enabled = false;
			break;
		case GameEventType.ShiftToScenarioMode:
			//to shift to scenario mode:
			//1. if BOOL parameter = false, de-activate player , else , deactivate player control and camera+audio
			//2. deactivate scenario camera + audio listener
			if(gameEvent.BoolParameter == true)
			{
				//let player visible in camera.
			   this.PlayerCamera.enabled = false;
			   this.PlayerAudioListener.enabled = false;
			   GameEvent event_player_control_off = new GameEvent(GameEventType.PlayerControlOff);
			   LevelManager.OnGameEvent(event_player_control_off, this);
			}
			else 
			{
			   LevelManager.Instance.player.transform.root.gameObject.SetActive(false);
			}
			this.ScenarioCamera.enabled = true;
			this.ScenarioAudioListener.enabled = true;
			break;
		}
	}
	
	public static void RegisterObjectDock(ObjectDock dock)
	{
		if(Instance.ObjectDockMonoDict.Keys.Contains(dock.Name) == false)
		{
		   Instance.ObjectDockMonoDict.Add(dock.Name, dock);
		}
	}
}
