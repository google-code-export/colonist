using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

/// <summary>
/// Jetpack soldier landing AI.
/// Specifically for Jetpack soldier to land from air.
/// How it works:
/// 1. Assign the name of the moveData: MoveData_FlyingToLandingAirSpot, MoveData_LandingFromAirSpot.
/// 2. Start this AI.
/// Jet pack soldier landing process:
/// 1. Find a random point of waypoint, as LandingSpot.
/// 2. Get the peer point in the air, this point as LandingAirSpot.
/// 3. Move to LandingAirSpot, with MoveData = MoveData_FlyingToLandingAirSpot.
/// 4. Move to LandingSpot, with MoveData = MoveData_LandingFromAirSpot.
/// </summary>
public class JetpackSoldierLandingAI : AI {
	
	public string MoveDataName_FlyingToLandingAirSpot = "";
	public string MoveDataName_LandingFromAirSpot = "";
	public string Animation_Landing = "";
	public float scanGroundRange = 20;
	public string nextAI = "";
	MoveData MoveData_FlyingToLandingAirSpot = null;
	MoveData MoveData_LandingFromAirSpot = null;
	
	/// <summary>
	/// The name of the applicable waypoint.
	/// </summary>
	public string[] ApplicableWaypointName = new string[] {};
	WayPoint[] applicableWaypoints = new WayPoint[]{};
	
	void Start()
	{
		MoveData_FlyingToLandingAirSpot = this.Unit.MoveDataDict[MoveDataName_FlyingToLandingAirSpot];
		MoveData_LandingFromAirSpot = this.Unit.MoveDataDict[MoveDataName_LandingFromAirSpot];
		applicableWaypoints = WayPoint.GetWaypoints(ApplicableWaypointName);
	}
	
    public override void StartAI()
    {
		Debug.Log("JetpackSoldierLandingAI is called!");
		Unit.CurrentAI = this;
		StartCoroutine("Start_LandingBehavior");
		this.enabled = true;
    }
	
	public override void StopAI()
	{
		StopAllCoroutines();
		this.enabled = false;
	}
	Vector3 landingSpot, landingAirSpot;
	
	public IEnumerator Start_LandingBehavior()
	{
		Debug.Log("Start_LandingBehavior is called!");
		//get the landing spot, which must not be too close to jetpack soldier:
		WayPoint[] farEnoughWaypoint = this.applicableWaypoints.Where(x => ((new Vector2(x.transform.position.x, x.transform.position.z) - new Vector2(transform.position.x, transform.position.z)).magnitude >=2)).ToArray();
		if(farEnoughWaypoint == null || farEnoughWaypoint.Length == 0)
		{
			farEnoughWaypoint = this.applicableWaypoints;//in case not found
		}
		landingSpot = Util.RandomFromArray<WayPoint>(farEnoughWaypoint).transform.position;
		landingAirSpot = new Vector3(landingSpot.x, transform.position.y, landingSpot.z);
		
//		Debug.DrawLine(transform.position, landingSpot);
//		Debug.DrawLine(transform.position, landingAirSpot,Color.red);
//		Debug.Break();
		
		//fly to landingAirSpot:
		Vector3 distance = landingAirSpot - transform.position;
		float totaltime = distance.magnitude / MoveData_FlyingToLandingAirSpot.MoveSpeed;
		Vector3 velocity = distance.normalized * MoveData_FlyingToLandingAirSpot.MoveSpeed;
		float starttime = Time.time;
		transform.LookAt(landingAirSpot);
		while((Time.time - starttime) <= totaltime)
		{
			transform.position += velocity * Time.deltaTime;
			animation.CrossFade(MoveData_FlyingToLandingAirSpot.AnimationName);
			yield return null;
		}
		
		//fly upward a little bit:
		starttime = Time.time;
		while((Time.time - starttime) <= 1f)
		{
			transform.position += Vector3.up * 1f * Time.deltaTime;
			animation.CrossFade(MoveData_LandingFromAirSpot.AnimationName);
			yield return null;
		}
		
		//now downward landing:
		distance = landingSpot - transform.position;
		totaltime = distance.magnitude / MoveData_LandingFromAirSpot.MoveSpeed;
		velocity = distance.normalized * MoveData_LandingFromAirSpot.MoveSpeed;
		starttime = Time.time;
		while((Time.time - starttime) <= totaltime)
		{
			transform.position += velocity * Time.deltaTime;
			animation.CrossFade(MoveData_LandingFromAirSpot.AnimationName);
			yield return null;
		}
		
		//play landing animation, and put to ground:
		animation.CrossFade(Animation_Landing);
		Util.PutToGround (transform, this.Unit.GroundLayer, 0.1f);
	}
	
	public void SwitchToNextAI()
	{
		Unit.AIDict[nextAI].StartAI();
		StopAllCoroutines();
		this.enabled = false;
	}
}
