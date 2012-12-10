using UnityEngine;
using System.Collections;

public class AIBugMovement : MonoBehaviour {
    public Vector3 targetPosition;
    public bool move = false;
    public float rotationSpeed = 10f;
    public float speed = 10f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (move)
        {
            MoveTowards(targetPosition);
        }
	}

    public void MoveTowards(Vector3 pos)
    {
        SmoothRotateTowards(pos);
        Transform transform = gameObject.transform;
        Vector3 direction = pos - transform.position;
        direction.y = 0;
        float rotateTime = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), rotateTime);

        // Modify speed so we slow down when we are not facing the target
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        //Dot between current direction and target direction, this can slow down current speed when current direction differs much to target direction
        float speedModifier = Vector3.Dot(forward, direction.normalized);
        speedModifier = Mathf.Clamp01(speedModifier);

        // Move the character
        direction = forward * speed * speedModifier;
        gameObject.animation.CrossFade("walk");
        gameObject.GetComponent<CharacterController>().SimpleMove(direction);
        //this.transform.position += this.transform.TransformDirection(new Vector3(0, 0, 1f));
        //this.GetComponent<CharacterController>().Move(this.transform.TransformDirection(new Vector3(0, 0, 1f)));
    }

    public void SmoothRotateTowards(Vector3 pos)
    {
        Transform transform = gameObject.transform;
        Vector3 direction = pos - transform.position;
        direction.y = 0;
        //magnitude = square_root(x*x + y*y + z*z)

        float rotateTime = rotationSpeed * Time.deltaTime;
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotateTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), rotateTime);
    }
}
