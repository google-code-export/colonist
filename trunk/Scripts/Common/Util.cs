using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Util : MonoBehaviour {

    /// <summary>
    /// Return element index in the array.
    /// If not exists, return -1
    /// </summary>
    static int IndexOfArray<T>(T[] array, T element)
    {
        int index = -1;
        if (array != null && array.Length > 0)
        {
            for(int i=0;i<array.Length; i++)
            {
                if(array[i].Equals(element)){
                    index = i;
                    break;
                }
            }
            return index;
        }
        return index;
    }

	public static int RecurrsiveDecrementProduct(int startValue, int step)
	{
		if(startValue==1) return 1;
		return startValue * RecurrsiveDecrementProduct(startValue - step, step);
	}
	
    /// <summary>
    /// Convert a 2D gesture direction to a 3D world direction
    /// GestureDirection.x - horizontal offset value
    /// gestureDirection.y - vertical offset value
    /// </summary>
    /// <param name="gestureDirection"></param>
    /// <returns></returns>
    public static Vector3 GestureDirectionToWorldDirection(Vector2 gestureDirection)
    {
        //Convert: gesture.x = world direction.x
        //         gesture.y = world direction.z
        //         world direction.y = 0     
        Vector3 worldDirection = LevelManager.Instance.ControlDirectionPivot.TransformDirection(new Vector3(gestureDirection.x, 0, gestureDirection.y));
        return worldDirection.normalized;
    }
	
    /// <summary>
    /// Copy an object to the last element of an existing array.
    /// Warning - potential performance impact!! Don't call this function in frame-frequent function with large Array!
    /// </summary>
    /// <param name="o"></param>
    /// <param name="array"></param>
    /// <returns></returns>
    public static T[] AddToArray<T>(T o, T[] array)
	{
		int length = array.Length;
		T[] newArray = new T[length + 1];
		for(int i=0; i<length; i++)
		{
			newArray[i] = array[i];
		}
		newArray[length] = o;
		return newArray;
	}

    public static string ArrayToString<T>(T[] Array)
    {
        string ret = "";
        foreach (T a in Array)
        {
            ret += a.ToString() + " ";
        }
        return ret;
    }

	public static void CloneArray<T>(T[] src, T[] dst)
	{
		for(int i=0;i<src.Length;i++)
			dst[i] = src[i];
	}
	
	public static IList<T> CloneList<T>(IList<T> list)
	{
		IList<T> ret = new List<T>();
		for(int i=0; i<list.Count; i++)
			ret.Add(list[i]);
		return ret;
	}
	
    /// <summary>
    /// Clone an array, exclude the "except"
    /// </summary>
    /// <param name="array"></param>
    /// <param name="except"></param>
    /// <returns></returns>
	public static T[] CloneExcept<T>(T[] array, T except)
    {
        T[] ret = new T[]{};
        for(int i=0;i<array.Length;i++)
        {
			T o = array[i];
            if(!o.Equals(except))
            {
                ret = AddToArray(o, ret);
            }
        }
        return ret;
    }

    /// <summary>
    /// Clone an array, exclude the element at index
    /// </summary>
    /// <param name="array"></param>
    /// <param name="except"></param>
    /// <returns></returns>
    public static T[] CloneExcept<T>(T[] array, int exceptIndex)
    {
        T[] ret = new T[] { };
        for (int i = 0; i < array.Length; i++)
        {
            if (i != exceptIndex)
            {
                ret = AddToArray(array[i], ret);
            }
        }
        return ret;
    }
	
    public const float CollisionDamageFactor = 1;
    public static float CalculateCollisionHitPower(float ImpactorMass, float velocity)
    {
        return ImpactorMass * velocity * CollisionDamageFactor;
    }

    public static float CalculateTotalMass(GameObject gameObject)
    {
        Rigidbody[] rigis = gameObject.GetComponentsInChildren<Rigidbody>();
        float totalMass = 0;
        foreach (Rigidbody rigi in rigis)
        {
            totalMass += rigi.mass;
        }
        return totalMass;
    }

    public static void CopyToList(RaycastHit[] Array, out IList<Collider> list)
    {
        list = new List<Collider>();
        foreach (RaycastHit hit in Array)
        {
            list.Add(hit.collider);
        }
    }

    public static void CopyTransform(Transform src, Transform dst)
    {
        dst.position = src.position; 
        dst.rotation = src.rotation;
        foreach (Transform child in dst)
        {
            Transform _src = src.Find(child.name);
            if (_src != null)
            {
                CopyTransform(_src, child);
            }
        }
    }

    public static void CopyTransformPartially(Transform src, Transform dst, string[] childPathToCopy)
    {
        foreach (string path in childPathToCopy)
        {
            Transform childSrc = src.Find(path);
            Transform childDst = dst.Find(path);
            childSrc.position = childDst.position;
            childSrc.rotation = childDst.rotation;
        }
    }

    public static Rect GetScreenOccupancy(ScreenOccupancy ScreenOccupancy)
    {
        switch (ScreenOccupancy)
        {
            case ScreenOccupancy.LeftScreen:
                return new Rect(0, 0, Screen.width / 2, Screen.height);
            case ScreenOccupancy.RightScreen:
                return new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height);
            case ScreenOccupancy.FullScreen:
            default:
                return new Rect(0, 0, Screen.width / 2, Screen.height);
        }
    }

    /// <summary>
    /// Get the character's center.
    /// If the gameCharacter has a character controller, return it's center of the controller
    /// Else return the transform.position
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetCharacterCenter(GameObject gameCharacter)
    {
        CharacterController controller = null;
        if ((controller = gameCharacter.GetComponent<CharacterController>()) != null)
        {
            return (controller.center + gameCharacter.transform.position);
        }
        else
        {
            return gameCharacter.transform.position;
        }
    }

    public static Collider findClosest(Vector3 pos, IList<Collider> colliders)
    {
        float d = 0f;
        return findClosest(pos, colliders, out d);
    }

    public static Collider findClosest(Vector3 pos, IList<Collider> colliders, out float Distance)
    {
        Collider closest = null;
        float closestDistance = 99999f;
        foreach (Collider c in colliders)
        {
            float distance = Vector3.Distance( c.transform.position, pos);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = c;
            }
        }
        Distance = closestDistance;
        return closest;
    }

	public static Collider findClosest(Vector3 pos, Collider[] colliders)
    {
        float d = 0f;
        return findClosest(pos,colliders,out d);
    }

    public static Collider findClosest(Vector3 pos, Collider[] colliders, out float Distance)
    {
        Collider closest = null;
        float closestDistance = 9999f;
        foreach (Collider c in colliders)
        {
            float distance = Mathf.Abs((c.transform.position - pos).magnitude);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = c;
            }
        }
        Distance = closestDistance;
        return closest;
    }

    public static Collider findFarest(Vector3 pos, Collider[] colliders)
    {
        Collider farest = null;
        float farestDistance = 0f;
        foreach (Collider c in colliders)
        {
            float distance = Mathf.Abs((c.transform.position - pos).magnitude);
            if (distance > farestDistance)
            {
                farestDistance = distance;
                farest = c;
            }
        }
        return farest;
    }

    public enum GamePlatform
    {
        Windows = 0,
        Android = 1,
        IPhone = 2,
        Unknow = 3
    }

    /// <summary>
    /// Return a point, positioned at a random point on a circle, which the circle is at height unit 
    /// above the center point.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="height"></param>
    /// <param name="minAngle"></param>
    /// <param name="maxAngle"></param>
    /// <returns></returns>
    public static Vector3 GetRandomPositionSurrondPoint(Vector3 center, float radius, float height, float minAngle, float maxAngle)
    {
        float randomAngle = 0;
        Vector3 newPosition = GetRandomPositionSurrondPoint(center, radius, height, minAngle, maxAngle, out randomAngle);
        return newPosition;
    }

    public static Vector3 GetRandomPositionSurrondPoint(Vector3 center, float radius, float height, float minAngle, float maxAngle, out float randomAngle)
    {
        float _randomAngle = Random.Range(minAngle, maxAngle);
        randomAngle = _randomAngle;
        Quaternion randomRotation = Quaternion.AngleAxis(_randomAngle, Vector3.up);
        Vector3 offset = new Vector3(0, 0, 1);
        offset *= radius;
        Vector3 newPositionDistanceOffset = randomRotation * offset;
        Vector3 newPosition = center + newPositionDistanceOffset;
        newPosition.y = center.y + height;
        return newPosition;
    }

    public static GamePlatform GetGamePlatform()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                return GamePlatform.Android;
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return GamePlatform.Windows;
            default:
                return GamePlatform.Unknow;
        }
    }

    static int XenzLayerMasker = -1;
    public static int GetXenzLayermask()
    {
        if (XenzLayerMasker == -1)
        {
            int layer = LayerMask.NameToLayer("Xenz");
            XenzLayerMasker = 1 << layer;
        }
        return XenzLayerMasker;
    }

    /// <summary>
    /// Check if a layer is within the layermask
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="lm"></param>
    /// <returns></returns>
    public static bool CheckLayerWithinMask(int layer, LayerMask lm)
    {
        int _lm = 1 << layer;
        return ((_lm & lm) == _lm);
    }

    public static int GetLayerMask(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        layer = 1 << layer;
        return layer;
    }

    public static bool IsTransformInsideBounds(Transform src, Collider dst)
    {
        return dst.bounds.Contains(src.position);
    }

    public static bool IsTransformInsideBounds(Transform src, Collider[] dst)
    {
        foreach(Collider dest in dst)
        {
            if(dest.bounds.Contains(src.position))
            {
                return true;
            }
        }
        return false;
    }

    public static Collider[] GetObjectsInLayer(string layerName, Vector3 position, float radius)
    {
        int layerMask = Util.GetLayerMask(layerName);
        Collider[] colliders = Physics.OverlapSphere(position, radius, layerMask);
        return colliders;
    }

    public static T RandomFromArray<T>(T[] array)
    {
        int idx = Random.Range(0, array.Length);
        return array[idx];
    }

    public static T RandomFromList<T>(IList<T> list)
    {
        if (list.Count == 1)
        {
            return list[0];
        }
        else
        {
            int RandomIndex = Random.Range(0, list.Count);
            return list[RandomIndex];
        }
    }

    public static string RandomFromArray(string []array)
    {
        if (array.Length == 0)
        {
            Debug.LogError("Util.RandomFromArray: Empty array parameter!");
            return string.Empty;
        }
        int min = 0;
        int max = array.Length;
        int idx = Random.Range(min, max);
        return array[idx];
    }

    public static Vector2? GestureDown()
    {
        Vector2? screenInput = null;
        if (Util.GetGamePlatform() == Util.GamePlatform.Android && Input.touchCount > 0
           && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch t1 = Input.GetTouch(0);
            screenInput = new Vector2(t1.position.x, t1.position.y);
        }
        else if (Util.GetGamePlatform() == Util.GamePlatform.Windows
            && Input.GetMouseButtonDown(0) == true)
        {
            screenInput = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        return screenInput;
    }

    public static Vector2? GestureMove()
    {
        Vector2? screenInput = null;
        if (Util.GetGamePlatform() == Util.GamePlatform.Android)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Touch t1 = Input.GetTouch(0);
                screenInput = new Vector2(t1.position.x, t1.position.y);
            }
        }
        else if (Util.GetGamePlatform() == Util.GamePlatform.Windows)
        {
            if (Input.GetMouseButton(0) == true)
            {
                screenInput = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
        }
        return screenInput;
    }

    /// <summary>
    /// Return the touch x-y, or mouse(0) x-y when touch end or mouse(0) up
    /// </summary>
    /// <returns></returns>
    public static Vector2? GestureUp()
    {
        Vector2? screenInput = null;
        if (Util.GetGamePlatform() == Util.GamePlatform.Android && Input.touchCount > 0
           && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Touch t1 = Input.GetTouch(0);
            screenInput = new Vector2(t1.position.x, t1.position.y);
        }
        else if (Util.GetGamePlatform() == Util.GamePlatform.Windows
            && Input.GetMouseButtonUp(0) == true)
        {
            screenInput = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        return screenInput;
    }

    public static void PutToGround(Transform transform,LayerMask terrainLayer, float OffsetHeight)
    {
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 9999, terrainLayer))
        {
            transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + OffsetHeight, hitInfo.point.z);
        }
    }

    /// <summary>
    /// Calculate angle between two direction, with y-axis ignored
    /// </summary>
    /// <param name="direction1"></param>
    /// <param name="direction2"></param>
    /// <returns></returns>
    public static float HorizontalAngle(Vector3 direction1, Vector3 direction2)
    {
        Vector3 d1 = new Vector3(direction1.x, 0, direction1.z);
        Vector3 d2 = new Vector3(direction1.x, 0, direction2.z);
        return Vector3.Angle(d1, d2);
    }

    /// <summary>
    /// Calculate angle between two direction, with x-axis ignored
    /// </summary>
    /// <param name="direction1"></param>
    /// <param name="direction2"></param>
    /// <returns></returns>
    public static float VerticalAngle(Vector3 direction1, Vector3 direction2)
    {
        Vector3 d1 = new Vector3(direction1.x, direction1.y, direction1.z);
        Vector3 d2 = new Vector3(direction2.x, direction2.y, direction2.z);
        return Vector3.Angle(d1, d2);
    }

    /// <summary>
    /// Rotate an angle in XZ surface. (By Axis-Y)
    /// </summary>
    /// <param name="rotation"></param>
    /// <param name="angle"></param>
    /// <param name="axis"></param>
    /// <returns></returns>
    public static Quaternion RotateAngleYAxis(Quaternion rotation, float angle)
    {
        rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y + Random.Range(-30f, 30f), rotation.eulerAngles.z);
        return rotation;
    }

    public static void RotateToward(Transform transform, Vector3 pos, bool smoothRotate, float rotationSpeed)
    {
        RotateToward(transform, pos, smoothRotate, rotationSpeed, Vector3.up);
    }

    /// <summary>
    /// rotationSpeed is ignored if smoothRotate = false
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="pos"></param>
    /// <param name="smoothRotate"></param>
    /// <param name="rotationSpeed"></param>
    /// <param name="upwardAxis"></param>
    public static void RotateToward(Transform transform, Vector3 pos, bool smoothRotate, float rotationSpeed, Vector3 upwardAxis)
    {
        Vector3 direction = pos - transform.position;
        direction.y = 0;
        //magnitude = square_root(x*x + y*y + z*z)
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotateTime);
        if (smoothRotate)
        {
            float rotateTime = rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, upwardAxis), rotateTime);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    /// <summary>
    /// Align From to To, if smoothAligh = false, aligns immediately, and the speed parameters are ignored
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="smoothAligh"></param>
    /// <param name="movementSpeed"></param>
    /// <param name="rotationSpeed"></param>
    public static void AlighToward(Transform from, Transform to, bool smoothAligh, float movementSpeed, float rotationSpeed)
    {
        if (smoothAligh)
        {
            from.position = Vector3.Lerp(from.position, to.position, movementSpeed * Time.deltaTime);
            from.rotation = Quaternion.Lerp(from.rotation, to.rotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            from.position = to.position;
            from.rotation = to.rotation;
        }
    }

    private static Vector3 AlighingVel = new Vector3();
    public static void AlighToward(Transform from, Transform to, float OverTime)
    {
        //from.position = Vector3.SmoothDamp(from.position, to.position, ref AlighingVel, OverTime);
        from.position = Vector3.Lerp(from.position, to.position, Mathf.Clamp01(1 / OverTime) * Time.deltaTime);
        from.rotation = Quaternion.Lerp(from.rotation, to.rotation, Mathf.Clamp01(1 / OverTime) * Time.deltaTime);
    }

    public static void ChangeGameObjectLayer(GameObject gameObject, int layer, bool recusiveChange)
    {
        gameObject.layer = layer;
        if (recusiveChange)
        {
            foreach (Transform child in gameObject.transform)
            {
                ChangeGameObjectLayer(child.gameObject, layer, recusiveChange);
            }
        }
    }

    public static bool CompareValue(float LeftValue, ValueComparisionOperator Operator, float RightValue)
    {
        bool ret = false;
        switch (Operator)
        {
            case ValueComparisionOperator.Equal:
                ret = Mathf.Approximately(LeftValue, RightValue);
                break;
            case ValueComparisionOperator.GreaterThan:
                ret = LeftValue > RightValue;
                break;
            case ValueComparisionOperator.GreaterOrEqual:
                ret = LeftValue > RightValue || Mathf.Approximately(LeftValue, RightValue);
                break;
            case ValueComparisionOperator.LessThan:
                ret = LeftValue < RightValue;
                break;
            case ValueComparisionOperator.LessOrEqual:
                ret = LeftValue < RightValue || Mathf.Approximately(LeftValue, RightValue);
                break;
            case ValueComparisionOperator.NotEqual:
                ret = !Mathf.Approximately(LeftValue, RightValue);
                break;
            default:
                break;
        }
        return ret;
    }

    public static bool CompareTransform(Transform from, Transform to)
    {
        bool comparePos = (from.position == to.position);
        bool compareRot = (from.rotation == to.rotation);
        Debug.Log("PosCompare:" + comparePos + " compareRot:" + compareRot);
        return comparePos && compareRot;
    }

    /// <summary>
    /// Return XZ distance of two vector, Y-axis value is ignored
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float Distance_XZ(Vector3 a, Vector3 b)
    {
        b.y = a.y;
        return Vector3.Distance(a, b);
    }
	
	/// <summary>
	/// Distances the of two characters.
	/// Return distance between :
	/// a.controller.center (if no controller, then transform.position) ;
	/// b.controller.center (if no controller, then transform.position) ;
	/// And minus the radius of the two controller(if exists).
	/// </summary>
    public static float DistanceOfCharacters(GameObject a, GameObject b)
    {
        CharacterController aController = a.GetComponent<CharacterController>();
        Vector3 aPos = aController == null ? a.transform.position : aController.center + aController.transform.position;

        CharacterController bController = b.GetComponent<CharacterController>();
        Vector3 bPos = bController == null ? b.transform.position : bController.center + bController.transform.position;

        float centerDistance = Vector3.Distance(aPos, bPos);
        if (aController != null)
        {
            centerDistance -= aController.radius;
        }
        if (bController != null)
        {
            centerDistance -= bController.radius;
        }
        return centerDistance;
    }

    public static float DistanceOfCharactersXZ(CharacterController a, CharacterController b)
    {
        Vector3 posA = a.center + a.transform.position;
        Vector3 posB = b.center + b.transform.position;
        return Distance_XZ(posA, posB) - a.radius - b.radius;
    }

    /// <summary>
    /// The distance of the point to a collider
    /// </summary>
    /// <param name="a"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static float DistanceToCollider(Collider a, Vector3 point)
    {
        Vector3 cloestPoint = a.ClosestPointOnBounds(point);
        return Vector3.Distance(cloestPoint, point);
    }

    /// <summary>
    /// The distance of the point to a collider, ignore the y axis
    /// </summary>
    /// <param name="a"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static float DistanceToColliderXZ(Collider a, Vector3 point)
    {
        Vector3 cloestPoint = a.ClosestPointOnBounds(point);
        return Util.Distance_XZ(cloestPoint, point);
    }

    /// <summary>
    /// Move smoothly by a given velocity.
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="Velocity"></param>
    /// <param name="controller"></param>
    /// <param name="rotationSpeed"></param>
    public static void MoveSmoothly(Transform transform, Vector3 Velocity, CharacterController controller, float rotationSpeed)
    {
        Vector3 forward = transform.forward;
        Util.RotateToward(transform, transform.position + Velocity, true, rotationSpeed);
        float speedModifier = Vector3.Dot(forward, Velocity.normalized);
        speedModifier = Mathf.Clamp01(speedModifier);
        controller.SimpleMove(Velocity * speedModifier);
    }

    public static void MoveTowards(Transform transform, Vector3 direction, CharacterController controller, float movementspeed)
    {
        direction = direction.normalized * movementspeed;
        controller.SimpleMove(direction);
    }

    public static void MoveTowards(Transform transform, Vector3 pos, CharacterController controller, bool rotateWhileMoving, bool smoothRotate, float movementspeed, float rotationSpeed)
    {
        if (rotateWhileMoving)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 direction = pos - transform.position;
            float speedModifier = 1;
            if (smoothRotate)
            {
                Util.RotateToward(transform, pos, true, rotationSpeed);
                direction.y = 0;
                //Modify speed so we slow down when we are not facing the target
                //Dot between current direction and target direction,
                //this can slow down current speed when current direction differs much to target direction
                speedModifier = Vector3.Dot(forward, direction.normalized);
                speedModifier = Mathf.Clamp01(speedModifier);
            }
            else
            {
                Util.RotateToward(transform, pos, false, 0);
            }
            // Move the character (rateframe independent)
            direction = direction.normalized * movementspeed * speedModifier;
            controller.SimpleMove(direction);
        }
        else
        {
            Vector3 direction = pos - transform.position;
            direction.y = 0;
            direction.Normalize();
            direction = direction * movementspeed;
            //Note: do NOT apply Time.deltaTime when calling SimpleMove(),
            //because SimpleMove() already take deltaTime into account !
            controller.SimpleMove(direction);
        }
     }

    public static void ActivateRecurrsive(GameObject o)
    {
        o.active = true;
        foreach (Transform child in o.transform)
        {
            ActivateRecurrsive(child.gameObject);
        }
    }

    public static void DeactivateRecurrsive(GameObject o)
    {
        o.active = false;
        foreach (Transform child in o.transform)
        {
            DeactivateRecurrsive(child.gameObject);
        }
    }

    public static void AddAnimationEvent(AnimationClip clip, int triggerFrame, int totalFrame, float totalAnimationLength, string functionName, int intParameter)
    {
        AnimationEvent _event = new AnimationEvent();
        _event.functionName = functionName;
        _event.time = ((float)triggerFrame / (float)totalFrame) * totalAnimationLength;
        _event.intParameter = intParameter;
        clip.AddEvent(_event);
    }

    public static void AddAnimationEvent(AnimationClip clip, int triggerFrame, int totalFrame, float totalAnimationLength, string functionName, UnityEngine.Object ObjectParameter)
    {
        AnimationEvent _event = new AnimationEvent();
        _event.functionName = functionName;
        _event.time = ((float)triggerFrame / (float)totalFrame) * totalAnimationLength;
        _event.objectReferenceParameter = ObjectParameter;
        clip.AddEvent(_event);
    }

    /// <summary>
    /// Activate or Deactivate ragdoll of the characterObject
    /// available = true to activate ragdoll
    /// available = false to deactivate ragdoll
    /// </summary>
    /// <param name="characterObject"></param>
    /// <param name="available"></param>
    public static void SetRagdoll(GameObject characterObject, bool available)
    {
        CharacterJoint[] joints = characterObject.GetComponentsInChildren<CharacterJoint>();
        foreach (CharacterJoint joint in joints)
        {
            Rigidbody rigi = joint.GetComponent<Rigidbody>();
            Rigidbody parentRigi = joint.connectedBody;
            Collider collider = joint.GetComponent<Collider>();
            if (rigi != null)
            {
                rigi.isKinematic = !available;
                rigi.useGravity = available;
            }
            if (parentRigi != null)
            {
                parentRigi.isKinematic = !available;
                parentRigi.useGravity = available;
            }
            if (collider != null)
            {
                collider.enabled = available;
            }
        }
    }

    /// <summary>
    /// Execute a function with parameter %from% increasing to %to% in %duration& seconds.
    /// Do NOT PASS A LONG execution function in!
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="func"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static IEnumerator ExecFunctionWithGradualParameter(float from, float to, System.Action<float> func, float duration)
    {
        const float loop = 10;
        YieldInstruction span = duration == 0 ? null : new WaitForSeconds(duration / loop);
        float interval = (to - @from) / loop;
        if (to > @from)
        {
            for (float i = from; i < to; i += interval)
            {
                func(i);
                yield return span;
            }
        }
        else
        {
            for (float i = from; i > to; i += interval)
            {
                func(i);
                yield return span;
            }
        }
    }
}
