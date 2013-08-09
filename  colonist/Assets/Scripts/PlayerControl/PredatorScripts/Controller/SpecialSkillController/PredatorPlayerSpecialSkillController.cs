using UnityEngine;
using System.Collections;

public enum PredatorPlayerSpecialAttackType
{
	SplitFromMiddleAttack = 0,
	BiteAndRushAttack = 1,
}

public abstract class PredatorPlayerSpecialSkillController : MonoBehaviour {
	
	public PredatorPlayerSpecialAttackType SpecialAttackType = PredatorPlayerSpecialAttackType.SplitFromMiddleAttack;
	
	/// <summary>
	/// When the target is attackable,  but not applicable to the special attack, the normal attack data will be used.
	/// </summary>
	public string NormalAttackDataName = "";	

	/// <summary>
	/// Implement this method to perform special attack in derivate class.
	/// </summary>
	public abstract IEnumerator DoSpecialAttack();
	/// <summary>
	/// Implement this method to perform normal attack in derivate class. 
	/// If the target unit it not applicable to the special attack type, DoNormalAttack() method is called.
	/// </summary>
	public abstract IEnumerator DoNormalAttack();
	
	/// <summary>
	/// Determines whether this instance can do special attack in the moment.
	/// </summary>
	public abstract bool CanDoSpecialAttackInTheMoment();
}
