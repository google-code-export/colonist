using UnityEngine;
using System.Collections;

/// <summary>
/// OverlappedCameraControl is used for put another camera over the current camera, the overlapped camera will occupy
/// part of the screen, this is useful for synchronously telling two tale.
/// </summary>
public class OverlappedCameraControl : RuntimeCameraControl
{
	
	/// <summary>
	/// The RectCurve controls how to overallped camera fade in.
	/// </summary>
	public RectCurve FadeInRectCurve = new RectCurve ();
	public RectCurve FadeOutRectCurve = new RectCurve ();
	
//	public RectCurve rectCurveForCameraViewPoint = new RectCurve();
	public float CameraWorkingDepth = 1;
	float startWorkingTime = 0;
	float cameraInitalDepth = -1;
	
	void Awake ()
	{
		cameraInitalDepth = camera.depth;
	}
	
	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	IEnumerator FadeInOverlappedCamera ()
	{
		if(this.camera.enabled == false)
		{
			this.camera.enabled = true;
		}
		camera.depth = CameraWorkingDepth;
		this.enabled = true;
		startWorkingTime = Time.time;
		while (true) {
			float durationTime = Mathf.Clamp (Time.time - startWorkingTime, 0, FadeInRectCurve.MaxTime);
			Rect cameraRect = FadeInRectCurve.Evaluate (durationTime);
			try{
			this.camera.rect = FadeInRectCurve.Evaluate (durationTime);
			}
			catch(System.Exception exc)
			{
				Debug.LogError("Error camera rect:" + this.camera.rect + "\r\n" + exc.Message);
			}
			if (durationTime > FadeInRectCurve.MaxTime) {
				this.enabled = false;
				break;
			}
			yield return null;
		}
		this.enabled = false;
	}
	
	IEnumerator FadeOutOverlappedCamera ()
	{
		this.enabled = true;
		startWorkingTime = Time.time;
		while (true) {
			float durationTime = Mathf.Clamp (Time.time - startWorkingTime, 0, FadeOutRectCurve.MaxTime);
			this.camera.rect = FadeOutRectCurve.Evaluate (durationTime);
			if (durationTime > FadeOutRectCurve.MaxTime) {
				this.enabled = false;
				break;
			}
			yield return null;
		}
		this.enabled = false;
		camera.depth = cameraInitalDepth;
		camera.enabled = false;
	}
	
}
