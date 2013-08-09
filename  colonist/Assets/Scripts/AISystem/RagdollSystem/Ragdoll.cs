using UnityEngine;
using System.Collections;

/// <summary>
/// Define a joint data for a Ragdoll
/// </summary>
[System.Serializable]
public class RagdollJointData
{
	public string Name = "Default Joint";
	public ForceMode forceMode = ForceMode.Impulse;
	public Rigidbody Joint = null;
	public bool CreateForce = false;
	/// <summary>
	/// If Detach is true, the joint will be detach from parent.
	/// </summary>
	public bool Detach = false;
	/// <summary>
	/// If DestoryJoint is true, the CharacterJoint component will be destoryed.
	/// </summary>
	public bool DestoryJoint = false;
	/// <summary>
	/// Delay N seconds after start ragdoll
	/// </summary>
	public float CreateForceDelay = 0;
	/// <summary>
	/// Is the force at random direction?
	/// </summary>
	public bool ForceRandomDirection = false;
	/// <summary>
	/// IsGlobalDirection - is the direction in global or local system?
	/// </summary>
	public bool IsGlobalDirection = false;
	/// <summary>
	/// Used when ForceRandomDirection = false
	/// </summary>
	public Vector3 ForceDirection = Vector3.zero;
	/// <summary>
	/// Used when ForceRandomDirectionFrom = true
	/// </summary>
	public Vector3 ForceRandomDirectionFrom = Vector3.zero;
	/// <summary>
	/// Used when ForceRandomDirectionTo = true
	/// </summary>
	public Vector3 ForceRandomDirectionTo = Vector3.one;
	/// <summary>
	/// Final force = Random.range(MinForceMagnitude,MaxForceMagnitude)
	/// </summary>
	public float MinForceMagnitude = 1;
	public float MaxForceMagnitude = 2;
	/// <summary>
	/// If SelfcontrolDestruction = true, this joint will not be destroyed with the parent.
	/// The destruction time is defined by SelfDestructionTime.
	/// If SelfcontrolDestruction = true,  the Detach flag must be set true too, or else the 
	/// object will be destroy with parent.
	/// </summary>
	public bool SelfcontrolDestruction = false;
	public float SelfDestructionTime = 5;
	/// <summary>
	/// If JointGameObjectInitialEnable = false, the joint's gameobject should be deactivate when creating ragdoll.
	/// </summary>
	public bool JointGameObjectInitialActive = true;
	
	/// <summary>
	/// if RandomRotation = true, the join object's rotation will be changed to a random value.
	/// </summary>
	public bool RandomRotation = false;
	
	/// <summary>
	/// If this flag = true, the rigibody will be set IsKinmatic = false and UseGravity = true
	/// </summary>
	public bool TurnOnRigibody = false;
	

	/// <summary>
	/// Gets a clone of this RagdollJointData. The Joint object is null and need to be assigned other way.
	/// </summary>
	public RagdollJointData GetClone ()
	{		
		RagdollJointData ragdollJointData = new RagdollJointData ();
		ragdollJointData.Name = this.Name;
		ragdollJointData.CreateForce = this.CreateForce;
		ragdollJointData.Detach = this.Detach;
		ragdollJointData.DestoryJoint = this.DestoryJoint;
		ragdollJointData.CreateForceDelay = this.CreateForceDelay;
		ragdollJointData.ForceRandomDirection = this.ForceRandomDirection;
		ragdollJointData.IsGlobalDirection = this.IsGlobalDirection;
		ragdollJointData.ForceDirection = this.ForceDirection;
		ragdollJointData.ForceRandomDirectionFrom = this.ForceRandomDirectionFrom;
		ragdollJointData.ForceRandomDirectionTo = this.ForceRandomDirectionTo;
		ragdollJointData.MinForceMagnitude = this.MinForceMagnitude;
		ragdollJointData.MaxForceMagnitude = this.MaxForceMagnitude;
		ragdollJointData.SelfcontrolDestruction = this.SelfcontrolDestruction;
		ragdollJointData.SelfDestructionTime = this.SelfDestructionTime;
		ragdollJointData.JointGameObjectInitialActive = this.JointGameObjectInitialActive;
		ragdollJointData.TurnOnRigibody = this.TurnOnRigibody;
		return ragdollJointData;
	}
}

public class Ragdoll : MonoBehaviour
{
	
