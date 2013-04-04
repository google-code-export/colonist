using UnityEngine;
using System.Collections;

public class Predator3rdPersonalApplyDamage : MonoBehaviour {
    
    public Predator3rdPersonalUnit predatorPlayerUnit = null;   
    public PrograssBar HealthPrograss = null;
    public ParticleSystem electricityHitEffect = null;

    void Awake()
    {
       predatorPlayerUnit = GetComponent<Predator3rdPersonalUnit>();
    }
	// Use this for initialization
	void Start () {
	     
	}
	
	// Update is called once per frame
	void Update () {
        //HealthPrograss.Value = HP / MaxHP;
	}

    public virtual IEnumerator ApplyDamage(DamageParameter param)
    {
        predatorPlayerUnit.HP -= param.damagePoint;
		
        Debug.Log("ApplyDamage at PredatorPlayer, current HP:" + predatorPlayerUnit.HP);
        switch (param.damageForm)
        {
            case DamageForm.ElectricityBoltHit:
                electricityHitEffect.Play();
                break;
        }
//        if (HP <= 0)
//        {
//            StartCoroutine("Die");
//        }
        //if (cameraController.LookAtTarget == null)
        //{
        //    cameraController.LookAtTarget = param.src.transform;
        //}
        yield return null;
    }


    public virtual IEnumerator Die(DamageParameter param)
    {
        Destroy(this.transform.root.gameObject);
        yield return null;
    }


}
