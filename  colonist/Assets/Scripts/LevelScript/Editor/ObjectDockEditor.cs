using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(ObjectDock))]
public class ObjectDockEditor : Editor {
	
	bool advancedEditor = false;
	IDictionary<string, bool> dictFlag = new Dictionary<string,bool>();
	public override void OnInspectorGUI ()
	{
		ObjectDock objectDock = target as ObjectDock;
		if(advancedEditor = EditorGUILayout.BeginToggleGroup(new GUIContent("Edit GameEvent",""),advancedEditor))
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
