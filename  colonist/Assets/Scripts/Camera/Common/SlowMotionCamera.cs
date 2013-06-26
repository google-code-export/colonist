using UnityEngine;
using System.Collections;

public enum SlowMotionCameraFocusMode
{
	/// <summary>
	/// Camera focus on a fixed point.
	/// </summary>
	FixedPoint = 0,
	/// <summary>
	/// Camera focus on a movable transform.
	/// </summary>
	OnTransform = 1,
}

public class SlowMotionCamera : TopDownCamera, I_GameEventReceiver
{
	SlowMotionCameraFocusMode slowMotionCameraMode = SlowMotionCameraFocusMode.FixedPoint;
	/// <summary>
	/// When slow motion camera starts, look at the LookAt transform
	/// </summary>
	public Vector3 LookAt = Vector3.zero;
	public Transform LookAtTransfrom = null;
	
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
	}
	
	void Update ()
	{
		if (Working) {
			//if slow motion time out, 
			//1. set false the flag, 
			//2. change the timescale to 1.
			//3. activate the previous camera script.
			if ((Time.time - SlowMotionStartTime) >= SlowMotionTimeLength_Overrided) {
				Working = false;
				Time.timeScale = 1;
				previousCameraScript.Working = true;
			} else {
				//incresing the time scale.
				float percentage = (Time.time - SlowMotionStartTime) / SlowMotionTimeLength_Overrided;
				Time.timeScale = Mathf.Lerp (SlowMotionTimeScale, 1, percentage);
			}
		}
	}
	
	void LateUpdate ()
	{
		if (Working) {
			switch (slowMotionCameraMode) {
			case SlowMotionCameraFocusMode.FixedPoint:
				ApplyCameraControlParameter (true, LookAt);
				break;
			case SlowMotionCameraFocusMode.OnTransform:
				ApplyCameraControlParameter (true, LookAtTransfrom.position);
				break;
			}
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
			this.LookAt = _target.collider != null ? _target.collider.bounds.center : _target.position;
			StartSlowMotion_FixedPoint (TimeLength);
			break;
			
		case GameEventType.PlayerCameraSlowMotionOnTransform:
			GameObject focused = _e.GameObjectParameter;
			this.LookAtTransfrom = focused.transform;
			StartSlowMotion_MovableTransform (TimeLength);
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
		ApplyCameraControlParameter (false, LookAt);
		Time.timeScale = SlowMotionTimeScale;
		Working = true;
		//preserves the previous active runtime camera script
		RuntimeCameraControl previousWorkingCameraScript = DisableCurrentCameraScript ();
		if (previousWorkingCameraScript != null) {
			this.previousCameraScript = previousWorkingCameraScript;
		}
		this.slowMotionCameraMode = SlowMotionCameraFocusMode.FixedPoint;
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
		ApplyCameraControlParameter (false, LookAtTransfrom.position);
		Time.timeScale = SlowMotionTimeScale;
		Working = true;
		//preserves the previous active runtime camera script
		RuntimeCameraControl previousWorkingCameraScript = DisableCurrentCameraScript ();
		if (previousWorkingCameraScript != null) {
			this.previousCameraScript = previousWorkingCameraScript;
		}
		this.slowMotionCameraMode = SlowMotionCameraFocusMode.OnTransform;
	}
	
	/// <summary>
	/// Disables the current running camera script and return it.
	/// </summary>
	RuntimeCameraControl DisableCurrentCameraScript ()
	{
		RuntimeCameraControl _previousCameraScript = null;
		foreach (RuntimeCameraControl cameraScript in transform.GetComponents<RuntimeCameraControl>()) {
			if (cameraScript != this && cameraScript.Working == true) {
				_previousCameraScript = cameraScript;
				cameraScript.Working = false;
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
			previousCameraScript.Working = true;
		}
		if (this.Working == true) {
			this.Working = false;
		}
	}
}
