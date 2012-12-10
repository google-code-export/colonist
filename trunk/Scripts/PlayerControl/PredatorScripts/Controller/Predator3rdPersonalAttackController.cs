using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[@RequireComponent(typeof (PredatorPlayerStatus))]
public class Predator3rdPersonalAttackController : MonoBehaviour {

    /// <summary>
    /// The length the claw can reach to
    /// </summary>
    public float ClawRadius = 3f;

    public float OffenseRadiusNear = 6;

    public string PreAttackAnimation = "preattack_right";

    public string PreAttackAnimation_Mixed = "preattack_right_mix";

    //public string StrikeUpwardAnimation = "attack_upward";
    //public string[] StrikeStarightAnimation = new string[]{"attack_straight"};
    //public string StrikeDownwardAnimation = "attack_downward";
    //public string StrikeMidCutAnimation = "attack_middle_cut";

    public string[] Strike_SingleClaw = new string[]{
        "attack_left_claw_spike-1", "attack_left_claw_spike-2",
        "attack_right_claw_spike-1", "attack_right_claw_spike-2"
    };
    public string[] Strike_DualClaws = new string[]{
        "attack_dual_claw_spike-1","attack_dual_claw_spike-2","attack_dual_claw_spike-3"
    };
    public string[] WavingClaw = new string[]{
        "attack_left_claw_waving","attack_right_claw_waving"
    };
    public string[] ClampingClaws = new string[]{
        "attack_clamp"
    };
    /// <summary>
    /// The attack animation list
    /// </summary>
    private IList<string> FullAttackAnimation = new List<string>();

    public LayerMask EnemyLayer;

    public float MaxAnimationSpeed = 0.95f;
    public float MinAnimationSpeed = 0.5f;
    private float animationSpeed = 0;

    public float PowerAccelerationTime = 1.3333f;
    public float MaxStrikePower = 100f;
    [HideInInspector]
    public float strikePower = 0f;

    /// <summary>
    /// The total time in seconds to accelerate to max strike power
    /// The default value is 1.3333f, it takes 1.3333 seconds to achieve max strike power!
    /// </summary>
    public float TotalTimeOfAcceleration = 1.33f;

    public PrograssBar PowerPrograss = null;

    /// <summary>
    /// The default combat by tap or slice
    /// </summary>
    public Combat defaultCombatTap;
    public Combat defaultCombatSlice;
    /// <summary>
    /// The ComboCombat in prefab which is to be setup by designer.
    /// </summary>
	public ComboCombat[] ComboCombats;
    
    private CharacterController controller = null;
    
    public int AttackAnimationLayer = 3;

	public GameObject CombatHintHUD;
	
	private IList<GestureInfomation> UnprocessGestureList = new List<GestureInfomation>();
	private PredatorPlayerStatus predatorStatus = null;
	
	void InitComboCombat()
	{
		foreach(ComboCombat comboCombat in ComboCombats)
		{
			comboCombat.Init();
		}
	}
	
	void InitAttackAnimation()
	{
        foreach (string ani in Strike_SingleClaw)
        {
            FullAttackAnimation.Add(ani);
        }
		foreach (string ani in Strike_DualClaws)
        {
            FullAttackAnimation.Add(ani);
        }
        foreach (string ani in WavingClaw)
        {
            FullAttackAnimation.Add(ani);
        }
        foreach (string ani in ClampingClaws)
        {
            FullAttackAnimation.Add(ani);
        }
        foreach (string attackAnimation in FullAttackAnimation)
        {
            animation[attackAnimation].layer = AttackAnimationLayer;
        }
	}
	
    Vector3 dis;
    Quaternion qDis;
    void Awake()
    {
        InitAttackAnimation();
		InitComboCombat();
        controller = this.GetComponent<CharacterController>();
        predatorStatus = GetComponent<PredatorPlayerStatus>();
        dis = transform.rotation.eulerAngles - predatorStatus.body.rotation.eulerAngles;
        qDis = Quaternion.FromToRotation(predatorStatus.body.forward, transform.forward);
    }
	
	void Start()
	{
		StartCoroutine(RepeatCheckUserGesture());
	}
	
#region User gesture and combat token processing 
    void NewUserGesture(GestureInfomation gestureInfo)
    {
        UnprocessGestureList.Add(gestureInfo);
    }
	
