using UnityEngine;
using System.Collections;

public class SpawnEnemyLift : SpawnEnemyPoint {
	
	public float DetachTime = 1.7f;
	private GameObject spawnedUnit = null;
	
	public override GameObject DoSpawn()
	{
		spawnedUnit = base.DoSpawn();
		spawnedUnit.transform.parent = this.transform;
		Invoke("Detach",DetachTime);
		return spawnedUnit;
	}
	
	void Detach()
	{
		spawnedUnit.transform.parent = null;
	}
	
}
