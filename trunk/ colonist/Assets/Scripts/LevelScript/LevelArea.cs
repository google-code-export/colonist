using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Define the condition check type to spawn next AtomSpawnData 
/// </summary>
public enum SpawnNextConditionType
{
	/// <summary>
	/// Start next AtomSpawnData when all the enemy dies.
	/// </summary>
	SpawnNextWhenAllUnitDie = 0,
	/// <summary>
	/// Start next AtomSpawnData when there're N unit remain living.
	/// </summary>
	SpawnNextWhenNUnitRemain = 1,
	
	/// <summary>
	/// Start next AtomSpawnData when N seconds pass.
	/// </summary>
	SpawnNextWhenTimePass = 2,
	
	/// <summary>
	/// Start next AtomSpawnData by event.
	/// </summary>
	SpawnByEvent = 3,
}

/// <summary>
/// SpawnIdentity is used to identify a unit's spawn by which LevelArea and which SpawnWave
/// </summary>
public class SpawnIdentity
{
	public string LevelAreaName = "";
	public string SpawnWaveName = "";
	public SpawnIdentity(string areaName, string waveName)
	{
		LevelAreaName = areaName;
		SpawnWaveName = waveName;
	}
}

/// <summary>
/// SpawnWave defines data for spawning NPC in one time. Include:
/// 1. Spawn game object prefab
/// 2. SpawnPoint
/// 3. Spawn next wave condition
/// 4. Sapwn next wave's ID
/// 5. Start delay time in seconds.
/// </summary>
[System.Serializable]
public class SpawnWave
{
	/// <summary>
	/// Unique name of the SpawnWave
	/// </summary>
	public string Name = "Default ID";
	/// <summary>
	/// The start delay time.
	/// </summary>
	public float StartDelay = 1;
	/// <summary>
	/// The spawn point array. when there're more than one element in the array, 
	/// will randomly pick one spawn enemy point.
	/// </summary>
	public SpawnEnemyPoint[] SpawnPoint = new SpawnEnemyPoint[] { };
	/// <summary>
	/// The spawnee game object. Assign prefab to this variable.
	/// </summary>
	public GameObject[] SpawneePrefab = new GameObject[] { };
	public bool RandomChooseSpawnPoint = true;
	
	/// <summary>
	/// Use this list to store the spawnee instance.
	/// </summary>
	[HideInInspector]
	public List<GameObject> InstanceList = new List<GameObject>();
	
	/// <summary>
	/// Record the spawn count.
	/// </summary>
	[HideInInspector]
	public int SpawnCount = 0;
	
	/// <summary>
	/// Record the die count.
	/// </summary>
	[HideInInspector]
	public int DieCount = 0;
	
	/// <summary>
	/// Turn on this flag when all game object has been spawned.
	/// </summary>
	[HideInInspector]
	public bool SpawnCompleted = false;
	
	[HideInInspector]
	public float SpawnStarttime = 0;
	
	[HideInInspector]
	public float SpawnCompletedTime = 0;
	
	/// <summary>
	/// The index of the spawn point. Only used when RandomChooseSpawnPoint = false
	/// </summary>
	[HideInInspector]
	public int SpawnPointIndex = 0;
	/// <summary>
	/// The next atom spawn data's name.
	/// </summary>
	public string NextWave = "";
	public SpawnNextConditionType spawnNextConditionType = SpawnNextConditionType.SpawnNextWhenAllUnitDie;
	public float SpawnByTimePass = 0;
	public int SpawnByUnitRemain = 0;
}

/// <summary>
/// LevelArea - one of the most important element to compose a complete level.
/// A LevelArea is a X-Z surface. When player enter/leave the area, things happen.
/// LevelArea depends on Collider.OnTriggerEnter / OnTriggerExit to work.
/// </summary>
[RequireComponent(typeof (Collider))]
public class LevelArea : MonoBehaviour, I_GameEventReceiver {
	public string Name = "Default Area";
	
    public string[] InvokeSpawningTag = new string[] { "Player" };
	
	public bool SpawnAutomaticallyWhenPlayerEnter = false;
	public SpawnWave[] SpawnWaveArray = new SpawnWave[]{};
	IDictionary<string, SpawnWave> SpawnWaveDict = new Dictionary<string,SpawnWave>();
	
    const string PlayerTag = "Player";

    /// <summary>
    /// list to store trigger gameobjects
    /// </summary>
    private IList<GameObject> triggerObjects = new List<GameObject>();
	
	SpawnWave CurrentSpawnWave = null;
	
	static IDictionary<string, LevelArea> LevelAreaMap = new Dictionary<string, LevelArea>();
	
	bool HasSpawned = false;
	
    void Awake()
    {
        collider.isTrigger = true;
		foreach(SpawnWave wave in SpawnWaveArray)
		{
			SpawnWaveDict.Add(wave.Name, wave);
		}
		LevelAreaMap.Add(this.Name, this);
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
		if(CurrentSpawnWave != null && CurrentSpawnWave.SpawnCompleted == true)
		{
			if(CurrentSpawnWave.NextWave != null && CurrentSpawnWave.NextWave != string.Empty)
			{
				//if next spawn wave's start condition match, start next spawn wave
			   if(CheckSpawnNextCondition(CurrentSpawnWave) == true)
			   {
			       CurrentSpawnWave = SpawnWaveDict[CurrentSpawnWave.NextWave];
				   StartCoroutine("Spawn",CurrentSpawnWave);
				   Debug.Log("Spawn next wave:" + CurrentSpawnWave.Name);
			   }
			}
		}
		
//		if(Input.GetKeyDown(KeyCode.T))
//		{
//			CurrentSpawnWave = this.SpawnWaveArray[0];
//			StartCoroutine("Spawn",CurrentSpawnWave);
//		}
	}
	
