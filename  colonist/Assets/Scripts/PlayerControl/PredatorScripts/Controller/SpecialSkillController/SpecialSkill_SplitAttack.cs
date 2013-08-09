using UnityEngine;
using System.Collections;

/// <summary>
/// Special attack skill - split attack.
/// Spike and split the enemies into half.
/// </summary>
public class SpecialSkill_SplitAttack : PredatorPlayerSpecialSkillController {
	
	public float RushDistance = 5;
	public float AttackableDistance = 1;
	public Transform Pivot = null;
	public float RaggRequired = 20;
	
	PredatorPlayerStatus playerStatus = null;
	Predator3rdPersonalAttackController attackController = null;
	GameObject CurrentSplitTarget = null;
	Predator3rdPersonalUnit predatorPlayerUnit = null;
	bool IsCurrentlyDoingSpecialAttack = false;
		
	// Use this for initialization
	void Awake () {
	    attackController = GetComponent<Predator3rdPersonalAttackController>();
		predatorPlayerUnit = GetComponent<Predator3rdPersonalUnit>();
		playerStatus = GetComponent<PredatorPlayerStatus>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.S))
		{
			SendMessage("DoSpecialAttack");
		}
	}
	
	
	/// <summary>
	/// Determines whether this instance can do special attack in the moment.
	/// </summary>
	public override bool CanDoSpecialAttackInTheMoment()
	{
		if(predatorPlayerUnit!= null)
		{
		  bool CanDo = predatorPlayerUnit.Rage  >= RaggRequired;
	      return CanDo;
		}
		else 
		{
			return false;
		}
	}
	
	public override IEnumerator DoSpecialAttack()
	{
		//Un-repeatable
		if(IsCurrentlyDoingSpecialAttack)
		{
			yield break;
		}
		//find target
		float DistanceToEnemy = 0;
		CurrentSplitTarget = attackController.FindTarget (transform.forward, RushDistance, out DistanceToEnemy);
		//if target exists, play special attack animation
		if(CurrentSplitTarget != null)
		{
			IsCurrentlyDoingSpecialAttack = true;
			if (DistanceToEnemy > AttackableDistance) {
			  yield return StartCoroutine(attackController.RushTo (CurrentSplitTarget.transform,0.3f));
			}
			animation.CrossFade("attack_dual_spike_and_split");
			yield return new WaitForSeconds(animation["attack_dual_spike_and_split"].length * 0.9f);
			IsCurrentlyDoingSpecialAttack = false;
			//Minus rage:
			this.predatorPlayerUnit.Rage -= this.RaggRequired;
		}
	}
	
	/// <summary>
	/// this method is called in animation event.
	/// 1. Replace the currentSplitTarget to the splitMiddle model.
	/// 2. Attach the target's physical anchor to the pivot of this gameobjecy
	/// </summary>
	void _KillVictimAndReplacePhysicalUnit()
	{
		if(CurrentSplitTarget != null)
		{
			//if the target is applicable to the special attack type, go ahead.
			if(CurrentSplitTarget.GetComponent<Unit>().IsSpeecialAttackApplicable(this.SpecialAttackType) == true)
			{
		       //1. replace the enemy object to the split ragdoll
		       CurrentSplitTarget = CurrentSplitTarget.GetComponent<Unit>().ReplaceToObject(ReplaceObjectData.ReplaceObjectType.ReplaceToVerticalSplittedBodyObject);
		       //2. attach the split ragdoll to the pivot
		       CurrentSplitTarget.GetComponent<PhysicalUnit>().PhysicalAttachChestToTransform(this.Pivot, this.gameObject);
			}
			//else if the target is not applicable to the special attack type, use normalAttack instead. 
			else 
			{
				StartCoroutine(DoNormalAttack());
			}
		}
	}
	
	public override IEnumerator DoNormalAttack ()
	{
		PredatorPlayerAttackData attackData = predatorPlayerUnit.PredatorAttackDataDict[this.NormalAttackDataName];
	    StartCoroutine(attackController.SendHitMessage(attackData,CurrentSplitTarget));
		yield return null;
	}
	
	/// <summary>
	/// Ask the target to perform the split effect.
	/// </summary>
    void _SendSplitEvent()
	{
		if(CurrentSplitTarget != null && CurrentSplitTarget.GetComponent<Ragdoll>() != null && CurrentSplitTarget.GetComponent<PhysicalUnit>() != null)
		{
		   CurrentSplitTarget.SendMessage("StartRagdoll");
		   CurrentSplitTarget.GetComponent<PhysicalUnit>().PhysicalDetachChest();
		}
	}
	
}