    /// <summary>
    /// Damon routine, check & process user input at interval = 0.15 seconds(cooldown time)
    /// </summary>
    /// <returns></returns>
	IEnumerator RepeatCheckUserGesture()
	{
		//the combat token by user
		string combatToken = "";
        int tokenCount = 0;
		while(true)
		{
           if (UnprocessGestureList.Count > 0)
           {
               GestureInfomation gestureInfoToBeProcessed = UnprocessGestureList[0];
			   combatToken += ((int)gestureInfoToBeProcessed.Type).ToString();
				//New Hint in GUI (HUD)
               CombatHintHUD.SendMessage("NewHint", gestureInfoToBeProcessed.Type);
			   //Search the matched combo, and return the combat
               Combat combat = GetComboCombatByToken(combatToken, gestureInfoToBeProcessed.Type);
               //Process the combat
               yield return StartCoroutine(ProcessCombat(combat));
			   //StartCoroutine(ProcessUserGesture(gestureInfoToBeProcessed));

			   UnprocessGestureList.Remove(gestureInfoToBeProcessed);
               //Only support up to 4 combo combat
               if (combatToken.Length == 4)
               {
                   combatToken = "";
               }
		      //Debug.Log("Process gesture:" + GestureList.Count + " at time:" + Time.time);
		   }
		   yield return new WaitForSeconds(0.15f);
		}
	}

    IEnumerator ProcessCombat(Combat combat)
    {
        yield return null;
    }

	private GameObject lastTarget = null;
    IEnumerator ProcessUserGesture(GestureInfomation gesture)
	{
		Vector3? worldDirection = null;
		GameObject target = null;
        //Find for the target:
		if(gesture.gestureDirection.HasValue)
		{
			worldDirection = Util.GestureDirectionToWorldDirection(gesture.gestureDirection.Value);
			target = LookingforBestTarget(worldDirection);
		}
		else 
		{
			target = LookingforBestTarget(transform.forward);
		}
        //Process the gesture:
		switch(gesture.Type)
		{
		    case GestureType.Single_Tap:
			string single_spike_animation = Util.RandomFromArray(Strike_SingleClaw);
			DamageForm damageForm = DamageForm.Predator_Strike_Single_Claw;
			
			if(target && Util.DistanceOfCharacters(this.gameObject, target) >= ClawRadius)
			{
			   yield return StartCoroutine(RushTo(target.transform));
			}
			yield return StartCoroutine(Strike(single_spike_animation,damageForm,target,10));
			break;
		}
	}

    private Combat GetDefaultCombat(GestureType GestureType)
    {
        switch (GestureType)
        {
            case GestureType.Single_Slice:
                return defaultCombatSlice;
            case GestureType.Single_Tap:
            default:
                return defaultCombatTap;
        }
    }

	/// <summary>
	/// Gets the combat by token. 
    /// For example : there are two combo combat defined in prefab: tap + tap + tap + slice (1110) , slice + tap + tap + slice (1001)
	///               then parameter token = 111 = returned 1110
    ///               parameter token = 1001 = return 1001
    ///               parameter token = 101 = return default combat of gesture type = tap , because no prefab combat can be found
	/// </summary>
	private Combat GetComboCombatByToken(string token, GestureType gestureType)
	{
		ComboCombat comboCombat = null;
		foreach(ComboCombat _comboCombat in this.ComboCombats)
		{
			if(_comboCombat.token.StartsWith(token))
			{
				comboCombat = _comboCombat;
				break;
			}
		}
        //If no matched combo combat is found, return the default combat
		if(comboCombat==null) 
		{
            return GetDefaultCombat(gestureType);
		}
		else 
		{
            return comboCombat.combat[token.Length - 1];
		}
	}
#endregion
	
    void Update()
    {
        //if (Input.GetKeyDown("t"))
        //{
        //    Vector3 dir = transform.rotation * new Vector3(0, 0, -10);//transform.TransformDirection(0,0,1);
        //    //transform.rotation = newRotation;
        //    Debug.DrawLine(transform.position, transform.position + dir, Color.red, 5);
        //    Quaternion.LookRotation(dir).SetEulerAngles(Quaternion.LookRotation(dir).eulerAngles - dis);
        //    Vector3 newEuler = Quaternion.LookRotation(dir).eulerAngles - dis;
        //    predatorStatus.body.rotation = Quaternion.Euler(newEuler);
        //}
    }

