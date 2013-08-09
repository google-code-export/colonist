using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

/// <summary>
/// Provides static editor function.
/// </summary>
public class EditorCommon
{
	#region Edit Unit property
	
	public static UnitBase EditBasicUnitProperty (UnitBase unit)
	{
		GUILayout.Label (new GUIContent ("Edit Unit Basic Property------------------------------------------", ""));
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Unit name:");
		unit.Name = GUILayout.TextField (unit.Name); 
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		unit.MaxHP = EditorGUILayout.FloatField (new GUIContent ("Max HP:", ""), unit.MaxHP);
		unit.Armor = (ArmorType)EditorGUILayout.EnumPopup (new GUIContent ("Unit armor:", ""),
			unit.Armor);
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		GUILayout.Label (new GUIContent ("Enemy layer:", ""));
		unit.EnemyLayer = EditorGUILayoutx.LayerMaskField ("", unit.EnemyLayer, true);
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		GUILayout.Label (new GUIContent ("Ground layer:", ""));
		unit.GroundLayer = EditorGUILayoutx.LayerMaskField ("", unit.GroundLayer, true);
		GUILayout.Label (new GUIContent ("Wall layer:", ""));
		unit.WallLayer = EditorGUILayoutx.LayerMaskField ("", unit.WallLayer, true);
		GUILayout.EndHorizontal ();
		return unit;
	}
	
	/// <summary>
	/// Edits the basic animation data.
	/// </summary>
	public static UnitAnimationData EditBasicAnimationData (GameObject gameObject, string StartLabel, UnitAnimationData AnimationData)
	{
		EditorGUILayout.Space ();
		GUILayout.Label (StartLabel);
		AnimationData.Name = EditorGUILayout.TextField (new GUIContent ("Name:", ""), AnimationData.Name);
		if (AnimationData.AnimationName == string.Empty) {
			string[] array = GetAnimationNames (gameObject);
			int index = 0;
			index = EditorGUILayout.Popup ("Animation:", index, array);
			AnimationData.AnimationName = array [index];
		} else {
			int index = 0;
			string[] array = GetAnimationNames (gameObject, AnimationData.AnimationName, out index);
			index = EditorGUILayout.Popup ("Animation:", index, array);
			if (index != -1) {
				AnimationData.AnimationName = array [index];
				EditorGUILayout.LabelField (new GUIContent ("Animation length: " + gameObject.animation [AnimationData.AnimationName].length + " seconds.", "动画时长"));
			} else {
				EditorGUILayout.LabelField ("Warning !!! Animation:" + AnimationData.AnimationName + " does not exist!");
			}
		}
		AnimationData.AnimationLayer = EditorGUILayout.IntField (new GUIContent ("Animation Layer", ""), AnimationData.AnimationLayer);
		AnimationData.AnimationSpeed = EditorGUILayout.FloatField (new GUIContent ("Animation Speed", ""), AnimationData.AnimationSpeed);
		AnimationData.AnimationWrapMode = (WrapMode)EditorGUILayout.EnumPopup (new GUIContent ("WrapMode:", ""), AnimationData.AnimationWrapMode);
		return AnimationData;
	}
	
	public static RotateData[] EditRotateDataArray (GameObject gameObject,
		                                        RotateData[] RotateDataArray)
	{
		if (GUILayout.Button ("Add Rotate data")) {
			RotateData RotateData = new RotateData ();
			IList<RotateData> l = RotateDataArray.ToList<RotateData> ();
			l.Add (RotateData);
			RotateDataArray = l.ToArray<RotateData> ();
		}
		for (int i = 0; i <RotateDataArray.Length; i++) {
			RotateData RotateData = RotateDataArray [i];
			EditorGUILayout.Space ();
			GUILayout.Label ("-------- Edit Rotate Data--");
			RotateData.Name = EditorGUILayout.TextField (new GUIContent ("Name:", ""), RotateData.Name);
			
			int RotateLeftAnimationIndex = 0, RotateRightAnimationIndex = 0;
			
			string [] array_left = GetAnimationNames (gameObject, RotateData.RotateLeftAnimationName, out RotateLeftAnimationIndex);
			RotateLeftAnimationIndex = EditorGUILayout.Popup ("Rotate Left Animation:", RotateLeftAnimationIndex, array_left);
			if (RotateLeftAnimationIndex >= 0) {
				RotateData.RotateLeftAnimationName = array_left [RotateLeftAnimationIndex];
				EditorGUILayout.LabelField (new GUIContent ("Animation length: " + gameObject.animation [RotateData.RotateLeftAnimationName].length + " seconds.", ""));
			}
			
			string [] array_right = GetAnimationNames (gameObject, RotateData.RotateRightAnimationName, out RotateRightAnimationIndex);
			RotateRightAnimationIndex = EditorGUILayout.Popup ("Rotate Right Animation:", RotateRightAnimationIndex, array_right);
			if (RotateRightAnimationIndex >= 0) {
				RotateData.RotateRightAnimationName = array_right [RotateRightAnimationIndex];
				EditorGUILayout.LabelField (new GUIContent ("Animation length: " + gameObject.animation [RotateData.RotateRightAnimationName].length + " seconds.", ""));
			}
			
			RotateData.AnimationLayer = EditorGUILayout.IntField (new GUIContent ("Animation Layer", ""), RotateData.AnimationLayer);
			RotateData.AnimationSpeed = EditorGUILayout.FloatField (new GUIContent ("Animation Speed", ""), RotateData.AnimationSpeed);
			RotateData.AnimationWrapMode = (WrapMode)EditorGUILayout.EnumPopup (new GUIContent ("WrapMode:", ""), RotateData.AnimationWrapMode);
			
			RotateData.RotateAngularSpeed = EditorGUILayout.FloatField (new GUIContent ("Rotate angular speed", ""), RotateData.RotateAngularSpeed);
			RotateData.AngleDistanceToStartRotate = EditorGUILayout.FloatField (
				new GUIContent ("Angle distance to rotate:", "Only rotate to face target when forward direction and face to target direction's angle distance > AngleDistanceToStartRotate"), 
				RotateData.AngleDistanceToStartRotate);
			//Delete this data
			if (GUILayout.Button ("Delete " + RotateData.Name)) {
				RotateDataArray = Util.CloneExcept<RotateData> (RotateDataArray, RotateData);
			}
			EditorGUILayout.Space ();
		}
		return RotateDataArray;
		
	}
	
