using UnityEngine;
using System.Collections;



/// <summary>
/// Defines basic AI functions. eg : 
/// 1. Move forward/backward for a short while
/// 2. Attack current target.
/// </summary>
public class AIFunction : MonoBehaviour
{
	/// <summary>
	/// Define 3 curves for speed.
	/// The time = moving time
	/// The value = moving speed
	/// </summary>
	public AnimationCurve SpeedCurve_1 = null;
	public AnimationCurve SpeedCurve_2 = null;
	public AnimationCurve SpeedCurve_3 = null;
	
	public float movebackspeed = 1, moveForwardSpeed = 1;
	CharacterController controller = null;
	bool backward = false;
	bool forward = false;
	float startBackwardTime = 0;
	float startForwardTime = 0;
	float stopBackwardTime = 0;
	float stopForwardTime = 0;
	Unit unit;
	public float MoveDistanceLimitation = 8;
	public float MoveOffset = -1;
	public float RotationSpeed = 15;
	
	/// <summary>
	/// if RotateToCurrentTarget = true, the character will rotate smoothly to face to the current target.
	/// </summary>
	bool RotateToCurrentTarget = false;
	float StopRotationTime = 0;
	
	/// <summary>
	/// When current Time > RevertReceiveDamageStatusTime, should revert the 
	/// unit's ReceiveDamageStatus to original : vulnerable
	/// </summary>
	float RevertReceiveDamageStatusTime = 0;
	
	Transform CurrentTarget {
		get {
			return this.unit.CurrentTarget;
		}
	}
	
	/// <summary>
	/// The current used speed curve.
	/// </summary>
	AnimationCurve CurrentSpeedCurve = null;
	
	void Awake ()
	{
		this.unit = GetComponent<Unit> ();
		controller = GetComponent<CharacterController> ();
	}
	
	void Update ()
	{
		if (backward && Time.time >= stopBackwardTime) {
			backward = false;
			CurrentSpeedCurve = null;
		}
		if (backward) {
			
			if(CurrentSpeedCurve != null)
			{
			   float CurrentTime = Time.time - this.startBackwardTime;
			   float CurrentMovebackSpeed = CurrentSpeedCurve.Evaluate(CurrentTime);
			   controller.SimpleMove (transform.TransformDirection (Vector3.back) * CurrentMovebackSpeed);
			}
			else {
			   controller.SimpleMove (transform.TransformDirection (Vector3.back) * movebackspeed);
			}
		}
		if (forward && Time.time >= stopForwardTime) {
			forward = false;
			CurrentSpeedCurve = null;
		}
		if (forward) {
			if(CurrentSpeedCurve != null)
			{
			   float CurrentTime = Time.time - this.startForwardTime;
			   float CurrentMoveforwardSpeed = CurrentSpeedCurve.Evaluate(CurrentTime);
			   controller.SimpleMove (transform.forward * CurrentMoveforwardSpeed);
			}
			else {
				controller.SimpleMove (transform.forward * moveForwardSpeed);
			}
			
		}
		
		if(RotateToCurrentTarget && CurrentTarget != null)
		{
			Util.RotateToward(this.transform, CurrentTarget.position, true, RotationSpeed);
		}
		if(RotateToCurrentTarget && Time.time >= StopRotationTime)
		{
			RotateToCurrentTarget = false;
		}
		
		if(this.unit != null && 
		   this.unit.receiveDamageStatus != UnitReceiveDamageStatus.vulnerable && 
		   Time.time > RevertReceiveDamageStatusTime)
		{
			this.unit.receiveDamageStatus = UnitReceiveDamageStatus.vulnerable;
		}
	}
	
	public void _Moveforward (float duration)
	{
		forward = true;
		stopForwardTime = Time.time + duration;
	}
	
	public void _Moveback (float duration)
	{
		backward = true;
		stopBackwardTime = Time.time + duration;
	}
	
