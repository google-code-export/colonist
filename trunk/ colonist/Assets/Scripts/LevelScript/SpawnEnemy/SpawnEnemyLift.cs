using UnityEngine;
using System.Collections;

public class SpawnEnemyLift : SpawnEnemy {
	
	public float DetachTime = 1.7f;
	
	public override void DoSpawn()
	{
		base.DoSpawn();
		spawnedUnit.transform.parent = this.transform;
		Invoke("Detach",DetachTime);
	}
	
	void Detach()
	{
		spawnedUnit.transform.parent = null;
		Debug.Log("Detach!!");
	}
	
	void Update()
	{
		if(spawnedUnit == null && Spawning == false)
		{
			Debug.Log("It's dead!");
			Invoke("Spawn",0);
		}
	}
}
