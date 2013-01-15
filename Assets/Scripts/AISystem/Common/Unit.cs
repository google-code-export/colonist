using UnityEngine;
using System.Collections;

/// <summary>
/// The class represents a basic Unit in the game
/// </summary>
[System.Serializable]
public class Unit : UnitBase
{

    [HideInInspector]
    public bool IsDead = false;

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

    void Awake()
    {
        InitUnit();
    }

    /// <summary>
    /// Call InitUnit at Monobehavior.Awake().
    /// Put the MoveData/IdleData/AttakData in dictionary.
    /// </summary>
    public void InitUnit()
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
                Debug.Log("Adding IdleData:" + idleData.Name);
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
}
