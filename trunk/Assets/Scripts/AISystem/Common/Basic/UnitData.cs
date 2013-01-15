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
    public bool SmoothRotate = true;
    public float RotateAngularSpeed = 30;
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
    /// <summary>
    /// DecalDataName - the decal object will be created when receiving damage
    /// </summary>
    public string[] DecalDataName = new string[] { };
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
    /// <summary>
    /// DecalDataName - the decal object will be created when die
    /// </summary>
    public string[] DecalDataName = new string[] { };

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
    #region use global effect object
    public bool UseGlobalEffect = true;
    public GlobalEffectType GlobalType = GlobalEffectType.HumanBlood_Splatter;
    #endregion

    #region use custom effect object
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
    #endregion
}


/// <summary>
/// The custom decal data - decal data will be created when receiving damage.
/// </summary>
[System.Serializable]
public class DecalData
{
    public string Name = string.Empty;

#region use global decal object
    public bool UseGlobalDecal = true;
    public GlobalDecalType GlobalType = GlobalDecalType.HumanBlood_Splatter01_Static;
#endregion

#region use custom decal object
    /// <summary>
    /// An array here. Randomly select one when creating.
    /// </summary>
    public Object[] DecalObjects = new Object[]{};
    /// <summary>
    /// ProjectDirection:
    /// Vertical = Decal create on ground.
    /// Horizontal = Decal create on wall.
    /// </summary>
    public HorizontalOrVertical ProjectDirection = HorizontalOrVertical.Vertical;
    /// <summary>
    /// if DestoryInTimeOut = true, the EffectObject will be destory in %DestoryTimeOut% seconds.
    /// </summary>
    public bool DestoryInTimeOut = true;
    public float DestoryTimeOut = 30;
    public float ScaleRate = 1;
    public LayerMask ApplicableLayer;
#endregion
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
    public DamageForm DamageForm = DamageForm.Common;
	/// <summary>
	/// WeaponType - WeaponType + Target's armor type = audio to play
	/// </summary>
	public WeaponType WeaponType = WeaponType.Default;
	
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
[System.Serializable]
public class AudioData
{
    public string Name;
    public AudioClip audioClip;
}
