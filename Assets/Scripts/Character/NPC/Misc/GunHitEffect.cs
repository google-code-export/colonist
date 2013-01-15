using UnityEngine;
using System.Collections;

public class GunHitEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, this.gameObject.GetComponent<ParticleSystem>().duration);
        this.gameObject.GetComponent<ParticleSystem>().enableEmission = true;
        this.gameObject.GetComponent<ParticleSystem>().Play(); 
	}
	
	// Update is called once per frame
	void Update () {
        
	}
}
