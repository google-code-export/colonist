using UnityEngine;
using System.Collections;

public class DualBlade : MonoBehaviour, WeaponControl  {
	
	public GameObject FrontBlade = null;
	public GameObject BackBlade = null;
	public GameObject[] HitEffectObjects = new GameObject[]{};
	/// <summary>
	/// The name of the weapon on sound effect. Which will be played in _WeaponOn routine.
	/// </summary>
	public string WeaponOnSoundEffectName = "";
	
    /// <summary>
	/// The name of the weapon off sound effect. Which will be played in _WeaponOff routine.
	/// </summary>
	public string WeaponOffSoundEffectName = "";
	
	public WeaponTrail frontTrail;
	public WeaponTrail backTrail;
	
	public float IterateTime = 0.1f;
	public float UpdateTrailLag = 0.1f;
	public bool InitOn = false;
	
	Unit unit = null;
	CharacterController characterController = null;
	void Awake()
	{
		unit = GetComponent<Unit>();
		characterController = GetComponent<CharacterController>();
	}
	
	void Start()
	{
		FrontBlade.SetActive(InitOn);
		BackBlade.SetActive(InitOn);
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
		
		GlobalAudioSystem.PlayAudioData( unit.AudioDataDict[WeaponOnSoundEffectName] );
	}
	
	public virtual void _WeaponOff()
	{
		FrontBlade.active = false;
		BackBlade.active = false;
		frontTrail.ClearTrail();
		backTrail.ClearTrail();
		
		GlobalAudioSystem.PlayAudioData( unit.AudioDataDict[WeaponOffSoundEffectName] );
	}
	
	public virtual void CreateHitEffect(GameObject hitObject)
	{
		//get the closest point and create the hit effect object:
		Vector3 center = characterController.center + transform.position;
		Vector3 hitPoint = hitObject.collider.ClosestPointOnBounds(center);
		foreach(GameObject hitEffect in HitEffectObjects)
		{
		   Object hitEffectObject = Object.Instantiate(hitEffect, hitPoint, Quaternion.identity);
		   Destroy(hitEffectObject, 3);
		}
	}
}
