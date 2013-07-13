using UnityEngine;
using System.Collections;

/// <summary>
/// Top down camera control script.
/// </summary>
[ExecuteInEditMode]
public class TopDownCamera : RuntimeCameraControl
{
	/// <summary>
	/// The camera control parameter for player camera.
	/// </summary>
	public TopDownCameraControlParameter topDownCameraParameter;
	
	/// <summary>
	/// The character which the camera is currenting viewing on
	/// </summary>
	private CharacterController PlayerCharacter = null;
	private Vector3 dampingVelocity = new Vector3 ();
	private bool ShouldAutoAdjustCamera = true;
	public float shake_decay = 0.002f;
	public float shake_intensity = 1.0f;
	public float shake_interval = 0.02f;
	private bool isShaking = false;
	private Vector3 originPosition;
	private float CameraDampInterval = 0.02f;
	private float CameraLastDampTime = 0;
	
	/// <summary>
	/// The current top down camera parameter.
	/// Can be overrided at runtime.
	/// By default, the predefined parameter is used.
	/// </summary>
	public TopDownCameraControlParameter CurrentTopDownCameraParameter = null;
	
	void Awake ()
	{
		PlayerCharacter = transform.root.GetComponentInChildren<CharacterController> ();
		CurrentTopDownCameraParameter = topDownCameraParameter;
	}

	// Use this for initialization
	void Start ()
	{
		//restore the original position
		originPosition = transform.position;
	}

	void LateUpdate ()
	{
		if (Working) {
			//avoid too frequent update
			if (Time.time - CameraLastDampTime >= CameraDampInterval) {
				ApplyCameraControlParameter (true, this.CurrentTopDownCameraParameter);
				CameraLastDampTime = Time.time;
			}
		}
	}

	private Vector3 GetCharacterCenter ()
	{
		if (PlayerCharacter == null)
			PlayerCharacter = transform.root.GetComponentInChildren<CharacterController> ();
		return PlayerCharacter.transform.position + PlayerCharacter.center;
	}
	
	public void SetCurrentTopdownCameraParameter(TopDownCameraControlParameter topdownCameraControlParameter)
	{
		this.CurrentTopDownCameraParameter = topdownCameraControlParameter;
	}
	
	public void ResetCurrentTopdownCameraParameter()
	{
		this.CurrentTopDownCameraParameter = this.topDownCameraParameter;
	}
	
	/// <summary>
	/// Applies the camera control parameter to the current camera.
	/// smoothDamp - if smoothly damp from current camera position ? if false, the camera control parameter takes effect immediately.
	/// CharacterCenter - the camera center.
	/// topdownCameraControlParameter - the topDownCameraControlParameter.
	/// </summary>
	public virtual void ApplyCameraControlParameter (bool SmoothDamp,
		                                             TopDownCameraControlParameter topdownCameraControlParameter)
	{
		AlignCameraPositionAndRotation (SmoothDamp, topdownCameraControlParameter);
	}
	
	/// <summary>
	/// Calculates the parameter control position by height, and far(distance) against the center position.
	/// height - Axis.Y to the center.
	/// far = Axis.(-Z) to the center(relative to LevelManager.ControlDirectionPivot, NOT RELATIVE TO REAL WORLD COORDINATE!!!)
	/// </summary>
	Vector3? CalculateParameterControlPosition (bool SmoothDamp, float height, float far, float smoothLag, Vector3 center)
	{
		Vector3? newPosition = null;
		if (LevelManager.Instance != null && LevelManager.Instance.ControlDirectionPivot != null) {
			Vector3 NewPositionOffset = LevelManager.Instance.ControlDirectionPivot.TransformDirection (Vector3.back) * far;
			NewPositionOffset += Vector3.up * height;
			newPosition = (SmoothDamp) ?
                                   Vector3.SmoothDamp (transform.position, center + NewPositionOffset, ref dampingVelocity, smoothLag) 
                                   : center + NewPositionOffset;
		} else {
			Debug.LogWarning ("Warning! NO Level manager instance found in scene, can not set position of the camera");
		}
		return newPosition;
	}
	
