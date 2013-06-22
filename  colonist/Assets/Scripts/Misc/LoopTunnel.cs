using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoopTunnel : MonoBehaviour
{
	public Collider TestCollider = null;
    /// <summary>
    /// The speed of tunnel
    /// </summary>
    public float Speed = 5f;
    public Transform First;
    public Transform Second;
	
	public Vector3 MoveDirection_Local = Vector3.forward;
	
    void Start()
    {
    }

    IEnumerator StartMoving()
    {
        yield return new WaitForEndOfFrame();//wait for starting
        while (true)
        {
			if(First.collider.bounds.Intersects(TestCollider.bounds))
			{
				Swap(First, Second);
				Transform t = First;
				First = Second;
				Second = t;
			}
            MoveForwards(First);
			MoveForwards(Second);
            yield return null;
            
        }
    }

    /// <summary>
    /// move the tunnel fowards
    /// </summary>
    /// <param name="tunnel"></param>
    /// <returns></returns>
    void MoveForwards(Transform tunnel)
    {
//        var length = Lengths[CurrentTunnel] * 2f;
        tunnel.position += tunnel.transform.TransformDirection(MoveDirection_Local).normalized * Speed * Time.deltaTime;
    }
	
	/// <summary>
	/// after swap, the current First will be at seconds, the current second will be at first.
	/// </summary>
    void Swap(Transform currentFirst, Transform currentSecond)
    {
        currentFirst.position = currentSecond.GetChild(0).position;
		currentFirst.rotation = currentSecond.GetChild(0).rotation;
//		Debug.Log("Swap !!!");
//		Debug.Break();
    }
}