	/// <summary>
	/// Edits the idle data array.
	/// Use the editor function like this:
	/// 			   Unit.IdleData = EditorCommon.EditIdleDataArray(selectedGameObject,
	///			                                                      Unit,
	///			                                                      Unit.IdleData);
	/// </summary>
	/// 
	public static IdleData[] EditIdleDataArray (GameObject gameObject,
		                                        IdleData[] IdleDataArray)
	{
		if (GUILayout.Button ("Add Idle data")) {
			IdleData IdleData = new IdleData ();
			IList<IdleData> l = IdleDataArray.ToList<IdleData> ();
			l.Add (IdleData);
			IdleDataArray = l.ToArray<IdleData> ();
		}
		UnitAnimationData[] UnitAnimationDataArray = IdleDataArray.ToArray<UnitAnimationData> ();
		for (int i = 0; i <IdleDataArray.Length; i++) {
			IdleData IdleData = IdleDataArray [i];
			EditBasicAnimationData (gameObject, 
			                        string.Format (" ---------------------- {0}", IdleData.Name), 
			                        IdleData as UnitAnimationData);
			IdleData.KeepFacingTarget = EditorGUILayout.Toggle (new GUIContent ("Keep facing target:", ""), IdleData.KeepFacingTarget);
			IdleData.SmoothRotate = EditorGUILayout.Toggle (new GUIContent ("Smooth rotate to facing target?", ""), IdleData.SmoothRotate);
			if (IdleData.SmoothRotate) {
				Unit unit = gameObject.GetComponent<Unit> ();
				if (unit.RotateData.Length > 0) {
					string[] rotateDataNameArray = unit.RotateData.Select (x => x.Name).ToArray ();
					IdleData.RotateDataName = EditorCommon.EditPopup ("Choose a rotate data name:", IdleData.RotateDataName, rotateDataNameArray);
				} else {
					EditorGUILayout.LabelField ("Warning! There is no rotate data defined in this unit!");
				}
			}
			//Delete this data
			if (GUILayout.Button ("Delete " + IdleData.Name)) {
				IdleDataArray = Util.CloneExcept<IdleData> (IdleDataArray, IdleData);
			}
			EditorGUILayout.Space ();
		}
		return IdleDataArray;
	}
	
	public static IdleData EditIdleData (GameObject gameObject,
		                                   IdleData IdleData)
	{
		EditBasicAnimationData (gameObject, 
			                    string.Format (" ---------------------- {0}", IdleData.Name), 
			                    IdleData as UnitAnimationData);		
		return IdleData;
	}
	
	/// <summary>
	/// Edits the move data array.
	/// </summary>
	public static MoveData[] EditMoveDataArray (GameObject gameObject,
		                                  MoveData[] MoveDataArray)
	{
		if (GUILayout.Button ("Add Move data")) {
			MoveData MoveData = new MoveData ();
			MoveDataArray = Util.AddToArray<MoveData> (MoveData, MoveDataArray);
		}
		for (int i = 0; i < MoveDataArray.Length; i++) {
			MoveData _MoveData = MoveDataArray [i];
			_MoveData = EditorCommon.EditMoveData (gameObject, _MoveData);
			//Delete this move data
			if (GUILayout.Button ("Delete " + _MoveData.Name)) {
				MoveDataArray = Util.CloneExcept<MoveData> (MoveDataArray, _MoveData);
			}
		}
		return MoveDataArray;
	}
	
	public static MoveData EditMoveData (GameObject gameObject,
		                                 MoveData MoveData)
	{

		if(MoveData.UseAnimation)
		{
		   EditBasicAnimationData (gameObject,
				                   string.Format (" ---------------------- {0}", MoveData.Name), 
				                   MoveData as UnitAnimationData);
		}
		else 
		{
			MoveData.Name = EditorGUILayout.TextField (new GUIContent ("Name:", ""), MoveData.Name);
		}
		MoveData.UseAnimation = EditorGUILayout.Toggle(new GUIContent("Use animation:", "If false, animation will not be played when moving."), MoveData.UseAnimation);
		
		MoveData.MoveSpeed = EditorGUILayout.FloatField (new GUIContent ("Speed:", "单位移动速度."), MoveData.MoveSpeed);
		MoveData.UseGravityWhenMoving = EditorGUILayout.Toggle(new GUIContent("Use gravity:", "If use Gravity, move method = SimpleMove, else, Move()"), MoveData.UseGravityWhenMoving);
		MoveData.CanRotate = EditorGUILayout.Toggle (new GUIContent ("CanRotate:", "单位移动的时候,是否朝向前进方向"), MoveData.CanRotate);
		if (MoveData.CanRotate) {
			MoveData.SmoothRotate = EditorGUILayout.Toggle (new GUIContent ("SmoothRotate:", "单位移动转向的时候,是否用角速度自动平滑转向动作."), MoveData.SmoothRotate);
			if (MoveData.SmoothRotate) {
				MoveData.RotateAngularSpeed = EditorGUILayout.FloatField (new GUIContent ("Angular Speed:", "单位旋转角速度"), MoveData.RotateAngularSpeed);
			}
		}
		MoveData.RedirectTargetInterval = EditorGUILayout.FloatField (new GUIContent ("Redirect target time interval:", "Only used in attack behavior. The time interval to redirect target position."), MoveData.RedirectTargetInterval);
		return MoveData;
	}
	
	/// <summary>
	/// Edits the attack data.
	/// </summary>
	public static AttackData[] EditAttackDataArray (Unit unit,
		                                  AttackData[] AttackDataArray)
	{
		if (GUILayout.Button ("Add Attack data")) {
			AttackData AttackData = new AttackData ();
			AttackDataArray = Util.AddToArray<AttackData> (AttackData, AttackDataArray);
		}
		for (int i = 0; i < AttackDataArray.Length; i++) {
			AttackData _AttackData = AttackDataArray [i];
			EditAttackData (unit, _AttackData);
			AttackDataArray [i] = _AttackData;
			//Delete this attack data
			if (GUILayout.Button ("Delete " + _AttackData.Name)) {
				AttackDataArray = Util.CloneExcept<AttackData> (AttackDataArray, _AttackData);
			}
		}
		return AttackDataArray;
	}
	
