using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[@RequireComponent(typeof (PredatorPlayerStatus))]
public class Predator3rdPersonalAttackController : MonoBehaviour {

    /// <summary>
    /// The length the claw can reach to
    /// </summary>
    public float ClawRadius = 3f;

    public float OffenseRadiusNear = 6;

    public string PreAttackAnimation = "preattack_right";

    public string PreAttackAnimation_Mixed = "preattack_right_mix";

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

    public float PowerAccelerationTime = 1.3333f;

    /// <summary>
    /// The gesture cool down time. Gesture process routine check user gesture input at every %GestureCooldown% seconds.
    /// </summary>
    public float CombatCooldown = 0.15f;
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
    public int AttackAnimationLayer = 3;
	public GameObject CombatHintHUD;

    public float MaxStrikePower = 100f;
    [HideInInspector]
    public float strikePower = 0f;

    /// <summary>
    /// Key = combo combat token
    /// Value = combo combat
    /// </summary>
    private Dictionary<string, ComboCombat> ComboCombatTokenDict = new Dictionary<string, ComboCombat>();
	private IList<GestureInfomation> UnprocessGestureList = new List<GestureInfomation>();
    private IList<Combat> UnprocessCombatList = new List<Combat>();
	private PredatorPlayerStatus predatorStatus = null;
    private CharacterController controller = null;
    /// <summary>
    /// When BlockUserGestureInput = true, the new coming player gesture will be dropped, and GestureList will be cleared per frame.
    /// </summary>
    private bool BlockUserGestureInput = false;
    
	void InitComboCombat()
	{
		foreach(ComboCombat comboCombat in ComboCombats)
		{
			comboCombat.Init();
            ComboCombatTokenDict[comboCombat.token] = comboCombat;
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
		//StartCoroutine(RepeatCheckUserGesture());
        StartCoroutine(RepeatCheckCombatList());
	}

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
        //when BlockUserGestureInput = true, block the player's gesture input
        if (BlockUserGestureInput && UnprocessGestureList.Count > 0)
        {
            UnprocessGestureList.Clear();
        }
        //In case application quit abnormally that the player could never gain control again, 
        //hereby it's necessary to reflag the BlockUserGestureInput to false
        if (BlockUserGestureInput && (Time.time - LastBlockPlayerGestureInputTime) >= this.CombatCooldown * 1.5f)
        {
            BlockUserGestureInput = false;
        }
        //If player do not append combat in 1 second, reset the playerComboToken and clear HUD hint
        if( ((Time.time - LastProcessPlayerGestureInputTime) > 1f) && playerComboToken != string.Empty)
        {
            playerComboToken = "";
            CombatHintHUD.SendMessage("ClearHint");
        }
    }

#region User gesture and combat token processing

    string playerComboToken = "";
    /// <summary>
    /// variable the last time of blocking player gesture input
    /// </summary>
    float LastBlockPlayerGestureInputTime = -1;
    /// <summary>
    /// variable the last time of receiving player gesture input
    /// </summary>
    float LastProcessPlayerGestureInputTime = -1;
    /// <summary>
    /// Call by GestureHandler.
    /// 1. New HUD Hint
    /// 2. Search combo combat by playerComboToken
    /// 3. Determine the playerCombo can go ahead , or terminated(unmatched)
    /// Push a new user gesture into queue - UnprocessGestureList
    /// </summary>
    /// <param name="gestureInfo"></param>
    void NewUserGesture(GestureInfomation gestureInfo)
    {
        if (!BlockUserGestureInput)
        {
            //New Hint in GUI (HUD)
            CombatHintHUD.SendMessage("NewHint", gestureInfo.Type);
            //Accumulate player combo token string
            playerComboToken += ((int)gestureInfo.Type).ToString();
            //Check if player combo token has a matched prefab
            bool playerTokenMatched = false, isLastCombat = false;
            Combat combat = GetComboCombatByToken(playerComboToken, gestureInfo.Type, out playerTokenMatched, out isLastCombat);

            //If player combo token not matched, set BlockUserGestureInput = true to block all player input, 
            //until the combat processing finish, then BlockUserGestureInput will be set true again.
            //reset playerComboToken
            if (playerTokenMatched == false)
            {
               combat.FinalCombat = true;
               LastBlockPlayerGestureInputTime = Time.time;
               BlockUserGestureInput = true;
               playerComboToken = "";
            }
            else if (isLastCombat && playerTokenMatched)
            {
                combat.FinalCombat = true;
                playerComboToken = "";
            }
            //record the last receiving player gesture input time
            LastProcessPlayerGestureInputTime = Time.time;
           // UnprocessGestureList.Add(gestureInfo);
            UnprocessCombatList.Add(combat);
        }
    }

