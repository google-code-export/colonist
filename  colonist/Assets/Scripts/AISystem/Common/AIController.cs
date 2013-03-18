using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SwitchAICondition
{
	DistanceToCurrentTarget = 0,
	AttackCounter = 1,
	DoDamageCounter = 2,
	HPLowerThanValue = 3,
}

[System.Serializable]
public class SwitchAIData
{
	public string id = "default";
	public SwitchAICondition condition = new SwitchAICondition ();
	public string SwitchFromAI_Name = "";
	
	/// <summary>
	/// The name of the switch to AI. 
	/// If there is more than one AI name in the array, the switch to AI is randomly picked inside
	/// </summary>
	public string[] SwitchToAI_Name = new string[]{};
	public float FloatValue = 3;
	public int IntValue = 3;
	
}
	
/// <summary>
/// AI controller - control and on and off of each AI component attached to the game object.
/// </summary>
public class AIController:MonoBehaviour
{
	public SwitchAIData[] switchAIData = new SwitchAIData[]{ };
	public float SwitchAIInterval = 1;
	Unit unit = null;
	
	public void Awake ()
	{
		unit = GetComponent<Unit>();
	}

	public void Start ()
	{
		StartCoroutine ("AlternateAI", SwitchAIInterval);
	}

	public void Update ()
	{
	}
	
	IEnumerator AlternateAI (float interval)
	{
		float lastScanTime = 0;
		while (true) {
			if ((Time.time - lastScanTime) < interval) {
				yield return null;
				continue;
			}
			foreach (SwitchAIData data in switchAIData) {
			    CheckSwitchAICondition(data);
			}
			lastScanTime = Time.time;
		}
		
	}
    
	bool CheckSwitchAICondition (SwitchAIData data)
	{
	    switch(data.condition)
		{
		case SwitchAICondition.DistanceToCurrentTarget:
			break;
		}
		return false;
	}
	
}


