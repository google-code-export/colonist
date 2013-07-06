using UnityEngine;
using System.Collections;

public class LaserBlade : MonoBehaviour, WeaponControl {
	
	public GameObject ControllableWeapon = null;

	public bool InitOn = false;
	
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
	}
}
