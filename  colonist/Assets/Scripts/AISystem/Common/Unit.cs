using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The class represents a basic Unit in the game.
/// Unit responsibility:
/// 1. Contains data definition.
/// 2. In the case of more than one AI component, switch AI in real time, according to condition data.
/// </summary>
[System.Serializable]
public class Unit : UnitBase , I_GameEventReceiver
{

    [HideInInspector]
    public bool IsDead = false;
#region Unit Data definition
    /// <summary>
    /// Defines the attack data of the unit.
    /// </summary>
    public AttackData[] AttackData = new AttackData[] { };
    /// <summary>
    /// Dictionary - Key = Name, Value = AttackData
    /// </summary>
    public System.Collections.Generic.IDictionary<string, AttackData> AttackDataDict = new System.Collections.Generic.Dictionary<string, AttackData>();
    /// <summary>
    /// Defines the movement data of the unit.
    /// </summary>
    public MoveData[] MoveData = new MoveData[] { };
    public System.Collections.Generic.IDictionary<string, MoveData> MoveDataDict = new System.Collections.Generic.Dictionary<string, MoveData>();
    /// <summary>
    /// Defines the idle data of the unit.
    /// </summary>
    public IdleData[] IdleData = new IdleData[] { };
    public System.Collections.Generic.IDictionary<string, IdleData> IdleDataDict = new System.Collections.Generic.Dictionary<string, IdleData>();
    /// <summary>
    /// Defines the effect data of the unit.
    /// </summary>
    public EffectData[] EffectData = new EffectData[] { };
    public System.Collections.Generic.IDictionary<string, EffectData> EffectDataDict = new System.Collections.Generic.Dictionary<string, EffectData>();
	
	/// <summary>
	/// Defines the rotate data of the unit
	/// </summary>
    public RotateData[] RotateData = new RotateData[] { };
	public System.Collections.Generic.IDictionary<string, RotateData> RotateDataDict = new System.Collections.Generic.Dictionary<string, RotateData>();
	
    /// <summary>
    /// There must be at least one ReceiveDamageData with DamageForm = Common, as the default receive damage data.
    /// </summary>
    public ReceiveDamageData[] ReceiveDamageData = new ReceiveDamageData[] { };
    /// <summary>
    /// ReceiveDamageDataDict.Key = DamageForm; Value = List of ReceiveDamageData.
    /// So, when applying a damage, lookup the dictionary by DamageForm, and use a random receive damage data from the list.
    /// </summary>
    public System.Collections.Generic.IDictionary<DamageForm, System.Collections.Generic.IList<ReceiveDamageData>> ReceiveDamageDataDict = new System.Collections.Generic.Dictionary<DamageForm, System.Collections.Generic.IList<ReceiveDamageData>>();

    /// <summary>
    /// There must be at least one DieData with DamageForm = Common, as the default die data.
    /// </summary>
    public DeathData[] DeathData = new DeathData[] { };
    /// <summary>
    /// DieData.Key = DamageForm; Value = List of DieData.
    /// So, when AI die, lookup the dictionary by DamageForm, and use a random receive die data from the list.
    /// </summary>
    public System.Collections.Generic.IDictionary<DamageForm, System.Collections.Generic.IList<DeathData>>
        DeathDataDict = new System.Collections.Generic.Dictionary<DamageForm, System.Collections.Generic.IList<DeathData>>();

    public DecalData[] DecalData = new DecalData[] { };
    public System.Collections.Generic.IDictionary<string, DecalData> DecalDataDict = new System.Collections.Generic.Dictionary<string, DecalData>();

    public AudioData[] AudioData = new AudioData[] { };
	
	public IDictionary<string,AI> AIDict = new Dictionary<string,AI>();
	
	/// <summary>
	/// The attack counter indicates how many times the Unit has attacked.
	/// Note: not necessary for every attack to do actual damage.
	/// </summary>
	[HideInInspector]
	public int AttackCounter = 0;
	/// <summary>
	/// The do damage counter indicates how many time the Unit has do damage to other units.
	/// </summary>
	[HideInInspector]
	public int DoDamageCounter = 0;
	
	/// <summary>
	/// The halt flat.
	/// If halt = true, the AI component should Halt its behavior according to this flag.
	/// </summary>
	[HideInInspector]
	public bool Halt = false;
	float ResetHaltTime = 0;
#endregion

#region Unit AI definition
	
	/// <summary>
	/// The name of the start AI.
	/// If StartAIName = "", it will be assigned to the first AI component's name by default.
	/// </summary>
	public string StartAIName = "";
	
	/// <summary>
	/// The current running AI.
	/// </summary>
	[HideInInspector]
	public AI CurrentAI = null;
	
	/// <summary>
	/// Indicate if the unit has been initialized.
	/// </summary>
	[HideInInspector]
	public bool UnitInitDone = false;
	
#endregion

	CharacterController controller = null;
	
    void Awake()
    {
        InitUnitData();
		InitAnimation();
        InitUnitAI();
		UnitInitDone = true;
    }
	
	public void Start()
	{
		
	}
	
	public void Update()
	{
        if (Time.time >= ResetHaltTime && Halt == true)
        {
            Halt = false;
        }
	}
	
