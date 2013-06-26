using UnityEngine;
using System.Collections;

public class AudioMuteUnmuteButtons : MonoBehaviour {
    public JoyButton Mute;
	public JoyButton Unmute;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	   if(Persistence.IsMute())
	   {
			Unmute.gameObject.active = true;
			Mute.gameObject.active = false;
	   }
	   else 
	   {
			Unmute.gameObject.active = false;
			Mute.gameObject.active = true;
	   }
	}
}