	/// <summary>
	/// _Call this function to issue an attack behavior.
	///  Base on attack type.
	/// </summary>
	public IEnumerator _Attack (string AttackDataName)
	{
		AttackData attackData = unit.AttackDataDict [AttackDataName];
		switch (attackData.hitTriggerType) {
		//if hitTriggerType = ByTime, wait for specified time then start hit testing
		case HitTriggerType.ByTime:
			if(attackData.HitTime > 0)
			   yield return new WaitForSeconds(attackData.HitTime);
			break;
		case HitTriggerType.ByAnimationEvent:
			break;
		}
		
		//Start hit testing, and accumulate attack counter:
		this.unit.AttackCounter ++;
		switch (attackData.Type) {
		case AIAttackType.Instant:
			AttackSingle (this.unit.CurrentTarget.gameObject, attackData);
			break;
		case AIAttackType.Projectile:
			CreateProjectile (attackData);
			break;
		//Regional attack is bit special, because Regional attack could hurts
		//more than one enemy(not just the CurrentTarget) as long as enemy intersect
		//with the HitTestCollider!
		case AIAttackType.Regional:
                    //scan enemy inside the HitTestDistance range:
			foreach (Collider c in Physics.OverlapSphere(transform.position, attackData.HitTestDistance, unit.EnemyLayer)) {
				AttackSingle (c.gameObject, attackData);
			}
			break;
		default:
			Debug.LogError ("Unsupported attack type:" + attackData.Type.ToString () + " at object:" + gameObject.name);
			break;
		}
	}
	
    
	/// <summary>
	/// Attacks a single target
	/// </summary>
	/// <param name='target'>
	/// Target.
	/// </param>
	/// <param name='attackData'>
	/// Attack data.
	/// </param>
	void AttackSingle (GameObject target, AttackData attackData)
	{
		bool ShouldSendHitMessage = CheckHitCondition (target, attackData);
//		Debug.Log ("Check hit condition result:" + ShouldSendHitMessage);
		if (ShouldSendHitMessage) {
			target.SendMessage ("ApplyDamage", attackData.GetDamageParameter (this.gameObject));
			//accumulate the doDamageCounter by 1
			this.unit.DoDamageCounter ++;
			if(this.unit.currentWeapon != null)
			{
				this.unit.currentWeapon.CreateHitEffect(target);
			}
		}
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
			float randomNumber = Random.Range (0f, 1f);
			ShouldSendHitMessage = (randomNumber <= AttackData.HitRate);
			break;
		case HitTestType.CollisionTest:
			ShouldSendHitMessage = AttackData.HitTestCollider.bounds.Intersects (target.collider.bounds);
			break;
		case HitTestType.DistanceTest:
			TargetDistance = Util.DistanceOfCharacters (gameObject, CurrentTarget.gameObject); 
			ShouldSendHitMessage = TargetDistance <= AttackData.HitTestDistance;
			break;
		case HitTestType.AngleTest:
			TargetAngularDiscrepancy = Util.Angle_XZ (transform.forward, (CurrentTarget.position - transform.position).normalized);
			ShouldSendHitMessage = TargetAngularDiscrepancy <= AttackData.HitTestAngularDiscrepancy;
			break;
		case HitTestType.DistanceAndAngleTest:
			TargetDistance = Util.DistanceOfCharacters (gameObject, CurrentTarget.gameObject); 
//			    TargetAngularDiscrepancy = Vector3.Angle(transform.forward, (CurrentTarget.position - transform.position).normalized);
			TargetAngularDiscrepancy = Util.Angle_XZ (transform.forward, (CurrentTarget.position - transform.position).normalized);
			ShouldSendHitMessage = TargetDistance <= AttackData.HitTestDistance && TargetAngularDiscrepancy <= AttackData.HitTestAngularDiscrepancy;
			break;
		}
		return ShouldSendHitMessage;
	}
	
	/// <summary>
	/// Creates the projectile.
	/// </summary>
	void CreateProjectile (AttackData attackData)
	{
		GameObject projectile = (GameObject)Object.Instantiate (attackData.Projectile.gameObject, attackData.ProjectileInstantiateAnchor.position, attackData.ProjectileInstantiateAnchor.rotation);
		projectile.GetComponent<Projectile> ().Src = gameObject;
		projectile.GetComponent<Projectile> ().Target = CurrentTarget.gameObject;
		projectile.GetComponent<Projectile> ().DamageParameter = attackData.GetDamageParameter (gameObject);
	}
	
	/// <summary>
	/// move to current target in %duration% seconds.
	/// Note: this function is very simple, if you're looking for a more controllable one, you should use AStarNavigator._NavtigateToCurrentTarget
	/// </summary>
	IEnumerator _MoveToCurrentTarget (float _duration)
	{
		CharacterController controller = this.GetComponent<CharacterController> ();
		float distance = Util.DistanceOfCharacters (this.gameObject, this.unit.CurrentTarget.gameObject) + MoveOffset;
		distance = Mathf.Clamp (distance, 0, MoveDistanceLimitation);
		Vector3 direction = (this.unit.CurrentTarget.position - transform.position).normalized;
		Vector3 velocity = direction * distance / _duration;
		float _start = Time.time;
		while ((Time.time - _start) < _duration) {
			controller.SimpleMove (velocity);
			yield return null;
		}
	}
	/// <summary>
	/// rotate to face the current target immediately.
	/// </summary>
	void _FaceToCurrentTarget ()
	{
		Vector3 targetPos_XZ = new Vector3 (this.unit.CurrentTarget.position.x, transform.position.y, this.unit.CurrentTarget.position.z);
		transform.LookAt (targetPos_XZ);
	}
	
	/// <summary>
	/// with the back face to current target.
	/// </summary>
	void _BackToCurrentTarget()
	{ 
		Vector3 targetPos_XZ = new Vector3 (this.unit.CurrentTarget.position.x, transform.position.y, this.unit.CurrentTarget.position.z);
		Vector3 direction = transform.position - targetPos_XZ;
		transform.LookAt( transform.position + direction * 5);
	}
	
	/// <summary>
	/// rotate smoothly to face to current target.
	/// </summary>
	void _RotateToCurrentTarget(float rotateTimeLength)
	{
		RotateToCurrentTarget= true;
		StopRotationTime = Time.time + rotateTimeLength;
	}
	
	/// <summary>
	/// Create a effect object.
	/// Note: the %name% MUST BE an effective name in the key set of Unit.EffectDataDict
	/// </summary>
	public void _CreateEffect (string name)
	{
		try{
		   EffectData effectdata = this.unit.EffectDataDict [name];
	       GlobalBloodEffectDecalSystem.CreateEffect(effectdata);
		}
		catch(System.Collections.Generic.KeyNotFoundException exc)
		{
		   Debug.LogError("Key not found exception for EffectData name:" + name);
		}
	}
	
	/// <summary>
	/// _s the ignore collision.
	/// </summary>
	public IEnumerator _IgnoreCollision(float time)
	{
		this.controller.detectCollisions = false;
		yield return new WaitForSeconds(time);
		this.controller.detectCollisions = true;
	}
	
	/// <summary>
	/// Stop a effectData.
	/// For now, only a EffectData where instantiateType = Play can be stopped.
	/// </summary>
	public void _StopEffect (string name)
	{
		EffectData effectdata = this.unit.EffectDataDict [name];
		if(effectdata.InstantionType == EffectObjectInstantiation.play)
		{
			effectdata.EffectObject.GetComponent<ParticleSystem>().Stop();
		}
	}
	
	public void _MoveBackAtCurve(int curveIndex)
	{
		switch(curveIndex)
		{
		case 0:
			CurrentSpeedCurve = this.SpeedCurve_1;
			break;
		case 1:
			CurrentSpeedCurve = this.SpeedCurve_2;
			break;
		case 2:
		default:
			CurrentSpeedCurve = this.SpeedCurve_3;
			break;
		}
		backward = true;
		startBackwardTime = Time.time;
		stopBackwardTime = Time.time + Util.GetCurveMaxTime(CurrentSpeedCurve);
	}
	
	public void _MoveForwardAtCurve(int curveIndex)
	{
		switch(curveIndex)
		{
		case 0:
			CurrentSpeedCurve = this.SpeedCurve_1;
			break;
		case 1:
			CurrentSpeedCurve = this.SpeedCurve_2;
			break;
		case 2:
		default:
			CurrentSpeedCurve = this.SpeedCurve_3;
			break;
		}
		forward = true;
		startForwardTime = Time.time;
		stopForwardTime = Time.time + Util.GetCurveMaxTime(CurrentSpeedCurve);
	}
	
	/// <summary>
	/// Change Unit.UnitReceiveDamageStatus to vulnerableButNotReactToDamage in furture N seconds.
	/// </summary>
	public void _UnitNotReactToDamageInSeconds(float seconds)
	{
		this.unit.receiveDamageStatus = UnitReceiveDamageStatus.vulnerableButNotReactToDamage;
		RevertReceiveDamageStatusTime = Time.time + seconds;
	}
	
	/// <summary>
	/// Change Unit.UnitReceiveDamageStatus to invincible in furture N seconds.
	/// </summary>
	public void _UnitInvincibleInSeconds(float seconds)
	{
		this.unit.receiveDamageStatus = UnitReceiveDamageStatus.invincible;
		RevertReceiveDamageStatusTime = Time.time + seconds;
	}
	
}
