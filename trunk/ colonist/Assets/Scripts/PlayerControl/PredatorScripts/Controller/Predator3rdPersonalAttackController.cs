using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[@RequireComponent(typeof(PredatorPlayerStatus))]
[@RequireComponent(typeof(Predator3rdPersonalUnit))]
public class Predator3rdPersonalAttackController : MonoBehaviour
{
	/// <summary>
	/// If player not enter next combat within ComboCombatOperationTimeout, the current combo token will be erased.
	/// </summary>
	public float ComboCombatOperationTimeout = 1.5f;
	/// <summary>
	/// This is an add-hoc adjustment, for the unit in the air, the predator should not be able to attack them :)
	/// </summary>
	public float AttackableHeightOffset = 3;
	/// <summary>
	/// The default combat - left claw
	/// </summary>
	private PredatorCombatData DefaultCombat_LeftClaw;
	/// <summary>
	/// The default combat_ right claw.
	/// </summary>
	private PredatorCombatData DefaultCombat_RightClaw;
	/// <summary>
	/// The default combat_ dual claw.
	/// </summary>
	private PredatorCombatData DefaultCombat_DualClaw;
	/// <summary>
	/// The ComboCombat in prefab which is to be setup by designer.
	/// </summary>
	private PredatorComboCombat[] ComboCombats;

	/// <summary>
	/// if enemy farer than RushRadius, predator will fastly approaching the enemy.
	/// </summary>
	private float RushRadius;
	/// <summary>
	/// The gesture cool down time. 
	/// Gesture process routine check user gesture input at every %GestureCooldown% seconds.
	/// </summary>
	private float CombatCooldown;
	
	/// <summary>
	/// The current target.
	/// </summary>
	private GameObject CurrentTarget = null;
	
	/// <summary>
	/// Key = combo combat token
	/// Value = combo combat
	/// </summary>
	private Dictionary<string, PredatorComboCombat> ComboCombatTokenDict = new Dictionary<string, PredatorComboCombat> ();
	private IList<UserInputData> UnprocessGestureList = new List<UserInputData> ();
	private IList<PredatorCombatData> UnprocessCombatList = new List<PredatorCombatData> ();
	private PredatorPlayerStatus predatorStatus = null;
	private CharacterController controller = null;
	private LayerMask EnemyLayer;
	/// <summary>
	/// When BlockUserGestureInput = true, the new coming player gesture will be dropped, and GestureList will be cleared per frame.
	/// </summary>
	private bool BlockUserGestureInput = false;
	private Predator3rdPersonalUnit PredatorPlayerUnit;

	void Awake ()
	{
		controller = this.GetComponent<CharacterController> ();
		predatorStatus = GetComponent<PredatorPlayerStatus> ();
		PredatorPlayerUnit = GetComponent<Predator3rdPersonalUnit> ();
		EnemyLayer = PredatorPlayerUnit.EnemyLayer;
		RushRadius = PredatorPlayerUnit.RushRadius;
		CombatCooldown = PredatorPlayerUnit.CombatCoolDown;
        
		DefaultCombat_LeftClaw = PredatorPlayerUnit.DefaultCombat_LeftClaw;
		DefaultCombat_RightClaw = PredatorPlayerUnit.DefaultCombat_RightClaw;
		DefaultCombat_DualClaw = PredatorPlayerUnit.DefaultCombat_DualClaw;
		ComboCombats = PredatorPlayerUnit.ComboCombat;
		foreach (PredatorComboCombat comboCombat in ComboCombats) {
			comboCombat.Init ();
			ComboCombatTokenDict [comboCombat.token] = comboCombat;
		}
	}
	
	void Start ()
	{
//		StartCoroutine (RepeatCheckCombatList ());
	}

	
    void OnEnable() {
		StartCoroutine("RepeatCheckCombatList");
	}
	
	void OnDisable()
	{
		StopCoroutine("RepeatCheckCombatList");
	}
	
