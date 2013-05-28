using UnityEngine;
using System.Collections;

/// <summary>
/// Dock movement type.
/// 1. Head to end point always. (Or head to end point at X-Z surface)
/// 2. Smoothly linearly aligh to end point rotation.(usually use for camera docking)
/// 3. Keep rotation unchanged as initial value.
/// </summary>
public enum DockRotationType
{
	HeadToEnd = 0,
	HeadToEnd_XZ = 1,
	LerpToEndPoint = 2,
	Unchange = 3,
}

/// <summary>
/// This class wraps object moving/rotating data.
/// </summary>
[System.Serializable]
public class ObjectDockData
{
	/// <summary>
	/// The name of the camera dock.
	/// </summary>
	public string Name = "";
	/// <summary>
	/// Move/rotate to the DockTransform.
	/// </summary>
	public Transform DockTransform;
	
	/// <summary>
	/// if DockImmediately = true, FleetTime is ignore
	/// </summary>
	public bool DockImmediately = false;
	/// <summary>
	/// The pending time - after docking to this dock, wait for %PendingTime% seconds.
	/// </summary>
	public float PendingTime = 0;
	/// <summary>
	/// How long should the object arrive this dock ?
	/// If the FleetTime == zero, camera will be forced to set in the dock immediately.
	/// </summary>
	public float FleetTime;
	public DockRotationType rotationType = DockRotationType.HeadToEnd;
	
	public AnimationCurve CurveForXAxis = AnimationCurve.Linear(0,0,1,1);
	public AnimationCurve CurveForYAxis = AnimationCurve.Linear(0,0,1,1);
	public AnimationCurve CurveForZAxis = AnimationCurve.Linear(0,0,1,1);
	
	/// <summary>
	/// When start docking at this camera dock, the message should be sent.
	/// </summary>
	public GameEvent[] event_at_start = new GameEvent[]{};
	
	/// <summary>
	/// When end docking at this camera dock, the message should be sent.
	/// </summary>
	public GameEvent[] event_at_end = new GameEvent[]{};
	

}

/// <summary>
/// ObjectDock behaves to move/rotate object between docks.
/// </summary>
public class ObjectDock : MonoBehaviour
{
	public string Name = "";
	public Transform objectToDock = null;
	public bool PlayOnAwake = false;
	public ObjectDockData[] objectDockData = new ObjectDockData[]{};
	
	void Awake()
	{
		
	}
	
