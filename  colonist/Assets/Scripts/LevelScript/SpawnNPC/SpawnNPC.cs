using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// SpawnData defines the data to spawn NPC.
/// Includes:
/// 1. Prefab to spawn. Mandatory.
/// 2. ObjectDock for the initial movement of the spawned unit. Optional.
/// 3. GameEvent to be sent , when spawning a unit.
/// 4. GameEvent to be sent , when a spawned unit destroyed.
/// 5. GameEvent to be sent, when a spawned die(require ApplyDamage component)
/// </summary>
[System.Serializable]
public class SpawnData
{
	public string Name = "";
	public string Description = "";
	public GameObject Spawned = null;
	/// <summary>
	/// If useObjectDock = true, objectDock must not be null.
	/// </summary>
	public ObjectDock objectDock = null;
}

/// <summary>
/// SpawnEntity - upper class of SpawnData. Consider a SpawnEntity is a "batch", which will spawn multiply NPC(using SpawnData)
/// in a batch.
/// SpawnEntity depends on SpawnData, defines these properties of spawn behavior:
/// 1. sequenece of spawning.
/// 2. how many time can respawn next.
/// 3. the wait seconds between 2 spawning.
/// </summary>
[System.Serializable]
public class SpawnEntity
{
	public string Name = "";
	/// <summary>
	/// if SpawnDelay > 0, will wait for the seconds then start spawning.
	/// </summary>
	public float SpawnDelay = 0;
	/// <summary>
	/// Assign the spawn data name in the array.
	/// </summary>
	public string[] SpawnDataNameArray = new string[]{ };
	
	/// <summary>
	/// The spawned objects lists.
	/// </summary>
	IList<GameObject> spawnedObjects = new List<GameObject>();
	
	public void AddSpawned(GameObject spawned)
	{
		spawnedObjects.Add(spawned);
	}
	
	/// <summary>
	/// return true if all unit is spawned
	/// </summary>
	public bool IsSpawnedCompleted()
	{
		return (spawnedObjects.Count == SpawnDataNameArray.Length);
	}
	
	/// <summary>
	/// return true if all spawned unit is dead.
	/// </summary>
	public bool IsSpawnedDieOut()
	{
		bool ret = true;
		if(IsSpawnedCompleted())
		{
			foreach(GameObject o in spawnedObjects)
			{
				if(o != null)
				{
					if(o.GetComponent<UnitHealth>() != null && o.GetComponent<UnitHealth>().GetCurrentHP() > 0)
					{
						ret = false;
						break;
					}
				}
			}
		}
		return ret;
	}
	
	public void ReplaceGameObject(GameObject _old, GameObject _new)
	{
		if(spawnedObjects.Contains(_old))
		{
			spawnedObjects.Remove(spawnedObjects.Where(x=>x==_old).First());			
		}
		spawnedObjects.Add(_new);
	}
}

/// <summary>
/// Monobehavior to control spawning NPC.
/// </summary>
public class SpawnNPC : MonoBehaviour, I_GameEventReceiver {
	public string Name = "";
	public SpawnData[] spawnNPCData = new SpawnData[]{};
	IDictionary<string, SpawnData> spawnDict = new Dictionary<string, SpawnData>();
	
	/// <summary>
	/// The events fired when the spawned units of the last SpawnData in spawnNPCData, has die out.
	/// </summary>
	public GameEvent[] Event_At_All_Spawned_DieOut = new GameEvent[]{};
	/// <summary>
	/// The spawn entity array.
	/// SpawnEntity will be spawned one by one. 
	/// </summary>
	public SpawnEntity[] spawnEntityArray = new SpawnEntity[] {};
	
	public SpawnEntity CurrentSpawnEntity = null;
	
	void Awake()
	{
		foreach(SpawnData s in spawnNPCData)
		{
		   spawnDict.Add(s.Name, s);
		}
	}
	
	public void OnGameEvent(GameEvent _event)
	{
		switch(_event.type)
		{
		case GameEventType.SpecifiedSpawn:
//			Spawn(spawnDict[_event.StringParameter]);
			StartCoroutine("DoSpawn");
			break;
		}
	}
	
	IEnumerator DoSpawn()
	{
	    foreach(SpawnEntity spawnEntity in this.spawnEntityArray)
		{
			CurrentSpawnEntity = spawnEntity;
			if(spawnEntity.SpawnDelay > 0)
			{
				yield return new WaitForSeconds(spawnEntity.SpawnDelay);
			}
			foreach(string spawnDataName in spawnEntity.SpawnDataNameArray)
			{
				SpawnData spawnData = spawnDict[spawnDataName];
				GameObject spawnedUnit = Spawn(spawnDict[spawnDataName]);
				spawnEntity.AddSpawned(spawnedUnit);
			}
			//when all spawned unit dies, turn to next SpawnEntity
			while(spawnEntity.IsSpawnedDieOut() == false)
			{
				yield return new WaitForSeconds(0.3333f);
			}
			Debug.Log("SpawnEntity:" + spawnEntity.Name + " has die out!");
		}
		
		if(Event_At_All_Spawned_DieOut != null && Event_At_All_Spawned_DieOut.Length > 0)
		{
			foreach(GameEvent e in Event_At_All_Spawned_DieOut)
			{
				LevelManager.OnGameEvent(e , this);
			}
		}
	}
	
	/// <summary>
	/// Given a spawn data, and perform the actual spawning behavior.
	/// </summary>	
	GameObject Spawn(SpawnData spawnData)
	{
		GameObject spawned = (GameObject)Object.Instantiate(spawnData.Spawned);
		//assign the spawner field.
		if(spawned.GetComponent<Unit>() != null )
		{
			spawned.GetComponent<Unit>().Spawner = this;
		}
		//locate the spawned object.
		spawnData.objectDock.Dock(spawned.transform);
		return spawned;
	}
	
	public void ReplaceSpawnedWithNewObject(GameObject _old, GameObject _new)
	{
		if(CurrentSpawnEntity!=null)
		{
			CurrentSpawnEntity.ReplaceGameObject(_old, _new);
		}
	}
}