	public string Name = "";
	/// <summary>
	/// The create ragdoll delay.
	/// </summary>
	public float CreateRagdollDelay = 0;
	/// <summary>
	/// If AutoDestory = true, destory in %LifeTime% seconds
	/// </summary>
	public bool AutoDestory = true;
	/// <summary>
	/// If this flag = true, the gameObjet will be destroyed in StartRagdoll routine.
	/// </summary>
	public bool DestroyAfterStartRagdoll = false;
	public float LifeTime = 5;
	/// <summary>
	/// Assign the center for this ragdoll (because ragdoll has NO character controller.)
	/// </summary>
	public Transform RagdollCenter = null;
	public bool StartAtAwake = true;
	
	/// <summary>
	/// The effect data array.
	/// The EffectData will be played when the ragdoll start.
	/// </summary>
	public EffectData[] EffectData = new EffectData[] { };
	
	/// <summary>
	/// The decal data array.
	/// The DecalData will be played when the ragdoll start.
	/// </summary>
	public DecalData[] DecalData = new DecalData[] { };
	public RagdollJointData[] RagdollJointDataArray = new RagdollJointData[] { };
	
	public GameEvent[] StartRagdollEvents = new GameEvent[]{};
	
	void Awake ()
	{
	}

	void Start ()
	{
		if (StartAtAwake) {
			Invoke ("StartRagdoll", CreateRagdollDelay);
		}
		if (AutoDestory && DestroyAfterStartRagdoll == false) {
			Invoke ("DestoryRagdoll", LifeTime);
		}
	}
	
	void Update ()
	{

	}

	public virtual void StartRagdoll ()
	{	
		foreach (EffectData effectData in EffectData) {
			GlobalBloodEffectDecalSystem.CreateEffect (effectData);
		}
		foreach (DecalData decalData in DecalData) {
			GlobalBloodEffectDecalSystem.CreateBloodDecal (RagdollCenter.position, decalData);
		}
		foreach (RagdollJointData JointData in RagdollJointDataArray) {
			if (JointData.Detach) {
				JointData.Joint.transform.parent = null;
			}
			if (JointData.DestoryJoint) {
				CharacterJoint joint = JointData.Joint.GetComponent<CharacterJoint> ();
				Destroy (joint);
			}
			if (JointData.JointGameObjectInitialActive == false) {
				JointData.Joint.gameObject.SetActive(false);
			}
			if (JointData.RandomRotation) {
				JointData.Joint.gameObject.transform.rotation = Random.rotation;
			}
			if(JointData.TurnOnRigibody)
			{
				JointData.Joint.isKinematic = false;
				JointData.Joint.useGravity = true;
			}
			if (JointData.CreateForce == false)
				continue;
			else {
				StartCoroutine ("AddForce", JointData);
			}
		}
		if(DestroyAfterStartRagdoll == true && AutoDestory == true)
		{
			Invoke ("DestoryRagdoll", LifeTime);
		}
		
		foreach(GameEvent e in StartRagdollEvents)
		{
			LevelManager.OnGameEvent(e, this);
		}
	}

	public virtual IEnumerator AddForce (RagdollJointData JointData)
	{
		if (JointData.CreateForceDelay > 0) {
			yield return new WaitForSeconds(JointData.CreateForceDelay);
		}
		//Activate the game object (if not)
		if (JointData.Joint.gameObject.activeInHierarchy == false) {
			JointData.Joint.gameObject.SetActive(true);
		}
		Vector3 ForceDirection = JointData.ForceRandomDirection ?
            Util.RandomVector (JointData.ForceRandomDirectionFrom, JointData.ForceRandomDirectionTo) :
            JointData.ForceDirection;
		if (JointData.IsGlobalDirection == false) {
			ForceDirection = transform.TransformDirection (ForceDirection);
		}
		Vector3 Force = ForceDirection.normalized * Random.Range (JointData.MinForceMagnitude, JointData.MaxForceMagnitude);
		JointData.Joint.AddForce (Force, JointData.forceMode);
	}

	public virtual void DestoryRagdoll ()
	{
        
		for (int i=0; i<RagdollJointDataArray.Length; i++) {
			if (RagdollJointDataArray [i].Joint != null && RagdollJointDataArray [i].Joint.gameObject != null) {
				GameObject jointObject = RagdollJointDataArray [i].Joint.gameObject;
				if (RagdollJointDataArray [i].SelfcontrolDestruction) {
					RagdollJointDataArray [i].Joint.gameObject.AddComponent<AutoDestroy> ().destroyInTime = RagdollJointDataArray [i].SelfDestructionTime;
					//for self-control destruction joint object, automatically detach it from parent, to avoid being destroyed with parent.
					RagdollJointDataArray [i].Joint.gameObject.transform.parent = null;
				} else {
					Destroy (jointObject);
				}
			}
		}
		Destroy (gameObject);
	}
}
