using UnityEngine;
using System.Collections;

/// <summary>
/// CheckPoint is passed to new scene when loading level.
/// After the first frame of Level Loaded, if there is a GameObject in scene with CheckPoint script attached to, means the checkpoint manager should 
/// load the previous saved checkpoint.
/// </summary>
public class CheckPoint : MonoBehaviour {

	public enum LoadCheckPointType 
	{
		/// <summary>
		/// Indicates this is a totally new game, player want to play the new level with the inital beginning.
		/// </summary>
		NewLevelStart = 0,
		
		/// <summary>
		/// Indicates this is a resumed game, player want to continue with the previous saved milestone.
		/// </summary>
		LoadLastCheckpoint = 1,
	}
	/// <summary>
	/// The type of the load check point.
	/// By default it's NewLevelStart.
	/// </summary>
	public LoadCheckPointType loadCheckPointType = LoadCheckPointType.NewLevelStart;
	
	/// <summary>
	/// The loaded check point's name.
	/// The variable is only used when loadCheckPointType == LoadLastCheckpoint
	/// </summary>
	public string LoadCheckPointName = "CheckPointName";
}
