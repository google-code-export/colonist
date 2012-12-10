using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class ButtonSkipScenario : JoyButton {
    
    public GameGUIHelper.RectPosition Location = GameGUIHelper.RectPosition.BottomLeft;

    void Awake()
    {
        this.JoyButtonName = "Skip";
        JoyButtonBound = GameGUIHelper.GetSquareOnGUICoordinate(Location, JoyButtonSize);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            LevelManager.GameEvent(new GameEvent(GameEventType.SkipScenario, null));

        }
	}

    /// <summary>
    /// Call when touch.phase = Began
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchBegin(Touch touch)
    {
         base.onTouchBegin(touch);
    }

    public override void onTouchMove(Touch touch)
    {
    }

    public override void onTouchStationary(Touch touch)
    {
    }

    public override void onTouchEnd(Touch touch)
    {
        base.onTouchEnd(touch);
        LevelManager.GameEvent(new GameEvent(GameEventType.SkipScenario, null));
    }

    void OnGUI()
    {
        Rect r = new Rect(JoyButtonBound.x + JoyButtonBoundOffset.x, JoyButtonBound.y + JoyButtonBoundOffset.y,
              JoyButtonSize, JoyButtonSize);
        GUI.DrawTexture(r, ButtonTexture, ScaleMode.ScaleToFit, true);
    }
}