	public static AttackData EditAttackData (UnitBase unit, AttackData _AttackData)
	{
		_AttackData.HasAnimation = EditorGUILayout.Toggle (string.Format ("{0} has animation?", _AttackData.Name), _AttackData.HasAnimation);
		if (_AttackData.HasAnimation) {
			EditBasicAnimationData (unit.gameObject,
				                    string.Format (" ---------------------- {0}", _AttackData.Name), 
				                    _AttackData as UnitAnimationData);
		} else {
			EditorGUILayout.LabelField (string.Format (" ---------------------- {0}", _AttackData.Name));
		}
		_AttackData.DamageForm = (DamageForm)EditorGUILayout.EnumPopup (new GUIContent ("Damage Form:", "伤害类型"), _AttackData.DamageForm);
		_AttackData.AttackableRange = EditorGUILayout.FloatField (new GUIContent ("Attack range:", "攻击范围"), _AttackData.AttackableRange);
		_AttackData.LookAtTarget = EditorGUILayout.Toggle (new GUIContent ("Look at target:", "Should the unit look at target before attacking ?"), _AttackData.LookAtTarget);
//			AttackData.AttackInterval = EditorGUILayout.FloatField (new GUIContent ("Attack Interval", "攻击间隔"), AttackData.AttackInterval);
		//Hit Test trigger type:
			
		EditorGUILayout.BeginHorizontal ();
		_AttackData.hitTriggerType = (HitTriggerType)EditorGUILayout.EnumPopup (new GUIContent ("Hit test trigger type:", "How to trigger hit test?"), 
				_AttackData.hitTriggerType);
		if (_AttackData.hitTriggerType == HitTriggerType.ByTime) {
			_AttackData.HitTime = EditorGUILayout.FloatField (new GUIContent ("Hit time:",
                            @"The time to trigger attack functionality."),
                            _AttackData.HitTime);
		}
		if (_AttackData.hitTriggerType == HitTriggerType.ByAnimationEvent) {
			string animationName = _AttackData.AnimationName;
			bool HasAttackEvent = false;
			if (unit.animation [animationName] != null) {
				AnimationEvent[] events = AnimationUtility.GetAnimationEvents (unit.animation [animationName].clip);
				if (events != null && events.Length > 0) {
					HasAttackEvent = events.Count (x => x.functionName == "_Attack") > 0;
				}
				if (HasAttackEvent == false) {
					if (GUILayout.Button ("Add attack event")) {
						AnimationEvent _e = new AnimationEvent ();
						_e.functionName = "_Attack";
						_e.stringParameter = _AttackData.Name;
						_e.time = 0;
						unit.animation [animationName].clip.AddEvent (_e);
//							AnimationUtility.SetAnimationEvents(unit.animation[animationName].clip , new AnimationEvent[] { _e } );
					}
				}
			}
		}
			
		EditorGUILayout.EndHorizontal ();
			
		_AttackData.DamagePointBase = EditorGUILayout.FloatField (new GUIContent ("Base Damage Point:", "基础伤害点数"), _AttackData.DamagePointBase);
		EditorGUILayout.BeginHorizontal ();
		_AttackData.MinDamageBonus = EditorGUILayout.FloatField (new GUIContent ("Min Damage Point Bonus:", ""), _AttackData.MinDamageBonus);
		_AttackData.MaxDamageBonus = _AttackData.MaxDamageBonus >= _AttackData.MinDamageBonus ?
                    _AttackData.MaxDamageBonus : _AttackData.MinDamageBonus;
		_AttackData.MaxDamageBonus = EditorGUILayout.FloatField (new GUIContent ("Max Damage Point Bonus:", ""), _AttackData.MaxDamageBonus);
		EditorGUILayout.EndHorizontal ();
		string DamageRange = (_AttackData.DamagePointBase + _AttackData.MinDamageBonus).ToString ()
                    + " ~ " + (_AttackData.DamagePointBase + _AttackData.MaxDamageBonus).ToString ();
		EditorGUILayout.LabelField (new GUIContent ("Damage range:" + DamageRange, "伤害点数范围"));
		_AttackData.Type = (AIAttackType)EditorGUILayout.EnumPopup (new GUIContent ("AI Attack Type:", "攻击类型 - 立刻的/投射/区域"), _AttackData.Type);
		switch (_AttackData.Type) {
		case AIAttackType.Instant:
			_AttackData.HitTestType = (HitTestType)EditorGUILayout.EnumPopup (new GUIContent ("*Hit Test Type:", "命中检测方式 - 一定命中/百分率/碰撞器校验/距离校验"), _AttackData.HitTestType);
			switch (_AttackData.HitTestType) {
			case HitTestType.AlwaysTrue:
				break;
			case HitTestType.HitRate:
				_AttackData.HitRate = EditorGUILayout.FloatField (new GUIContent ("*Hit Rate:", "命中率: 0 - 1"), _AttackData.HitRate);
				_AttackData.HitRate = Mathf.Clamp01 (_AttackData.HitRate);
				break;
			case HitTestType.CollisionTest:
				_AttackData.HitTestCollider = (Collider)EditorGUILayout.ObjectField (new GUIContent ("*Hit Test Collider:", "Assign a collider to test if the target is hit."), _AttackData.HitTestCollider, typeof(Collider));
				break;
			case HitTestType.DistanceTest:
				_AttackData.HitTestDistance = EditorGUILayout.FloatField (new GUIContent ("*Hit Test Distance:", "命中校验距离: "), _AttackData.HitTestDistance);
				break;
			case HitTestType.AngleTest:
				_AttackData.HitTestAngularDiscrepancy = EditorGUILayout.FloatField (new GUIContent ("*Hit Test Angular Distance:", "命中校验角差值范围: "), _AttackData.HitTestAngularDiscrepancy);
				break;
			case HitTestType.DistanceAndAngleTest:
				_AttackData.HitTestDistance = EditorGUILayout.FloatField (new GUIContent ("*Hit Test Distance:", "命中校验距离: "), _AttackData.HitTestDistance);
				_AttackData.HitTestAngularDiscrepancy = EditorGUILayout.FloatField (new GUIContent ("*Hit Test Angular Distance:", "命中校验角差值范围: "), _AttackData.HitTestAngularDiscrepancy);
				break;
			default:
				break;
			}
			break;
		case AIAttackType.Projectile:
			_AttackData.Projectile = (Projectile)EditorGUILayout.ObjectField (new GUIContent ("*Projectile:", "射弹对象"), _AttackData.Projectile, typeof(Projectile));
			_AttackData.ProjectileInstantiateAnchor = (Transform)EditorGUILayout.ObjectField (new GUIContent ("*Projectile Instantiate Anchor :", ""), _AttackData.ProjectileInstantiateAnchor, typeof(Transform));
			break;
		case AIAttackType.Regional:
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField (new GUIContent ("HitTestType:", "Regional 攻击方式的命中检测必须是CollisionTest:"));
			_AttackData.HitTestType = (HitTestType)EditorGUILayout.EnumPopup (_AttackData.HitTestType);
			_AttackData.HitTestType = HitTestType.CollisionTest;
			EditorGUILayout.EndHorizontal ();
			_AttackData.HitTestCollider = (Collider)EditorGUILayout.ObjectField (new GUIContent ("*Hit Test Collider:", "Assign a collider to test if the target is hit."), _AttackData.HitTestCollider, typeof(Collider));
			_AttackData.HitTestDistance = EditorGUILayout.FloatField (new GUIContent ("*Hit Test Distance:", "Enemy in this range will be test by hit collider."), _AttackData.HitTestDistance);
			break;
		}
		_AttackData.ScriptObjectAttachToTarget = (MonoBehaviour)EditorGUILayout.ObjectField (new GUIContent ("Script attach to target:", "造成伤害时,自动附加该脚本组件."), _AttackData.ScriptObjectAttachToTarget, typeof(MonoBehaviour));
		return _AttackData;
	}
	
