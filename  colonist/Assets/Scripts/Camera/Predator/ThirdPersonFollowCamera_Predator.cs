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
    public bool ShouldAutoAdjustCamera = true;
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

#region slow motion camera variables
    /// <summary>
    /// Define a couple of slow motion anchors, the camera will be alighed to the anchors when slow motion behavior starts.
    /// </summary>
    public Transform[] SlowMotionAnchors = new Transform[] { };
    /// <summary>
    /// The slowest time scale.
    /// </summary>
    public float SlowestTimeScale = 0.25f;
    /// <summary>
    /// Fade in time - how many seconds it takes to slow the time scale from 1 to %SlowestTimeScale%?
    /// If SlowFadeInTime = 0, the timescale is set to %SlowestTimeScale% immediately.
    /// </summary>
    public float FadeInSlowMotionTime = 0.3f;

    /// <summary>
    /// Fade out time - how many seconds it takes to recover time sclae from %SlowestTimeScale% to 1?
    /// If SlowFadeOutTime = 0, the timescale is set to 1 immediately.
    /// </summary>
    public float FadeOutSlowMotionTime = 1f;

    private bool IsSlowMotion = false;

#endregion

	public float shake_decay = 0.002f;
	public float shake_intensity = 1.0f;
	public float shake_interval = 0.02f;

    private bool isShaking = false;
    private Vector3 originPosition;
	
	private float CameraDampInterval = 0.02f;
	private float CameraLastDampTime = 0;
    //enum CameraDampMode
    //{
    //    ByParameter = 0,
    //    ByTransform = 1
    //}

	void Awake ()
	{
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
        if (Input.GetKeyDown(KeyCode.F))
        {
            //isShake = true;
            //StartCoroutine("Shake");
            StartCoroutine("SlowMotion");
        }
	}

	void LateUpdate ()
	{
        ShouldAutoAdjustCamera = !isShaking && !IsSlowMotion;
        if (ShouldAutoAdjustCamera && (Time.time - CameraLastDampTime >= CameraDampInterval))
        {
            SetPosition (true);
			CameraLastDampTime = Time.time;
        }
	}

	private Vector3 GetCharacterCenter ()
	{
		return Character.transform.position + Character.center;
	}

	private void SetPosition (bool smoothDamp)
	{
		Vector3 characterCeneter = GetCharacterCenter ();
		//Vector3 newPosition = characterCeneter + Vector3.up * DynamicLockHeight;
		//Vector3 NewPositionOffset = Character.transform.TransformDirection(Vector3.back) * Mathf.Abs(DynamicLockDistance);
		if(LevelManager.Instance != null && LevelManager.Instance.ControlDirectionPivot != null)
		{
		   Vector3 NewPositionOffset = LevelManager.Instance.ControlDirectionPivot.TransformDirection (Vector3.back) * DynamicDistance;
		   NewPositionOffset += Vector3.up * DynamicHeight;
		   Vector3 newPosition = (smoothDamp) ?
                                   Vector3.SmoothDamp (transform.position, GetCharacterCenter () + NewPositionOffset, ref dampingVelocity, smoothLag) 
                                   : characterCeneter + NewPositionOffset;
		    newPosition = AdjustLineOfSight (newPosition, GetCharacterCenter ());
		    transform.position = newPosition;
		}
		else 
			Debug.LogWarning("Warning! NO Level manager instance found in scene, can not set position of the camera");
		
		//Debug.DrawRay(Character.transform.position + Character.center, backOffset,Color.red, 5);
        //transform.LookAt(Character.center + Character.transform.position);
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

	void OnEnable ()
	{
		Debug.Log ("Position damp immediately!");
		SetPosition (false);
        transform.LookAt(Character.transform);
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
		isShaking =false;
	}


    public IEnumerator SlowMotion(Vector3 LookAtPoint)
    {
        if (IsSlowMotion || (SlowMotionAnchors.Length == 0))
            yield break;
        IsSlowMotion = true;
        GameObject temp = new GameObject();
        temp.transform.position = transform.position;
        temp.transform.rotation = transform.rotation;
        //select random slow motion anchor:
		
        Transform randomAnchor = Util.RandomFromArray<Transform>(SlowMotionAnchors);
        
        //fade out slowmotion:
        //yield return StartCoroutine(Util.AlighToward(transform, randomAnchor, FadeInSlowMotionTime));
        transform.position = randomAnchor.position;
        transform.rotation = randomAnchor.rotation;

        transform.LookAt(LookAtPoint);
        transform.position = AdjustLineOfSight(transform.position, LookAtPoint);

        Time.timeScale = SlowestTimeScale;
        yield return new WaitForSeconds(FadeInSlowMotionTime);
        Time.timeScale = 1;
        
        yield return StartCoroutine(Util.AlighToward(transform, temp.transform, FadeOutSlowMotionTime));
        SetPosition(false);
        transform.LookAt(this.Character.transform);
        IsSlowMotion = false;
        Destroy(temp);
    }
}