	/// <summary>
	/// Checks if the given spawn wave's next spawn wave can be started.
	/// </summary>
	bool CheckSpawnNextCondition(SpawnWave spawnData)
	{
		bool ret = false;
		switch(spawnData.spawnNextConditionType)
		{
		case SpawnNextConditionType.SpawnByEvent:
			break;
		case SpawnNextConditionType.SpawnNextWhenAllUnitDie:
			ret = (spawnData.DieCount == spawnData.SpawnCount);
			break;
		case SpawnNextConditionType.SpawnNextWhenTimePass:
			ret = (Time.time - spawnData.SpawnStarttime) > spawnData.SpawnByTimePass;
			break;
		case SpawnNextConditionType.SpawnNextWhenNUnitRemain:
			ret = ((spawnData.SpawnCount - spawnData.DieCount) <= spawnData.SpawnByUnitRemain);
			break;
		}
		return ret;
	}

    void OnDrawGizmosSelected()
    {
    }
	
	/// <summary>
	/// Starts spawning NPC.
	/// </summary>
	public void StartSpawn()
	{
		CurrentSpawnWave = this.SpawnWaveArray[0];
		StartCoroutine("Spawn",CurrentSpawnWave);
	}
	
    void OnTriggerEnter(Collider other)
    {
		if(this.enabled == false || HasSpawned)
			return;
		//If the player taged unit enter the LevelArea, the SpawnWave will be invoker and start to spawning unit.
		if(SpawnAutomaticallyWhenPlayerEnter && Util.ArrayContains<string>(InvokeSpawningTag, other.tag) && HasSpawned == false)
		{
		   HasSpawned = true;
           if( Util.ArrayContains<string>(InvokeSpawningTag, other.tag) )
		   {
			   StartSpawn();
		   }
		}
    }

    void OnTriggerExit(Collider other)
    {
    }
	
	/// <summary>
	/// Spawn the specified AtomSpawnData.
	/// </summary>
	/// <param name='spawnWave'>
	/// Atom spawn data.
	/// </param>
	IEnumerator Spawn(SpawnWave spawnWave)
	{
		if(spawnWave.StartDelay > 0)
		{
			yield return new WaitForSeconds(spawnWave.StartDelay);
		}
//		Debug.Break();
		for(int i=0; i<spawnWave.SpawneePrefab.Length; i++)
		{
			SpawnEnemyPoint spawnPoint = null;
			if(spawnWave.RandomChooseSpawnPoint)
			{
				spawnPoint = Util.RandomFromArray<SpawnEnemyPoint>(spawnWave.SpawnPoint);
			}
			else {
				spawnPoint = spawnWave.SpawnPoint[spawnWave.SpawnPointIndex];
				spawnWave.SpawnPointIndex = (++spawnWave.SpawnPointIndex) % spawnWave.SpawnPoint.Length;
			}
//			Debug.Log("Spawn point:" + spawnPoint.name);
			spawnPoint.Spawn(spawnWave.SpawneePrefab[i], GetSpawnIdentity(spawnWave));
		}
		spawnWave.SpawnStarttime = Time.time;
	}
	
	/// <summary>
	/// Callback function, to add the spawnee into monitor list.
	/// </summary>
	public void OnSpawning(GameObject _obj, SpawnIdentity identity)
	{
//		Debug.Log("On spawning:" + _obj.name);
		CurrentSpawnWave.InstanceList.Add(_obj);
		CurrentSpawnWave.SpawnCount++;
		//if spawn counter equals to the defined spawnee prefab, means the SpawnWave has been completed consumed.
		if(CurrentSpawnWave.SpawnCount == CurrentSpawnWave.SpawneePrefab.Length)
		{
			CurrentSpawnWave.SpawnCompleted = true;
			CurrentSpawnWave.SpawnCompletedTime = Time.time;
		}
	}
	/// <summary>
	/// When a spawned gameobject destoryed, call this method
	/// to remove the spawnee from monitor list .
	/// </summary>
	public void OnSpawneeDie(GameObject _obj, SpawnIdentity identity)
	{
		SpawnWaveDict[identity.SpawnWaveName].InstanceList.Remove(_obj);
		SpawnWaveDict[identity.SpawnWaveName].DieCount++;
	}
	
	SpawnIdentity GetSpawnIdentity(SpawnWave wave)
	{
		return new SpawnIdentity(this.Name, wave.Name);
	}
	
	public static LevelArea GetArea(string AreaName)
	{
		return LevelAreaMap[AreaName];
	}
	
	void OnDestroy ()
	{
		LevelAreaMap.Remove(this.Name);
	}
	
	public void OnGameEvent(GameEvent _e)
	{
		switch(_e.type)
		{
		    case GameEventType.LevelAreaStartSpawn:
			StartSpawn();
			break;
		}
	}
}
