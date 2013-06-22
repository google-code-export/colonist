public enum AXIS
{
    X = 0,
    Y = 1,
    Z = 2
}
public enum MovementControlMode
{
    /// <summary>
    /// Diablo style 
    /// </summary>
    CharacterRelative = 0,
    /// <summary>
    /// CS style
    /// </summary>
    CameraRelative = 1
}

public enum ScreenOccupancy
{
    LeftScreen = 0,
    RightScreen = 1,
    FullScreen = 2
}

public enum UserInputType
{
	None = 0,
    Single_Slice = 1,
    Single_Tap = 2,
	Single_Curve = 3,
	Button_Left_Claw_Tap = 4,
	Button_Left_Claw_Hold = 5,
	Button_Right_Claw_Tap = 6,
	Button_Right_Claw_Hold = 7,
	Button_Dual_Claw_Tap = 8,
	Button_Dual_Claw_Hold = 9,
   // Double_Silce = 3,
    // ->  <-
   // Horizontal_ZoomIn = 4,
    // <-  ->
   // Horizontal_ZoomOut = 5,
    
   // Vertical_ZoomIn = 6,
    //Vertical_ZoomOut = 7,

    /// <summary>
    /// Two fingers gesture, while one finger hold and another one slice on screen
    /// </summary>
    //Fixed_Slice = 8
}

public enum HorizontalOrVertical
{
    Horizontal = 0,
    Vertical = 1,
}

/// <summary>
/// Approximate direction
/// </summary>
public enum HorizontalDirection
{
    None = 0,
    /// <summary>
    /// 00:00
    /// </summary>
    Up = 1,
    /// <summary>
    /// 07:00
    /// </summary>
    Up_Right = 2,
    /// <summary>
    /// 15:00
    /// </summary>
    Right = 3,
    /// <summary>
    /// 20:00
    /// </summary>
    Down_Right = 4,
    /// <summary>
    /// 30:00
    /// </summary>
    Down = 5,
    /// <summary>
    /// 35:00
    /// </summary>
    Down_Left = 6,
    /// <summary>
    /// 45:00
    /// </summary>
    Left = 7,
    /// <summary>
    /// 52:00
    /// </summary>
    Up_Left = 8,
}


public enum DamageForm
{
    /// <summary>
    /// The default DamageForm
    /// </summary>
     Common = 0,

#region Human 1-999
     ElectricityBoltHit = 1,
     Collision = 2,
     Punctured = 3,
     Flame = 4,
	 LaserSword = 5,
	 BossHit = 6,
#endregion

#region PredatorPlayer 1000 ~ 1999
     Predator_Waving_Claw = 1000,
     Predator_Clamping_Claws = 1001,
     Predator_Strike_Single_Claw = 1002,
     Predator_Strike_Dual_Claw = 1003,
     Predator_Puncture = 1004,
	 Predator_Fast_Spiking = 1005,
#endregion
}

/// <summary>
/// This enum indicate in what status the AI movement is.
/// </summary>
public enum AIMovementStatus
{
    Stationary = 0,
    Walking = 1,
    Running = 2,
}

/// <summary>
/// Deprecated - legacy enumeration
/// </summary>
public enum AIStatus
{
    Idle = 0,
    ToAttack = 1,
    Attacking = 2,
    PostAttack = 3,
    Frozen = 4,
    Fallback = 5
}

public enum MoveDirection
{
    AnyDirection = -1,
    Forward = 0,
    Backward = 1,
    Rightward = 2,
    Leftward = 3
}


/// <summary>
/// Define the event happen in game playing
/// </summary>
public enum GameEventType
{
    //Game level event
//    LevelStart = 0,
    LevelPause = 1,
    LevelEnd = 2,
	/// <summary>
	/// notify that scenario has been started. Then the scenario control object set isPlayingScenario = true, to allow fast passing time scale.
	/// </summary>
    StartScenario = 3,
	/// <summary>
	/// LevelArea start spawning, the LevelArea Name is given in stringParameter
	/// </summary>
//	LevelAreaStartSpawn = 4,
	/// <summary>
	/// Invoke a method in gameobject(given in GameEvent.receiver), method name is given in GameEvent.CustomMessage, 
	/// parameter type is given at ParameterType.
	/// </summary>
	InvokeMethod = 5,
	/// <summary>
	/// Load a level in IntParameter or StringParameter.
	/// </summary>
	LoadLevel = 6,
	/// <summary>
	/// Name of the theme music is given in stringParameter.
	/// </summary>
	PlayBackgroundMusic = 7,
	/// <summary>
	/// Name of the theme music is given in stringParameter.
	/// </summary>
	StopBackgroundMusic = 8,
	
