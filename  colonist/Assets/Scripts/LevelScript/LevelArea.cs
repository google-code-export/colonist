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
	/// <summary>
	/// The name of this level area.
	/// </summary>
	public string Name = "Default Area Name";
	
	/// <summary>
	/// The event fired at player entrance.
	/// </summary>
	public GameEvent[] eventFiredAtPlayerEnter = new GameEvent[]{};
	/// <summary>
	/// The event fired at player exit.
	/// </summary>
	public GameEvent[] eventFiredAtPlayerExit = new GameEvent[]{};
	
	/// <summary>
	/// The working flag, only fire event when working = true.
	/// </summary>
	public bool working = true;
	
	/// <summary>
	/// A flag, mark if the entrance event has been fired.
	/// </summary>
	bool enterEventFired = false;
	
	/// <summary>
	/// A flag, mark if the exit event has been fired.
	/// </summary>
	bool exitEventFired = false;
	
	void Awake ()
	{
		//Mark the collider as trigger, as LevelArea is not supposed to be collided with anything.
		this.collider.isTrigger = true;
	}
	
	public void SetWorkingFlag(bool flag)
	{
		working = flag;
	}
	
	public void OnTriggerEnter (Collider other)
	{
		if (working && !enterEventFired && eventFiredAtPlayerEnter != null && eventFiredAtPlayerEnter.Length > 0) {
				//only trigger entrance with player
				if (other.gameObject.tag.ToLower ().Contains ("player")) {
					foreach (GameEvent e in eventFiredAtPlayerEnter) {
						LevelManager.OnGameEvent (e, this);
					}
					//clear the event array:
					eventFiredAtPlayerEnter = null;
				}
		}
	}
	
	public void OnTriggerExit (Collider other)
	{
       if (working && !exitEventFired && eventFiredAtPlayerExit != null && eventFiredAtPlayerExit.Length > 0) {
			//only trigger exit with player
			if (other.gameObject.tag.ToLower ().Contains ("player")) {
				foreach (GameEvent e in eventFiredAtPlayerExit) {
					LevelManager.OnGameEvent (e, this);
				}
				//clear the event array:
				eventFiredAtPlayerExit = null;
			}
		}
	}
}
