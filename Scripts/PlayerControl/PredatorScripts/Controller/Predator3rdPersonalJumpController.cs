using UnityEngine;
using System.Collections;

public class Predator3rdPersonalJumpController : MonoBehaviour {

    private CharacterController controller;
    public string JumpToGround = "jumpToGround";
    public string Jumping = "jumping";
    public string PrejumpAnimation = "prejump";
	
    [HideInInspector]
    public bool checkJump = true;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    bool JumpToGroundTrigger = false;
	float JumpValue = 0;
	Vector3 JumpDirection = Vector3.zero;
	void Update () {
//        if (controller.isGrounded == false && checkJump)
//        {
//            controller.SimpleMove(new Vector3());
//            animation.CrossFade(Jumping);
//            JumpToGroundTrigger = false;
//        }
//        else if(JumpToGroundTrigger == false)
//        {
//            JumpToGroundTrigger = true;
//            SendMessage("Grounding");
//        }
//		
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    SendMessage("JumpTo", transform.position + transform.forward * 10);
        //}
//		JumpDirection.y -= 8 * Time.deltaTime;        
//		// Move the controller    
//		controller.Move(JumpDirection * Time.deltaTime);
	}

    void Grounding()
    {
        animation.CrossFade(JumpToGround); 
        checkJump = false;
    }
	
	IEnumerator JumpUp()
	{
		animation.CrossFade(PrejumpAnimation);
		yield return new WaitForSeconds(animation[PrejumpAnimation].length);
		
		// Apply gravity    
		Vector3 moveDirection = new Vector3();
		moveDirection.y = 16;
		// Move the controller    
		checkJump = true;
		controller.Move(moveDirection * Time.deltaTime);
		while(controller.isGrounded == false)
		{
			Debug.Log("Jumping");
			animation.CrossFade(Jumping);
			moveDirection.y -= 8 * Time.deltaTime;
			controller.Move(moveDirection * Time.deltaTime);
			checkJump = true;
			yield return null;
		}
		Grounding();
	}
	
	private float jumpSpeed = 30f;
	IEnumerator JumpTo(Combat combat)
	{
        Vector3 direction = Util.GestureDirectionToWorldDirection(combat.gestureInfo.gestureDirection.Value);
        Vector3 toPosition = transform.position + direction * 5;
		float distance = Vector3.Distance(transform.position, toPosition);
        Vector3 dir = toPosition - transform.position;

        float time = distance / jumpSpeed;
		animation.Blend(this.PrejumpAnimation,1);
		yield return new WaitForSeconds(animation[PrejumpAnimation].length);
		float _s = Time.time;
		while((Time.time - _s) <= time)
		{
            Util.MoveTowards(transform, dir, controller, jumpSpeed);
            animation.CrossFade(this.Jumping);
            yield return null;
		}
        animation.CrossFade(JumpToGround);
	}
}
