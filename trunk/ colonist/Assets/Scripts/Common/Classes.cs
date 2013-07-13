using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gesture infomation.
/// GestureInfomation wraps user input data.
/// 
/// </summary>
public class UserInputData
{
    public UserInputType Type;
    public float StartTime;
    public float EndTime;
    public Vector2? gestureDirection;
    public UserInputData(UserInputType _Type, Vector2? direction, float _StartTime, float _EndTime)
    {
        Type = _Type;
        gestureDirection = direction;
        StartTime = _StartTime;
        EndTime = _EndTime;
    }
}

public class PredatorPlayerAttackCommand
{
    public DamageForm damageForm;
    /// <summary>
    /// to what direction should the attack command submit to
    /// </summary>
    public Vector3? direction;
    /// <summary>
    /// Gesture strength range at [0..1]
    /// </summary>
    public float Strength;

    public PredatorPlayerAttackCommand(DamageForm _damageForm, Vector3? _direction, float _Strength)
    {
        damageForm = _damageForm;
        direction = _direction;
        Strength = _Strength;
    }
}


/// <summary>
/// DamageParameter - the message sent to AIApplyDamage to describe detail information of a damage
/// </summary>
public class DamageParameter
{
    #region mandatory
    public GameObject src;
    public DamageForm damageForm;
    public float damagePoint;
    #endregion

    #region optional

    public enum ExtraParameterKey
    {
        PuncturedAnchor = 0,
		/// <summary>
		/// if extraParameter contains key = IsCriticalStrike and value = true, means this is CriticalStrike.
		/// </summary>
		IsCriticalStrike = 1,
		/// <summary>
		/// if IsCriticalStrike, this value stores the critical strike rate.
		/// </summary>
		CritcialStrikeRate = 2,
    }
    /// <summary>
    /// Use extraParameter to pass any non-standard parameter.
    /// </summary>
    /* Hashtable<ExtraParameterKey, GameObject > */
    public Hashtable extraParameter = new Hashtable();
    #endregion

    public DamageParameter(GameObject source, DamageForm form, float point)
    {
        this.src = source;
        this.damageForm = form;
        this.damagePoint = point;
    }
}

/// <summary>
/// Define parameter for a toss.
/// </summary>
[System.Serializable]
public class TossParameter
{
    /// <summary>
    /// GameObject that toss
    /// </summary>
    public GameObject Tosser;

    /// <summary>
    /// GameObject being tossed
    /// </summary>
    public GameObject Tossee;

    /// <summary>
    /// should be normalized direction.
    /// </summary>
    public Vector3 forceDirection = new Vector3();

    public float Force = 10;

    public TossParameter(GameObject tosser, GameObject tossee, float force, Vector3 direction)
    {
        Tosser = tosser;
        Tossee = tossee;
        Force = force;
        forceDirection = direction;
    }
}

public enum ParameterType
{
	None = 0,
	Int = 1,
	Float = 2,
	String = 3,
	Bool = 4,
}

/// <summary>
/// MessageData wrap the message send to game object.
/// Includes: 
/// 1. Message.
/// 2. Parameter type - Int, Float, String, Bool
/// 3. Parameter - Int, Float, String, Bool
/// </summary>
//[System.Serializable]
//public class MessageData
//{
//	public GameObject Receiver = null;
//	public string Message = "FunctionName";
//	public MessageParameterType messageParameterType = MessageParameterType.None;
//	public int IntParameter;
//	public float FloatParameter;
//	public string StringParameter = "";
//	public bool BoolParameter;
//}

/// <summary>
/// Game event definition.
/// GameEvent can be used in many purpose. It can be fired in run time, or defined in Inspector.
/// 
/// </summary>
[System.Serializable]
public class GameEvent
{
	public string Name = "";
    public GameEventType type;
	
	/// <summary>
	/// The receiver, if SendToLevelManager is true, the receiver is ignored.
	/// </summary>
	public GameObject receiver;
	[HideInInspector]
    public GameObject sender;
	
	public float delaySend = 0;
	
	public string CustomMessage = "FunctionName";
	public ParameterType parameterType = ParameterType.None;
	public int IntParameter;
	public float FloatParameter;
	public string StringParameter = "";
	public bool BoolParameter;
	public object ObjectParameter = null;
	public GameObject GameObjectParameter = null;
	public Vector2 Vector2Parameter = Vector2.zero;
	public Vector3 Vector3Parameter = Vector3.zero;
	public GameEvent(){}
	
