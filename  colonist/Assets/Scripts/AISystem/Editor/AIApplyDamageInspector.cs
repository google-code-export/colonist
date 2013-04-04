using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor (typeof(AIApplyDamage))]
public class AIApplyDamageInspector : Editor {
	
//	SerializedObject m_SerObj;
	bool baseInspector = false, advancedInspector = false;
	
	AIApplyDamage targetAIApplyDamage;
	
	public AIApplyDamageInspector()
	{
		
	}
 
	public override void OnInspectorGUI()
	{
		targetAIApplyDamage = target as AIApplyDamage;
		baseInspector = EditorGUILayout.BeginToggleGroup("Base inspector", baseInspector);
		if( baseInspector )
		{
			base.OnInspectorGUI();
		}
		EditorGUILayout.EndToggleGroup();
		
		advancedInspector = EditorGUILayout.BeginToggleGroup("Advanced inspector", advancedInspector);
		if( advancedInspector )
		{
			AI[] ai = targetAIApplyDamage.GetComponents<AI>();
			targetAIApplyDamage.SwitchToAIName = EditorCommon.EditPopup("Switch to AI:" , targetAIApplyDamage.SwitchToAIName, ai.Select(x=>x.Name).ToArray());
		}
	    EditorGUILayout.EndToggleGroup();
	}
}