	/// <summary>
	/// Edits the receive damage data.
	/// </summary>
	public static ReceiveDamageData[] EditReceiveDamageData (Unit unit, ReceiveDamageData[] ReceiveDamageDataArray)
	{
		if (GUILayout.Button ("Add ReceiveDamage data")) {
			ReceiveDamageData receiveDamageData = new ReceiveDamageData ();
			ReceiveDamageDataArray = Util.AddToArray<ReceiveDamageData> (receiveDamageData, ReceiveDamageDataArray);
		}
		for (int i = 0; i < ReceiveDamageDataArray.Length; i++) {
			ReceiveDamageData ReceiveDamageData = ReceiveDamageDataArray [i];
			if (ReceiveDamageData.HaltAI) {
				EditBasicAnimationData (unit.gameObject,
					                        string.Format (" ---------------------- {0}", ReceiveDamageData.Name), 
					                        ReceiveDamageData as UnitAnimationData);
			} else {
				GUILayout.Label (string.Format (" ---------------------- {0}", ReceiveDamageData.Name));
				ReceiveDamageData.Name = EditorGUILayout.TextField (new GUIContent ("Name:", ""), ReceiveDamageData.Name);
			}
			ReceiveDamageData.HaltAI = EditorGUILayout.Toggle (new GUIContent ("HaltAI", "Stop AI and animation and play receive damage animation?"), ReceiveDamageData.HaltAI);
			ReceiveDamageData.ApplicableDamageForm = EditDamageFormArray ("Edit applicable damageform:", ReceiveDamageData.ApplicableDamageForm);
			
			
			
			string[] effectDataNameArray = unit.EffectData.Select (x => x.Name).ToArray<string> ();
			ReceiveDamageData.EffectDataName = EditStringArray ("--------- Edit receive damage effect data-----", ReceiveDamageData.EffectDataName, effectDataNameArray);

			if (unit.DecalData != null && unit.DecalData.Length > 0) {
				string[] decalDataNameArray = unit.DecalData.Select (x => x.Name).ToArray<string> ();
				ReceiveDamageData.DecalDataName = EditStringArray ("--------- Edit receive damage decal data-----", ReceiveDamageData.DecalDataName, decalDataNameArray);
			}
			
			if (unit.AudioData != null && unit.AudioData.Length > 0) {
				string[] AudioDataNameArray = unit.AudioData.Select (x => x.Name).ToArray<string> ();
				ReceiveDamageData.AudioDataName = EditStringArray ("--------- Edit audio data-----", ReceiveDamageData.AudioDataName, AudioDataNameArray);
			}
                
			ReceiveDamageDataArray [i] = ReceiveDamageData;
			//Delete ReceiveDamageData
			if (GUILayout.Button ("Delete ReceiveDamageData:" + ReceiveDamageData.Name)) {
				ReceiveDamageDataArray = Util.CloneExcept<ReceiveDamageData> (ReceiveDamageDataArray, ReceiveDamageData);
			}
			EditorGUILayout.Space ();
		}
		return ReceiveDamageDataArray;

	}
	
	/// <summary>
	/// Edits the effect data.
	/// </summary>
	public static EffectData[] EditEffectData (EffectData[] EffectDataArray)
	{
		if (GUILayout.Button ("Add new Effect data")) {
			EffectData EffectData = new EffectData ();
			EffectDataArray = Util.AddToArray<EffectData> (EffectData, EffectDataArray);
			
		}
		for (int i = 0; i < EffectDataArray.Length; i++) {
			EffectData _EffectData = EffectDataArray [i];
			EditorGUILayout.LabelField ("------------------------ " + _EffectData.Name);
			_EffectData.Name = EditorGUILayout.TextField (new GUIContent ("Name", ""), _EffectData.Name);
			_EffectData.InstantionType = (EffectObjectInstantiation)EditorGUILayout.EnumPopup (
				                                                  new GUIContent ("Instantion type", "Create a new gameobject, or play a children object's particleSystem?"),
				                                                  _EffectData.InstantionType);
			
			//if type = create, create a new object, or use global setting
			if (_EffectData.InstantionType == EffectObjectInstantiation.create || _EffectData.InstantionType == EffectObjectInstantiation.createAndAttachToAnchor) {
				_EffectData.UseGlobalEffect = EditorGUILayout.Toggle (new GUIContent ("Use global effect?", ""), _EffectData.UseGlobalEffect);
			
				EditorGUILayout.BeginHorizontal ();
				_EffectData.CreateDelay = EditorGUILayout.BeginToggleGroup (new GUIContent ("Create in delay", ""), _EffectData.CreateDelay);
				if (_EffectData.CreateDelay) {
					_EffectData.CreateDelayTime = EditorGUILayout.FloatField (_EffectData.CreateDelayTime);
				}
				EditorGUILayout.EndToggleGroup ();
				EditorGUILayout.EndHorizontal ();
			
				_EffectData.Count = EditorGUILayout.IntField (new GUIContent ("Number count:", ""), _EffectData.Count);
			
				if (_EffectData.UseGlobalEffect) {
					_EffectData.GlobalType = (GlobalEffectType)EditorGUILayout.EnumPopup (new GUIContent ("Global effect type", "Use global effect instead of custom effect."),
						_EffectData.GlobalType);
					_EffectData.instantiationData = EditorCommon.EditInstantiationData (" -----------  Edit instantiation data ----------- ", _EffectData.instantiationData);
				} else {
					_EffectData.DestoryTimeOut = EditorGUILayout.FloatField (new GUIContent ("LifeTime:", ""), _EffectData.DestoryTimeOut);
					_EffectData.EffectObject = (GameObject)EditorGUILayout.ObjectField (new GUIContent ("Effect object", ""), _EffectData.EffectObject, typeof(GameObject));
					_EffectData.instantiationData = EditorCommon.EditInstantiationData (" -----------  Edit instantiation data ----------- ", _EffectData.instantiationData);
					//Delete this effect data
					if (GUILayout.Button ("Delete EffectData:" + _EffectData.Name)) {
						EffectDataArray = Util.CloneExcept<EffectData> (EffectDataArray, _EffectData);
					}
				}
			} else if (_EffectData.InstantionType == EffectObjectInstantiation.play) {
				_EffectData.EffectObject = (GameObject)EditorGUILayout.ObjectField ("Effect particle system:", _EffectData.EffectObject, typeof(GameObject));
				_EffectData.UseGlobalEffect = false;
				_EffectData.CreateDelay = false;
				_EffectData.DestoryTimeOut = 0;
			}
			EditorGUILayout.Space ();
			if (GUILayout.Button ("Remove EffectData:" + _EffectData.Name)) {
				EffectDataArray = Util.CloneExcept<EffectData> (EffectDataArray, _EffectData);
			}
		}
		return EffectDataArray;
	}
	
