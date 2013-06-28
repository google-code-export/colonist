using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// Dock to waypoint.
/// Waypoint is put in scene already.
/// </summary>
public class DockToWaypoint : MonoBehaviour {
	public bool UseAllWaypoint = false;
	public string[] applicableWaypoint = new string[]{};
	
	WayPoint[] waypoints = new WayPoint[]{};
	WayPoint _currentWaypoint = null;
	
	void Start()
	{
		if(UseAllWaypoint)
		{
		   waypoints = WayPoint.GetAllWaypoints();
		}
		else 
		{
		   waypoints = WayPoint.GetWaypoints(applicableWaypoint);
		}
	}
	
#region Event invoking functions
	
	/// <summary>
	/// Call this function to set %_CurrentWaypoint%
	/// </summary>
	void _SelectRandomWaypoint()
	{
		_currentWaypoint = Util.RandomFromArray<WayPoint>(waypoints);
	}
	
	void _SelectFarestWaypoint()
	{
		_currentWaypoint = Util.findFarest(transform.position, waypoints.Select(x=>x.transform).ToArray()).GetComponent<WayPoint>();
	}
	
	void _SelectNearestWaypoint()
	{
		_currentWaypoint = Util.findFarest(transform.position, waypoints.Select(x=>x.transform).ToArray()).GetComponent<WayPoint>();
	}
	
	/// <summary>
	/// set face angle to waypoint with up axis.
	/// if angle = 0, character face to _currentWaypoint;
	/// if angle = 180, character back to _currentWaypoint;
	/// </summary>
	void _SetFaceAngleToWaypointWithUpAxis(float angle)
	{
		if(_currentWaypoint == null)
		{
			Debug.LogError("Has no waypoint ! Make sure SelectWaypoint() method is called!");
		}
		Vector3 waypointPosition = _currentWaypoint.transform.position;
		transform.LookAt(new Vector3(waypointPosition.x, transform.position.y, waypointPosition.z));
		transform.Rotate(Vector3.up, angle);
	}
	
	/// <summary>
	/// Make sure you've called SelectWaypoint methods to set the %_currentWaypoint% variable.
	/// </summary>
	IEnumerator _MoveToWaypoint(float time)
	{
		if(_currentWaypoint == null)
		{
			Debug.LogError("Has no waypoint ! Make sure SelectWaypoint() method is called!");
		}
		Vector3 distance = _currentWaypoint.transform.position - transform.position;
		Vector3 velocity = distance / time;
		float _s = Time.time;
		CharacterController cc = GetComponent<CharacterController>();
		while( (Time.time - _s) <= time)
		{
			cc.SimpleMove(velocity);
			yield return null;
		}
	}
	
#endregion
}
