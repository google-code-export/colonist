using UnityEngine;
using System.Collections;

/// <summary>
/// Attach this script to game object which the player / NPC can jump over.
/// </summary>
[@RequireComponent (typeof(BoxCollider))]
[AddComponentMenu("Physics/JumpOverObstacle")]
public class JumpOverObstacle : MonoBehaviour {
    /// <summary>
    /// Specify which axis is the width of this obstacle.
    /// </summary>
    public AXIS PlyAxis = AXIS.Y;
    public LayerMask TerrainLayer;
    public float GroundHorizontalOffset = 2;

    private BoxCollider BoxCollider;

    void Awake()
    {
        BoxCollider = GetComponent<BoxCollider>();
    }

    public float GetWidth()
    {
        switch (PlyAxis)
        {
            case AXIS.X:
                return BoxCollider.size.x;
            case AXIS.Y:
                return BoxCollider.size.y;
            case AXIS.Z:
            default:
                return BoxCollider.size.z;
        }
    }

    public float GetHeight()
    {
        return (BoxCollider.bounds.max - BoxCollider.bounds.min).y;
    }

    /// <summary>
    /// Called by object which jump over the obstacle.
    /// 1. The caller should firstly make sure this is the obstacle to jump over
    /// 2. Output position: HeightPosition = The heightest point in jumping
    ///    Output position: GroundPosition = The final ground point in jumping
    /// </summary>
    /// <param name="transfromJumpOver"></param>
    /// <param name="GroundPosition"></param>
    /// <returns></returns>
    public bool GetJumpOverTrack(Transform transfromJumpOver,out Vector3 HeightPosition, out Vector3 GroundPosition)
    {
        RaycastHit hitInfo;
        bool isHit = this.BoxCollider.Raycast(new Ray(transfromJumpOver.position, transfromJumpOver.forward), out hitInfo, 999);
        if (isHit)
        {
            //Calculate height point :
            HeightPosition = hitInfo.point + new Vector3(0, GetHeight(), 0);
            if (transfromJumpOver.GetComponent<CharacterController>() != null)
            {
                HeightPosition.y += transfromJumpOver.GetComponent<CharacterController>().height;
            }
            
            //Calculate ground point :
            Quaternion reverse = Quaternion.identity;
            reverse.eulerAngles = new Vector3(reverse.eulerAngles.x, reverse.eulerAngles.y + 180, reverse.eulerAngles.z);
            Vector3 normalReverse = reverse * hitInfo.normal;
            Vector3 RayCastPointInAir = hitInfo.point + normalReverse * GroundHorizontalOffset + new Vector3(0,HeightPosition.y, 0);
            RaycastHit GroundHitInfo;
            if (Physics.Raycast(RayCastPointInAir, Vector3.down, out GroundHitInfo, 999, TerrainLayer) == false)
            {
                GroundPosition = Vector3.zero;
                Debug.LogError("Can't find ground position:" + this.gameObject.name);
                return false;
            }
            GroundPosition = GroundHitInfo.point;
            //Debug.DrawLine(GroundPosition, GroundPosition + Vector3.up * 10);
            //Debug.DrawLine(HeightPosition, Vector3.zero);
          
            return true;
        }
        else
        {
            GroundPosition = Vector3.zero;
            HeightPosition = Vector3.zero;
            return false;
        }
    }
}
