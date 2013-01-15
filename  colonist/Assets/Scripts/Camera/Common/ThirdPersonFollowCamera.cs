using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ThirdPersonFollowCamera : MonoBehaviour {

    public Transform PositionPivot = null;
    /// <summary>
    /// The default transform to look at, must not be null
    /// </summary>
    public Transform LookAt = null;

    public float DynamicLockDistance = 1;
    public float DynamicLockHeight = 1.5f;

    /// <summary>
    /// The target transform to look at. If this is null, than will pick DefaultViewPoint to look at.
    /// </summary>
    //[HideInInspector]
    //public Transform LookAtTarget = null;
    public float smoothLag = 0.3f;

    public LayerMask lineOfSightMask = 0;

    public Transform Character = null;

    private Vector3 velocity = new Vector3();
    private CharacterController controller = null;

    void Awake()
    {
        controller = Character.GetComponent<CharacterController>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void LateUpdate()
    {
        //Position damping
        PositionDamping();
        //Rotation damping
        transform.LookAt(this.LookAt);
    }

    private void PositionDamping()
    {
        Vector3 newPosition;
        newPosition = Vector3.SmoothDamp(transform.position, PositionPivot.position, ref velocity, smoothLag);
        //If the sight has been blocked by colliders, zoom in the camera to ensure the character's always visible
        newPosition = AdjustLineOfSight(newPosition, Character.position + controller.center);
        transform.position = newPosition;
    }

    /// <summary>
    /// If current camera sign being obstacled by object in layer, return the closet unhidden point
    /// </summary>
    /// <param name="newPosition"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    Vector3 AdjustLineOfSight(Vector3 newPosition, Vector3 target)
    {
        RaycastHit hit;
        if (Physics.Linecast(target, newPosition, out hit, lineOfSightMask.value))
        {
            velocity = Vector3.zero;
            return hit.point;
        }
        return newPosition;
    }

    void OnEnable()
    {
        transform.position = PositionPivot.position;
        //Rotation damping
        transform.LookAt(this.LookAt);
    }

    
}
