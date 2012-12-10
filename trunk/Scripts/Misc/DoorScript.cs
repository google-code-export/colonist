using UnityEngine;
using System.Collections;

[RequireComponent(typeof (BoxCollider))]
public class DoorScript : MonoBehaviour {

    private BoxCollider Detector = null;
    public Collider EntranceCollider;

    public LayerMask triggerLayer = 0;
    public string OpenDoorAnimation = "openDoor";
    public string CloseDoorAnimation = "closeDoor";

    private Stack ObjectInside = new Stack();

    void Awake()
    {
        Detector = this.GetComponent<BoxCollider>();
        Detector.isTrigger = true;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	   
	}

    public void OpenDoor()
    {
        this.animation.Play(OpenDoorAnimation);
        EntranceCollider.isTrigger = true;
    }

    public void CloseDoor()
    {
        this.animation.Play(CloseDoorAnimation);
        EntranceCollider.isTrigger = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (Util.CheckLayerWithMask(other.gameObject.layer, triggerLayer.value) == false)
        {
            return;
        }
        if (ObjectInside.Count == 0)
        {
            OpenDoor();
        }
        ObjectInside.Push(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (Util.CheckLayerWithMask(other.gameObject.layer, triggerLayer.value) == false)
        {
            return;
        }
        ObjectInside.Pop();
        if (ObjectInside.Count == 0)
        {
             CloseDoor();
        }
    }
}
