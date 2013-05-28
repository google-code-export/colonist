using UnityEngine;
using System.Collections;

public class GlobalAudioSystem : MonoBehaviour {
	public static GlobalAudioSystem Instance = null;
	
	void Awake()
	{
		Instance = this;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static void PlayAudioData(AudioData audioData)
	{
		switch(audioData.mode)
		{
		case PlayAudioMode.PlayAudiosource:
			//play the must-to-played
			foreach(AudioSource source in audioData.PlayerAudioSources)
			{
				source.Play();
			}
			//play the random-to-play
			if(audioData.randomAudioSources.Length > 0)
			{
				Util.RandomFromArray<AudioSource>(audioData.randomAudioSources).Play();
			}
			break;
		case PlayAudioMode.PlayAudioClipAtPosition:
			//play the must-to-played
			foreach(AudioClip clip in audioData.PlayedAudioClips)
			{
				AudioSource.PlayClipAtPoint(clip, audioData.Play3DClipAnchor.position);
			}
			//play the random-to-play
			if(audioData.randomAudioClips.Length > 0)
			{
				AudioSource.PlayClipAtPoint(Util.RandomFromArray<AudioClip>(audioData.randomAudioClips), audioData.Play3DClipAnchor.position);
			}
			break;
		}
	}
	
	
}
