using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {
	
	/// <summary>
	/// The gameobject will be destroyed in time.
	/// </summary>
	public float destroyInTime = 5;
	
	// Use this for initialization
	void Start () {
	     Destroy(this.gameObject, destroyInTime);
	}
	
}
