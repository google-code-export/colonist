using UnityEngine;
using System.Collections;

public class RobotWarrior_Unit : Unit {
	
	// Use this for initialization
    void Awake()
	{
        InitUnitData();
		InitAnimation();
        InitUnitAI();
		Debug.Log("Init Unit at frame:" + Time.frameCount);
	}
}
