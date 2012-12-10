using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Predator3rdPersonalFetchController))]
public class JoyButton_Fetch_Predator : JoyButton {

    Predator3rdPersonalFetchController fetchController = null;
    public Color baseColor = Color.white;

    public Vector2 Offset = new Vector2();
    public PrograssBar PowerHUD;

    public GameGUIHelper.RectPosition Location = GameGUIHelper.RectPosition.BottomRight;

    void Awake()
    {
        this.JoyButtonName = "Fetch";
        fetchController = this.GetComponent<Predator3rdPersonalFetchController>();
        ValueOffsetModifier = 5;
        JoyButtonBound = GameGUIHelper.GetSquareOnGUICoordinate(Location, JoyButtonSize, Offset);
    }

    void Start()
    {
        JoyButtonBound = GameGUIHelper.GetSquareOnGUICoordinate(Location, JoyButtonSize, Offset);
    }

    void Update()
    {
        //JoyButtonBound = GameGUIHelper.GetSquareOnGUICoordinate(Location, JoyButtonSize, Offset);
        if (this.hasFingerOnJoyButton)
        {
            PowerHUD.Value = Mathf.Clamp((PowerHUD.Value + (1f / fetchController.PowerAccelerationTime) * Time.deltaTime), 0, PowerHUD.MaxValue);
        }
    }

    public override void onTouchBegin(Touch touch)
    {
        if (PredatorPlayerStatus.IsAttacking==false)
        {
            base.onTouchBegin(touch);
            fetchController.SendMessage("PunctureTossPowerUp");
        }

    }
    public override void onTouchStationary(Touch touch)
    {
        fetchController.SendMessage("PunctureTossPowerUp");

    }
    //Do nothing on moving on Fetch button
    public override void onTouchMove(Touch touch)
    {
        onTouchStationary(touch);
    }

    public override void onTouchEnd(Touch touch)
    {
       // attackController.playerPressFetch = false;
        base.onTouchEnd(touch);
        SendMessage("PunctureOrToss");
        PowerHUD.Value = 0;
    }

    void OnGUI()
    {
        Rect r = new Rect(JoyButtonBound.x + JoyButtonBoundOffset.x, JoyButtonBound.y + JoyButtonBoundOffset.y,
              JoyButtonSize, JoyButtonSize);
        GUI.color = this.baseColor;
        GUI.DrawTexture(r, ButtonTexture, ScaleMode.ScaleToFit, true);
    }
}
