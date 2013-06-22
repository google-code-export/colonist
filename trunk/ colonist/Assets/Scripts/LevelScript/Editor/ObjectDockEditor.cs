using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(ObjectDock))]
public class ObjectDockEditor : Editor {
	
	bool advancedEditor_EditGameEvent = false;
	bool autoFillDockWithChildren = false;
	DockFleetMode defaultDockFleeMode = DockFleetMode.InGeneralSpeed;
	DockRotationType defaultDockRotationType = DockRotationType.Unchange;
	IDictionary<string, bool> dictFlag = new Dictionary<string,bool>();
	public override void OnInspectorGUI ()
	{
		ObjectDock objectDock = target as ObjectDock;
		if(advancedEditor_EditGameEvent = EditorGUILayout.BeginToggleGroup(new GUIContent("Edit GameEvent",""),advancedEditor_EditGameEvent))
		{
			foreach(ObjectDockData dockData in objectDock.objectDockData)
			{
				if(dictFlag.ContainsKey(dockData.Name)==false)
				{
					dictFlag.Add(dockData.Name, false);
				}
				if(dictFlag[dockData.Name] = EditorGUILayout.BeginToggleGroup("Edit ObjectDockData:" + dockData.Name, dictFlag[dockData.Name]))
				{
					if(dockData.event_at_start.Length > 0)
					{
						dockData.event_at_start = EditorCommon.EditGameEventArray("Edit StartEvent:", dockData.event_at_start);
					}
					if(dockData.event_at_end.Length > 0)
					{
						dockData.event_at_end = EditorCommon.EditGameEventArray("Edit EndEvent:", dockData.event_at_end);
					}
				}
				EditorGUILayout.EndToggleGroup();
			}
		}
		EditorGUILayout.EndToggleGroup();
		
		if(autoFillDockWithChildren = EditorGUILayout.BeginToggleGroup(new GUIContent("Fill dockdata of children",""),autoFillDockWithChildren))
		{
			defaultDockRotationType = (DockRotationType)EditorGUILayout.EnumPopup("Default rotation type:" , defaultDockRotationType);
			defaultDockFleeMode = (DockFleetMode)EditorGUILayout.EnumPopup("Default flee mode:" , defaultDockFleeMode);
			if(GUILayout.Button("Fill dockData"))
			{
				ObjectDockData[] dockDatas = new ObjectDockData[objectDock.transform.childCount];
				int counter = 0;
				foreach(Transform child in objectDock.transform)
				{
				   dockDatas[counter] = new ObjectDockData();
				   dockDatas[counter].Name = child.name;
				   dockDatas[counter].DockTransform = child;
				   dockDatas[counter].fleeMode = defaultDockFleeMode;
				   dockDatas[counter].rotationType = defaultDockRotationType;
				   counter++;
				}
				objectDock.objectDockData = dockDatas;
			}
		}
		EditorGUILayout.EndToggleGroup();
		
		EditorGUILayout.LabelField("--------------------  Base inspector ----------------------------");
		base.OnInspectorGUI();
		CheckDuplicatedName(objectDock.Name);
	}
	
	void CheckDuplicatedName(string Name)
	{
		Object[] ObjectDockArray = Editor.FindSceneObjectsOfType(typeof(ObjectDock));
		IList<ObjectDock> l = new List<ObjectDock>();
		foreach(Object od in ObjectDockArray)
		{
			l.Add((ObjectDock)od);
		}
		foreach(ObjectDock od in l)
		{
			if(l.Count(x=>x.Name == od.Name)>1)
			{
				Debug.LogError("Error : Duplicate object dock name found:" + od.Name);
			}
		}
	}
}
