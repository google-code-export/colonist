using UnityEngine;
using System.Collections;

public abstract class AbstractAI : MonoBehaviour, I_AIBehaviorHandler {
	
	public string Name = "DefaultName";
    /// <summary>
	/// Designer can put description of the AI at here.
	/// </summary>
	public string Description = "";
	
    public abstract void StartAI();

    public abstract void StopAI();

    public abstract void InitAI();

    public abstract void StartBehavior(AIBehavior behavior);

    public abstract void StopBehavior(AIBehavior behavior);
}