	public static InstantiationData EditInstantiationData (string label, InstantiationData instantiationData)
	{
		EditorGUILayout.LabelField (label);
		instantiationData.BasicAnchor = (Transform)EditorGUILayout.ObjectField (new GUIContent ("Base creation anchor:", "The basic anchor to create instance."),
			                                                                   instantiationData.BasicAnchor, 
			                                                                   typeof(Transform));
		EditorGUILayout.BeginHorizontal ();
		instantiationData.rotationOfInstance = (InstantiationRotationMode)EditorGUILayout.EnumPopup (new GUIContent ("Instance rotation mode:", "How to set the rotation of instance?"), instantiationData.rotationOfInstance);
		if (instantiationData.rotationOfInstance == InstantiationRotationMode.SpecifiedQuaternion) {
			Vector4 v = new Vector4 (instantiationData.specifiedQuaterion.x, instantiationData.specifiedQuaterion.y, 
				                    instantiationData.specifiedQuaterion.z, instantiationData.specifiedQuaterion.w);
			v = EditorGUILayout.Vector4Field ("Specified X,Y,Z,W:", v);
			instantiationData.specifiedQuaterion = new Quaternion (v.x, v.y, v.z, v.w);
		}
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal ();
		instantiationData.RandomPositionInsideSphere = EditorGUILayout.Toggle ("Randomly create inside sphere?", instantiationData.RandomPositionInsideSphere);
		if (instantiationData.RandomPositionInsideSphere) {
			instantiationData.RandomSphereUnit = EditorGUILayout.FloatField ("Random sphere radius:", instantiationData.RandomSphereUnit);
		}
		EditorGUILayout.EndHorizontal ();
		
		instantiationData.WorldOffset = EditorGUILayout.Vector3Field ("World position offset:", instantiationData.WorldOffset);
		return instantiationData;
	}
	
	/// <summary>
	/// Edits the decal data.
	/// </summary>
	public static DecalData[] EditDecalData (DecalData[] DecalDataArray)
	{
		if (GUILayout.Button ("Add Decal data")) {
			DecalData DecalData = new DecalData ();
			DecalDataArray = Util.AddToArray<DecalData> (DecalData, DecalDataArray);
		}
		for (int i = 0; i < DecalDataArray.Length; i++) {
			DecalData DecalData = DecalDataArray [i];
			EditorGUILayout.LabelField ("------------------------ " + DecalData.Name);
			DecalData.Name = EditorGUILayout.TextField (new GUIContent ("Name", ""), DecalData.Name); 
			DecalData.UseGlobalDecal = EditorGUILayout.Toggle (new GUIContent ("UseGlobalDecal?", "使用全局Decal设定?"), DecalData.UseGlobalDecal);
			if (DecalData.UseGlobalDecal) {
				DecalData.GlobalType = (GlobalDecalType)EditorGUILayout.EnumPopup (new GUIContent ("Global decal type:", ""), DecalData.GlobalType);
			} else {
				DecalData.DestoryInTimeOut = EditorGUILayout.Toggle (new GUIContent ("Destory this decal in timeout?", "Decal是否有Lifetime?"), DecalData.DestoryInTimeOut);
				if (DecalData.DestoryInTimeOut) {
					DecalData.DestoryTimeOut = EditorGUILayout.FloatField (new GUIContent ("Destory Timeout:", ""), DecalData.DestoryTimeOut);
				}
				DecalData.ProjectDirection = (HorizontalOrVertical)EditorGUILayout.EnumPopup (new GUIContent ("Decal Project Direction", "创建Decal的投射方向,对于地上的Decal,投射方向是Vertical,对于墙上的Decal,投射方向是Horizontal."), DecalData.ProjectDirection);
				DecalData.ApplicableLayer = EditorGUILayoutx.LayerMaskField ("ApplicableLayer", DecalData.ApplicableLayer);
				DecalData.DecalObjects = EditObjectArray ("--------------Edit Decal object ------", DecalData.DecalObjects);
				DecalData.ScaleRate = EditorGUILayout.FloatField (new GUIContent ("Scale rate:", "Final scale = initial scale * ScaleRate"), DecalData.ScaleRate);
			}
			//Delete this DecalData
			if (GUILayout.Button ("Delete DecalData:" + DecalData.Name)) {
				DecalDataArray = Util.CloneExcept<DecalData> (DecalDataArray, DecalData);
			}
			EditorGUILayout.Space ();
		}
		return DecalDataArray;
	}
	
	public static DeathData[] EditDeathData (Unit unit, DeathData[] DeathDataArray)
	{
		
		if (GUILayout.Button ("Add death data")) {
			DeathData DeathData = new DeathData ();
			DeathDataArray = Util.AddToArray<DeathData> (DeathData, DeathDataArray);
		}
		for (int i = 0; i < DeathDataArray.Length; i++) {
			DeathData DeathData = DeathDataArray [i];
			//Death animation is used only when: 1. there is no ragdoll, or 2.create ragdoll, after animation finishes.
			if (DeathData.UseDieReplacement == false ||
                   (DeathData.UseDieReplacement == true &&
                    (DeathData.ReplaceAfterAnimationFinish == true || DeathData.ReplaceAfterSeconds > 0))) {
				EditBasicAnimationData (unit.gameObject,
					                        string.Format (" ---------------------- {0}", DeathData.Name), 
					                        DeathData as UnitAnimationData);
			} else {
				GUILayout.Label (string.Format (" ---------------------- {0}", DeathData.Name));
				DeathData.Name = EditorGUILayout.TextField (new GUIContent ("Name:", ""), DeathData.Name);
			}
			DeathData.ApplicableDamageForm = EditDamageFormArray ("Applicable Damage Form", DeathData.ApplicableDamageForm);
			DeathData.UseDieReplacement = EditorGUILayout.Toggle (new GUIContent ("Use Die replacement:", "死亡时,是否创建替代布娃娃?"), DeathData.UseDieReplacement);
			if (DeathData.UseDieReplacement) {
				DeathData.ReplaceAfterAnimationFinish = EditorGUILayout.Toggle (new GUIContent ("Create replacement following animation", "Replace by ragdoll after animation over?"), DeathData.ReplaceAfterAnimationFinish);
				if (DeathData.ReplaceAfterAnimationFinish == false) {
					DeathData.ReplaceAfterSeconds = EditorGUILayout.FloatField (new GUIContent ("Replace ragdoll after N seconds?", "Ingore this field if its value = zero"), DeathData.ReplaceAfterSeconds);
				}
				DeathData.DieReplacement = (GameObject)EditorGUILayout.ObjectField (new GUIContent ("Die replacement:", ""), DeathData.DieReplacement, typeof(GameObject));
				DeathData.ReplaceOldObjectInSpawnedList = EditorGUILayout.Toggle (new GUIContent ("Replace spawned list", " This is only available for runtime spawned units"), DeathData.ReplaceOldObjectInSpawnedList);
				DeathData.DestoryCharacterController = EditorGUILayout.Toggle (new GUIContent ("Destory Character Controller when dying ?", ""), DeathData.DestoryCharacterController);
				DeathData.CopyChildrenTransformToDieReplacement = EditorGUILayout.Toggle (new GUIContent ("Copy transform?", "是否把替代物的关节位置调整到和死亡单位一致?"), DeathData.CopyChildrenTransformToDieReplacement);
			} else {
				EditorGUILayout.BeginHorizontal ();
				DeathData.DestoryGameObject = EditorGUILayout.Toggle (new GUIContent ("Destory GameObject:", ""), DeathData.DestoryGameObject);
				if (DeathData.DestoryGameObject) {
					DeathData.DestoryLagTime = EditorGUILayout.FloatField ("Destory lag time:", DeathData.DestoryLagTime);
				}
				EditorGUILayout.EndHorizontal ();
			}
			string[] effectDataNameArray = unit.EffectData.Select (x => x.Name).ToArray<string> ();
			DeathData.EffectDataName = EditStringArray ("--------- Edit death effect data-----", DeathData.EffectDataName, effectDataNameArray);
			string[] decalDataNameArray = unit.DecalData.Select (x => x.Name).ToArray<string> ();
			DeathData.DecalDataName = EditStringArray ("--------- Edit death decal data-----", DeathData.DecalDataName, decalDataNameArray);
			
			string[] audioDataNameArray = unit.AudioData.Select (x => x.Name).ToArray<string> ();
			DeathData.AudioDataName = EditStringArray ("--------- Edit audio data-----", DeathData.AudioDataName, audioDataNameArray);
			
			//Delete DeathData
			if (GUILayout.Button ("Delete DeathData:" + DeathData.Name)) {
				DeathDataArray = Util.CloneExcept<DeathData> (DeathDataArray, DeathData);
			}

		}
		return DeathDataArray;
	}
	
