using UnityEngine;
using System.Collections;

public interface I_AIBehaviorHandler {

    /// <summary>
    /// AI birth function
    /// </summary>
    IEnumerator StartAI();

    IEnumerator StopAI();

    IEnumerator InitAI();

    /// <summary>
    /// Determine if the behavior can be executed.
    /// </summary>
    /// <returns></returns>
    bool IsConditionMatched(AIBehavior behavior,  AIBehaviorCondition Condition);

    void StartBehavior(AIBehavior behavior);

    void StopBehavior(AIBehavior behavior);
}