	void Update ()
	{
		//when BlockUserGestureInput = true, block the player's gesture input
		if (BlockUserGestureInput && UnprocessGestureList.Count > 0) {
			UnprocessGestureList.Clear ();
		}
		//Release user input block, if time expires
		if (BlockUserGestureInput && Time.time >= ReleaseUserInputBlockTime) {
			BlockUserGestureInput = false;
		}
		//If player do not append combat in 1 second, reset the playerComboToken and clear HUD hint
		if (((Time.time - LastProcessPlayerGestureInputTime) > ComboCombatOperationTimeout) && playerComboToken != string.Empty) {
			playerComboToken = "";
			PredatorPlayerUnit.HUDObject.SendMessage ("ClearHint");
		}
	}

#region User gesture and combat token processing
	/// <summary>
	/// The current being processed combat.
	/// </summary>
	private PredatorCombatData currentProcessCombat = null;
	string playerComboToken = "";
	/// <summary>
	/// Time vaiables, when current time greater than this value, the block should be removed.
	/// </summary>
	float ReleaseUserInputBlockTime = float.MaxValue;
	/// <summary>
	/// variable the last time of receiving player gesture input
	/// </summary>
	float LastProcessPlayerGestureInputTime = -1;
	/// <summary>
	/// Call by GestureHandler.
	/// 1. New HUD Hint
	/// 2. Search combo combat by playerComboToken
	/// 3. Determine the playerCombo can go ahead , or terminated(unmatched)
	/// Put a new user gesture into queue - UnprocessGestureList
	/// </summary>
	/// <param name="gestureInfo"></param>
	public void NewUserGesture (UserInputData gestureInfo)
	{
		if (!BlockUserGestureInput) {

			//Accumulate player combo token string
			playerComboToken += ((int)gestureInfo.Type).ToString ();
			UserInputType[] nextCombat = PredictNextMatchedCombo(playerComboToken);
			SendMessage("ShowButtonHints", nextCombat);
//			Debug.Log("Current playerComboToken:" + playerComboToken);
			
			//Check if player combo token has a matched prefab
			bool playerTokenMatched = false, isLastCombat = false;
			PredatorCombatData combat = GetComboCombatByToken (playerComboToken, gestureInfo.Type, out playerTokenMatched, out isLastCombat);
			if(combat == null)
			{
				return;
			}
			//If player combo token not matched, set BlockUserGestureInput = true to block all player input, 
			//until the combat processing finish, then BlockUserGestureInput will be set true again.
			//reset playerComboToken
			if (playerTokenMatched == false) {
				combat.FinalCombat = true;
				BlockUserGestureInput = true;
				playerComboToken = "";
			} else if (isLastCombat && playerTokenMatched) {
				combat.FinalCombat = true;
				playerComboToken = "";
			}
			//record the last receiving player gesture input time
			LastProcessPlayerGestureInputTime = Time.time;
			// UnprocessGestureList.Add(gestureInfo);
			UnprocessCombatList.Add (combat);
		}
		else 
		{
			Debug.Log("Input was dropped at frame:" + Time.frameCount);
		}
	}
	
	void ClearUnprocessCombatList()
	{
		UnprocessCombatList.Clear();
	}
	
	/// <summary>
	/// Given a current combo string token, predicts what player taps can perform the next matched combo.
	/// This function is used to hint player, that clicks what button can trigger the next matched combocombat.
	/// </summary>
	UserInputType[] PredictNextMatchedCombo(string currentToken)
	{
		List<UserInputType> nextUserInputType = new List<UserInputType>();
		foreach(PredatorComboCombat comboCombat in this.ComboCombats)
		{
			if(comboCombat.token.StartsWith(currentToken) && comboCombat.token.Length > currentToken.Length)
			{
				string nextCombatToken = comboCombat.token[currentToken.Length].ToString();
				UserInputType type = (UserInputType)Enum.Parse(typeof(UserInputType), nextCombatToken);
				nextUserInputType.Add(type);
//				Debug.Log("Found input:" + type.ToString());
			}
		}
		return nextUserInputType.ToArray();
	}

	/// <summary>
	/// Daemon  routine, check if UnprocessCombatList contains element, then processing it.
	/// </summary>
	/// <returns></returns>
	IEnumerator RepeatCheckCombatList ()
	{
		while (true) {
			if (UnprocessCombatList.Count > 0) {
				PredatorCombatData combat = UnprocessCombatList [0];
				UnprocessCombatList.Remove (combat);
				//New Hint in GUI (HUD)
			    PredatorPlayerUnit.HUDObject.SendMessage ("NewHint", combat.userInput);
				yield return StartCoroutine(DoCombat(combat));
				if (combat.FinalCombat) {
					BlockUserGestureInput = false;
					PredatorPlayerUnit.HUDObject.SendMessage ("ClearHint");
					SendMessage("DontHint");
				}
			}
			yield return null;
		}
	}