	/// <summary>
	/// Stop all background music
	/// </summary>
	StopAllBackgroundMusic = 9,
	
	/// <summary>
	/// Save check point, the check point name is given in stringParameter.
	/// </summary>
	SaveCheckPoint = 11,
	/// <summary>
	/// Load last check point.
	/// </summary>
	LoadCheckPoint = 12,
	
	/// <summary>
	/// Set the controll pivot object to a new object in LevelManager.ControlDirectionPivot.
	/// </summary>
	SetPlayerControlDirectionPivot = 13,
	
    //Player Character Event
//    PlayerBirth = 100,
//    PlayerDie = 101,
    PlayerEnterArea = 102,
    PlayerLeaveArea = 103,
    DisplayDamageParameterOnPlayer = 104,
//    PlayerKill = 105,//Event when player kill NPC
//    PlayerAttack = 106,//Event when player perform an attack
//    PlayerReloading = 107,
    PlayerControlOn = 108,//player gained control
    PlayerControlOff = 109,//player lost control
	/// <summary>
	/// Deactivate the player object.
	/// Note: this will deactivated the root object of player, includes player camera, player control, player HUD ..etc
	/// </summary>
	PlayerSetToInactive = 110, 
	/// <summary>
	/// Activate the player object.
	/// Note: this will activated the root object of player, includes player camera, player control, player HUD ..etc
	/// </summary>
	PlayerSetToActive = 111, 
	WhiteInPlayerCamera = 112,//White In player camera
	WhiteOutPlayerCamera = 113,//White out player camera
	PlayerCameraOff = 114,//set off the player camera
	PlayerCameraOn = 115,//set on the player camera
	PlayerCameraAudioListenerOn = 116,//set on the player camera's audio listener
	PlayerCameraAudioListenerOff = 117,//set on the player camera's audio listener
	
	/// <summary>
	/// Requires player camera start slow motion mode, and focus on a fixed point (Not a transform).
	/// </summary>
	PlayerCameraSlowMotionOnFixedPoint = 118,
	
	/// <summary>
	/// Requires player camera start slow motion mode, and focus on a transform (So it's MOVEABLE!).
	/// The focus on Transform is given in GameEvent.GameObjectParameter.
	/// </summary>
	PlayerCameraSlowMotionOnTransform = 119,
	
    //NPC Event
//    NPCBirth = 200,
//    NPCDie = 201,
//    NPCEnterArea = 202,
//    NPCLeaveArea = 203,
	
	/// <summary>
	/// Place a NPC vertically on Ground.
	/// </summary>
	NPCPutToGround = 200,
	/// <summary>
	/// Notify that the game object is appling damage
	/// </summary>
    DisplayDamageParameterOnNPC = 204,
	/// <summary>
	/// Manipulate NPC to play animation, animation name is specified in GameEvent.StringParameter
	/// </summary>
	NPCPlayAnimation = 208, 
	/// <summary>
	/// Manipulate NPC to start AI, the AI name is given in GameEvent.StringParameter
	/// </summary>
	NPCStartAI = 209,
	/// <summary>
	/// NPC stop playing animation, the animation name is given in GameEvent.StringParameter
	/// </summary>
	NPCStopPlayingAnimation = 210,
	/// <summary>
	/// NPC start default A.
	/// </summary>
	NPCStartDefaultAI = 211,
	/// <summary>
	/// Manipulate NPC to face to player.
	/// </summary>
	NPCFaceToPlayer = 212,
	
	/// <summary>
	/// Request force to physical character. The receiver is the gameObject to be applied force.
	/// the vector3 parameter means the force direction, the float parameter means the force magnitude.
	/// </summary>
	ForceToPhysicsCharacter = 213,
	
	/// <summary>
	/// NPC play queue animation.
	/// The animation name is splitted by ; at StringParameter at the play sequence.
	/// example: 
	/// walk;stand;idle;fire
	/// </summary>
	NPCPlayQueueAnimation = 214,
	
