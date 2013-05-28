using UnityEngine;
using System.Collections;

public interface I_AIBehaviorHandler {

    void StartAI();

    void StopAI();

    void InitAI();

    void StartBehavior(AIBehavior behavior);

    void StopBehavior(AIBehavior behavior);
}
