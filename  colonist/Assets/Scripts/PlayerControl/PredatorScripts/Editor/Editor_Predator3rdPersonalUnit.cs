using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Predator3rdPersonalUnit))]
public class Editor_Predator3rdPersonalUnit : Editor
{
	bool UseBaseInspector, UseAdvancedInspector;
	bool EnableEditIdleData = false, EnableEditComboCombatData = false, EnableEditAttackData = false,
         EnableEditMoveData = false, EnableEditEffectData = false, EnableEditJumpData = false,
	     EnableEditReceiveDamageData = false, EnableEditDecalData = false, EnableEditDeathData = false,
	     EnableEditAttackAnimation = false, EnableEditAudioData = false;
	bool EnableEditDefaultLeftClawCombat = false, EnableEditDefaultRightClawCombat = false, EnableEditDefaultDualClawCombat;
	static IDictionary<ComboCombat,bool> DynamicEditingComboCombatSwitch = new Dictionary<ComboCombat,bool> ();
	
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
			
			//Edit attack basic variables
			if (EnableEditAttackData = EditorGUILayout.BeginToggleGroup ("---Edit attack variables", EnableEditAttackData)) {
				PlayerPredatorUnit.AttackRadius = EditorGUILayout.FloatField ("Attack radius:", PlayerPredatorUnit.AttackRadius);
				PlayerPredatorUnit.AttackAnimationLayer = EditorGUILayout.IntField ("Attack animation layer:", PlayerPredatorUnit.AttackAnimationLayer);
				PlayerPredatorUnit.CombatCoolDown = EditorGUILayout.FloatField ("common combat cooldown:", PlayerPredatorUnit.CombatCoolDown);
				//Edit attack animation array
				if (EnableEditAttackAnimation = EditorGUILayout.BeginToggleGroup ("    ----- Edit attack animation string array ----", EnableEditAttackAnimation)) {
					PlayerPredatorUnit.AttackAnimations = EditorCommon.EditStringArray ("All attack animation:",
				                                                                    PlayerPredatorUnit.AttackAnimations,
				                                                                    EditorCommon.GetAnimationNames (PlayerPredatorUnit.gameObject));
				}
				EditorGUILayout.EndToggleGroup ();

				//Edit default combat - left claw 
				if (EnableEditDefaultLeftClawCombat = EditorGUILayout.BeginToggleGroup ("  ---Edit default combat : left claw", EnableEditDefaultLeftClawCombat)) {
					PlayerPredatorUnit.DefaultCombat_LeftClaw = Editor_Predator3rdPersonalUnit.EditCombat (
					                                                                 "Edit default left claw combat:",
					                                                                 PlayerPredatorUnit,
					                                                                 PlayerPredatorUnit.DefaultCombat_LeftClaw);
					PlayerPredatorUnit.DefaultCombat_LeftClaw.gestureType = UserInputType.Button_Left_Claw_Tap;
				}
				EditorGUILayout.EndToggleGroup ();
				
				//Edit default combat - right claw 
				if (EnableEditDefaultRightClawCombat = EditorGUILayout.BeginToggleGroup ("  ---Edit default combat : right claw", EnableEditDefaultRightClawCombat)) {
					PlayerPredatorUnit.DefaultCombat_RightClaw = Editor_Predator3rdPersonalUnit.EditCombat (
					                       "Edit default right claw combat:", 
						                   PlayerPredatorUnit, 
						                   PlayerPredatorUnit.DefaultCombat_RightClaw);
					PlayerPredatorUnit.DefaultCombat_RightClaw.gestureType = UserInputType.Button_Right_Claw_Tap;
				}
				EditorGUILayout.EndToggleGroup ();

				//Edit default combat - dual claw 
				if (EnableEditDefaultDualClawCombat = EditorGUILayout.BeginToggleGroup ("  ---Edit default combat : dual claw", EnableEditDefaultDualClawCombat)) {
					PlayerPredatorUnit.DefaultCombat_DualClaw = Editor_Predator3rdPersonalUnit.EditCombat (
					                       "Edit default dual claw combat:", 
					                       PlayerPredatorUnit, 
					                       PlayerPredatorUnit.DefaultCombat_DualClaw);
					PlayerPredatorUnit.DefaultCombat_DualClaw.gestureType = UserInputType.Button_Dual_Claw_Tap;
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
	
	public static ComboCombat[] EditComboCombatData (Predator3rdPersonalUnit Unit, 
		                                          ComboCombat[] ComboCombatArray)
	{
		if (GUILayout.Button ("Add ComboCombat")) {
			ComboCombat ComboCombat = new ComboCombat ();
			ComboCombatArray = Util.AddToArray<ComboCombat> (ComboCombat, ComboCombatArray);
		}
		for (int i=0; i<ComboCombatArray.Length; i++) {
			ComboCombat ComboCombat = ComboCombatArray [i];
			
			if (DynamicEditingComboCombatSwitch.ContainsKey (ComboCombat) == false) {
				DynamicEditingComboCombatSwitch [ComboCombat] = false;
			}
			if (DynamicEditingComboCombatSwitch [ComboCombat] = EditorGUILayout.BeginToggleGroup (new GUIContent ("      ------- Edit combo combat:" + ComboCombat.comboName, ""),
				       DynamicEditingComboCombatSwitch [ComboCombat])) {
			
				ComboCombat.comboName = EditorGUILayout.TextField ("Combo combat name:", ComboCombat.comboName);
				ComboCombat.combat = EditCombatDataArray (Unit, ComboCombat.combat);

				if (GUILayout.Button ("Delete combo combat:" + ComboCombat.comboName)) {
					ComboCombatArray = Util.CloneExcept<ComboCombat> (ComboCombatArray, ComboCombat);
				}
			}
			EditorGUILayout.EndToggleGroup ();
			EditorGUILayout.Space ();
		}
		return ComboCombatArray;
	}
	
	public static Combat EditCombat (string label,
		                            Predator3rdPersonalUnit Unit, 
		                            Combat Combat)
	{
		EditorGUILayout.LabelField (label);
		Combat.name = EditorGUILayout.TextField ("Combat name:", Combat.name);
		Combat.damageForm = (DamageForm)EditorGUILayout.EnumPopup ("DamageForm:", Combat.damageForm);
		Combat.gestureType = (UserInputType)EditorGUILayout.EnumPopup (new GUIContent ("Gesture:", "The matched gesture trigger this combat?"), Combat.gestureType);
		
		EditorGUILayout.BeginHorizontal ();
		Combat.WaitUntilAnimationReturn = EditorGUILayout.Toggle ("Pend for animation", Combat.WaitUntilAnimationReturn);
		Combat.BlockPlayerInput = EditorGUILayout.Toggle ("Block player input", Combat.BlockPlayerInput);
		EditorGUILayout.EndHorizontal ();
		
		Combat.HitPoint = EditorGUILayout.FloatField ("Hit point:", Combat.HitPoint);
		Combat.specialCombatFunction = EditorGUILayout.TextField ("Special function:", Combat.specialCombatFunction);
		Combat.specialAnimation = EditorCommon.EditStringArray ("Combat animation:",
				                                                Combat.specialAnimation,
				                                                EditorCommon.GetAnimationNames (Unit.gameObject));
		return Combat;
	}
	
	public static Combat[] EditCombatDataArray (Predator3rdPersonalUnit Unit, 
		                                          Combat[] CombatArray)
	{
		if (GUILayout.Button ("Add Combat")) {
			Combat Combat = new Combat ();
			CombatArray = Util.AddToArray<Combat> (Combat, CombatArray);
		}
		if (CombatArray != null) {
			for (int i=0; i<CombatArray.Length; i++) {
				Combat Combat = CombatArray [i];
				Combat = EditCombat ("--- Edit combat:" + Combat.name, Unit, Combat);
				if (GUILayout.Button ("Delete combat:" + Combat.name)) {
					CombatArray = Util.CloneExcept<Combat> (CombatArray, Combat);
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
				EffectData.DestoryInTimeOut = EditorGUILayout.Toggle (new GUIContent ("Auto Destory?", ""), EffectData.DestoryInTimeOut);
				if (EffectData.DestoryInTimeOut) {
					EffectData.DestoryTimeOut = EditorGUILayout.FloatField (new GUIContent ("LifeTime:", ""), EffectData.DestoryTimeOut);
				}
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
			//audio.audioClip = (AudioClip)EditorGUILayout.ObjectField(new GUIContent ("Audio clip:", "Assign audio clip"), audio.audioClip, typeof(AudioClip));
			audio.audioClip = EditorCommon.EditAudioClipArray("Edit audio clip", audio.audioClip);
			//Delete this audio data
			if (GUILayout.Button ("Delete AduioData:" + audio.Name)) {
    			AudioDataArray = Util.CloneExcept<AudioData> (AudioDataArray, audio);
			}
		}
		EditorGUILayout.Space ();
		return AudioDataArray;
	}
}
