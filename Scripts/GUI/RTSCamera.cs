using UnityEngine;
using System.Collections;

public class RTSCamera : MonoBehaviour {

	public bool printDebugLog = true;

	void Update ()
	{
        //Allow mouse look at Windows
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            MoveCamera_Keyboard();
            MoveCamera_Mouse();
        }
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}
	
	public static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}
	
	private void MoveCamera_Keyboard()
	{
		float straight = Input.GetAxis("Straight");
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("VerticalUpDown");
		if(straight == 0 && horizontal == 0 && vertical == 0)
			return;

		if(straight != 0)
		{
		   Vector3 forward = this.transform.TransformDirection(straight > 0 ? Vector3.forward : Vector3.back);
           forward.y = 0;
		   this.transform.position += forward.normalized * Mathf.Abs(straight);
		}
		
		if(horizontal != 0)
		{
			Vector3 right = this.transform.TransformDirection(horizontal > 0 ? Vector3.right : Vector3.left);
			this.transform.position += right.normalized * Mathf.Abs(horizontal);
		}
		
		if(vertical != 0)
		{
			Vector3 up = this.transform.TransformDirection(vertical > 0 ? Vector3.up : Vector3.down);
			this.transform.position += up.normalized * Mathf.Abs(vertical);
		}
	}

    private void MoveCamera_Mouse()
    {
        //Mouse right or middle can trigger camera rotation:
        if (Input.GetAxis("RightMouse") == 0 && Input.GetAxis("MiddleMouse") == 0 )
        {
            return;
        }
        float X = Input.GetAxis("Mouse X");
        float Y = Input.GetAxis("Mouse Y");
        float Scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Y != 0)
        {
            Vector3 verticalY = this.transform.TransformDirection(Y > 0 ? Vector3.forward : Vector3.back);
            verticalY.y = 0;
            this.transform.position += verticalY.normalized * Mathf.Abs(Y);
        }
        if (X != 0)
        {
            Vector3 horizontalX = this.transform.TransformDirection(X > 0 ? Vector3.right : Vector3.left);
            this.transform.position += horizontalX.normalized * Mathf.Abs(X);
        }
        if (Scroll != 0)
        {
            Vector3 forward = this.transform.TransformDirection(Scroll > 0 ? Vector3.forward : Vector3.back);
            this.transform.position += forward.normalized * Mathf.Abs(Scroll); 
        }
    }
}