using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	/// <summary>
	/// The jump forward speed.
	/// </summary>
	public float JumpForwardSpeed = 10;
	/// <summary>
	/// The jump forward time.
	/// </summary>
	public float JumpForwardTime = 0.2f;
	/// <summary>
	/// The jump over speed.
	/// </summary>
	public float JumpOverSpeed = 30;
	/// <summary>
	/// The jump over check distance.
	/// </summary>
	public float JumpOverCheckDistance = 4;
    /// <summary>
    /// The obstacle layer to let predator jump over.
    /// </summary>
	public LayerMask ObstacleToJumpOver;
	/// <summary>
	/// The jump interval. The next jump must be at least %JumpInterval% seconds after the last jump.
	/// </summary>
	public float ResetJumpInterval = 1;
}

/// <summary>
/// Predator player unit.
/// IdleData - defines the idle animation data of the predator.
/// MoveData - defines the move animation-speed data of the predator.
/// JumpData - defines the jump animation-speed data of the predator.
/// PredatorAttackData - defines the Attack data of the predator.
/// PredatorCombatData - defines the PredatorCombatData of the predator.
/// </summary>
[RequireComponent(typeof(JoyButtonManager))]
[RequireComponent(typeof(Predator3rdPersonVisualEffectController))]
[RequireComponent(typeof(Predator3rdPersonAudioController))]
public class Predator3rdPersonalUnit : UnitBase, I_GameEventReceiver
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
	
	public PredatorPlayerAttackData[] PredatorAttackData = new PredatorPlayerAttackData[]{};
	public IDictionary<string, PredatorPlayerAttackData> PredatorAttackDataDict = new Dictionary<string, PredatorPlayerAttackData>();
	
	/// <summary>
	/// if enemy farer than RushRadius, predator will fastly approaching the enemy.
	/// </summary>
	public float RushRadius = 3;


	public float CombatCoolDown = 0.15f;
	
	/// <summary>
	/// All unmatched left claw combat performs by DefaultCombat_LeftClaw
	/// </summary>
    public PredatorCombatData DefaultCombat_LeftClaw = new PredatorCombatData();
	
    /// <summary>
	/// All unmatched right claw combat performs by DefaultCombat_RightClaw
	/// </summary>
    public PredatorCombatData DefaultCombat_RightClaw = new PredatorCombatData();
	
    /// <summary>
	/// All unmatched dual claw combat performs by DeffaultCombat_DualClaw
	/// </summary>
	public PredatorCombatData DefaultCombat_DualClaw = new PredatorCombatData();

	public PredatorComboCombat[] ComboCombat = new PredatorComboCombat[]{};
	[HideInInspector]
	public string[] AttackAnimations = new string[]{};
	
	#endregion
	
	/// <summary>
	/// Defines the effect data of the unit.
	/// </summary>
	public PlayerEffectData[] EffectData = new PlayerEffectData[] { };
	public IDictionary<string,PlayerEffectData> PlayerEffectDataDict = new Dictionary<string,PlayerEffectData>();
	
	/// <summary>
	/// Defines the audio data of the predator unit.
	/// </summary>
    public AudioData[] AudioData = new AudioData[]{};
	public IDictionary<string,AudioData> AudioDataDict = new Dictionary<string,AudioData>();
	
	/// <summary>
	/// CombatHintHUD - the hint to be displayed on right-top screen. Offer a visual tips to player
	/// the combat they have performed.
	/// </summary>
	public GameObject HUDObject;

	
	void Awake ()
	{
		HP = MaxHP;
        //Initalize move animation:
        animation[MoveData.AnimationName].layer = MoveData.AnimationLayer;

        //Initalize jump animation:
        animation[JumpData.JumpingAnimation].layer = JumpData.AnimationLayer;
        animation[JumpData.PreJumpAnimation].layer = JumpData.AnimationLayer;
        //animation[JumpData.GroundingAnimation].layer = JumpData.AnimationLayer;		
        foreach (PredatorComboCombat comboCombat in this.ComboCombat)
		{
            comboCombat.Init();
        }
		
		foreach(PlayerEffectData effectData in EffectData)
		{
			PlayerEffectDataDict.Add(effectData.Name, effectData);
		}
		
		foreach(AudioData audioData in AudioData)
		{
			AudioDataDict.Add(audioData.Name, audioData);
		}
		
		
		animation[this.IdleData.AnimationName].wrapMode = this.IdleData.AnimationWrapMode;
		animation[this.IdleData.AnimationName].speed = this.IdleData.AnimationSpeed;
		animation[this.IdleData.AnimationName].layer = this.IdleData.AnimationLayer;
		
		
		foreach(PredatorPlayerAttackData p in PredatorAttackData)
		{
			PredatorAttackDataDict.Add(p.Name, p);
			this.AttackAnimations = Util.AddToArray<string>(p.AnimationName,this.AttackAnimations);
			animation[p.AnimationName].wrapMode = p.AnimationWrapMode;
			animation[p.AnimationName].speed = p.AnimationSpeed;
			animation[p.AnimationName].layer = p.AnimationLayer;
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
        Gizmos.DrawWireSphere(transform.position, RushRadius);
    }
	
	public void OnGameEvent(GameEvent gameEvent)
	{
		switch(gameEvent.type)
		{
		case GameEventType.PlayerControlOff:
			SetPlayerControlOnOff(false);
			break;
		case GameEventType.PlayerControlOn:
			SetPlayerControlOnOff(true);
			break;
		}
	}
	
	/// <summary>
	/// Set players the control on or off.
	/// </summary>
	void SetPlayerControlOnOff(bool isOn)
	{
		//Turn on-off JoyButton & JoyButtonManager
		foreach(JoyButton button in this.transform.root.GetComponentsInChildren<JoyButton>())
		{
			button.enabled = isOn;
		}
		this.transform.root.GetComponentInChildren<JoyButtonManager>().enabled = isOn;
		//Turn on-off HUD
		foreach(HUD _HUD in this.transform.root.GetComponentsInChildren<HUD>())
		{
			_HUD.enabled = isOn;
		}
	}
}
