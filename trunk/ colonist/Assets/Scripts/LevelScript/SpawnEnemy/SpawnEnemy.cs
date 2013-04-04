using UnityEngine;
using System.Collections;

public enum SpawnMode 
{
	/// <summary>
	/// If spawn mode = animation event, the enemy spawning is triggered by AnimationEvent_Spawn event.
	/// </summary>
	ByAnimationEvent = 0,
	ByGameEvent = 1,
	ByTime = 2,
}

public enum SpawnPositionMode
{
	/// <summary>
	/// the spawn point is transform.position.
	/// </summary>
	SpawnPointPivot = 0,
	/// <summary>
	/// You need to assign one external transform.
	/// </summary>
	ExternalTransform = 1,
}

public enum SpawnRotationMode
{
	/// <summary>
	/// face to player.(ignore y-axis)
	/// </summary>
	FaceToPlayer = 0,
	/// <summary>
	/// Quaternion.Identity
	/// </summary>
	Identity = 1,
	/// <summary>
	/// transform.quaternion
	/// </summary>
	SpawnPointQuaternion = 2,
}

public enum RespawnMode 
{
	/// <summary>
	/// Respawn enemy after the previous enemy die.
	/// </summary>
	LastSpawneeDie = 0,
	/// <summary>
	/// Respawn in every fixed seconds.
	/// </summary>
	FixedTimeInterval = 1,
	/// <summary>
	/// Total enemy number lesser than a fixed number
	/// </summary>
	TotalEnemyNumberLesserThan = 2,
}


[System.Serializable]
public class SpawnEnemyRule
{
	public SpawnMode spawnMode = SpawnMode.ByAnimationEvent;
	public SpawnPositionMode spawnPositionMode = SpawnPositionMode.SpawnPointPivot;
	public SpawnRotationMode spawnRotationMode = SpawnRotationMode.FaceToPlayer;
	public int MaxSpawnTime = 3;
	
}


public class SpawnEnemy : MonoBehaviour {
	public string Name = "Point";
	public SpawnEnemyRule spawnRule = new SpawnEnemyRule();
	public GameObject SpawnUnit = null;
	/// <summary>
	/// The animation_to spawn. Within the animation, the event AnimationEvent_Spawn must be existed.
	/// </summary>
	public string Animation_Spawn = "";
	
	public Transform[] externalTransform = null;
	[HideInInspector]
	public GameObject spawnedUnit = null;
	[HideInInspector]
	public bool Spawning = false;
	void Update()
	{
		if(spawnedUnit == null && Spawning == false)
		{
			Invoke("Spawn",0);
		}
	}
	
	public void Spawn()
	{
		switch(spawnRule.spawnMode)
		{
		case SpawnMode.ByAnimationEvent:
			animation.Rewind();
			animation.CrossFade(Animation_Spawn);
			Spawning=true;
			break;
		case SpawnMode.ByGameEvent:
			break;
		case SpawnMode.ByTime:
			break;
	    default:
		    break;
		}
	}
	
	/// <summary>
	/// _animation event.
	/// </summary>
	public void _Spawn()
	{
		DoSpawn();
	}
	
	public virtual void DoSpawn()
	{
		GameObject spawnee = (GameObject)Object.Instantiate(SpawnUnit);
		
		switch(spawnRule.spawnPositionMode)
		{
		case SpawnPositionMode.SpawnPointPivot:
		    spawnee.transform.position = this.transform.position;
			break;
		case SpawnPositionMode.ExternalTransform:
			Transform _transform = Util.RandomFromArray<Transform>( this.externalTransform);
			spawnee.transform.position = _transform.position;
			break;
		}
		
		switch(spawnRule.spawnRotationMode)
		{
		case SpawnRotationMode.FaceToPlayer:
                Vector3 PlayerPos = LevelManager.Instance.player.transform.position;
                PlayerPos.y = spawnee.transform.position.y;
                spawnee.transform.LookAt(PlayerPos);
			break;
		case SpawnRotationMode.Identity:
			break;
		case SpawnRotationMode.SpawnPointQuaternion:
		default:
            spawnee.transform.rotation = transform.rotation;
			break;
		}
		spawnedUnit = spawnee;
		Spawning = false;
	}
}
