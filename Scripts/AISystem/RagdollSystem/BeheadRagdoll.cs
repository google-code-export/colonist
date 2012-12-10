using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BeheadRagdoll : MonoBehaviour {

    public string[] DieAnimations = new string[] { "", "Twitch_behead", "Twitch_behead2" };

    public Rigidbody Head = null;
    public Transform BeheadPivot = null;

    public ParticleSystem[] BeheadBloodSplatter = null;
    public Transform LeftThigh = null;
    public Vector3 RandomForceOfLeftThighAxis;
    public Transform RightThigh = null;
    public Vector3 RandomForceOfRightThighAxis;
    public float ThighMaxSwingLimited = 0.3f;
    public float ThighMinSwingLimited = -0.3f;

    public Transform root = null;
    public Transform head = null;

    public float MinForceX = 3;
    public float MaxForceX = 5;
    public float MinForceY = 3;
    public float MaxForceY = 5;
    public float MinForceZ = 3;
    public float MaxForceZ = 5;

    public bool SlowMotionRagdoll = true;

    private IList<Rigidbody> rigiBodys = new List<Rigidbody>();
    private IList<CharacterJoint> characterJoints = new List<CharacterJoint>();

    public Rigidbody[] ItemDroppedInDeath = null;

    void Awake()
    {
        foreach (string DieAnimation in DieAnimations)
        {
            if (DieAnimation != "")
            {
                animation[DieAnimation].AddMixingTransform(root);
                animation[DieAnimation].AddMixingTransform(head);
                animation[DieAnimation].RemoveMixingTransform(head);
            }
        }
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(StartRagdoll());
	}
	
	// Update is called once per frame
	void Update () {
	}

    IEnumerator StartRagdoll()
    {
        foreach (ParticleSystem ps in BeheadBloodSplatter)
        {
            ps.enableEmission = true;
            ps.Play();
        }

        Util.SetRagdoll(this.gameObject, false);

        Random.seed = System.DateTime.Now.Millisecond;
        Vector3 backward = transform.TransformDirection(Vector3.back);
        //rigi.AddForceAtPosition(force, BeheadPivot.position, ForceMode.Impulse);
        //detach the head - Wooow ! The head is flying !
        head.parent = null;
        Destroy(Head.GetComponent<CharacterJoint>());
        RandomPush();
        Invoke("AddForce", 0.05f);
        DropItem();
        string DieAnimation = Util.RandomFromArray(DieAnimations);
        if (DieAnimation != string.Empty)
        {
            animation.CrossFade(DieAnimation);
            yield return new WaitForSeconds(animation[DieAnimation].length);
        }
        Util.SetRagdoll(this.gameObject, true);

    }

    void AddForce()
    {
        Vector3 force = new Vector3(Random.Range(MinForceX, MaxForceX),
            Random.Range(MinForceY, MaxForceY),
            Random.Range(MinForceZ, MaxForceZ));
        Head.useGravity = true;
        Head.isKinematic = false;
        Head.collider.enabled = true;
        Head.AddForceAtPosition(force, BeheadPivot.position, ForceMode.Impulse);
    }

    void DisablePhysics()
    {
        foreach (CharacterJoint joint in characterJoints)
        {
            Object.Destroy(joint);
        }
        foreach (Rigidbody r in rigiBodys)
        {
            Object.Destroy(r);
        }
    }

    void RandomPush()
    {
        Random.seed = System.DateTime.Now.Millisecond;
        LeftThigh.transform.RotateAroundLocal(RandomForceOfLeftThighAxis, Random.Range(ThighMinSwingLimited, ThighMaxSwingLimited));
        RightThigh.transform.RotateAroundLocal(RandomForceOfRightThighAxis, Random.Range(ThighMinSwingLimited, ThighMaxSwingLimited));
    }

    void DropItem()
    {
        foreach (Rigidbody item in ItemDroppedInDeath)
        {
            Collider collider = item.GetComponent<Collider>();
            Rigidbody rigi = item.GetComponent<Rigidbody>();
            collider.enabled = true;
            rigi.isKinematic = false;
            rigi.useGravity = true;
            item.transform.parent = null;
        }
    }

}
