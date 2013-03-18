using UnityEngine;
using System.Collections;

/// <summary>
/// Define a joint data for a Ragdoll
/// </summary>
[System.Serializable]
public class RagdollJointData
{
    public string Name = "Default Joint";
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
}

public class Ragdoll : MonoBehaviour {
    public string Name = "";
    /// <summary>
    /// If AutoDestory = true, destory in %LifeTime% seconds
    /// </summary>
    public bool AutoDestory = true;
    public float LifeTime = 5;
    /// <summary>
    /// If DelgateFunction != String.Empty, will SendMessage(DelgateFunction) instead of destory the game object.
    /// For example, you want to fade out the ragdoll by cliping its material, so you can put the 
    /// clip material function name in %DelgateFunction% .
    /// </summary>
    public string DelgateDestoryFunction = string.Empty;
    /// <summary>
    /// Assign the center for  this ragdoll (because ragdoll has NO character controller.)
    /// </summary>
    public Transform RagdollCenter = null;
    public EffectData[] EffectData = new EffectData[] { };
    public DecalData[] DecalData = new DecalData[] { };
    public RagdollJointData[] RagdollJointData = new RagdollJointData[] { };

    void Awake()
    {
    }

    void Start()
    {
        StartRagdoll();
        if (AutoDestory)
        {
            Invoke("DestoryRagdoll", LifeTime);
        }
    }

    public virtual void StartRagdoll()
    {
        foreach (EffectData effectData in EffectData)
        {
            GlobalBloodEffectDecalSystem.CreateEffect(RagdollCenter.position, effectData);
        }
        foreach (DecalData decalData in DecalData)
        {
            GlobalBloodEffectDecalSystem.CreateBloodDecal(RagdollCenter.position, decalData);
        }
        foreach (RagdollJointData JointData in RagdollJointData)
        {
            if (JointData.Detach)
            {
                JointData.Joint.transform.parent = null;
            }
            if (JointData.DestoryJoint)
            {
                CharacterJoint joint = JointData.Joint.GetComponent<CharacterJoint>();
                Destroy(joint);
            }
            if (JointData.CreateForce == false)
                continue;
            else
            {
                StartCoroutine("AddForce", JointData);
            }
        }
    }

    public virtual IEnumerator AddForce(RagdollJointData JointData)
    {
        if (JointData.CreateForceDelay > 0)
        {
            yield return new WaitForSeconds(JointData.CreateForceDelay);
        }
        Vector3 ForceDirection = JointData.ForceRandomDirection ?
            Util.RandomVector(JointData.ForceRandomDirectionFrom, JointData.ForceRandomDirectionTo) :
            JointData.ForceDirection;
        if (JointData.IsGlobalDirection == false)
        {
            ForceDirection = transform.TransformDirection(ForceDirection);
        }
        Vector3 Force = ForceDirection.normalized * Random.Range(JointData.MinForceMagnitude, JointData.MaxForceMagnitude);
        JointData.Joint.AddForce(Force, ForceMode.Impulse);
        
    }

    public virtual void DestoryRagdoll()
    {
        if (DelgateDestoryFunction != null && DelgateDestoryFunction.Trim() != string.Empty)
        {
            SendMessage(DelgateDestoryFunction);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
