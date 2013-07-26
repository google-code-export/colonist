using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MusicData
{
	public string Name = "theme";
	public AudioSource audioSource = null;
	/// <summary>
	/// If true, the music is played at awake.
	/// </summary>
	public bool PlayAtAwake = false;
	/// <summary>
	/// In FadeInLength, the music gradually increase to volume in %musicSource%.
	/// </summary>
	public float FadeInLength = 3;
	/// <summary>
	/// In FadeInLength, the music gradually decrease to 0.
	/// </summary>
	public float FadeOutLength = 3;
	/// <summary>
	/// if auto expire = true, the music will self-stop after ExpireTime.
	/// </summary>
	public bool AutoExpire = false;
	public float ExpireTime = 20;
	
	/// <summary>
	/// Only one music in one rank can be played at the same time.
	/// For example: there are three musicData:
	/// Music01.Rank = 1
	/// Music02.Rank = 1
	/// Music03.Rank = 2.
	/// 
	/// When music01 is playing, the music02 must not be played, but music03 is not impacted because music03 is at different rank.
	/// 
	/// </summary>
	public int ExclusiveRank = 1;
	/// <summary>
	/// The initial volume of the audioSource.
	/// </summary>
	[HideInInspector]
	public float InitialVolume;
}

/// <summary>
/// Not like 3D world sound, background music is always hearable to player.
/// but due to the way Unity handles audio, there must be a 3D point to play a sound, so BackgroundMusicPlayer 
/// is positioned at runtime, it will automatically aligh itself to the active audio listener.
/// </summary>
public class BackgroundMusicPlayer : MonoBehaviour, I_GameEventReceiver
{
	public MusicData[] BackgroundMusicDataArray = new MusicData[] { };
	IDictionary<string, MusicData> BackgroundMusicDataDict = new Dictionary<string, MusicData> ();
	AudioListener[] AvailableAudioListeners = new AudioListener[]{};
	AudioListener currentAttachedAudioListener = null;
	public bool AutoUpdatePositionToAudioListener = true;
	// Use this for initialization
	void Awake ()
	{
		foreach (MusicData data in BackgroundMusicDataArray) {
			data.InitialVolume = data.audioSource.volume;
			BackgroundMusicDataDict.Add (data.Name, data);
		}
		Object[] audioListeners = Object.FindObjectsOfType (typeof(AudioListener));
		AvailableAudioListeners = Util.ConvertObjectArray<AudioListener> (audioListeners);
	}
	
	void Start ()
	{
		foreach (MusicData musicData in BackgroundMusicDataArray) {
			if(musicData.PlayAtAwake)
			   StartCoroutine("PlayMusic", musicData.Name);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(AutoUpdatePositionToAudioListener)
		   UpdatePosition ();
	}
	
	void UpdatePosition ()
	{
		//Update position of the current player to the top active listener.
		if (currentAttachedAudioListener == null || 
			currentAttachedAudioListener.enabled == false || 
			currentAttachedAudioListener.gameObject.active == false) {
			foreach (AudioListener listener in AvailableAudioListeners) {
				//there must only one AudioListener active at one time.
				if (listener.enabled && listener.gameObject.active) {
					//assign currentAttachedAudioListener to the top listener
					currentAttachedAudioListener = listener;
				}
			}
		}
		//align position
		if(currentAttachedAudioListener != null)
		   this.transform.position = currentAttachedAudioListener.transform.position;
	}
	
	public void OnGameEvent (GameEvent gameEvent)
	{
		switch (gameEvent.type) {
		case GameEventType.PlayBackgroundMusic:
			StartCoroutine("PlayMusic", gameEvent.StringParameter);
			break;
		case GameEventType.StopBackgroundMusic:
			StartCoroutine("StopMusic", gameEvent.StringParameter);
			break;
		case GameEventType.StopAllBackgroundMusic:
			StopAllMusic();
			break;
		}
	}
	
	IEnumerator PlayMusic (string MusicDataname)
	{
		float startTime = Time.time;
		MusicData musicData = BackgroundMusicDataDict [MusicDataname];
		//stop the musicdata at the same rank
		foreach(MusicData mu in this.BackgroundMusicDataArray)
		{
			if(mu != musicData && mu.ExclusiveRank == musicData.ExclusiveRank && mu.audioSource.isPlaying)
			{
				StartCoroutine("StopMusic", mu.Name);
			}
		}
		if(musicData.audioSource.enabled)
		{
		   musicData.audioSource.enabled = true;
		}
		musicData.audioSource.Play();
		if (musicData.FadeInLength > 0) {
			while ((Time.time - startTime) <= musicData.FadeInLength) {
				float _t = (Time.time - startTime) / musicData.FadeInLength;
				musicData.audioSource.volume = Mathf.Lerp(0, musicData.InitialVolume, _t);
			    yield return null;
			}
		}
		musicData.audioSource.volume = musicData.InitialVolume;
		
		if(musicData.AutoExpire)
		{
			yield return new WaitForSeconds(musicData.ExpireTime);
			StartCoroutine("StopMusic", musicData.Name);
		}
	}
	
	IEnumerator StopMusic (string MusicDataname)
	{
		float startTime = Time.time;
		MusicData musicData = BackgroundMusicDataDict [MusicDataname];
		musicData.audioSource.Play();
		if (musicData.FadeOutLength > 0) {
			while ((Time.time - startTime) <= musicData.FadeOutLength) {
				float _t = 1 - (Time.time - startTime) / musicData.FadeOutLength;
				musicData.audioSource.volume = Mathf.Lerp(0, musicData.InitialVolume, _t);
			    yield return null;
			}
		}
		musicData.audioSource.Stop();
		musicData.audioSource.enabled = false;
	}
	
	void StopAllMusic()
	{
		foreach( MusicData music in BackgroundMusicDataArray)
		{
			if(music.audioSource.isPlaying)
			{
				music.audioSource.Stop();
			}
		}
	}
}
