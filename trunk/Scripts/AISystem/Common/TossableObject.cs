using UnityEngine;
using System.Collections;

public class TossableObject : MonoBehaviour {

    /// <summary>
    /// At which rigibody should the inital toss force put on
    /// </summary>
    public Rigidbody TosseeTransformAnchor;

    public string ChangeToLayerAfterTossing = "impactor";

    public MonoBehaviour[] StopMonoWhenTossed = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void BeingToss(TossParameter tP)
    {
        if (ChangeToLayerAfterTossing != string.Empty)
        {
            foreach (Rigidbody rigi in this.GetComponentsInChildren<Rigidbody>())
            {
                rigi.gameObject.layer = LayerMask.NameToLayer(ChangeToLayerAfterTossing);
            }
        }
        TosseeTransformAnchor.AddForce(tP.forceDirection * tP.Force, ForceMode.Impulse);
        foreach (MonoBehaviour mono in StopMonoWhenTossed)
        {
            Destroy(mono);
        }
    }
}