    /// <summary>
    /// Daemon  routine, check if UnprocessCombatList contains element, then processing it.
    /// </summary>
    /// <returns></returns>
    IEnumerator RepeatCheckCombatList()
    {
        while (true)
        {
            if (UnprocessCombatList.Count > 0)
            {
                Combat combat = UnprocessCombatList[0];
                UnprocessCombatList.Remove(combat);
                if (combat.specialCombatFunction != string.Empty)
                {
                    yield return StartCoroutine(combat.specialCombatFunction, combat);
                }
                else
                {
                    yield return StartCoroutine(DefaultCombat(combat));
                }
                if (combat.FinalCombat)
                {
                    BlockUserGestureInput = false;
                    CombatHintHUD.SendMessage("ClearHint");
                }
            }
            yield return new WaitForSeconds(CombatCooldown);
        }
        
    }

    


    /// <summary>
    /// The default routine to process combat
    /// </summary>
    /// <param name="combat"></param>
    /// <returns></returns>
    IEnumerator DefaultCombat(Combat combat)
    {
        string attackAnimation = Util.RandomFromArray(combat.specialAnimation);
        //If combat.BlockUserInput = TRUE, user gesture input will be dropped during combat processing
        if (combat.BlockPlayerInput == true)
        {
            LastBlockPlayerGestureInputTime = Time.time;
            BlockUserGestureInput = combat.BlockPlayerInput;
        }
        
        //Look for enemy - in near radius
        float DistanceToEnemy = 0;
        GameObject target = LookforBestTarget(null, OffenseRadiusNear, out DistanceToEnemy);
        if (target != null)
        {
            Util.RotateToward(transform, target.transform.position, false, 0);
            if (DistanceToEnemy > ClawRadius)
            {
                yield return StartCoroutine(RushTo(target.transform,0.3f));
            }
            StartCoroutine(SendHitMessage(combat.damageForm, target, 10, animation[attackAnimation].length / 2));
        }
        animation.CrossFade(attackAnimation);
        //Send hit message in 1/2 time of animation
        //If the WaitUntilAnimationReturn = TRUE, then wait for animation over
        if (combat.WaitUntilAnimationReturn)
        {
            yield return new WaitForSeconds(animation[attackAnimation].length);
        }
        else
        {
            //re-accept user gesture input 
            yield return null;
        }
    }

	private GameObject lastTarget = null;
    /// <summary>
    /// Deprecated.
    /// </summary>
    /// <param name="gesture"></param>
    /// <returns></returns>
    //IEnumerator ProcessUserGesture(GestureInfomation gesture)
    //{
    //    Vector3? worldDirection = null;
    //    GameObject target = null;
    //    //Find for the target:
    //    if(gesture.gestureDirection.HasValue)
    //    {
    //        worldDirection = Util.GestureDirectionToWorldDirection(gesture.gestureDirection.Value);
    //        target = LookforBestTarget(worldDirection);
    //    }
    //    else 
    //    {
    //        target = LookforBestTarget(transform.forward);
    //    }
    //    //Process the gesture:
    //    switch(gesture.Type)
    //    {
    //        case GestureType.Single_Tap:
    //        string single_spike_animation = Util.RandomFromArray(Strike_SingleClaw);
    //        DamageForm damageForm = DamageForm.Predator_Strike_Single_Claw;
			
    //        if(target && Util.DistanceOfCharacters(this.gameObject, target) >= ClawRadius)
    //        {
    //           yield return StartCoroutine(RushTo(target.transform));
    //        }
    //        yield return StartCoroutine(Strike(single_spike_animation,damageForm,target,10));
    //        break;
    //    }
    //}

    /// <summary>
    /// Return the default tap/slice combat.
    /// </summary>
    /// <param name="GestureType"></param>
    /// <returns></returns>
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

