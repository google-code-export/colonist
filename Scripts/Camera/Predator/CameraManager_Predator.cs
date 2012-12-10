using UnityEngine;
using System.Collections;

public class CameraManager_Predator : MonoBehaviour {

    public static CameraManager_Predator Instance = null;
    public SlowMotionCamera slowMotionCamera = null;
    //public SpringFollowCamera_Predator PlayerControlCamera = null;

    void Awake()
    {
        Instance = this;
        slowMotionCamera.enabled = false;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void SlowMotionStart()
    {
      //  slowMotionCamera.ViewTarget = SlowMotionViewTarget;
        slowMotionCamera.enabled = true;
    }
}