	public GameEvent(GameEventType _type){
		this.type = _type;
	}
	
	public GameEvent Clone()
	{
        return this.MemberwiseClone() as GameEvent;
	}
}

public class TouchInfomation
{
    public Touch touch;
    public float Time;
	public TouchInfomation(){}
    public TouchInfomation(Touch t, float time)
    {
        this.touch = t;
        Time = time;
    }
}

/// <summary>
/// Predator player attack data.
/// One attackdata links to one animation.
/// What's More that AttackData, is PredatorPlayerAttackData defines critical attack data.
/// </summary>
[System.Serializable]
public class PredatorPlayerAttackData : AttackData
{
	/// <summary>
	/// if CanDoCriticalAttack, then this PredatorPlayerAttackData
	/// has %CriticalAttackChance% to do a %CriticalAttackBonusRate% critical attack.
	/// </summary>
	public bool CanDoCriticalAttack = false;
	public float CriticalAttackChance = 0.3f;
	public float CriticalAttackBonusRate = 1.2f;
	public override AttackData GetClone()
	{
		PredatorPlayerAttackData clone = new PredatorPlayerAttackData();
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
		clone.CanDoCriticalAttack = this.CanDoCriticalAttack;
		clone.CriticalAttackBonusRate = this.CriticalAttackBonusRate;
		clone.CriticalAttackChance = this.CriticalAttackChance;
		return clone;
	}
}

/// <summary>
/// One combat is one hit information of the predator
/// </summary>
[System.Serializable]
public class PredatorCombatData
{
	public string Name = "";
	
	/// <summary>
	/// Defines the name of PredatorPlayerAttackData which is used in this combat.
	/// </summary>
	public string[] useAttackDataName = new string[] {};
	
    /// <summary>
    /// If BlockUserInput = true, then will reject user input when animating.
    /// </summary>
	public bool BlockPlayerInput;
	
    /// <summary>
    /// If WaitUntilAnimationReturn = true, then will wait until animation is over.
    /// </summary>
    public bool WaitUntilAnimationReturn;
	
	/// <summary>
	/// If OverrideAttackData = true, these property of PredatorAttackData will be overrided by PredatorCombatData:
	///  - DamagePointBase
	///  - MinDamageBonus
	///  - MaxDamageBonus
	///  - CanDoCriticalAttack
	///  - CriticalAttackBonusRate
	///  - CriticalAttackChance
	/// </summary>
	public bool OverrideAttackData = false;
	
	public float DamagePointBase = 10;
	public float MinDamagePointBonus = 1;
	public float MaxDamagePointBonus = 1;
	public bool CanDoCriticalAttack = false;
	public float CriticalAttackBonusRate = 1.2f;
	public float CriticalAttackChance = 0.7f;
	
	/// <summary>
	/// Bound to the user input type.
	/// That what's the user input type will trigger this combat.
	/// </summary>
	public UserInputType userInput;

    /// <summary>
    /// If FinalCombat = true, then user input should be unblocked after this combat.
    /// </summary>
    [HideInInspector]
    public bool FinalCombat;
}

/// <summary>
/// Combo combat = a series of combat.
/// </summary>
[System.Serializable]
public class PredatorComboCombat
{
    public const int ComboCombatMaxCount = 5;
	public string comboName = "";
	public PredatorCombatData[] combat = new PredatorCombatData [] {};
	/// <summary>
	/// Initalize combat token.
	/// 1111 = tap * 4
	/// 1110 = tap * 3 + slice
	/// 11001 = tap + tap + slice + slice + tap
	/// </summary>
	public void Init()
	{
		foreach(PredatorCombatData ct in combat)
		{
			int _gesturetype = (int)ct.userInput;
			token += _gesturetype.ToString();
		}
	}
	[HideInInspector]
	public string token;
}

/// <summary>
/// Represent a float curve, which can be edited in inspector.
/// </summary>
[System.Serializable]
public class FloatCurve
{
	public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public float FromValue;
	public float ToValue;
	/// <summary>
	/// if UseRandomValue = true, the FromValue, ToValue is not replaced by Random(RandomFromValueMin, RandomFromValueMax) and Random(RandomToValueMin, RandomToValueMax) 
	/// 
	/// </summary>
	public bool UseRandomValue;
	public float RandomFromValueMin;
	public float RandomFromValueMax;
	public float RandomToValueMin;
	public float RandomToValueMax;
	[HideInInspector]
	public float MaxTime = 2;
	/// <summary>
	/// Gets the value at given percentage.
	/// The percentage must be between 0..1
	/// </summary>
	public float NormalizedEvaluate(float percentage)
	{
		float _from = UseRandomValue ? Random.Range(RandomFromValueMin, RandomFromValueMax) : FromValue;
		float _to = UseRandomValue ? Random.Range(RandomToValueMin, RandomToValueMax) : ToValue;
		float valueAtTime = curve.Evaluate(percentage);
		float v = Mathf.Lerp(_from, _to, valueAtTime);
		return v;
	}
	
