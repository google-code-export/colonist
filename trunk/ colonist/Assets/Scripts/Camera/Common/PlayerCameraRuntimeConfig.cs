using UnityEngine;
using System.Collections;

/// <summary>
/// Player camera runtime config parameter.
/// It handles runtime change on player camera topdown parameter.
/// </summary>
public class PlayerCameraRuntimeConfig : MonoBehaviour, I_GameEventReceiver {
	
	/// <summary>
	/// The top down camera control parameters will is defined by designer.
	/// The parameter will be pplied runtime by GameEvent with type = ApplyTopdownCameraParameter, stringParameter = cameraParameter name.
	/// </summary>
	public TopDownCameraControlParameter[] topDownCameraControlParameters = new TopDownCameraControlParameter[]{};
	
	ScenarioControl scenarioControlInstance = null;
	TopDownCamera playerTopdownCamera = null;
	
	void Start()
	{
		scenarioControlInstance = (ScenarioControl)FindObjectOfType(typeof(ScenarioControl));
		playerTopdownCamera = scenarioControlInstance.PlayerCamera.GetComponent<TopDownCamera>();
	}
	
	public void OnGameEvent(GameEvent _event)
	{
		switch(_event.type)
		{
		case GameEventType.ApplyTopdownCameraParameter:
			
			break;
		case GameEventType.ResetTopdownCameraParameter:
			
			break;
		}
	}
}
