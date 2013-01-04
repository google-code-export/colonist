using UnityEngine;
using System.Collections;

public abstract class UnitHealth :MonoBehaviour {
    public abstract void SetCurrentHP(float value);
    public abstract void SetMaxHP(float value);
    public abstract float GetCurrentHP();
    public abstract float GetMaxHP();
    public abstract IEnumerator ApplyDamage(DamageParameter damageParam);
    public abstract IEnumerator Die(DamageParameter damageParam);
}