    /// <summary>
    /// Call InitUnit at Monobehavior.Awake().
    /// Put all kinds of data into dictionary.
    /// </summary>
    public void InitUnitData()
    {
        HP = MaxHP;
        if (AttackData != null)
        {
            foreach (AttackData attackData in AttackData)
            {
                AttackDataDict.Add(attackData.Name, attackData);
            }
        }
        if (MoveData != null)
        {
            foreach (MoveData moveData in MoveData)
            {
                MoveDataDict.Add(moveData.Name, moveData);
            }
        }
        if (IdleData != null)
        {
            foreach (IdleData idleData in IdleData)
            {
                IdleDataDict.Add(idleData.Name, idleData);
            }
        }
        if (EffectData != null)
        {
            foreach (EffectData effectData in EffectData)
            {
                EffectDataDict.Add(effectData.Name, effectData);
            }
        }
		if(RotateData != null)
		{
            foreach (RotateData rotateData in RotateData)
            {
                RotateDataDict.Add(rotateData.Name, rotateData);
            }
		}
        if (ReceiveDamageData != null)
        {
            foreach (ReceiveDamageData receiveDamageData in ReceiveDamageData)
            {
                if (ReceiveDamageDataDict.ContainsKey(receiveDamageData.DamageForm) == false)
                {
                    System.Collections.Generic.IList<ReceiveDamageData> L = new System.Collections.Generic.List<ReceiveDamageData>();
                    L.Add(receiveDamageData);
                    ReceiveDamageDataDict[receiveDamageData.DamageForm] = L;
                }
                else
                {
                    ReceiveDamageDataDict[receiveDamageData.DamageForm].Add(receiveDamageData);
                }
            }
        }

        if (DeathData != null)
        {
            foreach (DeathData dieData in DeathData)
            {
                if (DeathDataDict.ContainsKey(dieData.DamageForm) == false)
                {
                    System.Collections.Generic.IList<DeathData> L = new System.Collections.Generic.List<DeathData>();
                    L.Add(dieData);
                    DeathDataDict[dieData.DamageForm] = L;
                }
                else
                {
                    DeathDataDict[dieData.DamageForm].Add(dieData);
                }
            }
        }

        if (DecalData != null)
        {
            foreach (DecalData decal in DecalData)
            {
                DecalDataDict.Add(decal.Name, decal);
            }
        }
    }
	
    public void InitAnimation()
	{
        //Initialize the animation data:
        foreach (UnitAnimationData data in IdleData)
        {
            animation[data.AnimationName].layer = data.AnimationLayer;
            animation[data.AnimationName].wrapMode = data.AnimationWrapMode;
            animation[data.AnimationName].speed = data.AnimationSpeed;
        }
        foreach (UnitAnimationData data in MoveData)
        {
            animation[data.AnimationName].layer = data.AnimationLayer;
            animation[data.AnimationName].wrapMode = data.AnimationWrapMode;
            animation[data.AnimationName].speed = data.AnimationSpeed;
        }
        foreach (UnitAnimationData data in AttackData)
        {
            animation[data.AnimationName].layer = data.AnimationLayer;
            animation[data.AnimationName].wrapMode = data.AnimationWrapMode;
            animation[data.AnimationName].speed = data.AnimationSpeed;
        }
		foreach(RotateData data in RotateData)
		{
			if(animation[data.RotateLeftAnimationName] != null)
			{
              animation[data.RotateLeftAnimationName].layer = data.AnimationLayer;
              animation[data.RotateLeftAnimationName].wrapMode = data.AnimationWrapMode;
              animation[data.RotateLeftAnimationName].speed = data.AnimationSpeed;
			}
			if(animation[data.RotateRightAnimationName] != null)
			{
			  animation[data.RotateRightAnimationName].layer = data.AnimationLayer;
              animation[data.RotateRightAnimationName].wrapMode = data.AnimationWrapMode;
              animation[data.RotateRightAnimationName].speed = data.AnimationSpeed;
			}
		}
		foreach(ReceiveDamageData data in ReceiveDamageData)
		{
            animation[data.AnimationName].layer = data.AnimationLayer;
            animation[data.AnimationName].wrapMode = data.AnimationWrapMode;
            animation[data.AnimationName].speed = data.AnimationSpeed;
		}
	}
	
	/// <summary>
	/// Inits the unit's AI related variables.
	/// </summary>
	public void InitUnitAI()
	{
		if(StartAIName == "")
		{
			StartAIName = GetComponent<AI>().Name;
		}
		foreach(AI _AI in GetComponents<AI>())
		{
			AIDict.Add(_AI.Name, _AI);
			if(_AI.Name == StartAIName)
			{
				_AI.enabled = true;
			}
			else {
				_AI.enabled = false;
			}
		}
		controller = GetComponent<CharacterController>();
		LevelManager.RegisterUnit(this);
	}
	
#region implement UnitHealth interface
    public override void SetCurrentHP(float value)
    {
        HP = value;
    }
    public override void SetMaxHP(float value)
    {
        MaxHP = value;
    }
    public override float GetCurrentHP()
    {
        return HP;
    }
    public override float GetMaxHP()
    {
        return MaxHP;
    }
    #endregion
	
		
#region Event listener
	public virtual IEnumerator OnGameEvent(GameEvent _event)
	{
		switch(_event.type)
		{
		    case GameEventType.ApplyDamage:
			    DamageParameter dp = (DamageParameter)_event.parameters[GameEventParameter.DamageParameter];
			    SendMessage("ApplyDamage", dp);
			    break;
		    default:
			    break;
		}
		yield break;
	}
	
	public void ResetAttackCounter()
	{
		this.AttackCounter = 0;
		this.DoDamageCounter = 0;
	}
	
	/// <summary>
	/// Sets the halt to true in next %duration% seconds
	/// </summary>
	public void HaltUnit(float duration)
	{
		Halt = true;
		ResetHaltTime = Time.time + duration;
	}
#endregion
	

}
