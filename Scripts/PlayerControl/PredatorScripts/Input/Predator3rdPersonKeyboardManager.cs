using UnityEngine;
using System.Collections;

/// <summary>
/// Manager class of Keyboard or Mouse(PC).
/// for Predator only.
/// </summary>
[RequireComponent(typeof(Predator3rdPersonMovementController))]
[RequireComponent(typeof(Predator3rdPersonalAttackController))]
public class Predator3rdPersonKeyboardManager : MonoBehaviour {

    private Predator3rdPersonMovementController PredatorMovementController = null;

	// Use this for initialization
	void Awake () {
        PredatorMovementController = this.GetComponent<Predator3rdPersonMovementController>();
	}
	
	// Update is called once per frame
	void Update () {
        UserInput();
	}

    void UserInput()
    {
        if (Util.GetGamePlatform() == Util.GamePlatform.Windows)
        {
            HandleMouseInput();
            HandleKeyboardInput();
        }
    }


    private void HandleMouseInput()
    {
    }

    private void HandleKeyboardInput()
    {
        //Movement
        if (Input.GetButton("Vertical"))
        {
            PredatorMovementController.MoveForwardModifier = Input.GetAxis("Vertical");
        }
        if (Input.GetButton("Horizontal"))
        {
            PredatorMovementController.MoveRightModifier = Input.GetAxis("Horizontal");
        }
        if (Input.GetButton("Jump"))
        {
            PredatorMovementController.MoveRightModifier = PredatorMovementController.MoveForwardModifier = 0;
        }

        //Rotate
         PredatorMovementController.RotateRightModifier = Input.GetAxis("Rotate");

        //Attacking
         if ((Input.GetKey("i") || Input.GetKey("j") 
             || Input.GetKey("l") || Input.GetKey("m") )&& !PredatorPlayerStatus.IsAttacking)
         {
             SendMessage("StrikePowerUp", SendMessageOptions.RequireReceiver);
         }
         else if (Input.GetKeyUp("i") && !PredatorPlayerStatus.IsAttacking)
         {
             SendMessage("Strike", DamageForm.Predator_Waving_Claw, SendMessageOptions.RequireReceiver);
         }
         else if (Input.GetKeyUp("j") && !PredatorPlayerStatus.IsAttacking)
         {
             SendMessage("Strike", DamageForm.Predator_Strike_Single_Claw, SendMessageOptions.RequireReceiver);
         }
         else if (Input.GetKeyUp("l") && !PredatorPlayerStatus.IsAttacking)
         {
             SendMessage("Strike", DamageForm.Predator_Clamping_Claws, SendMessageOptions.RequireReceiver);
         }
         else if (Input.GetKeyUp("m") && !PredatorPlayerStatus.IsAttacking)
         {
             SendMessage("Strike", DamageForm.Predator_Strike_Dual_Claw, SendMessageOptions.RequireReceiver);
         }
         else if (Input.GetKey("u") && !PredatorPlayerStatus.IsAttacking)
         {
             SendMessage("PunctureTossPowerUp");
         }
         else if (Input.GetKeyUp("u"))
         {
             SendMessage("PunctureOrToss");
         }
    }

}
