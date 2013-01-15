using UnityEngine;
using System.Collections;

public class BugRagdollReceiveDamage : ReceiveDamage{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override bool IsAlive()
    {
        return false;
    }

    public override IEnumerator DoDamage(DamageParameter damageParameter)
    {
		yield return null;
    }

}
