using UnityEngine;
using System.Collections;


/// <summary>
/// When ever unit collide with the gameobject will be killed.
/// </summary>
public class KillCollidedUnit : MonoBehaviour {
	
	public bool DestroyDirectly = false;
	public float DestroyLag = 5;
	
    void OnTriggerEnter(Collider other) 
	{
		KillUnit(other.gameObject);
    }
			
	void OnCollisionEnter(Collision collision)
	{
		KillUnit(collision.gameObject);
	}
	
	void KillUnit(GameObject other)
	{
		if(DestroyDirectly == false && other.GetComponent<UnitHealth>() != null)
		{
		   other.gameObject.SendMessage("ApplyDamage", new DamageParameter(this.gameObject, DamageForm.Common, float.MaxValue));
		}
		else 
		{
           Destroy(other.gameObject, DestroyLag);
		}
	}
	
}
