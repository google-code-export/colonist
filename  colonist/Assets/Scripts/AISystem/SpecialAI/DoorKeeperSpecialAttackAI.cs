using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorKeeperSpecialAttackAI : AbstractAI
{
	
	public string PreDrillAniamtion = "PreDrill";
	public string DrillingAnimation = "Drilling";
	public string BounceWallAnimation = "BounceWall";
	public string PreRollAnimation = "PreRoll";
	public string RollingAnimation = "Rolling";
	public string RollEndAnimation = "RollEnd";
	
	/// <summary>
	/// The drilling move speed curve.
	/// </summary>
	public AnimationCurve DrillingMoveSpeedCurve = AnimationCurve.Linear (0, 2.5f, 3, 10);
	
	/// <summary>
	/// The drilling animation speed curve.
	/// </summary>
	public AnimationCurve DrillingAnimationSpeedCurve = AnimationCurve.Linear (0, 1, 3, 4);
	
	Unit unit = null;
	bool HasHitWall = false;
	CharacterController characterController = null;

	void Awake ()
	{
		unit = this.GetComponent<Unit> ();
		characterController = GetComponent<CharacterController> ();
	}
	
	public override void StartAI ()
	{
		unit.CurrentTarget = LevelManager.Instance.player.transform;
		StartCoroutine ("DrillingAttack");
	}

	public override void StopAI ()
	{
	}

	public override void InitAI ()
	{
	}

	public override void StartBehavior (AIBehavior behavior)
	{
	}

	public override void StopBehavior (AIBehavior behavior)
	{
	}
	
	/// <summary>
	/// Execute special attack behavior - Drilling Attack.
	/// </summary>
	IEnumerator DrillingAttack ()
	{
		while (true) {
			//Play PreDrill animation:
			animation.CrossFade (PreDrillAniamtion);
			yield return new WaitForSeconds(animation[PreDrillAniamtion].length);
			//face to current target:
			GameObject Player = unit.CurrentTarget.gameObject;
			Vector3 currentTargetPosition = new Vector3 (Player.transform.position.x, this.transform.position.y, Player.transform.position.z);
			transform.LookAt (currentTargetPosition);
			//get the direction:
			Vector3 drillingForwardDirection = currentTargetPosition - transform.position;
			//move forward, unit hit the wall:
			animation.CrossFade (DrillingAnimation);
			
			//Set off isKinematic to let OnControllerColliderHit called.
		    HasHitWall = false;
			
			//Ignore collision between transform and player. Let transform pass through player without collision.
			Physics.IgnoreCollision(this.characterController, Player.collider, true);
			
			float _startTime = Time.time;
			while (HasHitWall == false) {
				float _t = Time.time - _startTime;
				float _speed = DrillingMoveSpeedCurve.Evaluate (_t);
//				Debug.Log("Current speed :" + _speed);
				Vector3 drillingForwardVelocity = drillingForwardDirection.normalized * _speed;
				animation [DrillingAnimation].speed = DrillingAnimationSpeedCurve.Evaluate (_t);
				characterController.SimpleMove (drillingForwardVelocity);
				yield return null;
			}
			//after, set on the collision detection between play and transform.
			Physics.IgnoreCollision(this.characterController, Player.collider, false);
			
			//hit wall, and bounce back
			animation.CrossFade (BounceWallAnimation);
		
			yield return new WaitForSeconds(animation[BounceWallAnimation].length + 0.5f);
		
		}
	}
	

	void OnControllerColliderHit (ControllerColliderHit  hit)
	{
		//if the gameObject is colliding with wall layer object:
		if (Util.CheckLayerWithinMask (hit.collider.collider.gameObject.layer, unit.WallLayer) == true) {
			HasHitWall = true;
//			Debug.Log("I hit wall!");
		}
		Debug.Log("I hit something:" + hit.collider.gameObject.name);
	}
	
	void OnCollisionEnter(Collision c)
	{
		Debug.Log("I collide something:" + c.collider.gameObject.name);
	}
}
