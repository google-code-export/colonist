using UnityEngine;
using System.Collections;

/// <summary>
/// Checkpoint data wraps:
/// 1. CheckPoint Name
/// 2. GameEvent that should be fired when loading checkpoint.
/// </summary>
[System.Serializable]
public class CheckpointData
{
	/// <summary>
	/// The name of the checkpoint.
	/// </summary>
	public string Name = "";
	public string Description = "";
	
	/// <summary>
	/// The event for checkpoint that should be fired when loading the CheckPoint.
	/// These events are checkpoint-specified.
	/// </summary>
	public GameEvent[] EventsForLoadingCheckpoint = new GameEvent[]{};

}

/// <summary>
/// Check point - manage the checkpoint data.
/// </summary>
public class CheckPointManager : MonoBehaviour
{
	/// <summary>
	/// The checkpoint data array.
	/// </summary>
	public CheckpointData[] CheckpointDataArray = new CheckpointData[]{ };
	public string FirstCheckPoint = "";
	
	/// <summary>
	/// These events are fired when saving checkpoint.
	/// They are for all checkpoints.
	/// You can define, for example, displaying the save icon event.
	/// </summary>
	public GameEvent[] EventsForSavingCheckpoint = new GameEvent[]{};
	
	/// <summary>
	/// checkPoint is passed through in runtime by the scene who loaded this scene, by calling Object.DontDestroyOnLoad().
	/// checkPoint indicates the purpose of player, that he/she want to load the level from beginning , or continue with the checkpoint of last play.
	/// </summary>
	CheckPoint checkPoint = null;

	public void Awake ()
	{
		Object checkPointObject = FindObjectOfType (typeof(CheckPoint));
		if(checkPointObject != null)
		{
			checkPoint = checkPointObject as CheckPoint;
		}
	}
	
	/// <summary>
	/// CALL this method to actually start the game, this is the real entrance of game thread.
	/// </summary>
	public void StartGame ()
	{
		//if there is checkPoint object pass in, load checkpoint according to the type
		if (checkPoint != null) {

			switch (checkPoint.loadCheckPointType) {
			case CheckPoint.LoadCheckPointType.LoadLastCheckpoint:
				//try to load the target checkpoint, if fails, then load the first checkpoint by default.
				if (this.LoadCheckpoint(checkPoint.LoadCheckPointName) == false) {
					this.LoadFirstCheckpoint ();
				}
				break;
			case CheckPoint.LoadCheckPointType.NewLevelStart:
				this.LoadFirstCheckpoint ();
				break;
			}
					
		//Destroy the checkpoint gameobject.
		Destroy(checkPoint.gameObject);
		}
		//else, load the first checkpoint.
		else 
		{
			this.LoadFirstCheckpoint ();
		}

	}
	
	/// <summary>
	/// By default, the first checkpoint starts the game of the level. Call this method to load the first checkpoint.
	/// </summary>
	public void LoadFirstCheckpoint ()
	{
		LoadCheckpoint (FirstCheckPoint);
	}
	
	/// <summary>
	/// Loads the checkpoint of name.
	public bool LoadCheckpoint (string Name)
	{
		foreach (CheckpointData chkData in CheckpointDataArray) {
			if (chkData.Name == Name) {
				foreach (GameEvent eve in chkData.EventsForLoadingCheckpoint) {
					LevelManager.OnGameEvent (eve, this);
				}
				return true;
			}
		}
		return false;
	}
	
	public void CheckPointReach (string Name)
	{
		//persist the progress in device.
		Persistence.SaveCheckPoint (LevelManager.Instance.LevelName, Name);
		foreach (GameEvent e in EventsForSavingCheckpoint) {
			LevelManager.OnGameEvent (e, this);
		}
	}
	
	/// <summary>
	/// Determines whether this whole game maintain a checkpoint, if yes, return true, 
	/// and the checkpoint level and checkpoint name is output.
	/// </summary>
	public bool HasLastCheckPoint (string currentLevelName, out string levelName, out string checkPoint)
	{
		Persistence.GetLastCheckPoint (out levelName, out checkPoint);
		//if checkpoint level != current level, return false
		if (levelName == currentLevelName) {
			return true;
		} else {
			return false;
		}
	}
	
	public bool GetLastCheckpoint(out string level, out string checkpoint)
	{
		bool hasCheckPoint = Persistence.GetLastCheckPoint (out level, out checkpoint);
		return hasCheckPoint;
	}
	
	/// <summary>
	/// Clears the checkpoint.
	/// Call this function when user want to start a new level.
	/// </summary>
	public void ClearCheckpoint ()
	{
		Persistence.ClearCheckPoint ();
	}
	

}
