using UnityEngine;
using System.Collections;

public class PlayRandomAudio : MonoBehaviour {
	public AudioSource[] RandomPlayAudio = new AudioSource[]{ };
	// Use this for initialization
	void Start () {
	    Util.RandomFromArray<AudioSource>(RandomPlayAudio).Play();
	}
}
