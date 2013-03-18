using UnityEngine;
using System.Collections;

public interface I_AIBehaviorHandler {

    /// <summary>
    /// AI birth function
    /// </summary>
    IEnumerator StartAI();

    void StopAI();

    void InitAI();

    void StartBehavior(AIBehavior behavior);

    void StopBehavior(AIBehavior behavior);
}
