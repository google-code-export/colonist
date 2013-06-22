using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class SceneDebugUtilityWindow : EditorWindow
{
	
	bool IsSearchByEventType = true;
	bool IsSearchByObjectInReceiverField = true;
	
	/// <summary>
	///  search through scene, see if any gameevent's type is this type.
	/// </summary>
	GameEventType searchEventType;
	
	/// <summary>
	/// search through scene, see if any gameevent's receiver is this object.
	/// </summary>
	GameObject SearchObjectReference;
	ObjectDock[] searchedOutput_ObjectDock = new ObjectDock[]{};
	SpawnNPC[] searchedOutput_SpawnNPC = new SpawnNPC[]{};
	GameEventLifeCycle[] searchedOutput_GameEventLC = new GameEventLifeCycle[]{};
	
	[MenuItem("Window/Scene Debugger")]	
	public static void init ()
	{
		SceneDebugUtilityWindow window = (SceneDebugUtilityWindow)EditorWindow.GetWindow (typeof(SceneDebugUtilityWindow)); 
	}
	
	void OnGUI ()
	{
		IsSearchByEventType = EditorGUILayout.Toggle ("search by event type:", IsSearchByEventType);
		if (IsSearchByEventType) {
			searchEventType = (GameEventType)EditorGUILayout.EnumPopup ("search by event type:", searchEventType);
		}
		IsSearchByObjectInReceiverField = EditorGUILayout.Toggle ("search by GameObject in Gameevent.Receiver:", IsSearchByObjectInReceiverField);
		if (IsSearchByObjectInReceiverField) {
			SearchObjectReference = (GameObject)EditorGUILayout.ObjectField ("search by GameObject:", SearchObjectReference, typeof(GameObject));
		}
		if (GUILayout.Button ("Search")) {
			Search ();
		}
		DisplayResult();
	}
	
	void Search ()
	{
		searchedOutput_ObjectDock = new ObjectDock[]{};
		searchedOutput_SpawnNPC = new SpawnNPC[]{};
		
		ObjectDock[] AllObjectDocks = Util.ConvertObjectArray<ObjectDock> (Object.FindObjectsOfType (typeof(ObjectDock)));
		SpawnNPC[] AllSpawnNPC = Util.ConvertObjectArray<SpawnNPC> (Object.FindObjectsOfType (typeof(SpawnNPC)));
		GameEventLifeCycle[] AllGameEventLifecycle = Util.ConvertObjectArray<GameEventLifeCycle> (Object.FindObjectsOfType(typeof(GameEventLifeCycle)));
			
		ObjectDock[] SearchedOutObjectDocksByEventType = new ObjectDock[]{};
		SpawnNPC[] SearchedOutSpawnNPCByEventType = new SpawnNPC[]{};
		GameEventLifeCycle[] SearchedOutGameEventLifeCycleByEventType = new GameEventLifeCycle[]{};
		
		ObjectDock[] SearchedOutObjectDocksByObjectReference = new ObjectDock[]{};
		SpawnNPC[] SearchedOutSpawnNPCByObjectReference = new SpawnNPC[]{};
		GameEventLifeCycle[] SearchedOutGameEventLifeCycleByObjectReference = new GameEventLifeCycle[]{};
		
		if (IsSearchByEventType) {
			//search every objectdocks, check gameevent type
			IEnumerable<ObjectDock> docks = AllObjectDocks.Where (x =>
				x.objectDockData.Count (y => y.event_at_start.Concat (y.event_at_end.AsEnumerable ()).Count (z => z.type == searchEventType) > 0) > 0
				);
			if (docks != null && docks.Count () > 0) {
				SearchedOutObjectDocksByEventType = docks.ToArray ();
			}
			//search every spawnNPC, check gameevent type
			IEnumerable<SpawnNPC> spawns = AllSpawnNPC.Where (x => x.Event_At_All_Spawned_DieOut.Count (
				y => y.type == searchEventType) > 0);
			if (spawns != null && spawns.Count () > 0) {
				SearchedOutSpawnNPCByObjectReference = spawns.ToArray ();
			}
			
			//search every gameevent lifecycle, check gameevent type
			IEnumerable<GameEventLifeCycle> gameEventLifeCycles = AllGameEventLifecycle.Where (x =>
				x.eventOnBirth.Count (y => y.type == searchEventType) > 0)
				.Concat(
					AllGameEventLifecycle.Where (x =>
				x.eventOnDestroy.Count (y => y.type == searchEventType) > 0))
				.Concat(
					AllGameEventLifecycle.Where (x =>
				x.eventOnUnitDie.Count (y => y.type == searchEventType) > 0));
				
			if (gameEventLifeCycles != null && gameEventLifeCycles.Count () > 0) {
				SearchedOutGameEventLifeCycleByEventType = gameEventLifeCycles.ToArray ();
			}
			
			searchedOutput_ObjectDock = SearchedOutObjectDocksByEventType;
			searchedOutput_SpawnNPC = SearchedOutSpawnNPCByObjectReference;
			searchedOutput_GameEventLC = SearchedOutGameEventLifeCycleByEventType;
		}
		
		if (IsSearchByObjectInReceiverField) {
			//search every object dock, check gameevent 's receiver
			IEnumerable<ObjectDock> docks = AllObjectDocks.Where (x =>
				x.objectDockData.Count (y => y.event_at_start.Concat (y.event_at_end.AsEnumerable ()).Count (z => z.receiver == SearchObjectReference) > 0) > 0
				);
			if (docks != null && docks.Count () > 0) {
				SearchedOutObjectDocksByObjectReference = docks.ToArray ();
			}
			//search every spawnNPC, check gameevent 's receiver
			IEnumerable<SpawnNPC> spawns = AllSpawnNPC.Where (x => x.Event_At_All_Spawned_DieOut.Count (
				y => y.receiver == SearchObjectReference) > 0);
			if (spawns != null && spawns.Count () > 0) {
				SearchedOutSpawnNPCByObjectReference = spawns.ToArray ();
			}
			
			//search every gameevent lifecycle, check gameevent 's receiver
			IEnumerable<GameEventLifeCycle> gameEventLifeCycles = AllGameEventLifecycle.Where (x =>
				x.eventOnBirth.Count (y => y.receiver == SearchObjectReference) > 0)
				.Concat(
					AllGameEventLifecycle.Where (x =>
				x.eventOnDestroy.Count (y => y.receiver == SearchObjectReference) > 0))
				.Concat(
					AllGameEventLifecycle.Where (x =>
				x.eventOnUnitDie.Count (y => y.receiver == SearchObjectReference) > 0));
			if (gameEventLifeCycles != null && gameEventLifeCycles.Count () > 0) {
				SearchedOutGameEventLifeCycleByObjectReference = gameEventLifeCycles.ToArray ();
			}
			
			if (searchedOutput_ObjectDock == null || searchedOutput_ObjectDock.Length == 0) {
				searchedOutput_ObjectDock = SearchedOutObjectDocksByObjectReference;
			} else {
				searchedOutput_ObjectDock = searchedOutput_ObjectDock.Intersect (SearchedOutObjectDocksByObjectReference.AsEnumerable ()).ToArray ();
			}
			
			if (searchedOutput_SpawnNPC == null || searchedOutput_SpawnNPC.Length == 0) {
				searchedOutput_SpawnNPC = SearchedOutSpawnNPCByObjectReference;
			} else {
				searchedOutput_SpawnNPC = searchedOutput_SpawnNPC.Intersect (SearchedOutSpawnNPCByObjectReference.AsEnumerable ()).ToArray ();
			}
			
			if (searchedOutput_GameEventLC == null || searchedOutput_GameEventLC.Length == 0) {
				searchedOutput_GameEventLC = SearchedOutGameEventLifeCycleByObjectReference;
			} else {
				searchedOutput_GameEventLC = searchedOutput_GameEventLC.Intersect (SearchedOutGameEventLifeCycleByObjectReference.AsEnumerable ()).ToArray ();
			}
		}
	}
	
	void DisplayResult ()
	{
		if (searchedOutput_ObjectDock.Length > 0) {
			EditorGUILayout.LabelField (" -------- Searched out objectdock: ---- ");
			foreach (ObjectDock od in searchedOutput_ObjectDock) {
				EditorGUILayout.ObjectField (od, typeof(ObjectDock));
			}
		}
		
		if (searchedOutput_SpawnNPC.Length > 0) {
			EditorGUILayout.LabelField (" -------- Searched out SpawnNPC: ---- ");
			foreach (SpawnNPC spawn in searchedOutput_SpawnNPC) {
				EditorGUILayout.ObjectField (spawn, typeof(SpawnNPC));
			}
		}
		
		if (searchedOutput_GameEventLC.Length > 0) {
			EditorGUILayout.LabelField (" -------- Searched out GameEventLifeCycle: ---- ");
			foreach (GameEventLifeCycle gameEventLifecycle in searchedOutput_GameEventLC) {
				EditorGUILayout.ObjectField (gameEventLifecycle, typeof(GameEventLifeCycle));
			}
		}
	}
}