	public float Evaluate(float time)
	{
		return curve.Evaluate(time);
	}
}

/// <summary>
/// Represent a Vector2 curve, which can be edited in inspector.
/// </summary>
[System.Serializable]
public class Vector2Curve
{
	public FloatCurve XCurve;
	public FloatCurve YCurve;
	/// <summary>
	/// Gets the value at given percentage.
	/// The percentage must be between 0..1
	/// </summary>
	public Vector2 NormalizedEvaluate(float percentage)
	{
		return new Vector2(XCurve.NormalizedEvaluate(percentage),YCurve.NormalizedEvaluate(percentage));
	}
	
	public Vector2 Evaluate(float time)
	{
		return new Vector2(XCurve.Evaluate(time), YCurve.Evaluate(time));
	}
}

/// <summary>
/// Represent a Vector3 curve, which can be edited in inspector.
/// </summary>
[System.Serializable]
public class Vector3Curve
{
	public FloatCurve XCurve;
	public FloatCurve YCurve;
	public FloatCurve ZCurve;
	/// <summary>
	/// Gets the value at given time.
	/// The percentage must be between 0..1
	/// </summary>
	public Vector3 NormalizedEvaluate(float percentage)
	{
		return new Vector3(XCurve.NormalizedEvaluate(percentage),YCurve.NormalizedEvaluate(percentage),ZCurve.NormalizedEvaluate(percentage));
	}
	
	public Vector3 Evaluate(float time)
	{
		return new Vector3(XCurve.Evaluate(time), YCurve.Evaluate(time), ZCurve.Evaluate(time));
	}	
}

[System.Serializable]
public class RectCurve
{
	public FloatCurve XCurve;
	public FloatCurve YCurve;
	public FloatCurve WidthCurve;
	public FloatCurve HeightCurve;
	public float MaxTime = 3;
	/// <summary>
	/// Gets the value at given time.
	/// The percentage must be between 0..1
	/// </summary>
	public Rect NormalizedEvaluate(float percentage)
	{
		return new Rect(XCurve.NormalizedEvaluate(percentage),
			            YCurve.NormalizedEvaluate(percentage),
			            WidthCurve.NormalizedEvaluate(percentage),
			            HeightCurve.NormalizedEvaluate(percentage)
			           );
	}
	
	public Rect Evaluate(float time)
	{
		return new Rect(XCurve.Evaluate(time),
			            YCurve.Evaluate(time),
			            WidthCurve.Evaluate(time),
			            HeightCurve.Evaluate(time)
			           );
	}	
}

[System.Serializable]
public class ColorCurve
{
	public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public Color fromColor = Color.white;
	public Color toColor = Color.white;
	/// <summary>
	/// Gets the value at given time.
	/// The percentage must be between 0..1
	/// </summary>
	public Color NormalizedEvaluate(float percentage)
	{
		float valueAtTime = curve.Evaluate(percentage);
		Color v = Color.Lerp(fromColor, toColor, valueAtTime);
		return v;
	}
}

public enum TopDownCameraControlMode
{
	/// <summary>
	/// The default mode, camera's position and focus point is dynamically set by parameter. And always look at the Player.
	/// </summary>
	Default = 0,
	
	/// <summary>
	/// Always put the camera at a fixed position, and look at the player.
	/// </summary>
	PoisitonAtPivotAndLookAtPlayer = 1,
	
	/// <summary>
	/// Always put the camera at a fixed position, and look at the given transform.
	/// </summary>
	PositionAtPivotAndLookAtTransform = 2,
	
	/// <summary>
	/// Position at pivot and look at a fixed position.
	/// </summary>
	PositionAtPivotAndLookAtPosition = 3,
	
	/// <summary>
	/// The position of the camera is dynamically control by topDown parameter,
	/// and look at a transform
	/// </summary>
	ParameterControlPositionAndLookAtTransform = 4,
	
