using UnityEngine;
using System.Collections;

public abstract class ScenEventListener : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public abstract void ScenarioEvent(GameEvent gameEvent);
}
