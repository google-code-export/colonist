using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;
using System.Text;

public class SetupCharacterJoint : EditorWindow
{

    /// <summary>
    /// Add character joint to the selected game object
    /// </summary>
    [MenuItem("Component/Ragdoll/Setup character joint...")]
    static void SetupJoint()
    {
        GameObject selectedObject = Selection.activeGameObject;
        _SetupJoint(selectedObject, true);
    }

    /// <summary>
    /// Disable sub rigibodies (isKinematic = false)
    /// </summary>
    [MenuItem("Component/Ragdoll/Disable character joint and sub-rigibody...")]
    static void DisableJoints()
    {
        GameObject selectedObject = Selection.activeGameObject;
        foreach (Rigidbody rigi in selectedObject.GetComponentsInChildren<Rigidbody>())
        {
            rigi.isKinematic = true;
        }
    }

    /// <summary>
    /// Disable sub rigibodies (isKinematic = false)
    /// </summary>
    [MenuItem("Component/Ragdoll/Activate character joint and sub-rigibody...")]
    static void ActivateJoints()
    {
        GameObject selectedObject = Selection.activeGameObject;
        foreach (Rigidbody rigi in selectedObject.GetComponentsInChildren<Rigidbody>())
        {
            rigi.isKinematic = false;
            rigi.useGravity = true;
        }
    }

    private static void _SetupJoint(GameObject gameObject, bool isTop)
    {
        gameObject.AddComponent<Rigidbody>();
        //Add character joint for non-top node:
        if (isTop == false)
        {
            CharacterJoint joint = gameObject.AddComponent<CharacterJoint>();
            joint.connectedBody = gameObject.transform.parent.GetComponent<Rigidbody>();
        }
        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject.GetComponent<CharacterJoint>() == null)
            {
                child.gameObject.AddComponent<CharacterJoint>();
            }
            if (child.transform.parent.GetComponent<Rigidbody>() != null)
            {
                child.gameObject.GetComponent<CharacterJoint>().connectedBody = child.transform.parent.rigidbody;
            }
            _SetupJoint(child.gameObject, false);
        }
    }


    [MenuItem("Component/Ragdoll/Remove character joint and Rigibody...")]
    static void RemoveCharacterJointAndRigibody()
    {
        GameObject selectedObject = Selection.activeGameObject;
        Rigidbody[] rigis = selectedObject.GetComponentsInChildren<Rigidbody>();
        CharacterJoint[] joints = selectedObject.GetComponentsInChildren<CharacterJoint>();
        foreach (CharacterJoint joint in joints)
        {
            DestroyImmediate(joint);
        }
        foreach (Rigidbody rigi in rigis)
        {
            DestroyImmediate(rigi);
        }
        foreach (Collider collider in selectedObject.GetComponentsInChildren<Collider>())
        {
            DestroyImmediate(collider);
        }
    }

    
}
