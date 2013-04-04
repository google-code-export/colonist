using UnityEngine;
using System.Collections;

public enum EnemySpawnTimeRule
{
    GameEvent = 0,
    Time_Fixed_Interval = 1,
}

public class EnemySpawnWave
{

}

[System.Serializable]
public class EnemySpawnShot
{
    public bool RandomCreateUnit = true;
    public GameObject[] Unit = new GameObject[]{};
    public Transform SpawnCenter;
    public float SpawnAreaRadius;
    public int SpawnNumberOneShot = 3;
	 
    public int TotalSpawnTime = 3;
    public float SpawnInterval = 3;

    /// <summary>
    /// record the current spawn time
    /// </summary>
    [HideInInspector]
    public int spawncount = 0;
}

/// <summary>
/// EnemySpawner spawn enemy 
/// </summary>
public class EnemySpawner : MonoBehaviour {
    public EnemySpawnShot spawnShot = new EnemySpawnShot();
    void Awake()
    {

    }
	// Use this for initialization
	void Start () {
	   StartCoroutine("StartSpawn");
	}
	
	// Update is called once per frame
	void Update () {
	 
	}

    IEnumerator StartSpawn()
    {
        while (spawnShot.spawncount < spawnShot.TotalSpawnTime)
        {
            for (int i = 0; i < spawnShot.SpawnNumberOneShot; i++)
            {

                Vector2 random = Random.insideUnitCircle * spawnShot.SpawnAreaRadius;
                Vector3 randomPoint = spawnShot.SpawnCenter.position + new Vector3(random.x, 0, random.y);
                GameObject spawnee = (GameObject)Object.Instantiate(Util.RandomFromArray<GameObject>(spawnShot.Unit), 
					randomPoint,  spawnShot.SpawnCenter.rotation);
				Util.PutToGround(spawnee.transform, LevelManager.Instance.GroundLayer, 0);
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(spawnShot.SpawnInterval);
        }
    }
	
	
}
