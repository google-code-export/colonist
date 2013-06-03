using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Predator3rdPersonalUnit))]
public class Editor_Predator3rdPersonalUnit : Editor
{
	bool UseBaseInspector, UseAdvancedInspector;
	bool EnableEditIdleData = false, EnableEditComboCombatData = false, 
	     EnableEditCombatData = false, EnableEditAttackData = false,
         EnableEditMoveData = false, EnableEditEffectData = false, EnableEditJumpData = false,
	     EnableEditReceiveDamageData = false, EnableEditDecalData = false, EnableEditDeathData = false,
	     EnableEditAttackAnimation = false, EnableEditAudioData = false;
	bool EnableEditDefaultLeftClawCombat = false, EnableEditDefaultRightClawCombat = false, EnableEditDefaultDualClawCombat;
	static IDictionary<PredatorComboCombat,bool> DynamicEditingComboCombatSwitch = new Dictionary<PredatorComboCombat,bool> ();
	
	static IDictionary<string, bool> DynamicalToggle = new Dictionary<string, bool>();
	
	public Editor_Predator3rdPersonalUnit ()
	{
	}
	
	public override void OnInspectorGUI ()
	{
		Predator3rdPersonalUnit PlayerPredatorUnit = this.target as Predator3rdPersonalUnit;
		if (UseBaseInspector = EditorGUILayout.BeginToggleGroup ("Base Inspector", UseBaseInspector)) {
			base.OnInspectorGUI ();
		}
		EditorGUILayout.EndToggleGroup ();
		
		if (UseAdvancedInspector = EditorGUILayout.BeginToggleGroup ("Advanced Inspector", UseAdvancedInspector)) {	

			PlayerPredatorUnit = (Predator3rdPersonalUnit)EditorCommon.EditBasicUnitProperty (PlayerPredatorUnit);
			if(GUILayout.Button("Save change"))
			{
				EditorUtility.SetDirty(PlayerPredatorUnit.gameObject);
			}
			//Edit Idle Data 
			if (EnableEditIdleData = EditorGUILayout.BeginToggleGroup ("---Edit Idle Data", EnableEditIdleData)) {
				PlayerPredatorUnit.IdleData = EditorCommon.EditIdleData (PlayerPredatorUnit.gameObject, PlayerPredatorUnit.IdleData);
			}
			EditorGUILayout.EndToggleGroup ();
			
			//Edit Move Data 
			if (EnableEditMoveData = EditorGUILayout.BeginToggleGroup ("---Edit Move Data", EnableEditMoveData)) {
				PlayerPredatorUnit.MoveData = EditorCommon.EditMoveData (PlayerPredatorUnit.gameObject, PlayerPredatorUnit.MoveData);
			}
			EditorGUILayout.EndToggleGroup ();
			
			//Edit Jump Data
			if (EnableEditJumpData = EditorGUILayout.BeginToggleGroup ("---Edit Jump Data", EnableEditJumpData)) {
				PlayerPredatorUnit.JumpData = EditJumpData (PlayerPredatorUnit.gameObject, PlayerPredatorUnit.JumpData);
			}
			EditorGUILayout.EndToggleGroup ();
			
			//Edit PredatorPlayerAttackData array
			if(EnableEditAttackData = EditorGUILayout.BeginToggleGroup ("---Edit Predator Attack Data", EnableEditAttackData)) {
			    PlayerPredatorUnit.PredatorAttackData = EditPredatorPlayerAttackDataArray(PlayerPredatorUnit, PlayerPredatorUnit.PredatorAttackData);
			}
			EditorGUILayout.EndToggleGroup ();
			
			//Edit attack basic variables
			if (EnableEditCombatData = EditorGUILayout.BeginToggleGroup ("---Edit attack variables", EnableEditCombatData)) {
				PlayerPredatorUnit.RushRadius = EditorGUILayout.FloatField (new GUIContent("Rush radius:", "Inside rush radius, predator rush to the target"), PlayerPredatorUnit.RushRadius);
//				PlayerPredatorUnit.AttackAnimationLayer = EditorGUILayout.IntField ("Attack animation layer:", PlayerPredatorUnit.AttackAnimationLayer);
				PlayerPredatorUnit.CombatCoolDown = EditorGUILayout.FloatField ("common combat cooldown:", PlayerPredatorUnit.CombatCoolDown);
				//Edit attack animation array
//				if (EnableEditAttackAnimation = EditorGUILayout.BeginToggleGroup ("    ----- Edit attack animation string array ----", EnableEditAttackAnimation)) {
//					PlayerPredatorUnit.AttackAnimations = EditorCommon.EditStringArray ("All attack animation:",
//				                                                                    PlayerPredatorUnit.AttackAnimations,
//				                                                                    EditorCommon.GetAnimationNames (PlayerPredatorUnit.gameObject));
//				}
//				EditorGUILayout.EndToggleGroup ();
				
				//Edit default combat - left claw 
				if (EnableEditDefaultLeftClawCombat = EditorGUILayout.BeginToggleGroup ("  ---Edit default combat : left claw", EnableEditDefaultLeftClawCombat)) {
					PlayerPredatorUnit.DefaultCombat_LeftClaw = Editor_Predator3rdPersonalUnit.EditCombat (
					                                                                 "Edit default left claw combat:",
					                                                                 PlayerPredatorUnit,
					                                                                 PlayerPredatorUnit.DefaultCombat_LeftClaw);
					PlayerPredatorUnit.DefaultCombat_LeftClaw.userInput = UserInputType.Button_Left_Claw_Tap;
				}
				EditorGUILayout.EndToggleGroup ();
				
				//Edit default combat - right claw 
				if (EnableEditDefaultRightClawCombat = EditorGUILayout.BeginToggleGroup ("  ---Edit default combat : right claw", EnableEditDefaultRightClawCombat)) {
					PlayerPredatorUnit.DefaultCombat_RightClaw = Editor_Predator3rdPersonalUnit.EditCombat (
					                       "Edit default right claw combat:", 
						                   PlayerPredatorUnit, 
						                   PlayerPredatorUnit.DefaultCombat_RightClaw);
					PlayerPredatorUnit.DefaultCombat_RightClaw.userInput = UserInputType.Button_Right_Claw_Tap;
				}
				EditorGUILayout.EndToggleGroup ();

				//Edit default combat - dual claw 
				if (EnableEditDefaultDualClawCombat = EditorGUILayout.BeginToggleGroup ("  ---Edit default combat : dual claw", EnableEditDefaultDualClawCombat)) {
					PlayerPredatorUnit.DefaultCombat_DualClaw = Editor_Predator3rdPersonalUnit.EditCombat (
					                       "Edit default dual claw combat:", 
					                       PlayerPredatorUnit, 
					                       PlayerPredatorUnit.DefaultCombat_DualClaw);
					PlayerPredatorUnit.DefaultCombat_DualClaw.userInput = UserInputType.Button_Dual_Claw_Tap;
				}
				EditorGUILayout.EndToggleGroup ();
				
				//Edit ComboCombat Data
				if (EnableEditComboCombatData = EditorGUILayout.BeginToggleGroup ("    ---Edit ComboCombat Data---", EnableEditComboCombatData)) {
					PlayerPredatorUnit.ComboCombat = Editor_Predator3rdPersonalUnit.EditComboCombatData (
						                                        PlayerPredatorUnit,
				                                                PlayerPredatorUnit.ComboCombat);
				}
				EditorGUILayout.EndToggleGroup ();
				
			}
			EditorGUILayout.EndToggleGroup ();

			//Edit Effect Data
			if (EnableEditEffectData = EditorGUILayout.BeginToggleGroup ("---Edit Effect Data---", EnableEditEffectData)) {
				PlayerPredatorUnit.EffectData = EditPlayerEffectDataArray (PlayerPredatorUnit.gameObject,
					                                                      PlayerPredatorUnit.EffectData);

			}
			EditorGUILayout.EndToggleGroup ();
			//Edit Audio Data
			if (EnableEditAudioData = EditorGUILayout.BeginToggleGroup ("---Edit Audio Data---", EnableEditAudioData)) {
				PlayerPredatorUnit.AudioData = EditAudioDataArray (PlayerPredatorUnit.gameObject,
					                                                      PlayerPredatorUnit.AudioData);

			}
			EditorGUILayout.EndToggleGroup ();
			
		}
		EditorGUILayout.EndToggleGroup ();
	}
	
