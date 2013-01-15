using UnityEngine;
using System.Collections;

public class BugHitVisualEffect : HitVisualEffect {
    
    public GameObject BulletHitEffect = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override GameObject GetBulletHitEffect()
    {
        return BulletHitEffect;
    }
}
