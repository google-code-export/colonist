using UnityEngine;
using System.Collections;

public class BodyCollision : MonoBehaviour {

    /// <summary>
    /// Only receive hit from a certain layer
    /// </summary>
    public string LayerToReceiveHit = "impactor";



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.layer != LayerMask.NameToLayer(LayerToReceiveHit))
        {
            return;
        }
        SendMessageUpwards("OnImpactorHit", hit);
    }
}
