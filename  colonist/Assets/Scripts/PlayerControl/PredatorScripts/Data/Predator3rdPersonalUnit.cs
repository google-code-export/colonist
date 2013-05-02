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
}

/// <summary>
/// Predator player unit.
/// Don't use AttackData, use ComboCombat instead.
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
	/// <summary>
	/// The attack radius.
	/// </summary>
	public float AttackRadius = 3;

    /// <summary>
    /// When enemy within the radius, predator can move quickly to approach the enemy.
    /// </summary>
    public float OffenseRadius = 6;

	public float CombatCoolDown = 0.15f;
	
	/// <summary>
	/// All unmatched left claw combat performs by DefaultCombat_LeftClaw
	/// </summary>
    public Combat DefaultCombat_LeftClaw = new Combat();
	
    /// <summary>
	/// All unmatched right claw combat performs by DefaultCombat_RightClaw
	/// </summary>
    public Combat DefaultCombat_RightClaw = new Combat();
	
    /// <summary>
	/// All unmatched dual claw combat performs by DeffaultCombat_DualClaw
	/// </summary>
	public Combat DefaultCombat_DualClaw = new Combat();

	public ComboCombat[] ComboCombat = new ComboCombat[]{};
	public string[] AttackAnimations = new string[]{};
	public int AttackAnimationLayer = 3;
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

        //Initalize attack animation:
        foreach (string attackAnimation in AttackAnimations)
        {
            animation[attackAnimation].layer = AttackAnimationLayer;
        }
        foreach (ComboCombat comboCombat in this.ComboCombat)
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