    private bool IsPlayerComboTokenMatched(string playerComboToken)
    {
        foreach (string prefabComboTokenKey in ComboCombatTokenDict.Keys)
        {
            if (prefabComboTokenKey.StartsWith(playerComboToken) && prefabComboTokenKey.Length >= playerComboToken.Length)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Gets the combat by token. If no prefab matched comboCombat can be found, return default combat.
    /// For example : there are two combo combat defined in prefab: tap + tap + tap + slice (1110) , slice + tap + tap + slice (1001)
    ///               then parameter token = 111 = returned 1110
    ///               parameter token = 1001 = return 1001
    ///               parameter token = 101 = return default combat of gesture type = tap , because no prefab combat can be found
    /// </summary>
    /// <param name="playerComboToken">the player combo token</param>
    /// <param name="gestureType">gesture type</param>
    /// <param name="tokenMatched">output, indicate if the token matched to a prefab combat</param>
    /// <param name="IsLastCombat">output, indicated if the matched combat is the last combat in combo</param>
    /// <returns></returns>
	private Combat GetComboCombatByToken(string playerComboToken, GestureType gestureType, out bool tokenMatched, out bool IsLastCombat)
	{
		ComboCombat comboCombat = null;
        IsLastCombat = false;
        //Search matched prefab combo combat, by playerCombatToken 
        foreach (string prefabComboTokenKey in ComboCombatTokenDict.Keys)
        {
            if (prefabComboTokenKey.StartsWith(playerComboToken) && prefabComboTokenKey.Length >= playerComboToken.Length)
            {
                comboCombat = ComboCombatTokenDict[prefabComboTokenKey];
                IsLastCombat = prefabComboTokenKey.Length == playerComboToken.Length;
                break;
            }
        }
        //If no matched combo combat is found, return the default combat
		if(comboCombat==null) 
		{
            tokenMatched = false;
            return GetDefaultCombat(gestureType);
		}
		else 
		{
            tokenMatched = true;
            return comboCombat.combat[playerComboToken.Length - 1];
		}
	}
#endregion

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
    /// Rush to a target, in %time%
    /// </summary>
    /// <param name="target"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator RushTo(Transform target, float time)
    {
        Vector3 direction = target.position - transform.position;
        Vector3 position = target.position;
		float distance = Util.DistanceOfCharacters(this.gameObject, target.gameObject);
        float _start = Time.time;
        Vector3 Speed = direction / time;
        predatorStatus.DisableUserMovement = true;
        while ((Time.time - _start) <= time)
        {
            Util.RotateToward(transform, position, false, 0);
            this.controller.SimpleMove(Speed);
            yield return null;
        }
        predatorStatus.DisableUserMovement = false;
    }

    private IEnumerator SendHitMessage(DamageForm DamageForm, GameObject enemy, float HitPoint, float lagTime)
    {
        if (enemy == null)
        {
            yield break;
        }
        if (Mathf.Approximately(lagTime, 0) == false)
        {
            yield return new WaitForSeconds(lagTime);
        }
        float distance = Util.DistanceOfCharacters(enemy, this.gameObject);
        if (distance <= ClawRadius)
        {
            DamageParameter damageParam = new DamageParameter(this.gameObject, DamageForm, HitPoint);
            try
            {
                enemy.SendMessage("ApplyDamage", damageParam);
            }
            catch (Exception exc)
            {
                Debug.LogError(exc.Message + " object receiver:" + enemy.name);
                
            }
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
    private GameObject _FindEnemyAtDirection(Vector3 direction, float capsuleRadius, float SweepDistance, out float distance)
    {
        float radius = controller.radius;
        Vector3 topPoint = controller.center + transform.position + Vector3.up * controller.height * 0.5f;
        Vector3 bottomPoint = topPoint - Vector3.down * controller.height;
        RaycastHit[] hits = Physics.CapsuleCastAll(topPoint, bottomPoint, capsuleRadius, direction, SweepDistance, EnemyLayer);
        if (hits != null && hits.Length > 0)
        {
            IList<Collider> colliderList;
            Util.CopyToList(hits, out colliderList);
            Collider cloest = Util.findClosest(transform.position, colliderList, out distance);
            return cloest.gameObject;
        }
        else
        {
            distance = 0;
            return null;
        }
    }

    /// <summary>
    /// Physics.OverlapSphere to detect enemy
    /// </summary>
    /// <param name="radius"></param>
    /// <returns></returns>
    private GameObject _FindEnemy(float radius, out float distance)
    {
        GameObject ret = null;
        //Get attackable object
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, radius, EnemyLayer);
        distance = 0;
        if (colliders != null && colliders.Length > 0)
        {
            Collider c = Util.findClosest(Util.GetCharacterCenter(this.gameObject), colliders, out distance);
            ret = c.gameObject;
        }
        return ret;
    }
	
/// <summary>
/// Lookingfors the best target.
/// If direction.HasValue = true, then use Physics.CapsuleCastAll to sweep in direction to detect enemy target
/// Else if direction == null, then Physics.Oversphere to find enemy around
/// </summary>
/// <param name="direction"></param>
/// <param name="Radius"></param>
/// <param name="distance">out - the distance to target, 0 if target not found</param>
/// <returns></returns>
    GameObject LookforBestTarget(Vector3? direction, float Radius, out float distance)
    {
        GameObject enemy = null;
        distance = 0;
        if (direction.HasValue)
        {
            float CapsuleRadius = controller.radius;
            enemy = _FindEnemyAtDirection(direction.Value, CapsuleRadius, Radius, out distance);
        }
        if(enemy == null)
        {
            enemy = _FindEnemy(Radius, out distance);
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
