using UnityEngine;
using System.Collections;

public class PlayRandomAudio : MonoBehaviour {
	public AudioSource[] RandomPlayAudio = new AudioSource[]{ };
	public AudioClip[] RandomPlayClip = new AudioClip[]{ };
	// Use this for initialization
	void Start () {
		if(RandomPlayAudio.Length > 0)
		{
	      Util.RandomFromArray<AudioSource>(RandomPlayAudio).Play();
		}
		if(RandomPlayClip.Length > 0)
		{
		  AudioClip c = Util.RandomFromArray<AudioClip>(RandomPlayClip);
		  AudioSource.PlayClipAtPoint(c,transform.position);
		}
	}
}
