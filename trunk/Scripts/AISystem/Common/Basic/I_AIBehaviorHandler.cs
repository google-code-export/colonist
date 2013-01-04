using UnityEngine;
using System.Collections;

public interface I_AIBehaviorHandler {

    /// <summary>
    /// ALter current behavior at every %ScanBehaviorInterval% seconds
    /// </summary>
    IEnumerator AlterBehavior(float ScanBehaviorInterval);

    /// <summary>
    /// Determine if the behavior can be executed.
    /// </summary>
    /// <returns></returns>
    bool IsConditionMatched(AIBehavior behavior,  AIBehaviorCondition Condition);

    void StartBehavior(AIBehavior behavior);

    void StopBehavior(AIBehavior behavior);
}
