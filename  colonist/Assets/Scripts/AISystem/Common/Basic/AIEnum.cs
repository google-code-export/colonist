public enum EffectObjectInstantiation
{
	creat = 0,
    play = 1,
}

public enum AIBehaviorType
{
    #region base 0~999
    /// <summary>
    /// Idle - AI standing idle, just animating Idle animation.
    /// If there're enemy coming insight, shift to AttackTo Command.
    /// </summary>
    Idle = 0,
    /// <summary>
    /// MoveToTransform - move to a transform
    /// </summary>
    MoveToTransform = 1,
    /// <summary>
    /// MoveAtDirection - move to a direction
    /// </summary>
    MoveAtDirection = 2,
	
	/// <summary>
	/// Move to current target.
	/// </summary>
	MoveToCurrentTarget = 3,
	
    /// <summary>
    /// Attack - find attackable target and start attacking
    /// </summary>
    Attack = 4,
    /// <summary>
    /// Assault to a position.
    /// During the way to position, if AI meet a target, AI should turn to attack the target
    /// </summary>
    AttackToPosition = 5,

    /// <summary>
    /// Assualt to direction.
    /// Move to a direction, if AI meet a target, AI should turn to attack the target.
    /// </summary>
    AttackToDirection = 6,

    /// <summary>
    /// Holds in a position without leaving.
    /// In holding mode, AI attack enemy in sight. But not tracking the enemy.
    /// </summary>
    HoldPosition = 7,
	
	/// <summary>
	/// Special AI behavior.
	/// This AI will be set off , another AI will be set active.
	/// </summary>
	SwitchToAI = 8,
    #endregion
}

public enum AIBehaviorPhase
{
    /// <summary>
    /// The default phase, when a behavior is not executed, it's sleeping
    /// </summary>
    Sleeping = 0,
    /// <summary>
    /// When a behavior is running
    /// </summary>
    Running = 1
}


/// <summary>
/// Define how should AI unit send ApplyDamage message.
/// </summary>
public enum AIAttackType
{
    /// <summary>
    /// Instant - think fist, or machine gun, the ApplyDamage message is sent immediately at a fixed time point.
    /// </summary>
    Instant = 0,
    /// <summary>
    /// Projectile - think a flying arrow, or missile, or fireball, hurt when it reaches enemy.
    /// The actual ApplyDamage message is sent when the projectile reaches enemy/target position
    /// </summary>
    Projectile = 1,

    /// <summary>
    /// Regional - think a flame thrower, create damage to all enemy units inside an area.
    /// HitTestType is always CollisionTest when AttackType = Regional.
    /// And AI.HitTestCollider MUST NOT BE NULL when attacktype = Regional.
    /// </summary>
    Regional = 2,
}

/// <summary>
/// In detective range, if there're more than one target, how should AI select 
/// CurrentTarget? This enum defines the selection rules.
/// </summary>
public enum SelectTargetRule
{
    /// <summary>
    /// Default = Closest. The default rules is selecting closest enemy. This is most efficient solution.
    /// What's more, when no enemy meets other rules, the default rule will be applied.
    /// For example, there're no player inside AI.DetectiveRange, then AI will select closest NPC enemy.
    /// </summary>
    Default = 0,
    /// <summary>
    /// AI select least HP enemy target
    /// </summary>
    //TargetMinHP = 1,
    /// <summary>
    /// AI select most HP enemy target
    /// </summary>
    //TargetMaxHP = 2,
    /// <summary>
    /// AI select closest enemy as target
    /// </summary>
    Closest = 3,
    /// <summary>
    /// AI select farest enemy as target
    /// </summary>
    Farest = 4,
}

/// <summary>
/// For NON-Projectile attack type only, define if AI should send the ApplyDamage message
/// to target.
/// </summary>
public enum HitTestType
{
    /// <summary>
    /// Always send ApplyMessage.
    /// </summary>
    AlwaysTrue = 0,
    /// <summary>
    /// HitRate = 0 to 1. For example, if HitRate = 0.7, means the chance to send out ApplyMessage = 70%.
    /// When HitRate = 1, it's the same to AlwaysTrue
    /// </summary>
    HitRate = 1,
    /// <summary>
    /// If the target's collider is intersecting with the hit test collider.
    /// For Regional AttackType, the HitTestType is always CollisionTest
    /// </summary>
    CollisionTest = 2,

    /// <summary>
    /// If the target to AI distance within the HitTestDistance.
    /// </summary>
    DistanceTest = 3,
	
	/// <summary>
	/// If the target to AI.transform.forward angular discrepancy within the HitTestAngle
	/// </summary>
	AngleTest = 4,
	
	/// <summary>
	/// Combination of DistanceTest and AngleTest
	/// </summary>
	DistanceAndAngleTest = 5,
}


/// <summary>
/// Define how should projectile unit send ApplyDamage message
/// </summary>
public enum ProjectileAttackType
{
    /// <summary>
    /// The projectile send ApplyDamage only to the target. Think bullet/arrow. 
    /// </summary>
    SingleTarget = 0,
    /// <summary>
    /// The projectile send ApplyDamage to an area, think cannon/missile
    /// </summary>
    Explosion = 1,
}


