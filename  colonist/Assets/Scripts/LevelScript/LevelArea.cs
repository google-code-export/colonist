using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// LevelArea - trigger the events when player enter/exit the level collider.
/// Note:  the events are intend to be triggered for only one time.
/// </summary>
[RequireComponent(typeof(Collider))]
public class LevelArea : MonoBehaviour
{
	
	public string Name = "Default Area Name"; 
	public GameEvent[] eventFiredAtPlayerEnter = new GameEvent[]{};
	public GameEvent[] eventFiredAtPlayerExit = new GameEvent[]{};
	bool enterEventFired = false;
	bool exitEventFired = false;
	
	void Awake ()
	{
		this.collider.isTrigger = true;
	}
	
	public void OnTriggerEnter (Collider other)
	{
		if (!enterEventFired && eventFiredAtPlayerEnter != null && eventFiredAtPlayerEnter.Length > 0) {
			if (other.gameObject.tag.ToLower ().Contains ("player")) {
				foreach (GameEvent e in eventFiredAtPlayerEnter) {
					LevelManager.OnGameEvent (e, this);
				}
			}
		}
	}
	
	public void OnTriggerExit (Collider other)
	{
		if (!exitEventFired && eventFiredAtPlayerExit != null && eventFiredAtPlayerExit.Length > 0) {
			if (other.gameObject.tag.ToLower ().Contains ("player")) {
				foreach (GameEvent e in eventFiredAtPlayerExit) {
					LevelManager.OnGameEvent (e, this);
				}
			}
		}
	}
}
