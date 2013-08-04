using UnityEngine;
using System.Collections;


/// <summary>
/// Grand entrance for boss unit.
/// </summary>
public class GrandEntrance : MonoBehaviour {
	
	/// <summary>
	/// Flag to indicate if culling unit in camera.
	/// </summary>
	public bool CullUnitInCamera = false;
	
	/// <summary>
	/// The cull layer mask for current camera.
	/// </summary>
	public LayerMask CullLayerMask;
	
	/// <summary>
	/// The new material for skybox.
	/// If this variable is not null, the skybox material will be replace to the assigned one.
	/// </summary>
	public Material NewMaterialForSkybox = null;
	
	/// <summary>
	/// The adaptive texture to display.
	/// </summary>
	public AdaptiveIcon TextureToDisplay = new AdaptiveIcon();
	
	/// <summary>
	/// The curve controls texture display position Vs time.
	/// </summary>
	public Vector2Curve TextureDisplayPositionCurve = new Vector2Curve();
	
	/// <summary>
	/// After entrance fade in completed, wait how many seconds to allow player tap/click to quit entrance and resume game ?
	/// NOTE: The FrozenIntervalTime is count by REAL WORLD TIME, not In-Game-Time(Time.time)
	/// </summary>
	public float FrozenIntervalTime = 2;
	
	/// <summary>
	/// The event_to be fired at_entrance complete.
	/// </summary>
	public GameEvent[] Event_At_Entrance_Complete = new GameEvent[]{};
	
	bool shouldDrawTexture = false;
	bool userPressAnyKey = false;
	bool entranceFinishFadingIn = false;
	bool entranceComplete = false;
    Material OldSkyboxMaterial = null;
	Camera currentCamera = null;
	LayerMask oldCullingMask;
	float entranceFinishFadingInRealTime = 0;
	
	void Start()
	{
	}
	
	IEnumerator EntranceStart()
	{
		entranceFinishFadingIn = false;
		currentCamera = ScenarioControl.Instance.ScenarioCamera;
		//Cull camera
		if(this.CullUnitInCamera)
		{
		   oldCullingMask = currentCamera.cullingMask;
		   currentCamera.cullingMask = CullLayerMask;
		}
		
		//Change new material for skybox(if exists)
		if(NewMaterialForSkybox != null && currentCamera.GetComponent<Skybox>() != null)
		{
			OldSkyboxMaterial = currentCamera.GetComponent<Skybox>().material;
			currentCamera.GetComponent<Skybox>().material = NewMaterialForSkybox;
		}
		
		//calculate total time (account by XCurve)
		float startTime = Time.time;
		float _totalTime = Util.GetCurveMaxTime(TextureDisplayPositionCurve.XCurve.curve);
		
		//Set on the shouldDrawTexture flag
		shouldDrawTexture = true; 
		//Dynamically change texture position by TextureDisplayPositionCurve
		while((Time.time - startTime) <= _totalTime)
		{
			float _t = Time.time - startTime;
			float LeftPosition = TextureDisplayPositionCurve.XCurve.Evaluate(_t);
			float TopPosition = TextureDisplayPositionCurve.YCurve.Evaluate(_t);
			TextureToDisplay.adaptiveRect.AdaptiveAnchor_Left.Aspect = LeftPosition;
			TextureToDisplay.adaptiveRect.AdaptiveAnchor_Top.Aspect = TopPosition;
			TextureToDisplay.realtimeRect = TextureToDisplay.adaptiveRect.GetBound();
			yield return null;
		}
		//record the real world time when entrance fade in complete.
		entranceFinishFadingInRealTime = Time.realtimeSinceStartup;
		entranceFinishFadingIn = true;
		
	    //Pause the game and wait for user action (any touch/move/key) to resume
		LevelManager.PauseGame();
	}
	
	void EntranceComplete()
	{
		//un-reproceedable
		if(entranceComplete == true)
		{
			return;
		}
		entranceComplete = true;
		//resume game
		LevelManager.ResumeGame();
		//change old skybox material back
		currentCamera.GetComponent<Skybox>().material = OldSkyboxMaterial;
		
		shouldDrawTexture = false;
		
		//reset old cullmask of camera
		if(this.CullUnitInCamera)
		{
		   currentCamera.cullingMask = oldCullingMask;
		}
		
		foreach(GameEvent e in this.Event_At_Entrance_Complete)
		{
			LevelManager.OnGameEvent(e, this);
		}
		
		//self-disable
        this.enabled = false;
	}
	
	void OnGUI()
	{
		if(shouldDrawTexture)
		{
		    GUI.DrawTexture(TextureToDisplay.realtimeRect, TextureToDisplay.texture, TextureToDisplay.scaleMode);
	    }
		//If time's long enough to let user resume:
		if((Time.realtimeSinceStartup - entranceFinishFadingInRealTime) >= this.FrozenIntervalTime)
		{
		   //if user press any key/touch/mouse, resume the game
		   if(entranceFinishFadingIn && (Event.current.type == EventType.KeyDown || Event.current.type == EventType.MouseDown))
		   {
			  EntranceComplete();
		   }
		}
	}
	
}
