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
		//do nothing if audiodata is disable.
		if(audioData.enabled == false)
			return;
		
		switch(audioData.mode)
		{
		case AudioObjectMode.AudioSouce:
			if(audioData.randomAudioSources.Length > 0)
			{
				AudioSource audio = Util.RandomFromArray<AudioSource>(audioData.randomAudioSources);
				///for playing and looping audiosource, do nothing
				if(audio.loop == true && audio.isPlaying == false)
				{
					audio.Play();
				}
				else if(audio.isPlaying == false)
				{
					audio.Play();
				}
			}
			break;
		case AudioObjectMode.Clip:
			if(audioData.randomAudioClips.Length > 0)
			{
				AudioSource.PlayClipAtPoint(Util.RandomFromArray<AudioClip>(audioData.randomAudioClips), audioData.Play3DClipAnchor.position, audioData.PlayClipVolume);
			}
			break;
		}
	}
	
	
}
