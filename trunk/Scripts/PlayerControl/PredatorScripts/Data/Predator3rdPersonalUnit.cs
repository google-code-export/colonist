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
    /// <summary>
    /// The obstacle layer to let predator jump over.
    /// </summary>
	public LayerMask ObstacleToJumpOver;
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

    /// <summary>
    /// When enemy within the radius, predator can move quickly to approach the enemy.
    /// </summary>
    public float OffenseRadius = 6;

	public float CombatCoolDown = 0.15f;

    public Combat DefaultCombat_Tap = new Combat();

    public Combat DefaultCombat_Slice = new Combat();

	public ComboCombat[] ComboCombat = new ComboCombat[]{};
	public string[] AttackAnimations = new string[]{};
	public int AttackAnimationLayer = 3;
	#endregion
	
	/// <summary>
	/// Defines the effect data of the unit.
	/// </summary>
	public PlayerEffectData[] EffectData = new PlayerEffectData[] { };
	
	void Awake ()
	{
		HP = MaxHP;
        //Initalize move animation:
        animation[MoveData.AnimationName].layer = MoveData.AnimationLayer;

        //Initalize jump animation:
        animation[JumpData.JumpingAnimation].layer = JumpData.AnimationLayer;
        animation[JumpData.PreJumpAnimation].layer = JumpData.AnimationLayer;
        //animation[JumpData.GroundingAnimation].layer = JumpData.AnimationLayer;

        //Initalize attack animation:
        foreach (string attackAnimation in AttackAnimations)
        {
            animation[attackAnimation].layer = AttackAnimationLayer;
        }
        foreach (ComboCombat comboCombat in this.ComboCombat)
        {
            comboCombat.Init();
        }
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, AttackRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, OffenseRadius);
    }
}
