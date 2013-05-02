using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIApplyDamage : MonoBehaviour, I_ReceiveDamage {

	Unit unit;
	CharacterController controller;
	
	/// <summary>
	/// if SwitchToAI flag is turned on, the current AI will be set off, another AI will be set on.
	/// </summary>
	public bool SwitchToAI = false;
	public string SwitchToAIName = string.Empty;
	float SwitchToAITime = float.MaxValue;
	
	/// <summary>
	/// When receive damager counter GE random of min and max, switch to AI.
	/// </summary>
	public int SwitchConditionCounter_Min = 1;
	public int SwitchConditionCounter_Max = 2;
	
	void Awake()
	{
		unit = GetComponent<Unit>();
		controller = GetComponent<CharacterController>();
	}
	
	void Update()
	{
//		if(Input.GetKeyDown("t"))
//		{
//			DamageParameter dp = new DamageParameter(this.gameObject, DamageForm.Predator_Strike_Single_Claw, 3);
//			SendMessage("ApplyDamage",dp);
//		}
		
		//Switch to another AI, if the applied receive damage data contains switch to AI definition
		if(SwitchToAIName != string.Empty && Time.time>=SwitchToAITime)
		{
			this.unit.SwitchAI(SwitchToAIName);
			//reset switch AI variables.
			SwitchToAITime = int.MaxValue;
			SwitchToAIName = string.Empty;
		}
	}
	
#region Receive Damage and Die
    public virtual IEnumerator ApplyDamage(DamageParameter damageParam)
    {
        if (unit.IsDead)
        {
            yield break;
        }
		//Dispatch ReceiveDamage GameEvent
		GameEvent e = new GameEvent(GameEventType.DisplayDamageParameterOnNPC);
		e.sender = this.gameObject;
		e.ObjectParameter = damageParam;
		LevelManager.GameEvent(e);
		
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
			this.unit.ReceiveDamageCounter++;
            yield break;
        }
        
    }

    public virtual IEnumerator DoDamage(DamageParameter damageParam)
    {
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
        //Create effect data
        if (receiveDamageData.EffectDataName != null && receiveDamageData.EffectDataName.Length > 0)
        {
            foreach (string effectDataName in receiveDamageData.EffectDataName)
            {
                EffectData EffectData = unit.EffectDataDict[effectDataName];
                GlobalBloodEffectDecalSystem.CreateEffect(transform.position + controller.center, EffectData);
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
		
		if(this.SwitchToAI)
		{
			int counter = Random.Range(SwitchConditionCounter_Min, SwitchConditionCounter_Max);
			this.SwitchToAITime = animation[receiveDamageData.AnimationName].length + Time.time;
			this.SwitchToAIName = this.SwitchToAIName;
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
//		LevelManager.UnregisterUnit(unit);
		
		if(this.unit.spawnIdentity != null)
		{
			LevelArea.GetArea(unit.spawnIdentity.LevelAreaName).OnSpawneeDie (this.gameObject, this.unit.spawnIdentity);
//			Debug.Log("Area:" + unit.spawnIdentity.LevelAreaName + " spawnee die:" + this.gameObject.name + "spawn wave:" + this.unit.spawnIdentity.SpawnWaveName);
		}
		
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
            animation.Play(DeathData.AnimationName);
			if(DeathData.DestoryGameObject)
			{
				yield return new WaitForSeconds(animation[DeathData.AnimationName].length + DeathData.DestoryLagTime);
				Destroy(gameObject);
			}
        }
        
    }

    public void CreateDecal()
    {
        //Locate the place to create decal:

    }
	
#endregion
}
