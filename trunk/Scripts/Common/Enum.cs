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

public enum GestureType
{

    Single_Slice = 0,
    Single_Tap = 1,
	Single_Curve = 2,
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
     Common = -1,
     Predator_Waving_Claw = 0,
     Predator_Clamping_Claws = 1,
     Predator_Strike_Single_Claw = 2,
     Predator_Strike_Dual_Claw = 3,
     //Predator_Straight = 4,
     ElectricityBoltHit = 5,
     Collision = 6,
     Punctured = 7
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
    LevelStart = 0,
    LevelPause = 1,
    LevelEnd = 2,
    StartScenario = 3,
    SkipScenario = 4,
    EndScenario = 5,
    //Player Character Event
    PlayerBirth = 100,
    PlayerDie = 101,
    PlayerEnterArea = 102,
    PlayerLeaveArea = 103,
    PlayerReceiveDamage = 104,
    PlayerKill = 105,//Event when player kill NPC
    PlayerAttack = 106,//Event when player perform an attack
    PlayerReloading = 107,
    PlayerControlOn = 108,//When player gained control
    PlayerControlOff = 109,//When player lost control

    //NPC Event
    NPCBirth = 200,
    NPCDie = 201,
    NPCEnterArea = 202,
    NPCLeaveArea = 203,
    NPCReceiveDamage = 204,
    NPCKill = 205,
    NPCAttack = 206,//Event when player perform an attack
    NPCReloading = 207
}