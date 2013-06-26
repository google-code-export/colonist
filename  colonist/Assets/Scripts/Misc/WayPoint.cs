using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Way point is nothing else, a mark to the static objects in game scene.
/// AI unit can refer to predefined WayPoint in scene, by referring to its name.
/// Note: one gameobject can have only one waypoint, and the gameobject's name must be unique to other gameobject which have waypoint.
/// </summary>
public class WayPoint : MonoBehaviour {
    
	private static IDictionary<string,WayPoint> WayDict = new Dictionary<string,WayPoint>();
	
	/// <summary>
	/// Gets the waypoint by name.
	/// </summary>
	public static WayPoint GetWaypoint(string name)
	{
		return WayDict[name];
	}
	
	void Awake()
	{
		WayDict.Add(this.name, this);
	}
	
	void OnDrawGizmos()
	{
		Gizmos.DrawSphere(this.transform.position, 1);
	}
}
