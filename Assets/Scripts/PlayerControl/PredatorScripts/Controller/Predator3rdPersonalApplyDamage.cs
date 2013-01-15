using UnityEngine;
using System.Collections;

public class Predator3rdPersonalApplyDamage : UnitHealth {
    [HideInInspector]
    public float HP;
    
    public float MaxHP = 100;
    public PrograssBar HealthPrograss = null;
    public ParticleSystem electricityHitEffect = null;

    void Awake()
    {
        HP = MaxHP;
    }
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

    public virtual IEnumerator ApplyDamage(DamageParameter param)
    {
        HP -= param.damagePoint;
        //Debug.Log("Predator HP:" + HP);
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


    public virtual IEnumerator Die(DamageParameter param)
    {
        Destroy(this.transform.root.gameObject);
        yield return null;
    }

    #region implement UnitHealth interface
    public override void SetCurrentHP(float value)
    {
        HP = value;
    }
    public override void SetMaxHP(float value)
    {
        MaxHP = value;
    }
    public override float GetCurrentHP()
    {
        return HP;
    }
    public override float GetMaxHP()
    {
        return MaxHP;
    }
    #endregion
}