	public static PredatorComboCombat[] EditComboCombatData (Predator3rdPersonalUnit Unit, 
		                                          PredatorComboCombat[] ComboCombatArray)
	{
		if (GUILayout.Button ("Add ComboCombat")) {
			PredatorComboCombat ComboCombat = new PredatorComboCombat ();
			ComboCombatArray = Util.AddToArray<PredatorComboCombat> (ComboCombat, ComboCombatArray);
		}
		for (int i=0; i<ComboCombatArray.Length; i++) {
			PredatorComboCombat ComboCombat = ComboCombatArray [i];
			
			if (DynamicEditingComboCombatSwitch.ContainsKey (ComboCombat) == false) {
				DynamicEditingComboCombatSwitch [ComboCombat] = false;
			}
			if (DynamicEditingComboCombatSwitch [ComboCombat] = EditorGUILayout.BeginToggleGroup (new GUIContent ("------- Edit combo combat:" + ComboCombat.comboName, ""),
				       DynamicEditingComboCombatSwitch [ComboCombat])) {
			
				ComboCombat.comboName = EditorGUILayout.TextField ("Combo combat name:", ComboCombat.comboName);
				ComboCombat.combat = EditCombatDataArray (Unit, ComboCombat.combat);

				if (GUILayout.Button ("Delete combo combat:" + ComboCombat.comboName)) {
					ComboCombatArray = Util.CloneExcept<PredatorComboCombat> (ComboCombatArray, ComboCombat);
				}
			}
			EditorGUILayout.EndToggleGroup ();
			EditorGUILayout.Space ();
		}
		return ComboCombatArray;
	}
	
