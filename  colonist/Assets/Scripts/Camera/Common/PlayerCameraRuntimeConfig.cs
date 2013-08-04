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
	
	TopDownCamera PlayerTopdownCamera
	{
		get{
			return ScenarioControl.Instance.PlayerCamera.GetComponent<TopDownCamera>();;
		}
	}
	
	public void OnGameEvent(GameEvent _event)
	{
		switch(_event.type)
		{
		case GameEventType.ApplyTopdownCameraParameter:
			TopDownCameraControlParameter controlParameter = GetTopDownCameraControlParameter(_event.StringParameter);
			PlayerTopdownCamera.SetCurrentTopdownCameraParameter(controlParameter);
			break;
		case GameEventType.ResetTopdownCameraParameter:
			PlayerTopdownCamera.ResetCurrentTopdownCameraParameter();
			break;
		}
	}
	
	TopDownCameraControlParameter GetTopDownCameraControlParameter(string parameterName)
	{
		foreach(TopDownCameraControlParameter cameraParameter in this.topDownCameraControlParameters)
		{
			if(cameraParameter.Name == parameterName)
			{
				return cameraParameter;
			}
		}
		Debug.LogError("Error ! Camera parameter name:" + parameterName + " is not found!");
		return null;
	}
}