	// Use this for initialization
	void Start ()
	{
		if(PlayOnAwake)
	       StartCoroutine("StartDocking", objectToDock);
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	public void Dock()
	{
		StartCoroutine("StartDocking", objectToDock);
	}
	
	/// <summary>
	/// Call this method in runtime, to dock the specified transform(not the one in inspector).
	/// </summary>
	public void Dock(Transform t)
	{
		StartCoroutine("StartDocking", t);
	}
	
	IEnumerator StartDocking(Transform _object)
	{
		foreach(ObjectDockData dock in this.objectDockData)
		{
			yield return StartCoroutine(Dock(dock, _object));
//			Debug.Log("Docking done on :" + dock.Name + " at time:" + Time.time);
		}
	}
	
	/// <summary>
	/// Dock the dockingObject to specified dock.
	/// </summary>
	IEnumerator Dock (ObjectDockData dock, Transform dockingObject)
	{
		//send start gameevent
		SendGameEvents(dock.event_at_start, dockingObject);
		if (dock.DockImmediately) 
		{
			//handle rotation
			dockingObject.rotation = dock.DockTransform.rotation;
			//handle position
			dockingObject.position = dock.DockTransform.position;
		} 
		else 
		{
			float starttime = Time.time;
			Vector3 startPos = dockingObject.position;
			Quaternion startRotation = dockingObject.rotation;
			Vector3 distance = dock.DockTransform.position - startPos;
			float AngleDistance = Quaternion.Angle(dockingObject.rotation, dock.DockTransform.rotation);
			float rotateAnglarSpeed = AngleDistance / dock.FleetTime;
			while ((Time.time - starttime) <= dock.FleetTime) {
				float percentage = (Time.time - starttime) / dock.FleetTime;
				float xnormalized = dock.CurveForXAxis.Evaluate(percentage);
				float ynormalized = dock.CurveForYAxis.Evaluate(percentage);
				float znormalized = dock.CurveForZAxis.Evaluate(percentage);
				Vector3 endPosition = new Vector3(distance.x * xnormalized, distance.y * ynormalized, distance.z * znormalized)  + startPos;
				
				//position this frame
				dockingObject.position = endPosition;
				//rotation of this frame
				switch (dock.rotationType) {
				case DockRotationType.HeadToEnd:
					dockingObject.LookAt (dock.DockTransform);
					break;
				case DockRotationType.HeadToEnd_XZ:
					dockingObject.LookAt (new Vector3 (dock.DockTransform.position.x, dockingObject.transform.position.y, dock.DockTransform.position.z));
					break;
				case DockRotationType.LerpToEndPoint:
//					Vector3 rotationEulerIncrementValue = dock.RotationCurve.GetValueAtPercentage (percentage);
//					Vector3 rotationEulerAngleOfTheFrame = startRotation.eulerAngles + rotationEulerIncrementValue;
//					dockingObject.rotation = Quaternion.Euler (rotationEulerAngleOfTheFrame);
					dockingObject.rotation = Quaternion.Lerp(startRotation, dock.DockTransform.rotation, percentage);//Quaternion.RotateTowards(dockingObject.rotation, dock.DockTransform.rotation, rotateAnglarSpeed * Time.deltaTime);
					break;
				case DockRotationType.Unchange:
					break;
				}
				yield return null;
			}
		}
		
		//send end gameevent
	    SendGameEvents(dock.event_at_end, dockingObject);
		if(dock.PendingTime > 0)
		{
			yield return new WaitForSeconds(dock.PendingTime);
		}
	}
	
	/// <summary>
	/// Sends the game events.
	/// What's a little different is, the ObjectDock can handle the GameEvent for spawned at the runtime.
	/// For example, For type = NPCPlayAnimation, the receiver can not be set in inspector (because the dockingObject is created in runtime!)
	/// So, the receiver need to be dynamically set to the GameEvent.
	/// </summary>
	void SendGameEvents(GameEvent[] events, Transform dockingObject)
	{
		foreach (GameEvent _event in events) {
			GameEvent _e = _event.Clone();
			if(_e.receiver == null)
			{
				//override the receiver field at GameEvent to every spawned 
				switch(_e.type)
				{
					case GameEventType.NPCPlayAnimation:
				    case GameEventType.NPCStopPlayingAnimation:
				    case GameEventType.NPCStartAI:
				    case GameEventType.NPCStartDefaultAI:
				    case GameEventType.InvokeMethod:
				    case GameEventType.DeactivateGameObject:
				    case GameEventType.DestroyGameObject:
				      _e.receiver = dockingObject.gameObject;
					  break;
				    default:
					  break;
				}
			}
			LevelManager.OnGameEvent (_e);
		}
	}
	
	void OnDrawGizmos ()
	{
		DrawGizmos();
	}
	
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		DrawGizmos();
	}
	
	void DrawGizmos()
	{
		//draw line between dock transform
		for (int i=0; i<this.objectDockData.Length; i++) {
			//Draw a wire cube at the trail node
			if (i == this.objectDockData.Length - 1) {
				Gizmos.DrawWireCube (objectDockData [i].DockTransform.position, Vector3.one * 0.25f);
				break;
			}
			//draw a wire sphere at the head node
			else if (i == 0) {
				Gizmos.DrawWireSphere (objectDockData [i].DockTransform.position, 0.25f);
			}
			//draw a filled sphere at middle node
			else 
			{
				Gizmos.DrawSphere (objectDockData [i].DockTransform.position, 0.25f);
			}
			Gizmos.DrawLine (objectDockData [i].DockTransform.position, objectDockData [i + 1].DockTransform.position);
		}
	}
}
