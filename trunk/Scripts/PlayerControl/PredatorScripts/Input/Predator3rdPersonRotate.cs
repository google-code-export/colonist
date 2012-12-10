using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PredatorPlayerStatus))]
public class Predator3rdPersonRotate : MonoBehaviour {

    private JoyButton[] buttons = null;
    private Predator3rdPersonMovementController movementController = null;

    [HideInInspector]
    public MovementControlMode EffectiveMode = MovementControlMode.CameraRelative;

    private PredatorPlayerStatus predatorPlayerStatus = null; 

    void Awake()
    {
        buttons = this.GetComponents<JoyButton>();
        movementController = this.GetComponent<Predator3rdPersonMovementController>();
        predatorPlayerStatus = GetComponent<PredatorPlayerStatus>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (predatorPlayerStatus.PlayerControlMode != EffectiveMode)
        {
            enabled = false;
            return;
        }
        foreach (Touch t in Input.touches)
        {
            if (isJoybuttonTouch(t) == false)
            {
                movementController.RotateRightModifier = t.deltaPosition.x;
            }
        }
	}

    bool isJoybuttonTouch(Touch t)
    {
        foreach (JoyButton b in buttons)
        {
            if (b.CheckTouch(t))
                return true;
        }
        return false;
    }

    void Rotate()
    {
    }
}
