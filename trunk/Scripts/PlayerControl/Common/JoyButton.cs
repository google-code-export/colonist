using UnityEngine;
using System.Collections;

/// <summary>
/// An abstract class of JoyButton
/// Offspring should implement methods:
/// - onTouchBegin
/// - onTouchMove
/// - onTouchEnd
/// </summary>
public abstract class JoyButton : MonoBehaviour
{
    /// <summary>
    /// Name of the JoyButton
    /// </summary>
    [HideInInspector]
    public string JoyButtonName;
    /// <summary>
    /// Texture of the JoyButton
    /// </summary>
    public Texture2D ButtonTexture;
    /// <summary>
    /// The Vertical-Value when user finger slip vertically on the button
    /// Positive value - upwards
    /// Negative value - downwards
    /// </summary>
    [HideInInspector]
    public float Joybutton_Up_Value = 0;
    /// <summary>
    /// The Horizontal-Value when user finger slip horizontally on the button
    /// Positive value - right
    /// Negative value - left
    /// </summary>
    [HideInInspector]
    public float Joybutton_Right_Value = 0;
    /// <summary>
    /// The button's bound, must be a square
    /// </summary>
    [HideInInspector]
    public Rect JoyButtonBound;

    public virtual void PlayerControlOn()
    {
        this.enabled = true;
    }

    public virtual void PlayerControlOff()
    {
        this.enabled = false;
    }

    /// <summary>
    /// The center of the bound
    /// </summary>
    [HideInInspector]
    public Vector2 Center
    {
        get
        {
            return JoyButtonBound.center;
        }
    }

    /// <summary>
    /// The button's size, it's always a rectangle.
    /// </summary>
    public float JoyButtonSize = 150;

    public Vector2 JoyButtonScreenOffset;

    /// <summary>
    /// The button's bound offset , the actual display position on screen = JoyButtonBound + JoyButtonBoundOffset
    /// </summary>
    [HideInInspector]
    public Vector2 JoyButtonBoundOffset = new Vector2();
    /// <summary>
    /// User Touch start position on this button
    /// </summary>
    [HideInInspector]
    public Vector2 TouchStartPosition;
    /// <summary>
    /// When the button being moved by the JoyButtonBoundOffset, the modifier of the output offset value
    /// </summary>
    [HideInInspector]
    public float ValueOffsetModifier = 20;
    /// <summary>
    /// has a finger on this button?
    /// </summary>
    [HideInInspector]
    public bool hasFingerOnJoyButton;
    /// <summary>
    /// The finger id on this button
    /// </summary>
    protected int fingerID = -1;

    /// <summary>
    /// Check if a touch should be processed by the Joybutton.
    /// Basically, for a touch at Began phase, check if the touch is in the Button bound area
    /// For touches at other phase, check if the touch fingerid is known.
    /// offspring class should override this method for customing the chech touch logic.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public virtual bool CheckTouch(Touch t)
    {
        switch (t.phase)
        {
            case TouchPhase.Began:
                return isTouchInsideBound(t);
            case TouchPhase.Canceled:
            case TouchPhase.Stationary:
            case TouchPhase.Ended:
            case TouchPhase.Moved:
                return this.fingerID == t.fingerId;
            default:
                return false;
        }
    }

    public virtual void ProcessTouch(Touch t)
    {
        switch (t.phase)
        {
            case TouchPhase.Began:
                onTouchBegin(t);
                break;
            case TouchPhase.Stationary:
                onTouchStationary(t);
                break;
            case TouchPhase.Moved:
                onTouchMove(t);
                break;
            case TouchPhase.Canceled:
            case TouchPhase.Ended:
                onTouchEnd(t);
                break;
        }
    }

    /// <summary>
    /// Use this feature to control, when there're two joybuttons overlapped on screen, 
    /// which one can get touch check first.
    /// </summary>
    public int Priority = 0;

    /// <summary>
    /// The time when the finger touch on the Joy button
    /// </summary>
    [HideInInspector]
    public float TouchStartTime;
    public bool isTouchInsideBound(Vector2 touchScreenCoord)
    {
        Vector2 guiCoord = GameGUIHelper.ConvertScreenTouchCoordToGUICoord(touchScreenCoord);
        bool ret = JoyButtonBound.Contains(guiCoord);
        return ret;
    }
    public bool isTouchInsideBound(Touch t)
    {
        return this.isTouchInsideBound(t.position);
    }
    /// <summary>
    /// Call when touch.phase = Began
    /// </summary>
    /// <param name="touch"></param>
    public virtual void onTouchBegin(Touch touch)
    {
        this.hasFingerOnJoyButton = true;
        this.fingerID = touch.fingerId;
        this.TouchStartPosition = touch.position;
        this.TouchStartTime = Time.time;
    }
    /// <summary>
    /// Call when touch.phase = Move
    /// </summary>
    /// <param name="touch"></param>
    public abstract void onTouchMove(Touch touch);
    /// <summary>
    /// Call when touch.phase = Move
    /// </summary>
    /// <param name="touch"></param>
    public abstract void onTouchStationary(Touch touch);
    /// <summary>
    /// Call when touch.phase = End
    /// </summary>
    /// <param name="touch"></param>
    public virtual void onTouchEnd(Touch touch)
    {
        this.hasFingerOnJoyButton = false;
        this.fingerID = -1;
        Joybutton_Up_Value = 0;
        Joybutton_Right_Value = 0;
    }

}