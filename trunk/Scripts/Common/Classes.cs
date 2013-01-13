using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureInfomation
{
    public GestureType Type;
    public float StartTime;
    public float EndTime;
    public Vector2? gestureDirection;
    public GestureInfomation(GestureType _Type, Vector2? direction, float _StartTime, float _EndTime)
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
/// ReceiveDamageBeheavior - defines animation/function beheavior when receiving damage.
/// This class specially works for AI. 
/// It define how the AI should react to damage.
/// Take AIApplyDamage for an example, when applying damage,  firstly look up the correct ReceiveDamageBeheavior
/// by searching DamageForm, and then, if a) IndependateFunctioning = false, CrossFade animation, SendMessage(Function)
/// else if b)IndependateFunctioning = true, NOT CrossFade animation, just SendMessage(Function)
/// 
/// </summary>
[System.Serializable]
public class ReceiveDamageBeheavior
{
    public string Name = "Description here";
    /// <summary>
    /// if Animation is not empty, will do animation.CrossFade(Animation) every time when receiving damage.
    /// </summary>
    public string[] animation;
    /// <summary>
    /// if Animation is not empty, will do animation[Animation].layer = AnimationLayer in Awake()
    /// </summary>
    public int AnimationLayer = 0;
    public WrapMode wrapMode = WrapMode.Default;

    /// <summary>
    /// The corresponding damageForm on this ReceiveDamageBeheavior
    /// </summary>
    public DamageForm damageForm = DamageForm.Common;

    /// <summary>
    /// If Function != string.empty, will call SendMessage(Function, DamageParameter)
    /// </summary>
    public string Function = "";

    /// <summary>
    /// If IndependentFunctioning = true, will not execute the default animating code, just 
    /// SendMessage(Function, DamageParameter) and exit the applying damage routine.
    /// If IndependentFunctioning = true, Function MUST BE NON-EMPTY.
    /// </summary>
    public bool IndependentFunctioning = false;

    /// <summary>
    /// If isTumble = true, then it means the character is disabled when animation is playing
    /// </summary>
    public bool isTumble = false;
    /// <summary>
    /// Used when isTumble = true
    /// </summary>
    public MoveDirection tumbleDirection;
    /// <summary>
    /// If canMove = true, then it means the character is moving when animation is playing
    /// </summary>
    public bool canMove = false;
    /// <summary>
    /// Used when canMove = true
    /// </summary>
    public MoveDirection moveDirection;
    public float MoveSpeed = 0.2f;


    /// <summary>
    /// Find the ReceiveDamageBeheavior by criteria, and if there's more than one match, result will be randomly
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public static ReceiveDamageBeheavior FindMatchRandomly(DamageForm DamageForm, MoveDirection direction, 
        ReceiveDamageBeheavior[] input, ReceiveDamageBeheavior DefaultIfNoMatch)
    {
        ArrayList list = new ArrayList();
        foreach (ReceiveDamageBeheavior beheavior in input)
        {
            if (beheavior.damageForm == DamageForm && beheavior.moveDirection == direction)
            {
                list.Add(beheavior);
            }
        }
        //No precisely matched? Skip the direction and research
        if (list.Count == 0)
        {
            ReceiveDamageBeheavior beheavior = FindMatchRandomly(DamageForm, input, DefaultIfNoMatch);
            return beheavior;
        }
        //Only one? Return
        else if (list.Count == 1)
        {
            return (ReceiveDamageBeheavior)list[0];
        }
        //Nice, return a random one
        else
        {
            int randomdex = Random.Range(0, list.Count);
            return input[randomdex];
        }
    }

    public static ReceiveDamageBeheavior FindMatchRandomly(DamageForm DamageForm, ReceiveDamageBeheavior[] input, ReceiveDamageBeheavior DefaultIfNoMatch)
    {
        ArrayList list = new ArrayList();
        foreach (ReceiveDamageBeheavior beheavior in input)
        {
            if (beheavior.damageForm == DamageForm)
            {
                list.Add(beheavior);
            }
        }
        if (list.Count == 0)
        {
            return DefaultIfNoMatch;
        }
        else if (list.Count == 1)
        {
            return (ReceiveDamageBeheavior)list[0];
        }
        else
        {
            int randomdex = Random.Range(0, list.Count);
            return input[randomdex];
        }
    }
}

[System.Serializable]
public class DieBeheavior
{
    public string Name = "Description here";
    /// <summary>
    /// The Damage Parameter which cause the death
    /// </summary>
    [HideInInspector]
	public DamageParameter damageParameter;

    public DamageForm damageForm;

    /// <summary>
    /// If DieReplacement != null, will instantiate a DieReplacement object and destory the attached gameobject
    /// </summary>
    public GameObject DieReplacement;
    /// <summary>
    /// If CopyTransfromToReplacement = true, will copy all the current tansform to replacement's transform
    /// </summary>
    public bool CopyTransfromToReplacement;

    /// <summary>
    /// If FunctionName != string.empty, will call Component.SendMessage(FunctionName)
    /// </summary>
    public string FunctionName = "";

    public GameObject[] ObjectToDestory = null;

    /// <summary>
    /// Find the DieBeheavior by criteria, and if there's more than one match, result will be randomly
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public static DieBeheavior FindMatchRandomly(DieBeheavior[] input, DamageForm damageForm, DieBeheavior defaultBeheavior)
    {
        ArrayList list = new ArrayList();
        foreach (DieBeheavior beheavior in input)
        {
            if (beheavior.damageForm == damageForm)
            {
                list.Add(beheavior);
            }
        }
        if (list.Count == 0)
        {
            return defaultBeheavior;
        }
        else if (list.Count == 1)
        {
            return (DieBeheavior)list[0];
        }
        else
        {
            int randomdex = Random.Range(0, list.Count);
            return input[randomdex];
        }
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

/// <summary>
/// This class defines the camera shifting beheavior in scenario mode.
/// </summary>
[System.Serializable]
public class CameraDock
{
    /// <summary>
    /// The position, rotation of this dock
    /// </summary>
    public Transform Transform;

    /// <summary>
    /// How long should the camera arrive this dock ?
    /// If the FleetTime == zero, camera will be forced to set in the dock immediately.
    /// </summary>
    public float FleetTime;

    public string function = string.Empty;

    /// <summary>
    /// If set true, Yield return the coroutine 
    /// </summary>
    public bool YieldExecution = false;

    public static IEnumerator DockTo(CameraDock dock, Camera camera, MonoBehaviour mono)
    {
        //If FleetTime == zero, aligh at once !
            if (dock.FleetTime == 0)
            {
                camera.transform.position = dock.Transform.position;
                camera.transform.rotation = dock.Transform.rotation;
            }
            //Smooth damp the camera to dock transform
            else
            {
                float Overtime = dock.FleetTime;
                float _t = Time.time;
                Vector3 vel = new Vector3();
                Vector3 direction = dock.Transform.position - camera.transform.position;
                Vector3 normalizedDirection = direction.normalized;
                float distance = direction.magnitude;
                float movementSpeed = distance / Overtime;

                float AngleDistance = Quaternion.Angle(camera.transform.rotation, dock.Transform.rotation);
                float rotateAnglarSpeed = AngleDistance / Overtime;
                while ((Time.time - _t) <= Overtime)
                {
                    //camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, dock.Transform.rotation, (1 / Overtime) * Time.deltaTime);
                    camera.transform.rotation = Quaternion.RotateTowards(camera.transform.rotation, dock.Transform.rotation, rotateAnglarSpeed * Time.deltaTime);
                    camera.transform.position += normalizedDirection * movementSpeed * Time.deltaTime;
                    yield return null;
                }
            }
            if (dock.function != string.Empty)
            {
                if (dock.YieldExecution)
                {
                    yield return mono.StartCoroutine(dock.function);
                }
                else
                {
                    mono.SendMessage(dock.function);
                }
            }
            yield return null;
    }

    /// <summary>
    /// The default routine to control the camera docking
    /// </summary>
    public static IEnumerator AutoDock(CameraDock[] cameraDock, Camera camera, MonoBehaviour mono)
    {
        foreach (CameraDock dock in cameraDock)
        {
            yield return mono.StartCoroutine(DockTo(dock, camera, mono));
        }
        yield break;
    }
}


public class GameEvent
{
    public GameEventType type;
    public GameObject sender;
    public Hashtable parameters;

    public GameEvent(GameEventType type, GameObject sender)
    {
        this.type = type;
        this.sender = sender;
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
	public GestureType gestureType;
	public string specialCombatFunction="";

    /// <summary>
    /// the gesture information that assoicated to the combat
    /// </summary>
    [HideInInspector]
    public GestureInfomation gestureInfo;

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