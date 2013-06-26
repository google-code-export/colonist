using UnityEngine;
using System.Collections;

public class DualBlade : MonoBehaviour, WeaponControl  {
	
	public GameObject FrontBlade = null;
	public GameObject BackBlade = null;

	public WeaponTrail frontTrail;
	public WeaponTrail backTrail;
	
	public float IterateTime = 0.1f;
	public float UpdateTrailLag = 0.1f;
	public bool InitOn = false;
	
	// Use this for initialization
	void Awake () {
		FrontBlade.active = InitOn;
		BackBlade.active = InitOn;
	}
	
	void Start()
	{

	}
	
	// Update is called once per frame
	void Update () {
		if(FrontBlade.active)
		{
		   frontTrail.Itterate(Time.time + IterateTime);
	       frontTrail.UpdateTrail(Time.time, UpdateTrailLag);
		}
		if(BackBlade.active)
		{
		   backTrail.Itterate(Time.time + IterateTime);
		   backTrail.UpdateTrail(Time.time, UpdateTrailLag);
		}
	}
	
	public virtual IEnumerator _WeaponOn(float delay)
	{
		if(delay > 0)
		{
			yield return new WaitForSeconds(delay);
		}
		FrontBlade.active = true;
		BackBlade.active = true;
		frontTrail.StartTrail(0.5f, 0.4f);
		backTrail.StartTrail(0.5f, 0.4f);
	}
	
	public virtual void _WeaponOff()
	{
		FrontBlade.active = false;
		BackBlade.active = false;
		frontTrail.ClearTrail();
		backTrail.ClearTrail();
	}
}
