using UnityEngine;
using System.Collections;

/// <summary>
/// Wrap the base data for a unitdata
/// </summary>
[System.Serializable]
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
	/// <summary>
	/// If UseAnimation = false, the MoveData's animation will not be played when moving.
	/// </summary>
	public bool UseAnimation = true;
	
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
	/// <summary>
	/// If UseGravityWhenMoving = true, the Behavior will use SimpleMove, else, use Move()
	/// </summary>
	public bool UseGravityWhenMoving = true;
	public MoveData GetClone()
	{
		MoveData clone = new MoveData();
		base.CloneBasic(clone as UnitAnimationData);
		clone.MoveSpeed = this.MoveSpeed;
		clone.CanRotate = this.CanRotate;
		clone.SmoothRotate = this.SmoothRotate;
		clone.RotateAngularSpeed = this.RotateAngularSpeed;
		clone.UseGravityWhenMoving = this.UseGravityWhenMoving;
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
    public DamageForm[] ApplicableDamageForm = new DamageForm[] { DamageForm.Common };
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
	
	/// <summary>
	/// The audioData that should be played when the receiveDamageData is used.
	/// </summary>
	public string[] AudioDataName = new string[] { };
	
    public ReceiveDamageData GetClone()
	{
		ReceiveDamageData clone = new ReceiveDamageData();
		base.CloneBasic(clone);
		clone.ApplicableDamageForm = Util.CloneArray<DamageForm>( this.ApplicableDamageForm );
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
    public DamageForm[] ApplicableDamageForm = new DamageForm[] { DamageForm.Common };
    /// <summary>
    /// EffectDataName - the effectdata will be created immediately when playing the animation.
    /// </summary>
    public string[] EffectDataName = new string[] { };
    /// <summary>
    /// DecalDataName - the decal object will be created when die
    /// </summary>
    public string[] DecalDataName = new string[] { };
	/// <summary>
	/// The AudioData which will be played when the DeathData triggered.
	/// </summary>
	public string[] AudioDataName = new string[]{ };
	
	/// <summary>
	/// If this flag is marked true, the gameObject will be destoryed if UseDieReplacement = false.
	/// </summary>
	public bool DestoryGameObject = false;
	/// <summary>
	/// DestoryLagTime is used when UseDieReplacement = false and DestoryGameObject = true
	/// The gameObject will be destoryed after N seconds later of Die animation.
	/// </summary>
	public float DestoryLagTime = 0;
	
    public bool UseDieReplacement = false;
    /// <summary>
    /// If UseDieReplacement = true, and:
    /// If ReplaceAfterAnimationFinish = true, will create the DieReplacement GameObject after animation finished.
    /// Else if ReplaceAfterAnimationFinish = false, will create DieReplacement immediately following death, without any animation.
    /// </summary>
    public bool ReplaceAfterAnimationFinish = true;
	/// <summary>
	/// If UseDieReplacement = true, and ReplaceAfterAnimationFinish = false, will create the DieReplacement in %ReplaceAfterSeconds% seconds.
	/// </summary>
	public float ReplaceAfterSeconds = 0;
    /// <summary>
    /// If UseDieRagdoll = true, will destory this gameObject and create the DieReplacement.
    /// </summary>
    public GameObject DieReplacement;
    /// <summary>
    /// If DieReplacement != null and CopyChildrenTransformToDieReplacement = true,
    /// the DieReplacement's children transform will be aligned to gameObject.
    /// </summary>
    public bool CopyChildrenTransformToDieReplacement = false;
	
	/// <summary>
	/// If ReplaceOldObjectInSpawnedList = true, means,
	/// when this unit killed, the SpawnedList should replace this gameObject by the Replacement.
	/// </summary>
	public bool ReplaceOldObjectInSpawnedList = false;
	
	/// <summary>
	/// If this flag = true, the Character Controller attach to this unit will be destoryed/deactivated
	/// when the DeathData is applied.
	/// This means, the movement of the unit is controlled by animation.
	/// But if flag = false, then you can attach animation event to death animation to control the movement.
	/// </summary>
	public bool DestoryCharacterController = true;
	
    public DeathData GetClone()
	{
		DeathData clone = new DeathData();
		base.CloneBasic(clone);
		clone.ApplicableDamageForm = Util.CloneArray<DamageForm>(this.ApplicableDamageForm);
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
/// Instantiation data defines the position/quaternion to instanitate a gameobject.
/// </summary>
[System.Serializable]
public class InstantiationData
{
	/// <summary>
	/// The basic anchor transform.
	/// </summary>
	public Transform BasicAnchor = null;
	/// <summary>
	/// If RandomPositionInsideSphere = true, create the instance at position = BasicAnchor + Random unit inside sphere
	/// </summary>
	public bool RandomPositionInsideSphere = false;
	/// <summary>
	/// if RandomInsideSphere = true, the radius of the random sphere
	/// </summary>
	public float RandomSphereUnit = 1;
	/// <summary>
	/// The final position = BasicAnchor + RandomInsideSphereUnit + WorldOffset.
	/// </summary>
	public Vector3 WorldOffset = Vector3.zero;
	
	/// <summary>
	/// The rotation mode of instance:
	/// 1. random rotation
	/// 2. identity
	/// 3. align to anchor
	/// 4. specified value
	/// </summary>
	public InstantiationRotationMode rotationOfInstance = InstantiationRotationMode.IdentityQuaternion;
	
	/// <summary>
	/// The specified quaterion.Only used when rotationOfInstance = InstantiationRotationMode.SpecifiedQuaternion
	/// </summary>
	public Quaternion specifiedQuaterion = Quaternion.identity;
	
    public InstantiationData GetClone()
	{
		InstantiationData clone = new InstantiationData();
		clone.RandomSphereUnit = this.RandomSphereUnit;
		clone.RandomPositionInsideSphere = this.RandomPositionInsideSphere;
		clone.WorldOffset = this.WorldOffset;
		clone.rotationOfInstance = this.rotationOfInstance;
		clone.specifiedQuaterion = this.specifiedQuaterion;
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
    public GlobalEffectType GlobalType = GlobalEffectType.HumanBlood_Splatter_Small;
    #endregion

    #region use custom effect object
	public InstantiationData instantiationData = new InstantiationData();
	/// <summary>
	/// When creating effect object instance, the creation anchor location = Anchor + random position inside a sphere of radius = RandomSphereRadius.
	/// If the RandomSphereRadius = 0, creation anchor location = Anchor position.
	/// </summary>
//	public float RandomSphereRadius = 0;
	
    public GameObject EffectObject = null;
	
	/// <summary>
	/// The destory time out.
	/// </summary>
    public float DestoryTimeOut = 1;
    #endregion
	
	/// <summary>
	/// The create delay flag.
	/// If CreateDelay is true, then the effect should be created in delay of %CreateDelayTime% seconds.
	/// </summary>
	public bool CreateDelay = true;
	public float CreateDelayTime = 1;
	/// <summary>
	/// The type of the instantion. create new instance, or play existing gameobject particle system.
	/// </summary>
	public EffectObjectInstantiation InstantionType = EffectObjectInstantiation.create;
	
	public EffectData GetClone()
	{
		EffectData clone = new EffectData();
		clone.Name = this.Name;
		clone.Count = this.Count;
		clone.UseGlobalEffect = this.UseGlobalEffect;
		clone.GlobalType = this.GlobalType;
		clone.EffectObject = this.EffectObject;
		clone.DestoryTimeOut = this.DestoryTimeOut;
		clone.CreateDelay = this.CreateDelay;
		clone.CreateDelayTime = this.CreateDelayTime;
		clone.instantiationData = this.instantiationData.GetClone();
		clone.InstantionType = this.InstantionType;
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
    public GlobalDecalType GlobalType = GlobalDecalType.HumanBlood_Splatter_Static;
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
	/// <summary>
	/// The transform object which define the projection position to create decal.
	/// It's optional, if ProjectDecalTransform = null, the Decal is created based on the create effect object.
	/// </summary>
	public Transform OnTransform = null;
	/// <summary>
	/// The create delay time.
	/// </summary>
	public float CreateDelay = 0;
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
	/// If HasAnimation flag = false, means this attack data has no aniamtion and can not be used for AttackBehavior. It can only be
	/// triggered by animation event.
	/// </summary>
	public bool HasAnimation = true;
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
	/// if LookAtTarget = true, AI face to target before attacking.
	/// </summary>
	public bool LookAtTarget = true;
	
    /// <summary>
    /// When target distance lesser than AttackableRange, begin attack.
    /// </summary>
    public float AttackableRange = 3;

    /// <summary>
    /// If attack type = Instant - The time to send hit message.
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
	
    public virtual AttackData GetClone()
	{
		AttackData clone = new AttackData();
		base.CloneBasic(clone as UnitAnimationData);
		clone.HasAnimation = this.HasAnimation;
		clone.Type = this.Type;
		clone.hitTriggerType = this.hitTriggerType;
		clone.DamageForm = this.DamageForm;
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

public enum AudioObjectMode
{
	AudioSouce = 0,
	Clip = 1,
}

public enum AudioPlayMode
{
	/// <summary>
	/// Audio is triggered by animation event.
	/// </summary>
	ByAnimationEvent = 0,
	/// <summary>
	/// Audio is binded to animation, when an animation is playing, the audio is played as well.
	/// If AudioPlayMode = BindToAnimation, the AudioObjectMode MUST BE AudioSouce
	/// </summary>
	BindToAnimation = 1,
}

/// <summary>
/// One audio data can contains more than one audio clip, randomly select one when playing. 
/// </summary>
[System.Serializable]
public class AudioData
{
    public string Name = "";
	/// <summary>
	/// The audio object mode.
	/// </summary>
	public AudioObjectMode mode = AudioObjectMode.AudioSouce;
	
	/// <summary>
	/// The audio play mode.
	/// </summary>
	public AudioPlayMode playMode = AudioPlayMode.ByAnimationEvent;
	
	/// <summary>
	/// if enabled is false, the audio would not be played.
	/// </summary>
	public bool enabled = true;
	
	/// <summary>
	/// The randomly placyed audio clips. Used when mode = Clip.
	/// </summary>
    public AudioClip[] randomAudioClips = new AudioClip[]{};
	/// <summary>
	/// The randomly picked audio sources. Used when mode = AudioSource.
	/// </summary>
	public AudioSource[] randomAudioSources = new AudioSource[]{};
	/// <summary>
	/// When mode = Clip, the transform position to be played in 3D world.
	/// </summary>
	public Transform Play3DClipAnchor = null;
	/// <summary>
	/// Only used when mode = Clip.
	/// </summary>
	public float PlayClipVolume = 1;
	/// <summary>
	/// Used when playMode = BindToAnimation
	/// </summary>
	public string[] AnimationBinded = new string[]{};
	public AudioSource BindedAudioSource = null;
	public AudioData GetClone()
	{
		AudioData clone = new AudioData();
		clone.Name = this.Name;
		clone.randomAudioClips = randomAudioClips;
		return clone;
	}
}


[System.Serializable]
/// <summary>
/// This class is used to defined the behavior of replacing this gameObject to another game object in runtime.
/// </summary>
public class ReplaceObjectData
{
   public enum ReplaceObjectType
   {
		ReplaceToVerticalSplittedBodyObject = 0,
   }
	public string Name = "";
	public ReplaceObjectType replaceType = ReplaceObjectType.ReplaceToVerticalSplittedBodyObject;
	public GameObject replacement = null;
}
