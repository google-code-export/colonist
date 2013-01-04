using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ThirdPersonFollowCamera_Predator : MonoBehaviour
{

	public PredatorPlayerStatus PredatorPlayerStatus = null;

	/// <summary>
	/// The position pivot to aligh this camera to, used in camera relative mode
	/// </summary>
	public Transform PositionPivot = null;
	/// <summary>
	/// The transform to look at,used in camera relative mode
	/// </summary>
	public Transform LookAt = null;
	public float DynamicDistance = 1;
	public float DynamicHeight = 1.5f;

	/// <summary>
	/// The target transform to look at. If this is null, than will pick DefaultViewPoint to look at.
	/// </summary>
	//[HideInInspector]
	//public Transform LookAtTarget = null;
	public float smoothLag = 0.3f;

	/// <summary>
	/// Layer mask to check if the current view sight has been blocked.
	/// </summary>
	public LayerMask lineOfSightMask = 0;

	/// <summary>
	/// The character which the camera is currenting viewing on
	/// </summary>
	public CharacterController Character = null;
	private Vector3 dampingVelocity = new Vector3 ();
    
	/// <summary>
	/// Determined 
	/// </summary>
	private CameraDampMode cameraDampMode = CameraDampMode.ByTransform;
	
	private bool isShake = false;
	public float shake_decay = 0.002f;
	public float shake_intensity = 1.0f;
	public float shake_interval = 0.02f;
    private Vector3 originPosition ;
	enum CameraDampMode
	{
		ByParameter = 0,
		ByTransform = 1
	}

	void Awake ()
	{
		SetCameraModelByPredatorStatus ();
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
        //if (Input.GetKey (KeyCode.F)) {
        //    isShake = true;
        //    StartCoroutine ("Shake");
        //}
	}

	void LateUpdate ()
	{
		if (!isShake) {
			if (cameraDampMode == CameraDampMode.ByTransform) {
				//Position damping
				PositionDampingByTransform ();
				//Rotation damping
				if (LookAt)
					transform.LookAt (this.LookAt);
			} else if (cameraDampMode == CameraDampMode.ByParameter) {
				PositionDampingByParameter (true);
			}
		}
	}

	private Vector3 GetCharacterCenter ()
	{
		return Character.transform.position + Character.center;
	}

	private void PositionDampingByParameter (bool smoothDamp)
	{
		Vector3 characterCeneter = GetCharacterCenter ();
		//Vector3 newPosition = characterCeneter + Vector3.up * DynamicLockHeight;
		//Vector3 NewPositionOffset = Character.transform.TransformDirection(Vector3.back) * Mathf.Abs(DynamicLockDistance);
		Vector3 NewPositionOffset = LevelManager.Instance.ControlDirectionPivot.TransformDirection (Vector3.back) * Mathf.Abs (DynamicDistance);
		NewPositionOffset += Vector3.up * DynamicHeight;
		Vector3 newPosition = (smoothDamp) ?
            Vector3.SmoothDamp (transform.position, GetCharacterCenter () + NewPositionOffset, ref dampingVelocity, smoothLag) 
            :
            characterCeneter + NewPositionOffset;
		newPosition = AdjustLineOfSight (newPosition, GetCharacterCenter ());
		transform.position = newPosition;
		//Debug.DrawRay(Character.transform.position + Character.center, backOffset,Color.red, 5);
	}

	private void PositionDampingByTransform ()
	{
		Vector3 newPosition;
		newPosition = Vector3.SmoothDamp (transform.position, PositionPivot.position, ref dampingVelocity, smoothLag);
		//If the sight has been blocked by colliders, zoom in the camera to ensure the character's always visible
		newPosition = AdjustLineOfSight (newPosition, Character.transform.position + Character.center);
		transform.position = newPosition;
	}

	/// <summary>
	/// If current camera sign being obstacled by object in layer, return the closet unhidden point
	/// </summary>
	/// <param name="newPosition"></param>
	/// <param name="target"></param>
	/// <returns></returns>
	private Vector3 AdjustLineOfSight (Vector3 newPosition, Vector3 target)
	{
		RaycastHit hit;
		if (Physics.Linecast (target, newPosition, out hit, lineOfSightMask.value)) {
			dampingVelocity = Vector3.zero;
			return hit.point;
		}
		return newPosition;
	}

	private void SetCameraModelByPredatorStatus ()
	{
		switch (PredatorPlayerStatus.PlayerControlMode) {
		case MovementControlMode.CameraRelative:
			cameraDampMode = CameraDampMode.ByTransform;
			break;
		case MovementControlMode.CharacterRelative:
		default:
			cameraDampMode = CameraDampMode.ByParameter;
			break;
		}
	}

	void OnEnable ()
	{
		SetCameraModelByPredatorStatus ();
		//Debug.Log("Camera mode:" + cameraDampMode);
		//add 20121227
		
		if (cameraDampMode == CameraDampMode.ByTransform) {
			transform.position = PositionPivot.position;
			//Rotation damping
			if (LookAt)
				transform.LookAt (this.LookAt);
		} else if (cameraDampMode == CameraDampMode.ByParameter) {
			Debug.Log ("Position damp immediately!");
			PositionDampingByParameter (false);
			transform.LookAt (Character.center + Character.transform.position);
		}
	}
	
	/// <summary>
	/// Shake from
	/// </summary>
	IEnumerator Shake ()
	{
		if (shake_intensity > 0) {
			transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
			shake_intensity -= shake_decay;
		}
        yield return new WaitForSeconds(shake_interval);
		isShake =false;
	}

 
}
