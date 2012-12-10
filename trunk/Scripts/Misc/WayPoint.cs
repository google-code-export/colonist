using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class WayPoint : MonoBehaviour {
    
    public WayPoint[] nextNodes;
    public LayerMask terrainLayer;

    public WayPoint[] reachablePoints = new WayPoint[]{};
    public WayPoint[] unreachablePoints = new WayPoint[]{};
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //Util.PutToGround(transform, terrainLayer,0.3f);
	}

    void OnDrawGizmosSelected()
    {

    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(0.2f, 0.2f, 0.2f));
    }

    public bool HasNext()
    {
        return nextNodes.Length > 0;
    }

    public WayPoint NextWaypoint()
    {
        if (nextNodes.Length > 0)
        {
            return Util.RandomFromArray<WayPoint>(nextNodes);
        }
        else
        {
            return null;
        }
    }

    public static IEnumerator AutoRoute(MonoBehaviour beheavior, WayPoint wp, CharacterController controller, float MovementSpeed, string animationWhenRouting)
    {
        //Debug.Log("Now route to :" + wp.name);
        while (true)
        {
            float distance = Util.Distance_XZ(wp.transform.position, controller.transform.position);
            if (distance >= 0.2f)
            {
                Util.MoveTowards(controller.transform, wp.transform.position, controller, true, false, MovementSpeed, 0f);
                if (animationWhenRouting != null && animationWhenRouting != string.Empty)
                {
                    controller.gameObject.animation.CrossFade(animationWhenRouting);
                }
                yield return null;
            }
            //Reach waypoint
            else if (wp.HasNext())
            {
                WayPoint nextWp = wp.NextWaypoint();
                //Debug.Log("Route next to :" + nextWp.name);
                yield return beheavior.StartCoroutine(AutoRoute(beheavior, nextWp, controller, MovementSpeed, animationWhenRouting));
                yield break;
            }
            else
                yield break;
        }
    }


}
