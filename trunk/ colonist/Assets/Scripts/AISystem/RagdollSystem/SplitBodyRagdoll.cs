using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplitBodyRagdoll : MonoBehaviour
{
	//	public string[] DieAnimations = new string[] { "", "Twitch_behead", "Twitch_behead2" };
	//	public Rigidbody Head = null;
	public Rigidbody Spine1 = null;
	public Transform BreakPivot = null;
	public ParticleSystem[] BeheadBloodSplatter = null;
	public Transform LeftThigh = null;
	public Vector3 RandomForceOfLeftThighAxis;
	public Transform RightThigh = null;
	public Vector3 RandomForceOfRightThighAxis;
	public float ThighMaxSwingLimited = 0.3f;
	public float ThighMinSwingLimited = -0.3f;
	public Transform root = null;
	public Transform spine1 = null;
	public float MinForceX = 3;
	public float MaxForceX = 5;
	public float MinForceY = 3;
	public float MaxForceY = 5;
	public float MinForceZ = 3;
	public float MaxForceZ = 5;
	public bool SlowMotionRagdoll = true;
	private IList<Rigidbody> rigiBodys = new List<Rigidbody> ();
	private IList<CharacterJoint> characterJoints = new List<CharacterJoint> ();
	public Rigidbody[] ItemDroppedInDeath = null;
	public Vector3 BreakBackOfSpine1;
    public EffectData[] EffectData = new EffectData[] { };
	void Awake ()
	{
	}

	// Use this for initialization
	void Start ()
	{
		StartRagdoll ();
        foreach (EffectData effect in EffectData)
        {
			GlobalBloodEffectDecalSystem.CreateEffect(root.position, effect);
        }
	}

	// Update is called once per frame
	void Update ()
	{
	}

	void StartRagdoll ()
	{
		Debug.Log ("CALLER2:" + this.gameObject.name);
		foreach (ParticleSystem ps in BeheadBloodSplatter) {
			ps.enableEmission = true;
			ps.Play ();
		}

		Util.SetRagdoll (this.gameObject, false);

		Random.seed = System.DateTime.Now.Millisecond;
		Vector3 backward = transform.TransformDirection (Vector3.back);
		//rigi.AddForceAtPosition(force, BeheadPivot.position, ForceMode.Impulse);
		//detach the head - Wooow ! The head is flying !
		if (Spine1 != null) {
			Destroy (Spine1.GetComponent<CharacterJoint> ());
		}
 
		RandomPush ();
		SendMessage ("AddForce", 0.05f);
    
		DropItem ();
		//		string DieAnimation = Util.RandomFromArray (DieAnimations);
		//		if (DieAnimation != string.Empty) {
		//			animation.CrossFade (DieAnimation);
		//			yield return new WaitForSeconds(animation[DieAnimation].length);
		//		}
		Util.SetRagdoll (this.gameObject, true);
		spine1.parent = null;
	}

	IEnumerator AddForce (float d_time)
	{
		yield return new WaitForSeconds(d_time);
//		Vector3 force = new Vector3 (Random.Range (MinForceX, MaxForceX),
//            Random.Range (MinForceY, MaxForceY),
//            Random.Range (MinForceZ, MaxForceZ));
		Spine1.useGravity = true;
		Spine1.isKinematic = false;
		Spine1.collider.enabled = true;
//		Spine1.AddForceAtPosition (force, BreakPivot.position, ForceMode.Impulse);
		
//		yield return new WaitForSeconds(0.1f);
		Random.seed = System.DateTime.Now.Millisecond;
		BreakBackOfSpine1 = BreakBackOfSpine1.normalized;
		BreakBackOfSpine1 += new Vector3 (Random.Range (MinForceX, MaxForceX),
            Random.Range (MinForceY, MaxForceY),
            Random.Range (MinForceZ, MaxForceZ));
		
		Spine1.AddForceAtPosition (BreakBackOfSpine1, Spine1.position, ForceMode.Impulse);
	}

	void DisablePhysics ()
	{
		foreach (CharacterJoint joint in characterJoints) {
			Object.Destroy (joint);
		}
		foreach (Rigidbody r in rigiBodys) {
			Object.Destroy (r);
		}
	}

	void RandomPush ()
	{
		Random.seed = System.DateTime.Now.Millisecond;
		if (LeftThigh != null) {
			LeftThigh.transform.RotateAroundLocal (RandomForceOfLeftThighAxis, Random.Range (ThighMinSwingLimited, ThighMaxSwingLimited));
		}
		if (RightThigh != null) {
			RightThigh.transform.RotateAroundLocal (RandomForceOfRightThighAxis, Random.Range (ThighMinSwingLimited, ThighMaxSwingLimited));
		}
	}

	void DropItem ()
	{
		foreach (Rigidbody item in ItemDroppedInDeath) {
			Collider collider = item.GetComponent<Collider> ();
			Rigidbody rigi = item.GetComponent<Rigidbody> ();
			collider.enabled = true;
			rigi.isKinematic = false;
			rigi.useGravity = true;
			item.transform.parent = null;
		}
	}
}