	//Spawn NPC event:
	/// <summary>
	/// Manipulate a spawnNPC behavior to start spawning a SpawnData, 
	/// GameEvent.receiver should be the spawnNPC object, the SpawnData name is given in GameEvent.StringParameter.
	/// </summary>
	SpecifiedSpawn = 300,
	
	//Scenario event:
	ShowGameDialogue = 400,//display a Dialogue, MUST pass Dialogue ID in GameEvent.StringParameter
	WhiteInScenarioCamera = 401,//White in scenario camera
	WhiteOutScenarioCamera = 402,//white out scenario camera
	
	ScenarioComplete = 405,//indicate the scenario stop playing
	ScenarioCameraOff = 406,//Set off the scenario camera
	ScenarioCameraOn = 407,//Set on the scenario camera
	ScenarioCameraAudioListenerOn = 408,//set on the Scenario camera's audio listener
	ScenarioCameraAudioListenerOff = 409,//set on the Scenario camera's audio listener
	/// <summary>
	/// Starts docking behavior, the dock monobehavior's name is given in GameEvent.StringParameter.
	/// </summary>
	StartDocking = 410, 
	/// <summary>
	/// Deactivate a gameobject
	/// </summary>
	DeactivateGameObject = 411,
	
	/// <summary>
	/// Destroy a gameobject, destroy delay is given at FloatParameter.
	/// </summary>
	DestroyGameObject = 412,
	/// <summary>
	/// Activate a game object in GameEvent.receiver field.
	/// </summary>
	ActivateGameObject = 413,
	/// <summary>
	/// Deactivate a gameobject and don't impact the children game object
	/// </summary>
	DeactivateGameObjectIgnoreChildren = 414,
	
	/// <summary>
	/// Activate a gameobject and don't impact the children game object.
	/// </summary>
	ActivateGameObjectIgnoreChildren = 415,
	
	/// <summary>
	/// Attach a specified object to the parent.
	/// object (child) = receiver
	/// object (parent) = objectParameter
	/// </summary>
	AttachObjectToSpecifiedParent = 416,
	
	/// <summary>
	/// Similiar to StartDocking(410), but the dockingObject is not the object given in inspector, 
	/// but using runtime object, given in gameevent.receiver.
	/// </summary>
	StartDockingOnRuntimeTarget = 417,
	
	//Game setting event
	/// <summary>
	/// Set language to a given value in StringParameter
	/// </summary>
	SetLanguage = 500,
	/// <summary>
	/// set quality to a given level in IntParameter.
	/// </summary>
	SetQuality = 501,
	/// <summary>
	/// Mute audio
	/// </summary>
	Mute = 502,
	/// <summary>
	/// Unmute audio
	/// </summary>
	UnMute = 503,
}

/// <summary>
/// Define the event parameter key list.
/// </summary>
public enum GameEventParameter
{
	DamageParameter = 0,
}


/// <summary>
/// Conjunct two condition
/// None = no conjunct - only one condition
/// And = Condition1 = true AND condition2 = true
/// Or = Condition1 = true Or condition2 = true
/// </summary>
public enum LogicConjunction
{
    None = 0,
    And = 1,
    Or = 2
}

public enum BooleanComparisionOperator
{
    IsTrue = 0,
    IsFalse = 1
}

public enum ValueComparisionOperator
{
    Equal = 0,
    LessThan = 1,
    LessOrEqual = 2,
    GreaterThan = 3,
    GreaterOrEqual = 4,
    NotEqual = 5,
}

/// <summary>
/// Hit trigger type is the type to trigger hit test.
/// ByTime - hit test occurs in specified time.
/// ByAnimationEvent - hit test is trigger by animation event.
/// </summary>
public enum HitTriggerType
{
	/// <summary>
	/// Send the ApplyDamage message N seconds after animation.
	/// </summary>
	ByTime = 0,
	/// <summary>
	/// ApplyDamage will be triggered by AnimationEvent.
	/// </summary>
	ByAnimationEvent = 1,
}

/// <summary>
/// Curve property type.
/// </summary>
public enum CurvePropertyType
{
	Local_PositionX = 0,
	Local_PositionY = 1,
	Local_PositionZ = 2,
	Local_RotationX = 3,
	Local_RotationY = 4,
	Local_RotationZ = 5,
	Local_RotationW = 6,
	Local_ScaleX = 7,
	Local_ScaleY = 8,
	Local_ScaleZ = 9,
	
	Unknown = 10,
}