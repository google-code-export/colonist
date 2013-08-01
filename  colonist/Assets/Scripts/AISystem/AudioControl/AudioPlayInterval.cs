using UnityEngine;
using System.Collections;

/// <summary>
/// Play the audio source in a fixed time interval.
/// How it work:
/// 1. attach a audioclip.
/// 2. set the interval.
/// The audio is played and after %length% + %interval% time, it's played again.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioPlayInterval : MonoBehaviour {
	
	public int interval = 3;
	
	float lastPlayTime = 0;
	
	void OnDisable()
	{
		StopAllCoroutines();
		audio.Stop();
	}
	
	IEnumerator PlayInFixInterval()
	{
		while(true)
		{
		   audio.Play();
		   yield return new WaitForSeconds(interval + audio.clip.length);
		}
	}
}
