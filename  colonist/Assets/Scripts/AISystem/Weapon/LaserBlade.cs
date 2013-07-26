using UnityEngine;
using System.Collections;

public class LaserBlade : MonoBehaviour, WeaponControl {
	
	public GameObject ControllableWeapon = null;
	public GameObject LaserHitEffect = null;
	public bool InitOn = false;
	Unit unit;
	CharacterController characterController;
	void Awake()
	{
		unit = GetComponent<Unit>();
		characterController = GetComponent<CharacterController>();
	}
	
	public virtual IEnumerator _WeaponOn(float delay)
	{
		if(delay > 0)
		{
			yield return new WaitForSeconds(delay);
		}
		ControllableWeapon.active = true;
	}
	
	public virtual void _WeaponOff()
	{
		ControllableWeapon.active = false;
	}
	
	public virtual void CreateHitEffect(GameObject hitObject)
	{
		//get the closest point and create the hit effect object:
		Vector3 center = characterController.center + transform.position;
		Vector3 hitPoint = hitObject.collider.ClosestPointOnBounds(center);
		Object hitEffectObject = Object.Instantiate(LaserHitEffect, hitPoint, Quaternion.identity);
		Destroy(hitEffectObject, 3);
	}
}
