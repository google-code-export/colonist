using UnityEngine;
using System.Collections;

public class BloodHitPlane : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	 
	}

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Blood OnCollisionEnter!");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Blood OnTriggerEnter!");
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("Blood OnParticleCollision !");
    }
    
}
