using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class AIApplyDamage : MonoBehaviour, I_ReceiveDamage {

	protected Unit unit;
	protected CharacterController controller;
	
	void Awake()
	{
		unit = GetComponent<Unit>();
		unit.receiveDamageStatus = UnitReceiveDamageStatus.vulnerable;
		controller = GetComponent<CharacterController>();
	}
	
	void Update()
	{
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
        if (unit.HP <= 0)
        {
			SendMessage("Die", damageParam);
            yield break;
        }
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
