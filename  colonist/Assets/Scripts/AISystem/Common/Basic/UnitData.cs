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
	public void CloneBasic(UnitAnimationData _unitAnimationData)
	{
		_unitAnimationData.Name = this.Name;
		_unitAnimationData.AnimationLayer = this.AnimationLayer;
		_unitAnimationData.AnimationName = this.AnimationName;
		_unitAnimationData.AnimationWrapMode = this.AnimationWrapMode;
		_unitAnimationData.AnimationSpeed = this.AnimationSpeed;
	}
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
	
	/// <summary>
	/// The redirect target position interval.
	/// This variable is only used in Attack behavior.
	/// </summary>
	public float RedirectTargetInterval = 0.15f;
	
	public MoveData GetClone()
	{
		MoveData clone = new MoveData();
		base.CloneBasic(clone as UnitAnimationData);
		clone.MoveSpeed = this.MoveSpeed;
		clone.CanRotate = this.CanRotate;
		clone.SmoothRotate = this.SmoothRotate;
		clone.RotateAngularSpeed = this.RotateAngularSpeed;
		return clone;
	}
}

/// <summary>
/// The class wrap rotate data.
/// </summary>
[System.Serializable]
public class RotateData
{
	
    public string Name = string.Empty;
    public int AnimationLayer = 0;
    public string RotateLeftAnimationName = string.Empty;
	public string RotateRightAnimationName = string.Empty;
    public WrapMode AnimationWrapMode = WrapMode.Default;
    public float AnimationSpeed = 1;
	
	/// <summary>
	/// The smooth rotate angular speed.
	/// </summary>
    public float RotateAngularSpeed = 10;
	
	/// <summary>
	/// Only rotate to face target when forward direction and face to target direction's angle distance > AngleDistanceToStartRotate
	/// </summary>
	public float AngleDistanceToStartRotate = 3;
}


/// <summary>
/// The class wrap idle data
/// </summary>
[System.Serializable]
public class IdleData : UnitAnimationData
{
	/// <summary>
	/// If KeepFacingTarget = true, character will fact to current target during Idle.
	/// </summary>
	public bool KeepFacingTarget = true;
	/// <summary>
	/// if KeepFacingTarget = true, should the character rotate to facing target smoothly? If true, then the 
	/// RotateDataName MUST be assigned.
	/// </summary>
	public bool SmoothRotate = false;
	public string RotateDataName = "";
	public IdleData GetClone()
	{
		IdleData clone = new IdleData();
		base.CloneBasic(clone);
		clone.KeepFacingTarget = this.KeepFacingTarget;
		clone.SmoothRotate = this.SmoothRotate;
		clone.RotateDataName = this.RotateDataName;
		return clone;
	}
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
	
    public ReceiveDamageData GetClone()
	{
		ReceiveDamageData clone = new ReceiveDamageData();
		base.CloneBasic(clone);
		clone.DamageForm = this.DamageForm;
		clone.HaltAI = this.HaltAI;
	    clone.EffectDataName = Util.CloneArray<string>(this.EffectDataName);
		clone.DecalDataName = Util.CloneArray<string>(this.DecalDataName);
		return clone;
	}
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
	
    public DeathData GetClone()
	{
		DeathData clone = new DeathData();
		base.CloneBasic(clone);
		clone.DamageForm = this.DamageForm;
		clone.UseDieReplacement = this.UseDieReplacement;
		clone.ReplaceAfterAnimationFinish = this.ReplaceAfterAnimationFinish;
		clone.DieReplacement = this.DieReplacement;
		clone.CopyChildrenTransformToDieReplacement = this.CopyChildrenTransformToDieReplacement;
	    clone.EffectDataName = Util.CloneArray<string>(this.EffectDataName);
		clone.DecalDataName = Util.CloneArray<string>(this.DecalDataName);
		return clone;
	}
}

/// <summary>
/// The class wrap effect data
/// </summary>
[System.Serializable]
public class EffectData
{
    public string Name = string.Empty;
	/// <summary>
	/// The count of the effect
	/// </summary>
	public int Count = 1;
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
	
