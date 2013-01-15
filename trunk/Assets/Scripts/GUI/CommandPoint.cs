using UnityEngine;
using System.Collections;

public class CommandPoint : MonoBehaviour {

    public float LifeTime = 2f; 

	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, LifeTime);
	}
	
	// Update is called once per frame
	void Update () {
 
	}
}
