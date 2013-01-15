using UnityEngine;
using System.Collections;

public class Init : MonoBehaviour {

    public

    void Awake()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("xenz"), LayerMask.NameToLayer("corpse"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("corpse"));

    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
