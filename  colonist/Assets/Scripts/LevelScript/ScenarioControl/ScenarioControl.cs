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
	
	public static ScenarioControl Instance = null;
	
	bool isPlayingScenario = false;
	
	public float SkippingTimeScale = 3;
	IDictionary<string, ObjectDock> ObjectDockMonoDict = new Dictionary<string, ObjectDock>();
	void Awake()
	{
		Instance = this;
		foreach(Camera c in Object.FindObjectsOfType(typeof(Camera)))
		{
			if(c.tag == "PlayerCamera")
			{
				PlayerCamera = c;
			}
			if(c.tag == "ScenarioCamera")
			{
				ScenarioCamera = c;
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
        case GameEventType.StartScenario:
			isPlayingScenario = true;
			break;
		case GameEventType.ScenarioComplete:
			isPlayingScenario = false;
			Time.timeScale = 1;
			break;
		case GameEventType.ScenarioCameraOff:
		    this.ScenarioCamera.enabled = false;
			break;
		case GameEventType.ScenarioCameraOn:
		    this.ScenarioCamera.enabled = true;
			break;
		case GameEventType.PlayerCameraOn:
		    this.PlayerCamera.enabled = true;
			break;
        case GameEventType.PlayerCameraOff:		    
			this.PlayerCamera.enabled = false;
			break;
        case GameEventType.ScenarioCameraAudioListenerOn:
		    this.ScenarioCamera.GetComponent<AudioListener>().enabled = true; 
			break;
		case GameEventType.ScenarioCameraAudioListenerOff:
		    this.ScenarioCamera.GetComponent<AudioListener>().enabled = false;
			break;
		case GameEventType.PlayerCameraAudioListenerOn:
		    this.PlayerCamera.GetComponent<AudioListener>().enabled = true;
			break;
        case GameEventType.PlayerCameraAudioListenerOff:		    
			this.PlayerCamera.GetComponent<AudioListener>().enabled = false;
			break;
		case GameEventType.StartDocking:
			ObjectDockMonoDict[gameEvent.StringParameter].Dock();
			break;
		case GameEventType.PlayerCameraSlowMotionOnFixedPoint:
		case GameEventType.PlayerCameraSlowMotionOnTransform:
			this.PlayerCamera.SendMessage("OnGameEvent", gameEvent);
			break;
		}
	}
}
