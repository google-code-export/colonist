using UnityEngine;
using System.Collections;

public abstract class WeaponControlBase : MonoBehaviour, WeaponControl {
	/// <summary>
	/// The hitEffect array,
	/// </summary>
	public GameObject[] effectHitObjects = new GameObject[]{};
	
	/// <summary>
	/// if HasHitEffect = true, means when the unit hits something, there will be hitEffect created by CreateHitEffect() method.
	/// </summary>
	public bool HasHitEffect = false;
	
	/// <summary>
	/// Implement this method in sub class. To create hit effect when sending applyDamage().
	/// </summary>
	public abstract void CreateHitEffect(GameObject hitObject);
	
	public abstract IEnumerator _WeaponOn(float delay);
	
	public abstract void _WeaponOff();
	
}