	public static AudioData[] EditAudioData (Unit unit, AudioData[] AudioDataArray)
	{
		if (GUILayout.Button ("Add audio data")) {
			AudioData audioData = new AudioData ();
			audioData.enabled = true;
			AudioDataArray = Util.AddToArray<AudioData> (audioData, AudioDataArray);
		}
		foreach (AudioData audioData in AudioDataArray) {
			audioData.Name = EditorGUILayout.TextField ("AudioData Name:", audioData.Name);
			audioData.mode = (AudioObjectMode)EditorGUILayout.EnumPopup ("Audio object mode:", audioData.mode);
			audioData.playMode = (AudioPlayMode)EditorGUILayout.EnumPopup ("Play mode:", audioData.playMode);
			if (audioData.playMode == AudioPlayMode.BindToAnimation) {
				//when playmode = bindToAnimation, the Audio object must be an AudioSource.
				audioData.mode = AudioObjectMode.AudioSouce;
			}
			switch (audioData.mode) {
			case AudioObjectMode.AudioSouce:
				audioData.randomAudioSources = EditorCommon.EditObjectArray<AudioSource> ("Edit audioSources:", audioData.randomAudioSources);
				if (audioData.playMode == AudioPlayMode.BindToAnimation) {
					audioData.AnimationBinded = EditorCommon.EditStringArray ("Animations Binded:", audioData.AnimationBinded, EditorCommon.GetAnimationNames (unit.gameObject));
					audioData.BindedAudioSource = (AudioSource)EditorGUILayout.ObjectField (new GUIContent ("Binded audio:", "When the animation is playing/stoped, the AudioSource is playing/stoped automatically."), audioData.BindedAudioSource, typeof(AudioSource));
				}
				break;
			case AudioObjectMode.Clip:
				audioData.randomAudioClips = EditorCommon.EditObjectArray<AudioClip> ("Edit audioClips:", audioData.randomAudioClips);
				audioData.PlayClipVolume = EditorGUILayout.Slider ("Volume:", audioData.PlayClipVolume, 0, 1);
				audioData.Play3DClipAnchor = (Transform)EditorGUILayout.ObjectField ("Play audio anchor:", audioData.Play3DClipAnchor, typeof(Transform));
				break;
			}
			if (GUILayout.Button ("Delete audio data:" + audioData.Name)) {
				AudioDataArray = Util.CloneExcept<AudioData> (AudioDataArray, audioData);
			}
			EditorGUILayout.Space ();
		}
		return AudioDataArray;
	}
	
	#endregion
	
	
	#region Helper functions
	public static string[] GetAnimationNames (GameObject gameObject, string CurrentAnimationName, out int index)
	{
		IList<string> AnimationList = new List<string> ();
		foreach (AnimationState state in gameObject.animation) {
			AnimationList.Add (state.name);
		}
		index = AnimationList.IndexOf (CurrentAnimationName);
		return AnimationList.ToArray<string> ();
	}
	
	/// <summary>
	/// Return all animation state names in string array.
	/// </summary>
	public static string[] GetAnimationNames (GameObject gameObject)
	{
		IList<string> AnimationList = new List<string> ();
		foreach (AnimationState state in gameObject.animation) {
			AnimationList.Add (state.name);
		}
		return AnimationList.ToArray<string> ();
	}

	/// <summary>
	/// Return element index in the array.
	/// If not exists, return 0
	/// </summary>
	public static int IndexOfArray<T> (T[] array, T element)
	{
		int index = 0;
		if (array != null && array.Length > 0) {
			for (int i = 0; i < array.Length; i++) {
				if (array [i].Equals (element)) {
					index = i;
					break;
				}
			}
			return index;
		}
		return index;
	}

	public static Object[] EditObjectArray (string label, Object[] OrigArray)
	{
		EditorGUILayout.LabelField (label);
		Object[] newArray = OrigArray;
		if (GUILayout.Button ("Add new object element")) {
			newArray = Util.ExpandArray<Object> (OrigArray, 1);
		}
		if (newArray != null && newArray.Length > 0) {
			for (int i = 0; i < newArray.Length; i++) {
				EditorGUILayout.BeginHorizontal ();
				Object element = newArray [i];
				element = EditorGUILayout.ObjectField (element, typeof(Object));
				if (GUILayout.Button ("Remove")) {
					newArray = Util.CloneExcept<Object> (newArray, i);
					break;
				}
				newArray [i] = element;
				EditorGUILayout.EndHorizontal ();
			}
		}
		return newArray;
	}
	
	public static T[] EditObjectArray<T> (string label, T[] OrigArray) where T : Object
	{
		EditorGUILayout.LabelField (label);
		T[] newArray = OrigArray;
		if (GUILayout.Button ("Add new object element")) {
			newArray = Util.ExpandArray<T> (OrigArray, 1);
		}
		if (newArray != null && newArray.Length > 0) {
			for (int i = 0; i < newArray.Length; i++) {
				EditorGUILayout.BeginHorizontal ();
				T element = newArray [i];
				element = (T)EditorGUILayout.ObjectField (element, typeof(T));
				if (GUILayout.Button ("Remove")) {
					newArray = Util.CloneExcept<T> (newArray, i);
					break;
				}
				newArray [i] = element;
				EditorGUILayout.EndHorizontal ();
			}
		}
		return newArray;
	}
	
