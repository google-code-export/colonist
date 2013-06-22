using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Audio controller. Control audio playing.
/// </summary>
public class AudioController : MonoBehaviour
{
	
	Unit unit;
	IList<AudioData> AnimationBindedAudioData = new List<AudioData> ();
	
	void Awake ()
	{
		unit = GetComponent<Unit> ();
		foreach (AudioData ad in unit.AudioData) {
			if (ad.playMode == AudioPlayMode.BindToAnimation) {
				AnimationBindedAudioData.Add (ad);
			}
		}
	}
	
	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		foreach (AudioData ad in AnimationBindedAudioData) {
			if (ad.enabled) {
				//Play binded audio, if the binded animation is playing
				if (unit.IsPlayingAnimation (ad.AnimationBinded) && (ad.BindedAudioSource.enabled == false || ad.BindedAudioSource.isPlaying == false)) {
					ad.BindedAudioSource.enabled = true;
					ad.BindedAudioSource.Play();
					Debug.Log("Replay bind audio:" + ad.Name);
				}
				//Stop binded audio, if the binded animation is not playing
				if (unit.IsPlayingAnimation (ad.AnimationBinded) == false && (ad.BindedAudioSource.enabled == true || ad.BindedAudioSource.isPlaying == true)) {
					ad.BindedAudioSource.Stop();
					ad.BindedAudioSource.enabled = false;
				}
			}
		}
	}
	
	public void _PlayAudio (string AudioDataName)
	{
		GlobalAudioSystem.PlayAudioData (unit.AudioDataDict [AudioDataName]);
	}
	
	void _EnableAudio (string AudioDataName)
	{
		unit.AudioDataDict [AudioDataName].enabled = true;
	}
	
	void _DisableAudio (string AudioDataName)
	{
		unit.AudioDataDict [AudioDataName].enabled = false;
	}
}
