using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIApplyDamage : MonoBehaviour, I_ReceiveDamage {

	Unit unit;
	CharacterController controller;
	void Awake()
	{
		unit = GetComponent<Unit>();
		controller = GetComponent<CharacterController>();
	}
	
	void Update()
	{
		if(Input.GetKeyDown("t"))
		{
			DamageParameter dp = new DamageParameter(this.gameObject, DamageForm.Predator_Strike_Single_Claw, 3);
			SendMessage("ApplyDamage",dp);
		}
	}
	
#region Receive Damage and Die
    public virtual IEnumerator ApplyDamage(DamageParameter damageParam)
    {
        if (unit.IsDead)
        {
            yield break;
        }
        //Minus HP:
        unit.HP -= damageParam.damagePoint;
        if (unit.HP <= 0)
        {
            StartCoroutine("Die", damageParam);
            yield break;
        }
        else
        {
            StartCoroutine("DoDamage", damageParam);
            yield break;
        }
        
    }

    public virtual IEnumerator DoDamage(DamageParameter damageParam)
    {
        //Get the right ReceiveDamageData
        ReceiveDamageData ReceiveDamageData = null;
        if (unit.ReceiveDamageDataDict.ContainsKey(damageParam.damageForm))
        {
            if (unit.ReceiveDamageDataDict[damageParam.damageForm].Count == 1)
            {
                ReceiveDamageData = unit.ReceiveDamageDataDict[damageParam.damageForm][0];
            }
            else
            {
                int RandomIndex = Random.Range(0, unit.ReceiveDamageDataDict[damageParam.damageForm].Count);
                ReceiveDamageData = unit.ReceiveDamageDataDict[damageParam.damageForm][RandomIndex];
            }
        }
        //If ReceiveDamageDataDict[DamageForm.Common] = null or Count ==0, will have error!
        //So make sure you have assign a Common receive damage data!
        else
        {
            ReceiveDamageData = unit.ReceiveDamageDataDict[DamageForm.Common][0];
        }
        //Create effect data
        if (ReceiveDamageData.EffectDataName != null && ReceiveDamageData.EffectDataName.Length > 0)
        {
            foreach (string effectDataName in ReceiveDamageData.EffectDataName)
            {
                EffectData EffectData = unit.EffectDataDict[effectDataName];
                GlobalBloodEffectDecalSystem.CreateEffect(transform.position + controller.center, EffectData);
            }
        }
        //Create blood decal:
        if (ReceiveDamageData.DecalDataName != null && ReceiveDamageData.DecalDataName.Length > 0)
        {
            foreach (string decalName in ReceiveDamageData.DecalDataName)
            {
                DecalData DecalData = unit.DecalDataDict[decalName];
                GlobalBloodEffectDecalSystem.CreateBloodDecal(transform.position + controller.center, DecalData);
            }
        }

        //Halt AI if set true
        if (ReceiveDamageData.HaltAI)
        {
            animation.Rewind(ReceiveDamageData.AnimationName);
            animation.Play(ReceiveDamageData.AnimationName);
			SendMessage("HaltUnit", animation[ReceiveDamageData.AnimationName].length);
            yield return new WaitForSeconds(animation[ReceiveDamageData.AnimationName].length);
        }
    }

    public virtual IEnumerator Die(DamageParameter DamageParameter)
    {
		LevelManager.UnregisterUnit(unit);
		
        //Basic death processing.
        controller.enabled = false;
        unit.IsDead = true;
        foreach(AI _ai in GetComponents<AI>())
		{
			_ai.StopAI();
		}
        animation.Stop();

        //Handle DeathData:
        DeathData DeathData = null;
        if(unit.DeathDataDict.ContainsKey(DamageParameter.damageForm))
        {
            IList<DeathData> DeathDataList = unit.DeathDataDict[DamageParameter.damageForm];
            DeathData = Util.RandomFromList(DeathDataList);
        }
        else 
        {
            DeathData = Util.RandomFromList<DeathData>( unit.DeathDataDict[DamageForm.Common]);
        }

        //Create effect data 
        if (DeathData.EffectDataName != null && DeathData.EffectDataName.Length > 0)
        {
            foreach (string effectDataName in DeathData.EffectDataName)
            {
                EffectData EffectData = unit.EffectDataDict[effectDataName];
                GlobalBloodEffectDecalSystem.CreateEffect(transform.position + controller.center, EffectData);
            }
        }
        //Create blood decal:
        if (DeathData.DecalDataName != null && DeathData.DecalDataName.Length > 0)
        {
            foreach (string decalName in DeathData.DecalDataName)
            {
                DecalData DecalData = unit.DecalDataDict[decalName];
                GlobalBloodEffectDecalSystem.CreateBloodDecal(transform.position + controller.center, DecalData);
            }
        }


        if(DeathData.UseDieReplacement)
        {
            if(DeathData.ReplaceAfterAnimationFinish)
            {
                animation.CrossFade(DeathData.AnimationName);
                yield return new WaitForSeconds(animation[DeathData.AnimationName].length);
            }
            GameObject DieReplacement = (GameObject)Object.Instantiate(DeathData.DieReplacement, transform.position, transform.rotation);
            if(DeathData.CopyChildrenTransformToDieReplacement)
            {
                Util.CopyTransform(transform, DieReplacement.transform);
            }
            Destroy(gameObject);
        }
        else 
        {
            animation.CrossFade(DeathData.AnimationName);
        }
        
    }

    public void CreateDecal()
    {
        //Locate the place to create decal:

    }
	
#endregion
}