	public static DamageForm[] EditDamageFormArray (string label, DamageForm[] damageFormArray)
	{
		EditorGUILayout.LabelField (label);
		if (GUILayout.Button ("Add new DamageForm element")) {
			DamageForm element = DamageForm.Common;
			damageFormArray = Util.AddToArray<DamageForm> (element, damageFormArray);
		}
        
		for (int i = 0; i < damageFormArray.Length; i++) {
			EditorGUILayout.BeginHorizontal ();
			damageFormArray [i] = (DamageForm)EditorGUILayout.EnumPopup (damageFormArray [i]);
			if (GUILayout.Button ("Remove")) {
				damageFormArray = Util.CloneExcept<DamageForm> (damageFormArray, i);
				EditorGUILayout.EndHorizontal ();
				break;
			}
			EditorGUILayout.EndHorizontal ();
		}
		return damageFormArray;
	}
	
	/// <summary>
	/// Edits the string array. The element is input manually.
	/// </summary>
	public static string[] EditStringArray (string label, string[] array)
	{
		EditorGUILayout.LabelField (label);
		if (GUILayout.Button ("Add new string element")) {
			string element = "";
			array = Util.AddToArray<string> (element, array);
		}
        
		for (int i = 0; i < array.Length; i++) {
			array [i] = EditorGUILayout.TextField (array [i]);
			if (GUILayout.Button ("Remove")) {
				array = Util.CloneExcept<string> (array, i);
				break;
			}
			
		}
		return array;
	}
	
	/// <summary>
	/// Edit a string array. The element is chosen from the popup of displayOption.
	/// array - the array to edit.
	/// displayOption - the popup group to let user select.
	/// </summary>
	public static string[] EditStringArray (string label, string[] array, string[] displayOption)
	{
		EditorGUILayout.LabelField (label);
		if (GUILayout.Button ("Add new string element")) {
			string element = "";
			array = Util.AddToArray<string> (element, array);
		}
        
		for (int i = 0; i < array.Length; i++) {
			EditorGUILayout.BeginHorizontal ();
			string element = array [i];
			int index = IndexOfArray<string> (displayOption, element);
			int oldIndex = index;
			index = EditorGUILayout.Popup ("Choose one of :", index, displayOption);
            
			element = displayOption [index];
			array [i] = element;
                
            
			if (GUILayout.Button ("Remove")) {
				array = Util.CloneExcept<string> (array, i);
				break;
			}
			EditorGUILayout.EndHorizontal ();
		}
		return array;
	}
	
	/// <summary>
	/// Edits the given name's property field of the object.
	/// </summary>
	/// <param name='fieldname'>
	/// Fieldname.
	/// </param>
	/// <param name='gameObject'>
	/// Game object.
	/// </param>
	public static void EditPropertyField (string label, string description, 
		                                  string fieldname, Object theObject)
	{
		SerializedObject serObj = new SerializedObject (theObject);
		SerializedProperty p = serObj.FindProperty (fieldname);
		EditorGUILayout.PropertyField (p, new GUIContent (label, description));
		serObj.Update ();
	}
	
	/// <summary>
	/// Given a value, and a string array, let the value selected from the drop down list composed of displayOption.
	/// </summary>
	public static string EditPopup (string label, string _value, string[] displayOption)
	{
		if (displayOption.Length > 0) {
			int index = IndexOfArray<string> (displayOption, _value);
			index = EditorGUILayout.Popup (label, index, displayOption);
			return displayOption [index];
		} else {
			EditorGUILayout.LabelField (label);
			return "";
		}
	}
	
	public static Object EditPopupOfTypeInChildren (string label, 
		                                           Object _value, 
		                                           GameObject gameObject, 
		                                           System.Type Type)
	{
		Component[] co = gameObject.GetComponentsInChildren (Type);
		string[] names = co.Select (x => x.gameObject.name).ToArray ();
		int index = (_value == null) ? 0 : IndexOfArray<string> (names, _value.name);
		index = EditorGUILayout.Popup (index, names);
		return co.Where (x => x.name == names [index]).First ();
	}
	
	/// <summary>
	/// Copies the unit and AI component from Unit A to Unit B.
	/// </summary>
	public static void CopyUnitAndAIComponent (Unit A, GameObject B)
	{
		Unit unit = B.AddComponent<Unit> ();
		A.CloneTo (unit);
		EditorUtility.SetDirty (B);
	}
	
	public static void CopyAnimationEvents (AnimationClip A, AnimationClip B)
	{
		AnimationEvent[] events = AnimationUtility.GetAnimationEvents (A);
		AnimationUtility.SetAnimationEvents (B, events);
	}
	
	public static void AddAnimationEvent (AnimationClip clip, string function, 
		                                 string param, AnimationParameterType parameterType)
	{
		AnimationEvent _event = new AnimationEvent ();
		_event.time = 0;
		_event.functionName = function;
		switch (parameterType) {
		case AnimationParameterType.StringParam:
			_event.stringParameter = param;
			break;
		case AnimationParameterType.IntParam:
			_event.intParameter = int.Parse (param);
			break;
		case AnimationParameterType.FloatParam:
			_event.floatParameter = float.Parse (param);
			break;
		}
		clip.AddEvent (_event);
	}
	
    #endregion
	
#region ScenarioData edit
//	public static void EditScenarioData(ScenarioData scenarioData)
//	{
//		scenarioData.Name = EditorGUILayout.TextField("Scenario Name:" , scenarioData.Name);
//		scenarioData.cameraDock = (CameraDock)EditorGUILayout.ObjectField("Camera Docks:", scenarioData.cameraDock, typeof(CameraDock));
//		scenarioData.gameEventAtScenarioStart =  EditGameEventArray("---------- Edit GameEvents at scenario start -------", scenarioData.gameEventAtScenarioStart);
//		scenarioData.gameEventAtScenarioCameraDockingCompleted =  EditGameEventArray("---------- Edit GameEvents at scenario camera on dock -------", scenarioData.gameEventAtScenarioCameraDockingCompleted);
//	}
	
	public static GameEvent[] EditGameEventArray (string label, GameEvent[] array)
	{
		GUILayout.Label (label);
		if (GUILayout.Button ("Add GameEvent:")) {
			GameEvent e = new GameEvent ();
			array = Util.AddToArray<GameEvent> (e, array);
		}
		foreach (GameEvent _e in array) {
			EditGameEvent (_e);
			if (GUILayout.Button ("Delete GameEvent:" + _e.Name)) {
				array = Util.CloneExcept<GameEvent> (array, _e);
			}
			EditorGUILayout.Separator ();
		}
		return array;
	}
	
	public static void EditGameEvent (GameEvent _event)
	{
		_event.Name = EditorGUILayout.TextField ("Event name:", _event.Name);
		EditorGUILayout.BeginHorizontal ();
		_event.type = (GameEventType)EditorGUILayout.EnumPopup ("Event type:", _event.type);
		_event.delaySend = EditorGUILayout.FloatField ("Delay:", _event.delaySend);
		EditorGUILayout.EndHorizontal ();
		_event.receiver = (GameObject)EditorGUILayout.ObjectField ("Receiver gameobject:", _event.receiver, typeof(GameObject));
		EditorGUILayout.BeginHorizontal ();
		_event.IntParameter = EditorGUILayout.IntField ("Int param:", _event.IntParameter);
		_event.FloatParameter = EditorGUILayout.FloatField ("Float param:", _event.FloatParameter);
		_event.BoolParameter = EditorGUILayout.Toggle ("Bool param:", _event.BoolParameter);
		EditorGUILayout.EndHorizontal ();
		
		_event.StringParameter = EditorGUILayout.TextField ("String param:", _event.StringParameter);
		_event.CustomMessage = EditorGUILayout.TextField ("Custom msg:", _event.CustomMessage);
	}
#endregion
	
