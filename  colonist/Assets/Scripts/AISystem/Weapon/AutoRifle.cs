using UnityEngine;
using System.Collections;

public class AutoRifle : WeaponControlBase {

	public override void CreateHitEffect(GameObject hitObject)
	{
		if(this.HasHitEffect == false)
		{
			return;
		}
		//can't work without collider
		if(hitObject.collider == null)
		{
			return;
		}
		RaycastHit hitInfo;
		if(Physics.Linecast(transform.position, hitObject.collider.bounds.center, out hitInfo))
		{
			Vector3 effectPos = hitInfo.point;
			foreach(GameObject effectObject in effectHitObjects)
			{
				Object.Instantiate(effectObject, effectPos, Random.rotation);
			}
		}
	}
	
	public override IEnumerator _WeaponOn(float delay)
	{
	    yield break;	
	}
	
	public override void _WeaponOff()
	{
		return;
	}
}
