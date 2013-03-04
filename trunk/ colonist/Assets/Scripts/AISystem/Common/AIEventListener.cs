using UnityEngine;
using System.Collections;

public class AIEventListener : MonoBehaviour
{
	public float movebackspeed = 1;
	CharacterController controller = null;
	bool backward = false;
	float stopBackward = 0;

	void Awake ()
	{
		controller = GetComponent<CharacterController> ();
	}
	
	void Update ()
	{
		if (backward && Time.time >= stopBackward) {
			backward = false;
		}
		
		if (backward) {
			controller.SimpleMove (transform.TransformDirection (Vector3.back) * movebackspeed);
		}
	}
	
	public void _Moveback (float duration)
	{
		backward = true;
		stopBackward = Time.time + duration;
	}
}