	/// <summary>
	/// Creates the ragdoll from template.The template is an already working ragdoll.
	/// </summary>
	public static void CreateRagdollFromTemplate (GameObject template, GameObject destination)
	{
		//process template's character joint
		CharacterJoint[] joints = template.GetComponentsInChildren<CharacterJoint> ();
		foreach (CharacterJoint joint in joints) {
			string path = Util.GetChildPath (template.transform, joint.transform);
			Transform dstJointTransform = destination.transform.Find (path);
			if (dstJointTransform == null) {
				Debug.LogWarning ("Warning: " + path + " is not found at destination transform!");
				continue;
			}
			if (dstJointTransform.gameObject.GetComponent<CharacterJoint> () == null) {
				dstJointTransform.gameObject.AddComponent<CharacterJoint> ();
			}
			string connectedRigidPath = Util.GetChildPath (template.transform, joint.connectedBody.transform);
			Transform dstConnectedRigid = destination.transform.Find (connectedRigidPath);
			if (dstConnectedRigid == null) {
				Debug.LogWarning ("Warning: " + path + " is not found at destination transform!");
				continue;
			}
			if (dstConnectedRigid.GetComponent<Rigidbody> () == null) {
				dstConnectedRigid.gameObject.AddComponent<Rigidbody> ();
			}
			//assign the connected rigidbody
			dstJointTransform.gameObject.GetComponent<CharacterJoint> ().connectedBody = dstConnectedRigid.GetComponent<Rigidbody> ();
			Util.CopyCharacterJointSetting (joint, dstJointTransform.gameObject.GetComponent<CharacterJoint> ());
			if (joint.GetComponent<Collider> () != null) {
				Util.CopyCollider (joint.GetComponent<Collider> (), dstJointTransform);
			}
			if (joint.connectedBody.GetComponent<Collider> () != null) {
				Util.CopyCollider (joint.connectedBody.GetComponent<Collider> (), dstJointTransform.gameObject.GetComponent<CharacterJoint> ().connectedBody.transform);
			}
		}
	}
	
	public static void CopyCurveDataIntoTransform (Transform root, AnimationClipCurveData curveData, float time)
	{
		string transformPath = curveData.path;
		Transform _targetTransform = root.Find (transformPath);
		if (_targetTransform == null) {
			Debug.LogError (transformPath + " can not be found !");
			return;
		}
		string property = curveData.propertyName;
		CurvePropertyType curvePropertyType = CurvePropertyNameToType (property);
		float curveValue = curveData.curve.Evaluate (time);
//		Debug.Log("Curve value:" + curveValue + " transform:" + _targetTransform.name + " type:" + curvePropertyType.ToString());
		switch (curvePropertyType) {
		case CurvePropertyType.Local_PositionX:
			_targetTransform.localPosition = new Vector3 (curveValue, _targetTransform.localPosition.y, _targetTransform.localPosition.z);
			break;
		case CurvePropertyType.Local_PositionY:
			_targetTransform.localPosition = new Vector3 (_targetTransform.localPosition.x, curveValue, _targetTransform.localPosition.z);
			break;
		case CurvePropertyType.Local_PositionZ:
			_targetTransform.localPosition = new Vector3 (_targetTransform.localPosition.x, _targetTransform.localPosition.y, curveValue);
			break;
			
		case CurvePropertyType.Local_RotationX:
			_targetTransform.localRotation = new Quaternion (curveValue, _targetTransform.localRotation.y, _targetTransform.localRotation.z, _targetTransform.localRotation.w);
			break;
		case CurvePropertyType.Local_RotationY:
			_targetTransform.localRotation = new Quaternion (_targetTransform.localRotation.x, curveValue, _targetTransform.localRotation.z, _targetTransform.localRotation.w);
			break;
		case CurvePropertyType.Local_RotationZ:
			_targetTransform.localRotation = new Quaternion (_targetTransform.localRotation.x, _targetTransform.localRotation.y, curveValue, _targetTransform.localRotation.w);
			break;
		case CurvePropertyType.Local_RotationW:
			_targetTransform.localRotation= new Quaternion (_targetTransform.localRotation.x, _targetTransform.localRotation.y, _targetTransform.localRotation.w, curveValue);
			break;
			
		case CurvePropertyType.Local_ScaleX:
			_targetTransform.localScale = new Vector3  (curveValue, _targetTransform.localScale.y, _targetTransform.localScale.z);
			break;
		case CurvePropertyType.Local_ScaleY:
			_targetTransform.localScale = new Vector3  (_targetTransform.localScale.x, curveValue, _targetTransform.localScale.z);
			break;
		case CurvePropertyType.Local_ScaleZ:
			_targetTransform.localScale = new Vector3  (_targetTransform.localScale.x, _targetTransform.localScale.y, curveValue);
			break;
		}
	}
	
	public static CurvePropertyType CurvePropertyNameToType (string curveProperty)
	{
		//rotation
		if (curveProperty.ToLower ().Contains ("localrotation.x")) {
			return CurvePropertyType.Local_RotationX;
		}
		if (curveProperty.ToLower ().Contains ("localrotation.y")) {
			return CurvePropertyType.Local_RotationY;
		}
		if (curveProperty.ToLower ().Contains ("localrotation.z")) {
			return CurvePropertyType.Local_RotationZ;
		}
		if (curveProperty.ToLower ().Contains ("localrotation.w")) {
			return CurvePropertyType.Local_RotationW;
		}
		//position
		if (curveProperty.ToLower ().Contains ("localposition.x")) {
			return CurvePropertyType.Local_PositionX;
		}
		if (curveProperty.ToLower ().Contains ("localposition.y")) {
			return CurvePropertyType.Local_PositionY;
		}
		if (curveProperty.ToLower ().Contains ("localposition.z")) {
			return CurvePropertyType.Local_PositionZ;
		}
		//scale
		if (curveProperty.ToLower ().Contains ("localscale.x")) {
			return CurvePropertyType.Local_ScaleX;
		}
		if (curveProperty.ToLower ().Contains ("localscale.y")) {
			return CurvePropertyType.Local_ScaleY;
		}
		if (curveProperty.ToLower ().Contains ("localscale.z")) {
			return CurvePropertyType.Local_ScaleZ;
		}
		
		Debug.LogError ("Unknown CurveData property:" + curveProperty);
		return CurvePropertyType.Unknown;
	}
}

