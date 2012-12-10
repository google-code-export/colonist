using UnityEngine;
using System.Collections;

public class Predator3rdPersonCameraController : MonoBehaviour {

    public Camera workingCamera = null;
    public Transform cameraPos = null;
	// Use this for initialization
	void Awake () {
	    if(workingCamera == null)
		{
			workingCamera = Camera.main;
		}
	}
	
	// Update is called once per frame
	void Update () {
        //Util.AlighToward(workingCamera.transform, cameraPos, true, 0.01f, 0.01f);
	}
}
