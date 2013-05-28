using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SetupLevelBasic {
	
	/// <summary>
	/// Creates the base level objects.
	/// LevelManager
	/// FPS displayer
	/// GameMenu
	/// </summary>
	[MenuItem("GameObject/Level/CreateBasic")]	
	public static void CreateBaseLevelObjects ()
	{
	    //Create LevelManager
		GameObject levelManger = new GameObject();
		levelManger.name = "LevelManager";
		levelManger.AddComponent<LevelManager>().ControlDirectionPivot = levelManger.transform;
		
		//Create FPS displayer
		GameObject FPSDisplayer = new GameObject();
		FPSDisplayer.name = "FPS";
		FPSDisplayer.AddComponent<FPS>();
		FPSDisplayer.transform.parent = levelManger.transform;
		
		//Create game menu
		GameObject gameMenu = new GameObject();
		gameMenu.name = "Menu";
		gameMenu.AddComponent<GameMenu>();
	    gameMenu.transform.parent = levelManger.transform;
		
		//Create global blood system
		GameObject GloalEffectDecal = new GameObject();
		GloalEffectDecal.name = "GloalEffectDecal";
		GloalEffectDecal.AddComponent<GlobalBloodEffectDecalSystem>();
		GloalEffectDecal.transform.parent = levelManger.transform;
		
		//Create gamedialog system
	    GameObject gameDialogObject = new GameObject();
		gameDialogObject.name = "GameDialogue";
		gameDialogObject.AddComponent<GameDialogue>();
		gameDialogObject.transform.parent = levelManger.transform;
	}

}
