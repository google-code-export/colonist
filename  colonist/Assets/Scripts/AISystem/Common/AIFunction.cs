using UnityEngine;
using System.Collections;

/// <summary>
/// Defines basic AI functions. eg : 
/// 1. Move forward/backward for a short while
/// 2. Attack current target.
/// </summary>
public class AIFunction : MonoBehaviour
{
	public float movebackspeed = 1, moveForwardSpeed = 1;
	CharacterController controller = null;
	bool backward = false, forward = false;
	float stopBackward = 0, stopForward = 0;
	Unit unit;
	public float MoveDistanceLimitation = 8;
	public float MoveOffset = -1;
	public float RotationSpeed = 15;
	
	/// <summary>
	/// if RotateToCurrentTarget = true, the character will rotate smoothly to face to the current target.
	/// </summary>
	bool RotateToCurrentTarget = false;
	float StopRotationTime = 0;
	
	Transform CurrentTarget {
		get {
			return this.unit.CurrentTarget;
		}
	}
	
	void Awake ()
	{
		this.unit = GetComponent<Unit> ();
		controller = GetComponent<CharacterController> ();
	}
	
	void Update ()
	{
		if (backward && Time.time >= stopBackward) {
			backward = false;
		}
		if (backward) {
			controller.SimpleMove (transform.TransformDirection (Vector3.back) * movebackspeed);
		}
		if (forward && Time.time >= stopForward) {
			forward = false;
		}
		if (forward) {
			controller.SimpleMove (transform.forward * moveForwardSpeed);
		}
		
		if(RotateToCurrentTarget && CurrentTarget != null)
		{
			Util.RotateToward(this.transform, CurrentTarget.position, true, RotationSpeed);
		}
		if(RotateToCurrentTarget && Time.time >= StopRotationTime)
		{
			RotateToCurrentTarget = false;
		}
	}
	
	public void _Moveforward (float duration)
	{
		forward = true;
		stopForward = Time.time + duration;
	}
	
	public void _Moveback (float duration)
	{
		backward = true;
		stopBackward = Time.time + duration;
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
	/// </summary>
	IEnumerator _MoveToCurrentTarget (float _duration)
	{
		CharacterController controller = this.GetComponent<CharacterController> ();
		float distance = Util.DistanceOfCharactersXZ (controller, this.unit.CurrentTarget.GetComponent<CharacterController> ()) + MoveOffset;
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
	/// <param name="name"></param>
	public void _CreateEffect (string name)
	{
		EffectData effectdata = this.unit.EffectDataDict [name];
		GlobalBloodEffectDecalSystem.CreateEffect(this.controller.center + transform.position, effectdata);
	}
	
}
