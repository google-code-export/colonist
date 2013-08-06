using UnityEngine;
using System.Collections;

public class SlowMotionCamera : TopDownCamera, I_GameEventReceiver
{	
	/// <summary>
	/// How long the slow motion last, in seconds
	/// </summary>
	public float SlowMotionDuration = 1.5f;
	
	/// <summary>
	/// When slow motioning, the time scale.
	/// </summary>
	public float SlowMotionTimeScale = 0.2f;
	float SlowMotionStartTime = 0;
	RuntimeCameraControl previousCameraScript = null;
	float SlowMotionTimeLength_Overrided;
	
	void Awake ()
	{
		SlowMotionTimeLength_Overrided = SlowMotionDuration;
		CurrentTopDownCameraParameter = topDownCameraParameter;
	}
	
	void Update ()
	{
		if(LevelManager.IsPause() == true)
		{
			return;
		}
		else {
		   //if slow motion time out, 
		   //1. set false the flag, 
		   //2. change the timescale to 1.
		   //3. activate the previous camera script.
		   if ((Time.time - SlowMotionStartTime) >= SlowMotionTimeLength_Overrided) {
				this.enabled = false;
				Time.timeScale = 1;
				previousCameraScript.enabled = true;
		   } 
		   else 
		   {
			//incresing the time scale.
			float percentage = (Time.time - SlowMotionStartTime) / SlowMotionTimeLength_Overrided;
			Time.timeScale = Mathf.Lerp (SlowMotionTimeScale, 1, percentage);
		   }
		}
	}
	
	void LateUpdate ()
	{
		
			switch (CurrentTopDownCameraParameter.mode) {
			case TopDownCameraControlMode.ParameterControlPositionAndLookAtPosition:
				ApplyCameraControlParameter (true, CurrentTopDownCameraParameter);
				break;
			case TopDownCameraControlMode.ParameterControlPositionAndLookAtTransform:
				//it's possible, that the focus gameobject is destroyed afterward, in runtime, so we avoid the error by checking cameraFocusOnTransform.
				if(CurrentTopDownCameraParameter.cameraFocusOnTransform != null)
				{
				   ApplyCameraControlParameter (true, CurrentTopDownCameraParameter);
				}
				else 
				{
					this.enabled = false; //in this case, reset the working status.
					Time.timeScale = 1;
					previousCameraScript.enabled = true;
				}
				break;
			}
		
	}
	
	public void OnGameEvent (GameEvent _e)
	{
		float? TimeLength = null;
		if (_e.parameterType == ParameterType.Float) {
			TimeLength = _e.FloatParameter;
		}
		
		switch (_e.type) {
		case GameEventType.PlayerCameraSlowMotionOnFixedPoint:
			Transform _target = _e.sender.transform;
			this.CurrentTopDownCameraParameter.cameraFocusOnPosition = _target.collider != null ? _target.collider.bounds.center : _target.position;
			//change camera mode:
			CurrentTopDownCameraParameter.mode = TopDownCameraControlMode.ParameterControlPositionAndLookAtPosition;
			StartSlowMotion_FixedPoint (TimeLength);
			break;
			
		case GameEventType.PlayerCameraSlowMotionOnTransform:
			if(_e.GameObjectParameter == null)
			{
				break;
			} 
			//do nothing, if the gameObjectParameter is missing.(It can be destroyed in runtime)
			else 
			{
			  //change camera mode:
			  CurrentTopDownCameraParameter.mode = TopDownCameraControlMode.ParameterControlPositionAndLookAtTransform;
			  this.CurrentTopDownCameraParameter.cameraFocusOnTransform = _e.GameObjectParameter.transform;
			  StartSlowMotion_MovableTransform (TimeLength);
			}
			break;
		}
	}
	
	/// <summary>
	/// Call this method to start the slow motion. The camera will focus at the fixed world point.
	/// If OverridedTimelength is not null, the timelength will override the defined timeLength value in
	/// Inspector.
	/// </summary>
	void StartSlowMotion_FixedPoint (float? OverridedTimelength)
	{
		if (OverridedTimelength.HasValue) {
			SlowMotionTimeLength_Overrided = OverridedTimelength.Value;
		} else {
			//Use the default defined SlowMotion Duration:
			SlowMotionTimeLength_Overrided = this.SlowMotionDuration;
		}
		SlowMotionStartTime = Time.time;
		ApplyCameraControlParameter (false, CurrentTopDownCameraParameter);
		Time.timeScale = SlowMotionTimeScale;
		enabled = true;
		//preserves the previous active runtime camera script
		RuntimeCameraControl previousWorkingCameraScript = DisableCurrentCameraScript ();
		if (previousWorkingCameraScript != null) {
			this.previousCameraScript = previousWorkingCameraScript;
		}
		
	}
	
	
	/// <summary>
	/// Starts the slow motion_camera, and camera will keep dynamically focusing
	/// the transform.
	/// If timelength is not null, the timelength will override the defined timeLength value in
	/// Inspector.
	/// </summary>
	/// <param name='timelength'>
	/// Timelength.
	/// </param>
	void StartSlowMotion_MovableTransform (float? OverridedTimelength)
	{
		if (OverridedTimelength.HasValue) {
			SlowMotionTimeLength_Overrided = OverridedTimelength.Value;
		} else {
			//Use the default defined SlowMotion Duration:
			SlowMotionTimeLength_Overrided = this.SlowMotionDuration;
		}
		SlowMotionStartTime = Time.time;
		ApplyCameraControlParameter (false, CurrentTopDownCameraParameter);
		Time.timeScale = SlowMotionTimeScale;
		enabled = true;
		//preserves the previous active runtime camera script
		RuntimeCameraControl previousWorkingCameraScript = DisableCurrentCameraScript ();
		if (previousWorkingCameraScript != null) {
			this.previousCameraScript = previousWorkingCameraScript;
		}
	}
	
	/// <summary>
	/// Disables the current running camera script and return it.
	/// </summary>
	RuntimeCameraControl DisableCurrentCameraScript ()
	{
		RuntimeCameraControl _previousCameraScript = null;
		foreach (RuntimeCameraControl cameraScript in transform.GetComponents<RuntimeCameraControl>()) {
			if (cameraScript != this && cameraScript.enabled == true) {
				_previousCameraScript = cameraScript;
				cameraScript.enabled = false;
			}
		}
		return _previousCameraScript;
	}
	
	/// <summary>
	/// In case the script is disabled in the middle of scenario, so we set the timescale to 1.
	/// </summary>
	void OnDisable ()
	{
		Time.timeScale = 1;
		if (previousCameraScript != null) {
			previousCameraScript.enabled = true;
		}
		if (this.enabled == true) {
			this.enabled = false;
		}
	}
}
