using UnityEngine;
using System.Collections;
using System;

public class AIChainsawWorkerDie : MonoBehaviour {

    public string NormalDieAnimation = "receive_damage2";

    public GameObject Ragdoll_beheaded = null;

    public Transform[] ItemDroppedInDeath = null;

    void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {
        //Disable ragibody and character joint at awake
        Util.SetRagdoll(this.gameObject, false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Die(DamageForm form)
    {
        switch (form)
        {
            case DamageForm.Predator_Waving_Claw:
                Debug.Log("Die behead!");
                DieBehead();
                break;
            case DamageForm.Predator_Strike_Single_Claw:
            default:
                StartCoroutine(DieNormally());
                break;
        }
        return;
    }

    IEnumerator DieNormally()
    {
        if (NormalDieAnimation != string.Empty)
        {
            animation.Stop();
            animation.Rewind(NormalDieAnimation);
            animation.CrossFade(NormalDieAnimation);
            yield return new WaitForSeconds(animation[NormalDieAnimation].length);
            animation.Stop();
        }
        else
        {
            animation.Stop();
        }
        //Enable ragibody and character joint's collider
        Util.SetRagdoll(this.gameObject, true);
        this.transform.root.gameObject.layer = LayerMask.NameToLayer("Default");
        this.transform.root.gameObject.GetComponent<CharacterController>().enabled = false;
        DropItem();
        //In order to increse performance, wait shortly to re-disable the character joints.
        //yield return new WaitForSeconds(3.5f);
        //Util.SetCharacterJoints(this.gameObject, false);
    }

    void DieBehead()
    {
        GameObject ragdoll = (GameObject)GameObject.Instantiate(Ragdoll_beheaded, this.transform.position, this.transform.rotation);
        //Util.CopyTransform(transform, ragdoll.transform);
        Destroy(this.gameObject);
    }

    void DropItem()
    {
        foreach (Transform item in ItemDroppedInDeath)
        {
            Collider collider = item.GetComponent<Collider>();
            Rigidbody rigi = item.GetComponent<Rigidbody>();
            collider.enabled = true;
            rigi.isKinematic = false;
            rigi.useGravity = true;
            item.parent = null;
        }
    }
}
