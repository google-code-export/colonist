using UnityEngine;
using System.Collections;

/// <summary>
/// Force pivot represents a rigidbody wraper, that can be applied force by PhysicsCharacter.
/// </summary>
[System.Serializable]
public class ForcePivot
{
	public Rigidbody PivotRigid = null;
	public ForceMode forceMode;
	public Vector3 DirectionOffset_Min = Vector3.zero;
	public Vector3 DirectionOffset_Max = Vector3.zero;
}

/// <summary>
/// PhysicsCharacter represents a special ragdoll. The nature of physics character, is that this character can be switched to physical control, and switch back to animation control later.
/// For example, predator can physically falldown. After 3 seconds, physic character mode is turned off, and predator stands up in animation.
/// </summary>
public class PhysicsCharacter : MonoBehaviour, I_GameEventReceiver {
	
	Rigidbody[] rigids = new Rigidbody[]{};
	public Transform centerOfMass = null;
	public ForcePivot[] ForcePivotArray = null;
	public float RecoverAfterNSeconds = 3;
	public string RecoverAnimation = "";
	
	void Awake()
	{
		rigids = GetComponentsInChildren<Rigidbody>();
	}
	
	void Start()
	{
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.T))
		{
			foreach(MonoBehaviour mono in this.GetComponents<MonoBehaviour>())
			{
				if(mono != this)
				{
					mono.enabled = false;
				}
			}
			animation.Stop();
			ApplyForceToPhysicsCharacter(transform.TransformDirection(Vector3.back), 50);
		}
	}
	
	public void ActivateRagdollJoint()
	{
		foreach (Rigidbody rigi in rigids)
		{
			rigi.useGravity = true;
			rigi.isKinematic = false;
		}
	}
	
	public void DeactivateRagdollJoint()
	{
		foreach (Rigidbody rigi in rigids)
		{
			rigi.useGravity = false;
			rigi.isKinematic = true;
		}
	}
	
	public void OnGameEvent(GameEvent e)
	{
		switch(e.type)
		{
		case GameEventType.ForceToPhysicsCharacter:
			ApplyForceToPhysicsCharacter(e.Vector3Parameter, e.FloatParameter);
			break;
		}
	}
	
	public void ApplyForceToPhysicsCharacter(Vector3 direction, float ForceMagnitude)
	{
		ActivateRagdollJoint();
		foreach (ForcePivot forcePivot in ForcePivotArray)
		{
            Vector3 ForceDirection = direction + Util.RandomVector(forcePivot.DirectionOffset_Min, forcePivot.DirectionOffset_Max);
			Vector3 Force = ForceDirection * ForceMagnitude;
			forcePivot.PivotRigid.AddForce(Force,forcePivot.forceMode);
		}
		Invoke("Recover", RecoverAfterNSeconds);
	}
	
	void Recover()
	{
		DeactivateRagdollJoint();
	    Util.AlignParentToChild(centerOfMass.parent, centerOfMass);
	    animation.Play(RecoverAnimation);
	}
}