	/// <summary>
	/// Process a PredatorCombatData and perform attack behavior.
	/// </summary>
	/// <param name="combat"></param>
	/// <returns></returns>
	IEnumerator DoCombat (PredatorCombatData combat)
	{
		string useAttackDataName = Util.RandomFromArray<string>(combat.useAttackDataName);
		PredatorPlayerAttackData attackData = this.PredatorPlayerUnit.PredatorAttackDataDict[useAttackDataName];
		if(combat.OverrideAttackData)
		{
			attackData = attackData.GetClone() as PredatorPlayerAttackData;
			attackData.DamagePointBase = combat.DamagePointBase;
			attackData.MinDamageBonus = combat.MinDamagePointBonus;
			attackData.MaxDamageBonus = combat.MaxDamagePointBonus;
			attackData.CanDoCriticalAttack = combat.CanDoCriticalAttack;
			attackData.CriticalAttackBonusRate = combat.CriticalAttackBonusRate;
			attackData.CriticalAttackChance = combat.CriticalAttackChance;
		}
		currentProcessCombat = combat;
		//If combat.BlockUserInput = TRUE, user gesture input will be dropped during combat processing
		if (combat.BlockPlayerInput == true) {
			ReleaseUserInputBlockTime = Time.time + animation[attackData.AnimationName].length * 0.5f;
			BlockUserGestureInput = true;
		}
        
		//Look for enemy - in near radius
		float DistanceToEnemy = 0;
		CurrentTarget = FindTarget (transform.forward, RushRadius, out DistanceToEnemy);
		
//		Debug.Log(string.Format("Find the target:{0}, enemy distance:{1}, AttackableRange:{2}", CurrentTarget,DistanceToEnemy,attackData.AttackableRange) );
		
		if (CurrentTarget != null) {
			Util.RotateToward (transform, CurrentTarget.transform.position, false, 0);
			//if target farer than attack attackableRange, rush to the target.
			if (DistanceToEnemy > attackData.AttackableRange) {
				yield return StartCoroutine(RushTo (CurrentTarget.transform,0.3f));
			}
			//if attackData.hitTriggerType = ByTime, means the message should be sent N seconds after animation.
			//else if attackData.hitTriggerType = ByEvent, the message should be triggered by animation event.
			if(attackData.hitTriggerType == HitTriggerType.ByTime)
			{
			   StartCoroutine (SendHitMessage (attackData, CurrentTarget));
			}
		}
		animation.CrossFade (attackData.AnimationName);
		if(combat.FinalCombat)
		{
			Invoke("ClearUnprocessCombatList", animation[attackData.AnimationName].length * 0.5f);
		}
		if (combat.WaitUntilAnimationReturn) {
		    yield return new WaitForSeconds(animation[attackData.AnimationName].length);
		}
		else 
			yield return new WaitForSeconds(CombatCooldown);
	}
	
	void _Attack(string AttackDataName)
	{
		PredatorPlayerAttackData attackData = this.PredatorPlayerUnit.PredatorAttackDataDict[AttackDataName];
	    if(CurrentTarget != null && IsTargetDead(CurrentTarget) == false)
		{
			StartCoroutine (SendHitMessage (attackData, CurrentTarget));
		}
	}

	private GameObject lastTarget = null;

	/// <summary>
	/// Return the default left/right/dual claw combat.
	/// </summary>
	private PredatorCombatData GetDefaultCombat (UserInputType InputType)
	{
		switch (InputType) {
		case UserInputType.Button_Left_Claw_Tap:
		case UserInputType.Button_Left_Claw_Hold:
			return DefaultCombat_LeftClaw;
			
		case UserInputType.Button_Right_Claw_Tap:
		case UserInputType.Button_Right_Claw_Hold:
			return DefaultCombat_RightClaw;
			
		case UserInputType.Button_Dual_Claw_Hold:
		case UserInputType.Button_Dual_Claw_Tap:
			return DefaultCombat_DualClaw;
		default:
			return null;
		}
	}

