using UnityEngine;
using System.Collections;

/// <summary>
/// Base AI.
/// </summary>
public class AI : MonoBehaviour {
    /// <summary>
    /// The flag to halt an AI.
    /// true = Halt
    /// false = No halt
    /// </summary>
    [HideInInspector]
    public bool Halt = false;

    [HideInInspector]
    public Transform currentTarget;

    /// <summary>
    /// offspring should call InitAI() at Awake() 
    /// </summary>
    public void InitAI()
    {
        LevelManager.RegisterAI(this);
    }

    /// <summary>
    /// offspring should call StopAI() at offspring.StopAI()
    /// </summary>
    public virtual void StopAI()
    {
        LevelManager.UnregisterAI(this);
    }

    public void ResetTarget()
    {
        currentTarget = null;
    }

    public void ResetTarget(int layer)
    {
        if(currentTarget != null && currentTarget.gameObject.layer == layer)
           currentTarget = null;
    }

}