	/// <summary>
	/// Calculates the camera top parameter position and rotation
	/// If the topdown mode = Default | ParameterControl , the position is runtime calculated.
	/// If the topdown mode = PositionAtPivot, the position is directly set to the pivot transform's position.
	/// </summary>	
	void AlignCameraPositionAndRotation (bool smoothDamp, TopDownCameraControlParameter topdownCameraControlParameter)
	{
		Vector3 CameraPivotPosition = Vector3.zero;
		Vector3? CameraLookAtPosition = null;
		Vector3? ParameterCalculatePosition = null;
		
		switch (topdownCameraControlParameter.mode) {
			
			//note: for Default mode, CameraLookAtPosition IS NOT ASSIGNED ! Because in runtime, we can't focus camera on player, that will shake the camera too much!
		case TopDownCameraControlMode.Default:
			ParameterCalculatePosition = CalculateParameterControlPosition (smoothDamp, topdownCameraControlParameter.DynamicHeight, 
				                                                           topdownCameraControlParameter.DynamicDistance, 
				                                                           topdownCameraControlParameter.smoothLag_Position, 
				                                                           GetCharacterCenter ());
			CameraPivotPosition = ParameterCalculatePosition.Value;
			CameraLookAtPosition = GetCharacterCenter();
//			CameraPivotPosition = AdjustLineOfSight (CameraPivotPosition, GetCharacterCenter());
			break;
		case TopDownCameraControlMode.ParameterControlPositionAndLookAtPosition:
			ParameterCalculatePosition = CalculateParameterControlPosition (smoothDamp, topdownCameraControlParameter.DynamicHeight, 
				                                                           topdownCameraControlParameter.DynamicDistance, 
				                                                           topdownCameraControlParameter.smoothLag_Position, 
				                                                           topdownCameraControlParameter.cameraFocusOnPosition);
			CameraLookAtPosition = topdownCameraControlParameter.cameraFocusOnPosition;
			CameraPivotPosition = ParameterCalculatePosition.Value;
			break;
		case TopDownCameraControlMode.ParameterControlPositionAndLookAtTransform:
			ParameterCalculatePosition = CalculateParameterControlPosition (smoothDamp, topdownCameraControlParameter.DynamicHeight, 
				                                                           topdownCameraControlParameter.DynamicDistance, 
				                                                           topdownCameraControlParameter.smoothLag_Position, 
				                                                           topdownCameraControlParameter.cameraFocusOnTransform.position);
			CameraLookAtPosition = topdownCameraControlParameter.cameraFocusOnTransform.position;
			CameraPivotPosition = ParameterCalculatePosition.Value;
			break;
		case TopDownCameraControlMode.PoisitonAtPivotAndLookAtPlayer:
			CameraPivotPosition = topdownCameraControlParameter.CameraPositionPivot.position;
			CameraLookAtPosition = GetCharacterCenter ();
			break;
		case TopDownCameraControlMode.PositionAtPivotAndLookAtPosition:
			CameraPivotPosition = topdownCameraControlParameter.CameraPositionPivot.position;
			CameraLookAtPosition = topdownCameraControlParameter.cameraFocusOnPosition;
			break;
		case TopDownCameraControlMode.PositionAtPivotAndLookAtTransform:
			CameraPivotPosition = topdownCameraControlParameter.CameraPositionPivot.position;
			CameraLookAtPosition = topdownCameraControlParameter.cameraFocusOnTransform.position;
			break;
		}
		
		//finally, asign the position and rotation of the camera.
		
		transform.position = CameraPivotPosition;
		if(CameraLookAtPosition.HasValue)
		{
//		   transform.LookAt (CameraLookAtPosition.Value);
		   if(smoothDamp)
		   {
		      Quaternion ToRotation = transform.rotation;
		      ToRotation.SetLookRotation(CameraLookAtPosition.Value -transform.position, Vector3.up);
		      transform.rotation = Quaternion.Lerp(transform.rotation, ToRotation, topdownCameraControlParameter.smoothLag_Rotation);
			  transform.rotation = ToRotation;
//				transform.LookAt (CameraLookAtPosition.Value);
		   }
		   else
		   {
			  transform.LookAt (CameraLookAtPosition.Value);
		   }
		   CameraPivotPosition = AdjustLineOfSight (CameraPivotPosition, CameraLookAtPosition.Value);
		}
	}

	/// <summary>
	/// If current camera sign being obstacled by object in layer, return the closet unhidden point
	/// </summary>
	/// <param name="newPosition"></param>
	/// <param name="target"></param>
	/// <returns></returns>
	protected Vector3 AdjustLineOfSight (Vector3 newPosition, Vector3 target)
	{
		RaycastHit hit;
		if (Physics.Linecast (target, newPosition, out hit, CurrentTopDownCameraParameter.lineOfSightMask.value)) {
			dampingVelocity = Vector3.zero;
			return hit.point;
		}
		return newPosition;
	}

	void OnEnable ()
	{
		//Debug.Log ("Position damp immediately!");
		ApplyCameraControlParameter (false, this.CurrentTopDownCameraParameter);
		transform.LookAt (PlayerCharacter.transform);
	}
	
	/// <summary>
	/// Shake from
	/// </summary>
	public IEnumerator Shake ()
	{
		isShaking = true;
		if (shake_intensity > 0) {
			transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
			shake_intensity -= shake_decay;
		}
		yield return new WaitForSeconds(shake_interval);
		isShaking = false;
	}
}
