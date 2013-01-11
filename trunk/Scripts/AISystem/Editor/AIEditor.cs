using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIEditor : EditorWindow
{

	[MenuItem("Component/AI/AIEditor")]	
	public static void EditAI ()
	{
		EditorWindow.GetWindow (typeof(AIEditor));
	}

	AI AI;
	bool EnableEditUnit = false, EnableEditIdleData = false, EnableEditAttackData = false,
         EnableEditMoveData = false, EnableEditEffectData = false, EnableEditReceiveDamageData = false,
         EnableEditDecalData = false, EnableEditDeathData = false, EnableEditAIBehavior = false;
	float MainWindowWidth, MainWindowHeight;
	Vector2 ScrollPosition = Vector2.zero;

	void OnGUI ()
	{
		MainWindowWidth = position.width;
		MainWindowHeight = position.height;
		GameObject selectedGameObject = Selection.activeGameObject;
		if (selectedGameObject == null) {
			Debug.LogWarning ("No gameObject is selected.");
			return;
		}
		//Attach AI script button
		if (selectedGameObject.GetComponent<AI> () == null) {
			Rect newAIScriptButton = new Rect (0, 0, MainWindowWidth - 10, 30);
			if (GUI.Button (newAIScriptButton, "Attach AI script")) {
				selectedGameObject.AddComponent<AI> ();
			}
			return;
		}

		if (GUILayout.Button ("Save object")) {
			EditorUtility.SetDirty (AI);
		}

		AI = selectedGameObject.GetComponent<AI> ();
		if (AI.Unit == null) {
			AI.Unit = AI.GetComponent<Unit> ();
		}
		ScrollPosition = EditorGUILayout.BeginScrollView (ScrollPosition, false, true, null);
#region Edit Unit
		EnableEditUnit = EditorGUILayout.BeginToggleGroup ("Edit Unit", EnableEditUnit);
		if (EnableEditUnit) {
			EditBasicUnitProperty ();
			//Edit Idle Data 
			EditIdleData ();

			//Edit Move Data 
			EditMoveData ();

			//Edit attack data
			EditAttackData ();

			//Edit Effect Data
			EditEffectData ();

			//Edit Decal data
			EditDecalData ();
			//Edit receive damage data:
			EditReceiveDamageData ();

			//Edit death data
			EditDeathData ();
		}
		EditorGUILayout.EndToggleGroup ();
        #endregion
        
#region Edit AI
		EnableEditAIBehavior = EditorGUILayout.BeginToggleGroup ("Edit AI", EnableEditAIBehavior);
		if (EnableEditAIBehavior) {
			EditBaseAIProperty ();
			EditorGUILayout.LabelField ("-------------------------Edit AI behavior---------------");
			if (GUILayout.Button ("Add new AI behavior")) {
				AIBehavior AIBehavior = new AIBehavior ();
				IList<AIBehavior> l = AI.Behaviors.ToList<AIBehavior> ();
				l.Add (AIBehavior);
				AI.Behaviors = l.ToArray<AIBehavior> ();
			}
			for (int i = 0; i < AI.Behaviors.Length; i++) {
				AIBehavior behavior = AI.Behaviors [i];
				EditAIBehavior (behavior);
			}
		}
		EditorGUILayout.EndToggleGroup ();
#endregion
		EditorGUILayout.EndScrollView ();
	}

    #region Edit Unit property
	public virtual void EditBasicUnitProperty ()
	{
		GUILayout.Label (new GUIContent ("Edit Unit Basic Property------------------------------------------", "在此编辑单位的基础属	性"));
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Unit name:");
		AI.Unit.Name = GUILayout.TextField (AI.Unit.Name); 
		GUILayout.EndHorizontal ();
		AI.Unit.MaxHP = EditorGUILayout.FloatField (new GUIContent ("Max HP:", "这个单位的最大生命值"), AI.Unit.MaxHP);
		GUILayout.BeginHorizontal ();
		GUILayout.Label (new GUIContent ("Enemy layer:", "所有处于在EnemyLayer层的单位能被这个单位检测到."));
		AI.Unit.EnemyLayer = EditorGUILayoutx.LayerMaskField ("", AI.Unit.EnemyLayer, true);
		GUILayout.EndHorizontal ();
	}

	public virtual void EditBasicAnimationData (string StartLabel, UnitAnimationData AnimationData)
	{
		EditorGUILayout.Space ();
		GUILayout.Label (StartLabel);
		AnimationData.Name = EditorGUILayout.TextField (new GUIContent ("Name:", ""), AnimationData.Name);
		if (AnimationData.AnimationName == string.Empty) {
			string[] array = GetAnimationNames (AI.gameObject);
			int index = 0;
			index = EditorGUILayout.Popup ("Animation:", index, array);
			AnimationData.AnimationName = array [index];
		} else {
			int index = 0;
			string[] array = GetAnimationNames (AI.gameObject, AnimationData.AnimationName, out index);
			index = EditorGUILayout.Popup ("Animation:", index, array);
			AnimationData.AnimationName = array [index];
			EditorGUILayout.LabelField (new GUIContent ("Animation length:         " + AI.animation [AnimationData.AnimationName].length + " seconds.", "动画时长"));
		}
		AnimationData.AnimationLayer = EditorGUILayout.IntField (new GUIContent ("Animation Layer", "在此编辑这个Idle动画的层"), AnimationData.AnimationLayer);
		AnimationData.AnimationSpeed = EditorGUILayout.FloatField (new GUIContent ("Animation Speed", "在此编辑这个Idle动画的播放速度"), AnimationData.AnimationSpeed);
		AnimationData.AnimationWrapMode = (WrapMode)EditorGUILayout.EnumPopup (new GUIContent ("WrapMode:", "动画的WrapMode"), AnimationData.AnimationWrapMode);
	}

	public virtual void EditIdleData ()
	{
		EnableEditIdleData = EditorGUILayout.BeginToggleGroup ("---Edit Idle Data", EnableEditIdleData);
		if (EnableEditIdleData) {
			if (GUILayout.Button ("Add Idle data")) {
				IdleData IdleData = new IdleData ();
				IList<IdleData> l = AI.Unit.IdleData.ToList<IdleData> ();
				l.Add (IdleData);
				AI.Unit.IdleData = l.ToArray<IdleData> ();
			}
			UnitAnimationData[] UnitAnimationDataArray = AI.Unit.IdleData.ToArray<UnitAnimationData> ();
			for (int i = 0; i < AI.Unit.IdleData.Length; i++) {
				IdleData IdleData = AI.Unit.IdleData [i];
				EditBasicAnimationData (string.Format (" ---------------------- {0}", IdleData.Name), IdleData as UnitAnimationData);

				//Delete this data
				if (GUILayout.Button ("Delete " + IdleData.Name)) {
					IList<IdleData> l = AI.Unit.IdleData.ToList<IdleData> ();
					l.Remove (IdleData);
					AI.Unit.IdleData = l.ToArray<IdleData> ();
				}
				EditorGUILayout.Space ();
			}
		}
		EditorGUILayout.EndToggleGroup ();
	}

	public virtual void EditMoveData ()
	{
		EnableEditMoveData = EditorGUILayout.BeginToggleGroup ("---Edit Move Data", EnableEditMoveData);
		if (EnableEditMoveData) {
			if (GUILayout.Button ("Add Move data")) {
				MoveData MoveData = new MoveData ();
				IList<MoveData> l = AI.Unit.MoveData.ToList<MoveData> ();
				l.Add (MoveData);
				AI.Unit.MoveData = l.ToArray<MoveData> ();
			}
			for (int i = 0; i < AI.Unit.MoveData.Length; i++) {
				MoveData MoveData = AI.Unit.MoveData [i];
				EditBasicAnimationData (string.Format (" ---------------------- {0}", MoveData.Name), MoveData as UnitAnimationData);
				MoveData.MoveSpeed = EditorGUILayout.FloatField (new GUIContent ("Speed:", "单位移动速度"), MoveData.MoveSpeed);
				MoveData.CanRotate = EditorGUILayout.Toggle (new GUIContent ("CanRotate:", "单位移动的时候,是否朝向前进方向"), MoveData.CanRotate);
				if (MoveData.CanRotate) {
					MoveData.SmoothRotate = EditorGUILayout.Toggle (new GUIContent ("SmoothRotate:", "单位移动转向的时候,是否用角速度自动平滑转向动作."), MoveData.SmoothRotate);
					if (MoveData.SmoothRotate) {
						MoveData.RotateAngularSpeed = EditorGUILayout.FloatField (new GUIContent ("Angular Speed:", "单位旋转角速度"), MoveData.RotateAngularSpeed);
					}
				}
				//Delete this move data
				if (GUILayout.Button ("Delete " + MoveData.Name)) {
					IList<MoveData> l = AI.Unit.MoveData.ToList<MoveData> ();
					l.Remove (MoveData);
					AI.Unit.MoveData = l.ToArray<MoveData> ();
				}
			}
		}
		EditorGUILayout.EndToggleGroup ();
	}

	public virtual void EditAttackData ()
	{
		EnableEditAttackData = EditorGUILayout.BeginToggleGroup ("---Edit Attack Data", EnableEditAttackData);
		if (EnableEditAttackData) {
			if (GUILayout.Button ("Add Attack data")) {
				AttackData AttackData = new AttackData ();
				IList<AttackData> l = AI.Unit.AttackData.ToList<AttackData> ();
				l.Add (AttackData);
				AI.Unit.AttackData = l.ToArray<AttackData> ();
			}
			for (int i = 0; i < AI.Unit.AttackData.Length; i++) {
				AttackData AttackData = AI.Unit.AttackData [i];
				EditBasicAnimationData (string.Format (" ---------------------- {0}", AttackData.Name), AttackData as UnitAnimationData);
				AttackData.DamageForm = (DamageForm)EditorGUILayout.EnumPopup (new GUIContent ("Damage Form:", "伤害类型"), AttackData.DamageForm);
				AttackData.AttackableRange = EditorGUILayout.FloatField (new GUIContent ("Attack range:", "攻击范围"), AttackData.AttackableRange);
				AttackData.AttackInterval = EditorGUILayout.FloatField (new GUIContent ("Attack Interval", "攻击间隔"), AttackData.AttackInterval);
				AttackData.DamagePointBase = EditorGUILayout.FloatField (new GUIContent ("Base Damage Point:", "基础伤害点数"), AttackData.DamagePointBase);
				EditorGUILayout.BeginHorizontal ();
				AttackData.MinDamageBonus = EditorGUILayout.FloatField (new GUIContent ("Min Damage Point Bonus:", "最低伤害加成"), AttackData.MinDamageBonus);
				AttackData.MaxDamageBonus = AttackData.MaxDamageBonus >= AttackData.MinDamageBonus ?
                    AttackData.MaxDamageBonus : AttackData.MinDamageBonus;
				AttackData.MaxDamageBonus = EditorGUILayout.FloatField (new GUIContent ("Max Damage Point Bonus:", "最高伤害加成"), AttackData.MaxDamageBonus);
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
						AttackData.HitTestCollider = (Collider)EditorGUILayout.ObjectField (new GUIContent ("*Hit Test Collider:", "命中校验碰撞器"), AttackData.HitTestCollider, typeof(Collider));
						break;
					case HitTestType.DistanceTest:
						AttackData.HitTestDistance = EditorGUILayout.FloatField (new GUIContent ("*Hit Test Distance:", "命中校验距离: "), AttackData.HitTestDistance);
						break;
					default:
						break;
					}
					break;
				case AIAttackType.Projectile:
					AttackData.Projectile = (Projectile)EditorGUILayout.ObjectField (new GUIContent ("*Projectile:", "射弹对象"), AttackData.Projectile, typeof(Projectile));
					AttackData.ProjectileInstantiateAnchor = (Transform)EditorGUILayout.ObjectField (new GUIContent ("*Projectile Instantiate Anchor :", "ŽŽœšÉäµ¯¶ÔÏóµÄTransform"), AttackData.ProjectileInstantiateAnchor, typeof(Transform));
					break;
				case AIAttackType.Regional:
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField (new GUIContent ("HitTestType:", "Regional 攻击方式的命中检测必须是CollisionTest:"));
					AttackData.HitTestType = (HitTestType)EditorGUILayout.EnumPopup (AttackData.HitTestType);
					AttackData.HitTestType = HitTestType.CollisionTest;
					EditorGUILayout.EndHorizontal ();
					AttackData.HitTestCollider = (Collider)EditorGUILayout.ObjectField (new GUIContent ("*Hit Test Collider:", "命中校验碰撞器"), AttackData.HitTestCollider, typeof(Collider));
					AttackData.HitTestDistance = EditorGUILayout.FloatField (new GUIContent ("*Hit Test Distance:", "在此距离内的敌人会被碰撞器校验: "), AttackData.HitTestDistance);
					break;
				}
				AttackData.ScriptObjectAttachToTarget = (MonoBehaviour)EditorGUILayout.ObjectField (new GUIContent ("Script attach to target:", "造成伤害时,自动附加该脚本组件."), AttackData.ScriptObjectAttachToTarget, typeof(MonoBehaviour));

				//Delete this attack data
				if (GUILayout.Button ("Delete " + AttackData.Name)) {
					IList<AttackData> l = AI.Unit.AttackData.ToList<AttackData> ();
					l.Remove (AttackData);
					AI.Unit.AttackData = l.ToArray<AttackData> ();
				}
			}
		}
		EditorGUILayout.EndToggleGroup ();
	}

	public virtual void EditDecalData ()
	{
		EnableEditDecalData = EditorGUILayout.BeginToggleGroup ("---Edit Decal Data", EnableEditDecalData);
		if (EnableEditDecalData) {
			if (GUILayout.Button ("Add Decal data")) {
				DecalData DecalData = new DecalData ();
				IList<DecalData> l = AI.Unit.DecalData.ToList<DecalData> ();
				l.Add (DecalData);
				AI.Unit.DecalData = l.ToArray<DecalData> ();
			}
			for (int i = 0; i < AI.Unit.DecalData.Length; i++) {
				DecalData DecalData = AI.Unit.DecalData [i];
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
					//Delete this DecalData
					if (GUILayout.Button ("Delete DecalData:" + DecalData.Name)) {
						IList<DecalData> l = AI.Unit.DecalData.ToList<DecalData> ();
						l.Remove (DecalData);
						AI.Unit.DecalData = l.ToArray<DecalData> ();
					}
				}
				EditorGUILayout.Space ();
			}
		}
		EditorGUILayout.EndToggleGroup ();
	}

	public virtual void EditEffectData ()
	{
		EnableEditEffectData = EditorGUILayout.BeginToggleGroup ("---Edit Effect Data", EnableEditEffectData);
		if (EnableEditEffectData) {
			if (GUILayout.Button ("Add Effect data")) {
				EffectData EffectData = new EffectData ();
				IList<EffectData> l = AI.Unit.EffectData.ToList<EffectData> ();
				l.Add (EffectData);
				AI.Unit.EffectData = l.ToArray<EffectData> ();
			}
			for (int i = 0; i < AI.Unit.EffectData.Length; i++) {
				EffectData EffectData = AI.Unit.EffectData [i];
				EditorGUILayout.LabelField ("------------------------ " + EffectData.Name);
				EffectData.Name = EditorGUILayout.TextField (new GUIContent ("Name", ""), EffectData.Name);
				
				EffectData.UseGlobalEffect = EditorGUILayout.Toggle (new GUIContent ("Use global effect?", "是否使用全局Effect?"), EffectData.UseGlobalEffect);
				if(EffectData.UseGlobalEffect)
				{
					EffectData.GlobalType = (GlobalEffectType)EditorGUILayout.EnumPopup(new GUIContent ("Global effect type", "是全局Effect类型"),
						EffectData.GlobalType);
				}
				else {
				  EffectData.DestoryInTimeOut = EditorGUILayout.Toggle (new GUIContent ("Destory this effect in timeout?", "是否在N秒内删除这个效果?"), EffectData.DestoryInTimeOut);
				  if (EffectData.DestoryInTimeOut) {
				    	EffectData.DestoryTimeOut = EditorGUILayout.FloatField (new GUIContent ("Destory Timeout:", ""), EffectData.DestoryTimeOut);
				  }
				  EffectData.EffectObject = (GameObject)EditorGUILayout.ObjectField (new GUIContent ("Effect object", "特效对象"), EffectData.EffectObject, typeof(GameObject));
				  EffectData.Anchor = (Transform)EditorGUILayout.ObjectField (new GUIContent ("Effect creation anchor", "创建特效对象的锚点"), EffectData.Anchor, typeof(Transform));
				  //Delete this attack data
				  if (GUILayout.Button ("Delete EffectData:" + EffectData.Name)) {
  					  IList<EffectData> l = AI.Unit.EffectData.ToList<EffectData> ();  
					  l.Remove (EffectData);
				  	  AI.Unit.EffectData = l.ToArray<EffectData> ();
				  }
				}
				EditorGUILayout.Space ();
			}
		}
		EditorGUILayout.EndToggleGroup ();
	}

	public virtual void EditReceiveDamageData ()
	{
		EnableEditReceiveDamageData = EditorGUILayout.BeginToggleGroup ("---Edit Receive Damage Data", EnableEditReceiveDamageData);
		if (EnableEditReceiveDamageData) {
			if (GUILayout.Button ("Add ReceiveDamage data")) {
				ReceiveDamageData receiveDamageData = new ReceiveDamageData ();
				IList<ReceiveDamageData> l = AI.Unit.ReceiveDamageData.ToList<ReceiveDamageData> ();
				l.Add (receiveDamageData);
				AI.Unit.ReceiveDamageData = l.ToArray<ReceiveDamageData> ();
			}
			for (int i = 0; i < AI.Unit.ReceiveDamageData.Length; i++) {
				ReceiveDamageData ReceiveDamageData = AI.Unit.ReceiveDamageData [i];
				if (ReceiveDamageData.HaltAI) {
					EditBasicAnimationData (string.Format (" ---------------------- {0}", ReceiveDamageData.Name), ReceiveDamageData as UnitAnimationData);
				} else {
					GUILayout.Label (string.Format (" ---------------------- {0}", ReceiveDamageData.Name));
					ReceiveDamageData.Name = EditorGUILayout.TextField (new GUIContent ("Name:", ""), ReceiveDamageData.Name);
				}
				ReceiveDamageData.HaltAI = EditorGUILayout.Toggle (new GUIContent ("HaltAI", "受到伤害时,是否停止AI,播放受伤动画?"), ReceiveDamageData.HaltAI);
				ReceiveDamageData.DamageForm = (DamageForm)EditorGUILayout.EnumPopup (new GUIContent ("Damage Form", "触发这个ReceiveDamage的DamageForm, Common类型是默认数据."), ReceiveDamageData.DamageForm);
				string[] effectDataNameArray = AI.Unit.EffectData.Select (x => x.Name).ToArray<string> ();
				ReceiveDamageData.EffectDataName = EditStringArray ("--------- Edit receive damage effect data-----", ReceiveDamageData.EffectDataName, effectDataNameArray);

				if (AI.Unit.DecalData != null && AI.Unit.DecalData.Length > 0) {
					string[] decalDataNameArray = AI.Unit.DecalData.Select (x => x.Name).ToArray<string> ();
					ReceiveDamageData.DecalDataName = EditStringArray ("--------- Edit receive damage decal data-----", ReceiveDamageData.DecalDataName, decalDataNameArray);
				}
                
                
				AI.Unit.ReceiveDamageData [i] = ReceiveDamageData;
				//Delete ReceiveDamageData
				if (GUILayout.Button ("Delete ReceiveDamageData:" + ReceiveDamageData.Name)) {
					IList<ReceiveDamageData> l = AI.Unit.ReceiveDamageData.ToList<ReceiveDamageData> ();
					l.Remove (ReceiveDamageData);
					AI.Unit.ReceiveDamageData = l.ToArray<ReceiveDamageData> ();
				}
				EditorGUILayout.Space ();
			}
		}
		EditorGUILayout.EndToggleGroup ();
	}

	public virtual void EditDeathData ()
	{
		EnableEditDeathData = EditorGUILayout.BeginToggleGroup ("---Edit Death Data", EnableEditDeathData);
		if (EnableEditDeathData) {
			if (GUILayout.Button ("Add death data")) {
				DeathData DeathData = new DeathData ();
				IList<DeathData> l = AI.Unit.DeathData.ToList<DeathData> ();
				l.Add (DeathData);
				AI.Unit.DeathData = l.ToArray<DeathData> ();
			}
			for (int i = 0; i < AI.Unit.DeathData.Length; i++) {
				DeathData DeathData = AI.Unit.DeathData [i];
				//Death animation is used only when: 1. there is no ragdoll, or 2.create ragdoll, after animation finishes.
				if (DeathData.UseDieReplacement == false ||
                   (DeathData.UseDieReplacement == true &&
                    DeathData.ReplaceAfterAnimationFinish == true)) {
					EditBasicAnimationData (string.Format (" ---------------------- {0}", DeathData.Name), DeathData as UnitAnimationData);
				} else {
					GUILayout.Label (string.Format (" ---------------------- {0}", DeathData.Name));
					DeathData.Name = EditorGUILayout.TextField (new GUIContent ("Name:", ""), DeathData.Name);
				}
				DeathData.DamageForm = (DamageForm)EditorGUILayout.EnumPopup (new GUIContent ("Damage Form", "触发这个DeathData的DamageForm, Common类型是默认数据."), DeathData.DamageForm);
				DeathData.UseDieReplacement = EditorGUILayout.Toggle (new GUIContent ("Use Die replacement:", "死亡时,是否创建替代布娃娃?"), DeathData.UseDieReplacement);
				if (DeathData.UseDieReplacement) {
					DeathData.ReplaceAfterAnimationFinish = EditorGUILayout.Toggle (new GUIContent ("Create replacement following animation", "ÊÇ·ñÔÚ¶¯»­œáÊøÖ®ºóŽŽœšÌæŽúÎï?"), DeathData.ReplaceAfterAnimationFinish);
					DeathData.DieReplacement = (GameObject)EditorGUILayout.ObjectField (new GUIContent ("Die replacement:", ""), DeathData.DieReplacement, typeof(GameObject));
					DeathData.CopyChildrenTransformToDieReplacement = EditorGUILayout.Toggle (new GUIContent ("Copy transform?", "是否把替代物的关节位置调整到和死亡单位一致?"), DeathData.CopyChildrenTransformToDieReplacement);
				}
				string[] effectDataNameArray = AI.Unit.EffectData.Select (x => x.Name).ToArray<string> ();
				DeathData.EffectDataName = EditStringArray ("--------- Edit death effect data-----", DeathData.EffectDataName, effectDataNameArray);
				string[] decalDataNameArray = AI.Unit.DecalData.Select (x => x.Name).ToArray<string> ();
				DeathData.DecalDataName = EditStringArray ("--------- Edit death decal data-----", DeathData.DecalDataName, decalDataNameArray);
				//Delete DeathData
				if (GUILayout.Button ("Delete DeathData:" + DeathData.Name)) {
					IList<DeathData> l = AI.Unit.DeathData.ToList<DeathData> ();
					l.Remove (DeathData);
					AI.Unit.DeathData = l.ToArray<DeathData> ();
				}

			}
		}
		EditorGUILayout.EndToggleGroup ();
	}

    #endregion

    #region Edit AI Behavior property

	public virtual void EditBaseAIProperty ()
	{
		EditorGUILayout.LabelField (new GUIContent ("------------- AI Base property", ""));
		AI.OffensiveRange = EditorGUILayout.FloatField (new GUIContent ("AI Offensive range", "当敌人进入Offsensive range, AI会主动发起进攻."), AI.OffensiveRange);
		AI.DetectiveRange = EditorGUILayout.FloatField (new GUIContent ("AI Detective range", "当敌人进入Detective range, AI会监测到这个敌人.DetectiveRange应该大于Offensive Range."), AI.DetectiveRange);
		AI.DetectiveRange = AI.DetectiveRange >= AI.OffensiveRange ? AI.DetectiveRange : AI.OffensiveRange;
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label (new GUIContent ("Attack Obstacle:", "攻击障碍物层,如果目标和AI之间存在该层的碰撞器,则AI无法攻击到目标."));
		AI.AttackObstacle = EditorGUILayoutx.LayerMaskField ("", AI.AttackObstacle);
		EditorGUILayout.EndHorizontal ();
		AI.AlterBehaviorInterval = EditorGUILayout.FloatField (new GUIContent ("Behavior alternation time",
            "AIÐÐÎªÂÖÌæÖÜÆÚ,²»ÍÆŒöÐÞžÄÕâžöÖµ."), AI.AlterBehaviorInterval);
	}

	public virtual void EditAIBehavior (AIBehavior behavior)
	{
		EditorGUILayout.LabelField (new GUIContent ("------------- Edit AI Behavior: " + behavior.Name + " ----------------------", ""));
		behavior.Name = EditorGUILayout.TextField (new GUIContent ("Behavior Name:", ""), behavior.Name);
		behavior.Type = (AIBehaviorType)EditorGUILayout.EnumPopup (new GUIContent ("Behavior type:", ""), behavior.Type);
		behavior.Priority = EditorGUILayout.IntField (new GUIContent ("Priority:", " 行为优先级,每个行为必须有独立的优先级,优先级不能冲突."), behavior.Priority);
		if (AI.Behaviors.Where (x => x.Priority == behavior.Priority).Count () > 1) {
			EditorGUILayout.LabelField (new GUIContent ("!!! You can not have more than one behavior in priority:" + behavior.Priority));
		}
		behavior.SelectTargetRule = (SelectTargetRule)EditorGUILayout.EnumPopup (new GUIContent ("Select enemy rule:", "当这个行为生效的时候,选择敌人的规则, 默认是Closest,也就是选择最近的敌人做为当前目标."), behavior.SelectTargetRule);
		//Edit behavior data
		EditAIBehaviorData (behavior);

		//Edit Start condition
		EditorGUILayout.LabelField (new GUIContent (" --- Edit Start Condition of behavior - " + behavior.Name, ""));
		EditAIBehaviorCondition (behavior, behavior.StartCondition);
		EditorGUILayout.Space ();

		//Edit End condition
		EditorGUILayout.LabelField (new GUIContent (" --- Edit End Condition of behavior - " + behavior.Name, ""));
		EditAIBehaviorCondition (behavior, behavior.EndCondition);
		if (GUILayout.Button ("Delete " + behavior.Type.ToString () + " behavior: " + behavior.Name)) {
			IList<AIBehavior> l = AI.Behaviors.ToList<AIBehavior> ();
			l.Remove (behavior);
			AI.Behaviors = l.ToArray<AIBehavior> ();
		}
		EditorGUILayout.Space ();
	}

	public virtual void EditAIBehaviorData (AIBehavior behavior)
	{
		string[] IdleDataName = AI.Unit.IdleData.Select (x => x.Name).ToArray<string> ();
		string[] AttackDataName = AI.Unit.AttackData.Select (x => x.Name).ToArray<string> ();
		string[] MoveDataName = AI.Unit.MoveData.Select (x => x.Name).ToArray<string> ();
		int idx = 0;
		switch (behavior.Type) {
		case AIBehaviorType.Idle:
			if (IdleDataName == null || IdleDataName.Length == 0) {
				EditorGUILayout.LabelField ("!!!There is no Idle Data defined in this Unit!!!");
			} else {
				idx = IndexOfArray<string> (IdleDataName, behavior.IdleDataName);
				idx = EditorGUILayout.Popup ("Use Idle data:", idx, IdleDataName);
				behavior.IdleDataName = IdleDataName [idx];
			}
			break;
		case AIBehaviorType.MoveToTransform:
			if (MoveDataName == null || MoveDataName.Length == 0) {
				EditorGUILayout.LabelField ("!!!There is no Move Data defined in this Unit!!!");
			} else {
				idx = IndexOfArray<string> (MoveDataName, behavior.MoveDataName);
				idx = EditorGUILayout.Popup ("Use Move data:", idx, MoveDataName);
				behavior.MoveDataName = MoveDataName [idx];
				behavior.MoveToTarget = (Transform)EditorGUILayout.ObjectField (new GUIContent ("Move to target", ""),
                        behavior.MoveToTarget, typeof(Transform));
			}
			break;
		case AIBehaviorType.MoveAtDirection:
			if (MoveDataName == null || MoveDataName.Length == 0) {
				EditorGUILayout.LabelField ("!!!There is no Move Data defined in this Unit!!!");
			} else {
				idx = IndexOfArray<string> (MoveDataName, behavior.MoveDataName);
				idx = EditorGUILayout.Popup ("Use Move data:", idx, MoveDataName);
				behavior.MoveDataName = MoveDataName [idx];
				behavior.MoveDirection = EditorGUILayout.Vector3Field ("Move at direction", behavior.MoveDirection);
				behavior.IsWorldDirection = EditorGUILayout.Toggle (new GUIContent ("Is world direction?", "Move at Direction 指定的方向,是世界方向还是局部方向?"), behavior.IsWorldDirection);
			}
			break;
		case AIBehaviorType.Attack:
		case AIBehaviorType.AttackToPosition:
		case AIBehaviorType.AttackToDirection:
		case AIBehaviorType.HoldPosition:
			if (AttackDataName == null || AttackDataName.Length == 0) {
				EditorGUILayout.LabelField ("!!!There is no Attack Data defined in this Unit!!!");
				return;
			}
			if (MoveDataName == null || MoveDataName.Length == 0) {
				EditorGUILayout.LabelField ("!!!There is no Move Data defined in this Unit!!!");
				return;
			}
                //Attack Data:
			idx = IndexOfArray<string> (AttackDataName, behavior.AttackDataName);
			idx = EditorGUILayout.Popup ("Attack data:", idx, AttackDataName);
			behavior.AttackDataName = AttackDataName [idx];
                // Move data:
			idx = IndexOfArray<string> (MoveDataName, behavior.MoveDataName);
			idx = EditorGUILayout.Popup ("Move data:", idx, MoveDataName);
			behavior.MoveDataName = MoveDataName [idx];

			if (behavior.Type == AIBehaviorType.AttackToPosition) {
				behavior.MoveToTarget = (Transform)EditorGUILayout.ObjectField (new GUIContent ("Move to target", ""),
                        behavior.MoveToTarget, typeof(Transform));
			}
			if (behavior.Type == AIBehaviorType.AttackToDirection) {
				behavior.MoveDirection = EditorGUILayout.Vector3Field ("Move at direction", behavior.MoveDirection);
				behavior.IsWorldDirection = EditorGUILayout.Toggle (new GUIContent ("Is world direction?", "Move at Direction Öž¶šµÄ·œÏò,ÊÇÊÀœç·œÏò»¹ÊÇŸÖ²¿·œÏò?"), behavior.IsWorldDirection);
			}
			if (behavior.Type == AIBehaviorType.HoldPosition) {
				behavior.HoldRadius = EditorGUILayout.FloatField (new GUIContent ("Hold Position:", "ŒáÊØÕóµØµÄ·¶Î§"), behavior.HoldRadius);
			}
			break;
		}
	}

	public virtual void EditAIBehaviorCondition (AIBehavior behavior, AIBehaviorCondition Condition)
	{
		EditorGUILayout.BeginHorizontal ();
		Condition.Conjunction = (LogicConjunction)EditorGUILayout.EnumPopup (new GUIContent ("ConditionData1 ", ""), Condition.Conjunction);
		if (Condition.Conjunction == LogicConjunction.Or || Condition.Conjunction == LogicConjunction.And) {
			EditorGUILayout.LabelField ("ConditionData2");
		}
		EditorGUILayout.EndHorizontal ();
      
		EditorGUILayout.LabelField ("----ConditionData1");
		EditAIBehaviorConditionData (Condition.ConditionData1);

		if (Condition.Conjunction == LogicConjunction.And || Condition.Conjunction == LogicConjunction.Or) {
			EditorGUILayout.LabelField ("----ConditionData2");
			EditAIBehaviorConditionData (Condition.ConditionData2);
		}
	}

	public virtual void EditAIBehaviorConditionData (ConditionData ConditionData)
	{
		ConditionData.ConditionType = (AIBehaviorConditionType)EditorGUILayout.EnumPopup (new GUIContent ("Condition type:", ""), ConditionData.ConditionType);
		switch (ConditionData.ConditionType) {
		case AIBehaviorConditionType.Boolean:
			EditBooleanConditionData (ConditionData);
			break;
		case AIBehaviorConditionType.ValueComparision:
			EditValueComparisionConditionData (ConditionData);
			break;
		}
	}

	public virtual void EditBooleanConditionData (ConditionData ConditionData)
	{
		EditorGUILayout.BeginHorizontal ();
		ConditionData.BooleanCondition = (AIBooleanConditionEnum)EditorGUILayout.EnumPopup (ConditionData.BooleanCondition);
		ConditionData.BooleanOperator = (BooleanComparisionOperator)EditorGUILayout.EnumPopup (ConditionData.BooleanOperator);
		switch (ConditionData.BooleanCondition) {
		case AIBooleanConditionEnum.AlwaysTrue:
			break;
		case AIBooleanConditionEnum.CurrentTargetInLayer:
			ConditionData.LayerMaskForComparision = EditorGUILayoutx.LayerMaskField ("Current target in layermask:", ConditionData.LayerMaskForComparision);
			break;
		case AIBooleanConditionEnum.EnemyInDetectiveRange:
			break;
		case AIBooleanConditionEnum.EnemyInOffensiveRange:
			break;
		case AIBooleanConditionEnum.InArea:
			EditorGUILayout.LabelField (new GUIContent ("Use inspector to assign Area !", "AIEditor 暂不支持编辑这个字段."));
			break;
		}
		EditorGUILayout.EndHorizontal ();
	}

	public virtual void EditValueComparisionConditionData (ConditionData ConditionData)
	{
		EditorGUILayout.BeginHorizontal ();
		ConditionData.ValueComparisionCondition = (AIValueComparisionCondition)EditorGUILayout.EnumPopup (ConditionData.ValueComparisionCondition);
		ConditionData.ValueOperator = (ValueComparisionOperator)EditorGUILayout.EnumPopup (ConditionData.ValueOperator);
		switch (ConditionData.ValueComparisionCondition) {
		case AIValueComparisionCondition.BehaviorLastExecutionInterval:
		case AIValueComparisionCondition.CurrentTagetDistance:
		case AIValueComparisionCondition.FarestEnemyDistance:
		case AIValueComparisionCondition.NearestEnemyDistance:
			ConditionData.RightValueForComparision = EditorGUILayout.FloatField (ConditionData.RightValueForComparision);
			break;
		case AIValueComparisionCondition.RandomValue:
			ConditionData.RightValueForComparision = EditorGUILayout.Slider (ConditionData.RightValueForComparision, 0, 100);
			break;
		case AIValueComparisionCondition.CurrentTargetHPPercentage:
		case AIValueComparisionCondition.HPPercentage:
			ConditionData.RightValueForComparision = EditorGUILayout.Slider (ConditionData.RightValueForComparision, 0, 1);
			break;
		case AIValueComparisionCondition.ExeuctionCount:
			ConditionData.RightValueForComparision = EditorGUILayout.IntField ((int)ConditionData.RightValueForComparision);
			break;
		}
		EditorGUILayout.EndHorizontal ();
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

	public static Object[] EditObjectArray (string label, Object[] Array)
	{
		EditorGUILayout.LabelField (label);
		Object[] newArray = Array;
		if (GUILayout.Button ("Add new object element")) {
			Object element = new Object ();
			newArray = Util.AddToArray<Object> (element, newArray);
		}

		for (int i = 0; i < Array.Length; i++) {
			EditorGUILayout.BeginHorizontal ();
			Object element = Array [i];
			element = EditorGUILayout.ObjectField (element, typeof(Object));
			if (GUILayout.Button ("Remove")) {
				newArray = Util.CloneExcept<Object> (newArray, i);
				break;
			}
			Array [i] = element;
			EditorGUILayout.EndHorizontal ();
		}
		return newArray;
	}

	/// <summary>
	/// Edit a string array.
	/// array - the array to edit.
	/// displayOption - the popup group to let user select.
	/// </summary>
	/// <param name="label"></param>
	/// <param name="array"></param>
	/// <param name="displayOption"></param>
	/// <returns></returns>
	public string[] EditStringArray (string label, string[] array, string[] displayOption)
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

    #endregion
}
