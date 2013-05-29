using UnityEngine;
using System.Collections;

/// <summary>
/// PhysicsCharacter represents a special ragdoll. The nature of physics character, is that this character can be switched to physics mode.
/// For example, predator can physically falldown. After 3 seconds, physic character mode is turned off, and predator stands up in animation.
/// </summary>
public class PhysicsCharacter : Ragdoll {
	
	void Awake()
	{
	}
	
	public void ActivateRagdollJoint()
	{
		foreach (RagdollJointData JointData in RagdollJointData)
		{
			JointData.Joint.useGravity = true;
			JointData.Joint.isKinematic = false;
		}
	}
	
	public void DeactivateRagdollJoint()
	{
		foreach (RagdollJointData JointData in RagdollJointData)
		{
			JointData.Joint.useGravity = false;
			JointData.Joint.isKinematic = true;
		}
	}
	
	public virtual void StartRagdoll()
	{
		ActivateRagdollJoint();
		foreach (RagdollJointData JointData in RagdollJointData)
		{
            if (JointData.CreateForce == false)
                continue;
            else
            {
                StartCoroutine("AddForce", JointData);
            }
		}
	}
}
