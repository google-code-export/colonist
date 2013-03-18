using UnityEngine;
using System.Collections;

public class Level00 : MonoBehaviour {

    public Camera workingCamera = null;
    public MonoBehaviour playerCameraControl = null;
    public Transform cameraDock1 = null;
    public Transform cameraDock2 = null;
    void Awake()
    {
        if (workingCamera == null)
        {
            workingCamera = Camera.main;
        }
    }

	// Use this for initialization
	IEnumerator Start () {
        playerCameraControl.enabled = false;
        //Util.AlighToward(workingCamera.transform, cameraDock1.transform, false, 0, 0);
        yield return StartCoroutine("SetCamera", 3f);
        playerCameraControl.enabled = true;
        yield return null;
	}
	
	// Update is called once per frame
	void Update () {

	}

    IEnumerator SetCamera(float duration)
    {
        float _time = Time.time;
        
        while ((Time.time - _time) <= duration)
        {
            workingCamera.transform.position = cameraDock1.position;
            workingCamera.transform.rotation = cameraDock1.rotation;
            yield return null;
        }

        _time = Time.time;
        while ((Time.time - _time) <= duration)
        {
            //Util.AlighToward(this.workingCamera.transform, cameraDock2, true, 0.03f, 30f);
            yield return null;
        }

        yield return null;
    }
}
