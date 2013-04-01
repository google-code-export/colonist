using UnityEngine;
using System.Collections;

public class Predator3rdPersonAudioController : MonoBehaviour {
	
	Predator3rdPersonalUnit predatorUnit;
	
	void Awake()
	{
		predatorUnit = GetComponent<Predator3rdPersonalUnit>();
	}
	
	public void PlaySound(string name)
	{
		AudioData audioData = predatorUnit.AudioDataDict[name];
		AudioSource.PlayClipAtPoint( Util.RandomFromArray<AudioClip>(audioData.audioClip), transform.position);
	}
}
