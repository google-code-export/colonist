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
        PuncturedAnchor = 0
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
	public Vector2 Vector2Parameter = Vector2.zero;
	public Vector3 Vector3Parameter = Vector3.zero;
	public GameEvent(){}
	
	public GameEvent(GameEventType _type){
		this.type = _type;
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
/// One combat is one hit information of the predator
/// </summary>
[System.Serializable]
public class Combat
{
	public string name = "";
    /// <summary>
    /// If BlockUserInput = true, then will reject user input when animating.
    /// </summary>
	public bool BlockPlayerInput;
    public float HitPoint = 10;
    /// <summary>
    /// If WaitUntilAnimationReturn = true, then will wait until animation is over.
    /// </summary>
    public bool WaitUntilAnimationReturn;
	/// <summary>
	/// The special animation - if the special animation is null, will play default animation assoicating to the Gesture type.
	/// </summary>
	public string[] specialAnimation = new string[]{};
    public DamageForm damageForm;
	public UserInputType gestureType;
	public string specialCombatFunction="";

    /// <summary>
    /// the gesture information that assoicated to the combat
    /// </summary>
    [HideInInspector]
    public UserInputData gestureInfo;

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
public class ComboCombat
{
    public const int ComboCombatMaxCount = 5;
	public string comboName = "";
	public Combat[] combat = new Combat [] {};
	/// <summary>
	/// Initalize combat token.
	/// 1111 = tap * 4
	/// 1110 = tap * 3 + slice
	/// 11001 = tap + tap + slice + slice + tap
	/// </summary>
	public void Init()
	{
		foreach(Combat ct in combat)
		{
			int _gesturetype = (int)ct.gestureType;
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
	
	/// <summary>
	/// Gets the value at given percentage.
	/// The percentage must be between 0..1
	/// </summary>
	public float GetValueAtPercentage(float percentage)
	{
		float _from = UseRandomValue ? Random.Range(RandomFromValueMin, RandomFromValueMax) : FromValue;
		float _to = UseRandomValue ? Random.Range(RandomToValueMin, RandomToValueMax) : ToValue;
		float valueAtTime = curve.Evaluate(percentage);
		float v = Mathf.Lerp(_from, _to, valueAtTime);
		return v;
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
	public Vector2 GetValueAtPercentage(float percentage)
	{
		return new Vector2(XCurve.GetValueAtPercentage(percentage),YCurve.GetValueAtPercentage(percentage));
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
	public Vector3 GetValueAtPercentage(float percentage)
	{
		return new Vector3(XCurve.GetValueAtPercentage(percentage),YCurve.GetValueAtPercentage(percentage),ZCurve.GetValueAtPercentage(percentage));
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
	public Color GetValueAtPercentage(float percentage)
	{
		float valueAtTime = curve.Evaluate(percentage);
		Color v = Color.Lerp(fromColor, toColor, valueAtTime);
		return v;
	}
}