using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour {
	public int level = 0;
	public float delay = 3;
	// Use this for initialization
	void Start () {
	    Invoke("_LoadLevel", delay);
	}
	
	void _LoadLevel()
	{
		Application.LoadLevel(level);
	}
}
