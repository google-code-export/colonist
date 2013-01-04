using UnityEngine;
using System.Collections;

/// <summary>
/// Wrap the base data for a unitdata
/// </summary>
public class UnitAnimationData
{
    public string Name = string.Empty;
    public int AnimationLayer = 0;
    public string AnimationName = string.Empty;
    public WrapMode AnimationWrapMode = WrapMode.Default;
    public float AnimationSpeed = 1;
}

/// <summary>
/// The class wrap move data.
/// </summary>
[System.Serializable]
public class MoveData : UnitAnimationData
{
    public float MoveSpeed = 1;
    /// <summary>
    /// CanRotate + SmoothRotate + RotateAngularSpeed
    /// Defines, when unit moving, should the unit rotate face to move direction?
    /// CanRotate = true = rotate facing when moving
    /// SmoothRotate = true = rotating is smoothly and the rotate angular speed = RotateAngularSpeed
    /// </summary>
    public bool CanRotate = true;
    public bool SmoothRotate = false;
    public float RotateAngularSpeed = 10;
}

/// <summary>
/// The class wrap rotate data.
/// </summary>
[System.Serializable]
public class RotateData : UnitAnimationData
{
    public float RotateAngularSpeed = 10;
}


/// <summary>
/// The class wrap idle data
/// </summary>
[System.Serializable]
public class IdleData : UnitAnimationData
{

}

/// <summary>
/// The class wrap receive damage data
/// </summary>
[System.Serializable]
public class ReceiveDamageData : UnitAnimationData
{
    /// <summary>
    /// DamageForm of this ReceiveDamageData.
    /// </summary>
    public DamageForm DamageForm = DamageForm.Common;
    /// <summary>
    /// If HaltAI = true, AI will play receive damage animation, and Halt all behavior.
    /// </summary>
    public bool HaltAI;
    /// <summary>
    /// EffectDataName - the effectdata will be created immediately when playing the animation.
    /// </summary>
    public string[] EffectDataName = new string[]{};
}
/// <summary>
/// The class wrap receive unit die data
/// </summary>
[System.Serializable]
public class DeathData : UnitAnimationData
{
    /// <summary>
    /// DamageForm of this ReceiveDamageData.
    /// </summary>
    public DamageForm DamageForm = DamageForm.Common;
    /// <summary>
    /// EffectDataName - the effectdata will be created immediately when playing the animation.
    /// </summary>
    public string[] EffectDataName = new string[] { };

    public bool UseDieReplacement = false;
    /// <summary>
    /// If UseDieReplacement = true, will create the DieReplacement GameObject after animation finished.
    /// Else if UseDieReplacement = false, will create DieReplacement immediately following death, without any animation.
    /// </summary>
    public bool ReplaceAfterAnimationFinish = true;

    /// <summary>
    /// If UseDieRagdoll = true, will destory this gameObject and create the DieReplacement.
    /// </summary>
    public GameObject DieReplacement;
    /// <summary>
    /// If DieReplacement != null and CopyChildrenTransformToDieReplacement = true,
    /// the DieReplacement's children transform will be aligned to gameObject.
    /// </summary>
    public bool CopyChildrenTransformToDieReplacement = false;
}

/// <summary>
/// The class wrap effect data
/// </summary>
[System.Serializable]
public class EffectData
{
    public string Name = string.Empty;
    /// <summary>
    /// Instantiate the EffectObject at the position/rotation of the EffectObject
    /// </summary>
    public Transform Anchor = null;
    public GameObject EffectObject = null;
    /// <summary>
    /// if DestoryInTimeOut = true, the EffectObject will be destory in %DestoryTimeOut% seconds.
    /// </summary>
    public bool DestoryInTimeOut = true;
    public float DestoryTimeOut = 1;
}

/// <summary>
/// The class wrap attack data.
/// </summary>
[System.Serializable]
public class AttackData : UnitAnimationData
{
    /// <summary>
    /// Attack type - the type of attack beheavior
    /// </summary>
    public AIAttackType Type = AIAttackType.Instant;

    /// <summary>
    /// DamageForm - the damage form in sending "ApplyDamage" interface
    /// </summary>
    public DamageForm DamageForm = DamageForm.Collision;