	/// <summary>
	/// The create delay flag.
	/// If CreateDelay is true, then the effect should be created in delay of %CreateDelayTime% seconds.
	/// </summary>
	public bool CreateDelay = true;
	public float CreateDelayTime = 1;
	
	public EffectData GetClone()
	{
		EffectData clone = new EffectData();
		clone.Name = this.Name;
		clone.Count = this.Count;
		clone.UseGlobalEffect = this.UseGlobalEffect;
		clone.GlobalType = this.GlobalType;
		clone.EffectObject = this.EffectObject;
		clone.DestoryTimeOut = this.DestoryTimeOut;
		clone.DestoryInTimeOut = this.DestoryInTimeOut;
		clone.CreateDelay = this.CreateDelay;
		clone.CreateDelayTime = this.CreateDelayTime;
		return clone;
	}
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
	
	public DecalData GetClone()
	{
		DecalData clone = new DecalData();
		clone.Name = this.Name;
		clone.UseGlobalDecal = this.UseGlobalDecal;
		clone.GlobalType = this.GlobalType;
	    clone.DecalObjects = Util.CloneArray<Object>(this.DecalObjects);
		clone.ProjectDirection = this.ProjectDirection;
		clone.DestoryInTimeOut = this.DestoryInTimeOut;	
		clone.DestoryTimeOut = this.DestoryTimeOut;	
		clone.ScaleRate = this.ScaleRate;	
		clone.ApplicableLayer = this.ApplicableLayer;	
		return clone;
	}
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
	
    public HitTriggerType hitTriggerType = HitTriggerType.ByTime;
	
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
    /// Used when HitTestType = CollisionTest or DistanceTest, or AngleTest, or DistanceAndAngleTest
    /// When HitTestType = CollisionTest, HitTestDistance determines the scan enemy range.
    /// When HitTestType = DistanceTest|AngleTest|DistanceAndAngleTest, HitTestDistance determine if the hit message should be sent to CurrentTarget.
    /// </summary>
    public float HitTestDistance = 3;
	
	/// <summary>
	/// The hit test angular discrepancy.
	/// </summary>
	public float HitTestAngularDiscrepancy = 10;
	
    /// <summary>
    /// When target distance lesser than AttackableRange, begin attack.
    /// </summary>
    public float AttackableRange = 3;

    /// <summary>
    /// If attack type = combat - The time to send hit message.
    /// If attack type = arrow - The time to create the arrow object
    /// </summary>
    public float HitTime;

    /// <summary>
    /// After last attacking, how long will the next attack take place.
    /// </summary>
//    public float AttackInterval = 0.5f;

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
	
    public AttackData GetClone()
	{
		AttackData clone = new AttackData();
		base.CloneBasic(clone as UnitAnimationData);
		clone.Type = this.Type;
		clone.hitTriggerType = this.hitTriggerType;
		clone.DamageForm = this.DamageForm;
		clone.WeaponType = this.WeaponType;
		clone.HitTestType = this.HitTestType;
		clone.HitRate = this.HitRate;
		clone.HitTestCollider = this.HitTestCollider;
		clone.HitTestDistance = this.HitTestDistance;
		clone.HitTestAngularDiscrepancy = this.HitTestAngularDiscrepancy;
		clone.AttackableRange = this.AttackableRange;
		clone.HitTime = this.HitTime;
		clone.Projectile = this.Projectile;
		clone.ProjectileInstantiateAnchor = this.ProjectileInstantiateAnchor;
		clone.DamagePointBase = this.DamagePointBase;
		clone.MinDamageBonus = this.MinDamageBonus;
		clone.MaxDamageBonus = this.MaxDamageBonus;
		clone.ScriptObjectAttachToTarget = this.ScriptObjectAttachToTarget;
		return clone;
	}
}

/// <summary>
/// One audio data can contains more than one audio clip, randomly select one when playing. 
/// </summary>
[System.Serializable]
public class AudioData
{
    public string Name = "";
    public AudioClip[] audioClip = new AudioClip[]{};
	public AudioData GetClone()
	{
		AudioData clone = new AudioData();
		clone.Name = this.Name;
		clone.audioClip = Util.CloneArray<AudioClip>(audioClip);
		return clone;
	}
}
