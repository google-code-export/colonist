using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A basic Monobehavior to control how a character receives damage :
///  - Managing HP
///  - Die replaced by ragdoll, external or activating self-ragdoll.
/// </summary>
public class ReceiveDamage : MonoBehaviour {

    public float HP = 100f;
    public float MaxHP = 100f;

    /// <summary>
    /// if true, will Instantiate external ragdoll object and copy transform
    /// else if false, will activate the rigibody & joints of current character - ActivateSelfRagdoll
    /// </summary>
    public bool useExternalRagdoll = false;

    /// <summary>
    /// The ExternalRagdoll is instantiated if useExternalRagdoll = true
    /// </summary>
    public GameObject ExternalRagdoll = null;

    public IList<Rigidbody> AllRigis = new List<Rigidbody>();

    public IList<CharacterJoint> AllJoints = new List<CharacterJoint>();

    /// <summary>
    /// Only available when useExternalRagdoll = true
    /// 
    /// If CopyFullTransform = true & useExternalRagdoll = true, then when replacing ragdoll to current
    /// transform, every child transform's position and rotation will be copied to the children of replaced ragdoll
    /// Else if CopyFullTransform = false & useExternalRagdoll = true, will copy only the transform path specified by
    /// CopiedTransformChildrenPath
    /// </summary>
    public bool CopyFullTransform = false;
    public string[] CopiedTransformChildrenPath = null;

    void Awake()
    {
        Debug.Log("In base awake");
        if (useExternalRagdoll == false)
        {
            foreach (Rigidbody rigi in this.GetComponentsInChildren<Rigidbody>())
            {
                AllRigis.Add(rigi);
            }
            foreach (CharacterJoint joint in this.GetComponentsInChildren<CharacterJoint>())
            {
                AllJoints.Add(joint);
            }
        }
    }

	// Use this for initialization
	void Start () {
	  
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// - Instantiate ragdoll prefab
    /// - Copy transfrom (full copy or limited copy)
    /// - Destory current gameobject
    /// </summary>
    public virtual void ReplaceRagdoll()
    {
        GameObject externalRagdoll = (GameObject)GameObject.Instantiate(ExternalRagdoll, this.transform.position, this.transform.rotation);
        //CopyFullTransform or parially copy
        if (CopyFullTransform == false && CopiedTransformChildrenPath != null && CopiedTransformChildrenPath.Length > 0)
        {
            foreach (string path in CopiedTransformChildrenPath)
            {
                Transform _src = this.transform.Find(path);
                Transform _dst = externalRagdoll.transform.Find(path);
                if (_dst != null && _src != null)
                {
                    _dst.position = _src.position;
                    _dst.rotation = _src.rotation;
                }
            }
        }
        else
        {
            Util.CopyTransform(this.transform, externalRagdoll.transform);
        }
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Treat self as a ragdoll, deactivate every children rigibody
    /// </summary>
    public virtual void DeactivateSelfRagdoll()
    {
        foreach (Rigidbody rigi in AllRigis)
        {
            rigi.isKinematic = true;
            rigi.useGravity = false;
        }
    }

    /// <summary>
    /// Treat self as a ragdoll, activate every children rigibody
    /// </summary>
    public virtual void ActivateSelfRagdoll()
    {
        foreach (Rigidbody rigi in AllRigis)
        {
            rigi.isKinematic = false;
            rigi.useGravity = true;
        }
    }

    
    public virtual bool IsAlive()
    {
        return HP > 0;
    }

    public virtual IEnumerator DoDamage(DamageParameter damageParameter)
    {
        HP -= damageParameter.damagePoint;
        if (HP <= 0)
        {
            if (useExternalRagdoll)
            {
                ReplaceRagdoll();
            }
            else
            {
                ActivateSelfRagdoll();
            }
            SendMessage("Die", null);
        }
        yield return null;
    }

    public virtual IEnumerator Die(object parameter)
    {
        Destroy(this.gameObject);
        yield return null;
    }
}
