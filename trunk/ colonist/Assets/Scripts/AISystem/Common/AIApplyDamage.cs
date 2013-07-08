using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ApplyDamageConditionType
{
	/// <summary>
	/// The unit receives damage greater than a valve. 
	/// </summary>
	ReceiveDamageAmountGreaterThanValue = 0,
	/// <summary>
	/// In a fixed time, receive amount great enough.
	/// For example, in 3 seconds receive damage > 50.
	/// </summary>
	ReceiveDamageAmountInTimePeriod = 1,
}

/// <summary>
/// Defines in what condition should applying the ReceiveDamageData.
/// For example, there is a high-level enemy, which you want the enemy not react to player's every hit, 
/// but only react to when player do a great amount of hit in a period of time.
/// </summary>
[System.Serializable]
public class ApplyDamagerCondition
{
	public string Name = "";
	public ApplyDamageConditionType applyDamageConditionType = ApplyDamageConditionType.ReceiveDamageAmountGreaterThanValue;
	/// <summary>
	/// The in-time period.
	/// </summary>
	public float InTime = 3;
	/// <summary>
	/// The damage amount valve to trigger ReceiveDamageData
	/// </summary>
	public float DamageAmountValve = 50;
	
	/// <summary>
	/// The last reset time, recorded in runtime.
	/// </summary>
	[HideInInspector]
	public float LastResetTime = 0;
	
	/// <summary>
	/// The current damage amount recorded in run-time.
	/// </summary>
	[HideInInspector]
	public float CurrentDamageAmount = 0;
	
	/// <summary>
	/// Shoulds do the reset behavior to this ApplyDamageCondition object?
	/// It's only applicable for those time-related type.
	/// </summary>
//	public bool ShouldReset()
//	{
//		bool shouldReset = true;
//		switch(this.applyDamageConditionType)
//		{
//		case ApplyDamageConditionType.ReceiveDamageAmountGreaterThanValue:
//			shouldReset = CurrentDamageAmount > 0;
//			break;
//		case ApplyDamageConditionType.ReceiveDamageAmountInTimePeriod:
//			shouldReset = CurrentDamageAmount > 0 && ((Time.time - LastResetTime) > InTime);
//			break;
//		}
//		return shouldReset;
//	}
//	
	/// <summary>
	/// for those time-related condition-type, reset the condition data.
	/// </summary>
	public void Reset()
	{
		switch(this.applyDamageConditionType)
		{
		case ApplyDamageConditionType.ReceiveDamageAmountGreaterThanValue:
			CurrentDamageAmount = 0;
			break;
		case ApplyDamageConditionType.ReceiveDamageAmountInTimePeriod:
			CurrentDamageAmount = 0;
			LastResetTime = 0;
			break;
		}
	}
	
	/// <summary>
	/// Determines whether this instance is apply damage condition match.
	/// if the applyDamage condition matched, reset the internal condition values.
	/// </summary>
	public bool IsApplyDamageConditionMatch ()
	{
		bool isMatched = false;
		switch(this.applyDamageConditionType)
		{
		case ApplyDamageConditionType.ReceiveDamageAmountGreaterThanValue:
			isMatched = CurrentDamageAmount > this.DamageAmountValve;
			if(isMatched)
			{
				this.Reset();
			}
			break;
		case ApplyDamageConditionType.ReceiveDamageAmountInTimePeriod:
			isMatched = CurrentDamageAmount > this.DamageAmountValve;
			if(isMatched)
			{
				this.Reset();
			}
			break;
		}
		return isMatched;
	}
	
	/// <summary>
	/// CALL this method, when the unit apply a damageParameter.
	/// </summary>
	public void UpdateDamageInfo(DamageParameter dp)
	{
		switch(this.applyDamageConditionType)
		{
		case ApplyDamageConditionType.ReceiveDamageAmountGreaterThanValue:
			this.CurrentDamageAmount += dp.damagePoint;
			break;
		case ApplyDamageConditionType.ReceiveDamageAmountInTimePeriod:
			this.CurrentDamageAmount += dp.damagePoint;
			this.LastResetTime = Time.time;
			break;
		}
	}
}

public class AIApplyDamage : MonoBehaviour, I_ReceiveDamage {
	
	public ApplyDamagerCondition[] ApplyDamagerConditionArray = new ApplyDamagerCondition[]{};
	
	protected Unit unit;
	protected CharacterController controller;
	
