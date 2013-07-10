using UnityEngine;
using System.Collections;

/// <summary>
/// Put GUI texture at center.
/// </summary>
[ExecuteInEditMode]
public class CenterizeGUITexture : MonoBehaviour {
	public Vector2 ImageAspect = Vector2.one;
	// Use this for initialization
	void Start () {

	}
	
	void Update()
	{
	    float width = Screen.width * ImageAspect.x;
		float height = Screen.width * ImageAspect.y;
		this.guiTexture.pixelInset = new Rect(-width/2, -height/2, width,height);
	}
}