	public static PredatorCombatData EditCombat (string label,
		                            Predator3rdPersonalUnit Unit, 
		                            PredatorCombatData Combat)
	{
		EditorGUILayout.LabelField (label);
		Combat.Name = EditorGUILayout.TextField ("Combat name:", Combat.Name);
		Combat.userInput = (UserInputType)EditorGUILayout.EnumPopup (new GUIContent ("Gesture:", "The matched gesture trigger this combat?"), Combat.userInput);
		string[] AllAttackDataName = Unit.PredatorAttackData.Select(x=>x.Name).ToArray();
		Combat.useAttackDataName = EditorCommon.EditStringArray("Use these attack data:",Combat.useAttackDataName,AllAttackDataName);
		EditorGUILayout.BeginHorizontal ();
		Combat.WaitUntilAnimationReturn = EditorGUILayout.Toggle ("Wait animation return:", Combat.WaitUntilAnimationReturn);
		Combat.BlockPlayerInput = EditorGUILayout.Toggle ("Block player input", Combat.BlockPlayerInput);
		EditorGUILayout.EndHorizontal ();
		Combat.OverrideAttackData = EditorGUILayout.BeginToggleGroup(new GUIContent("Override attackData setting", "If true, some of the attack data property would be overrided"), Combat.OverrideAttackData);
		Combat.DamagePointBase = EditorGUILayout.FloatField("DamagePoint base:",Combat.DamagePointBase); 
		EditorGUILayout.BeginHorizontal();
		Combat.MinDamagePointBonus = EditorGUILayout.FloatField("Min damage bonus:",Combat.MinDamagePointBonus); 
		Combat.MaxDamagePointBonus = EditorGUILayout.FloatField("Max damage bonus:",Combat.MaxDamagePointBonus); 
		EditorGUILayout.EndHorizontal();
		Combat.CanDoCriticalAttack = EditorGUILayout.Toggle("Can do critical attack", Combat.CanDoCriticalAttack);
		if(Combat.CanDoCriticalAttack)
		{
			EditorGUILayout.BeginHorizontal();
			Combat.CriticalAttackChance = EditorGUILayout.Slider("Critical chance:", Combat.CriticalAttackChance, 0, 1);
			Combat.CriticalAttackBonusRate = EditorGUILayout.FloatField("Critical bonus rate", Combat.CriticalAttackBonusRate);
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndToggleGroup();
		return Combat;
	}
	
	public static PredatorCombatData[] EditCombatDataArray (Predator3rdPersonalUnit Unit, 
		                                          PredatorCombatData[] CombatArray)
	{
		if (GUILayout.Button ("Add Combat")) {
			PredatorCombatData Combat = new PredatorCombatData ();
			CombatArray = Util.AddToArray<PredatorCombatData> (Combat, CombatArray);
		}
		if (CombatArray != null) {
			for (int i=0; i<CombatArray.Length; i++) {
				PredatorCombatData Combat = CombatArray [i];
				Combat = EditCombat ("--- Edit combat:" + Combat.Name, Unit, Combat);
				if (GUILayout.Button ("Delete combat:" + Combat.Name)) {
					CombatArray = Util.CloneExcept<PredatorCombatData> (CombatArray, Combat);
				}
				EditorGUILayout.Space ();
			}
		}
		return CombatArray;
	}
	
	public static PredatorPlayerJumpData EditJumpData (GameObject gameObject, PredatorPlayerJumpData jumpData)
	{
		jumpData.Name = EditorGUILayout.TextField ("Name:", jumpData.Name);
		jumpData.JumpForwardTime = EditorGUILayout.FloatField ("Jump forward time", jumpData.JumpForwardTime);
		jumpData.JumpForwardSpeed = EditorGUILayout.FloatField ("Jump forward speed", jumpData.JumpForwardSpeed);
		jumpData.JumpOverSpeed = EditorGUILayout.FloatField ("Jump over speed", jumpData.JumpOverSpeed);
		jumpData.JumpOverCheckDistance = EditorGUILayout.FloatField ("Jump over obstacle test distance", jumpData.JumpOverCheckDistance);
		jumpData.ObstacleToJumpOver = EditorGUILayoutx.LayerMaskField ("Jump over obstacle layer:", jumpData.ObstacleToJumpOver);
		jumpData.AnimationLayer = EditorGUILayout.IntField ("Jump animation layer:", jumpData.AnimationLayer);
		jumpData.PreJumpAnimation = EditorCommon.EditPopup ("PreJump Animation",
			                                               jumpData.PreJumpAnimation,
			                                               EditorCommon.GetAnimationNames (gameObject));
		jumpData.JumpingAnimation = EditorCommon.EditPopup ("Jumping Animation",
			                                               jumpData.JumpingAnimation,
			                                               EditorCommon.GetAnimationNames (gameObject));
		jumpData.GroundingAnimation = EditorCommon.EditPopup ("Grounding Animation",
			                                               jumpData.GroundingAnimation,
			                                               EditorCommon.GetAnimationNames (gameObject));
		return jumpData;
	}
	
	public static PlayerEffectData[] EditPlayerEffectDataArray (GameObject gameObject,
		                                                       PlayerEffectData[] PlayerEffectDataArray)
	{
		if (GUILayout.Button ("Add Effect data")) {
			PlayerEffectData EffectData = new PlayerEffectData ();
			PlayerEffectDataArray = Util.AddToArray<PlayerEffectData> (EffectData, PlayerEffectDataArray);
		}
		for (int i = 0; i < PlayerEffectDataArray.Length; i++) {
			PlayerEffectData EffectData = PlayerEffectDataArray [i];
			EditorGUILayout.LabelField ("------------------------ " + EffectData.Name);
			EffectData.Name = EditorGUILayout.TextField (new GUIContent ("Name", ""), EffectData.Name);
			EffectData.DamageForm = (DamageForm)EditorGUILayout.EnumPopup ("Damage form:", EffectData.DamageForm);
			EffectData.UseGlobalEffect = EditorGUILayout.Toggle (new GUIContent ("Use global effect?", "是否使用全局Effect?"), EffectData.UseGlobalEffect);
			
			if (EffectData.PlayParticle = EditorGUILayout.BeginToggleGroup (new GUIContent ("Play local particle object?", ""), EffectData.PlayParticle)) {
				EffectData.particlesystem = (ParticleSystem)EditorCommon.EditPopupOfTypeInChildren ("Assign a particle object to play:",
					EffectData.particlesystem, gameObject, typeof(ParticleSystem));
			}
			EditorGUILayout.EndToggleGroup ();
			
			if (EffectData.UseGlobalEffect) {
				EffectData.GlobalType = (GlobalEffectType)EditorGUILayout.EnumPopup (new GUIContent ("Global effect type", "是全局Effect类型"),
						EffectData.GlobalType);
			} else {

				EffectData.DestoryTimeOut = EditorGUILayout.FloatField (new GUIContent ("LifeTime:", ""), EffectData.DestoryTimeOut);
				
				EffectData.EffectObject = (GameObject)EditorGUILayout.ObjectField (new GUIContent ("Effect object", ""), EffectData.EffectObject, typeof(GameObject));
				EffectData.instantiationData = EditorCommon.EditInstantiationData(" -----------  Edit instantiation data ----------- ", EffectData.instantiationData);
				//Delete this effect data
				if (GUILayout.Button ("Delete EffectData:" + EffectData.Name)) {
					PlayerEffectDataArray = Util.CloneExcept<PlayerEffectData> (PlayerEffectDataArray, EffectData);
				}
			}
			EditorGUILayout.Space ();
		}
		return PlayerEffectDataArray;
	}
	
	public static AudioData[] EditAudioDataArray(GameObject gameObject,
                                                 AudioData[] AudioDataArray)
	{
		if (GUILayout.Button ("Add Audio Data")) {
			AudioData audioData = new AudioData ();
			AudioDataArray = Util.AddToArray<AudioData> (audioData, AudioDataArray);
		}
		for(int i=0; i<AudioDataArray.Length; i++)
		{
			AudioData audio = AudioDataArray [i];
			EditorGUILayout.LabelField ("------------------------ " + audio.Name);
			audio.Name = EditorGUILayout.TextField (new GUIContent ("Name", ""), audio.Name);
			audio.randomAudioClips = EditorCommon.EditObjectArray<AudioClip>("Edit Audio clips:", audio.randomAudioClips);
			//Delete this audio data
			if (GUILayout.Button ("Delete AduioData:" + audio.Name)) {
    			AudioDataArray = Util.CloneExcept<AudioData> (AudioDataArray, audio);
			}
		}
		EditorGUILayout.Space ();
		return AudioDataArray;
	}
	
	public static PredatorPlayerAttackData[] EditPredatorPlayerAttackDataArray(UnitBase unit, PredatorPlayerAttackData[] predatorPlayerAttackDataArray)
	{
		if(GUILayout.Button("Add PredatorPlayerAttackData:"))
		{
		   PredatorPlayerAttackData _p = new PredatorPlayerAttackData();
		   predatorPlayerAttackDataArray = Util.AddToArray<PredatorPlayerAttackData>(_p, predatorPlayerAttackDataArray);
		}
		for(int i=0; i<predatorPlayerAttackDataArray.Length; i++)
		{
		   PredatorPlayerAttackData attackData = predatorPlayerAttackDataArray[i];
		   if(DynamicalToggle.Keys.Contains(attackData.Name) == false)
		   {
				DynamicalToggle.Add(attackData.Name, true);
		   }
   		   EditorGUILayout.BeginHorizontal();
		   EditorGUILayout.LabelField("Edit attack data:" + attackData.Name);
		   DynamicalToggle[attackData.Name] = EditorGUILayout.Toggle(DynamicalToggle[attackData.Name]);
		   EditorGUILayout.EndHorizontal();
		   if(DynamicalToggle[attackData.Name] == false)
		   {
				continue;
		   }
		   attackData = EditPredatorPlayerAttackData(unit,attackData);
		   predatorPlayerAttackDataArray[i] = attackData;
		   if(GUILayout.Button("Delete PredatorPlayerAttackData:"+attackData.Name))
		   {
			  predatorPlayerAttackDataArray = Util.CloneExcept<PredatorPlayerAttackData>(predatorPlayerAttackDataArray, attackData);
		   }
		}
		return predatorPlayerAttackDataArray;
	}
	
	public static PredatorPlayerAttackData EditPredatorPlayerAttackData(UnitBase unit, PredatorPlayerAttackData predatorPlayerAttackData)
	{
		EditorCommon.EditAttackData(unit, predatorPlayerAttackData);
		predatorPlayerAttackData.CanDoCriticalAttack = EditorGUILayout.Toggle("Can do critical attack ?" , predatorPlayerAttackData.CanDoCriticalAttack);
		if(predatorPlayerAttackData.CanDoCriticalAttack)
		{
			predatorPlayerAttackData.CriticalAttackChance = EditorGUILayout.Slider("Critical attack chance:", 0,1,predatorPlayerAttackData.CriticalAttackChance);
			predatorPlayerAttackData.CriticalAttackBonusRate = EditorGUILayout.FloatField("Critical attack bonus rate:", predatorPlayerAttackData.CriticalAttackBonusRate);
		}
		return predatorPlayerAttackData;
	}
}
