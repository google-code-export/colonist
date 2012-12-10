using UnityEngine;
using System.Collections;

public abstract class HitVisualEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public abstract GameObject GetBulletHitEffect();
}