	void Awake()
	{
		unit = GetComponent<Unit>();
		unit.receiveDamageStatus = UnitReceiveDamageStatus.vulnerable;
		controller = GetComponent<CharacterController>();
	}
	
#region Receive Damage and Die
    public virtual IEnumerator ApplyDamage(DamageParameter damageParam)
    {
        if (unit.IsDead)
        {
            yield break;
		}
		
		if(unit.receiveDamageStatus == UnitReceiveDamageStatus.invincible)
		{
			yield break;
		}
		
        //Minus HP:
        unit.HP -= damageParam.damagePoint;
		//if HP < 0, goto Die routine
        if (unit.HP <= 0)
        {
			SendMessage("Die", damageParam);
            yield break;
        }
		//Else, goto DoDamage routine.
        else
        {
            StartCoroutine("DoDamage", damageParam);
			this.unit.ReceiveDamageCounter++;
            yield break;
        }
        
    }
	
	
	/// <summary>
	/// Receives a damageParam, and performs consequential behavior - play particle, play animation..etc.
	/// </summary>
    public virtual IEnumerator DoDamage(DamageParameter damageParam)
    {
		if(this.unit.receiveDamageStatus == UnitReceiveDamageStatus.vulnerableButNotReactToDamage || 
			this.unit.receiveDamageStatus == UnitReceiveDamageStatus.invincible)
			yield break;
		
#region if the unit is defined with ApplyDamageCondition, update the condition data.
        if(this.ApplyDamagerConditionArray != null && ApplyDamagerConditionArray.Length > 0)
		{
			foreach(ApplyDamagerCondition applyDamageCondition in ApplyDamagerConditionArray)
			{
				applyDamageCondition.UpdateDamageInfo(damageParam);
			}
		}
#endregion
		
#region check the ApplyDamageCondition
		bool CanApplyDamage = false;
        if(this.ApplyDamagerConditionArray != null && ApplyDamagerConditionArray.Length > 0)
		{
			foreach(ApplyDamagerCondition applyDamageCondition in ApplyDamagerConditionArray)
			{
				if(applyDamageCondition.IsApplyDamageConditionMatch())
				{
					CanApplyDamage = true;
					continue;
				}
			}
		}
		else 
		{
			//if no applyDamageCondition is defined, we assume the unit can apply any damage by default.
			CanApplyDamage = true;
		}
		
		if(CanApplyDamage == false)
		{
			yield break;
		}
#endregion
		
		#region Look for the suitable ReceiveDamageData 
		//if there is no receive damage defined, quit now!
		if(this.unit.ReceiveDamageData.Length == 0)
		{
			yield break;
		}
        //Get ReceiveDamageData
        ReceiveDamageData receiveDamageData = null;
        if (unit.ReceiveDamageDataDict.ContainsKey(damageParam.damageForm))
        {
            if (unit.ReceiveDamageDataDict[damageParam.damageForm].Count == 1)
            {
                receiveDamageData = unit.ReceiveDamageDataDict[damageParam.damageForm][0];
            }
            else //if more than one matched receive damage data is found, randomly choose one.
            {
                int RandomIndex = Random.Range(0, unit.ReceiveDamageDataDict[damageParam.damageForm].Count);
                receiveDamageData = unit.ReceiveDamageDataDict[damageParam.damageForm][RandomIndex];
            }
        }
        //If ReceiveDamageDataDict[DamageForm.Common] = null or Count ==0, will have error!
        //So make sure you have assign a Common receive damage data!
        else
        {
            receiveDamageData = unit.ReceiveDamageDataDict[DamageForm.Common][0];
        }
		#endregion
        yield return StartCoroutine(ProcessReceiveDamageData(receiveDamageData));
    }
	
	public virtual IEnumerator ProcessReceiveDamageData(ReceiveDamageData receiveDamageData)
	{
		if(this.unit.receiveDamageStatus == UnitReceiveDamageStatus.vulnerableButNotReactToDamage || 
			this.unit.receiveDamageStatus == UnitReceiveDamageStatus.invincible)
			yield break;
		//Create effect data
        if (receiveDamageData.EffectDataName != null && receiveDamageData.EffectDataName.Length > 0)
        {
            foreach (string effectDataName in receiveDamageData.EffectDataName)
            {
                EffectData effectData = unit.EffectDataDict[effectDataName];
                GlobalBloodEffectDecalSystem.CreateEffect(effectData); 
            }
        }
        //Create blood decal:
        if (receiveDamageData.DecalDataName != null && receiveDamageData.DecalDataName.Length > 0)
        {
            foreach (string decalName in receiveDamageData.DecalDataName)
            {
                DecalData DecalData = unit.DecalDataDict[decalName];
                GlobalBloodEffectDecalSystem.CreateBloodDecal(transform.position + controller.center, DecalData);
            }
        }
		
		//Play audio:
		if(receiveDamageData.AudioDataName != null && receiveDamageData.AudioDataName.Length > 0)
		{
            foreach (string audioName in receiveDamageData.AudioDataName)
            {
                GetComponent<AudioController>()._PlayAudio(audioName);
            }
		}
		
        //Halt AI if set true, stop all animation, and play the receive damage animation
        if (receiveDamageData.HaltAI)
        {
			animation.Stop();
            animation.Rewind();
            animation.CrossFade(receiveDamageData.AnimationName);
			SendMessage("HaltUnit", animation[receiveDamageData.AnimationName].length);
            yield return new WaitForSeconds(animation[receiveDamageData.AnimationName].length);
        }
	}

