using UnityEngine;
using System.Collections;

/// <summary>
/// A script which can fulfill the common requirement of weapon control:
/// 1. create hit effect
/// 2. set on/off the weapon.
/// </summary>
public class WeaponControlCommon : WeaponControlBase {
	public GameObject[] weaponObjects = new GameObject[]{};
	 	/// <summary>
	/// The hitEffect array,
	/// </summary>
	public GameObject[] effectHitObjects = new GameObject[]{};

	/// <summary>
	/// Implement this method in sub class. To create hit effect when sending applyDamage().
	/// </summary>
	public override void CreateHitEffect(GameObject hitObject)
	{
		//get the closest point and create the hit effect object:
		Vector3 center = this.GetComponent<CharacterController>() != null ? 
			             this.GetComponent<CharacterController>().center :
				         (this.collider != null )? this.collider.bounds.center : this.transform.position;
		
		Vector3 hitPoint = hitObject.collider.ClosestPointOnBounds(center);
		foreach(GameObject hitEffect in effectHitObjects)
		{
		   Object hitEffectObject = Object.Instantiate(hitEffect, hitPoint, Quaternion.identity);
		   Destroy(hitEffectObject, 3);
		}
	}
	
	public override IEnumerator _WeaponOn(float delay)
	{
		if(delay > 0)
		{
			yield return new WaitForSeconds(delay);
		}
		foreach(GameObject obj in weaponObjects)
		{
			obj.SetActive(true);
		}
	}
	
	public override void _WeaponOff()
	{
		foreach(GameObject obj in weaponObjects)
		{
			obj.SetActive(false);
		}
	}
}