	/// <summary>
	/// The position of the camera is dynamically control by topDown parameter,
	/// and look at a fixed point
	/// </summary>
	ParameterControlPositionAndLookAtPosition = 5,
}

/// <summary>
/// CameraDynamicalControlParameter represents parameter to control the camera.
/// </summary>
[System.Serializable]
public class TopDownCameraControlParameter
{
	public string Name = "";
	public TopDownCameraControlMode mode = TopDownCameraControlMode.Default; 
//	/// <summary>
//	/// The transform to look at. Can be changed in runtime. e.g. for Slow motion camera.
//	/// </summary>
//	public GameObject LookAt = null;
	public float DynamicDistance = 1;
	public float DynamicHeight = 1.5f;
	/// <summary>
	/// The target transform to look at. If this is null, than will pick DefaultViewPoint to look at.
	/// </summary>
	//[HideInInspector]
	//public Transform LookAtTarget = null;
	public float smoothLag = 0.3f;

	/// <summary>
	/// Layer mask to check if the current view sight has been blocked.
	/// </summary>
	public LayerMask lineOfSightMask = 0;
	
	/// <summary>
	/// The transform pivot used when mode = PoisitonAtPivotAndLookAtPlayer.
	/// Camera will be aligned to the position in runtime.
	/// </summary>
	public Transform CameraPositionPivot = null;
	/// <summary>
	/// The transform pivot used when mode = PositionAtPivotAndLookAtTransform.
	/// Camera will look at the transform pivot in runtime.
	/// </summary>
	public Transform cameraFocusOnTransform = null;
	
	/// <summary>
	/// The camera focus on position.
	/// Differs to cameraFocusOnTransform, this is a static vector3 position.
	/// </summary>
	public Vector3 cameraFocusOnPosition = Vector3.zero;
	
}

/// <summary>
/// A rectangle that self-adaptive to GUI screen.
/// You can use AdaptiveRect to create:
/// 1. A rectangle on a fixed point, in fixed size.
/// 2. A rectangle on a fixed point, in screen size-relative size.
/// 3. A rectangle on a screen-relative point, in a fixed size..
/// ... etc.
/// </summary>
[System.Serializable]
public class AdaptiveRect
{
	/// <summary>
	/// The bound anchor to screen left.
	/// </summary>
	public AdaptiveAnchor AdaptiveAnchor_Left;
	/// <summary>
	/// The bound anchor to screen top.
	/// </summary>
	public AdaptiveAnchor AdaptiveAnchor_Top;
	/// <summary>
	/// The bound width.
	/// </summary>
	public AdaptiveLength AdaptiveWidth;
	/// <summary>
	/// The bound height.
	/// </summary>
	public AdaptiveLength AdaptiveHeight;
	
	/// <summary>
	/// If the JoyButton need to refer to other joyButton, assign true and ReferrenceJoyButtonName.
	/// </summary>
	public bool HasReference = false;
	public string ReferrenceJoyButtonNanme = "";
	
	[HideInInspector]
	public JoyButton ReferrenceJoyButton = null;
	
	/// <summary>
	/// Use this variable to store the bound (save per-frame cacluation)
	/// </summary>
	[HideInInspector]
	public Rect Bound = new Rect();
	
	public Vector2 GetAnchor()
	{
		Vector2 anchor = HasReference ? new Vector2(AdaptiveAnchor_Left.GetValue(ReferrenceJoyButton.adaptiveBound.GetAnchor()),
			                                     AdaptiveAnchor_Top.GetValue(ReferrenceJoyButton.adaptiveBound.GetAnchor()))
			                       : new Vector2(AdaptiveAnchor_Left.GetValue(), AdaptiveAnchor_Top.GetValue());
		return anchor;
	}
	
	public Vector2 GetSize()
	{
		Vector2 size = new Vector2(AdaptiveWidth.GetValue(), AdaptiveHeight.GetValue());
		return size;
	}
	
	public Rect GetBound()
	{
		Vector2 anchor = GetAnchor();
		Vector2 size = GetSize();
		return new Rect(anchor.x, anchor.y, size.x, size.y);
	}
}

/// <summary>
/// An icon, self-adaptive to current screen resolution.
/// </summary>
[System.Serializable]
public class AdaptiveIcon {
	public Color color = Color.white;
	public AdaptiveRect adaptiveRect = new AdaptiveRect();
	public Texture texture;
	public ScaleMode scaleMode = ScaleMode.StretchToFill;
	[HideInInspector]
	public Rect realtimeRect;
}
