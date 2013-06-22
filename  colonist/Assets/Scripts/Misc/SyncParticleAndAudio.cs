using UnityEngine;
using System.Collections;

public class SyncParticleAndAudio : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	     if((this.audio.isPlaying == false)
			&&
			(this.particleSystem != null && this.particleSystem.isPlaying)
			|| (this.particleEmitter != null && this.particleEmitter.emit)
			)
		{
			this.audio.Play();
		}
	}
}
