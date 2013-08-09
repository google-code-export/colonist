using UnityEngine;
using System.Collections;

/// <summary>
/// Physics unit represents a unit that has physical feature.
/// </summary>
public class PhysicalUnit : Unit {
	/// <summary>
	/// The split ragdoll replacement.
	/// </summary>
	public GameObject SplitRagdollReplacement = null;
	/// <summary>
	/// The chest.
	/// </summary>
	public Rigidbody Chest;
	
	bool ShouldAttachChest = false;
	Transform ChestAnchor = null;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	   if(ShouldAttachChest)
	   {
		  Util.AlignChildTransformPosition(this.transform, Chest.transform, ChestAnchor.position);
	   }
	}
	
	/// <summary>
	/// Replaces to split ragdoll.
	/// </summary>
	public GameObject ReplaceToSplitRagdoll(Transform Pivot)
	{
		GameObject replacement = Object.Instantiate(SplitRagdollReplacement) as GameObject;
		Object.Destroy(this.gameObject);
		return replacement;
	}
	
	/// <summary>
	/// Physicals the attach chest to transform.
	/// </summary>
	public void PhysicalAttachChestToTransform(Transform Anchor, GameObject attacheeObject)
	{
		ChestAnchor = Anchor;
		Util.AlignChildTransformPosition(this.transform, Chest.transform, ChestAnchor.position);
		ShouldAttachChest = true;
		transform.LookAt(attacheeObject.transform);
	}
	
	/// <summary>
	/// Physicals the attach chest to transform.
	/// </summary>
	public void PhysicalDetachChest()
	{
		ShouldAttachChest = false;
	}
}
