using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Combat hint.
/// The HUD at screen left top.
/// To display uesr gesture: 
/// dot, slice, tick
/// </summary>
[ExecuteInEditMode]
public class CombatHint : HUD {
	/// <summary>
	/// The texture for hinting player combat action.
	/// </summary>
    public Texture LeftClaw_Tap;
	public Texture LeftClaw_Hold;
	public Texture RightClaw_Tap;
	public Texture RightClaw_Hold;
	public Texture DualClaw_Tap;
	public Texture DualClaw_Hold;
	
	public float width = 32;
	public float height = 32;
	
	private IList<UserInputType> PlayerCombatList = new List<UserInputType>();
    private int MaxCount = ComboCombat.ComboCombatMaxCount;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
    void NewHint(UserInputType PlayerInputType)
	{
		if(PlayerCombatList.Count == MaxCount)
		{
			PlayerCombatList.Clear();
		}
		PlayerCombatList.Add(PlayerInputType);
	}

    void ClearHint()
    {
        PlayerCombatList.Clear();
    }
	
    void OnGUI()
    {
		//From left to right
		for(int i=0; i<PlayerCombatList.Count; i++)
		{
            int GridSpaceCount = MaxCount + 1;
            Rect area1 = new Rect(Screen.width - width * (GridSpaceCount - i), 0 + height, width, height);
            UserInputType gestureType = PlayerCombatList[i];
            GUI.DrawTexture(area1, GestureToTexture(gestureType), ScaleMode.ScaleToFit, true);
		}
	}
	
	Texture GestureToTexture(UserInputType gestureType)
	{
		switch(gestureType)
		{
		case UserInputType.Button_Right_Claw_Tap:
			return RightClaw_Tap;
		case UserInputType.Button_Right_Claw_Hold:
			return RightClaw_Hold;
	    case UserInputType.Button_Left_Claw_Tap:
			return LeftClaw_Tap;
	    case UserInputType.Button_Left_Claw_Hold:
			return LeftClaw_Hold;
        case UserInputType.Button_Dual_Claw_Tap:
			return DualClaw_Tap;
	    case UserInputType.Button_Dual_Claw_Hold:
			return DualClaw_Hold;			
			
		default:
			return RightClaw_Tap;
		}
	}
}
