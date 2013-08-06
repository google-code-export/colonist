using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(SpawnNPC))]
public class SpawnNPCEditor : Editor {
	
	bool baseEditor = false;
	bool EditSpawnData = false;
	bool EditSpawnEntity = false;
	public override void OnInspectorGUI ()
	{
		SpawnNPC spawnNPC = target as SpawnNPC;
		//base inspector
		if(baseEditor = EditorGUILayout.BeginToggleGroup(new GUIContent("Base inspector.", ""), baseEditor))
		{
		   base.OnInspectorGUI();
		}
		EditorGUILayout.EndToggleGroup();
		//Edit spawn data
		if(EditSpawnData = EditorGUILayout.BeginToggleGroup(new GUIContent("Edit SpawnData.", ""), EditSpawnData))
		{
		   spawnNPC.spawnNPCData = EditSpawnDataArray(spawnNPC.spawnNPCData);
		}
		EditorGUILayout.EndToggleGroup();
	    
		//Edit spawn entity
		if(EditSpawnEntity = EditorGUILayout.BeginToggleGroup(new GUIContent("Edit SpawnEntity.", ""), EditSpawnEntity))
		{
		   spawnNPC.spawnEntityArray = EditSpawnEntityArray(spawnNPC.spawnEntityArray, spawnNPC.spawnNPCData.Select(x=>x.Name).ToArray());
		}
		EditorGUILayout.EndToggleGroup();
	}
	
	SpawnData[] EditSpawnDataArray(SpawnData[] spawnDataArray){
		if(GUILayout.Button("Add SpawnData"))
		{
			spawnDataArray = Util.AddToArray<SpawnData>(new SpawnData(), spawnDataArray);
		}
		for(int i=0; i<spawnDataArray.Length; i++)
		{
			EditorGUILayout.LabelField("------------------------ SpawnData:" + spawnDataArray[i].Name);
			spawnDataArray[i].Name = EditorGUILayout.TextField("Name:",spawnDataArray[i].Name);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Description:");
			spawnDataArray[i].Description = EditorGUILayout.TextArea(spawnDataArray[i].Description);
			EditorGUILayout.EndHorizontal();
			spawnDataArray[i].Spawned = (GameObject)EditorGUILayout.ObjectField("Spawned object:",spawnDataArray[i].Spawned, typeof(GameObject));
			spawnDataArray[i].objectDock = (ObjectDock)EditorGUILayout.ObjectField("Spawned Dock:",spawnDataArray[i].objectDock, typeof(ObjectDock));
			if(GUILayout.Button("Remove this SpawnData"))
			{
				spawnDataArray = Util.CloneExcept<SpawnData>(spawnDataArray, spawnDataArray[i]);
			}
			EditorGUILayout.Space();
		}
		return spawnDataArray;
	}
	
	SpawnEntity[] EditSpawnEntityArray(SpawnEntity[] spawnEntityArray, string[] SpawnDataNameArray)
	{
		if(GUILayout.Button("Add SpawnEntity"))
		{
			spawnEntityArray = Util.AddToArray<SpawnEntity>(new SpawnEntity(), spawnEntityArray);
		}
		
		for(int i=0; i<spawnEntityArray.Length; i++)
		{
			EditorGUILayout.LabelField("------------------------ SpawnEntity:" + spawnEntityArray[i].Name);
			spawnEntityArray[i].Name = EditorGUILayout.TextField("Name:",spawnEntityArray[i].Name);
			spawnEntityArray[i].SpawnDataNameArray = EditorCommon.EditStringArray("Use these spawn data:", 
				                                                               spawnEntityArray[i].SpawnDataNameArray,
				                                                               SpawnDataNameArray);
			spawnEntityArray[i].WaitForSpawnedDieOut = EditorGUILayout.Toggle(new GUIContent("Wait for spawned unit die out to spawn next:",""),
				spawnEntityArray[i].WaitForSpawnedDieOut);
			if(spawnEntityArray[i].WaitForSpawnedDieOut == false)
			{
				spawnEntityArray[i].WaitTime = EditorGUILayout.FloatField(new GUIContent("Wait time to spawn next:",""),spawnEntityArray[i].WaitTime);
			}
			if(GUILayout.Button("Remove this SpawnEntity"))
			{
				spawnEntityArray = Util.CloneExcept<SpawnEntity>(spawnEntityArray, spawnEntityArray[i]);
				
			}
			EditorGUILayout.Space();
		}
		return spawnEntityArray;
	}
	
}
