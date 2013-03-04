using UnityEngine;
using System.Collections;

/// <summary>
/// Abstract pathfind class.
/// </summary>
public abstract class Navigator : MonoBehaviour {
    public abstract void StartNavigation(Transform target, bool IsMovingTarget, MoveData MoveData);
	public abstract void StopNavigation();
}
