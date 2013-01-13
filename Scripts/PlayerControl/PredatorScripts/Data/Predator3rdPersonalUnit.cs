using UnityEngine;
using System.Collections;

/// <summary>
/// Predator player jump data wrapper.
/// </summary>
[System.Serializable]
public class PredatorPlayerJumpData
{
	public string Name = "Jump";
	public string PreJumpAnimation = "";
	public string JumpingAnimation = "";
	public string GroundingAnimation = "";
	public int AnimationLayer = 3;
	public float JumpForwardSpeed = 10;
	public float JumpForwardTime = 0.2f;
	public float JumpOverSpeed = 30;
	public float JumpOverCheckDistance = 4;
	public LayerMask ObstacleToJumpOver;
}

/// <summary>
/// Predator player effect data.
/// When applying a damage form, the effect object will be created.
/// </summary>
[System.Serializable]
public class PlayerEffectData : EffectData
{
	public DamageForm DamageForm = DamageForm.Common;
	/// <summary>
	/// If PlayParticle = true, then the particlesystem must not be null.
	/// When receiving damage form attack, the particlesystem will play.
	/// </summary>
	public bool PlayParticle = false;
	public ParticleSystem particlesystem = null;
}

/// <summary>
/// Predator player unit.
/// Don't use AttackData, use ComboCombat instead.
/// </summary>
public class Predator3rdPersonalUnit : UnitBase
{
	
	#region variables for Idle
	public IdleData IdleData = new IdleData();
	#endregion
	
	#region variables for Move
	public MoveData MoveData = new MoveData();
    #endregion
	
	#region variables for Jump
	public PredatorPlayerJumpData JumpData = new PredatorPlayerJumpData();
    #endregion
	
	#region variables for Attack
	/// <summary>
	/// The attack radius.
	/// </summary>
	public float AttackRadius = 3;
	public float CombatCooldown = 0.15f;
	public ComboCombat[] ComboCombat = new ComboCombat[]{};
	public string[] AttackAnimations = new string[]{};
	public int AttackAnimationLayer = 3;
	public string[] LeftClawAttackDataName = new string[] { };
	public string[] RightClawAttackDataName = new string[] { };
	
	#endregion
	
	/// <summary>
	/// Defines the effect data of the unit.
	/// </summary>
	public PlayerEffectData[] EffectData = new PlayerEffectData[] { };
	
	void Awake ()
	{
		HP = MaxHP;
	}
	
    #region implement UnitHealth abstract
	public override void SetCurrentHP (float value)
	{
		HP = value;
	}

	public override void SetMaxHP (float value)
	{
		MaxHP = value;
	}

	public override float GetCurrentHP ()
	{
		return HP;
	}

	public override float GetMaxHP ()
	{
		return MaxHP;
	}
    #endregion
}
