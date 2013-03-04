using UnityEngine;
using System.Collections;


/// <summary>
/// Interface of receiving damage event.
/// </summary>
public interface I_ReceiveDamage {
	
	/// <summary>
	/// This is the entry routine to apply damage.
	/// In general, this routine should be called inside ReceiveEvent() function
	/// </summary>
	IEnumerator ApplyDamage(DamageParameter damageParam);
	
	/// <summary>
	/// This routine is called when character react to damage.
	/// In general, this routine should be called inside ApplyDamage.
	/// </summary>
	IEnumerator DoDamage(DamageParameter damageParam);
	
	/// <summary>
	/// This routine is called when character dies.
	/// In general, this routine should be called inside ApplyDamage.
	/// </summary>
	IEnumerator Die(DamageParameter DamageParameter);
}