    public virtual IEnumerator Die(DamageParameter DamageParameter)
	{
        //Basic death processing.
		if(controller != null)
           controller.enabled = false;
        unit.IsDead = true;
		//stop and remove AI
        foreach(AI _ai in GetComponents<AI>())
		{
			_ai.StopAI();
			Destroy(_ai);
		}
		//stop and remove navigator
		foreach(Navigator nav in GetComponents<Navigator>())
		{
			nav.StopAllCoroutines();
			Destroy(nav);
		}
		if(animation != null)
           animation.Stop();

        //Handle DeathData:
        DeathData deathData = null;
        if(unit.DeathDataDict.ContainsKey(DamageParameter.damageForm))
        {
            IList<DeathData> DeathDataList = unit.DeathDataDict[DamageParameter.damageForm];
            deathData = Util.RandomFromList(DeathDataList);
        }
        else 
        {
			//if no DeathData matched to the DamageForm in DamageParameter,use the DamageForm.Common
            deathData = Util.RandomFromList<DeathData>( unit.DeathDataDict[DamageForm.Common]);
        }

        //Create effect data 
        if (deathData.EffectDataName != null && deathData.EffectDataName.Length > 0)
        {
            foreach (string effectDataName in deathData.EffectDataName)
            {
                EffectData effectData = unit.EffectDataDict[effectDataName];
                GlobalBloodEffectDecalSystem.CreateEffect(effectData);
            }
        }
        //Create blood decal:
        if (deathData.DecalDataName != null && deathData.DecalDataName.Length > 0)
        {
            foreach (string decalName in deathData.DecalDataName)
            {
                DecalData DecalData = unit.DecalDataDict[decalName];
                GlobalBloodEffectDecalSystem.CreateBloodDecal(transform.position + controller.center, DecalData);
            }
        }
		//Play audio:
		if(deathData.AudioDataName != null && deathData.AudioDataName.Length > 0)
		{
			foreach (string audioDataName in deathData.AudioDataName)
            {
                GetComponent<AudioController>()._PlayAudio(audioDataName);
            }
		}

        if(deathData.UseDieReplacement)
        {
            if(deathData.ReplaceAfterAnimationFinish)
            {
                animation.CrossFade(deathData.AnimationName);
                yield return new WaitForSeconds(animation[deathData.AnimationName].length);
            }
			else if(deathData.ReplaceAfterSeconds > 0)
			{
				animation.CrossFade(deathData.AnimationName);
				yield return new WaitForSeconds(deathData.ReplaceAfterSeconds);
			}
            GameObject DieReplacement = (GameObject)Object.Instantiate(deathData.DieReplacement, transform.position, transform.rotation);
            if(deathData.CopyChildrenTransformToDieReplacement)
            {
                Util.CopyTransform(transform, DieReplacement.transform);
            }
			//if deathData.ReplaceOldObjectInSpawnedList is true, means this object has a replacement in SpawnedList.
			if(deathData.ReplaceOldObjectInSpawnedList)
			{
				// the Spawner must not be null, when ReplaceOldObjectInSpawnedList is true
				if(unit.Spawner != null)
				{
				   unit.Spawner.ReplaceSpawnedWithNewObject(this.gameObject, DieReplacement);
				}
				else 
				{
					Debug.LogError(string.Format("Unit:{0} hsa no Spawner", unit.gameObject.name));
				}
			}
            Destroy(gameObject);
        }
        else 
        {
            animation.Play(deathData.AnimationName);
			if(deathData.DestoryGameObject)
			{
				Destroy(gameObject, deathData.DestoryLagTime);
			}
        }
        
    }
#endregion

}