public enum ProjectileMovementMode
{
    /// <summary>
    /// Think bullet - Lanuch from start point, fly always at forward direction. 
    /// What's different to StraightLineToPosition mode, StarightForward has no specified end position.
    /// In this mode, Projectile has to destory itself when hitting something/time expired.
    /// Note: StarightForward projectile CAN NO self guide!
    /// </summary>
    StraightForward = 0,
    /// <summary>
    /// Projectile from start point -> target point in a straight line.
    /// Think missile - it fly from a start point to end point in a straight line
    /// </summary>
    StraightLineToPosition = 1,
    /// <summary>
    /// Projectile flies from start point -> target point in parabola line
    /// Think shell - fires from a start point, falls to end point in parabola line.
    /// </summary>
    Parabola = 2,
}

/// <summary>
/// Enum for AI behavior condition
///  - Boolean , is case true of false?
///  - ValueComparision, is a value eq/gt/ge/le/lt some value?
///  - And, conjunct two condition with AND operator
///  - Or, conjunct two condition with OR operator
/// </summary>
public enum AIBehaviorConditionType
{
     Boolean = 0,
     ValueComparision = 1,
}

public enum AIBooleanConditionEnum
{
#region Basic 0-999
    /// <summary>
    /// AlwaysTrue - Use this as default AI start behavior.
    /// </summary>
    AlwaysTrue = 0,
    /// <summary>
    /// Is Enemy within AI.OffenseRange?
    /// </summary>
    EnemyInOffensiveRange = 1,
    /// <summary>
    /// Is Enemy within AI.DetectiveRange?
    /// </summary>
    EnemyInDetectiveRange = 2,
    /// <summary>
    /// Is AI within a specified area?
    /// </summary>
    InArea = 3,
    /// <summary>
    /// Current target's gameObject.Layer within/not within some layermask
    /// </summary>
    CurrentTargetInLayer = 4,

    /// <summary>
    /// Is there any enemy in specified area ?
    /// </summary>
    EnemyInArea = 5,
	
	/// <summary>
	/// The latest running behavior's name is/is not %string value%
	/// </summary>
	LatestBehaviorNameIs = 6,
    
	/// <summary>
	/// Alike LatestBehaviorNameIs, can support more than one right value. The right value is %string array%.
	/// </summary>
	LastestBehaviorNameIsOneOf = 7,
#endregion
}

public enum AIValueComparisionCondition
{
#region Basic 0-999
    /// <summary>
    /// The nearest enemy distance eq/gt/ge/le/lt some value
    /// </summary>
    NearestEnemyDistance = 0,
    /// <summary>
    /// The farest enemy distance eq/gt/ge/le/lt some value
    /// </summary>
    FarestEnemyDistance = 1,
    /// <summary>
    /// HP Percentage of the AI eq/gt/ge/le/lt some value
    /// </summary>
    HPPercentage = 2,
    /// <summary>
    /// HP Percentage of current taget eq/gt/ge/le/lt some value
    /// </summary>
    CurrentTargetHPPercentage = 3,
    /// <summary>
    /// The time from last execution end time to now.
    /// </summary>
    BehaviorLastExecutionInterval = 4,
    /// <summary>
    /// Distance of current target eq/gt/ge/le/lt some value
    /// </summary>
    CurrentTagetDistance = 5,

    /// <summary>
    /// Execution counter eq/gt/ge/le/lt some value
    /// </summary>
    ExeuctionCount = 6,
    
    /// <summary>
    /// A random float value between 0 - 100
    /// </summary>
    RandomValue = 7,
	
    /// <summary>
	/// How long has the behavior being executed
	/// </summary>
	BehaveTime = 8,
	
	/// <summary>
	/// Attack counter value of the Unit.
	/// </summary>
	AttackCount = 9,
	/// <summary>
	/// DoDamage counter value of the Unit.
	/// </summary>
	DoDamageCount = 10,

#endregion
}

/// <summary>
/// Armor type is used when judging some combat details.
/// Armor type is important for audio playing.
/// </summary>
public enum ArmorType
{
	Default = 0,
	#region human 1~999
	NoArmor_Human = 1,
	LightArmor_Human = 2,
	MiddleArmor_Human = 3,
	HeavyArmor_Human = 4,
    Metal_1 = 5,
	Metal_2 = 6,
	#endregion
	
	#region xenz 1000~1999
	NoArmor_Xenz = 1000,
	#endregion
}

/// <summary>
/// Weapon type.
/// When playing hit audio, [weapon type + armor type] determines what audio should be played.
/// </summary>
public enum WeaponType
{
	Default = 0,
	#region human 1~999
	Bullet = 1,
	ElectricalWeapon = 2,
	Incendiator = 3,
	#endregion
	
	#region xenz 1000~1999
	Predator_Spine = 1000,
	GiantAnt_Flame = 1001,
	#endregion
}