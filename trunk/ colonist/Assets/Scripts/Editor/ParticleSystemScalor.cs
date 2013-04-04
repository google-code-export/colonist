using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ParticleSystemScalor : EditorWindow {

	[MenuItem("Component/ParticleSystem/Scale")]	
	public static void init ()
	{
		ParticleSystemScalor window = (ParticleSystemScalor)EditorWindow.GetWindow (typeof(ParticleSystemScalor));
	}
	
	private float scaleFactor = 1;
	
	void OnGUI ()
	{
		GameObject selectedGameObject = Selection.activeGameObject;
		EditorGUILayout.LabelField("Scale factor:" + scaleFactor);
		scaleFactor = EditorGUILayout.Slider (scaleFactor, 0.1f, 10);
		
		if(GUILayout.Button("Update scale"))
		{
		   ParticleSystem[] AllPS = selectedGameObject.GetComponentsInChildren<ParticleSystem>();
			foreach(ParticleSystem ps in AllPS)
			{
				UpdateParticleScale(ps, scaleFactor);
			}
		}
	}
	
	void UpdateParticleScale(ParticleSystem ps, float _ScaleFactor)
	{
		float newSize = ps.startSize * _ScaleFactor;
		ps.startSize = newSize;
	}
}
