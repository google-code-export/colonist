using UnityEngine;
using System.Collections;

public class AIBugAttack : MonoBehaviour {
    /// <summary>
    /// Dont set value directly to this property, use SetTarget() instead.
    /// </summary>
    public ReceiveDamage currentTarget;

    public float meleeAttackRange = 8f;
    public bool attack = false;
    public float meleeAP = 20f;
    /// <summary>
    /// Attack speed in second
    /// </summary>
    public float attackSpeed = 1.05f;

    private float lastDoDamageTime;
    private AIBugMovement bugMovement = null;

	// Use this for initialization
	void Start () {
        bugMovement = this.GetComponent<AIBugMovement>();
	}
    bool istargetalive;
	// Update is called once per frame
	void Update () {
        if (attack && currentTarget != null && currentTarget.IsAlive())
        {
            Attack(currentTarget);
        }
	}

    void Attack(ReceiveDamage target)
    {
        animation.CrossFade("attack");
        this.bugMovement.SmoothRotateTowards(target.transform.position);
        if ((Time.time - lastDoDamageTime) >= attackSpeed)
        {
            lastDoDamageTime = Time.time;
            //target.DoDamage(new DamageParameter(meleeAP));
            //Debug.Log("Attack:" + Time.time);
        }
    }

    public void SetTarget(ReceiveDamage target)
    {
        currentTarget = target;
    }

    public bool IsTargetInMeleeRange(ReceiveDamage target)
    {
        float acceptMeleeRange = meleeAttackRange;
        float distance = Mathf.Abs((target.transform.position - this.transform.position).magnitude);
        if ( (this.meleeAttackRange + acceptMeleeRange) < distance )
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, this.meleeAttackRange);
    }
}