    /// <summary>
    /// Crossfade PreAttack, accumluating power to perform a powerful strike!
    /// </summary>
    void StrikePowerUp()
    {
        string preattackAnim = (PredatorPlayerStatus.IsMoving) ? PreAttackAnimation_Mixed : PreAttackAnimation;
        strikePower = Mathf.Clamp((strikePower + (MaxStrikePower / PowerAccelerationTime) * Time.deltaTime), 0, MaxStrikePower);
        //Debug.Log("Strike power:" + strikePower);
        animation.CrossFade(preattackAnim);
    }



	/// <summary>
	/// Play strike aniamtion, and send hit message to target
	/// </summary>
	/// <param name="animationToPlay"></param>
	/// <param name="damageForm"></param>
	/// <param name="target"></param>
	/// <param name="hitPoint"></param>
	/// <returns></returns>
    IEnumerator Strike(string animationToPlay, DamageForm damageForm, GameObject target, float hitPoint)
    {
        float length = animation[animationToPlay].length;
		if(target != null)
		{
			transform.LookAt(target.transform);
		}
		animation.CrossFade(animationToPlay);
        //TimeLength1 = first part of strike animation
        //TimeLength2 = last part of strike animation
		float TimeLength1 = length * 0.5f;
		float TimeLength2 = length - TimeLength1;
		yield return new WaitForSeconds(TimeLength1);
        //Send hit message to target, after TimeLength1
		if(target != null)
		{
		  if(Util.DistanceOfCharacters(this.gameObject,target) <= ClawRadius)
		  {
			 SendHitMessage(damageForm, target, hitPoint);
		  }
		}
		yield return new WaitForSeconds(TimeLength2);
    }

    private IEnumerator RushTo(Transform target)
    {
		transform.LookAt(target);
        Vector3 direction = target.position - transform.position;
		float distance = Util.DistanceOfCharacters(this.gameObject, target.gameObject);
        float _start = Time.time;
        float timeLength = 0.3f;
        Vector3 Speed = direction / timeLength;
        while ((Time.time - _start) <= timeLength)
        {
            this.controller.SimpleMove(Speed);
            yield return null;
        }
    }

    private void SendHitMessage(DamageForm DamageForm, GameObject enemy, float HitPoint)
    {
        float distance = Util.DistanceOfCharacters(enemy, this.gameObject);
        if (distance <= ClawRadius)
        {
            DamageParameter damageParam = new DamageParameter(this.gameObject, DamageForm, HitPoint);
            enemy.SendMessage("ApplyDamage", damageParam);
        }
    }

    public bool IsPlayingAttack()
    {
        foreach (string attackAni in FullAttackAnimation)
        {
            if (animation.IsPlaying(attackAni))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Physics.CapsuleCastAll to detect enemy.
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private GameObject _FindEnemyAtDirection(Vector3 direction, float capsuleRadius, float SweepDistance)
    {
        float radius = controller.radius;
        Vector3 topPoint = controller.center + transform.position + Vector3.up * controller.height * 0.5f;
        Vector3 bottomPoint = topPoint - Vector3.down * controller.height;
        RaycastHit[] hits = Physics.CapsuleCastAll(topPoint, bottomPoint, capsuleRadius, direction, SweepDistance, EnemyLayer);
        if (hits != null && hits.Length > 0)
        {
            IList<Collider> colliderList;
            Util.CopyToList(hits, out colliderList);
            float distance = 0;
            Collider cloest = Util.findClosest(transform.position, colliderList, out distance);
            return cloest.gameObject;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Physics.OverlapSphere to detect enemy
    /// </summary>
    /// <param name="radius"></param>
    /// <returns></returns>
    private GameObject _FindEnemy(float radius)
    {
        GameObject ret = null;
        //Get attackable object
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, radius, EnemyLayer);
        if (colliders != null && colliders.Length > 0)
        {
            Collider c = Util.findClosest(Util.GetCharacterCenter(this.gameObject), colliders);
            ret = c.gameObject;
        }
        return ret;
    }
	
	/// <summary>
	/// Lookingfors the best target.
	/// If direction.HasValue = true, then use Physics.CapsuleCastAll to sweep in direction to detect enemy target
	/// Else if direction == null, then Physics.Oversphere to find enemy around
	/// </summary>
    GameObject LookingforBestTarget(Vector3? direction)
    {
        GameObject enemy = null;
        if (direction.HasValue)
        {
            float CapsuleRadius = controller.radius;
            enemy = _FindEnemyAtDirection(direction.Value, CapsuleRadius, OffenseRadiusNear);
        }
        if(enemy == null)
        {
            enemy = _FindEnemy(OffenseRadiusNear);
        }
        return enemy;
    }

    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, ClawRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, OffenseRadiusNear);
    }
}
