using UnityEngine;
using System.Collections;

public class FloatTextureOffset : MonoBehaviour {
	
	/// <summary>
	/// Defines X,Y offset of an image per second.
	/// </summary>
	public Vector2 FloatOffsetPerSeconds = Vector2.zero;
	
	// Update is called once per frame
	void Update () {
		this.renderer.material.SetTextureOffset("_MainTex", 
			new Vector2(this.renderer.material.mainTextureOffset.x + FloatOffsetPerSeconds.x * Time.deltaTime,
			            this.renderer.material.mainTextureOffset.y + FloatOffsetPerSeconds.y * Time.deltaTime));
	}
}
