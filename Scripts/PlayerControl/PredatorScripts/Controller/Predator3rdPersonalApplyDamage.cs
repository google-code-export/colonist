using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GetHP))]
public class Predator3rdPersonalApplyDamage : MonoBehaviour {

    public float HP
    {
        get
        {
            return GetComponent<GetHP>().HP;
        }
        set
        {
            GetComponent<GetHP>().HP = value;
        }
    }
    public float MaxHP = 100;
    public PrograssBar HealthPrograss = null;
 
    public ParticleSystem electricityHitEffect = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        HealthPrograss.Value = HP / MaxHP;
	}

    public float GetHealth()
    {
        return HP;
    }

    public IEnumerator ApplyDamage(DamageParameter param)
    {
        HP -= param.damagePoint;
        switch (param.damageForm)
        {
            case DamageForm.ElectricityBoltHit:
                electricityHitEffect.Play();
                break;
        }
        if (HP <= 0)
        {
            StartCoroutine("Die");
        }
        //if (cameraController.LookAtTarget == null)
        //{
        //    cameraController.LookAtTarget = param.src.transform;
        //}
        yield return null;
    }


    public IEnumerator Die()
    {
        Destroy(this.transform.root.gameObject);
        yield return null;
    }
    
}
