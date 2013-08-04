using UnityEngine;
using System.Collections;

public abstract class WeaponControlBase : MonoBehaviour, WeaponControl {
	
	/// <summary>
	/// Implement this method in sub class. To create hit effect when sending applyDamage().
	/// </summary>
	public abstract void CreateHitEffect(GameObject hitObject);
	
	public abstract IEnumerator _WeaponOn(float delay);
	
	public abstract void _WeaponOff();
	
}