    /// <summary>
    /// The HitTestType. Used only when Type = Instant
    /// </summary>
    public HitTestType HitTestType = HitTestType.AlwaysTrue;

    /// <summary>
    /// Used only when HitTestType = HitRate. A value between 0 and 1.
    /// </summary>
    public float HitRate = 0.5f;

    /// <summary>
    /// When HitTestType = CollisionTest, HitTestCollider MUST NOT BE NULL.
    /// </summary>
    public Collider HitTestCollider;

    /// <summary>
    /// Used when HitTestType = CollisionTest or DistanceTest.
    /// When HitTestType = CollisionTest, HitTestDistance determines the scan enemy range.
    /// When HitTestType = DistanceTest, HitTestDistance determine if the hit message should be sent to CurrentTarget.
    /// </summary>
    public float HitTestDistance = 3;

    /// <summary>
    /// When target distance lesser than AttackableRange, begin attack.
    /// </summary>
    public float AttackableRange;

    /// <summary>
    /// If attack type = combat - The time to send hit message.
    /// If attack type = arrow - The time to create the arrow object
    /// </summary>
    public float HitTime;

    /// <summary>
    /// After last attacking, how long will the next attack take place.
    /// </summary>
    public float AttackInterval = 0.5f;

    /// <summary>
    /// Only used if attackType = Projectile.
    /// The Projectile object.
    /// </summary>
    public Projectile Projectile;
    /// <summary>
    /// Only used if attackType = Projectile.
    /// The Projectile object will be instantiated alighing transform of ProjectileInstancingAnchor
    /// </summary>
    public Transform ProjectileInstantiateAnchor;

    /// <summary>
    /// Base damage point
    /// </summary>
    public float DamagePointBase = 10;

    /// <summary>
    /// Final damage point = DamagePointBase * Random(MinDamageBonus, MaxDamageBonus)
    /// </summary>
    public float MinDamageBonus = 1;
    public float MaxDamageBonus = 2;

    /// <summary>
    /// The script object attach to target when hitting
    /// </summary>
    public MonoBehaviour ScriptObjectAttachToTarget;

    public DamageParameter GetDamageParameter(GameObject DamageSource)
    {
        return new DamageParameter(DamageSource, this.DamageForm, DamagePointBase + Random.Range(MinDamageBonus, MaxDamageBonus));
    }
}



/// <summary>
/// The class represents a basic Unit in the game
/// </summary>
[System.Serializable]
public class Unit
{
    public string Name = "Default Name";
    /// <summary>
    /// What's the layer of the enemy
    /// </summary>
    public LayerMask EnemyLayer;

    /// <summary>
    /// Health point.
    /// </summary>
    public float MaxHP = 100;
 
    public float HP = 100;
    [HideInInspector]
    public bool IsDead = false;

    /// <summary>
    /// Defines the attack data of the unit.
    /// </summary>
    public AttackData[] AttackData = new AttackData[]{};
    /// <summary>
    /// Dictionary - Key = Name, Value = AttackData
    /// </summary>
    public System.Collections.Generic.IDictionary<string, AttackData> AttackDataDict = new System.Collections.Generic.Dictionary<string, AttackData>();
    /// <summary>
    /// Defines the movement data of the unit.
    /// </summary>
    public MoveData[] MoveData = new MoveData[]{};
    public System.Collections.Generic.IDictionary<string, MoveData> MoveDataDict = new System.Collections.Generic.Dictionary<string, MoveData>();
    /// <summary>
    /// Defines the idle data of the unit.
    /// </summary>
    public IdleData[] IdleData = new IdleData[]{};
    public System.Collections.Generic.IDictionary<string, IdleData> IdleDataDict = new System.Collections.Generic.Dictionary<string, IdleData>();
    /// <summary>
    /// Defines the effect data of the unit.
    /// </summary>
    public EffectData[] EffectData = new EffectData[]{};
    public System.Collections.Generic.IDictionary<string, EffectData> EffectDataDict = new System.Collections.Generic.Dictionary<string, EffectData>();
    
    /// <summary>
    /// There must be at least one ReceiveDamageData with DamageForm = Common, as the default receive damage data.
    /// </summary>
    public ReceiveDamageData[] ReceiveDamageData = new ReceiveDamageData[]{};
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
            foreach(ReceiveDamageData receiveDamageData in ReceiveDamageData)
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
    }
}