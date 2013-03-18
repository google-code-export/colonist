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
			AnimationData.AnimationName = array [index];
			EditorGUILayout.LabelField (new GUIContent ("Animation length: " + gameObject.animation [AnimationData.AnimationName].length + " seconds.", "动画时长"));
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
			
			string []array_left = GetAnimationNames(gameObject,RotateData.RotateLeftAnimationName, out RotateLeftAnimationIndex);
			RotateLeftAnimationIndex = EditorGUILayout.Popup ("Rotate Left Animation:", RotateLeftAnimationIndex, array_left);
			if(RotateLeftAnimationIndex >= 0)
			{
				RotateData.RotateLeftAnimationName = array_left[RotateLeftAnimationIndex];
		        EditorGUILayout.LabelField (new GUIContent ("Animation length: " + gameObject.animation [RotateData.RotateLeftAnimationName].length + " seconds.", ""));
			}
			
			string []array_right = GetAnimationNames(gameObject,RotateData.RotateRightAnimationName, out RotateRightAnimationIndex);
			RotateRightAnimationIndex = EditorGUILayout.Popup ("Rotate Right Animation:", RotateRightAnimationIndex, array_right);
			if(RotateRightAnimationIndex >= 0)
			{
				RotateData.RotateRightAnimationName = array_right[RotateRightAnimationIndex];
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
		EditBasicAnimationData (gameObject,
				                string.Format (" ---------------------- {0}", MoveData.Name), 
				                MoveData as UnitAnimationData);
		MoveData.MoveSpeed = EditorGUILayout.FloatField (new GUIContent ("Speed:", "单位移动速度."), MoveData.MoveSpeed);
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
			AttackData AttackData = AttackDataArray [i];
			EditBasicAnimationData (unit.gameObject,
				                        string.Format (" ---------------------- {0}", AttackData.Name), 
				                        AttackData as UnitAnimationData);
			AttackData.DamageForm = (DamageForm)EditorGUILayout.EnumPopup (new GUIContent ("Damage Form:", "伤害类型"), AttackData.DamageForm);
			AttackData.AttackableRange = EditorGUILayout.FloatField (new GUIContent ("Attack range:", "攻击范围"), AttackData.AttackableRange);
			AttackData.AttackInterval = EditorGUILayout.FloatField (new GUIContent ("Attack Interval", "攻击间隔"), AttackData.AttackInterval);
			AttackData.DamagePointBase = EditorGUILayout.FloatField (new GUIContent ("Base Damage Point:", "基础伤害点数"), AttackData.DamagePointBase);
			EditorGUILayout.BeginHorizontal ();
			AttackData.MinDamageBonus = EditorGUILayout.FloatField (new GUIContent ("Min Damage Point Bonus:", ""), AttackData.MinDamageBonus);
			AttackData.MaxDamageBonus = AttackData.MaxDamageBonus >= AttackData.MinDamageBonus ?
                    AttackData.MaxDamageBonus : AttackData.MinDamageBonus;
			AttackData.MaxDamageBonus = EditorGUILayout.FloatField (new GUIContent ("Max Damage Point Bonus:", ""), AttackData.MaxDamageBonus);
			EditorGUILayout.EndHorizontal ();
			string DamageRange = (AttackData.DamagePointBase + AttackData.MinDamageBonus).ToString ()
                    + " ~ " + (AttackData.DamagePointBase + AttackData.MaxDamageBonus).ToString ();
			EditorGUILayout.LabelField (new GUIContent ("Damage range:" + DamageRange, "伤害点数范围"));
			AttackData.Type = (AIAttackType)EditorGUILayout.EnumPopup (new GUIContent ("AI Attack Type:", "攻击类型 - 立刻的/投射/区域"), AttackData.Type);
			switch (AttackData.Type) {
			case AIAttackType.Instant:
				AttackData.HitTime = EditorGUILayout.FloatField (new GUIContent ("Hit time:",
                            @"如果AttackType = Instant,HitTime表示发送Apply Damage的时间;
如果AttackType = Projectile, 表示创建Projectile的时间."),
                            AttackData.HitTime);
				AttackData.HitTestType = (HitTestType)EditorGUILayout.EnumPopup (new GUIContent ("*Hit Test Type:", "命中检测方式 - 一定命中/百分率/碰撞器校验/距离校验"), AttackData.HitTestType);
				switch (AttackData.HitTestType) {
				case HitTestType.AlwaysTrue:
					break;
				case HitTestType.HitRate:
					AttackData.HitRate = EditorGUILayout.FloatField (new GUIContent ("*Hit Rate:", "命中率: 0 - 1"), AttackData.HitRate);
					AttackData.HitRate = Mathf.Clamp01 (AttackData.HitRate);
					break;
				case HitTestType.CollisionTest:
					AttackData.HitTestCollider = (Collider)EditorGUILayout.ObjectField (new GUIContent ("*Hit Test Collider:", "Assign a collider to test if the target is hit."), AttackData.HitTestCollider, typeof(Collider));
					break;
				case HitTestType.DistanceTest:
					AttackData.HitTestDistance = EditorGUILayout.FloatField (new GUIContent ("*Hit Test Distance:", "命中校验距离: "), AttackData.HitTestDistance);
					break;
				case HitTestType.AngleTest:
					AttackData.HitTestAngularDiscrepancy = EditorGUILayout.FloatField (new GUIContent ("*Hit Test Angular Distance:", "命中校验角差值范围: "), AttackData.HitTestAngularDiscrepancy);
					break;
				case HitTestType.DistanceAndAngleTest:
					AttackData.HitTestDistance = EditorGUILayout.FloatField (new GUIContent ("*Hit Test Distance:", "命中校验距离: "), AttackData.HitTestDistance);
					AttackData.HitTestAngularDiscrepancy = EditorGUILayout.FloatField (new GUIContent ("*Hit Test Angular Distance:", "命中校验角差值范围: "), AttackData.HitTestAngularDiscrepancy);
					break;
				default:
					break;
				}
				break;
			case AIAttackType.Projectile:
				AttackData.Projectile = (Projectile)EditorGUILayout.ObjectField (new GUIContent ("*Projectile:", "射弹对象"), AttackData.Projectile, typeof(Projectile));
				AttackData.ProjectileInstantiateAnchor = (Transform)EditorGUILayout.ObjectField (new GUIContent ("*Projectile Instantiate Anchor :", ""), AttackData.ProjectileInstantiateAnchor, typeof(Transform));
				break;
			case AIAttackType.Regional:
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (new GUIContent ("HitTestType:", "Regional 攻击方式的命中检测必须是CollisionTest:"));
				AttackData.HitTestType = (HitTestType)EditorGUILayout.EnumPopup (AttackData.HitTestType);
				AttackData.HitTestType = HitTestType.CollisionTest;
				EditorGUILayout.EndHorizontal ();
				AttackData.HitTestCollider = (Collider)EditorGUILayout.ObjectField (new GUIContent ("*Hit Test Collider:", "Assign a collider to test if the target is hit."), AttackData.HitTestCollider, typeof(Collider));
				AttackData.HitTestDistance = EditorGUILayout.FloatField (new GUIContent ("*Hit Test Distance:", "Enemy in this range will be test by hit collider."), AttackData.HitTestDistance);
				break;
			}
			AttackData.ScriptObjectAttachToTarget = (MonoBehaviour)EditorGUILayout.ObjectField (new GUIContent ("Script attach to target:", "造成伤害时,自动附加该脚本组件."), AttackData.ScriptObjectAttachToTarget, typeof(MonoBehaviour));
			AttackDataArray [i] = AttackData;
			//Delete this attack data
			if (GUILayout.Button ("Delete " + AttackData.Name)) {
				AttackDataArray = Util.CloneExcept<AttackData> (AttackDataArray, AttackData);
			}
		}
		return AttackDataArray;
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
			ReceiveDamageData.HaltAI = EditorGUILayout.Toggle (new GUIContent ("HaltAI", "受到伤害时,是否停止AI,播放受伤动画?"), ReceiveDamageData.HaltAI);
			ReceiveDamageData.DamageForm = (DamageForm)EditorGUILayout.EnumPopup (new GUIContent ("Damage Form", "触发这个ReceiveDamage的DamageForm, Common类型是默认数据."), ReceiveDamageData.DamageForm);
			string[] effectDataNameArray = unit.EffectData.Select (x => x.Name).ToArray<string> ();
			ReceiveDamageData.EffectDataName = EditStringArray ("--------- Edit receive damage effect data-----", ReceiveDamageData.EffectDataName, effectDataNameArray);

			if (unit.DecalData != null && unit.DecalData.Length > 0) {
				string[] decalDataNameArray = unit.DecalData.Select (x => x.Name).ToArray<string> ();
				ReceiveDamageData.DecalDataName = EditStringArray ("--------- Edit receive damage decal data-----", ReceiveDamageData.DecalDataName, decalDataNameArray);
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
			EffectData EffectData = EffectDataArray [i];
			EditorGUILayout.LabelField ("------------------------ " + EffectData.Name);
			EffectData.Name = EditorGUILayout.TextField (new GUIContent ("Name", ""), EffectData.Name);
				
			EffectData.UseGlobalEffect = EditorGUILayout.Toggle (new GUIContent ("Use global effect?", ""), EffectData.UseGlobalEffect);
			
			EditorGUILayout.BeginHorizontal();
			EffectData.CreateDelay = EditorGUILayout.BeginToggleGroup(new GUIContent("Create in delay",""), EffectData.CreateDelay );
			if(EffectData.CreateDelay)
			{
				EffectData.CreateDelayTime = EditorGUILayout.FloatField(EffectData.CreateDelayTime);
			}
			EditorGUILayout.EndToggleGroup();
			EditorGUILayout.EndHorizontal();
			
			EffectData.Count = EditorGUILayout.IntField(new GUIContent("Number of effect object:",""), EffectData.Count);
			
			if (EffectData.UseGlobalEffect) {
				EffectData.GlobalType = (GlobalEffectType)EditorGUILayout.EnumPopup (new GUIContent ("Global effect type", "是全局Effect类型"),
						EffectData.GlobalType);
			} else {
				EffectData.DestoryInTimeOut = EditorGUILayout.Toggle (new GUIContent ("Auto Destory?", ""), EffectData.DestoryInTimeOut);
				if (EffectData.DestoryInTimeOut) {
					EffectData.DestoryTimeOut = EditorGUILayout.FloatField (new GUIContent ("LifeTime:", ""), EffectData.DestoryTimeOut);
				}
				EffectData.EffectObject = (GameObject)EditorGUILayout.ObjectField (new GUIContent ("Effect object", ""), EffectData.EffectObject, typeof(GameObject));
				EffectData.Anchor = (Transform)EditorGUILayout.ObjectField (new GUIContent ("Effect creation anchor", ""), EffectData.Anchor, typeof(Transform));
				//Delete this effect data
				if (GUILayout.Button ("Delete EffectData:" + EffectData.Name)) {
					EffectDataArray = Util.CloneExcept<EffectData> (EffectDataArray, EffectData);
				}
			}
			
			EditorGUILayout.Space ();
		}
		return EffectDataArray;
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
                    DeathData.ReplaceAfterAnimationFinish == true)) {
				EditBasicAnimationData (unit.gameObject,
					                        string.Format (" ---------------------- {0}", DeathData.Name), 
					                        DeathData as UnitAnimationData);
			} else {
				GUILayout.Label (string.Format (" ---------------------- {0}", DeathData.Name));
				DeathData.Name = EditorGUILayout.TextField (new GUIContent ("Name:", ""), DeathData.Name);
			}
			DeathData.DamageForm = (DamageForm)EditorGUILayout.EnumPopup (new GUIContent ("Damage Form", "触发这个DeathData的DamageForm, Common类型是默认数据."), DeathData.DamageForm);
			DeathData.UseDieReplacement = EditorGUILayout.Toggle (new GUIContent ("Use Die replacement:", "死亡时,是否创建替代布娃娃?"), DeathData.UseDieReplacement);
			if (DeathData.UseDieReplacement) {
				DeathData.ReplaceAfterAnimationFinish = EditorGUILayout.Toggle (new GUIContent ("Create replacement following animation", "等待动画播放完毕后再创建替代物?"), DeathData.ReplaceAfterAnimationFinish);
				DeathData.DieReplacement = (GameObject)EditorGUILayout.ObjectField (new GUIContent ("Die replacement:", ""), DeathData.DieReplacement, typeof(GameObject));
				DeathData.CopyChildrenTransformToDieReplacement = EditorGUILayout.Toggle (new GUIContent ("Copy transform?", "是否把替代物的关节位置调整到和死亡单位一致?"), DeathData.CopyChildrenTransformToDieReplacement);
			}
			string[] effectDataNameArray = unit.EffectData.Select (x => x.Name).ToArray<string> ();
			DeathData.EffectDataName = EditStringArray ("--------- Edit death effect data-----", DeathData.EffectDataName, effectDataNameArray);
			string[] decalDataNameArray = unit.DecalData.Select (x => x.Name).ToArray<string> ();
			DeathData.DecalDataName = EditStringArray ("--------- Edit death decal data-----", DeathData.DecalDataName, decalDataNameArray);
			//Delete DeathData
			if (GUILayout.Button ("Delete DeathData:" + DeathData.Name)) {
				DeathDataArray = Util.CloneExcept<DeathData> (DeathDataArray, DeathData);
			}

		}
		return DeathDataArray;
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
			//Object element = new Object ();
			//newArray = Util.AddToArray<Object> (element, newArray);
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
	
	public static AudioClip[] EditAudioClipArray (string label, AudioClip[] clipArray)
	{
		EditorGUILayout.LabelField (label);
		AudioClip[] newArray = clipArray;
		if (GUILayout.Button ("Add new audio clip element")) {
			AudioClip element = null;
			newArray = Util.AddToArray<AudioClip> (element, newArray);
		}
		if (newArray != null && newArray.Length > 0) {
			for (int i = 0; i < newArray.Length; i++) {
				EditorGUILayout.BeginHorizontal ();
				AudioClip element = newArray [i];
				element = (AudioClip)EditorGUILayout.ObjectField (element, typeof(AudioClip));
				if (GUILayout.Button ("Remove")) {
					newArray = Util.CloneExcept<AudioClip> (newArray, i);
					break;
				}
				newArray [i] = element;
				EditorGUILayout.EndHorizontal ();
			}
		}
		return newArray;
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
			array [i] = EditorGUILayout.TextField(array[i]);
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
	/// Given a value, and a string array, let the value selected from the drop down list composed of displayOption.
	/// </summary>
	public static string EditPopup (string label, string _value, string[] displayOption)
	{
		if(displayOption.Length > 0)
		{
		   int index = IndexOfArray<string> (displayOption, _value);
		   index = EditorGUILayout.Popup (label, index, displayOption);
		   return displayOption [index];
		}
		else 
		{
			EditorGUILayout.LabelField(label);
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
	
    #endregion
	
	
}

