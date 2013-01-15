using UnityEngine;
using System.Collections;

public class CutRagdoll : MonoBehaviour {

    public Transform UpperBody;

	// Use this for initialization
	void Awake () {
	
	}

    void Start()
    {
        SendMessage("StartRagdoll");
    }

    IEnumerator StartRagdoll()
    {
        Util.SetRagdoll(this.gameObject, false);
        yield return null;
    }
}