	private bool IsPlayerComboTokenMatched (string playerComboToken)
	{
		foreach (string prefabComboTokenKey in ComboCombatTokenDict.Keys) {
			if (prefabComboTokenKey.StartsWith (playerComboToken) && prefabComboTokenKey.Length >= playerComboToken.Length) {
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
	private PredatorCombatData GetComboCombatByToken (string playerComboToken, UserInputType gestureType, out bool tokenMatched, out bool IsLastCombat)
	{
		PredatorComboCombat comboCombat = null;
		IsLastCombat = false;
		//Search matched prefab combo combat, by playerCombatToken 
		foreach (string prefabComboTokenKey in ComboCombatTokenDict.Keys) {
			if (prefabComboTokenKey.StartsWith (playerComboToken) && prefabComboTokenKey.Length >= playerComboToken.Length) {
				comboCombat = ComboCombatTokenDict [prefabComboTokenKey];
				IsLastCombat = prefabComboTokenKey.Length == playerComboToken.Length;
				break;
			}
		}
		//If no matched combo combat is found, return the default combat
		if (comboCombat == null) {
			tokenMatched = false;
			return GetDefaultCombat (gestureType);
		} else {
			tokenMatched = true;
			try{
			   return comboCombat.combat [playerComboToken.Length - 1];
			}
			catch(Exception exc)
			{
				Debug.LogError(exc);
				return null;
			}
		}
	}
#endregion


	/// <summary>
	/// Rush to a target, in %time%
	/// </summary>
	/// <param name="target"></param>
	/// <param name="time"></param>
	/// <returns></returns>
	public IEnumerator RushTo (Transform target, float time)
	{
		Vector3 direction = target.position - transform.position;
		Vector3 position = target.position;
		float distance = Util.DistanceOfCharacters (this.gameObject, target.gameObject) - 0.8f;
		float _start = Time.time;
		Vector3 velocity = direction.normalized * (distance / time);
		predatorStatus.DisableUserMovement = true;
		while ((Time.time - _start) <= time) {
			Util.RotateToward (transform, position, false, 0);
			this.controller.SimpleMove (velocity);
			yield return null;
		}
		predatorStatus.DisableUserMovement = false;
	}
	
	
	
    /// <summary>
	/// Given an AttackData, check by its HitTestType, 
	/// return true/false to indicate if AI has hit the target.
	/// </summary>
	bool CheckHitCondition (GameObject target, AttackData AttackData)
	{
		bool ShouldSendHitMessage = false;
		float TargetAngularDiscrepancy = 0, TargetDistance = 0;
		switch (AttackData.HitTestType) {
		case HitTestType.AlwaysTrue:
			ShouldSendHitMessage = true;
			break;
		case HitTestType.HitRate:
			float randomNumber = UnityEngine.Random.Range (0f, 1f);
			ShouldSendHitMessage = (randomNumber <= AttackData.HitRate);
			break;
		case HitTestType.CollisionTest:
			ShouldSendHitMessage = AttackData.HitTestCollider.bounds.Intersects (target.collider.bounds);
			break;
		case HitTestType.DistanceTest:
			TargetDistance = Util.DistanceOfCharacters (gameObject, CurrentTarget); 
			ShouldSendHitMessage = TargetDistance <= AttackData.HitTestDistance;
			break;
		case HitTestType.AngleTest:
			TargetAngularDiscrepancy = Util.Angle_XZ (transform.forward, (CurrentTarget.transform.position - transform.position).normalized);
			ShouldSendHitMessage = TargetAngularDiscrepancy <= AttackData.HitTestAngularDiscrepancy;
			break;
		case HitTestType.DistanceAndAngleTest:
			TargetDistance = Util.DistanceOfCharacters (gameObject, CurrentTarget.gameObject); 
//			    TargetAngularDiscrepancy = Vector3.Angle(transform.forward, (CurrentTarget.position - transform.position).normalized);
			TargetAngularDiscrepancy = Util.Angle_XZ (transform.forward, (CurrentTarget.transform.position - transform.position).normalized);
			ShouldSendHitMessage = TargetDistance <= AttackData.HitTestDistance && TargetAngularDiscrepancy <= AttackData.HitTestAngularDiscrepancy;
			break;
		}
		return ShouldSendHitMessage;
	}
	
	/// <summary>
	/// Sends the ApplyDamage message to enemy.
	/// </summary>
	public IEnumerator SendHitMessage (PredatorPlayerAttackData attackData, GameObject enemy)
	{
		if (enemy == null) {
			yield break;
		}
		//if attackData.hitTime > 0, wait for the hitTime.
		if (Mathf.Approximately (attackData.HitTime, 0) == false) {
			yield return new WaitForSeconds(attackData.HitTime);
		}
	    //in case the enemy object is destoryed between the previous yield time.
		if (enemy == null) 
		{
			yield break;
		}
		//if attack type = Instant, means there is only one target to be attacked.
	    if(attackData.Type == AIAttackType.Instant)
		{
		    if(CheckHitCondition(enemy, attackData)) 
			{
			   //send applyDamage to target.
			   DamageParameter damageParam = GetDamageParameter(attackData);
			   enemy.SendMessage ("ApplyDamage", damageParam);
               
			   //at each hit, plus the rage
			   this.PredatorPlayerUnit.Rage = Mathf.Clamp(this.PredatorPlayerUnit.Rage + this.PredatorPlayerUnit.RageEarnPerHit, 0, this.PredatorPlayerUnit.MaxRage);
				
			   //send GameEvent to HUD to display the damage text.
			   GameEvent _e = new GameEvent(GameEventType.DisplayDamageParameterOnNPC);
			   _e.receiver = enemy;
			   _e.ObjectParameter = damageParam;
			   PredatorPlayerUnit.HUDObject.SendMessage("OnGameEvent", _e);
		    }
		}
		else if(attackData.Type == AIAttackType.Regional)
		{
			Collider[] colliders = Physics.OverlapSphere(transform.position, this.RushRadius, this.EnemyLayer);
			foreach(Collider collider in colliders)
			{
				if(attackData.HitTestCollider.bounds.Intersects(collider.bounds))
				{
			       //send applyDamage to every object intersect with the hitTestCollider.
			       DamageParameter damageParam = GetDamageParameter(attackData);
			       collider.gameObject.SendMessage ("ApplyDamage", damageParam);
//				   this.SendMessage("AddRage", damageParam);
			       //send GameEvent to HUD to display the damage text.
			       GameEvent _e = new GameEvent(GameEventType.DisplayDamageParameterOnNPC);
			       _e.receiver = collider.gameObject;
			       _e.ObjectParameter = damageParam;
			       PredatorPlayerUnit.HUDObject.SendMessage("OnGameEvent", _e);
				}
			}
		}
	}

	public bool IsPlayingAttack ()
	{
		foreach (string attackAni in PredatorPlayerUnit.AttackAnimations) {
			if (animation.IsPlaying (attackAni))
				return true;
		}
		return false;
	}

	/// <summary>
	/// Physics.CapsuleCastAll to detect enemy at designed direction.
	/// And return the cloest enemy.
	/// </summary>
	/// <param name="radius"></param>
	/// <param name="direction"></param>
	/// <returns></returns>
	private GameObject _FindEnemyAtDirection (Vector3 direction, float SweepDistance, out float distance)
	{
		distance = 0;
		float CapsuleRadius = controller.radius;
		Vector3 topPoint = controller.center + transform.position + Vector3.up * controller.height * 0.5f;
		Vector3 bottomPoint = topPoint - Vector3.down * controller.height;
		RaycastHit[] hits = Physics.CapsuleCastAll (topPoint, bottomPoint, CapsuleRadius, direction, SweepDistance, EnemyLayer);
		if (hits != null && hits.Length > 0) {
			GameObject[] enemies = hits.Select(x=>x.collider.gameObject).ToArray<GameObject>();
			//Filter out the unattackable targets.
			GameObject[] validTargets = enemies.Where(x=> this.IsTargetAttackable(x) == true).ToArray();
			if(validTargets != null && validTargets.Length > 0)
			{
			   GameObject closetEnemy = Util.FindClosestCharacter(this.gameObject, validTargets, out distance);
			   return closetEnemy;
			}
			else 
			{
				return null;
			}
		} else {
			return null;
		}
	}

    /// <summary>
    /// Lookingfors the best target, and set the varaiable CurrentTarget.
    /// 1. Make a capsule cast at forward direction, if there is enemy, select the closest as current target.
    /// 2. If there is no enemy swapped by capsule cast at #1, check the previous target, if the target falls inside RushToRadius, use the previous target as current target. 
    /// 3. If no target is found at #2, Physics.ShereOverlapped to find all enemy around, if enemy found, return the closest.
    /// 4. If no target is found at #3, return null.
    /// </summary>
    /// <returns>
    /// The target.
    /// </returns>
	public GameObject FindTarget (Vector3 DetectDirection, float RushToRadius, out float distance)
	{
		GameObject NextTarget = null;
		//#1 - capsule cast at DetectDirection
		NextTarget = _FindEnemyAtDirection(transform.forward, RushToRadius, out distance);
		//#2 - check if previous target falls inside RushToRadius
		if(NextTarget == null)
		{
			if(CurrentTarget != null && IsTargetDead(CurrentTarget) == false && 
				((distance = Util.DistanceOfCharacters(this.gameObject, CurrentTarget)) <= RushToRadius) && IsTargetAttackable(CurrentTarget) == true)
			{
				NextTarget = CurrentTarget;
			}
		}
		//#3 - Physics.Overlapped to find all enemy,
		if(NextTarget == null)
		{
			Collider[] colliders = Physics.OverlapSphere(this.transform.position, RushToRadius, EnemyLayer);
			if(colliders != null && colliders.Length > 0)
			{
			   GameObject[] allTargets = colliders.Select(x=>x.gameObject).ToArray();
			   GameObject[] allAttackableTargets = allTargets.Where (x=> IsTargetAttackable(x)).ToArray();
			   if(allAttackableTargets != null && allAttackableTargets.Length > 0)
			   {
			      NextTarget = Util.FindClosest(this.transform.position, allAttackableTargets, out distance);
			   }
			}
		}
		
		return NextTarget;
	}
	
	public DamageParameter GetDamageParameter(PredatorPlayerAttackData attackData)
	{
		DamageParameter dp = new DamageParameter(this.gameObject, attackData.DamageForm, 0);
		UnityEngine.Random.seed = DateTime.Now.Millisecond;
		//calculate damage point = base point + random(min, max)
		dp.damagePoint = attackData.DamagePointBase + UnityEngine.Random.Range(attackData.MinDamageBonus, attackData.MaxDamageBonus);
		//if critical attack is enabled and random chance matches, multiple the bonus rate.
		//Usually, designer can put the critical attack chance to the final combat of a combo-combat, this is like the great-final hit.
		if(attackData.CanDoCriticalAttack && UnityEngine.Random.Range(0f,1f) <= attackData.CriticalAttackChance)
		{
			dp.damagePoint *= attackData.CriticalAttackBonusRate;
			dp.extraParameter.Add(DamageParameter.ExtraParameterKey.IsCriticalStrike, true);
			dp.extraParameter.Add(DamageParameter.ExtraParameterKey.CritcialStrikeRate, attackData.CriticalAttackBonusRate);
		}
		return dp;
	}
	
    /// <summary>
    /// Determines whether the target is dead:
    /// 1. character controller . enabled = false
    /// 2. unit hp lesser than 0
    /// </summary>
	bool IsTargetDead(GameObject gameObject)
	{
		bool isDead = false;
		if(gameObject.GetComponent<UnitBase>() != null && gameObject.GetComponent<UnitBase>().GetCurrentHP()<=0)
		{
			isDead = true;
		}
		return isDead;
	}
	
	bool IsTargetAttackable(GameObject gameObject)
	{
		bool isAttackable = true;
		if(gameObject.GetComponent<UnitBase>() != null && gameObject.GetComponent<UnitBase>().IsUnitAttackable() == false)
		{
			isAttackable = false;
		}
		//don't attack the unit in the air :)
		if(Mathf.Abs(transform.position.y - gameObject.transform.position.y) > this.AttackableHeightOffset)
		{
			isAttackable = false;
		}
		return isAttackable;
	}
}
