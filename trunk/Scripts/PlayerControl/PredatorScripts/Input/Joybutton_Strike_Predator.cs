using UnityEngine;
using System.Collections;


/// <summary>
/// The button controls the strike action of the Predator protagonist, also by the button to speed up moving
/// Action List:
///  - Hold to power up
///  - Hold and slip down to speed up
///  - Hold and slip up to perform behead attack
///  - Hold and slip left to perform puncture attack
///  - Hold and slip right to perform mid attack
///  - Hold and release to perform normal attack
/// 
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof (Predator3rdPersonalAttackController))]
public class Joybutton_Strike_Predator : JoyButton   {

    private Predator3rdPersonalAttackController attackController = null;
    public Color baseColor = Color.red;

    public PrograssBar PowerHUD;

    public GameGUIHelper.RectPosition Location = GameGUIHelper.RectPosition.BottomRight;

    void Awake()
    {
        this.JoyButtonName = "Strike";
        attackController = this.GetComponent<Predator3rdPersonalAttackController>();
        ValueOffsetModifier = 5;
    }

    void Start()
    {
        JoyButtonBound = GameGUIHelper.GetSquareOnGUICoordinate(Location, JoyButtonSize);
    }

    void Update()
    {
        //JoyButtonBound = GameGUIHelper.GetSquareOnGUICoordinate(Location, JoyButtonSize);
        if (this.hasFingerOnJoyButton)
        {
            PowerHUD.Value = Mathf.Clamp((PowerHUD.Value + (1f / attackController.PowerAccelerationTime) * Time.deltaTime), 0, PowerHUD.MaxValue);
        }
    }

    /// <summary>
    /// Call when touch.phase = Began
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchBegin(Touch touch)
    {
        //Debug.Log("Strike button. OntouchBegin!");
        //Abondon the touch if the predator is now in attacking animation !!
        if (PredatorPlayerStatus.IsAttacking==false)
        {
            base.onTouchBegin(touch);
            attackController.SendMessage("StrikePowerUp");
        }
    }
    /// <summary>
    /// Call when touch.phase = Move
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchMove(Touch touch)
    {
        onTouchStationary(touch);
    }
    /// <summary>
    /// Call when touch.phase = Move
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchStationary(Touch touch)
    {
        if (PredatorPlayerStatus.IsAttacking == false)
        {
            attackController.SendMessage("StrikePowerUp");
        }
    }
    /// <summary>
    /// Call when touch.phase = End
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchEnd(Touch touch)
    {
        base.onTouchEnd(touch);
        Vector2 direction = touch.position - this.TouchStartPosition;
        float VerticalDistance = Mathf.Abs(direction.y);
        float HorizontalDistance = Mathf.Abs(direction.x);
        DamageForm attackForm = DamageForm.Predator_Strike_Single_Claw;
        if (VerticalDistance >= HorizontalDistance)
        {
            //Finger Slice Up
            if (direction.y > 0)
            {
                attackForm = DamageForm.Predator_Waving_Claw;
            }
            //Finger Slice down
            else
            {
                attackForm = DamageForm.Predator_Strike_Dual_Claw;
            }
        }
        else
        {
            //Finger Slice right
            if (direction.x > 0)
            {
                attackForm = DamageForm.Predator_Clamping_Claws;
            }
            //Finger Slice left
            else
            {
                attackForm = DamageForm.Predator_Strike_Single_Claw;
            }
        }
        attackController.SendMessage("Strike", attackForm, SendMessageOptions.RequireReceiver);

        PowerHUD.Value = 0;
    }
    /// <summary>
    /// Call in MonoBehavior.OnGUI
    /// </summary>
    /// <param name="touch"></param>
    public virtual void DrawButton()
    {
        GUI.color = this.baseColor;
        Rect r = new Rect(JoyButtonBound.x + JoyButtonBoundOffset.x, JoyButtonBound.y + JoyButtonBoundOffset.y,
            JoyButtonSize, JoyButtonSize);
        GUI.DrawTexture(r, ButtonTexture);
    }

    void OnGUI()
    {
        this.DrawButton();
    }
 
}
