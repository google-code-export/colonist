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
	
	public Transform Focus = null;
	
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

	void Awake ()
	{
		PlayerCharacter = transform.root.GetComponentInChildren<CharacterController> ();
	}

	// Use this for initialization
	void Start ()
	{
		//restore the original position
		originPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	void LateUpdate ()
	{
		if (Working) {
			//avoid too frequent update
			if (Time.time - CameraLastDampTime >= CameraDampInterval) {
				if(Focus == null)
				{
				   SetPosition (true, GetCharacterCenter ());
				}
				else 
				{
				   SetPosition (true, Focus.position);
				}
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

	protected void SetPosition (bool smoothDamp, Vector3 CharacterCenter)
	{
		//Vector3 newPosition = characterCeneter + Vector3.up * DynamicLockHeight;
		//Vector3 NewPositionOffset = Character.transform.TransformDirection(Vector3.back) * Mathf.Abs(DynamicLockDistance);
		if (LevelManager.Instance != null && LevelManager.Instance.ControlDirectionPivot != null) {
			Vector3 NewPositionOffset = LevelManager.Instance.ControlDirectionPivot.TransformDirection (Vector3.back) * topDownCameraParameter.DynamicDistance;
			NewPositionOffset += Vector3.up * topDownCameraParameter.DynamicHeight;
			Vector3 newPosition = (smoothDamp) ?
                                   Vector3.SmoothDamp (transform.position, CharacterCenter + NewPositionOffset, ref dampingVelocity, topDownCameraParameter.smoothLag) 
                                   : CharacterCenter + NewPositionOffset;
			newPosition = AdjustLineOfSight (newPosition, CharacterCenter);
			transform.position = newPosition;
		} else 
			Debug.LogWarning ("Warning! NO Level manager instance found in scene, can not set position of the camera");
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
		if (Physics.Linecast (target, newPosition, out hit, topDownCameraParameter.lineOfSightMask.value)) {
			dampingVelocity = Vector3.zero;
			return hit.point;
		}
		return newPosition;
	}

	void OnEnable ()
	{
		Debug.Log ("Position damp immediately!");
		SetPosition (false, GetCharacterCenter ());
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

	public IEnumerator SlowMotion (Vector3 LookAtPoint)
	{
		yield return null;
	}
}
