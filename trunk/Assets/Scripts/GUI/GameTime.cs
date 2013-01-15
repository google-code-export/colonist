using UnityEngine;
using System.Collections;

public class GameTime : MonoBehaviour {
	public float timeScale = 0.3f;
	// Use this for initialization
	void Start () {
	    Time.timeScale  = timeScale;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
